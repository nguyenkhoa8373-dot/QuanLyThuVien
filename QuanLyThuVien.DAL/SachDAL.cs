using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.DAL
{
    public class SachDAL
    {
        public List<SachDTO> LayDanhSachSach()
        {
            List<SachDTO> danhSach = new List<SachDTO>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Sach WHERE IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SachDTO sach = new SachDTO();
                            sach.MaSach = reader["MaSach"].ToString();
                            sach.TenSach = reader["TenSach"].ToString();
                            sach.TheLoai = reader["TheLoai"] == DBNull.Value ? null : reader["TheLoai"].ToString();
                            sach.SoLuongTon = reader["SoLuongTon"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SoLuongTon"]);
                            sach.IsDeleted = reader["IsDeleted"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(reader["IsDeleted"]);
                            danhSach.Add(sach);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); }
            return danhSach;
        }

        public bool ThemSach(SachDTO sach)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Sach (MaSach, TenSach, TheLoai, SoLuongTon, IsDeleted) VALUES (@MaSach, @TenSach, @TheLoai, @SoLuongTon, 0)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSach", sach.MaSach);
                        cmd.Parameters.AddWithValue("@TenSach", sach.TenSach);
                        cmd.Parameters.AddWithValue("@TheLoai", (object?)sach.TheLoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SoLuongTon", (object?)sach.SoLuongTon ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); return false; }
        }

        public bool SuaSach(SachDTO sach)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Sach SET TenSach = @TenSach, TheLoai = @TheLoai, SoLuongTon = @SoLuongTon WHERE MaSach = @MaSach AND IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSach", sach.MaSach);
                        cmd.Parameters.AddWithValue("@TenSach", sach.TenSach);
                        cmd.Parameters.AddWithValue("@TheLoai", (object?)sach.TheLoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SoLuongTon", (object?)sach.SoLuongTon ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); return false; }
        }

        public bool XoaSach(string maSach)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
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
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); return false; }
        }
    }
}