
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.GUI.Views
{
    public partial class TrangChu : UserControl
    {
        // Khoi tao cac lop BLL de lay du lieu tu SQL Server
        private readonly SachBLL sachBLL = new SachBLL();
        private readonly DocGiaBLL docGiaBLL = new DocGiaBLL();
        private readonly PhieuMuonBLL phieuMuonBLL = new PhieuMuonBLL();

        public TrangChu()
        {
            InitializeComponent();

            TaiDuLieuTuSQL();
        }

        // Lay toan bo du lieu tu SQL Server
        private void TaiDuLieuTuSQL()
        {
            try
            {
                // Lay danh sach sach
                var danhSachSach = sachBLL.LayDanhSachSach();

                // Lay danh sach doc gia
                var danhSachDocGia = docGiaBLL.LayDanhSachDocGia();

                // Lay danh sach phieu muon
                var danhSachPhieu = phieuMuonBLL.LayDanhSachPhieuMuon();

                // Lay chi tiet tung phieu
                var duLieuPhieu = danhSachPhieu.Select(p => new
                {
                    Phieu = p,
                    ChiTiet = phieuMuonBLL.LayChiTietCuaPhieu(p.MaPhieu)
                }).ToList();

                // Cap nhat thong ke
                CapNhatThongKe(
                    danhSachSach.Count,
                    danhSachDocGia.Count,
                    duLieuPhieu
                );

                // Hien thi hoat dong gan day
                HienThiHoatDongGanDay(duLieuPhieu);

                // Cap nhat thong bao
                CapNhatThongBao(duLieuPhieu);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu từ SQL Server:\n" + ex.Message,
                    "Lỗi dữ liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        // Kiem tra phieu con sach chua tra
        private bool ConSachChuaTra(dynamic chiTiet)
        {
            return chiTiet.Any((Func<ChiTietPhieuMuonDTO, bool>)(ct =>
                ct.TrangThai == "Đang mượn"));
        }

        // Cap nhat cac the thong ke
        private void CapNhatThongKe(
            int tongSach,
            int tongDocGia,
            dynamic duLieuPhieu)
        {
            int dangMuon = 0;
            int quaHan = 0;

            foreach (var item in duLieuPhieu)
            {
                bool conSach = item.ChiTiet.Any(
                    (Func<ChiTietPhieuMuonDTO, bool>)(ct =>
                        ct.TrangThai == "Đang mượn"));

                // Tinh ca truong hop phieu khong co chi tiet tra ve
                if (!conSach && item.ChiTiet.Count == 0)
                    conSach = item.Phieu.TrangThai == "Đang mượn";

                if (conSach)
                {
                    dangMuon++;

                    if (item.Phieu.HanTra.HasValue &&
                        item.Phieu.HanTra.Value.Date < DateTime.Today)
                    {
                        quaHan++;
                    }
                }
            }

            txtTongSach.Text = tongSach.ToString();
            txtTongDocGia.Text = tongDocGia.ToString();
            txtDangMuon.Text = dangMuon.ToString();
            txtQuaHan.Text = quaHan.ToString();
        }

        // Hien thi danh sach hoat dong gan day
        private void HienThiHoatDongGanDay(dynamic duLieuPhieu)
        {
            var danhSachHoatDong = new List<object>();

            foreach (var item in duLieuPhieu)
            {
                var phieu = item.Phieu;

                if (item.ChiTiet.Count == 0)
                {
                    danhSachHoatDong.Add(new
                    {
                        MaPhieu = phieu.MaPhieu,
                        MaDocGia = phieu.MaDocGia,
                        MaSach = "-",
                        NgayMuon = phieu.NgayMuon.HasValue
                            ? phieu.NgayMuon.Value.ToString("dd/MM/yyyy")
                            : "",
                        TrangThai = phieu.TrangThai ?? ""
                    });

                    continue;
                }

                foreach (ChiTietPhieuMuonDTO ct in item.ChiTiet)
                {
                    string trangThai = ct.TrangThai;

                    if (trangThai == "Đang mượn" &&
                        phieu.HanTra.HasValue &&
                        phieu.HanTra.Value.Date < DateTime.Today)
                    {
                        trangThai = "Quá hạn";
                    }

                    danhSachHoatDong.Add(new
                    {
                        MaPhieu = phieu.MaPhieu,
                        MaDocGia = phieu.MaDocGia,
                        MaSach = ct.MaSach,
                        NgayMuon = phieu.NgayMuon.HasValue
                            ? phieu.NgayMuon.Value.ToString("dd/MM/yyyy")
                            : "",
                        TrangThai = trangThai
                    });
                }
            }

            dgHoatDong.ItemsSource = danhSachHoatDong
                .OrderByDescending(x =>
                {
                    var ngay = x.GetType()
                        .GetProperty("NgayMuon")
                        .GetValue(x)?.ToString();

                    return DateTime.TryParseExact(
                        ngay,
                        "dd/MM/yyyy",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime ketQua)
                        ? ketQua
                        : DateTime.MinValue;
                })
                .ToList();
        }

        // Cap nhat thong bao
        private void CapNhatThongBao(dynamic duLieuPhieu)
        {
            int dangMuon = 0;
            int quaHan = 0;

            foreach (var item in duLieuPhieu)
            {
                bool conSach = item.ChiTiet.Any(
                    (Func<ChiTietPhieuMuonDTO, bool>)(ct =>
                        ct.TrangThai == "Đang mượn"));

                if (!conSach && item.ChiTiet.Count == 0)
                    conSach = item.Phieu.TrangThai == "Đang mượn";

                if (conSach)
                {
                    dangMuon++;

                    if (item.Phieu.HanTra.HasValue &&
                        item.Phieu.HanTra.Value.Date < DateTime.Today)
                    {
                        quaHan++;
                    }
                }
            }

            txtThongBao1.Text =
                "• Hiện có " + dangMuon + " lượt mượn chưa trả.";

            txtThongBao2.Text =
                "• Hiện có " + quaHan + " lượt mượn quá hạn.";

            if (quaHan > 0)
            {
                txtThongBao3.Text =
                    "• Có sách đang quá hạn cần được xử lý.";
            }
            else
            {
                txtThongBao3.Text =
                    "• Hệ thống đang hoạt động bình thường.";
            }
        }
    }
}
