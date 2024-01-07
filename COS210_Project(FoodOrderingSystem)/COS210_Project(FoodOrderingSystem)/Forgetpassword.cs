using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COS210_Project_FoodOrderingSystem_
{
    public partial class Forgetpassword : Form
    {
        string cus_id;
        public Forgetpassword()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  //forget password button
        {
            //check textbox is null
            if (txtemail.Text != "")
            {
                try
                {   //if not null
                    SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM CUSTOMERINFO WHERE EMAIL=@email", con);
                    cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        //email found here
                        while ((reader.Read()))
                        {
                            string name = reader.GetString(reader.GetOrdinal("NAME"));
                            cus_id = reader.GetString(reader.GetOrdinal("ID_WITH_PREFIX"));

                            DialogResult dt = MessageBox.Show("Is that you " + name + "?", "Confirm", MessageBoxButtons.YesNo) ;
                            if (dt == DialogResult.Yes)
                            {
                                resetpasswordform resetform = new resetpasswordform(name,cus_id);
                                resetform.Show();
                                this.Hide();
                            }
                            else if (dt == DialogResult.No)
                            {
                                return;
                            }
                        }
                        con.Close();
                        reader.Close();
                    }
                    else
                    {
                        //email not found
                        MessageBox.Show("This email does not exist in our records");
                        txtemail.Clear();
                        return;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter your Email first");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }
    }
}
