using System;
using System.Data.SqlClient;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.DAL
{
    public class TaiKhoanDAL
    {
        // Hàm kiểm tra Đăng nhập: Trả về thông tin Tài khoản nếu đúng, trả về null nếu sai
        public TaiKhoanDTO DangNhap(string tenDangNhap, string matKhau)
        {
            TaiKhoanDTO taiKhoan = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    // So sánh Tên đăng nhập và Mật khẩu trong CSDL
                    string query = "SELECT TenDangNhap, HoTen, Quyen FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                taiKhoan = new TaiKhoanDTO();
                                taiKhoan.TenDangNhap = reader["TenDangNhap"].ToString();
                                taiKhoan.HoTen = reader["HoTen"].ToString();
                                taiKhoan.Quyen = reader["Quyen"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi đăng nhập: " + ex.Message);
            }

            return taiKhoan; // Tầng BLL sẽ kiểm tra, nếu null là báo "Sai tài khoản/mật khẩu"
        }
    }
}