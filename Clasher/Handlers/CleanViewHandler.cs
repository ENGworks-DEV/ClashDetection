using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clasher.Handlers
{
    /// <summary>
    /// Handler class destinated to provide an event handler for actions and modifications of the active document
    /// </summary>
    public class CleanViewHandler : IExternalEventHandler
    {
        /// <summary>
        /// Event handler that provides context to execute actions over the Active UI Document and performs cleaning of the highlited elements
        /// </summary>
        /// <param name="app">Injected UIApplication from revit API</param>
        public void Execute(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;

            using (Transaction tx = new Transaction(app.ActiveUIDocument.Document, "CleanViewTransaction"))
            {
                try
                {
                    tx.Start();
                    RevitTools.ResetView();
                    uiDoc.RefreshActiveView();
                    tx.Commit();
                }
                catch
                {
                    TaskDialog.Show("Clasher Detection: Error", "An error has occured. Cannot clean view");
                }
            }
        }

        public string GetName()
        {
            return "View Handler";
        }

    }
}
