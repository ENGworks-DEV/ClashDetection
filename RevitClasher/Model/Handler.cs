using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitClasher
{
    public class ExternalEventClashDetection : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            using (Transaction tx = new Transaction(app.ActiveUIDocument.Document))
            {
                tx.Start("ClashDetective");



                Clash.Execute(app.ActiveUIDocument.Document);

                //MainUserControl.thisForm.FillResults();

                app.ActiveUIDocument.RefreshActiveView();
                tx.Commit();

            }
         
        }

        public string GetName()
        {
            return "External Event ClashDetective";
        }
    }
}
