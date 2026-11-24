
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyDocGia : UserControl
    {
        // Danh sách độc giả tạm thời
        // Sau này sẽ thay bằng dữ liệu lấy từ SQL Server
        private ObservableCollection<DocGiaDTO> danhSachDocGia = new();

        public QuanLyDocGia()
        {
            InitializeComponent();

            // Hiển thị danh sách khi mở giao diện
            TaiDanhSachDocGia();
        }

        // 1. Hiển thị danh sách độc giả
        private void TaiDanhSachDocGia()
        {
            dgDocGia.ItemsSource = null;

            dgDocGia.ItemsSource = danhSachDocGia
                .Where(dg => !dg.IsDeleted)
                .ToList();
        }

        // 2. Kiểm tra thông tin nhập
        private bool KiemTraForm()
        {
            string maDocGia = txtMaDocGia.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();

            if (string.IsNullOrWhiteSpace(maDocGia))
            {
                MessageBox.Show("Vui lòng nhập mã độc giả!");
                txtMaDocGia.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Vui lòng nhập họ và tên!");
                txtHoTen.Focus();
                return false;
            }

            // Kiểm tra độ dài theo SQL
            if (maDocGia.Length > 10)
            {
                MessageBox.Show("Mã độc giả không được quá 10 ký tự!");
                return false;
            }

            if (hoTen.Length > 100)
            {
                MessageBox.Show("Họ tên không được quá 100 ký tự!");
                return false;
            }

            if (soDienThoai.Length > 15)
            {
                MessageBox.Show("Số điện thoại không được quá 15 ký tự!");
                return false;
            }

            if (diaChi.Length > 200)
            {
                MessageBox.Show("Địa chỉ không được quá 200 ký tự!");
                return false;
            }

            // SQL cho phép số điện thoại NULL.
            // Nếu có nhập thì chỉ nhận chữ số.
            if (!string.IsNullOrWhiteSpace(soDienThoai) &&
                !soDienThoai.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại chỉ được nhập chữ số!");
                return false;
            }

            return true;
        }

        // 3. Thêm độc giả
        private void ThemDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm())
                return;

            string maDocGia = txtMaDocGia.Text.Trim();

            // Kiểm tra mã độc giả đã tồn tại
            bool daTonTai = danhSachDocGia.Any(dg =>
                dg.MaDocGia.Equals(
                    maDocGia,
                    StringComparison.OrdinalIgnoreCase));

            if (daTonTai)
            {
                MessageBox.Show("Mã độc giả đã tồn tại!");
                return;
            }

            DocGiaDTO docGia = new DocGiaDTO
            {
                MaDocGia = maDocGia,
                TenDocGia = txtHoTen.Text.Trim(),

                // Cho phép bỏ trống số điện thoại
                SoDienThoai = string.IsNullOrWhiteSpace(txtSoDienThoai.Text)
                    ? null
                    : txtSoDienThoai.Text.Trim(),

                // Cho phép bỏ trống địa chỉ
                DiaChi = string.IsNullOrWhiteSpace(txtDiaChi.Text)
                    ? null
                    : txtDiaChi.Text.Trim(),

                IsDeleted = false
            };

            danhSachDocGia.Add(docGia);

            TaiDanhSachDocGia();
            NhapLai();

            txtThongBaoDG.Text = "Thêm độc giả thành công!";
        }

        // 4. Sửa thông tin độc giả
        private void SuaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa!");
                return;
            }

            if (!KiemTraForm())
                return;

            // Không cho thay đổi mã độc giả
            docGia.TenDocGia = txtHoTen.Text.Trim();

            docGia.SoDienThoai =
                string.IsNullOrWhiteSpace(txtSoDienThoai.Text)
                ? null
                : txtSoDienThoai.Text.Trim();

            docGia.DiaChi =
                string.IsNullOrWhiteSpace(txtDiaChi.Text)
                ? null
                : txtDiaChi.Text.Trim();

            TaiDanhSachDocGia();
            NhapLai();

            txtThongBaoDG.Text = "Cập nhật thông tin thành công!";
        }

        // 5. Xóa độc giả
        private void XoaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa!");
                return;
            }

            MessageBoxResult ketQua = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa độc giả "
                + docGia.TenDocGia + "?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua != MessageBoxResult.Yes)
                return;

            // Đánh dấu xóa tạm thời
            // Khi kết nối SQL sẽ chuyển thành IsDeleted = 1
            docGia.IsDeleted = true;

            TaiDanhSachDocGia();
            NhapLai();

            txtThongBaoDG.Text = "Xóa độc giả thành công!";
        }

        // 6. Chọn độc giả trên bảng
        private void dgDocGia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgDocGia.SelectedItem is DocGiaDTO docGia)
            {
                txtMaDocGia.Text = docGia.MaDocGia;
                txtHoTen.Text = docGia.TenDocGia;
                txtSoDienThoai.Text = docGia.SoDienThoai ?? "";
                txtDiaChi.Text = docGia.DiaChi ?? "";

                // Không cho sửa mã độc giả
                txtMaDocGia.IsReadOnly = true;
            }
        }

        // 7. Tìm kiếm độc giả
        private void TimKiemDG_Click(
            object sender,
            RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiemDG.Text.Trim();

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                TaiDanhSachDocGia();
                return;
            }

            var ketQua = danhSachDocGia
                .Where(dg => !dg.IsDeleted)
                .Where(dg =>
                    dg.MaDocGia.Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase)

                    || dg.TenDocGia.Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase)

                    || (dg.SoDienThoai ?? "").Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase)

                    || (dg.DiaChi ?? "").Contains(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            dgDocGia.ItemsSource = ketQua;

            txtThongBaoDG.Text =
                "Tìm thấy " + ketQua.Count + " độc giả.";
        }

        // 8. Hiển thị tất cả độc giả
        private void HienTatCaDG_Click(
            object sender,
            RoutedEventArgs e)
        {
            txtTimKiemDG.Clear();

            TaiDanhSachDocGia();

            txtThongBaoDG.Text = "Đã hiển thị tất cả độc giả.";
        }

        // 9. Nhập lại thông tin
        private void NhapLai_Click(
            object sender,
            RoutedEventArgs e)
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
    }
}
