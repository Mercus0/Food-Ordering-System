using Nevron.Nov.Graphics;
using Nevron.Nov.UI;
using System;
using System.Collections;
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
    public partial class OrderList : Form
    {
        ArrayList list = new ArrayList();
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Yan Naing\\Documents\\FoodOrderingAndDeliverySystem.mdf\";Integrated Security=True;Connect Timeout=30");
        private string _customerId;
        double totalamount = 0;
        public OrderList(ArrayList a, string customerId)
        {
            _customerId = customerId;
            InitializeComponent();
            list = a;
        }


        private void OrderList_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT NAME,PHONE,ADDRESS FROM CUSTOMERINFO where ID_WITH_PREFIX=@Id", con);
                cmd.Parameters.AddWithValue("@Id", _customerId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    label10.Text = reader.GetString(0);
                    label11.Text = reader.GetString(1);
                    label12.Text = reader.GetString(2);
                }
                con.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            display();
        }
        void display()
        {
            int rowno = 0;
            
            dgborder.Rows.Clear();
            if (list.Count > 0)
            {
                foreach (ItemClass c in list)
                {
                    int n = dgborder.Rows.Add();
                    dgborder.Rows[n].Cells[0].Value = c.Id.ToString();

                    //call the function
                    ItemDetail details = getitemdetail(c.Id);

                    dgborder.Rows[n].Cells[1].Value = details.Name;
                    dgborder.Rows[n].Cells[2].Value = details.price.ToString();
                    dgborder.Rows[n].Cells[3].Value = c.quantity.ToString();
                    double itemamount = c.quantity * details.price;
                    dgborder.Rows[n].Cells[4].Value = itemamount.ToString();
                    totalamount += itemamount;
                    rowno = n;
                }
                dgborder.Rows[rowno + 1].Cells[2].Value = "Total Amount >>>>";
                dgborder.Rows[rowno + 1].Cells[4].Value = totalamount.ToString();
            }
        }

        //get from database 
        ItemDetail getitemdetail(string id)
        {  
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT NAME,PRICE FROM ITEMS WHERE Id LIKE '" + id + "'", con);
                SqlDataReader dr = cmd.ExecuteReader();
                ItemDetail details = new ItemDetail();
                if (dr.Read())
                {
                    details.Name = dr.GetString(dr.GetOrdinal("NAME"));
                    details.price = (float)dr.GetDouble(dr.GetOrdinal("PRICE"));
                }
                con.Close();    
                return details;
        }

        class ItemDetail
        {
            public string Name { get; set; }
            public float price { get; set; }
        }

        private void reselect_Click(object sender, EventArgs e)
        {
            list.Clear();
            display();
        }

        //button comfirm
        private void btnconfirm_Click(object sender, EventArgs e)
        {
            if (dgborder.Rows.Count == 1)
            {
                MessageBox.Show("Please choose at least One");
            }
            else
            {
                try
                {
                    con.Open();
                    DateTime orderdate = DateTime.Now;
                    string datetime = orderdate.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [ORDER](CUST_ID,ORDER_DATE,Total)VALUES(@CUST_ID,@Date_time,@total);SELECT SCOPE_IDENTITY();", con))
                    {
                        cmd.Parameters.AddWithValue("@CUST_ID", _customerId);
                        cmd.Parameters.AddWithValue("@Date_time", datetime);
                        cmd.Parameters.AddWithValue("@total", totalamount.ToString());

                        int orderId = Convert.ToInt32(cmd.ExecuteScalar());

                        using (SqlCommand cmd1 = new SqlCommand("INSERT INTO [OrderDetials](Order_Id, Item_Id, Qty) VALUES (@Order_Id, @Item_Id, @Qty)", con))
                        {
                            for (int i = 0; i < dgborder.Rows.Count - 1; i++)
                            {
                                string itemId = dgborder.Rows[i].Cells[0].Value.ToString();
                                int qty = Convert.ToInt32(dgborder.Rows[i].Cells[3].Value);
                                using (SqlCommand checkItemCmd = new SqlCommand("SELECT Id FROM ITEMS WHERE Id = @Item_Id", con))
                                {
                                    checkItemCmd.Parameters.AddWithValue("@Item_Id", itemId);
                                    object result = checkItemCmd.ExecuteScalar();
                                }
                                cmd1.Parameters.Clear();
                                cmd1.Parameters.AddWithValue("@Order_Id", orderId);
                                cmd1.Parameters.AddWithValue("@Item_Id", itemId);
                                cmd1.Parameters.AddWithValue("@Qty", qty);
                                cmd1.ExecuteNonQuery();
                            }
                            MessageBox.Show("Thanks for Ordering, we will sent to you with in 4 hours,\n you can cancel this order within in 4 hours.", "Thank You", MessageBoxButtons.OK);
                            list.Clear();
                            this.Close();
                        }
                    }
                    con.Close();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void dgborder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
