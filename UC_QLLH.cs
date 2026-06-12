using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp01
{
    public partial class UC_QLLH : UserControl
    {
        public UC_QLLH()
        {
            InitializeComponent();
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
                string sql = "SELECT * FROM Classes";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvLopHoc.DataSource = dt;
            }
        }

        private void dgvLopHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
        }
    }
}
