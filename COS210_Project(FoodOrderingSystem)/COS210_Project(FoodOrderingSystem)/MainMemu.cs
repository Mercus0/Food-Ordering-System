using Nevron.Nov.Grid;
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
using System.IO;
using System.Net.Http.Headers;
using System.Collections;
using Nevron.Nov;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Net;
using System.Xml.Linq;

namespace COS210_Project_FoodOrderingSystem_
{
    public partial class MainMemu : Form
    {
        public ArrayList itemlist = new ArrayList();    //create a array to store item id
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
        SqlCommand cmd;
        SqlDataReader dr;
        private PictureBox pic;
        private Label price;
        private Label name;
        private Button button;
        private NumericUpDown qty;

        int quan=1;

        private string _customerId;
        public MainMemu(string customerId)
        {
            _customerId= customerId;
            InitializeComponent();
        }

        //form load here
        private void MainMemu_Load(object sender, EventArgs e)
        {
            lblid.Text = _customerId;
            con.Open();
            SqlCommand cmd=new SqlCommand("SELECT NAME FROM CUSTOMERINFO where ID_WITH_PREFIX=@Id", con);
            cmd.Parameters.AddWithValue("@Id", _customerId);
            SqlDataReader reader=cmd.ExecuteReader();
            while (reader.Read())
            {
                lblname.Text=reader.GetString(0);
            }
            con.Close();
            getalldata();
        }


        //load pictures from database
        private void getalldata()
        {
            flowLayoutPanel1.Controls.Clear();
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT PICTURE,NAME,PRICE,Id,REMARK FROM ITEMS WHERE STATUS='Avaliable'", con);
                dr= cmd.ExecuteReader();
                while(dr.Read())
                {
                    //image read from data base
                    long len = dr.GetBytes(0, 0, null, 0, 0);
                    byte[] array=new byte[System.Convert.ToInt32(len)+1];
                    dr.GetBytes(0,0,array, 0, System.Convert.ToInt32(len));
                    pic=new PictureBox();
                    pic.Width = 250;
                    pic.Height = 250;
                    pic.BackgroundImageLayout = ImageLayout.Stretch;
                    MemoryStream ms = new MemoryStream(array);
                    Bitmap bitmap = new Bitmap(ms);
                    pic.BackgroundImage = bitmap;
                    pic.Dock= DockStyle.Fill;
                    pic.Tag= dr["REMARK"].ToString();

                    //name
                    name = new Label();
                    name.Text = dr["NAME"].ToString();
                    name.TextAlign = ContentAlignment.MiddleCenter;
                    name.Dock = DockStyle.Left;

                    //Price
                    price=new Label();
                    price.Text = dr["PRICE"].ToString()+" MMK";
                    price.TextAlign = ContentAlignment.MiddleCenter;
                    price.Dock = DockStyle.Right;

                    //button
                    button = new Button();
                    button.Width = 250;
                    button.Height = 40;
                    button.Text = "Add to Cart";
                    button.Dock = DockStyle.Fill;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Tag = dr["Id"].ToString();

                    //qty
                    qty=new NumericUpDown();
                    qty.Minimum = 1;
                    qty.Maximum = 20;
                    qty.Width = 50;
                    qty.Height = 50;
                    qty.Dock= DockStyle.Right;

                    //create panel3
                    Panel panel3=new Panel();
                    panel3.Dock = DockStyle.Bottom;
                    panel3.Width = 250;
                    panel3.Height = 40;
                    panel3.Controls.Add(button);
                    panel3.Controls.Add(qty);

                    //create panel2
                    Panel panel2 = new Panel();
                    panel2.Dock = DockStyle.Top;
                    panel2.BackColor = Color.RosyBrown;
                    panel2.Width = 250;
                    panel2.Height = 25;
                    panel2.Controls.Add(name);
                    panel2.Controls.Add(price);

                    //create panel1
                    Panel panel1 =new Panel();
                    panel1.Width = 250;panel1.Height = 375;
                    panel1.BorderStyle=BorderStyle.FixedSingle;
                    panel1.Controls.Add(pic);
                    panel1.Controls.Add(panel2);
                    panel1.Controls.Add(panel3);

                    //add main panel here
                    flowLayoutPanel1.Controls.Add(panel1);
                    button.Click += new EventHandler(onclick);
                    pic.Click += new EventHandler(imgclick);
                    qty.Click += new EventHandler(qtyclick);
                }
                con.Close();
            }catch(Exception ex) {
               MessageBox.Show(ex.Message);
            }

        }

