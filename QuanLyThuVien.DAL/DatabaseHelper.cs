using System.Data.SqlClient;

namespace QuanLyThuVien.DAL
{
    public class DatabaseHelper
    {
        // Tên DB chuẩn theo file SQL bạn đã gửi là QuanLyThuVienDB
        public static string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=QuanLyThuVienDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Thêm hàm này để DAL có thể gọi
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}