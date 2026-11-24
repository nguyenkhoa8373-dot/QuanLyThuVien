
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyDocGia : UserControl
    {
        // Kết nối tầng nghiệp vụ
        private readonly DocGiaBLL docGiaBLL = new DocGiaBLL();

        // Danh sách độc giả
        private List<DocGiaDTO> danhSachDocGia = new();

        public QuanLyDocGia()
        {
            InitializeComponent();

            // Tải dữ liệu khi mở màn hình
            TaiDanhSachDocGia();
        }

        // 1. Tải danh sách độc giả
        private void TaiDanhSachDocGia()
        {
            try
            {
                danhSachDocGia = docGiaBLL.LayDanhSachDocGia();
                dgDocGia.ItemsSource = danhSachDocGia;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách độc giả: " + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 2. Thêm độc giả
        private void ThemDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm())
                return;

            DocGiaDTO docGia = new DocGiaDTO
            {
                MaDocGia = txtMaDocGia.Text.Trim(),
                TenDocGia = txtHoTen.Text.Trim(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim()
            };

            bool ketQua = docGiaBLL.ThemDocGia(
                docGia, out string thongBao);

            txtThongBaoDG.Text = thongBao;

            if (ketQua)
            {
                TaiDanhSachDocGia();
                NhapLai();
                txtThongBaoDG.Text = thongBao;
            }
        }

        // 3. Sửa thông tin độc giả
        private void SuaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa!");
                return;
            }

            if (!KiemTraForm())
                return;

            // Giữ nguyên mã độc giả đã chọn
            docGia.TenDocGia = txtHoTen.Text.Trim();
            docGia.SoDienThoai = txtSoDienThoai.Text.Trim();
            docGia.DiaChi = txtDiaChi.Text.Trim();

            bool ketQua = docGiaBLL.SuaDocGia(
                docGia, out string thongBao);

            txtThongBaoDG.Text = thongBao;

            if (ketQua)
            {
                TaiDanhSachDocGia();
                NhapLai();
                txtThongBaoDG.Text = thongBao;
            }
        }

        // 4. Xóa độc giả
        private void XoaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần xóa!");
                return;
            }

            MessageBoxResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn xóa độc giả " +
                docGia.TenDocGia + "?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (xacNhan != MessageBoxResult.Yes)
                return;

            bool ketQua = docGiaBLL.XoaDocGia(
                docGia.MaDocGia, out string thongBao);

            txtThongBaoDG.Text = thongBao;

            if (ketQua)
            {
                TaiDanhSachDocGia();
                NhapLai();
                txtThongBaoDG.Text = thongBao;
            }
        }

        // 5. Kiểm tra dữ liệu nhập
        private bool KiemTraForm()
        {
            if (string.IsNullOrWhiteSpace(txtMaDocGia.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập mã độc giả và họ tên!");

                return false;
            }

            string soDienThoai = txtSoDienThoai.Text.Trim();

            if (soDienThoai.Length > 0 &&
                (soDienThoai.Length != 10 ||
                 soDienThoai[0] != '0' ||
                 !soDienThoai.All(char.IsDigit)))
            {
                MessageBox.Show(
                    "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0!");

                return false;
            }

            return true;
        }

        // 6. Chọn độc giả trên bảng
        private void dgDocGia_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (dgDocGia.SelectedItem is DocGiaDTO docGia)
            {
                txtMaDocGia.Text = docGia.MaDocGia;
                txtHoTen.Text = docGia.TenDocGia;
                txtSoDienThoai.Text = docGia.SoDienThoai;
                txtDiaChi.Text = docGia.DiaChi;

                // Không cho sửa mã khi đang cập nhật
                txtMaDocGia.IsReadOnly = true;
            }
        }

        // 7. Tìm kiếm độc giả
        private void TimKiemDG_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiemDG.Text.Trim();

                dgDocGia.ItemsSource = string.IsNullOrWhiteSpace(tuKhoa)
                    ? docGiaBLL.LayDanhSachDocGia()
                    : docGiaBLL.TimDocGia(tuKhoa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm: " + ex.Message);
            }
        }

        // 8. Hiển thị tất cả độc giả
        private void HienTatCaDG_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemDG.Clear();
            TaiDanhSachDocGia();
        }

        // 9. Nhập lại thông tin
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
    }
}
