using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.DAL
{
    public class SachDAL
    {
        // 1. Hàm Lấy danh sách
        public List<SachDTO> LayDanhSachSach()
        {
            List<SachDTO> danhSach = new List<SachDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Sach WHERE IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SachDTO sach = new SachDTO();
                                sach.MaSach = reader["MaSach"].ToString();
                                sach.TenSach = reader["TenSach"].ToString();

                                // ĐÃ SỬA LỖI 3: Kiểm tra DBNull cho TheLoai và SoLuongTon
                                sach.TheLoai = reader["TheLoai"] == DBNull.Value ? null : reader["TheLoai"].ToString();
                                sach.SoLuongTon = reader["SoLuongTon"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SoLuongTon"]);

                                danhSach.Add(sach);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi hệ thống khi lấy danh sách sách: " + ex.Message);
            }
            return danhSach;
        }

        // 2. Hàm Thêm sách mới
        public bool ThemSach(SachDTO sach)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Sach (MaSach, TenSach, TheLoai, SoLuongTon, IsDeleted) VALUES (@MaSach, @TenSach, @TheLoai, @SoLuongTon, 0)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSach", sach.MaSach);
                        cmd.Parameters.AddWithValue("@TenSach", sach.TenSach);
                        // ĐÃ SỬA: Xử lý truyền null xuống SQL an toàn
                        cmd.Parameters.AddWithValue("@TheLoai", (object?)sach.TheLoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SoLuongTon", (object?)sach.SoLuongTon ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thêm sách: " + ex.Message);
                return false;
            }
        }

        // 3. Hàm Sửa thông tin sách
        public bool SuaSach(SachDTO sach)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "UPDATE Sach SET TenSach = @TenSach, TheLoai = @TheLoai, SoLuongTon = @SoLuongTon WHERE MaSach = @MaSach AND IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSach", sach.MaSach);
                        cmd.Parameters.AddWithValue("@TenSach", sach.TenSach);
                        // ĐÃ SỬA: Xử lý truyền null xuống SQL an toàn
                        cmd.Parameters.AddWithValue("@TheLoai", (object?)sach.TheLoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SoLuongTon", (object?)sach.SoLuongTon ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi sửa sách: " + ex.Message);
                return false;
            }
        }

        // 4. Hàm Xóa sách (Xóa mềm)
        public bool XoaSach(string maSach)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "UPDATE Sach SET IsDeleted = 1 WHERE MaSach = @MaSach";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xóa sách: " + ex.Message);
                return false;
            }
        }
    }
}