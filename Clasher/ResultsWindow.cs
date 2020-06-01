using System;
using System.Collections.Generic;
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
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow()
        {
            InitializeComponent();
            FillResults();
        }

        internal void FillResults()
        {
            Clashes.Items.Clear();
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
