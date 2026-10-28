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
    }
}