using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.DAL
{
    public class DocGiaDAL
    {
        public List<DocGiaDTO> LayDanhSachDocGia()
        {
            List<DocGiaDTO> danhSach = new List<DocGiaDTO>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM DocGia WHERE IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DocGiaDTO dg = new DocGiaDTO();
                            dg.MaDocGia = reader["MaDocGia"].ToString();
                            dg.TenDocGia = reader["TenDocGia"].ToString();
                            dg.SoDienThoai = reader["SoDienThoai"] == DBNull.Value ? null : reader["SoDienThoai"].ToString();
                            dg.DiaChi = reader["DiaChi"] == DBNull.Value ? null : reader["DiaChi"].ToString();
                            dg.IsDeleted = reader["IsDeleted"] != DBNull.Value && Convert.ToBoolean(reader["IsDeleted"]);
                            danhSach.Add(dg);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); }
            return danhSach;
        }

        public bool ThemDocGia(DocGiaDTO dg)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO DocGia (MaDocGia, TenDocGia, SoDienThoai, DiaChi, IsDeleted) VALUES (@MaDocGia, @TenDocGia, @SoDienThoai, @DiaChi, 0)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDocGia", dg.MaDocGia);
                        cmd.Parameters.AddWithValue("@TenDocGia", dg.TenDocGia);
                        cmd.Parameters.AddWithValue("@SoDienThoai", (object?)dg.SoDienThoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiaChi", (object?)dg.DiaChi ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); return false; }
        }

        public bool SuaDocGia(DocGiaDTO dg)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE DocGia SET TenDocGia = @TenDocGia, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi WHERE MaDocGia = @MaDocGia AND IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDocGia", dg.MaDocGia);
                        cmd.Parameters.AddWithValue("@TenDocGia", dg.TenDocGia);
                        cmd.Parameters.AddWithValue("@SoDienThoai", (object?)dg.SoDienThoai ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiaChi", (object?)dg.DiaChi ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); return false; }
        }

        public bool XoaDocGia(string maDocGia)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE DocGia SET IsDeleted = 1 WHERE MaDocGia = @MaDocGia";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); return false; }
        }

        public List<DocGiaDTO> TimKiemDocGia(string tuKhoa)
        {
            List<DocGiaDTO> danhSach = new List<DocGiaDTO>();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM DocGia WHERE (TenDocGia LIKE @TuKhoa OR MaDocGia LIKE @TuKhoa) AND IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DocGiaDTO dg = new DocGiaDTO();
                                dg.MaDocGia = reader["MaDocGia"].ToString();
                                dg.TenDocGia = reader["TenDocGia"].ToString();
                                dg.SoDienThoai = reader["SoDienThoai"] == DBNull.Value ? null : reader["SoDienThoai"].ToString();
                                dg.DiaChi = reader["DiaChi"] == DBNull.Value ? null : reader["DiaChi"].ToString();
                                dg.IsDeleted = reader["IsDeleted"] != DBNull.Value && Convert.ToBoolean(reader["IsDeleted"]);
                                danhSach.Add(dg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi: " + ex.Message); }
            return danhSach;
        }
    }
}