using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVT_AutomateClash
{
    class Clash

    {

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
        public static List<Element> clashingElements(Document doc, Application app, Document link = null)
        {
            //TODO : Create a separate addin to save configuration

            var linkedElements = new List<Element>();
            var localElements = new List<Element>();
            var ClashingElements = new List<Element>();
            //If second document was not provided, use first doc
            var SecondDocument = link == null ? doc: link;

            //Get the transformation of the linked model from the project location
            var transform = GetTransform(doc, SecondDocument);

            Autodesk.Revit.DB.Options opt = new Options();
            var ActiveViewBB = doc.ActiveView as View3D;

            BoundingBoxXYZ bbV = ActiveViewBB.GetSectionBox();
      

                var VisibleLinkedElements = new List<GeometryElement>();

                //Hard coded selection
                LogicalOrFilter filterLinkedCategories = new LogicalOrFilter(SelectionB());
                FilteredElementCollector walls = new FilteredElementCollector(SecondDocument).WherePasses(filterLinkedCategories).WhereElementIsNotElementType();
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

                        LogicalOrFilter logicalOrFilter = new LogicalOrFilter(SelectionA());

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
            return ClashingElements;
        }


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

        /// <summary>
        /// Selection A, first list, local elements
        /// </summary>
        /// <returns></returns>
        internal static List<ElementFilter> SelectionA()
        {
            //TODO: change to UI selection
            var categories =  new List<ElementFilter>() {
                new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves),
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),

        };


            return categories;
        }

        /// <summary>
        /// Selection B, second list
        /// </summary>
        /// <returns></returns>
        internal static List<ElementFilter> SelectionB()
        {//TODO: change to UI selection
            var categories = new List<ElementFilter>() {
                new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns),
                new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming),

        };


            return categories;
        }
    }
}
