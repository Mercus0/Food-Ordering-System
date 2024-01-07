using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COS210_Project_FoodOrderingSystem_
{
    public partial class messagebox : Form
    {
        string message;
        public messagebox(string message)
        {
            InitializeComponent();
            this.message = message;
        }

        private void button1_Click(object sender, EventArgs e)  //Ok button
        {
            this.Close();
        }

        private void messagebox_Load(object sender, EventArgs e)
        {
            lblerrormessage.Text = message;
        }
    }
}
