
using System;
using System.Windows;
using System.Windows.Controls;
using QuanLyThuVien.DTO;
using QuanLyThuVien.BLL;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyDocGia : UserControl
    {
        private readonly DocGiaBLL docGiaBLL = new DocGiaBLL();

        public QuanLyDocGia()
        {
            InitializeComponent();
            LoadDocGia();
        }

        // 1. Tai danh sach doc gia tu SQL
        private void LoadDocGia()
        {
            try
            {
                dgDocGia.ItemsSource = docGiaBLL.LayDanhSachDocGia();
                txtThongBaoDG.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Khong the tai danh sach doc gia: " + ex.Message,
                    "Loi ket noi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 2. Kiem tra thong tin nhap
        private bool KiemTraForm()
        {
            string ma = txtMaDocGia.Text.Trim();
            string ten = txtHoTen.Text.Trim();
            string sdt = txtSoDienThoai.Text.Trim();

            if (string.IsNullOrWhiteSpace(ma))
            {
                MessageBox.Show("Vui long nhap ma doc gia!");
                txtMaDocGia.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Vui long nhap ho ten!");
                txtHoTen.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui long nhap so dien thoai!");
                txtSoDienThoai.Focus();
                return false;
            }

            if (ma.Length > 10 || ten.Length > 100)
            {
                MessageBox.Show("Thong tin vuot qua do dai cho phep!");
                return false;
            }

            if (sdt.Length != 10 ||
                !sdt.StartsWith("0") ||
                !long.TryParse(sdt, out _))
            {
                MessageBox.Show(
                    "So dien thoai phai gom 10 chu so va bat dau bang 0!");
                txtSoDienThoai.Focus();
                return false;
            }

            return true;
        }

        // 3. Them doc gia vao SQL
        private void ThemDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm())
                return;

            DocGiaDTO dg = new DocGiaDTO
            {
                MaDocGia = txtMaDocGia.Text.Trim(),
                TenDocGia = txtHoTen.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                IsDeleted = false
            };

            try
            {
                bool ketQua = docGiaBLL.ThemDocGia(
                    dg, out string thongBao);

                MessageBox.Show(thongBao);

                if (ketQua)
                {
                    LoadDocGia();
                    NhapLai();
                    txtThongBaoDG.Text = "Them doc gia thanh cong!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi them doc gia: " + ex.Message);
            }
        }

        // 4. Sua doc gia trong SQL
        private void SuaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO dgCu)
            {
                MessageBox.Show("Vui long chon doc gia can sua!");
                return;
            }

            if (!KiemTraForm())
                return;

            DocGiaDTO dgMoi = new DocGiaDTO
            {
                MaDocGia = dgCu.MaDocGia,
                TenDocGia = txtHoTen.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                IsDeleted = false
            };

            try
            {
                bool ketQua = docGiaBLL.SuaDocGia(
                    dgMoi, out string thongBao);

                MessageBox.Show(thongBao);

                if (ketQua)
                {
                    LoadDocGia();
                    NhapLai();
                    txtThongBaoDG.Text = "Cap nhat doc gia thanh cong!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi sua doc gia: " + ex.Message);
            }
        }

        // 5. Xoa doc gia trong SQL
        private void XoaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO dg)
            {
                MessageBox.Show("Vui long chon doc gia can xoa!");
                return;
            }

            MessageBoxResult xacNhan = MessageBox.Show(
                "Ban co chac chan muon xoa doc gia "
                + dg.TenDocGia + "?",
                "Xac nhan xoa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (xacNhan != MessageBoxResult.Yes)
                return;

            try
            {
                bool ketQua = docGiaBLL.XoaDocGia(
                    dg.MaDocGia, out string thongBao);

                MessageBox.Show(thongBao);

                if (ketQua)
                {
                    LoadDocGia();
                    NhapLai();
                    txtThongBaoDG.Text = "Xoa doc gia thanh cong!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi xoa doc gia: " + ex.Message);
            }
        }

        // 6. Nhap lai form
        private void NhapLai_Click(object sender, RoutedEventArgs e)
        {
            NhapLai();
        }

        private void NhapLai()
        {
            txtMaDocGia.Clear();
            txtHoTen.Clear();
            txtSoDienThoai.Clear();
            txtDiaChi.Clear();

            txtMaDocGia.IsReadOnly = false;

            dgDocGia.SelectedItem = null;

            txtThongBaoDG.Text = "";
        }

        // 7. Tim kiem doc gia
        private void TimKiemDG_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiemDG.Text.Trim();

                var ketQua = docGiaBLL.TimDocGia(tuKhoa);

                dgDocGia.ItemsSource = ketQua;

                txtThongBaoDG.Text =
                    "Tim thay " + ketQua.Count + " doc gia.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tim kiem: " + ex.Message);
            }
        }

        // 8. Hien thi tat ca doc gia
        private void HienTatCaDG_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemDG.Clear();

            LoadDocGia();

            txtThongBaoDG.Text = "Da hien thi tat ca doc gia.";
        }

        // 9. Chon doc gia tren DataGrid
        private void dgDocGia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgDocGia.SelectedItem is DocGiaDTO dg)
            {
                txtMaDocGia.Text = dg.MaDocGia;
                txtHoTen.Text = dg.TenDocGia;
                txtSoDienThoai.Text = dg.SoDienThoai ?? "";
                txtDiaChi.Text = dg.DiaChi ?? "";

                // Khong cho sua ma doc gia
                txtMaDocGia.IsReadOnly = true;
            }
        }
    }
}
