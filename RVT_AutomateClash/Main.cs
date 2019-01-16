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
            var link = Clash.Documents(doc, app);
            using (MainForm thisform = new MainForm())
            {

                thisform.ShowDialog();
                if (thisform.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Clash.elementsClashing = Clash.clashingElements(doc, app);

                    using (Transaction t = new Transaction(doc, "Clash"))
                    {
                        t.Start();
                        TaskDialog.Show("ll", Clash.elementsClashing.Count().ToString());
                        RevitTools.OverrideInView(Clash.elementsClashing, doc);

                        t.Commit();

                    }

                }
            }

            ResultForm result = new ResultForm();

                result.Show();

                return Result.Succeeded;
        }

    }

    public class MainFormTools

    {

        public static Document linkedDocument { get; set; }

        public static List<ElementFilter> SelectedCategories { get; set; }


        public static List<ElementFilter> SelectedHostCategories { get; set; }


        /// <summary>
        /// List of available category in model
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static SortedList<string, Category> ListOfCategories(Document doc)
        {
            //https://forums.autodesk.com/t5/revit-api-forum/how-to-get-category-list/td-p/4477699

            Categories categories = doc.Settings.Categories;

            SortedList<string, Category> myCategories = new SortedList<string, Category>();

            foreach (Category c in categories)

            {

                if (c.AllowsBoundParameters && c.CategoryType == CategoryType.Model)

                    myCategories.Add(c.Name, c);

            }
            return myCategories;
        }

    }
    
}