using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

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
            System.Data.DataTable dt = new System.Data.DataTable();
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

            System.Data.DataTable dt = new System.Data.DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            adp.Fill(dt);
            itemsDataGridView.DataSource = dt;


        }


        //button to close the program
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Export inventory report
        private void exportItemListButton_Click(object sender, EventArgs e)
        {
            string queryString = "select * from items";
            string filePath = @"C:\Users\tjake\Desktop\Inventory Report.xlsx";
            try
            {
                // Connect to the SQL Server database and retrieve the data you want to export
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(queryString, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Create a new excel application and workbook to save data in
                            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                            Microsoft.Office.Interop.Excel.Workbook excelWorkbook = excelApp.Workbooks.Add();
                            Microsoft.Office.Interop.Excel.Worksheet excelWorksheet = excelWorkbook.Worksheets[1];
                            //loop to add headers
                            int col = 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                excelWorksheet.Cells[1, col].Value2 = reader.GetName(i);
                                col++;
                            }
                            // loop to add remaining data on the next rows
                            int row = 2;
                            while (reader.Read())
                            {
                                col = 1;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    excelWorksheet.Cells[row, col].Value2 = reader[i];
                                    col++;
                                }
                                row++;
                            }
                            // Save the workbook and close the Excel application
                            excelWorkbook.SaveAs(filePath);
                            excelWorkbook.Close();
                            excelApp.Quit();
                        }
                    }
                }
                MessageBox.Show("Exported", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        


        //Export low stock report 
        private void exportLowStockButton_Click(object sender, EventArgs e)
        {
            string queryString = "select * from items where Stock <= Reorder";
            string filePath = @"C:\Users\tjake\Desktop\Low Stock Report.xlsx";
            try
            {
                // Connect to the SQL Server database and retrieve the data you want to export
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(queryString, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Create a new excel application and workbook to save data in
                            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                            Microsoft.Office.Interop.Excel.Workbook excelWorkbook = excelApp.Workbooks.Add();
                            Microsoft.Office.Interop.Excel.Worksheet excelWorksheet = excelWorkbook.Worksheets[1];
                            //loop to add headers
                            int col = 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                excelWorksheet.Cells[1, col].Value2 = reader.GetName(i);
                                col++;
                            }
                            // loop to add remaining data on the next rows
                            int row = 2;
                            while (reader.Read())
                            {
                                col = 1;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    excelWorksheet.Cells[row, col].Value2 = reader[i];
                                    col++;
                                }
                                row++;
                            }
                            // Save the workbook and close the Excel application
                            excelWorkbook.SaveAs(filePath);
                            excelWorkbook.Close();
                            excelApp.Quit();
                        }
                    }
                }
                MessageBox.Show("Exported", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




 
    }
}