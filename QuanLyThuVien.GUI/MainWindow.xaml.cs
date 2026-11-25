using System.Windows;
using QuanLyThuVien.GUI.Views;

namespace QuanLyThuVien.GUI
{
    public partial class MainWindow : Window
    {
        // SỬA: cache instance de tranh tao moi + query lai SQL moi lan click
        private TrangChu _trangChu;
        private QuanLyMuonSach _quanLySach;   // đổi thành QuanLySach nếu bạn rename class
        private QuanLyDocGia _quanLyDocGia;
        private QuanLyMuonTra _quanLyMuonTra;

        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new TrangChu();
        }

        private void TrangChu_Click(object sender, RoutedEventArgs e)
        {
            _trangChu ??= new TrangChu();
            MainContent.Content = _trangChu;
        }

        private void QuanLySach_Click(object sender, RoutedEventArgs e)
        {
            // LƯU Ý: QuanLyMuonSach thuc chat la form CRUD sach.
            // Nen doi ten class thanh QuanLySach.
            _quanLySach ??= new QuanLyMuonSach();
            MainContent.Content = _quanLySach;
        }

        private void QuanLyDocGia_Click(object sender, RoutedEventArgs e)
        {
            _quanLyDocGia ??= new QuanLyDocGia();
            MainContent.Content = _quanLyDocGia;
        }

        private void QuanLyMuonTra_Click(object sender, RoutedEventArgs e)
        {
            _quanLyMuonTra ??= new QuanLyMuonTra();
            MainContent.Content = _quanLyMuonTra;
        }
    }
}