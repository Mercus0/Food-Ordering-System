using Nevron.Nov.UI;
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

namespace COS210_Project_FoodOrderingSystem_
{
    public partial class Myorders : Form
    {
        int rowIndex;
        int orderId;
        private string _customerId;
        SqlCommand cmd;
        DataTable dt;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
        public Myorders(string customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            dataGridView1.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Myorders_Load(object sender, EventArgs e)
        {
            btnback.Hide();
            btncancel.Hide();
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT NAME,PHONE,ADDRESS FROM CUSTOMERINFO where ID_WITH_PREFIX=@Id", con);
                cmd.Parameters.AddWithValue("@Id", _customerId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    label10.Text = reader.GetString(0);
                    label11.Text = reader.GetString(1);
                    label12.Text = reader.GetString(2);
                }
                con.Close();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Invoicelist();
        }

        //invoice list
        public void Invoicelist()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            try
            {
                con.Open();
                adapter = new SqlDataAdapter();
                    cmd = new SqlCommand("SELECT o.ID AS Invoice_No, c.NAME AS Name, o.ORDER_DATE AS DATE,o.Total as Total FROM[dbo].[ORDER] o JOIN[dbo].[CUSTOMERINFO] c ON o.CUST_ID = c.ID_WITH_PREFIX WHERE o.CUST_ID = @Id", con);
                cmd.Parameters.AddWithValue("@Id", _customerId);
                adapter.SelectCommand = cmd;
                dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //datagrid view click
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)     
        {
            btnback.Show();
            btncancel.Show();
            rowIndex = dataGridView1.CurrentCell.RowIndex;
            orderId = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[0].Value);
            label16.Text = orderId.ToString();
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT ORDER_DATE FROM [ORDER] WHERE ID=@order_id", con);
                cmd.Parameters.AddWithValue("@order_id", orderId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime datetime = reader.GetDateTime(0);
                    label15.Text = datetime.ToString("dd-MM-yyyy");
                }
                dataGridView1.DataSource = null;
                //set the datagirdview column
                dataGridView1.ColumnCount = 5;
                dataGridView1.Columns[0].Name = "No";
                dataGridView1.Columns[0].Width = 30;

                dataGridView1.Columns[1].Name = "Particular";

                dataGridView1.Columns[2].Name = "Qty";
                dataGridView1.Columns[2].Width = 30;

                dataGridView1.Columns[3].Name = "Price";
                dataGridView1.Columns[3].Width = 50;
                dataGridView1.Columns[4].Name = "Amount";
                dataGridView1.Columns[4].Width = 75;
                reader.Close();

                cmd = new SqlCommand("SELECT i.NAME,od.Qty,i.PRICE FROM [dbo].[ITEMS] i JOIN [dbo].[OrderDetials] od ON i.ID = od.Item_Id JOIN [dbo].[ORDER] o ON o.ID = od.Order_Id WHERE o.ID = @orderid", con);
                cmd.Parameters.AddWithValue("@orderid", orderId);
                reader = cmd.ExecuteReader();
                int i = 1;
                double net = 0;
                while (reader.Read())
                {
                    string itemname = reader.GetString(0);
                    int qty = reader.GetInt32(1);
                    float price = (float)reader.GetDouble(2);
                    double nettotal = qty * price;

                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);
                    row.Cells[0].Value = i;
                    row.Cells[1].Value = itemname;
                    row.Cells[2].Value = qty;
                    row.Cells[3].Value = price; 
                    row.Cells[4].Value = nettotal;
                    dataGridView1.Rows.Add(row);
                    net += nettotal;
                    i++;
                }
                DataGridViewRow total = new DataGridViewRow();
                total.CreateCells(dataGridView1);
                total.Cells[3].Value = "Total";
                total.Cells[4].Value = net;
                dataGridView1.Rows.Add(total);
                reader.Close();
                con.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //cancel order button
        private void btncancel_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("select ORDER_DATE from [ORDER] WHERE ID=@order_id", con);
                cmd.Parameters.AddWithValue("@order_id", orderId);
                DateTime start = Convert.ToDateTime(cmd.ExecuteScalar());    
                TimeSpan different = DateTime.Now - start;
                con.Close();
                if (different.TotalHours >= 4)
                {
                    MessageBox.Show("This order is already comfirm");
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure want to cancel this order", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //cancel order here
                    if (result == DialogResult.Yes)
                    {   //delete from orderdetail table
                        con.Open();
                        cmd = new SqlCommand("delete from [OrderDetials] where Order_ID=@order_id", con);
                        cmd.Parameters.AddWithValue("@order_id", orderId);
                        cmd.ExecuteNonQuery();

                        //delete from order table
                        cmd = new SqlCommand("delete from [ORDER] where ID=@order_id", con);
                        cmd.Parameters.AddWithValue("@order_id", orderId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cancel Successfully");
                        con.Close();
                        Invoicelist();
                        btnback.Hide();
                    }
                    else if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                btncancel.Hide();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            Invoicelist();
            btnback.Hide();
            btncancel.Hide();
        }
    }
}
