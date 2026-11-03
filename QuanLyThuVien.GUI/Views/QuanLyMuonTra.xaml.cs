
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyMuonTra : UserControl
    {
        ObservableCollection<PhieuMuonHienThi> danhSachPhieu = new();

        public QuanLyMuonTra()
        {
            InitializeComponent();

            danhSachPhieu.Add(new PhieuMuonHienThi
            {
                MaPhieu = 1,
                MaDocGia = "DG001",
                TenDocGia = "Nguyen Van A",
                TenSach = "Lap trinh C#",
                NgayMuon = "01/11/2026",
                NgayHenTra = "10/11/2026",
                TrangThai = "Đang mượn"
            });

            danhSachPhieu.Add(new PhieuMuonHienThi
            {
                MaPhieu = 2,
                MaDocGia = "DG002",
                TenDocGia = "Tran Thi B",
                TenSach = "Lap trinh C++",
                NgayMuon = "20/10/2026",
                NgayHenTra = "30/10/2026",
                TrangThai = "Đã trả"
            });

            dgMuonTra.ItemsSource = danhSachPhieu;
        }

        private void TaoPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(out DateTime ngayMuon, out DateTime ngayHenTra))
                return;

            int ma = danhSachPhieu.Count == 0
                ? 1
                : danhSachPhieu.Max(p => p.MaPhieu) + 1;

            danhSachPhieu.Add(new PhieuMuonHienThi
            {
                MaPhieu = ma,
                MaDocGia = txtMaDocGia.Text.Trim(),
                TenDocGia = txtTenDocGia.Text.Trim(),
                TenSach = txtTenSach.Text.Trim(),
                NgayMuon = ngayMuon.ToString("dd/MM/yyyy"),
                NgayHenTra = ngayHenTra.ToString("dd/MM/yyyy"),
                TrangThai = "Đang mượn"
            });

            NhapLai();
            txtThongBaoMT.Text = "Tạo phiếu mượn thành công!";
        }

        private void XacNhanTra_Click(object sender, RoutedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is not PhieuMuonHienThi phieu)
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn cần xác nhận trả!");
                return;
            }

            if (phieu.TrangThai == "Đã trả")
            {
                MessageBox.Show("Sách này đã được trả trước đó!");
                return;
            }

            phieu.TrangThai = "Đã trả";
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

            if (MessageBox.Show("Bạn có chắc muốn xóa phiếu này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                danhSachPhieu.Remove(phieu);
                NhapLai();
                txtThongBaoMT.Text = "Xóa phiếu thành công!";
            }
        }

        private bool KiemTraForm(out DateTime ngayMuon, out DateTime ngayHenTra)
        {
            ngayMuon = DateTime.MinValue;
            ngayHenTra = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(txtMaDocGia.Text) ||
                string.IsNullOrWhiteSpace(txtTenDocGia.Text) ||
                string.IsNullOrWhiteSpace(txtTenSach.Text) ||
                string.IsNullOrWhiteSpace(txtNgayMuon.Text) ||
                string.IsNullOrWhiteSpace(txtNgayHenTra.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin phiếu mượn!");
                return false;
            }

            if (!DateTime.TryParseExact(txtNgayMuon.Text.Trim(), "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out ngayMuon))
            {
                MessageBox.Show("Ngày mượn không hợp lệ! Hãy nhập dd/MM/yyyy.");
                return false;
            }

            if (!DateTime.TryParseExact(txtNgayHenTra.Text.Trim(), "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out ngayHenTra))
            {
                MessageBox.Show("Ngày hẹn trả không hợp lệ! Hãy nhập dd/MM/yyyy.");
                return false;
            }

            if (ngayHenTra <= ngayMuon)
            {
                MessageBox.Show("Ngày hẹn trả phải sau ngày mượn!");
                return false;
            }

            return true;
        }

        private void dgMuonTra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is PhieuMuonHienThi phieu)
            {
                txtMaDocGia.Text = phieu.MaDocGia;
                txtTenDocGia.Text = phieu.TenDocGia;
                txtTenSach.Text = phieu.TenSach;
                txtNgayMuon.Text = phieu.NgayMuon;
                txtNgayHenTra.Text = phieu.NgayHenTra;
            }
        }

        private void TimKiemMT_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiemMT.Text.Trim();

            dgMuonTra.ItemsSource = string.IsNullOrWhiteSpace(tuKhoa)
                ? danhSachPhieu
                : danhSachPhieu.Where(p =>
                    p.MaDocGia.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.TenDocGia.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.TenSach.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.TrangThai.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                    p.MaPhieu.ToString().Contains(tuKhoa)).ToList();
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
            txtMaDocGia.Clear();
            txtTenDocGia.Clear();
            txtTenSach.Clear();
            txtNgayMuon.Clear();
            txtNgayHenTra.Clear();
            dgMuonTra.SelectedItem = null;
            txtThongBaoMT.Text = "";
        }
    }

    public class PhieuMuonHienThi
    {
        public int MaPhieu { get; set; }
        public string MaDocGia { get; set; } = "";
        public string TenDocGia { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string NgayMuon { get; set; } = "";
        public string NgayHenTra { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
}
