using System;
using System.Windows;
using System.Windows.Threading;

namespace QuanLyThuVien.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Bắt exception không được xử lý ở UI thread
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Bắt exception ở background thread
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show(
                    "Lỗi nghiêm trọng:\n" + ex?.Message,
                    "Lỗi không xác định",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };
        }

        private void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "Đã xảy ra lỗi:\n\n" + e.Exception.Message +
                "\n\nChi tiết:\n" + e.Exception.StackTrace,
                "Lỗi ứng dụng",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Giữ ứng dụng chạy tiếp (có thể đổi thành Shutdown() nếu muốn)
            e.Handled = true;
        }
    }
}