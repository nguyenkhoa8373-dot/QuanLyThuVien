using System.Data.SqlClient;

namespace QuanLyThuVien.DAL
{
    public class DatabaseHelper
    {
        public static string ConnectionString = @"Server=.\SQLEXPRESS;Database=QuanLyThuVienDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Thêm hàm này để DAL có thể gọi
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}