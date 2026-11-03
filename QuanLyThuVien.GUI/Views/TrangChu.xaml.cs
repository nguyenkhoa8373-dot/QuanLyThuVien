
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace QuanLyThuVien.GUI.Views
{
    public partial class TrangChu : UserControl
    {
        public TrangChu()
        {
            InitializeComponent();

            ObservableCollection<HoatDong> danhSach =
                new ObservableCollection<HoatDong>();

            danhSach.Add(new HoatDong
            {
                MaDocGia = "DG001",
                TenDocGia = "Nguyen Van A",
                TenSach = "Lap trinh C#",
                NgayMuon = "01/11/2026",
                TrangThai = "Đang mượn"
            });

            danhSach.Add(new HoatDong
            {
                MaDocGia = "DG002",
                TenDocGia = "Tran Thi B",
                TenSach = "Lap trinh C++",
                NgayMuon = "02/11/2026",
                TrangThai = "Đã trả"
            });

            danhSach.Add(new HoatDong
            {
                MaDocGia = "DG003",
                TenDocGia = "Le Van C",
                TenSach = "Co so du lieu",
                NgayMuon = "03/11/2026",
                TrangThai = "Đang mượn"
            });

            dgHoatDong.ItemsSource = danhSach;
        }
    }

    public class HoatDong
    {
        public string MaDocGia { get; set; } = "";
        public string TenDocGia { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string NgayMuon { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
}
