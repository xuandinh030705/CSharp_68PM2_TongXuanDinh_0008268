using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp01
{
    public partial class Main : Form
    {
       
        private UC_QLLH uc_qllh = new UC_QLLH();
        private UC_QLSV uc_qlsv = new UC_QLSV();

        public Main()
        {
     
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
           
            ShowUserControl(uc_qlsv);
        }

      
        private void ShowUserControl(UserControl uc)
        {
            panelContainer.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(uc);
        }

        private void btnQLLH_Click(object sender, EventArgs e)
        {
            ShowUserControl(uc_qllh);
            DoiTrangThaiNut(btnQLLH);
        }

        private void btnQLSV_Click(object sender, EventArgs e)
        {
            ShowUserControl(uc_qlsv);
            DoiTrangThaiNut(btnQLSV);
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Dang Xuat a", "Thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

     
        private void DoiTrangThaiNut(Button activeBtn)
        {
            btnQLSV.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnQLLH.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            activeBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }
    }
}