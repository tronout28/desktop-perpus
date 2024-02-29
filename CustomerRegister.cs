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

namespace Libra
{
    public partial class CustomerRegister : Form
    {
        public CustomerRegister()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=medicineLogin;Integrated Security=True;TrustServerCertificate=True");

        private void btn_submit_Click(object sender, EventArgs e)
        { 
            // Check if username and password are not empty
            if (!string.IsNullOrEmpty(tb_username.Text) && !string.IsNullOrEmpty(tb_pass.Text))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM customerLogin WHERE username=@username AND password=@password";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", tb_username.Text);
                    checkCmd.Parameters.AddWithValue("@password", tb_pass.Text);

                    int existingRecords = (int)checkCmd.ExecuteScalar();

                    if (existingRecords > 0)
                    {
                        MessageBox.Show("Username and password already exist.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // If the username and password do not exist, insert the new record
                        string insertQuery = "INSERT INTO customerLogin (username, password) VALUES (@username, @password)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@username", tb_username.Text);
                            insertCmd.Parameters.AddWithValue("@password", tb_pass.Text);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                DialogResult dialogResult = MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                if (dialogResult == DialogResult.OK)
                                {
                                    CustomerLogin customerLogin = new CustomerLogin();
                                    customerLogin.Show();
                                    this.Hide();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Registration failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Username and password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.Show();
            this.Hide();
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin keluar?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.OK)
            {
                Application.Exit();
            }
            else
            {

            }
        }
    }
}
