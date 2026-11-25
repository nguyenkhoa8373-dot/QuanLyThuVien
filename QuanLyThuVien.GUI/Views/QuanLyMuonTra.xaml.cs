using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI.Views
{
    public partial class QuanLyMuonTra : UserControl
    {
        private readonly PhieuMuonBLL phieuMuonBLL = new();

        private List<PhieuMuonHienThi> danhSachPhieu = new();

        public QuanLyMuonTra()
        {
            InitializeComponent();

            txtNgayTra.IsReadOnly = true;
            txtTienPhat.IsReadOnly = true;

            LoadPhieuMuon();
        }

        // 1. Tai danh sach phieu muon tu SQL
        private void LoadPhieuMuon()
        {
            try
            {
                List<PhieuMuonHienThi> danhSachMoi = new();

                List<PhieuMuonDTO> dsPhieu =
                    phieuMuonBLL.LayDanhSachPhieuMuon();

                foreach (PhieuMuonDTO phieu in dsPhieu)
                {
                    List<ChiTietPhieuMuonDTO> dsChiTiet =
                        phieuMuonBLL.LayChiTietCuaPhieu(phieu.MaPhieu);

                    bool conSachChuaTra = dsChiTiet.Any(
                        ct => string.Equals(
                            ct.TrangThai,
                            "Đang mượn",
                            StringComparison.OrdinalIgnoreCase));

                    // SỬA: tinh trang thai cap phieu (dung cho phieu khong co chi tiet)
                    string trangThaiPhieu;
                    if (!conSachChuaTra)
                    {
                        trangThaiPhieu = "Đã trả";
                    }
                    else if (phieu.HanTra.HasValue &&
                             phieu.HanTra.Value.Date < DateTime.Today)
                    {
                        trangThaiPhieu = "Quá hạn";
                    }
                    else
                    {
                        trangThaiPhieu = "Đang mượn";
                    }

                    foreach (ChiTietPhieuMuonDTO ct in dsChiTiet)
                    {
                        // SỬA: tinh trang thai theo tung chi tiet (an toan khi mo rong)
                        string trangThaiChiTiet;
                        if (string.Equals(
                                ct.TrangThai,
                                "Đã trả",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            trangThaiChiTiet = "Đã trả";
                        }
                        else if (phieu.HanTra.HasValue &&
                                 phieu.HanTra.Value.Date < DateTime.Today)
                        {
                            trangThaiChiTiet = "Quá hạn";
                        }
                        else
                        {
                            trangThaiChiTiet = "Đang mượn";
                        }

                        danhSachMoi.Add(new PhieuMuonHienThi
                        {
                            MaPhieu = phieu.MaPhieu ?? "",
                            MaDocGia = phieu.MaDocGia ?? "",
                            MaSach = ct.MaSach ?? "",
                            SoLuongMuon = ct.SoLuongMuon,

                            NgayMuon = phieu.NgayMuon?.ToString("dd/MM/yyyy") ?? "",
                            HanTra = phieu.HanTra?.ToString("dd/MM/yyyy") ?? "",
                            NgayTra = ct.NgayTra?.ToString("dd/MM/yyyy") ?? "",

                            TrangThai = trangThaiChiTiet,

                            TienPhat = ct.TienPhat ?? 0
                        });
                    }

                    // Phieu chua co chi tiet
                    if (dsChiTiet.Count == 0)
                    {
                        danhSachMoi.Add(new PhieuMuonHienThi
                        {
                            MaPhieu = phieu.MaPhieu ?? "",
                            MaDocGia = phieu.MaDocGia ?? "",
                            NgayMuon = phieu.NgayMuon?.ToString("dd/MM/yyyy") ?? "",
                            HanTra = phieu.HanTra?.ToString("dd/MM/yyyy") ?? "",
                            TrangThai = trangThaiPhieu
                        });
                    }
                }

                danhSachPhieu = danhSachMoi;

                dgMuonTra.ItemsSource = null;
                dgMuonTra.ItemsSource = danhSachPhieu;

                txtThongBaoMT.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Khong the tai danh sach phieu muon tu SQL Server:\n\n" +
                    ex.Message,
                    "Loi ket noi du lieu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 2. Kiem tra thong tin nhap
        private bool KiemTraForm(
            out DateTime ngayMuon,
            out DateTime hanTra,
            out int soLuong)
        {
            ngayMuon = DateTime.MinValue;
            hanTra = DateTime.MinValue;
            soLuong = 0;

            // SỬA: bo sung kiem tra txtMaPhieu rong
            if (string.IsNullOrWhiteSpace(txtMaPhieu.Text) ||
                string.IsNullOrWhiteSpace(txtMaDocGia.Text) ||
                string.IsNullOrWhiteSpace(txtMaSach.Text) ||
                string.IsNullOrWhiteSpace(txtNgayMuon.Text) ||
                string.IsNullOrWhiteSpace(txtHanTra.Text))
            {
                MessageBox.Show("Vui long nhap day du thong tin!");
                return false;
            }

            string maPhieu = txtMaPhieu.Text.Trim();
            string maDocGia = txtMaDocGia.Text.Trim();
            string maSach = txtMaSach.Text.Trim();

            if (maPhieu.Length > 10)
            {
                MessageBox.Show("Ma phieu khong duoc vuot qua 10 ky tu!");
                return false;
            }

            if (maDocGia.Length > 10 || maSach.Length > 10)
            {
                MessageBox.Show("Ma doc gia va ma sach toi da 10 ky tu!");
                return false;
            }

            if (!int.TryParse(
                txtSoLuongMuon.Text.Trim(),
                out soLuong) || soLuong <= 0)
            {
                MessageBox.Show("So luong muon phai lon hon 0!");
                return false;
            }

            if (!DateTime.TryParseExact(
                txtNgayMuon.Text.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out ngayMuon))
            {
                MessageBox.Show(
                    "Ngay muon khong hop le! Dinh dang dd/MM/yyyy.");
                return false;
            }

            if (!DateTime.TryParseExact(
                txtHanTra.Text.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out hanTra))
            {
                MessageBox.Show(
                    "Han tra khong hop le! Dinh dang dd/MM/yyyy.");
                return false;
            }

            if (ngayMuon.Date > DateTime.Today)
            {
                MessageBox.Show("Ngay muon khong duoc lon hon ngay hien tai!");
                return false;
            }

            if (hanTra.Date <= ngayMuon.Date)
            {
                MessageBox.Show("Han tra phai sau ngay muon!");
                return false;
            }

            return true;
        }

        // 3. Tao phieu muon va luu vao SQL
        private void TaoPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (!KiemTraForm(
                out DateTime ngayMuon,
                out DateTime hanTra,
                out int soLuong))
            {
                return;
            }

            PhieuMuonDTO phieu = new()
            {
                MaPhieu = txtMaPhieu.Text.Trim(),
                MaDocGia = txtMaDocGia.Text.Trim(),
                NgayMuon = ngayMuon,
                HanTra = hanTra,
                TrangThai = "Đang mượn"
            };

            List<ChiTietPhieuMuonDTO> dsChiTiet = new()
            {
                new ChiTietPhieuMuonDTO
                {
                    MaSach = txtMaSach.Text.Trim(),
                    SoLuongMuon = soLuong,
                    TrangThai = "Đang mượn"
                }
            };

            try
            {
                bool ketQua = phieuMuonBLL.TaoPhieuMuon(
                    phieu,
                    dsChiTiet,
                    out string thongBao);

                MessageBox.Show(
                    thongBao,
                    ketQua ? "Thanh cong" : "That bai",
                    MessageBoxButton.OK,
                    ketQua ? MessageBoxImage.Information : MessageBoxImage.Warning);

                if (ketQua)
                {
                    LoadPhieuMuon();
                    NhapLai();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi tao phieu muon:\n\n" + ex.Message,
                    "Loi SQL",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 4. Xac nhan tra sach
        private void XacNhanTra_Click(object sender, RoutedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is not PhieuMuonHienThi phieu)
            {
                MessageBox.Show("Vui long chon phieu can tra!");
                return;
            }

            if (phieu.TrangThai == "Đã trả")
            {
                MessageBox.Show("Phieu nay da duoc tra!");
                return;
            }

            // CẢNH BÁO: TraSach hien tai chi nhan MaPhieu -> tra TOAN BO sach trong phieu.
            // Neu 1 phieu co nhieu sach, can sua BLL/DAL de tra tung cuon theo MaSach.
            MessageBoxResult xacNhan = MessageBox.Show(
                "Xac nhan tra TOAN BO sach cua phieu " + phieu.MaPhieu + "?",
                "Xac nhan tra sach",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (xacNhan != MessageBoxResult.Yes)
                return;

            try
            {
                bool ketQua = phieuMuonBLL.TraSach(
                    phieu.MaPhieu,
                    out string thongBao);

                MessageBox.Show(thongBao);

                if (ketQua)
                {
                    LoadPhieuMuon();
                    NhapLai();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Loi tra sach:\n\n" + ex.Message,
                    "Loi SQL",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // 5. Xoa phieu
        private void XoaPhieu_Click(object sender, RoutedEventArgs e)
        {
            // CẢNH BÁO: chuc nang chua duoc ho tro trong BLL/DAL.
            // Can bo sung XoaPhieu (xoa ChiTietPhieuMuon truoc, roi xoa PhieuMuon, trong transaction).
            MessageBox.Show(
                "Chuc nang xoa phieu chua duoc ho tro trong BLL va DAL.\n" +
                "Can bo sung xu ly xoa phieu va hoan kho trong SQL truoc.",
                "Thong bao",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // 6. Chon phieu tren DataGrid
        private void dgMuonTra_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgMuonTra.SelectedItem is PhieuMuonHienThi phieu)
            {
                txtMaPhieu.Text = phieu.MaPhieu;
                txtMaDocGia.Text = phieu.MaDocGia;
                txtMaSach.Text = phieu.MaSach;

                txtSoLuongMuon.Text =
                    phieu.SoLuongMuon.ToString();

                txtNgayMuon.Text = phieu.NgayMuon;
                txtHanTra.Text = phieu.HanTra;
                txtNgayTra.Text = phieu.NgayTra;

                txtTienPhat.Text =
                    phieu.TienPhat.ToString("N0");

                // SỬA: khoa ma phieu khi dang chon (tranh tao nham phieu moi)
                txtMaPhieu.IsReadOnly = true;
            }
            else
            {
                // SỬA: mo khoa khi bo chon
                txtMaPhieu.IsReadOnly = false;
            }
        }

        // 7. Tim kiem phieu muon
        private void TimKiemMT_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtTimKiemMT.Text.Trim();

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                dgMuonTra.ItemsSource = null;      // SỬA: reset
                dgMuonTra.ItemsSource = danhSachPhieu;
                return;
            }

            var ketQua = danhSachPhieu.Where(p =>
                (p.MaPhieu ?? "").Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase) ||

                (p.MaDocGia ?? "").Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase) ||

                (p.MaSach ?? "").Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase) ||

                (p.TrangThai ?? "").Contains(
                    tuKhoa, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            dgMuonTra.ItemsSource = null;          // SỬA: reset
            dgMuonTra.ItemsSource = ketQua;
        }

        // 8. Hien thi tat ca
        private void HienTatCaMT_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemMT.Clear();
            dgMuonTra.ItemsSource = null;          // SỬA: reset
            dgMuonTra.ItemsSource = danhSachPhieu;
        }

        // 9. Nhap lai form
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

            txtMaPhieu.IsReadOnly = false;

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

        public string NgayMuon { get; set; } = "";
        public string HanTra { get; set; } = "";
        public string NgayTra { get; set; } = "";

        public string TrangThai { get; set; } = "";

        public decimal TienPhat { get; set; }
    }
}