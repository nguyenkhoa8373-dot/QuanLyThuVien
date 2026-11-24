
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class TrangChu : UserControl
    {
        // Danh sach du lieu mau
        private ObservableCollection<ThongKeSach> danhSachSach = new();

        private ObservableCollection<string> danhSachDocGia = new();

        private ObservableCollection<HoatDong> danhSachMuon = new();

        public TrangChu()
        {
            InitializeComponent();

            KhoiTaoDuLieuMau();

            CapNhatThongKe();

            HienThiHoatDongGanDay();

            CapNhatThongBao();
        }

        // Khoi tao du lieu mau
        private void KhoiTaoDuLieuMau()
        {
            danhSachSach = new ObservableCollection<ThongKeSach>
            {
                new ThongKeSach
                {
                    MaSach = "S001",
                    TenSach = "Lap trinh C#",
                    SoLuongTon = 10
                },

                new ThongKeSach
                {
                    MaSach = "S002",
                    TenSach = "Lap trinh C++",
                    SoLuongTon = 5
                },

                new ThongKeSach
                {
                    MaSach = "S003",
                    TenSach = "Dac nhan tam",
                    SoLuongTon = 8
                }
            };

            danhSachDocGia = new ObservableCollection<string>
            {
                "DG001",
                "DG002",
                "DG003"
            };

            danhSachMuon = new ObservableCollection<HoatDong>
            {
                new HoatDong
                {
                    MaPhieu = "PM001",
                    MaDocGia = "DG001",
                    MaSach = "S001",
                    NgayMuon = new DateTime(2026, 11, 1),
                    HanTra = new DateTime(2026, 11, 10),
                    NgayTra = null,
                    TrangThai = "Đang mượn"
                },

                new HoatDong
                {
                    MaPhieu = "PM002",
                    MaDocGia = "DG002",
                    MaSach = "S002",
                    NgayMuon = new DateTime(2026, 11, 15),
                    HanTra = new DateTime(2026, 11, 25),
                    NgayTra = null,
                    TrangThai = "Đang mượn"
                },

                new HoatDong
                {
                    MaPhieu = "PM003",
                    MaDocGia = "DG003",
                    MaSach = "S003",
                    NgayMuon = new DateTime(2026, 10, 20),
                    HanTra = new DateTime(2026, 10, 30),
                    NgayTra = new DateTime(2026, 10, 28),
                    TrangThai = "Đã trả"
                }
            };
        }

        // Cap nhat cac the thong ke
        private void CapNhatThongKe()
        {
            int tongSach = danhSachSach.Count;

            int tongDocGia = danhSachDocGia.Count;

            int dangMuon = danhSachMuon.Count(p =>
                p.TrangThai == "Đang mượn");

            int quaHan = danhSachMuon.Count(p =>
                p.TrangThai == "Đang mượn" &&
                p.HanTra.Date < DateTime.Today);

            txtTongSach.Text = tongSach.ToString();

            txtTongDocGia.Text = tongDocGia.ToString();

            txtDangMuon.Text = dangMuon.ToString();

            txtQuaHan.Text = quaHan.ToString();
        }

        // Hien thi danh sach hoat dong gan day
        private void HienThiHoatDongGanDay()
        {
            dgHoatDong.ItemsSource = danhSachMuon
                .OrderByDescending(p => p.NgayMuon)
                .Select(p => new
                {
                    p.MaPhieu,
                    p.MaDocGia,
                    p.MaSach,

                    NgayMuon = p.NgayMuon.ToString("dd/MM/yyyy"),

                    p.TrangThai
                })
                .ToList();
        }

        // Cap nhat thong bao
        private void CapNhatThongBao()
        {
            int dangMuon = danhSachMuon.Count(p =>
                p.TrangThai == "Đang mượn");

            int quaHan = danhSachMuon.Count(p =>
                p.TrangThai == "Đang mượn" &&
                p.HanTra.Date < DateTime.Today);

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

    // Lop thong ke sach
    public class ThongKeSach
    {
        public string MaSach { get; set; } = "";

        public string TenSach { get; set; } = "";

        public int SoLuongTon { get; set; }
    }

    // Lop thong tin hoat dong muon tra
    public class HoatDong
    {
        public string MaPhieu { get; set; } = "";

        public string MaDocGia { get; set; } = "";

        public string MaSach { get; set; } = "";

        public DateTime NgayMuon { get; set; }

        public DateTime HanTra { get; set; }

        public DateTime? NgayTra { get; set; }

        public string TrangThai { get; set; } = "";
    }
}
