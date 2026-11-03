using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.DAL
{
    public class SachDAL
    {
        // LƯU Ý: Nếu lúc đăng nhập SQL Server (SSMS) bạn dùng tên Server khác (ví dụ máy bạn tên là KHOA\SQLEXPRESS), hãy sửa lại chỗ Data Source=... cho khớp nhé.
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyThuVienDB;Integrated Security=True";

        public List<SachDTO> LayDanhSachSach()
        {
            List<SachDTO> danhSach = new List<SachDTO>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MaSach, TenSach, TheLoai, SoLuongTon, IsDeleted FROM Sach WHERE IsDeleted = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SachDTO sach = new SachDTO();

                            sach.MaSach = reader["MaSach"].ToString();
                            sach.TenSach = reader["TenSach"].ToString();
                            sach.TheLoai = reader["TheLoai"].ToString();
                            sach.SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]);
                            sach.IsDeleted = Convert.ToBoolean(reader["IsDeleted"]);

                            danhSach.Add(sach);
                        }
                    }
                }
            }
            return danhSach;
        }
        // Hàm Thêm sách mới
        public bool ThemSach(SachDTO sach)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Dùng dấu @ để tạo tham số, chống hacker tấn công SQL Injection
                string query = "INSERT INTO Sach (MaSach, TenSach, TheLoai, SoLuongTon, IsDeleted) VALUES (@MaSach, @TenSach, @TheLoai, @SoLuongTon, 0)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSach", sach.MaSach);
                    cmd.Parameters.AddWithValue("@TenSach", sach.TenSach);
                    cmd.Parameters.AddWithValue("@TheLoai", sach.TheLoai);
                    cmd.Parameters.AddWithValue("@SoLuongTon", sach.SoLuongTon);

                    // ExecuteNonQuery trả về số dòng bị ảnh hưởng. Nếu > 0 nghĩa là thêm thành công.
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Hàm Sửa thông tin sách
        public bool SuaSach(SachDTO sach)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Sach SET TenSach = @TenSach, TheLoai = @TheLoai, SoLuongTon = @SoLuongTon WHERE MaSach = @MaSach AND IsDeleted = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSach", sach.MaSach);
                    cmd.Parameters.AddWithValue("@TenSach", sach.TenSach);
                    cmd.Parameters.AddWithValue("@TheLoai", sach.TheLoai);
                    cmd.Parameters.AddWithValue("@SoLuongTon", sach.SoLuongTon);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Hàm Xóa sách (Xóa mềm - Ẩn đi chứ không xóa mất dữ liệu cũ)
        public bool XoaSach(string maSach)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Thay vì dùng lệnh DELETE, ta cập nhật IsDeleted = 1 để ẩn sách đi
                string query = "UPDATE Sach SET IsDeleted = 1 WHERE MaSach = @MaSach";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSach", maSach);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}