using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp01
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {

            String username = txt_username.Text;
            String password = txt_password.Text;

            if (username == "0008268@st.huce.edu.vn" && password == "0008268")
            {
                MessageBox.Show("Dnhap thanh cong");
                Form QLSinhVien  = new QLSinhVien();

              
                QLSinhVien.Show();

               
                this.Hide();
            }
            else
            {
                MessageBox.Show("Dang Nhap that bai");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
