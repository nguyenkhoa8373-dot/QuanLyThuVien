
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyMuonSach : UserControl
    {
        ObservableCollection<SachHienThi> danhSachSach = new();

        public QuanLyMuonSach()
        {
            InitializeComponent();

            danhSachSach.Add(new SachHienThi { MaSach = 1, TenSach = "Lap trinh C#", TacGia = "Nguyen Van A", TheLoai = "Cong nghe", SoLuong = 10 });
            danhSachSach.Add(new SachHienThi { MaSach = 2, TenSach = "Lap trinh C++", TacGia = "Nguyen Van B", TheLoai = "Cong nghe", SoLuong = 5 });
            danhSachSach.Add(new SachHienThi { MaSach = 3, TenSach = "Dac nhan tam", TacGia = "Dale Carnegie", TheLoai = "Ky nang", SoLuong = 8 });

            dgSach.ItemsSource = danhSachSach;
        }

        private void ThemSach_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(out int soLuong)) return;

            int ma = danhSachSach.Count == 0 ? 1 : danhSachSach.Max(s => s.MaSach) + 1;

            danhSachSach.Add(new SachHienThi
            {
                MaSach = ma,
                TenSach = txtTenSach.Text.Trim(),
                TacGia = txtTacGia.Text.Trim(),
                TheLoai = txtTheLoai.Text.Trim(),
                SoLuong = soLuong
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

            if (!KiemTraForm(out int soLuong)) return;

            sach.TenSach = txtTenSach.Text.Trim();
            sach.TacGia = txtTacGia.Text.Trim();
            sach.TheLoai = txtTheLoai.Text.Trim();
            sach.SoLuong = soLuong;

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

            if (MessageBox.Show("Bạn có chắc muốn xóa sách này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                danhSachSach.Remove(sach);
                NhapLai();
                txtThongBao.Text = "Xóa sách thành công!";
            }
        }

        private bool KiemTraForm(out int soLuong)
        {
            soLuong = 0;

            if (string.IsNullOrWhiteSpace(txtTenSach.Text) ||
                string.IsNullOrWhiteSpace(txtTacGia.Text) ||
                string.IsNullOrWhiteSpace(txtTheLoai.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sách!");
                return false;
            }

            if (!int.TryParse(txtSoLuong.Text, out soLuong) || soLuong < 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên không âm!");
                return false;
            }

            return true;
        }

        private void dgSach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSach.SelectedItem is SachHienThi sach)
            {
                txtTenSach.Text = sach.TenSach;
                txtTacGia.Text = sach.TacGia;
                txtTheLoai.Text = sach.TheLoai;
                txtSoLuong.Text = sach.SoLuong.ToString();
            }
        }

        private void TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            dgSach.ItemsSource = string.IsNullOrWhiteSpace(tuKhoa)
                ? danhSachSach
                : danhSachSach.Where(s =>
                    s.TenSach.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase) ||
                    s.TacGia.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase) ||
                    s.TheLoai.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase)).ToList();
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
            txtTenSach.Clear();
            txtTacGia.Clear();
            txtTheLoai.Clear();
            txtSoLuong.Clear();
            dgSach.SelectedItem = null;
            txtThongBao.Text = "";
        }
    }

    public class SachHienThi
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = "";
        public string TacGia { get; set; } = "";
        public string TheLoai { get; set; } = "";
        public int SoLuong { get; set; }
    }
}
