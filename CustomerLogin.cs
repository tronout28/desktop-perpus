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
    public partial class CustomerLogin : Form
    {
        public CustomerLogin()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=medicineLogin;Integrated Security=True;TrustServerCertificate=True");

        private void btn_submit_Click(object sender, EventArgs e)
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM customerLogin WHERE username=@username AND password=@password";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", tb_username.Text);
            cmd.Parameters.AddWithValue("@password", tb_pass.Text);
            int count = (int)cmd.ExecuteScalar();

            if (count > 0)
            {
                CustomerDashboard customerDashboard = new CustomerDashboard();
                customerDashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Login Gagal Coba Ulang Kembali", "Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin keluar?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(dialogResult == DialogResult.OK)
            {
                Application.Exit();
            }
            else
            {

            }
        }

        private void btn_regist_Click(object sender, EventArgs e)
        {
            CustomerRegister customerRegister = new CustomerRegister();
            customerRegister.Show();
            this.Hide();
        }
    }
}
