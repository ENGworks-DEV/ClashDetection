using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RVT_AutomateClash
{
    public partial class ResultForm : Form
    {
        public ResultForm()
        {

            this.TopMost = true;

            InitializeComponent();
   
        }

        private void ResultForm_Load(object sender, EventArgs e)
        {
            foreach (var item in Clash.elementsClashing)
            {
                listBox1.Items.Add(item.Id);

            }

            

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Int32.Parse(listBox1.SelectedItem.ToString());
            RevitTools.Focus(id);
        }
    }
}
