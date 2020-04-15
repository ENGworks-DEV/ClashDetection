using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using RevitClasher.Handlers;
using RevitClasher.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace RevitClasher
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : Window
    {

        private ExternalEvent m_ExEvent;
        private ExternalEventClashDetection m_Handler;
        public static bool _wasExecuted;
        public static MainUserControl thisForm;
        public static ObservableCollection<ClashItems> elementsClashingB { get; set; }
        public static ObservableCollection<ClashItems> elementsClashingA { get; set; }
        public static bool _Reset = false;
        internal static bool _CropBox = false;
        private ExternalEvent _externalCleanEvent;
        private CleanViewHandler _cleanViewHandler;

        /// <summary>
        /// Constructs a instance of the main view for the revit addIn
        /// </summary>
        /// <param name="exEvent">External event for clashing process</param>
        /// <param name="handler">Handler for clashing process</param>
        /// <param name="externalCleanEvent">External event for cleaning view process</param>
        public MainUserControl(
            ExternalEvent exEvent,
            ExternalEventClashDetection handler,
            ExternalEvent externalCleanEvent
        ){
            

            InitializeComponent();
            m_ExEvent = exEvent;
            m_Handler = handler;
            _externalCleanEvent = externalCleanEvent;
            FillForm();

            this.DataContext = this;

            elementsClashingA = new ObservableCollection<ClashItems>();

            elementsClashingA.CollectionChanged += updateA;

            this.Topmost = true;

            ModelName.Content = RevitTools.Doc.Title.ToString();
        }

        public string projectVersion = CommonAssemblyInfo.Number;
        public string ProjectVersion
        {
            get { return projectVersion; }
            set { projectVersion = value; }
        }

        private void updateA(object sender, NotifyCollectionChangedEventArgs e)
        {
            Results.Items.Add(MainUserControl.elementsClashingA.Last());

            if (Results.Items.Count == 0)
            {
                TextBlockError.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TextBlockError.Visibility = System.Windows.Visibility.Hidden;
            }
        }


        public void FillForm()
        {

            //Fill combo with linked documents
            foreach (var item in Clash.Documents(RevitTools.Doc, RevitTools.App))
            {
                ListOfLinks.Items.Add(item.Key);
            }
            var SelectionA = new List<CheckBox>();
            var SelectionB = new List<CheckBox>();
            var categories = FormTools.ListOfCategories(RevitTools.Doc);
            foreach (var c in categories)
            {
                //List of 
                CheckBox chbox = new CheckBox();
                chbox.Content = c.Key;
                SelectionB.Add(chbox);
                //   SelectionAList.Items.Add(chbox);

            }

            foreach (var c in categories)
            {
                //List of 
                CheckBox chbox = new CheckBox();
                chbox.Content = c.Key;
                SelectionA.Add(chbox);
                //   SelectionAList.Items.Add(chbox);

            }
            SelectionAList.ItemsSource = SelectionA;
            SelectionBList.ItemsSource = SelectionB;

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



        private void SaveConfiguration()
        {
            Properties.Settings.Default.ListOfLinks = ListOfLinks.SelectedIndex;
            StringCollection selectionA = new StringCollection();
            var listA = ((IEnumerable<CheckBox>)SelectionAList.ItemsSource);
            var listB = ((IEnumerable<CheckBox>)SelectionBList.ItemsSource);
            for (int i = 0; i < listA.Count(); i++)
            {
                CheckBox chbox = listA.ElementAt(i) as CheckBox;
                if (chbox.IsChecked.Value)
                {
                    selectionA.Add(i.ToString());
                }
            }
            Properties.Settings.Default.SelectionA = selectionA;


            StringCollection selectionB = new StringCollection();
            for (int i = 0; i < listB.Count(); i++)
            {
                CheckBox chbox = listB.ElementAt(i) as CheckBox;
                if (chbox.IsChecked.Value)
                {
                    selectionB.Add(i.ToString());
                }
            }
            Properties.Settings.Default.SelectionB = selectionB;
            Properties.Settings.Default.Save();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            this.Results.Items.Clear();
            StringCollection selectionACollection = Properties.Settings.Default.SelectionA;
            StringCollection selectionBCollection = Properties.Settings.Default.SelectionB;
            String validationMessage = string.Empty;
            bool validationFlag = false;
            
            // Validation of elements selected
            if(selectionACollection.Count == 0 || selectionBCollection.Count == 0)
            {
                validationFlag = true;
                validationMessage = "Both categories must have elements selected to check for clashes";
            } 

            // If validationFlag is true. The validation dialog is showed.
            if(validationFlag)
            {
                ValidationDialog mValidationDialog = new ValidationDialog(validationMessage);
                mValidationDialog.Owner = this;
                bool closedDialog = (bool)mValidationDialog.ShowDialog();
                if(closedDialog)
                {
                    return;
                }
            }

            _Reset = false;
            //Save Selection
            SaveConfiguration();
            _wasExecuted = false;
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
            m_ExEvent.Raise();


        }



        private void OnSelectedA(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Results.SelectedItem != null)
                {
                    var vRVTElement = (ClashItems)Results.SelectedItem;
                    RevitTools.Focus(vRVTElement);
                }
            }
            catch
            {

            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // This runs "Execute" method on the handler class @see CleanViewHander class on "Handlers" folder
            // For more information of how this handlers works... 
            // go to: https://thebuildingcoder.typepad.com/blog/2015/12/external-event-and-10-year-forum-anniversary.html
            _externalCleanEvent.Raise();
        }

        private void SearchA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectionAList.ItemsSource != null)
            {
                CollectionView view = CollectionViewSource.GetDefaultView(SelectionAList.ItemsSource) as CollectionView;
                view.Filter = CategoryFilterA;

            }

        }
        private bool CategoryFilterA(object item)
        {
            CheckBox Items = (CheckBox)item;
            return Items.Content.ToString().ToUpper().Contains(SearchA.Text.ToUpper());
        }

        private void SearchB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectionBList.ItemsSource != null)
            {
                CollectionView view = CollectionViewSource.GetDefaultView(SelectionBList.ItemsSource) as CollectionView;
                view.Filter = CategoryFilterB;

            }
        }

        private bool CategoryFilterB(object item)
        {
            CheckBox Items = (CheckBox)item;
            return Items.Content.ToString().ToUpper().Contains(SearchB.Text.ToUpper());
        }



        private void IsolateElements_click(object sender, RoutedEventArgs e)
        {
            Selection selection = RevitTools.Uidoc.Selection;
            ICollection<ElementId> selectedIds = RevitTools.Uidoc.Selection.GetElementIds();
            if (selectedIds.Count > 0)
            {
                try
                {
                    RevitTools.Doc.ActiveView.IsolateElementsTemporary(selectedIds);
                    RevitTools.Uidoc.RefreshActiveView();
                }
                catch (Exception vEx)
                {
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //_CropBox = CropBox.IsChecked ?? false;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Title_Link(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://engworks.com/renumber-parts/");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectionAList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}