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

namespace RevitClasher
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("64E3DE6F-4F9F-4942-9EAC-02B2D54F6222"));

        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Application app = uiapp.Application;
            RevitTools.Doc = doc;
            RevitTools.App = app;
            RevitTools.Uidoc = uidoc;

            Autodesk.Revit.DB.View view = doc.ActiveView;
          

            if (doc.ActiveView.ViewType != ViewType.ThreeD)
            {
                TaskDialog.Show("Error", "It only works on a 3D view");
                return Result.Failed;
            }
            var testView = doc.ActiveView as View3D;
           
            if (!testView.IsSectionBoxActive)
            {
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("ClashDetective");
                    testView.IsSectionBoxActive = true;
                    uiapp.ActiveUIDocument.RefreshActiveView();
                    tx.Commit();

                }
            }

            //Debug
            var link = Clash.Documents(doc, app);
            try
            {
                App.thisApp.ShowForm();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            

        }
       
    }

    public class FormTools

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