        //qty click
        private void qtyclick(object sender, EventArgs e)
        {
            quan= (int)((NumericUpDown)sender).Value;
        }

        //picture click
        public void imgclick(object sender, EventArgs e)
        {
            String remark=((PictureBox)sender).Tag.ToString();
            MessageBox.Show(remark);
        }

        //add to add click  
        public void onclick(object sender, EventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();
            ItemClass item=new ItemClass(tag,quan);
            itemlist.Add(item);
        }


        //view voucher
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OrderList orderlist= new OrderList(itemlist,_customerId);
            orderlist.Show();
        }

        //My order button 
        private void button2_Click(object sender, EventArgs e)
        {
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT ID FROM [ORDER] where CUST_ID= @Id",con);
            adapter.SelectCommand.Parameters.AddWithValue("@Id",_customerId);
            DataTable dt=new DataTable();
            adapter.Fill(dt);
            con.Close();
            if (dt.Rows.Count > 0)
            {
                var myorders = new Myorders(_customerId);
                myorders.Show();
            }
            else
            {
                MessageBox.Show("You haven't any order yet");
            }
        }

        //All button
        private void button3_Click(object sender, EventArgs e)
        {
            getalldata();
        }

        private void getdata(string v, object ava)
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var login = new LoginForm();
            login.Show();
            this.Close();
        }

        //Main button
        private void button4_Click(object sender, EventArgs e)
        {
            getselecteddata("Main");
        }
        void getselecteddata(string dataname)
        {
            flowLayoutPanel1.Controls.Clear();
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT PICTURE,NAME,PRICE,Id,REMARK FROM ITEMS WHERE STATUS='Avaliable' AND CATEGORY=@category", con);
                cmd.Parameters.AddWithValue("@category",dataname);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //image read from data base
                    long len = dr.GetBytes(0, 0, null, 0, 0);
                    byte[] array = new byte[System.Convert.ToInt32(len) + 1];
                    dr.GetBytes(0, 0, array, 0, System.Convert.ToInt32(len));
                    pic = new PictureBox();
                    pic.Width = 250;
                    pic.Height = 250;
                    pic.BackgroundImageLayout = ImageLayout.Stretch;
                    MemoryStream ms = new MemoryStream(array);
                    Bitmap bitmap = new Bitmap(ms);
                    pic.BackgroundImage = bitmap;
                    pic.Dock = DockStyle.Fill;
                    pic.Tag = dr["REMARK"].ToString();

                    //name
                    name = new Label();
                    name.Text = dr["NAME"].ToString();
                    name.TextAlign = ContentAlignment.MiddleCenter;
                    name.Dock = DockStyle.Left;

                    //Price
                    price = new Label();
                    price.Text = dr["PRICE"].ToString() + " MMK";
                    price.TextAlign = ContentAlignment.MiddleCenter;
                    price.Dock = DockStyle.Right;

                    //button
                    button = new Button();
                    button.Width = 250;
                    button.Height = 40;
                    button.Text = "Add to Cart";
                    button.Dock = DockStyle.Fill;
                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Tag = dr["Id"].ToString();

                    //qty
                    qty = new NumericUpDown();
                    qty.Minimum = 1;
                    qty.Maximum = 20;
                    qty.Width = 50;
                    qty.Height = 50;
                    qty.Dock = DockStyle.Right;

                    //create panel3
                    Panel panel3 = new Panel();
                    panel3.Dock = DockStyle.Bottom;
                    panel3.Width = 250;
                    panel3.Height = 40;
                    panel3.Controls.Add(button);
                    panel3.Controls.Add(qty);

                    //create panel2
                    Panel panel2 = new Panel();
                    panel2.Dock = DockStyle.Top;
                    panel2.BackColor = Color.RosyBrown;
                    panel2.Width = 250;
                    panel2.Height = 25;
                    panel2.Controls.Add(name);
                    panel2.Controls.Add(price);

                    //create panel1
                    Panel panel1 = new Panel();
                    panel1.Width = 250; panel1.Height = 375;
                    panel1.BorderStyle = BorderStyle.FixedSingle;
                    panel1.Controls.Add(pic);
                    panel1.Controls.Add(panel2);
                    panel1.Controls.Add(panel3);

                    //add main panel here
                    flowLayoutPanel1.Controls.Add(panel1);
                    button.Click += new EventHandler(onclick);
                    pic.Click += new EventHandler(imgclick);
                    qty.Click += new EventHandler(qtyclick);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Slides button
        private void button5_Click(object sender, EventArgs e)
        {
            getselecteddata("Sides");
        }

        //Bervages
        private void button6_Click(object sender, EventArgs e)
        {
            getselecteddata("Beverages");
        }

        //search textbox
        private void searchbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            string data = searchbox.Text.ToString();
            flowLayoutPanel1.Controls.Clear();
            cmd = new SqlCommand("SELECT PICTURE,NAME,PRICE,Id,REMARK FROM ITEMS where NAME like @search+'%'",con);
            con.Open();
            cmd.Parameters.AddWithValue("@search", data);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //image read from data base
                long len = dr.GetBytes(0, 0, null, 0, 0);
                byte[] array = new byte[System.Convert.ToInt32(len) + 1];
                dr.GetBytes(0, 0, array, 0, System.Convert.ToInt32(len));
                pic = new PictureBox();
                pic.Width = 250;
                pic.Height = 250;
                pic.BackgroundImageLayout = ImageLayout.Stretch;
                MemoryStream ms = new MemoryStream(array);
                Bitmap bitmap = new Bitmap(ms);
                pic.BackgroundImage = bitmap;
                pic.Dock = DockStyle.Fill;
                pic.Tag = dr["REMARK"].ToString();

                //name
                name = new Label();
                name.Text = dr["NAME"].ToString();
                name.TextAlign = ContentAlignment.MiddleCenter;
                name.Dock = DockStyle.Left;

                //Price
                price = new Label();
                price.Text = dr["PRICE"].ToString() + " MMK";
                price.TextAlign = ContentAlignment.MiddleCenter;
                price.Dock = DockStyle.Right; ;

                //button
                button = new Button();
                button.Width = 250;
                button.Height = 40;
                button.Text = "Add to Cart";
                button.Dock = DockStyle.Fill;
                button.BackgroundImageLayout = ImageLayout.Stretch;
                button.Tag = dr["Id"].ToString();

                //qty
                qty = new NumericUpDown();
                qty.Minimum = 1;
                qty.Maximum = 20;
                qty.Width = 50;
                qty.Height = 50;
                qty.Dock = DockStyle.Right;

                //create panel3
                Panel panel3 = new Panel();
                panel3.Dock = DockStyle.Bottom;
                panel3.Width = 250;
                panel3.Height = 40;
                panel3.Controls.Add(button);
                panel3.Controls.Add(qty);

                //create panel2
                Panel panel2 = new Panel();
                panel2.Dock = DockStyle.Top;
                panel2.BackColor = Color.RosyBrown;
                panel2.Width = 250;
                panel2.Height = 25;
                panel2.Controls.Add(name);
                panel2.Controls.Add(price);

                //create panel1
                Panel panel1 = new Panel();
                panel1.Width = 250; panel1.Height = 375;
                panel1.BorderStyle = BorderStyle.FixedSingle;
                panel1.Controls.Add(pic);
                panel1.Controls.Add(panel2);
                panel1.Controls.Add(panel3);

                //add main panel here
                flowLayoutPanel1.Controls.Add(panel1);
                button.Click += new EventHandler(onclick);
                pic.Click += new EventHandler(imgclick);
                qty.Click += new EventHandler(qtyclick);
            }
            con.Close();
        }

        private void searchbox_MouseClick(object sender, MouseEventArgs e)
        {
            searchbox.SelectAll();
        }
    }
}



 