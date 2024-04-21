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
    

    public partial class InventoryManagerForm : Form
    {
        public string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\tjake\\source\\repos\\InventoryManagerProject\\InventoryManagerProject\\inventoryDB.mdf;Integrated Security=True";

        public InventoryManagerForm()
        {
            InitializeComponent();
        }

        private void itemsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.itemsBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.itemsDataSet);
        }

        private void InventoryManagerForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'itemsDataSet.items' table. You can move, or remove it, as needed.
            this.itemsTableAdapter.Fill(this.itemsDataSet.items);
        }

        //refreshes the content displayed to itemsDataGridView after each user action
        public void refreshItems()
        {
            SqlConnection con = new SqlConnection(connectionString);
            DataTable dt = new DataTable();
            //query to select all when resetting the datagridview
            SqlDataAdapter adp = new SqlDataAdapter("SELECT *FROM Items", con);
            con.Open();
            adp.Fill(dt);
            itemsDataGridView.DataSource = dt;
            con.Close();
        }

        //add new item to database
        private void newItem_Click(object sender, EventArgs e)
        {
            //create and open new item form. Refresh after control returns to main form
            NewItemForm newItemForm = new NewItemForm();
            newItemForm.ShowDialog(this);
            refreshItems();
        }

        //update an existing item
        private void updateButton_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            //command to udpate database item values
            SqlCommand cmd = new SqlCommand("Update items set Description=@Description, Category=@Category, Price=@Price, Stock=@Stock, Reorder=@Reorder where SKU=@SKU", con);
            cmd.Parameters.AddWithValue("@SKU", int.Parse(skuTextBox.Text));
            cmd.Parameters.AddWithValue("@Description", descriptionTextBox.Text);
            cmd.Parameters.AddWithValue("@Category", categoryTextBox.Text);
            cmd.Parameters.AddWithValue("@Price", double.Parse(priceTextBox.Text));
            cmd.Parameters.AddWithValue("@Stock", int.Parse(stockTextBox.Text));
            cmd.Parameters.AddWithValue("@Reorder", int.Parse(reorderTextBox.Text));
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Item updated");
            refreshItems();
        }


        //delete an existing item
        private void deleteButton_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete items where SKU=@SKU", con);
            cmd.Parameters.AddWithValue("@SKU", int.Parse(skuTextBox.Text));
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Item deleted");
            refreshItems();
        }

        //populate textboxes with current cell selection in itemsDataGridView
        private void itemsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            skuTextBox.Text = itemsDataGridView.CurrentRow.Cells[0].Value.ToString();
            descriptionTextBox.Text = itemsDataGridView.CurrentRow.Cells[1].Value.ToString();
            categoryTextBox.Text = itemsDataGridView.CurrentRow.Cells[2].Value.ToString();
            priceTextBox.Text = itemsDataGridView.CurrentRow.Cells[3].Value.ToString();
            stockTextBox.Text = itemsDataGridView.CurrentRow.Cells[4].Value.ToString();
            reorderTextBox.Text = itemsDataGridView.CurrentRow.Cells[5].Value.ToString();

        }

        //search all columns of data based on text entered. This method drops the need for a search button.
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            //query to search all columns and their values
            string searchQuery = "Select SKU,Description,Category,Price,Stock,Reorder from items";
            searchQuery += " Where SKU Like '%' +@parm1+ '%'";
            searchQuery += " OR Description Like '%' +@parm1+ '%'";
            searchQuery += " OR Category Like '%' +@parm1+ '%'";
            searchQuery += " OR Price Like '%' +@parm1+ '%'";
            searchQuery += " OR Stock Like '%' +@parm1+ '%'";
            searchQuery += " OR Reorder Like '%' +@parm1+ '%'";

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(searchQuery, con);

            cmd.Parameters.AddWithValue("parm1", searchTextBox.Text);

            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dt);
            itemsDataGridView.DataSource = dt;


        }
    }
}