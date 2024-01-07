using Nevron.Nov.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COS210_Project_FoodOrderingSystem_
{
    public partial class resetpasswordform : Form
    {
        private string name;
        private string cus_id;
        public resetpasswordform(string name,string cusid)
        {
            InitializeComponent();
            this.name = name;
            this.cus_id = cusid;
        }

        //reset password button
        private void button1_Click(object sender, EventArgs e)  
        {
            try
            {
                if (txtpassword.Text != txtcompassword.Text)
                {
                    MessageBox.Show("Do not match password");
                }

                else if (txtpassword.Text == txtcompassword.Text)
                {
                    if ((string.IsNullOrEmpty(txtpassword.Text)) || (string.IsNullOrEmpty(txtcompassword.Text)))
                    {
                        MessageBox.Show("Please fill the password and comfirm_password first");
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
                        con.Open();
                        SqlCommand cmd = new SqlCommand("Update CUSTOMERINFO set PASSWORD=@password where ID_WITH_PREFIX=@cusid", con);
                        cmd.Parameters.AddWithValue("@password", txtpassword.Text);
                        cmd.Parameters.AddWithValue("@cusid", cus_id);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Succeessful changed password");
                        this.Close();
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //password validing
        private void txtpassword_Validating(object sender, CancelEventArgs e)   
        {
            string password = txtpassword.Text;
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                //code here to show error message
                string message = "Password must be at least 8 characters long and \ncontain at least one uppercase letter, one lowercase \nletter, one number, and one special character.";
                messagebox msg=new messagebox(message);
                msg.Show();
                label2.ForeColor= Color.Red;
                txtpassword.Clear();
            }
            else
            {
                label2.ForeColor = Color.Blue;
            }
        }

        private void resetpasswordform_Load(object sender, EventArgs e) //form load
        {
            lblusername.Text = name;
        }

        //check box
        private void checkBox1_CheckedChanged(object sender, EventArgs e)  
        {
            if (checkBox1.Checked)
            {
                txtpassword.PasswordChar = '\0';
                txtcompassword.PasswordChar = '\0';
            }
            else
            {
                txtpassword.PasswordChar = '*';
                txtcompassword.PasswordChar = '*';
            }
        }
    }
}
