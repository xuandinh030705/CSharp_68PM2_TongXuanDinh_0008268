using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp01
{
    public partial class UC_QLLH : UserControl
    {
        private string _editingClassId = null;

        public UC_QLLH()
        {
            InitializeComponent();
            dgvLopHoc.CellClick += DgvLopHoc_CellClick;
            btn_edit.Click += Btn_edit_Click;
            btn_delete.Click += Btn_delete_Click;
            btn_clear.Click += Btn_clear_Click;
            btn_search.Click += Btn_search_Click;
        }

        private void UC_QLLH_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string keyword = txtTimKiem.Text.Trim();
                string sql = string.IsNullOrEmpty(keyword)
                    ? "SELECT * FROM Classes"
                    : "SELECT * FROM Classes WHERE ClassId LIKE @search OR MaLop LIKE @search OR TenLop LIKE @search OR GhiChu LIKE @search";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(keyword))
                {
                    string p = "%" + keyword + "%";
                    cmd.Parameters.AddWithValue("@search", p);
                }
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvLopHoc.DataSource = dt;
            }
        }

        private void ResetForm()
        {
            txtMaID.Clear();
            txtMaLop.Clear();
            txtTenLop.Clear();
            txtGhiChu.Clear();
            txtMaID.Enabled = true;
            _editingClassId = null;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            string maID = txtMaID.Text.Trim();
            string maLop = txtMaLop.Text.Trim();
            string tenLop = txtTenLop.Text.Trim();
            string ghiChu = txtGhiChu.Text.Trim();

            if (string.IsNullOrEmpty(maID) || string.IsNullOrEmpty(maLop) || string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã ID, Mã lớp và Tên lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Classes (ClassId, MaLop, TenLop, GhiChu) VALUES (@maID, @maLop, @tenLop, @ghiChu)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@maID", maID);
                cmd.Parameters.AddWithValue("@maLop", maLop);
                cmd.Parameters.AddWithValue("@tenLop", tenLop);
                cmd.Parameters.AddWithValue("@ghiChu", ghiChu);

                try
                {
                    cmd.ExecuteNonQuery();
                    ResetForm();
                    LoadData();
                    MessageBox.Show("Thêm lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvLopHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvLopHoc.Rows[e.RowIndex];
            txtMaID.Text = row.Cells["ClassId"].Value?.ToString();
            txtMaLop.Text = row.Cells["MaLop"].Value?.ToString();
            txtTenLop.Text = row.Cells["TenLop"].Value?.ToString();
            txtGhiChu.Text = row.Cells["GhiChu"].Value?.ToString();
            txtMaID.Enabled = false;
            _editingClassId = txtMaID.Text;
        }

        private void Btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_editingClassId))
            {
                MessageBox.Show("Vui lòng chọn lớp học từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLop = txtMaLop.Text.Trim();
            string tenLop = txtTenLop.Text.Trim();
            string ghiChu = txtGhiChu.Text.Trim();

            if (string.IsNullOrEmpty(maLop) || string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp và Tên lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Classes SET MaLop=@maLop, TenLop=@tenLop, GhiChu=@ghiChu WHERE ClassId=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", _editingClassId);
                cmd.Parameters.AddWithValue("@maLop", maLop);
                cmd.Parameters.AddWithValue("@tenLop", tenLop);
                cmd.Parameters.AddWithValue("@ghiChu", ghiChu);

                try
                {
                    cmd.ExecuteNonQuery();
                    ResetForm();
                    LoadData();
                    MessageBox.Show("Cập nhật lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_editingClassId))
            {
                MessageBox.Show("Vui lòng chọn lớp học từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa lớp học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM Classes WHERE ClassId=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editingClassId);
                    cmd.ExecuteNonQuery();
                    ResetForm();
                    LoadData();
                    MessageBox.Show("Xóa lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Btn_clear_Click(object sender, EventArgs e)
        {
            ResetForm();
            txtTimKiem.Clear();
            LoadData();
        }

        private void Btn_search_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dgvLopHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
