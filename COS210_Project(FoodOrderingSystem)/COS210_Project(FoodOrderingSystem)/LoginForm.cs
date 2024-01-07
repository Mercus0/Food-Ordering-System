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
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Xml.Linq;

namespace COS210_Project_FoodOrderingSystem_
{
    public partial class LoginForm : Form
    {
        bool isDragging = false;
        private Point lastcusorpos;
        private string name = "Enter your name here";
        private string pass = "Enter your password here";
        public string customerID;
        SqlConnection con=new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");

        public LoginForm()
        {
            InitializeComponent();
            lblinvalidname.Hide();
            lblinvalidpassword.Hide();
        }

        private void pictureBox5_MouseHover(object sender, EventArgs e) //link to facebook mouseover
        {
            picfacebook.Padding = new Padding(1);   
            picfacebook.BackColor = Color.SandyBrown;   //set the border color
        }

        private void picfacebook_MouseLeave(object sender, EventArgs e) //link facebook mouseleave
        {
            picfacebook.Padding = new Padding(0);
            picfacebook.BackColor = Color.White;
        }

        private void LoginForm_MouseUp(object sender, MouseEventArgs e) //for relocation form to up
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)   //for relocation form to mouse move
        {
            if (isDragging)
            {
                int dx=e.X-lastcusorpos.X;
                int dy=e.Y-lastcusorpos.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)   //for relocation form to mousedown
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastcusorpos = e.Location;
            }
        }

        private void button1_Click(object sender, EventArgs e)  //Login button click
        {        
            if ((txtname.Text.Equals("admin")) && (txtpassword.Text.Equals("admin")))
            {
                //admin form
                var adminpage = new Adamin_dashboard_form();
                adminpage.Show();
                this.Hide();
            }
            else
            {
                bool check1 = false;
                bool check2 = false;
                if ((txtname.Text == "") || (txtpassword.Text == ""))
                {
                    txtname.Text = name; txtpassword.Text = pass; txtname.ForeColor = Color.Gray; txtpassword.ForeColor = Color.Gray;
                    lblinvalidname.Hide(); lblinvalidpassword.Hide(); txtpassword.PasswordChar = '\0';
                    return;
                }
                else
                {
                    try
                    {
                        // Query for name
                        String query = "SELECT * FROM CUSTOMERINFO WHERE NAME COLLATE SQL_Latin1_General_CP1_CS_AS='" + txtname.Text + "'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                        DataTable dtName = new DataTable();
                        adapter.Fill(dtName);
                        if (dtName.Rows.Count == 1)
                        {
                            check1 = true;
                        }

                        // Query for password
                        String query1 = "SELECT * FROM CUSTOMERINFO WHERE PASSWORD COLLATE SQL_Latin1_General_CP1_CS_AS='" + txtpassword.Text + "'";
                        SqlDataAdapter adapter1 = new SqlDataAdapter(query1, con);
                        DataTable dtPassword = new DataTable();
                        adapter1.Fill(dtPassword);
                        if (dtPassword.Rows.Count == 1)
                        {
                            check2 = true;
                        }

                        if (check1 && check2)
                        {
                           
                            customerID = dtPassword.Rows[0]["ID_WITH_PREFIX"].ToString();
                            var mainmenu = new MainMemu(customerID);
                            mainmenu.Show();
                            this.Hide();
                        }
                        else if (!check1 && check2)
                        {
                            lblinvalidname.Show();
                        }
                        else if (check1 && !check2)
                        {
                            lblinvalidpassword.Show();
                        }
                        else
                        {
                            lblinvalidname.Show(); lblinvalidpassword.Show();
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e) //form load
        {
            txtname.Text = name;txtname.ForeColor = Color.Gray; txtpassword.Text = pass; txtpassword.ForeColor = Color.Gray;
        }

        private void txtname_MouseClick(object sender, MouseEventArgs e)    //txtname mouseclick
        {
            txtname.SelectAll();
        }

        private void txtpassword_MouseClick(object sender, MouseEventArgs e)//txtpassword mouseclick
        {
            txtpassword.SelectAll();
        }

        private void txtname_TextChanged(object sender, EventArgs e)    //txtname textchanged
        {
            txtname.ForeColor = Color.Black;
            lblinvalidname.Hide();
        }

        private void txtpassword_TextChanged(object sender, EventArgs e) //txtpassword textchanged
        {
            txtpassword.ForeColor = Color.Black;
            lblinvalidpassword.Hide();
        }

        private void picfacebook_Click(object sender, EventArgs e)  //link to facebook
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/GoodPlaceCafeCoffeeShop");
        }

        private void button1_Click_1(object sender, EventArgs e)    //close button
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)//minimixed button
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)//hide button
        {
            if (txtpassword.PasswordChar == '\0')
            {
                txtpassword.PasswordChar = '*';
                btnshow.BringToFront();
            }
        }

        private void button4_Click(object sender, EventArgs e)//show button
        {
            if (txtpassword.PasswordChar == '*')
            {
                txtpassword.PasswordChar = '\0';
                btnhide.BringToFront();
            }
        }

        private void txtpassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtpassword.PasswordChar = '*';
            if (e.KeyChar==' ')
            {
                e.Handled = true;
                MessageBox.Show("Space should not enter in password");
            }
            btnshow.BringToFront();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var register = new RegisterForm();
            register.Show();
            this.Hide();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //forget password
        {
            var Forgetform = new Forgetpassword();
            Forgetform.Show();
        }
    }
}