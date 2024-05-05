using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagerProject
{
    public partial class LoginForm : Form
    {

        

        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\tjake\\source\\repos\\InventoryManagerProject\\InventoryManagerProject\\inventoryDB.mdf;Integrated Security=True";
            SqlConnection con = new SqlConnection(connectionString);
            //case sensitive query for both username and password
            string query = "select * from Employees where username = '"+username+ "' collate sql_latin1_general_cp1_cs_as and password = '" +password+"' collate sql_latin1_general_cp1_cs_as";
            SqlDataAdapter adp = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            //if the datatable is filled with any data i.e. theres at least 1 row, that means the query had a match from the employee database and login is valid
            if (dt.Rows.Count > 0)
            {
                //hide the login page and display inventory manager form upon successful credential input
                InventoryManagerForm form = new InventoryManagerForm();
                this.Hide();
                form.ShowDialog();
            }
            else 
            {
                MessageBox.Show("Invalid credentials, input is case sensitive, try again", "Error");
            }  
            
        }



        //close button redundancy
        private void closeButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
