using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace RVT_AutomateClash
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("64E3DE6F-4F9F-4942-9EAC-02B2D54F6222"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Application app = uiapp.Application;
            RevitTools.Doc = doc;
            RevitTools.App = app;
            RevitTools.Uidoc = uidoc;



            if (doc.ActiveView.ViewType != ViewType.ThreeD)
            {
                TaskDialog.Show("Error", "It only works on a 3D view");
                return Result.Failed;
            }
            var testView = doc.ActiveView as View3D;
            if (!testView.IsSectionBoxActive)
            {
                TaskDialog.Show("Error", "It only works if the 3D view has a section box active");
                return Result.Failed;
            }

            //Debug
           
           var link =  Clash.Documents(doc, app);
            //Debug
            MainFormTools.elementsClashing = Clash.clashingElements(doc, app, link.First().Value);


            using (Transaction t = new Transaction(doc, "Clash"))
            {
                t.Start();

                RevitTools.OverrideInView(MainFormTools.elementsClashing, doc);

                t.Commit();

            }
            ResultForm result = new ResultForm();

                result.Show();

                return Result.Succeeded;
        }

    }

    public class MainFormTools

    {
        public static List<Element> elementsClashing { get; set; }

        public static SortedList<String, Document> Documents(Document doc, Application app)
        {
            SortedList<string, Document> output = new SortedList<string, Document>();

            foreach (Document d in app.Documents)
            {
                if (d.IsLinked)
                {
                    output.Add(d.Title, d);
                }
            }
            return output;
        }

        public static Document linkedDocument { get; set; }

        public static List<ElementFilter> SelectedCategories { get; set; }


        public static List<ElementFilter> SelectedHostCategories { get; set; }



        public static SortedList<string, Category> ListOfCategories(Document doc)
        {
            //https://forums.autodesk.com/t5/revit-api-forum/how-to-get-category-list/td-p/4477699

            Categories categories = doc.Settings.Categories;

            SortedList<string, Category> myCategories = new SortedList<string, Category>();

            foreach (Category c in categories)

            {

                if (c.AllowsBoundParameters)

                    myCategories.Add(c.Name, c);

            }
            return myCategories;
        }

    }
    public class RevitTools
    {

        public static void Focus(int id)
        {
            ElementId Id = new ElementId(id);
            Element e = Doc.GetElement(Id);
            Uidoc.ShowElements(e);

        }

        public static Document Doc { get; set; }
        public static Application App { get; set; }
        public static UIDocument Uidoc { get; internal set; }

        /// <summary>
        /// Gets the transformation applied to a linked file
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="linked"></param>
        /// <returns></returns>
        public static Transform GetTransform(Document doc, Document linked)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance));
            Transform output = null;
            foreach (var item in collector)
            {
                var rvtlink = item as RevitLinkInstance;
                if (rvtlink.Name.Contains(linked.Title))
                {

                    output = rvtlink.GetTransform();
                }

            }

            return output;
        }

        public static bool belongsToView(View3D view3D, BoundingBoxXYZ bb)
        {
            var SectionBox = view3D.GetSectionBox();
            var vMax = SectionBox.Max + SectionBox.Transform.Origin;
            var vMin = SectionBox.Min + SectionBox.Transform.Origin;
            var bbMax = bb.Max;
            var bbMin = bb.Min;

            if (bbMin.X < vMax.X && bbMax.X > vMin.X)
            {
                if (bbMin.Y < vMax.Y && bbMax.Y > vMin.Y)
                    if (bbMin.Z < vMax.Z && bbMax.Z > vMin.Z)
                    {
                        return true;
                    }
            }
            return false;
        }
        /// <summary>
        /// Clashing elements visible in view against a linked file.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static List<Element> clashingElements(Document doc, Application app)
        {
            //TODO : Create a separate addin to save configuration

            var linkedElements = new List<Element>();
            var localElements = new List<Element>();
            var ClashingElements = new List<Element>();
            var d = MainFormTools.linkedDocument;

            //Get the transformation of the linked model from the project location
            var transform = GetTransform(doc, d);

            Autodesk.Revit.DB.Options opt = new Options();
            var ActiveViewBB = doc.ActiveView as View3D;

            BoundingBoxXYZ bbV = ActiveViewBB.GetSectionBox();
            //Get all linked walls in view
            if (MainFormTools.SelectedCategories != null)
            {
                var VisibleLinkedElements = new List<GeometryElement>();

                LogicalOrFilter filterLinkedCategories = new LogicalOrFilter(MainFormTools.SelectedCategories);
                FilteredElementCollector walls = new FilteredElementCollector(d).WherePasses(filterLinkedCategories).WhereElementIsNotElementType();
                linkedElements = walls.ToElements() as List<Element>;

                foreach (var item in linkedElements)
                {

                    GeometryElement geom = item.get_Geometry(opt);
                    ////Get bounding box from transformed geometry
                    var geomTranslated = geom.GetTransformed(transform);
                    var bbox = geomTranslated.GetBoundingBox();
                    if (belongsToView(ActiveViewBB, bbox))
                    {
                        //VisibleLinkedElements.Add(geomTranslated);
                        Outline outline = new Outline(bbox.Min, bbox.Max);
                        BoundingBoxIntersectsFilter bbFilter = new BoundingBoxIntersectsFilter(outline);

                        LogicalOrFilter logicalOrFilter = new LogicalOrFilter(MainFormTools.SelectedHostCategories);

                        FilteredElementCollector bbClashingCollector = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(logicalOrFilter).WherePasses(bbFilter);
                        foreach (var element in bbClashingCollector.ToElements())
                        {
                            if (!ClashingElements.Contains(element))
                            {
                                if (getClashWithSolid(doc, geomTranslated, element))
                                { ClashingElements.Add(element); }
                            }
                        }
                    }

                }
            }
            return ClashingElements;
        }


        /// <summary>
        /// Clear all overrides in view and just overrides selection to red
        /// </summary>
        /// <param name="L"></param>
        /// <param name="doc"></param>
        public static void OverrideInView(List<Element> L, Document doc)
        {
            var activeView = doc.ActiveView;

            FilteredElementCollector coll = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsViewIndependent().WhereElementIsNotElementType();
            var output = coll.ToElementIds();
            OverrideGraphicSettings clean = new OverrideGraphicSettings();
            foreach (var element in output)
            {
                activeView.SetElementOverrides(element, clean);
            }

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetProjectionLineColor(new Color(255, 0, 0));
            foreach (var e in L)
            {
                activeView.SetElementOverrides(e.Id, ogs);
            }


        }

        /// <summary>
        /// Check if an element is clashing with a solid
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool getClashWithSolid(Document doc, GeometryElement a, Element b)
        {

            //ElementId id = doc.GetElement(b).Id;
            List<ElementId> l = new List<ElementId>();
            l.Add(b.Id);

            bool output = false;

            //Look for each solid in linked model
            foreach (var geomObj in a)
            {
                Solid geomSolid = geomObj as Solid;
                if (null != geomSolid)
                {

                    ElementIntersectsSolidFilter elementIntersectsSolidFilter = new ElementIntersectsSolidFilter(geomSolid);
                    FilteredElementCollector collector = new FilteredElementCollector(doc, l).WherePasses(elementIntersectsSolidFilter);
                    foreach (var item in collector)
                    {
                        output = true;
                        break;
                    }

                }
                if (false != output) break;
            }

            return output;
        }
    }
}