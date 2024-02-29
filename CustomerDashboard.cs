using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Libra
{
    public partial class CustomerDashboard : Form
    {
        public CustomerDashboard()
        {
            InitializeComponent();
            load_form();
            usn();
            bind_data();
        }

        SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=Libra;Integrated Security=True");

        private void bind_data()
        {
            SqlCommand cmd = new SqlCommand("Select noId,namaBuku As namaBuku, namaPenulis As namaPenulis, tanggalPenulisan As tanggalPenulisan ,tentang As tentang,penerbit As penerbit, tanggalPinjam As tanggalPinjam, tanggalKembali from libraData", conn);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dt.Clear();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Bold);
        }

        private void usn()
        {
            SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=medicineLogin;Integrated Security=True");

            try
            {
                using (connection)
                {
                    SqlCommand cmd = new SqlCommand("SELECT username FROM customerLogin", connection);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Access the username from the result set
                            string username = reader["username"].ToString();

                            // Update the lbl_usn label with the retrieved username
                            lbl_usn.Text = username;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void load_form()
        {
            this.mainPanel.Controls.Clear();
            HomePanel formHome = new HomePanel();
            formHome.TopLevel = false;
            formHome.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(formHome);
            this.mainPanel.Tag = formHome;
            formHome.Show();

        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            sidePanel.Height = btn_home.Height;
            sidePanel.Top = btn_home.Top;

            this.mainPanel.Controls.Clear();
            HomePanel formHome = new HomePanel();
            formHome.TopLevel = false;
            formHome.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(formHome);
            this.mainPanel.Tag = formHome;
            formHome.Show();

            panel3.Show();
        }

        private void btn_borrow_Click(object sender, EventArgs e)
        {
            sidePanel.Height = btn_borrow.Height;
            sidePanel.Top = btn_borrow.Top;

            panel3.Hide();
            this.mainPanel.Controls.Clear();
            CustomerBorrow formHome = new CustomerBorrow();
            formHome.TopLevel = false;
            formHome.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(formHome);
            this.mainPanel.Tag = formHome;
            formHome.Show();
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin Log Out?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            if(dialogResult == DialogResult.OK)
            {
                CustomerLogin customerLogin = new CustomerLogin();
                customerLogin.Show();
                this.Hide();
            }
            else
            {

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SqlCommand cmd5 = new SqlCommand("Select noId,namaBuku As namaBuku from libraData where namaBuku Like @namaBuku+'%'", conn);
            cmd5.Parameters.AddWithValue("noId", tb_cari.Text);
            cmd5.Parameters.AddWithValue("namaBuku", tb_cari.Text);
            cmd5.Parameters.AddWithValue("namaPenulis", tb_cari.Text);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd5;
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index;
            index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView1.Rows[index];
            tb_name.Text = selectedRow.Cells[1].Value.ToString();
            tb_penulis.Text = selectedRow.Cells[2].Value.ToString();
            tb_tanggal.Text = selectedRow.Cells[3].Value.ToString();
            tb_tentang.Text = selectedRow.Cells[4].Value.ToString();
            tb_penerbit.Text = selectedRow.Cells[5].Value.ToString();
        }

        private int id = 1;

        private void btn_submit_Click(object sender, EventArgs e)
        {
            id++;

            if (IsIdExists(id))
            {
                MessageBox.Show($"ID {id} already exists. Please choose a different ID.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO libraData(noId, namaBuku, namaPenulis, tanggalPenulisan, tentang, penerbit) VALUES(@noId, @namaBuku, @namaPenulis, @tanggalPenulisan, @tentang, @penerbit)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("noId", id);
                cmd.Parameters.AddWithValue("namaBuku", tb_name.Text);
                cmd.Parameters.AddWithValue("namaPenulis", tb_penulis.Text);
                cmd.Parameters.AddWithValue("tanggalPenulisan", tb_tanggal.Text);
                cmd.Parameters.AddWithValue("tentang", tb_tentang.Text);
                cmd.Parameters.AddWithValue("penerbit", tb_penerbit.Text);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data submitted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                    bind_data();
                }
            }
        }

        private bool IsIdExists(int id)
        {
            string query = "SELECT COUNT(*) FROM libraData WHERE noId = @noId";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@noId", id);
                try
                {
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error checking ID existence: " + ex.Message);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["noId"].Value);

                SqlCommand cmd4 = new SqlCommand("Update libraData set namaBuku=@namaBuku,namaPenulis=@namaPenulis,tanggalPenulisan=@tanggalPenulisan,tentang=@tentang,penerbit=@penerbit where noId=@noId", conn);
                cmd4.Parameters.AddWithValue("noId", selectedId);
                cmd4.Parameters.AddWithValue("namaBuku", tb_name.Text);
                cmd4.Parameters.AddWithValue("namaPenulis", tb_penulis.Text);
                cmd4.Parameters.AddWithValue("tanggalPenulisan", tb_tanggal.Text);
                cmd4.Parameters.AddWithValue("tentang", tb_tentang.Text);
                cmd4.Parameters.AddWithValue("penerbit", tb_penerbit.Text);

                try
                {
                    conn.Open();
                    cmd4.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating record: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            tb_penerbit.Text = string.Empty;
            tb_name.Text = string.Empty;
            tb_penulis.Text = string.Empty;
            tb_tanggal.Text = string.Empty;
            tb_tentang.Text = string.Empty;
            tb_penerbit.Text = string.Empty;
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the value from the first cell of the selected row
                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["noId"].Value);

                // Confirmation dialog
                DialogResult result = MessageBox.Show("Yakin ingin menghapus data ini?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Delete the record based on the selected id
                    SqlCommand cmd3 = new SqlCommand("DELETE FROM libraData WHERE noId = @noId", conn);
                    cmd3.Parameters.AddWithValue("@noId", selectedId);

                    try
                    {
                        conn.Open();
                        cmd3.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        conn.Close();
                        bind_data();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap imagebmp = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            dataGridView1.DrawToBitmap(imagebmp, new Rectangle(0, 0, imagebmp.Width, imagebmp.Height));
            e.Graphics.DrawImage(imagebmp, 120, 20);
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.PrintPreviewControl.Zoom = 1;
            printPreviewDialog1.ShowDialog();
        }
    }
}
