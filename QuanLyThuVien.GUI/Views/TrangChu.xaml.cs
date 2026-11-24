using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class TrangChu : UserControl
    {
        public TrangChu()
        {
            InitializeComponent();

            // Du lieu mau, se thay bang du lieu tu SQL sau
            ObservableCollection<ThongKeSach> danhSachSach = new()
            {
                new ThongKeSach { MaSach = "S001", TenSach = "Lap trinh C#", SoLuongTon = 10 },
                new ThongKeSach { MaSach = "S002", TenSach = "Lap trinh C++", SoLuongTon = 5 },
                new ThongKeSach { MaSach = "S003", TenSach = "Dac nhan tam", SoLuongTon = 8 }
            };

            ObservableCollection<string> danhSachDocGia = new()
            {
                "DG001", "DG002", "DG003"
            };

            ObservableCollection<HoatDong> danhSachMuon = new()
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

            // Cap nhat cac the thong ke
            txtTongSach.Text = danhSachSach.Count.ToString();
            txtTongDocGia.Text = danhSachDocGia.Count.ToString();

            int dangMuon = danhSachMuon.Count(p => p.TrangThai == "Đang mượn");

            int quaHan = danhSachMuon.Count(p =>
                p.TrangThai == "Đang mượn" &&
                p.HanTra.Date < DateTime.Today);

            txtDangMuon.Text = dangMuon.ToString();
            txtQuaHan.Text = quaHan.ToString();

            // Hien thi hoat dong gan day
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

            // Thong bao
            txtThongBao1.Text = "• Hiện có " + dangMuon + " lượt mượn chưa trả.";
            txtThongBao2.Text = "• Hiện có " + quaHan + " lượt mượn quá hạn.";
            txtThongBao3.Text = "• Hệ thống đang hoạt động bình thường.";
        }
    }

    public class ThongKeSach
    {
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public int SoLuongTon { get; set; }
    }

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