
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyDocGia : UserControl
    {
        private ObservableCollection<DocGiaHienThi> danhSachDocGia = new();

        public QuanLyDocGia()
        {
            InitializeComponent();

            // Dữ liệu mẫu ban đầu
            danhSachDocGia.Add(new DocGiaHienThi
            {
                MaDocGia = 1,
                HoTen = "Nguyen Van A",
                NgaySinh = "10/05/2005",
                GioiTinh = "Nam",
                SoDienThoai = "0912345678"
            });

            danhSachDocGia.Add(new DocGiaHienThi
            {
                MaDocGia = 2,
                HoTen = "Tran Thi B",
                NgaySinh = "20/08/2004",
                GioiTinh = "Nữ",
                SoDienThoai = "0987654321"
            });

            danhSachDocGia.Add(new DocGiaHienThi
            {
                MaDocGia = 3,
                HoTen = "Le Van C",
                NgaySinh = "15/03/2006",
                GioiTinh = "Nam",
                SoDienThoai = "0901234567"
            });

            HienThiDanhSach();
        }

        // Hiển thị danh sách độc giả
        private void HienThiDanhSach()
        {
            string tuKhoa = txtTimKiemDG.Text.Trim();

            var ketQua = danhSachDocGia.Where(d =>
                d.HoTen.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                d.SoDienThoai.Contains(tuKhoa) ||
                d.MaDocGia.ToString().Contains(tuKhoa)
            ).ToList();

            dgDocGia.ItemsSource = ketQua;
        }

        // Thêm độc giả
        private void ThemDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm()) return;

            int ma = danhSachDocGia.Count == 0
                ? 1
                : danhSachDocGia.Max(d => d.MaDocGia) + 1;

            danhSachDocGia.Add(new DocGiaHienThi
            {
                MaDocGia = ma,
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = txtNgaySinh.Text.Trim(),
                GioiTinh = (cbGioiTinh.SelectedItem as ComboBoxItem)
                    ?.Content?.ToString() ?? "",
                SoDienThoai = txtSoDienThoai.Text.Trim()
            });

            txtTimKiemDG.Clear();
            HienThiDanhSach();
            NhapLai();

            txtThongBaoDG.Text = "Thêm độc giả thành công!";
        }

        // Sửa thông tin độc giả
        private void SuaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaHienThi docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa!");
                return;
            }

            if (!KiemTraForm()) return;

            docGia.HoTen = txtHoTen.Text.Trim();
            docGia.NgaySinh = txtNgaySinh.Text.Trim();
            docGia.GioiTinh = (cbGioiTinh.SelectedItem as ComboBoxItem)
                ?.Content?.ToString() ?? "";
            docGia.SoDienThoai = txtSoDienThoai.Text.Trim();

            txtTimKiemDG.Clear();
            HienThiDanhSach();
            NhapLai();

            txtThongBaoDG.Text = "Cập nhật độc giả thành công!";
        }

        // Xóa độc giả
        private void XoaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaHienThi docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa!");
                return;
            }

            MessageBoxResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn xóa độc giả này?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (xacNhan == MessageBoxResult.Yes)
            {
                danhSachDocGia.Remove(docGia);

                HienThiDanhSach();
                NhapLai();

                txtThongBaoDG.Text = "Xóa độc giả thành công!";
            }
        }

        // Kiểm tra thông tin nhập
        private bool KiemTraForm()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtNgaySinh.Text) ||
                string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin độc giả!");
                return false;
            }

            if (!DateTime.TryParseExact(
                txtNgaySinh.Text.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
            {
                MessageBox.Show("Ngày sinh phải có định dạng dd/MM/yyyy!");
                return false;
            }

            string sdt = txtSoDienThoai.Text.Trim();

            if (sdt.Length != 10 ||
                sdt[0] != '0' ||
                !sdt.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0!");
                return false;
            }

            return true;
        }

        // Chọn độc giả trên bảng
        private void dgDocGia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgDocGia.SelectedItem is DocGiaHienThi docGia)
            {
                txtHoTen.Text = docGia.HoTen;
                txtNgaySinh.Text = docGia.NgaySinh;
                txtSoDienThoai.Text = docGia.SoDienThoai;

                foreach (ComboBoxItem item in cbGioiTinh.Items)
                {
                    if (item.Content?.ToString() == docGia.GioiTinh)
                    {
                        cbGioiTinh.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // Tìm kiếm độc giả
        private void TimKiemDG_Click(object sender, RoutedEventArgs e)
        {
            HienThiDanhSach();
        }

        // Hiển thị toàn bộ độc giả
        private void HienTatCaDG_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemDG.Clear();
            HienThiDanhSach();
        }

        // Nhập lại thông tin
        private void NhapLai_Click(object sender, RoutedEventArgs e)
        {
            NhapLai();
        }

        private void NhapLai()
        {
            txtHoTen.Clear();
            txtNgaySinh.Clear();
            txtSoDienThoai.Clear();

            cbGioiTinh.SelectedIndex = 0;

            dgDocGia.SelectedItem = null;

            txtThongBaoDG.Text = "";
        }
    }

    // Model hiển thị độc giả trên giao diện
    public class DocGiaHienThi
    {
        public int MaDocGia { get; set; }

        public string HoTen { get; set; } = "";

        public string NgaySinh { get; set; } = "";

        public string GioiTinh { get; set; } = "";

        public string SoDienThoai { get; set; } = "";
    }
}
