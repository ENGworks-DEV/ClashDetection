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

namespace RevitClasher.Views.Dialogs
{
    /// <summary>
    /// Lógica de interacción para ValidationDialog.xaml
    /// </summary>
    public partial class ValidationDialog : Window
    {

        private string _validationMessage;

        /// <summary>
        /// Constructs a new instance of Validation Dialog
        /// </summary>
        /// <param name="valMessage">Message to prompt</param>
        public ValidationDialog(string valMessage)
        {
            InitializeComponent();
            OnStartUpValidationDialog(valMessage);
        }

        /// <summary>
        /// Loads configurations and sets message of validation dialog
        /// </summary>
        /// <param name="valMessage">String that contains the validation message</param>
        private void OnStartUpValidationDialog(string valMessage)
        {
            // Setting class variables
            _validationMessage = valMessage;

            // Initialazing methods
            setValidationMessage();
        }

        private void setValidationMessage()
        {
            MessageBox.Text = _validationMessage;
        }

        /// <summary>
        /// Acepts the result and dimiss the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Close the validation dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
