using System;
using System.Windows;
using System.Windows.Controls;
using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI.Views
{
    // CANH BAO: Ten class la QuanLyMuonSach nhung noi dung la CRUD sach.
    // Neu day la form quan ly kho sach, nen doi ten thanh QuanLySach.
    // Neu day la form muon/tra, can viet lai hoan toan voi MuonTraBLL.
    public partial class QuanLyMuonSach : UserControl
    {
        private readonly SachBLL sachBLL = new SachBLL();

        public QuanLyMuonSach()
        {
            InitializeComponent();
            TaiDanhSachSach();
        }

        // 1. Tai danh sach sach tu SQL
        private void TaiDanhSachSach()
        {
            try
            {
                // SỬA: reset ItemsSource truoc khi gan
                dgSach.ItemsSource = null;
                dgSach.ItemsSource = sachBLL.LayDanhSachSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Khong the tai danh sach sach!\n" + ex.Message,
                    "Loi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 2. Kiem tra du lieu nhap
        // SỬA: them tham so kiemTraMa
        private bool KiemTraForm(out int soLuong, bool kiemTraMa = true)
        {
            soLuong = 0;

            string maSach = txtMaSach.Text.Trim();
            string tenSach = txtTenSach.Text.Trim();
            string theLoai = txtTheLoai.Text.Trim();
            string soLuongText = txtSoLuong.Text.Trim();

            if (kiemTraMa && string.IsNullOrWhiteSpace(maSach))
            {
                MessageBox.Show("Vui long nhap ma sach!");
                txtMaSach.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tenSach))
            {
                MessageBox.Show("Vui long nhap ten sach!");
                txtTenSach.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(theLoai))
            {
                MessageBox.Show("Vui long nhap the loai!");
                txtTheLoai.Focus();
                return false;
            }

            // SỬA: tach kiem tra do dai ma rieng
            if (kiemTraMa && maSach.Length > 10)
            {
                MessageBox.Show("Ma sach khong duoc vuot qua 10 ky tu!");
                txtMaSach.Focus();
                return false;
            }

            if (tenSach.Length > 100 || theLoai.Length > 50)
            {
                MessageBox.Show("Thong tin vuot qua do dai cho phep!");
                return false;
            }

            // SỬA: kiem tra rong truoc khi parse
            if (string.IsNullOrWhiteSpace(soLuongText))
            {
                MessageBox.Show("Vui long nhap so luong ton!");
                txtSoLuong.Focus();
                return false;
            }

            if (!int.TryParse(soLuongText, out soLuong) || soLuong < 0)
            {
                MessageBox.Show("So luong ton phai la so nguyen khong am!");
                txtSoLuong.Focus();
                return false;
            }

            if (soLuong > 100000)
            {
                MessageBox.Show("So luong ton qua lon!");
                txtSoLuong.Focus();
                return false;
            }

            return true;
        }

        // 3. Them sach vao SQL
        private void ThemSach_Click(object sender, RoutedEventArgs e)
        {
            // SỬA: reset thong bao
            txtThongBao.Text = "";

            if (!KiemTraForm(out int soLuong))
                return;

            SachDTO sach = new SachDTO
            {
                MaSach = txtMaSach.Text.Trim(),
                TenSach = txtTenSach.Text.Trim(),
                TheLoai = txtTheLoai.Text.Trim(),
                SoLuongTon = soLuong,
                IsDeleted = false
            };

            try
            {
                bool ketQua = sachBLL.ThemSach(sach, out string thongBao);

                MessageBox.Show(thongBao);

                if (ketQua)
                {
                    TaiDanhSachSach();
                    NhapLai();
                    txtThongBao.Text = "Them sach thanh cong!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi khi them sach: " + ex.Message,
                    "Loi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 4. Sua sach trong SQL
        private void SuaSach_Click(object sender, RoutedEventArgs e)
        {
            // SỬA: reset thong bao
            txtThongBao.Text = "";

            if (dgSach.SelectedItem is not SachDTO sachCu)
            {
                MessageBox.Show("Vui long chon sach can sua!");
                return;
            }

            // SỬA: kiem tra an toan ma sach
            if (txtMaSach.Text.Trim() != sachCu.MaSach)
            {
                MessageBox.Show("Khong duoc sua ma sach!");
                txtMaSach.Text = sachCu.MaSach;
                return;
            }

            // SỬA: khi Sua thi khong can kiem tra lai ma
            if (!KiemTraForm(out int soLuong, kiemTraMa: false))
                return;

            SachDTO sachMoi = new SachDTO
            {
                MaSach = sachCu.MaSach,
                TenSach = txtTenSach.Text.Trim(),
                TheLoai = txtTheLoai.Text.Trim(),
                SoLuongTon = soLuong,
                IsDeleted = false
            };

            try
            {
                bool ketQua = sachBLL.SuaSach(sachMoi, out string thongBao);

                MessageBox.Show(thongBao);

                if (ketQua)
                {
                    TaiDanhSachSach();
                    NhapLai();
                    txtThongBao.Text = "Cap nhat sach thanh cong!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi khi sua sach: " + ex.Message,
                    "Loi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 5. Xoa sach trong SQL
        private void XoaSach_Click(object sender, RoutedEventArgs e)
        {
            // SỬA: reset thong bao
            txtThongBao.Text = "";

            if (dgSach.SelectedItem is not SachDTO sach)
            {
                MessageBox.Show("Vui long chon sach can xoa!");
                return;
            }

            MessageBoxResult ketQua = MessageBox.Show(
                "Ban co chac chan muon xoa sach "
                + sach.TenSach + "?",
                "Xac nhan xoa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua != MessageBoxResult.Yes)
                return;

            try
            {
                bool thanhCong = sachBLL.XoaSach(
                    sach.MaSach,
                    out string thongBao);

                MessageBox.Show(thongBao);

                if (thanhCong)
                {
                    TaiDanhSachSach();
                    NhapLai();
                    txtThongBao.Text = "Xoa sach thanh cong!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi khi xoa sach: " + ex.Message,
                    "Loi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 6. Chon sach tren bang
        private void dgSach_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgSach.SelectedItem is SachDTO sach)
            {
                txtMaSach.Text = sach.MaSach;
                txtTenSach.Text = sach.TenSach;
                txtTheLoai.Text = sach.TheLoai ?? "";
                txtSoLuong.Text = (sach.SoLuongTon ?? 0).ToString();

                // Khong cho sua ma sach
                txtMaSach.IsReadOnly = true;
            }
            else
            {
                // SỬA: khi bo chon thi mo khoa lai ma
                txtMaSach.IsReadOnly = false;
            }
        }

        // 7. Tim kiem sach trong SQL
        private void TimKiem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                // SỬA: neu tu khoa rong thi hien thi tat ca
                if (string.IsNullOrWhiteSpace(tuKhoa))
                {
                    TaiDanhSachSach();
                    txtThongBao.Text = "Da hien thi tat ca sach.";
                    return;
                }

                var ketQua = sachBLL.TimSach(tuKhoa);

                // SỬA: reset ItemsSource truoc khi gan
                dgSach.ItemsSource = null;
                dgSach.ItemsSource = ketQua;

                txtThongBao.Text =
                    "Tim thay " + ketQua.Count + " sach.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi tim kiem sach: " + ex.Message,
                    "Loi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 8. Hien thi tat ca sach
        private void HienTatCa_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiem.Clear();
            TaiDanhSachSach();
            txtThongBao.Text = "Da hien thi tat ca sach.";
        }

        // 9. Nhap lai
        private void NhapLai_Click(object sender, RoutedEventArgs e)
        {
            NhapLai();
        }

        private void NhapLai()
        {
            txtMaSach.Clear();
            txtTenSach.Clear();
            txtTheLoai.Clear();
            txtSoLuong.Clear();

            txtMaSach.IsReadOnly = false;

            dgSach.SelectedItem = null;

            txtThongBao.Text = "";
        }
    }
}