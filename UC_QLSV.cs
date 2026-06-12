using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp01
{
    public partial class UC_QLSV : UserControl
    {
        private int _pageSize = 10;
        private int _currentPage = 1;
        private int _totalRecords = 0;
        private string _editingMSSV = null;

        public UC_QLSV()
        {
            InitializeComponent();
            dgvSinhVien.CellClick += DgvSinhVien_CellClick;
            btn_edit.Click += Btn_edit_Click;
            btn_delete.Click += Btn_delete_Click;
            btn_clear.Click += Btn_clear_Click;
            btn_search.Click += Btn_search_Click;
            btn_head.Click += Btn_head_Click;
            button7.Click += Btn_prev_Click;
            button8.Click += Btn_next_Click;
            btn_tail.Click += Btn_tail_Click;

            dgvSinhVien.ReadOnly = true;
            dgvSinhVien.AllowUserToAddRows = false;
            dgvSinhVien.AllowUserToDeleteRows = false;
            dgvSinhVien.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void UC_QLSV_Load(object sender, EventArgs e)
        {
            dtpNgaySinh.Value = DateTime.Today;
            LoadLopHoc();
            LoadData();
        }

        private void LoadLopHoc()
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM Classes";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbxLopHoc.DataSource = dt;
                cbxLopHoc.DisplayMember = "ClassName";
                cbxLopHoc.ValueMember = "ClassId";
            }
        }

        private void LoadData()
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string keyword = textBox1.Text.Trim();
                string searchPattern = "%" + keyword + "%";

                MySqlCommand cmdCount = new MySqlCommand(
                    "SELECT COUNT(*) FROM Students WHERE IsDeleted = 0 " +
                    "AND (MSSV LIKE @search OR FullName LIKE @search OR ClassId LIKE @search)", conn);
                cmdCount.Parameters.AddWithValue("@search", searchPattern);
                _totalRecords = Convert.ToInt32(cmdCount.ExecuteScalar());

                int offset = (_currentPage - 1) * _pageSize;
                int totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalRecords / _pageSize));

                string sql = "SELECT MSSV, FullName, DateOfBirth, Gender, ClassId FROM Students " +
                    "WHERE IsDeleted = 0 AND (MSSV LIKE @search OR FullName LIKE @search OR ClassId LIKE @search) " +
                    "ORDER BY MSSV LIMIT @offset, @pageSize";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@search", searchPattern);
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@pageSize", _pageSize);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvSinhVien.DataSource = dt;

                label4.Text = $"Trang {_currentPage}/{totalPages} | {_totalRecords} bản ghi";
            }
        }

        private void ResetForm()
        {
            txt_mssv.Clear();
            txt_name.Clear();
            cboGioiTinh.SelectedIndex = -1;
            dtpNgaySinh.Value = DateTime.Today;
            cbxLopHoc.SelectedIndex = -1;
            txt_mssv.Enabled = true;
            _editingMSSV = null;
        }

        private void Btn_clear_Click(object sender, EventArgs e)
        {
            ResetForm();
            _currentPage = 1;
            textBox1.Clear();
            LoadData();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            string mssv = txt_mssv.Text.Trim();
            string name = txt_name.Text.Trim();
            string gioitinh = cboGioiTinh.Text.Trim();
            DateTime dateTime = dtpNgaySinh.Value;

            if (string.IsNullOrEmpty(mssv) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ MSSV và Họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbxLopHoc.SelectedValue == null)
            {
                MessageBox.Show("Danh sách lớp học đang trống hoặc bạn chưa chọn lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string classId = cbxLopHoc.SelectedValue.ToString();

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Students (MSSV, FullName, DateOfBirth, Gender, ClassId) VALUES (@mssv, @name, @dob, @gender, @classId)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mssv", mssv);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@dob", dateTime);
                cmd.Parameters.AddWithValue("@gender", gioitinh);
                cmd.Parameters.AddWithValue("@classId", classId);

                try
                {
                    cmd.ExecuteNonQuery();
                    ResetForm();
                    _currentPage = 1;
                    LoadData();
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
            txt_mssv.Text = row.Cells["MSSV"].Value?.ToString();
            txt_name.Text = row.Cells["FullName"].Value?.ToString();
            dtpNgaySinh.Value = row.Cells["DateOfBirth"].Value != DBNull.Value
                ? Convert.ToDateTime(row.Cells["DateOfBirth"].Value)
                : DateTime.Today;
            cboGioiTinh.Text = row.Cells["Gender"].Value?.ToString();

            if (row.Cells["ClassId"].Value != null)
            {
                cbxLopHoc.SelectedValue = row.Cells["ClassId"].Value.ToString();
            }

            txt_mssv.Enabled = false;
            _editingMSSV = txt_mssv.Text;
        }

        private void Btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_editingMSSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mssv = txt_mssv.Text.Trim();
            string name = txt_name.Text.Trim();
            string gioitinh = cboGioiTinh.Text.Trim();
            DateTime dateTime = dtpNgaySinh.Value;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập Họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbxLopHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string classId = cbxLopHoc.SelectedValue.ToString();

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Students SET FullName=@name, DateOfBirth=@dob, Gender=@gender, ClassId=@classId WHERE MSSV=@mssv";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@mssv", _editingMSSV);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@dob", dateTime);
                cmd.Parameters.AddWithValue("@gender", gioitinh);
                cmd.Parameters.AddWithValue("@classId", classId);

                try
                {
                    cmd.ExecuteNonQuery();
                    ResetForm();
                    LoadData();
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_editingMSSV))
            {
                MessageBox.Show("Vui lòng chọn sinh viên từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            DialogResult result2 = MessageBox.Show("Xóa mềm (ẩn) hay xóa cứng (khỏi CSDL)?\nChọn Yes = Xóa mềm, No = Xóa cứng", "Chọn kiểu xóa", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result2 == DialogResult.Cancel) return;

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                try
                {
                    if (result2 == DialogResult.Yes)
                    {
                        MySqlCommand cmd = new MySqlCommand("UPDATE Students SET IsDeleted=1 WHERE MSSV=@mssv", conn);
                        cmd.Parameters.AddWithValue("@mssv", _editingMSSV);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand("DELETE FROM Students WHERE MSSV=@mssv", conn);
                        cmd.Parameters.AddWithValue("@mssv", _editingMSSV);
                        cmd.ExecuteNonQuery();
                    }
                    ResetForm();
                    _currentPage = 1;
                    LoadData();
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Btn_search_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadData();
        }

        private void Btn_head_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            LoadData();
        }

        private void Btn_prev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void Btn_next_Click(object sender, EventArgs e)
        {
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalRecords / _pageSize));
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadData();
            }
        }

        private void Btn_tail_Click(object sender, EventArgs e)
        {
            _currentPage = Math.Max(1, (int)Math.Ceiling((double)_totalRecords / _pageSize));
            LoadData();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}
