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
                var danhSachSach = sachBLL.LayDanhSachSach();
                var danhSachDocGia = docGiaBLL.LayDanhSachDocGia();
                var danhSachPhieu = phieuMuonBLL.LayDanhSachPhieuMuon();

                // SỬA: dung class thay vi anonymous type + dynamic
                var duLieuPhieu = danhSachPhieu.Select(p => new PhieuVaChiTiet
                {
                    Phieu = p,
                    ChiTiet = phieuMuonBLL.LayChiTietCuaPhieu(p.MaPhieu)
                }).ToList();

                // SỬA: gop cap nhat thong ke + thong bao
                CapNhatThongKeVaThongBao(
                    danhSachSach.Count,
                    danhSachDocGia.Count,
                    duLieuPhieu
                );

                HienThiHoatDongGanDay(duLieuPhieu);
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

        // SỬA: kieu tuong minh, khong dynamic
        private bool ConSachChuaTra(List<ChiTietPhieuMuonDTO> chiTiet)
        {
            return chiTiet.Any(ct => ct.TrangThai == "Đang mượn");
        }

        // SỬA: tach ham tinh thong ke dung chung
        private (int dangMuon, int quaHan) TinhThongKe(List<PhieuVaChiTiet> duLieuPhieu)
        {
            int dangMuon = 0;
            int quaHan = 0;

            foreach (var item in duLieuPhieu)
            {
                bool conSach = ConSachChuaTra(item.ChiTiet);

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

            return (dangMuon, quaHan);
        }

        // SỬA: gop 2 ham thanh 1, tranh lap logic
        private void CapNhatThongKeVaThongBao(
            int tongSach,
            int tongDocGia,
            List<PhieuVaChiTiet> duLieuPhieu)
        {
            var (dangMuon, quaHan) = TinhThongKe(duLieuPhieu);

            txtTongSach.Text = tongSach.ToString();
            txtTongDocGia.Text = tongDocGia.ToString();
            txtDangMuon.Text = dangMuon.ToString();
            txtQuaHan.Text = quaHan.ToString();

            txtThongBao1.Text = "• Hiện có " + dangMuon + " lượt mượn chưa trả.";
            txtThongBao2.Text = "• Hiện có " + quaHan + " lượt mượn quá hạn.";
            txtThongBao3.Text = quaHan > 0
                ? "• Có sách đang quá hạn cần được xử lý."
                : "• Hệ thống đang hoạt động bình thường.";
        }

        // Hien thi danh sach hoat dong gan day
        private void HienThiHoatDongGanDay(List<PhieuVaChiTiet> duLieuPhieu)
        {
            // SỬA: dung class co kieu tuong minh thay vi anonymous + reflection
            var danhSachHoatDong = new List<HoatDongHienThi>();

            foreach (var item in duLieuPhieu)
            {
                var phieu = item.Phieu;

                if (item.ChiTiet.Count == 0)
                {
                    danhSachHoatDong.Add(new HoatDongHienThi
                    {
                        MaPhieu = phieu.MaPhieu,
                        MaDocGia = phieu.MaDocGia,
                        MaSach = "-",
                        NgayMuon = phieu.NgayMuon ?? DateTime.MinValue,
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

                    danhSachHoatDong.Add(new HoatDongHienThi
                    {
                        MaPhieu = phieu.MaPhieu,
                        MaDocGia = phieu.MaDocGia,
                        MaSach = ct.MaSach,
                        NgayMuon = phieu.NgayMuon ?? DateTime.MinValue,
                        TrangThai = trangThai
                    });
                }
            }

            // SỬA: sap xep truc tiep, khong reflection
            dgHoatDong.ItemsSource = danhSachHoatDong
                .OrderByDescending(x => x.NgayMuon)
                .ToList();
        }
    }

    // SỬA: class ho tro thay cho dynamic
    public class PhieuVaChiTiet
    {
        public PhieuMuonDTO Phieu { get; set; }
        public List<ChiTietPhieuMuonDTO> ChiTiet { get; set; }
    }

    public class HoatDongHienThi
    {
        public string MaPhieu { get; set; }
        public string MaDocGia { get; set; }
        public string MaSach { get; set; }
        public DateTime NgayMuon { get; set; }
        public string NgayMuonText => NgayMuon.ToString("dd/MM/yyyy");
        public string TrangThai { get; set; }
    }
}