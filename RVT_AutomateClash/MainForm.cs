using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using Newtonsoft.Json;

namespace RVT_AutomateClash
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            foreach (var f in  Application.OpenForms.OfType<ResultForm>().ToList())
            {
                f.Close();
            }

            FillForm();


        }
        
        /// <summary>
        /// Fill all elements in form
        /// </summary>
        public  void FillForm()
        {
            
            //Fill combo with linked documents
            foreach (var item in Clash.Documents(RevitTools.Doc,RevitTools.App))
            {
                linkedModelsCombo.Items.Add(item.Key);
            }
            
            foreach (var c in MainFormTools.ListOfCategories(RevitTools.Doc))
            {
                //List of 
                LinkCategories.Items.Add(c.Key);
                
                hostCategories.Items.Add(c.Key);
            }
            //Presetting everything from a previous run
            Settings settings = ReadSettings();
            foreach (int item in settings.LinkedCategories)
            {
                LinkCategories.SetItemChecked(item, true);
            }
            foreach (int item in settings.LocalCategories)
            {
                hostCategories.SetItemChecked(item, true);
            }
            linkedModelsCombo.SelectedIndex = settings.LinkedModel;

        }
    
        private void linkedModelsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void categoryCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();

            var index = linkedModelsCombo.SelectedIndex;
            var l = Clash.Documents(RevitTools.Doc, RevitTools.App);
            // Get document from select index
            MainFormTools.linkedDocument = l.Values[index];
            MainFormTools.SelectedCategories = new List<ElementFilter>();
            MainFormTools.SelectedHostCategories = new List<ElementFilter>();

            settings.LinkedModel = index;

            //Categories
            var cat = MainFormTools.ListOfCategories(RevitTools.Doc);

            //Get categories from select index
            foreach (int item in LinkCategories.CheckedIndices)
            {
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), cat.Values[item].Id.ToString());
                MainFormTools.SelectedCategories.Add(new ElementCategoryFilter(myCatEnum));
                //Saving the linked categories for a new run
                settings.LinkedCategories.Add(item);
                
            }

            //Get categories from select index
            foreach (int item in hostCategories.CheckedIndices)
            {

                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), cat.Values[item].Id.ToString());
                MainFormTools.SelectedHostCategories.Add(new ElementCategoryFilter(myCatEnum));
                //Saving the local categories for a new run
                settings.LocalCategories.Add(item);

            }

            var json = JsonConvert.SerializeObject(settings);
            SaveSettings(json);
            

            this.DialogResult =  DialogResult.OK;
        }


       public static Settings ReadSettings()
        {
            var text = "";
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\settings";

            try
            {
                using (StreamReader read = new StreamReader(path))

                {
                    text = read.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<Settings>(text);
            }
            catch { return new Settings(); }
            
        }
        public static void SaveSettings(string text)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\settings";
            using (StreamWriter writetext = new StreamWriter(path))
            {
                writetext.Write(text);
            }


        }

        private void hostCategories_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void hideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
    public class Settings
        {
        public  int LinkedModel { get; set; }
        public  List<int>LinkedCategories { get; set; }
        public  List<int> LocalCategories { get; set; }

        public  Settings()
        {
            LinkedModel = 0;
            LinkedCategories = new List<int>();
            LocalCategories = new List<int>();
        }
        }
}
