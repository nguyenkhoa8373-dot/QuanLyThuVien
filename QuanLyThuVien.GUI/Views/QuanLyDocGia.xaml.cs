
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
        // Su dung danh sach doc gia dung chung
        private ObservableCollection<DocGiaDTO> danhSachDocGia;

        public QuanLyDocGia()
        {
            InitializeComponent();

            // Lay du lieu tu kho du lieu chung
            danhSachDocGia = DuLieuDocGia.DanhSach;

            TaiDanhSachDocGia();
        }

        // 1. Hien thi danh sach doc gia
        private void TaiDanhSachDocGia()
        {
            dgDocGia.ItemsSource = null;

            dgDocGia.ItemsSource = danhSachDocGia
                .Where(dg => !dg.IsDeleted)
                .ToList();
        }

        // 2. Kiem tra thong tin nhap
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

            if (!string.IsNullOrWhiteSpace(soDienThoai) &&
                !soDienThoai.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại chỉ được nhập chữ số!");
                return false;
            }

            return true;
        }

        // 3. Them doc gia
        private void ThemDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm())
                return;

            string maDocGia = txtMaDocGia.Text.Trim();

            // Kiem tra trung ma
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

                SoDienThoai = string.IsNullOrWhiteSpace(txtSoDienThoai.Text)
                    ? null
                    : txtSoDienThoai.Text.Trim(),

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

        // 4. Sua thong tin doc gia
        private void SuaDocGia_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocGia.SelectedItem is not DocGiaDTO docGia)
            {
                MessageBox.Show("Vui lòng chọn độc giả cần sửa!");
                return;
            }

            if (!KiemTraForm())
                return;

            // Khong cho thay doi ma doc gia
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

        // 5. Xoa doc gia
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

            // Xoa mem
            docGia.IsDeleted = true;

            TaiDanhSachDocGia();

            NhapLai();

            txtThongBaoDG.Text = "Xóa độc giả thành công!";
        }

        // 6. Chon doc gia tren bang
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

                // Khong cho sua ma doc gia
                txtMaDocGia.IsReadOnly = true;
            }
        }

        // 7. Tim kiem doc gia
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

        // 8. Hien thi tat ca doc gia
        private void HienTatCaDG_Click(
            object sender,
            RoutedEventArgs e)
        {
            txtTimKiemDG.Clear();

            TaiDanhSachDocGia();

            txtThongBaoDG.Text = "Đã hiển thị tất cả độc giả.";
        }

        // 9. Nhap lai thong tin
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

    // Kho du lieu doc gia dung chung
    public static class DuLieuDocGia
    {
        public static ObservableCollection<DocGiaDTO> DanhSach { get; }
            = KhoiTaoDuLieu();

        private static ObservableCollection<DocGiaDTO> KhoiTaoDuLieu()
        {
            return new ObservableCollection<DocGiaDTO>
            {
                new DocGiaDTO
                {
                    MaDocGia = "DG001",
                    TenDocGia = "Nguyen Van An",
                    SoDienThoai = "0987654321",
                    DiaChi = "Ha Noi",
                    IsDeleted = false
                },

                new DocGiaDTO
                {
                    MaDocGia = "DG002",
                    TenDocGia = "Tran Thi Binh",
                    SoDienThoai = "0912345678",
                    DiaChi = "Vinh Phuc",
                    IsDeleted = false
                },

                new DocGiaDTO
                {
                    MaDocGia = "DG003",
                    TenDocGia = "Le Van Cuong",
                    SoDienThoai = "0978123456",
                    DiaChi = "Bac Ninh",
                    IsDeleted = false
                }
            };
        }
    }
}
