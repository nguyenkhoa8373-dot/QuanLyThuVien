
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyDocGia : UserControl
    {
        ObservableCollection<DocGiaHienThi> danhSachDocGia = new();

        public QuanLyDocGia()
        {
            InitializeComponent();

            danhSachDocGia.Add(new DocGiaHienThi { MaDocGia = 1, HoTen = "Nguyen Van A", NgaySinh = "10/05/2005", GioiTinh = "Nam", SoDienThoai = "0912345678" });
            danhSachDocGia.Add(new DocGiaHienThi { MaDocGia = 2, HoTen = "Tran Thi B", NgaySinh = "20/08/2004", GioiTinh = "Nữ", SoDienThoai = "0987654321" });
            danhSachDocGia.Add(new DocGiaHienThi { MaDocGia = 3, HoTen = "Le Van C", NgaySinh = "15/03/2006", GioiTinh = "Nam", SoDienThoai = "0901234567" });

            dgDocGia.ItemsSource = danhSachDocGia;
        }

        private void ThemDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm()) return;

            int ma = danhSachDocGia.Count == 0 ? 1 : danhSachDocGia.Max(d => d.MaDocGia) + 1;

            danhSachDocGia.Add(new DocGiaHienThi
            {
                MaDocGia = ma,
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = txtNgaySinh.Text.Trim(),
                GioiTinh = (cbGioiTinh.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "",
                SoDienThoai = txtSoDienThoai.Text.Trim()
            });

            NhapLai();
            txtThongBaoDG.Text = "Thêm độc giả thành công!";
        }

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
            docGia.GioiTinh = (cbGioiTinh.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            docGia.SoDienThoai = txtSoDienThoai.Text.Trim();

            dgDocGia.Items.Refresh();
            NhapLai();
            txtThongBaoDG.Text = "Cập nhật độc giả thành công!";
        }

        private void XoaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaHienThi docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa độc giả này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                danhSachDocGia.Remove(docGia);
                NhapLai();
                txtThongBaoDG.Text = "Xóa độc giả thành công!";
            }
        }

        private bool KiemTraForm()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtNgaySinh.Text) ||
                string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin độc giả!");
                return false;
            }

            if (!System.DateTime.TryParseExact(txtNgaySinh.Text.Trim(),
                "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Ngày sinh phải có định dạng dd/MM/yyyy!");
                return false;
            }

            if (txtSoDienThoai.Text.Trim().Length < 9 ||
                !txtSoDienThoai.Text.Trim().All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!");
                return false;
            }

            return true;
        }

        private void dgDocGia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDocGia.SelectedItem is DocGiaHienThi docGia)
            {
                txtHoTen.Text = docGia.HoTen;
                txtNgaySinh.Text = docGia.NgaySinh;
                txtSoDienThoai.Text = docGia.SoDienThoai;

                foreach (ComboBoxItem item in cbGioiTinh.Items)
                {
                    if (item.Content.ToString() == docGia.GioiTinh)
                    {
                        cbGioiTinh.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void TimKiemDG_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiemDG.Text.Trim();

            dgDocGia.ItemsSource = string.IsNullOrWhiteSpace(tuKhoa)
                ? danhSachDocGia
                : danhSachDocGia.Where(d =>
                    d.HoTen.Contains(tuKhoa, System.StringComparison.OrdinalIgnoreCase) ||
                    d.SoDienThoai.Contains(tuKhoa) ||
                    d.MaDocGia.ToString().Contains(tuKhoa)).ToList();
        }

        private void HienTatCaDG_Click(object sender, RoutedEventArgs e)
        {
            dgDocGia.ItemsSource = danhSachDocGia;
            txtTimKiemDG.Clear();
        }

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

    public class DocGiaHienThi
    {
        public int MaDocGia { get; set; }
        public string HoTen { get; set; } = "";
        public string NgaySinh { get; set; } = "";
        public string GioiTinh { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
    }
}
