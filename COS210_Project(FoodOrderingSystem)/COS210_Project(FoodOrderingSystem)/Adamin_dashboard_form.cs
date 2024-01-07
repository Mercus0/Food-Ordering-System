using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace COS210_Project_FoodOrderingSystem_
{
    
    public partial class Adamin_dashboard_form : Form
    {
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\" + "FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
        string imageUrl = null;
        SqlCommand cmd;
        public Adamin_dashboard_form()
        {
            InitializeComponent();
        }

        private void Adamin_dashboard_form_Load(object sender, EventArgs e)
        {
            display();  //form load (customer data grid view)
            itemdisplay(); //stocks data grid view

            btnitemupdate.Hide(); //items update button
            btnitemdelete.Hide(); //items delete button

            //all voucher datagrid view
            try
            {
                con.Open();
                int i = 1;
                cmd = new SqlCommand("select o.ID,CONVERT(date,o.ORDER_DATE),c.NAME,o.Total from [ORDER] o join [CUSTOMERINFO] c ON o.CUST_ID=c.ID_WITH_PREFIX", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int inv_no = reader.GetInt32(0);
                    DateTime date = reader.GetDateTime(1);
                    string cust_name = reader.GetString(2);
                    int total = reader.GetInt32(3);

                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(allvouchers);
                    row.Cells[0].Value = i;
                    row.Cells[1].Value = inv_no;
                    row.Cells[2].Value = date.ToString("dd/MM/yyyy");
                    row.Cells[3].Value = cust_name;
                    row.Cells[4].Value = total;
                    allvouchers.Rows.Add(row);
                    i++;
                    
                }reader.Close();
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


                try
                {
                    //today total sales
                    con.Open();
                    int totalsale = 0;
                    cmd = new SqlCommand("Select CAST(ISNULL(SUM(Total),0)AS INT) from [ORDER] where CONVERT(date,ORDER_DATE) = CONVERT(date, GETDATE())", con);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        totalsale = (int)result;
                        lbltotal.Text = totalsale.ToString();
                    }
                    else
                    {
                        lbltotal.Text = "0.00";
                    }
                    con.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //customer datagrid view click
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) 
        {
            try{
                lblabel.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtname.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtpassword.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtphone.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                if (dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString() == "Male")
                {
                    rdomale.Checked = true;
                } else rdofemale.Checked = true;

                txtemail.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                txtaddress.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            } catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        //customer delete button
        private void button3_Click(object sender, EventArgs e)  
        {
            if (lblabel.Text.ToString() == "")
            {
                MessageBox.Show("Please choose to delete.");
            }
            else
            {
                try
                {
                    DialogResult result = MessageBox.Show("Are you sure want to delete?", "Comfirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {

                        con.Open();
                        cmd = new SqlCommand("select ID from [ORDER] where CUST_ID=@cusid", con);
                        cmd.Parameters.AddWithValue("@cusid", lblabel.Text);
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<int> orderids = new List<int>();
                        while (reader.Read())
                        {
                            orderids.Add((int)reader["ID"]);
                        }
                        reader.Close();

                        foreach (int orderid in orderids)
                        {
                            cmd = new SqlCommand("Delete from OrderDetials where Order_Id=@orderid", con);
                            cmd.Parameters.AddWithValue("@orderid", orderid);
                            cmd.ExecuteNonQuery();
                        }

                        cmd = new SqlCommand("Delete from [ORDER] where CUST_ID=@cusid", con);
                        cmd.Parameters.AddWithValue("@cusid", lblabel.Text);
                        cmd.ExecuteNonQuery();

                        cmd = new SqlCommand("delete from CUSTOMERINFO where ID_WITH_PREFIX=@customerid", con);
                        cmd.Parameters.AddWithValue("@customerid", lblabel.Text);
                        cmd.ExecuteNonQuery();

                        con.Close();
                        clear();
                        display();

                    }
                    else if (result == DialogResult.No)
                    {
                        return;
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void display()      //Customer data gridview
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT NAME,PASSWORD,PHONE,GENDER,EMAIL,ADDRESS,ID_WITH_PREFIX FROM CUSTOMERINFO", con);
                SqlDataReader reader = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    int row = dataGridView1.Rows.Add();
                    dataGridView1.Rows[row].Cells[0].Value = reader["ID_WITH_PREFIX"].ToString();
                    dataGridView1.Rows[row].Cells[1].Value = reader["NAME"].ToString();
                    dataGridView1.Rows[row].Cells[2].Value = reader["PASSWORD"].ToString();
                    dataGridView1.Rows[row].Cells[3].Value = reader["PHONE"].ToString();
                    dataGridView1.Rows[row].Cells[4].Value = reader["GENDER"].ToString();
                    dataGridView1.Rows[row].Cells[5].Value = reader["EMAIL"].ToString();
                    dataGridView1.Rows[row].Cells[6].Value = reader["ADDRESS"].ToString();
                }
                int rowcount = dataGridView1.RowCount - 1;
                lblcount.Text = "There are " + rowcount + " customer in our records.";
                con.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void itemdisplay()   //stock datagridview
        {
            dataGridView2.Rows.Clear();
            try
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("SELECT Id,NAME,PRICE,PICTURE,CATEGORY,STATUS,REMARK FROM ITEMS", con);
                SqlDataReader read = cmd2.ExecuteReader();
                dataGridView2.Rows.Clear();
                while (read.Read())
                {
                    int row = dataGridView2.Rows.Add();
                    dataGridView2.Rows[row].Cells[0].Value = read["Id"].ToString();
                    dataGridView2.Rows[row].Cells[1].Value = read["NAME"].ToString();
                    dataGridView2.Rows[row].Cells[2].Value = read["PRICE"].ToString();
                    dataGridView2.Rows[row].Cells[3].Value = read["CATEGORY"].ToString();
                    dataGridView2.Rows[row].Cells[4].Value = read["STATUS"].ToString();
                    dataGridView2.Rows[row].Cells[5].Value = read["REMARK"].ToString();
                }
                con.Close();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void clear()    //clear function for customer 
        {
            lblabel.Text = "";
            txtname.Clear();
            txtphone.Clear();
            txtpassword.Clear();
            rdomale.Checked= false;rdofemale.Checked = false;
            txtemail.Clear();
            txtaddress.Clear();     
        }


        //update button
        private void button4_Click(object sender, EventArgs e)  
        {
            if (lblabel.Text.ToString()=="")
            {
                MessageBox.Show("Please choose to update.");
            }
            else {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE CUSTOMERINFO SET NAME=@NAME,PASSWORD=@PASSWORD,PHONE=@PHONE,GENDER=@GENDER,EMAIL=@EMAIL,ADDRESS=@ADDRESS WHERE ID_WITH_PREFIX=@ID", con);
                    cmd.Parameters.AddWithValue("@NAME", txtname.Text);  //User Name textbox
                    cmd.Parameters.AddWithValue("@PHONE", txtphone.Text);// phoneno textbox
                    cmd.Parameters.AddWithValue("@PASSWORD", txtpassword.Text);  //password textbox
                    cmd.Parameters.AddWithValue("@EMAIL", txtemail.Text);   //email textbox


                    if (rdomale.Checked == true)  //gender textbox
                    {
                        cmd.Parameters.AddWithValue("@GENDER", "Male");
                    }
                    else if (rdofemale.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@GENDER", "Female");
                    }
                    else if ((rdofemale.Checked == false) && (rdomale.Checked == false))
                    {
                        return;
                    }

                    cmd.Parameters.AddWithValue("@ADDRESS", txtaddress.Text);    //address textbox
                    cmd.Parameters.AddWithValue("@ID", lblabel.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Update customer data successfully");

                    con.Close();
                    display();
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Adamin_dashboard_form_HelpRequested(object sender, HelpEventArgs hlpevent) //function f1 key refresh
        {
            display();
            itemdisplay();
            hlpevent.Handled = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)  //refresh button
        {
            display();
        }


        //browse button
        private void button3_Click_1(object sender, EventArgs e)    
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imageUrl = ofd.FileName;
                    pictureBox1.Image = Image.FromFile(imageUrl);
                }
            }
        }

        //add button from items
        private void button6_Click(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(txtitem.Text))||(string.IsNullOrEmpty(itemname.Text)))
            {
                MessageBox.Show("Please fill the item information first.");
            }
            else {
                try
                {
                    Image img = pictureBox1.Image;
                    byte[] arr;
                    ImageConverter converter = new ImageConverter();
                    arr = (byte[])converter.ConvertTo(img, typeof(byte[]));
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO ITEMS(Id,NAME,PRICE,PICTURE,PICTUREURL,CATEGORY,STATUS,REMARK) VALUES(@Id,@NAME,@PRICE,@PICTURE,@PICTUREURL,@CATEGORY,@STATUS,@REMARK)", con);
                    cmd.Parameters.AddWithValue("@Id", txtitem.Text);
                    cmd.Parameters.AddWithValue("@NAME", itemname.Text);
                    cmd.Parameters.AddWithValue("@PRICE", float.Parse(txtprice.Text));
                    cmd.Parameters.AddWithValue("@PICTURE", arr);
                    cmd.Parameters.AddWithValue("@PICTUREURL", imageUrl);
                    cmd.Parameters.AddWithValue("@CATEGORY", cbocatagory.Text);
                    cmd.Parameters.AddWithValue("@STATUS", cbostatus.Text);
                    cmd.Parameters.AddWithValue("@REMARK", txtremark.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                MessageBox.Show("Successful update");
                txtitem.Clear();
                itemclear();
                itemdisplay();
            }

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            
        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        //item datagrid view click
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            panel14.Hide();
            txtitem.Hide();
            btnadditem.Hide();
            btnitemdelete.Show();
            btnitemupdate.Show();
            lblitemid.Text= dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
            itemname.Text = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtprice.Text = dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString();
            cbocatagory.Text = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
            cbostatus.Text = dataGridView2.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtremark.Text = dataGridView2.Rows[e.RowIndex].Cells[5].Value.ToString();
        }

        //Items update button
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                // Update the ITEMS table
                con.Open();
                cmd = new SqlCommand("UPDATE ITEMS SET NAME=@NAME, PRICE=@PRICE, CATEGORY=@CATEGORY, STATUS=@STATUS, REMARK=@REMARK WHERE Id=@id", con);
                cmd.Parameters.AddWithValue("@id", lblitemid.Text);
                cmd.Parameters.AddWithValue("@NAME", itemname.Text);
                cmd.Parameters.AddWithValue("@PRICE", float.Parse(txtprice.Text));
                cmd.Parameters.AddWithValue("@CATEGORY", cbocatagory.Text);
                cmd.Parameters.AddWithValue("@STATUS", cbostatus.Text);
                cmd.Parameters.AddWithValue("@REMARK", txtremark.Text);
                cmd.ExecuteNonQuery();

                con.Close();
                MessageBox.Show("Update item successfully");
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                itemclear();
                itemdisplay();
        }


        //Item delete button
        private void button10_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure want to delete?", "Comfirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    cmd = new SqlCommand("delete from OrderDetials where Item_id=@id",con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@id",lblitemid.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    
                    cmd = new SqlCommand("DELETE FROM ITEMS WHERE Id=@Id", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@Id", lblitemid.Text.ToString());
                    cmd.ExecuteNonQuery();
                    con.Close();

                    itemclear();
                    itemdisplay();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else if (result == DialogResult.No)
            {
                return;
            }
        }
        
        //clear itemtextboxs
        public void itemclear()
        {
            lblitemid.Text="";
            txtitem.Show();
            panel14.Show();
            itemname.Clear();
            txtprice.Clear();
            pictureBox1.Hide();
            cbocatagory.SelectedIndex= -1;
            cbostatus.SelectedIndex= -1;
            txtremark.Clear();
            btnadditem.Show();
            btnitemupdate.Hide();
            btnitemdelete.Hide();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        //admin logout button
        private void button5_Click_1(object sender, EventArgs e)
        {
            var login = new LoginForm();
            login.Show();
            this.Close();
        }

        //search key
        private void txtsearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT NAME,PASSWORD,PHONE,GENDER,EMAIL,ADDRESS,ID_WITH_PREFIX FROM CUSTOMERINFO WHERE NAME LIKE @search+'%'", con);
                cmd.Parameters.AddWithValue("@search",txtsearch.Text.ToString());
                SqlDataReader reader = cmd.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    int row = dataGridView1.Rows.Add();
                    dataGridView1.Rows[row].Cells[0].Value = reader["ID_WITH_PREFIX"].ToString();
                    dataGridView1.Rows[row].Cells[1].Value = reader["NAME"].ToString();
                    dataGridView1.Rows[row].Cells[2].Value = reader["PASSWORD"].ToString();
                    dataGridView1.Rows[row].Cells[3].Value = reader["PHONE"].ToString();
                    dataGridView1.Rows[row].Cells[4].Value = reader["GENDER"].ToString();
                    dataGridView1.Rows[row].Cells[5].Value = reader["EMAIL"].ToString();
                    dataGridView1.Rows[row].Cells[6].Value = reader["ADDRESS"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //refresh key
        private void button7_Click(object sender, EventArgs e)
        {
            itemdisplay();
        }

        //item search
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            dataGridView2.Rows.Clear();
            try
            {
                SqlCommand cmd2 = new SqlCommand("SELECT Id,NAME,PRICE,PICTURE,CATEGORY,STATUS,REMARK FROM ITEMS where NAME like @name+'%'", con);
                con.Open();
                cmd2.Parameters.AddWithValue("@name", textBox2.Text.ToString());
                SqlDataReader read = cmd2.ExecuteReader();
                while (read.Read())
                {
                    int row = dataGridView2.Rows.Add();
                    dataGridView2.Rows[row].Cells[0].Value = read["Id"].ToString();
                    dataGridView2.Rows[row].Cells[1].Value = read["NAME"].ToString();
                    dataGridView2.Rows[row].Cells[2].Value = read["PRICE"].ToString();
                    dataGridView2.Rows[row].Cells[3].Value = read["CATEGORY"].ToString();
                    dataGridView2.Rows[row].Cells[4].Value = read["STATUS"].ToString();
                    dataGridView2.Rows[row].Cells[5].Value = read["REMARK"].ToString();
                }
                con.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //cancel
        private void button1_Click(object sender, EventArgs e)
        {
            itemclear();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        //add new customer
        private void button6_Click_1(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(txtname.Text)) || (string.IsNullOrEmpty(txtpassword.Text)))
            {
                MessageBox.Show("Please fill the item information first.");
            }
            else
            {
                try
                {
                    con.Open();
                    cmd = new SqlCommand("INSERT INTO CUSTOMERINFO ([NAME], [PASSWORD], [PHONE], [GENDER], [EMAIL], [ADDRESS]) VALUES (@Name, @Password, @Phone, @Gender, @Email, @Address)", con);
                    cmd.Parameters.AddWithValue("@Name", txtname.Text);
                    cmd.Parameters.AddWithValue("@Password", txtpassword.Text);
                    cmd.Parameters.AddWithValue("@Phone", txtphone.Text);
                    if (rdomale.Checked == true)  //gender textbox
                    {
                        cmd.Parameters.AddWithValue("@Gender", "Male");
                    }
                    else if (rdofemale.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@Gender", "Female");
                    }
                    cmd.Parameters.AddWithValue("@Email", txtemail.Text);
                    cmd.Parameters.AddWithValue("Address", txtaddress.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    display();
                    clear();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        
        }

        private void allvouchers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
