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
        private ObservableCollection<PhieuMuonHienThi> danhSachPhieu = new();

        public QuanLyMuonTra()
        {
            InitializeComponent();

            // Du lieu mau, se ket noi SQL sau
            danhSachPhieu.Add(new PhieuMuonHienThi
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
            });

            danhSachPhieu.Add(new PhieuMuonHienThi
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
            });

            dgMuonTra.ItemsSource = danhSachPhieu;
        }

        private bool KiemTraForm(out DateTime ngayMuon, out DateTime hanTra,
                                 out int soLuong, out decimal tienPhat)
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
                MessageBox.Show("Vui lòng nhập đầy đủ mã phiếu, mã độc giả, mã sách và ngày mượn, hạn trả!");
                return false;
            }

            if (txtMaPhieu.Text.Trim().Length > 10 ||
                txtMaDocGia.Text.Trim().Length > 10 ||
                txtMaSach.Text.Trim().Length > 10)
            {
                MessageBox.Show("Mã phiếu, mã độc giả và mã sách không được vượt quá 10 ký tự!");
                return false;
            }

            if (!int.TryParse(txtSoLuongMuon.Text, out soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng mượn phải là số nguyên lớn hơn 0!");
                return false;
            }

            if (!DateTime.TryParseExact(txtNgayMuon.Text.Trim(), "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayMuon))
            {
                MessageBox.Show("Ngày mượn không hợp lệ! Hãy nhập dd/MM/yyyy.");
                return false;
            }

            if (!DateTime.TryParseExact(txtHanTra.Text.Trim(), "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out hanTra))
            {
                MessageBox.Show("Hạn trả không hợp lệ! Hãy nhập dd/MM/yyyy.");
                return false;
            }

            if (hanTra <= ngayMuon)
            {
                MessageBox.Show("Hạn trả phải sau ngày mượn!");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtTienPhat.Text) &&
                !decimal.TryParse(txtTienPhat.Text, out tienPhat))
            {
                MessageBox.Show("Tiền phạt không hợp lệ!");
                return false;
            }

            if (tienPhat < 0)
            {
                MessageBox.Show("Tiền phạt không được âm!");
                return false;
            }

            return true;
        }

        private void TaoPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(out DateTime ngayMuon, out DateTime hanTra,
                             out int soLuong, out decimal tienPhat))
                return;

            string maPhieu = txtMaPhieu.Text.Trim();

            if (danhSachPhieu.Any(p => p.MaPhieu == maPhieu))
            {
                MessageBox.Show("Mã phiếu đã tồn tại!");
                return;
            }

            danhSachPhieu.Add(new PhieuMuonHienThi
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
            });

            NhapLai();
            txtThongBaoMT.Text = "Tạo phiếu mượn thành công!";
        }

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

            phieu.NgayTra = DateTime.Today;
            phieu.TrangThai = "Đã trả";

            if (DateTime.Today > phieu.HanTra)
            {
                int soNgayTre = (DateTime.Today - phieu.HanTra).Days;
                phieu.TienPhat = soNgayTre * 5000m;
            }

            dgMuonTra.Items.Refresh();
            NhapLai();
            txtThongBaoMT.Text = "Xác nhận trả sách thành công!";
        }

        private void XoaPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is not PhieuMuonHienThi phieu)
            {
                MessageBox.Show("Vui lòng chọn phiếu cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa phiếu này?",
                "Xác nhận", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                danhSachPhieu.Remove(phieu);
                NhapLai();
                txtThongBaoMT.Text = "Xóa phiếu thành công!";
            }
        }

        private void dgMuonTra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is PhieuMuonHienThi phieu)
            {
                txtMaPhieu.Text = phieu.MaPhieu;
                txtMaDocGia.Text = phieu.MaDocGia;
                txtMaSach.Text = phieu.MaSach;
                txtSoLuongMuon.Text = phieu.SoLuongMuon.ToString();
                txtNgayMuon.Text = phieu.NgayMuon.ToString("dd/MM/yyyy");
                txtHanTra.Text = phieu.HanTra.ToString("dd/MM/yyyy");
                txtNgayTra.Text = phieu.NgayTra?.ToString("dd/MM/yyyy") ?? "";
                txtTienPhat.Text = phieu.TienPhat.ToString();
            }
        }

        private void TimKiemMT_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiemMT.Text.Trim();

            dgMuonTra.ItemsSource = string.IsNullOrWhiteSpace(tuKhoa)
                ? danhSachPhieu
                : danhSachPhieu.Where(p =>
                    p.MaPhieu.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.MaDocGia.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.MaSach.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.TrangThai.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase))
                  .ToList();
        }

        private void HienTatCaMT_Click(object sender, RoutedEventArgs e)
        {
            dgMuonTra.ItemsSource = danhSachPhieu;
            txtTimKiemMT.Clear();
        }

        private void NhapLai_Click(object sender, RoutedEventArgs e)
        {
            NhapLai();
        }

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
}