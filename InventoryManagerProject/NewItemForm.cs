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


namespace InventoryManagerProject
{
    public partial class NewItemForm : Form
    {
        
        public NewItemForm()
        {
            InitializeComponent();
            
        }

        //add the new item to the database
        private void addItemButton_Click(object sender, EventArgs e)
        {
            InventoryManagerForm formInstance = new InventoryManagerForm();
            SqlConnection con = new SqlConnection(formInstance.connectionString);
            con.Open();
            //command to add new data to database
            SqlCommand cmd = new SqlCommand("insert into items values (@SKU, @Description, @Category, @Price, @Stock, @Reorder)", con);
            cmd.Parameters.AddWithValue("@SKU", int.Parse(newSKUTextBox.Text));
            cmd.Parameters.AddWithValue("@Description", newDescriptionTextBox.Text);
            cmd.Parameters.AddWithValue("@Category", newCategoryTextBox.Text);
            cmd.Parameters.AddWithValue("@Price", double.Parse(newPriceTextBox.Text));
            cmd.Parameters.AddWithValue("@Stock", int.Parse(newStockTextBox.Text));
            cmd.Parameters.AddWithValue("@Reorder", int.Parse(newReorderTextBox.Text));
            cmd.ExecuteNonQuery();
            con.Close();

            //confirm to user that new item was added to the database
            MessageBox.Show("Item added");

            //close the new item form
            Close();
        }
    }
}
