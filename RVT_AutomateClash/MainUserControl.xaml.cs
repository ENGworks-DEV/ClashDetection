using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevitClasher
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : Window
    {
        public MainUserControl()
        {
            InitializeComponent();
            FillForm();
        }
        public void FillForm()
        {

            //Fill combo with linked documents
            foreach (var item in Clash.Documents(RevitTools.Doc, RevitTools.App))
            {
                ListOfLinks.Items.Add(item.Key);
            }

            var categories = FormTools.ListOfCategories(RevitTools.Doc);
            foreach (var c in categories)
            {
                //List of 
                CheckBox chbox = new CheckBox();
                chbox.Content = c.Key;
                SelectionBList.Items.Add(chbox);
             //   SelectionAList.Items.Add(chbox);

            }

            foreach (var c in categories)
            {
                //List of 
                CheckBox chbox = new CheckBox();
                chbox.Content = c.Key;
                SelectionAList.Items.Add(chbox);
                //   SelectionAList.Items.Add(chbox);

            }

            //Presetting everything from a previous run
            foreach (var item in RevitClasher.Properties.Settings.Default.SelectionB)
            {
                int i = int.Parse(item);
                CheckBox chbox = SelectionBList.Items[i] as CheckBox;
                chbox.IsChecked = true;
            }
            foreach (var item in RevitClasher.Properties.Settings.Default.SelectionA)
            {
                int i = int.Parse(item);
                CheckBox chbox = SelectionAList.Items[i] as CheckBox;
                chbox.IsChecked = true;
            }
            ListOfLinks.SelectedIndex = Properties.Settings.Default.ListOfLinks;

        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            SaveConfiguration();

        }

        private void SaveConfiguration()
        {
            Properties.Settings.Default.ListOfLinks = ListOfLinks.SelectedIndex;
            StringCollection selectionA = new StringCollection();
            for (int i = 0; i < SelectionAList.Items.Count; i++)
            {
                CheckBox chbox = SelectionAList.Items[i] as CheckBox;
                if (chbox.IsChecked.Value)
                {
                    selectionA.Add(i.ToString());
                }
            }
            Properties.Settings.Default.SelectionA = selectionA;


            StringCollection selectionB = new StringCollection();
            for (int i = 0; i < SelectionBList.Items.Count; i++)
            {
                CheckBox chbox = SelectionBList.Items[i] as CheckBox;
                if (chbox.IsChecked.Value)
                {
                    selectionB.Add(i.ToString());
                }
            }
            Properties.Settings.Default.SelectionB = selectionB;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            //Save Selection
            SaveConfiguration();

            var index = Properties.Settings.Default.ListOfLinks;
            var l = Clash.Documents(RevitTools.Doc, RevitTools.App);
            // Get document from select index
            FormTools.linkedDocument = l.Values[index];
            FormTools.SelectedCategories = new List<ElementFilter>();
            FormTools.SelectedHostCategories = new List<ElementFilter>();

            //Categories
            var cat = FormTools.ListOfCategories(RevitTools.Doc);

            //Get categories from select index
            foreach (var i in Properties.Settings.Default.SelectionB)
            {
                int item = int.Parse(i);
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), cat.Values[item].Id.ToString());
                FormTools.SelectedCategories.Add(new ElementCategoryFilter(myCatEnum));


            }

            //Get categories from select index
            foreach (var i in Properties.Settings.Default.SelectionA)
            {
                int item = int.Parse(i);
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), cat.Values[item].Id.ToString());
                FormTools.SelectedHostCategories.Add(new ElementCategoryFilter(myCatEnum));
                //Saving the local categories for a new run
                

            }
            Clash.Execute();
            FillResults();
        }

        internal void FillResults()
        {
            foreach (var item in Clash.elementsClashing)
            {
                Clashes.Items.Add(item.Id);

            }
        }

        private void OnSelected(object sender, RoutedEventArgs e)
        {
            int id = Int32.Parse(Clashes.SelectedItem.ToString());
            RevitTools.Focus(id);
        }

    }

}
