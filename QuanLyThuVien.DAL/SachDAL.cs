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
                // Gọi DatabaseHelper.ConnectionString thay vì biến cục bộ
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
                                sach.TheLoai = reader["TheLoai"].ToString();
                                sach.SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]);
                                danhSach.Add(sach);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Lỗi SQL (sai tên cột, sập server...) sẽ nhảy vào đây, phần mềm không bị văng
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
                        cmd.Parameters.AddWithValue("@TheLoai", sach.TheLoai);
                        cmd.Parameters.AddWithValue("@SoLuongTon", sach.SoLuongTon);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thêm sách: " + ex.Message);
                return false; // Báo cho tầng BLL biết là việc thêm đã thất bại
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
                        cmd.Parameters.AddWithValue("@TheLoai", sach.TheLoai);
                        cmd.Parameters.AddWithValue("@SoLuongTon", sach.SoLuongTon);
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