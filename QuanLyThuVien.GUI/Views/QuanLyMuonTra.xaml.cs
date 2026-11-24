
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyMuonTra : UserControl
    {
        // Su dung danh sach du lieu chung
        private ObservableCollection<PhieuMuonHienThi> danhSachPhieu;

        public QuanLyMuonTra()
        {
            InitializeComponent();

            // Lay du lieu tu kho du lieu chung
            danhSachPhieu = DuLieuPhieuMuon.DanhSach;

            dgMuonTra.ItemsSource = danhSachPhieu;
        }

        // Kiem tra du lieu nhap vao
        private bool KiemTraForm(
            out DateTime ngayMuon,
            out DateTime hanTra,
            out int soLuong,
            out decimal tienPhat)
        {
            ngayMuon = DateTime.MinValue;
            hanTra = DateTime.MinValue;
            soLuong = 0;
            tienPhat = 0;

            if (string.IsNullOrWhiteSpace(txtMaPhieu.Text) ||
                string.IsNullOrWhiteSpace(txtMaDocGia.Text) ||
                string.IsNullOrWhiteSpace(txtMaSach.Text) ||
                string.IsNullOrWhiteSpace(txtNgayMuon.Text) ||
                string.IsNullOrWhiteSpace(txtHanTra.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin phiếu mượn!");
                return false;
            }

            string maPhieu = txtMaPhieu.Text.Trim();
            string maDocGia = txtMaDocGia.Text.Trim();
            string maSach = txtMaSach.Text.Trim();

            if (maPhieu.Length > 10 ||
                maDocGia.Length > 10 ||
                maSach.Length > 10)
            {
                MessageBox.Show("Các mã không được vượt quá 10 ký tự!");
                return false;
            }

            if (!int.TryParse(txtSoLuongMuon.Text, out soLuong) ||
                soLuong <= 0)
            {
                MessageBox.Show("Số lượng mượn phải lớn hơn 0!");
                return false;
            }

            if (!DateTime.TryParseExact(
                txtNgayMuon.Text.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out ngayMuon))
            {
                MessageBox.Show("Ngày mượn không hợp lệ! Định dạng dd/MM/yyyy.");
                return false;
            }

            if (!DateTime.TryParseExact(
                txtHanTra.Text.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out hanTra))
            {
                MessageBox.Show("Hạn trả không hợp lệ! Định dạng dd/MM/yyyy.");
                return false;
            }

            if (hanTra <= ngayMuon)
            {
                MessageBox.Show("Hạn trả phải sau ngày mượn!");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtTienPhat.Text))
            {
                if (!decimal.TryParse(
                    txtTienPhat.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out tienPhat))
                {
                    MessageBox.Show("Tiền phạt không hợp lệ!");
                    return false;
                }
            }

            if (tienPhat < 0)
            {
                MessageBox.Show("Tiền phạt không được âm!");
                return false;
            }

            return true;
        }

        // Tao phieu muon
        private void TaoPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(
                out DateTime ngayMuon,
                out DateTime hanTra,
                out int soLuong,
                out decimal tienPhat))
            {
                return;
            }

            string maPhieu = txtMaPhieu.Text.Trim();

            // Kiem tra trung ma phieu
            if (danhSachPhieu.Any(p =>
                p.MaPhieu.Equals(maPhieu, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã phiếu đã tồn tại!");
                return;
            }

            // Tao doi tuong phieu muon
            PhieuMuonHienThi phieuMoi = new PhieuMuonHienThi
            {
                MaPhieu = maPhieu,
                MaDocGia = txtMaDocGia.Text.Trim(),
                MaSach = txtMaSach.Text.Trim(),
                SoLuongMuon = soLuong,
                NgayMuon = ngayMuon,
                HanTra = hanTra,
                NgayTra = null,
                TrangThai = "Đang mượn",
                TienPhat = tienPhat
            };

            danhSachPhieu.Add(phieuMoi);

            dgMuonTra.ItemsSource = danhSachPhieu;

            NhapLai();

            txtThongBaoMT.Text = "Tạo phiếu mượn thành công!";
        }

        // Xac nhan tra sach
        private void XacNhanTra_Click(object sender, RoutedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is not PhieuMuonHienThi phieu)
            {
                MessageBox.Show("Vui lòng chọn phiếu cần xác nhận trả!");
                return;
            }

            if (phieu.TrangThai == "Đã trả")
            {
                MessageBox.Show("Phiếu này đã được trả trước đó!");
                return;
            }

            DateTime ngayTra = DateTime.Today;

            phieu.NgayTra = ngayTra;
            phieu.TrangThai = "Đã trả";

            // Tinh tien phat neu tra muon
            if (ngayTra.Date > phieu.HanTra.Date)
            {
                int soNgayTre = (ngayTra.Date - phieu.HanTra.Date).Days;

                phieu.TienPhat = soNgayTre * 5000m;
            }
            else
            {
                phieu.TienPhat = 0;
            }

            dgMuonTra.Items.Refresh();

            NhapLai();

            txtThongBaoMT.Text = "Xác nhận trả sách thành công!";
        }

        // Xoa phieu muon
        private void XoaPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is not PhieuMuonHienThi phieu)
            {
                MessageBox.Show("Vui lòng chọn phiếu cần xóa!");
                return;
            }

            MessageBoxResult ketQua = MessageBox.Show(
                "Bạn có chắc muốn xóa phiếu này?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua == MessageBoxResult.Yes)
            {
                danhSachPhieu.Remove(phieu);

                NhapLai();

                txtThongBaoMT.Text = "Xóa phiếu thành công!";
            }
        }

        // Khi chon mot phieu trong bang
        private void dgMuonTra_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is PhieuMuonHienThi phieu)
            {
                txtMaPhieu.Text = phieu.MaPhieu;
                txtMaDocGia.Text = phieu.MaDocGia;
                txtMaSach.Text = phieu.MaSach;

                txtSoLuongMuon.Text = phieu.SoLuongMuon.ToString();

                txtNgayMuon.Text =
                    phieu.NgayMuon.ToString("dd/MM/yyyy");

                txtHanTra.Text =
                    phieu.HanTra.ToString("dd/MM/yyyy");

                txtNgayTra.Text =
                    phieu.NgayTra?.ToString("dd/MM/yyyy") ?? "";

                txtTienPhat.Text =
                    phieu.TienPhat.ToString("N0");
            }
        }

        // Tim kiem phieu muon
        private void TimKiemMT_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiemMT.Text.Trim();

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                dgMuonTra.ItemsSource = danhSachPhieu;
                return;
            }

            var ketQua = danhSachPhieu.Where(p =>
                p.MaPhieu.Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase) ||

                p.MaDocGia.Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase) ||

                p.MaSach.Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase) ||

                p.TrangThai.Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            dgMuonTra.ItemsSource = ketQua;
        }

        // Hien thi tat ca phieu muon
        private void HienTatCaMT_Click(object sender, RoutedEventArgs e)
        {
            dgMuonTra.ItemsSource = danhSachPhieu;

            txtTimKiemMT.Clear();
        }

        // Nut nhap lai
        private void NhapLai_Click(object sender, RoutedEventArgs e)
        {
            NhapLai();
        }

        // Xoa thong tin tren form
        private void NhapLai()
        {
            txtMaPhieu.Clear();
            txtMaDocGia.Clear();
            txtMaSach.Clear();
            txtSoLuongMuon.Clear();
            txtNgayMuon.Clear();
            txtHanTra.Clear();
            txtNgayTra.Clear();
            txtTienPhat.Clear();

            dgMuonTra.SelectedItem = null;

            txtThongBaoMT.Text = "";
        }
    }

    // Lop du lieu phieu muon
    public class PhieuMuonHienThi
    {
        public string MaPhieu { get; set; } = "";

        public string MaDocGia { get; set; } = "";

        public string MaSach { get; set; } = "";

        public int SoLuongMuon { get; set; }

        public DateTime NgayMuon { get; set; }

        public DateTime HanTra { get; set; }

        public DateTime? NgayTra { get; set; }

        public string TrangThai { get; set; } = "";

        public decimal TienPhat { get; set; }
    }

    // Kho du lieu phieu muon dung chung
    public static class DuLieuPhieuMuon
    {
        public static ObservableCollection<PhieuMuonHienThi> DanhSach { get; }
            = KhoiTaoDuLieu();

        private static ObservableCollection<PhieuMuonHienThi> KhoiTaoDuLieu()
        {
            return new ObservableCollection<PhieuMuonHienThi>
            {
                new PhieuMuonHienThi
                {
                    MaPhieu = "PM001",
                    MaDocGia = "DG001",
                    MaSach = "S001",
                    SoLuongMuon = 1,
                    NgayMuon = new DateTime(2026, 11, 1),
                    HanTra = new DateTime(2026, 11, 10),
                    NgayTra = null,
                    TrangThai = "Đang mượn",
                    TienPhat = 0
                },

                new PhieuMuonHienThi
                {
                    MaPhieu = "PM002",
                    MaDocGia = "DG002",
                    MaSach = "S002",
                    SoLuongMuon = 1,
                    NgayMuon = new DateTime(2026, 10, 20),
                    HanTra = new DateTime(2026, 10, 30),
                    NgayTra = new DateTime(2026, 10, 28),
                    TrangThai = "Đã trả",
                    TienPhat = 0
                }
            };
        }
    }
}
