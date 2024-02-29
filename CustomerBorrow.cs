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
    public partial class CustomerBorrow : Form
    {
        public CustomerBorrow()
        {
            InitializeComponent();
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

        private void btn_clear_Click(object sender, EventArgs e)
        {

            tb_name.Text = string.Empty;
            tb_penulis.Text = string.Empty;
            tb_tanggalpinjam.Text = string.Empty;
            tb_tanggalkembali.Text = string.Empty;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index;
            index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView1.Rows[index];
            tb_name.Text = selectedRow.Cells[1].Value.ToString();
            tb_penulis.Text = selectedRow.Cells[2].Value.ToString();
            tb_tanggalpinjam.Text = selectedRow.Cells[6].Value.ToString();
            tb_tanggalkembali.Text = selectedRow.Cells[7].Value.ToString();
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

                string connection = "Data Source=localhost;Initial Catalog=Libra;Integrated Security=True";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    string updateQuery = "UPDATE libraData SET tanggalPinjam = @tanggalPinjam, tanggalKembali = @tanggalKembali WHERE noId = @noId AND namaBuku = @namaBuku";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        // Assuming you have the values in your textboxes
                        cmd.Parameters.AddWithValue("@tanggalPinjam", tb_tanggalpinjam.Text);
                        cmd.Parameters.AddWithValue("@tanggalKembali", tb_tanggalkembali.Text);
                        if (int.TryParse(selectedRow.Cells["noId"].Value.ToString(), out int noIdValue))
                        {
                            cmd.Parameters.AddWithValue("@noId", noIdValue);
                        }
                        else
                        {
                            MessageBox.Show("Invalid 'noId' value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Stop execution if conversion fails
                        }

                        cmd.Parameters.AddWithValue("@namaBuku", selectedRow.Cells["namaBuku"].Value.ToString());

                        try
                        {
                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                bind_data();
                            }
                            else
                            {
                                MessageBox.Show("No matching record found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row before clicking the 'Submit' button.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
