using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace COS210_Project_FoodOrderingSystem_
{
    
    public partial class RegisterForm : Form
    {
        bool isDragging = false;
        private Point lastcusorpos;

        string username = "Enter your name here";
        string password = "Enter password here";
        string phoneno = "Enter your phone number here";
        public RegisterForm()
        {
            InitializeComponent();     
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            txtaddress.ForeColor = Color.Black;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void RegisterForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void RegisterForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int dx = e.X - lastcusorpos.X;
                int dy = e.Y - lastcusorpos.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        private void RegisterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastcusorpos = e.Location;
            }
        }

        private void btncreate_Click(object sender, EventArgs e)    //register button
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO CUSTOMERINFO ([NAME], [PASSWORD], [PHONE], [GENDER], [EMAIL], [ADDRESS]) VALUES (@Name, @Password, @Phone, @Gender, @Email, @Address)", con))
                {
                    if ((txtname.Text == username) || (txtname.Text == "") || (txtname.Text.Length < 3))
                    {
                        lblusername.Show();
                        return;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Name", txtname.Text);  //User Name textbox
                    }

                    //check phone no
                    if ((txtphone.Text == phoneno))
                    {
                        lblphoneno.Show();
                        return;
                    }
                    else
                    {
                        if((!txtphone.Text.StartsWith("09"))||(txtphone.Text.Length!=11))
                        { 
                            lblphoneno.Show();
                            return;
                        }
                        else 
                            cmd.Parameters.AddWithValue("@Phone",txtphone.Text);// phoneno textbox
                    }

                    //check invalid password or not
                    if ((txtpassword.Text == password) || (txtpassword.Text.Length <= 6))
                    {
                        lblpassword.Show();
                        return;
                    }
                    else
                    {
                        string check=txtpassword.Text;
                        if(!Regex.IsMatch(check, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
                        {
                            string message = "Password must be at least 8 characters long and \ncontain at least one uppercase letter, one lowercase \nletter, one number, and one special character.";
                            messagebox msg = new messagebox(message);
                            msg.Show();
                            return;
                        }
                        else {
                            cmd.Parameters.AddWithValue("@Password", txtpassword.Text);  //password textbox
                        }
                    }

                    if(txtpassword.Text != txtretypepassword.Text) {
                        return;
                    }

                    //check invalid email or not
                    string email = txtemail.Text.Trim();
                    if (email != "" && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        cmd.Parameters.AddWithValue("@Email", txtemail.Text);   //email textbox
                    }
                    else
                    {
                        lblemail.Show();return;
                    }

                    if (rdomale.Checked == true)  //gender textbox
                    {
                        cmd.Parameters.AddWithValue("@Gender", "Male");
                    }
                    else if (rdofemale.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@Gender", "Female");
                    }else if((rdofemale.Checked == false) &&(rdomale.Checked==false)) {
                        lblgenter.Show();
                        return;
                    }

                    //check invalid address or not
                    if ((txtaddress.Text == "Enter your address")||(txtaddress.Text=="")||(txtaddress.Text.Length<10 ))
                    {
                        lbladdress.Show();
                        return;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Address", txtaddress.Text);    //address textbox
                    }
                    cmd.ExecuteNonQuery();
                    con.Close();
                }              
                MessageBox.Show("Register Successfully");
                var loginform=new LoginForm();
                this.Close();
                loginform.Show();
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            txtname.ForeColor = Color.Gray;lblusername.Hide();
            txtphone.ForeColor = Color.Gray;lblphoneno.Hide();
            txtpassword.ForeColor = Color.Gray;lblpassword.Hide();
            txtretypepassword.ForeColor=Color.Gray; lblretypepassword.Hide();
            txtemail.ForeColor = Color.Gray;lblemail.Hide();
            lblgenter.Hide();
            txtaddress.ForeColor = Color.Gray;lbladdress.Hide();
        }

        private void txtname_MouseClick(object sender, MouseEventArgs e)
        {
            txtname.SelectAll();
        }

        private void txtphone_MouseClick(object sender, MouseEventArgs e)
        {
            txtphone.SelectAll();
        }

        private void txtpassword_MouseClick(object sender, MouseEventArgs e)
        {
            txtpassword.SelectAll();
        }

        private void txtretypepassword_MouseClick(object sender, MouseEventArgs e)
        {
            txtretypepassword.SelectAll();
        }

        private void txtemail_MouseClick(object sender, MouseEventArgs e)
        {
            txtemail.SelectAll();
        }

        private void txtaddress_MouseClick(object sender, MouseEventArgs e)
        {
            txtaddress.SelectAll();
        }

        private void txtname_TextChanged(object sender, EventArgs e)
        {
            txtname.ForeColor = Color.Black;lblusername.Hide();
        }

        private void txtname_Leave(object sender, EventArgs e)
        {
            if (txtname.Text == "")
            {
                txtname.Text = "Enter your name here";
                txtname.ForeColor = Color.Gray;
            }
        }

        private void txtphone_TextChanged(object sender, EventArgs e)
        {
            txtphone.ForeColor=Color.Black;lblphoneno.Hide();
        }

        private void txtphone_Leave(object sender, EventArgs e)
        {
            if (txtphone.Text == "")
            {
                txtphone.Text = "Enter your phone number here";
                txtphone.ForeColor = Color.Gray;
            }
        }

        private void txtpassword_TextChanged(object sender, EventArgs e)
        {
            txtpassword.ForeColor=Color.Black;lblpassword.Hide();
        }

        private void txtpassword_Leave(object sender, EventArgs e)
        {
            if (txtpassword.Text =="")
            {
               txtpassword.Text = "Enter password here";
               txtpassword.ForeColor = Color.Gray;
            }
        }

        private void txtretypepassword_Leave(object sender, EventArgs e)
        {
            if (txtretypepassword.Text == "")
            {
                txtretypepassword.Text = "Retype password";
                txtretypepassword .ForeColor= Color.Gray;
            }
        }

        private void txtemail_Leave(object sender, EventArgs e)
        {
            if (txtemail.Text == "")
            {
                txtemail.Text = "Enter your email";
                txtemail.ForeColor = Color.Gray;
            }
        }

        private void txtaddress_Leave(object sender, EventArgs e)
        {
            if (txtaddress.Text == "")
            {
                txtaddress.Text = "Enter your address";
                txtaddress.ForeColor = Color.Gray;
            }
        }

        private void txtemail_TextChanged(object sender, EventArgs e)
        {
            txtemail.ForeColor = Color.Black;lblemail.Hide();
        }

        private void txtretypepassword_TextChanged(object sender, EventArgs e)
        {
            txtretypepassword.ForeColor = Color.Black;
            string password = txtpassword.Text;
            string re_password = txtretypepassword.Text;
            if (password != re_password)
            {
                lblretypepassword.Show();
            }
            else
            {
                lblretypepassword.Hide();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)

        {
            var loginform=new LoginForm();
            loginform.Show();
            this.Hide();
        }

        private void txtphone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar)&& !char.IsDigit(e.KeyChar) ){
                e.Handled= true;
                MessageBox.Show("Phone number should enter number only", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
