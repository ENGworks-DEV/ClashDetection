
#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Resources;
#endregion

namespace RevitClasher
{
    class App : IExternalApplication
    {

        public Result OnStartup(UIControlledApplication application)
        {

            // Get the absolut path of this assembly
            string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly(
                ).Location;

            // Create a ribbon panel
            RibbonPanel m_projectPanel = application.CreateRibbonPanel(
                "RevitClasher");
            
            //Button
            PushButton pushButton = m_projectPanel.AddItem(new PushButtonData(
                "RevitClasher", "Revit Clasher", ExecutingAssemblyPath,
                "RevitClasher.Main")) as PushButton;

            //Add Help ToolTip 
            pushButton.ToolTip = "RevitClasher";

            //Add long description 
            pushButton.LongDescription =
             "This addin helps you to renumber MEP part with a prefix";

            // Set the large image shown on button.
            pushButton.LargeImage = PngImageSource(
                "RevitClasher.Resources.RevitClasherLogo.png");

            // Get the location of the solution DLL
            string path = System.IO.Path.GetDirectoryName(
               System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Combine path with \
            string newpath = Path.GetFullPath(Path.Combine(path, @"..\"));

            // Set the contextual help to point Help.html
            //ContextualHelp contextHelp = new ContextualHelp(
            //    ContextualHelpType.ChmFile,
            //    newpath + "Resources\\Help.html");

            ContextualHelp contextHelp = new ContextualHelp(
                ContextualHelpType.Url,
                "https://engworks.com/renumber-parts/");

            // Assign contextual help to pushbutton
            pushButton.SetContextualHelp(contextHelp);

            m_MyForm = null;   // no dialog needed yet; the command will bring it
            thisApp = this;  // static access to this application instance
            
            return Result.Succeeded;

        }

        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            // Get Bitmap from Resources folder
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
            var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream,
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            if (m_MyForm != null && m_MyForm.Visibility.Equals(Visibility.Visible))
            {
                m_MyForm.Close();
            }

         
            return Result.Succeeded;
        }

        // class instance
        public static App thisApp = null;

        private MainUserControl m_MyForm;
        //   The external command invokes this on the end-user's request
        public void ShowForm()
        {
            // If we do not have a dialog yet, create and show it
            if (m_MyForm == null)
            {
                // A new handler to handle request posting by the dialog
                ExternalEventClashDetection handler = new ExternalEventClashDetection();

                // External Event for the dialog to use (to post requests)
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                // We give the objects to the new dialog;
                // The dialog becomes the owner responsible for disposing them, eventually.
                m_MyForm = new MainUserControl(exEvent, handler);
                m_MyForm.Closed += MyFormClosed;
                m_MyForm.Show();
            }
           
        }

        private void MyFormClosed(object sender, EventArgs e)
        {
            m_MyForm = null;
        }
    }
}
