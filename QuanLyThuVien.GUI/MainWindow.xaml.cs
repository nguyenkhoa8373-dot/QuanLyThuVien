
using System.Windows;
using QuanLyThuVien.GUI.Views;

namespace QuanLyThuVien.GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContent.Content = new TrangChu();
        }

        private void TrangChu_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TrangChu();
        }

        private void QuanLySach_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QuanLyMuonSach();
        }

        private void QuanLyDocGia_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QuanLyDocGia();
        }

        private void QuanLyMuonTra_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QuanLyMuonTra();
        }
    }
}
