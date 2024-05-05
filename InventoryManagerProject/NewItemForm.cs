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
        

        //constructor
        public NewItemForm()
        {
            InitializeComponent();

        }



        //add the new item to the database
        private void addItemButton_Click(object sender, EventArgs e)
        {
            //error handling to ensure unique key is provided, but will also catch other error types as well and display the error message
            try
            {
                InventoryManagerForm formInstance = new InventoryManagerForm();
                //create and open the connection
                SqlConnection con = new SqlConnection(formInstance.connectionString);
                //command to add new data to database
                SqlCommand cmd = new SqlCommand("insert into items values (@SKU, @Description, @Category, @Price, @Stock, @Reorder)", con);

                cmd.Parameters.AddWithValue("@SKU", int.Parse(newSKUTextBox.Text));
                cmd.Parameters.AddWithValue("@Description", newDescriptionTextBox.Text);
                cmd.Parameters.AddWithValue("@Category", newCategoryTextBox.Text);
                cmd.Parameters.AddWithValue("@Price", double.Parse(newPriceTextBox.Text));
                cmd.Parameters.AddWithValue("@Stock", int.Parse(newStockTextBox.Text));
                cmd.Parameters.AddWithValue("@Reorder", int.Parse(newReorderTextBox.Text));

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //confirm to user that new item was added to the database
                MessageBox.Show("Item added");


                //close the new item form
                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK);
            }


            


        }
            

        //cancel button redundancy for newitemform
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
