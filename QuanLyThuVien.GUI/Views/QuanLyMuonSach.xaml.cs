
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyMuonSach : UserControl
    {
        // Dung chung du lieu giua cac lan mo man hinh
        private readonly ObservableCollection<SachDTO> danhSachSach
            = DuLieuSach.DanhSach;

        public QuanLyMuonSach()
        {
            InitializeComponent();

            // Hien thi danh sach sach
            TaiDanhSachSach();
        }

        // 1. Hien thi danh sach sach
        private void TaiDanhSachSach()
        {
            dgSach.ItemsSource = danhSachSach
                .Where(s => s.IsDeleted != true)
                .ToList();
        }

        // 2. Kiem tra du lieu nhap
        private bool KiemTraForm(out int soLuong)
        {
            soLuong = 0;

            string maSach = txtMaSach.Text.Trim();
            string tenSach = txtTenSach.Text.Trim();
            string theLoai = txtTheLoai.Text.Trim();

            if (string.IsNullOrWhiteSpace(maSach))
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

            if (maSach.Length > 10 ||
                tenSach.Length > 100 ||
                theLoai.Length > 50)
            {
                MessageBox.Show("Thong tin vuot qua do dai cho phep!");
                return false;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out soLuong)
                || soLuong < 0)
            {
                MessageBox.Show("So luong ton phai la so nguyen khong am!");
                txtSoLuong.Focus();
                return false;
            }

            if (soLuong > 100000)
            {
                MessageBox.Show("So luong ton qua lon!");
                return false;
            }

            return true;
        }

        // 3. Them sach
        private void ThemSach_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(out int soLuong))
                return;

            string maSach = txtMaSach.Text.Trim();

            bool daTonTai = danhSachSach.Any(s =>
                s.IsDeleted != true &&
                string.Equals(
                    s.MaSach,
                    maSach,
                    StringComparison.OrdinalIgnoreCase));

            if (daTonTai)
            {
                MessageBox.Show("Ma sach da ton tai!");
                return;
            }

            SachDTO sach = new SachDTO
            {
                MaSach = maSach,
                TenSach = txtTenSach.Text.Trim(),
                TheLoai = txtTheLoai.Text.Trim(),
                SoLuongTon = soLuong,
                IsDeleted = false
            };

            danhSachSach.Add(sach);

            TaiDanhSachSach();
            NhapLai();

            txtThongBao.Text = "Them sach thanh cong!";
        }

        // 4. Sua sach
        private void SuaSach_Click(object sender, RoutedEventArgs e)
        {
            if (dgSach.SelectedItem is not SachDTO sach)
            {
                MessageBox.Show("Vui long chon sach can sua!");
                return;
            }

            if (!KiemTraForm(out int soLuong))
                return;

            // Khong cho sua ma sach
            sach.TenSach = txtTenSach.Text.Trim();
            sach.TheLoai = txtTheLoai.Text.Trim();
            sach.SoLuongTon = soLuong;

            TaiDanhSachSach();
            NhapLai();

            txtThongBao.Text = "Cap nhat sach thanh cong!";
        }

        // 5. Xoa sach
        private void XoaSach_Click(object sender, RoutedEventArgs e)
        {
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

            // Danh dau da xoa, chua xoa khoi bo nho
            sach.IsDeleted = true;

            TaiDanhSachSach();
            NhapLai();

            txtThongBao.Text = "Xoa sach thanh cong!";
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
                txtSoLuong.Text =
                    (sach.SoLuongTon ?? 0).ToString();

                // Khong cho sua ma sach
                txtMaSach.IsReadOnly = true;
            }
        }

        // 7. Tim kiem sach
        private void TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            var ketQua = danhSachSach
                .Where(s => s.IsDeleted != true)
                .Where(s =>
                    string.IsNullOrWhiteSpace(tuKhoa) ||
                    s.MaSach.Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase) ||
                    s.TenSach.Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase) ||
                    (s.TheLoai ?? "").Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            dgSach.ItemsSource = ketQua;

            txtThongBao.Text =
                "Tim thay " + ketQua.Count + " sach.";
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

    // Du lieu sach mau dung chung trong GUI
    public static class DuLieuSach
    {
        public static ObservableCollection<SachDTO> DanhSach
            = new ObservableCollection<SachDTO>
        {
            new SachDTO
            {
                MaSach = "S001",
                TenSach = "Lap trinh C#",
                TheLoai = "Cong nghe",
                SoLuongTon = 10,
                IsDeleted = false
            },

            new SachDTO
            {
                MaSach = "S002",
                TenSach = "Lap trinh C++",
                TheLoai = "Cong nghe",
                SoLuongTon = 5,
                IsDeleted = false
            },

            new SachDTO
            {
                MaSach = "S003",
                TenSach = "Dac nhan tam",
                TheLoai = "Ky nang",
                SoLuongTon = 8,
                IsDeleted = false
            }
        };
    }
}
