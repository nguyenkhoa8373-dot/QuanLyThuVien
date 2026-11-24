using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyMuonSach : UserControl
    {
        private ObservableCollection<SachHienThi> danhSachSach = new();

        public QuanLyMuonSach()
        {
            InitializeComponent();

            // Du lieu mau, se thay bang du lieu SQL sau
            danhSachSach.Add(new SachHienThi
            {
                MaSach = "S001",
                TenSach = "Lap trinh C#",
                TheLoai = "Cong nghe",
                SoLuongTon = 10
            });

            danhSachSach.Add(new SachHienThi
            {
                MaSach = "S002",
                TenSach = "Lap trinh C++",
                TheLoai = "Cong nghe",
                SoLuongTon = 5
            });

            danhSachSach.Add(new SachHienThi
            {
                MaSach = "S003",
                TenSach = "Dac nhan tam",
                TheLoai = "Ky nang",
                SoLuongTon = 8
            });

            dgSach.ItemsSource = danhSachSach;
        }

        private bool KiemTraForm(out int soLuong)
        {
            soLuong = 0;

            if (string.IsNullOrWhiteSpace(txtMaSach.Text) ||
                string.IsNullOrWhiteSpace(txtTenSach.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sách và tên sách!");
                return false;
            }

            if (txtMaSach.Text.Trim().Length > 10 ||
                txtTenSach.Text.Trim().Length > 100 ||
                txtTheLoai.Text.Trim().Length > 50)
            {
                MessageBox.Show("Thông tin nhập vượt quá độ dài cho phép!");
                return false;
            }

            if (!int.TryParse(txtSoLuong.Text, out soLuong) || soLuong < 0)
            {
                MessageBox.Show("Số lượng tồn phải là số nguyên không âm!");
                return false;
            }

            return true;
        }

        private void ThemSach_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(out int soLuong)) return;

            string maSach = txtMaSach.Text.Trim();

            if (danhSachSach.Any(s => s.MaSach == maSach))
            {
                MessageBox.Show("Mã sách đã tồn tại!");
                return;
            }

            danhSachSach.Add(new SachHienThi
            {
                MaSach = maSach,
                TenSach = txtTenSach.Text.Trim(),
                TheLoai = txtTheLoai.Text.Trim(),
                SoLuongTon = soLuong
            });

            NhapLai();
            txtThongBao.Text = "Thêm sách thành công!";
        }

        private void SuaSach_Click(object sender, RoutedEventArgs e)
        {
            if (dgSach.SelectedItem is not SachHienThi sach)
            {
                MessageBox.Show("Vui lòng chọn sách cần sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenSach.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sách!");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong < 0)
            {
                MessageBox.Show("Số lượng tồn phải là số nguyên không âm!");
                return;
            }

            if (txtTenSach.Text.Trim().Length > 100 ||
                txtTheLoai.Text.Trim().Length > 50)
            {
                MessageBox.Show("Thông tin nhập vượt quá độ dài cho phép!");
                return;
            }

            sach.TenSach = txtTenSach.Text.Trim();
            sach.TheLoai = txtTheLoai.Text.Trim();
            sach.SoLuongTon = soLuong;

            dgSach.Items.Refresh();
            NhapLai();
            txtThongBao.Text = "Cập nhật sách thành công!";
        }

        private void XoaSach_Click(object sender, RoutedEventArgs e)
        {
            if (dgSach.SelectedItem is not SachHienThi sach)
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sách này?",
                "Xác nhận", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                danhSachSach.Remove(sach);
                NhapLai();
                txtThongBao.Text = "Xóa sách thành công!";
            }
        }

        private void dgSach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSach.SelectedItem is SachHienThi sach)
            {
                txtMaSach.Text = sach.MaSach;
                txtTenSach.Text = sach.TenSach;
                txtTheLoai.Text = sach.TheLoai;
                txtSoLuong.Text = sach.SoLuongTon.ToString();
            }
        }

        private void TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            dgSach.ItemsSource = string.IsNullOrWhiteSpace(tuKhoa)
                ? danhSachSach
                : danhSachSach.Where(s =>
                    s.MaSach.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase) ||
                    s.TenSach.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase) ||
                    s.TheLoai.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase))
                  .ToList();
        }

        private void HienTatCa_Click(object sender, RoutedEventArgs e)
        {
            dgSach.ItemsSource = danhSachSach;
            txtTimKiem.Clear();
        }

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
            dgSach.SelectedItem = null;
            txtThongBao.Text = "";
        }
    }

    public class SachHienThi
    {
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string TheLoai { get; set; } = "";
        public int SoLuongTon { get; set; }
    }
}