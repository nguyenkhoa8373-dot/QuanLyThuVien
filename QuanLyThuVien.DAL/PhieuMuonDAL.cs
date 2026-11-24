using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.DAL
{
    public class PhieuMuonDAL
    {
        // ==========================================
        // 1. MƯỢN SÁCH (Tạo phiếu & Trừ số lượng tồn)
        // ==========================================
        public bool TaoPhieuMuon(PhieuMuonDTO phieuMuon, List<ChiTietPhieuMuonDTO> danhSachChiTiet)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Lưu phiếu mượn
                    string queryPhieu = "INSERT INTO PhieuMuon (MaPhieu, MaDocGia, NgayMuon, HanTra) VALUES (@MaPhieu, @MaDocGia, @NgayMuon, @HanTra)";
                    using (SqlCommand cmdPhieu = new SqlCommand(queryPhieu, conn, transaction))
                    {
                        cmdPhieu.Parameters.AddWithValue("@MaPhieu", phieuMuon.MaPhieu);
                        cmdPhieu.Parameters.AddWithValue("@MaDocGia", phieuMuon.MaDG);
                        cmdPhieu.Parameters.AddWithValue("@NgayMuon", phieuMuon.NgayMuon);
                        cmdPhieu.Parameters.AddWithValue("@HanTra", phieuMuon.HanTra);
                        cmdPhieu.ExecuteNonQuery();
                    }

                    // Lưu chi tiết & Trừ kho
                    string queryChiTiet = "INSERT INTO ChiTietPhieuMuon (MaPhieu, MaSach, SoLuongMuon, TrangThai) VALUES (@MaPhieu, @MaSach, @SoLuongMuon, N'Đang mượn')";
                    string queryTruSach = "UPDATE Sach SET SoLuongTon = SoLuongTon - @SoLuongMuon WHERE MaSach = @MaSach AND SoLuongTon >= @SoLuongMuon";

                    foreach (var chiTiet in danhSachChiTiet)
                    {
                        using (SqlCommand cmdChiTiet = new SqlCommand(queryChiTiet, conn, transaction))
                        {
                            cmdChiTiet.Parameters.AddWithValue("@MaPhieu", phieuMuon.MaPhieu);
                            cmdChiTiet.Parameters.AddWithValue("@MaSach", chiTiet.MaSach);
                            cmdChiTiet.Parameters.AddWithValue("@SoLuongMuon", chiTiet.SoLuongMuon);
                            cmdChiTiet.ExecuteNonQuery();
                        }

                        using (SqlCommand cmdTruSach = new SqlCommand(queryTruSach, conn, transaction))
                        {
                            cmdTruSach.Parameters.AddWithValue("@SoLuongMuon", chiTiet.SoLuongMuon);
                            cmdTruSach.Parameters.AddWithValue("@MaSach", chiTiet.MaSach);

                            if (cmdTruSach.ExecuteNonQuery() == 0)
                            {
                                throw new Exception($"Sách mã {chiTiet.MaSach} không đủ số lượng tồn kho!");
                            }
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Lỗi tạo phiếu mượn: " + ex.Message);
                    return false;
                }
            }
        }
        // ==========================================
        // 2. TRẢ SÁCH (Đã tích hợp tự động tính Tiền Phạt)
        // ==========================================
        public bool TraSach(string maPhieu)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Lấy chi tiết sách đang mượn
                    List<ChiTietPhieuMuonDTO> danhSachChiTiet = new List<ChiTietPhieuMuonDTO>();
                    string queryLayChiTiet = "SELECT MaSach, SoLuongMuon FROM ChiTietPhieuMuon WHERE MaPhieu = @MaPhieu AND TrangThai = N'Đang mượn'";
                    using (SqlCommand cmdLay = new SqlCommand(queryLayChiTiet, conn, transaction))
                    {
                        cmdLay.Parameters.AddWithValue("@MaPhieu", maPhieu);
                        using (SqlDataReader reader = cmdLay.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChiTietPhieuMuonDTO chiTiet = new ChiTietPhieuMuonDTO();
                                chiTiet.MaSach = reader["MaSach"].ToString();
                                chiTiet.SoLuongMuon = Convert.ToInt32(reader["SoLuongMuon"]);
                                danhSachChiTiet.Add(chiTiet);
                            }
                        }
                    }

                    // Hoàn lại sách vào kho
                    string queryHoanSach = "UPDATE Sach SET SoLuongTon = SoLuongTon + @SoLuongMuon WHERE MaSach = @MaSach";

                    // Cập nhật trạng thái và TỰ ĐỘNG TÍNH TIỀN PHẠT (Dùng DATEDIFF so sánh Ngày Trả và Hạn Trả)
                    string queryCapNhat = @"
                        UPDATE ChiTietPhieuMuon 
                        SET TrangThai = N'Đã trả', 
                            NgayTra = @NgayTra,
                            TienPhat = CASE 
                                           WHEN DATEDIFF(day, (SELECT HanTra FROM PhieuMuon WHERE MaPhieu = @MaPhieu), @NgayTra) > 0 
                                           THEN DATEDIFF(day, (SELECT HanTra FROM PhieuMuon WHERE MaPhieu = @MaPhieu), @NgayTra) * 5000
                                           ELSE 0 
                                       END
                        WHERE MaPhieu = @MaPhieu AND MaSach = @MaSach";

                    foreach (var chiTiet in danhSachChiTiet)
                    {
                        // Cộng kho
                        using (SqlCommand cmdHoan = new SqlCommand(queryHoanSach, conn, transaction))
                        {
                            cmdHoan.Parameters.AddWithValue("@SoLuongMuon", chiTiet.SoLuongMuon);
                            cmdHoan.Parameters.AddWithValue("@MaSach", chiTiet.MaSach);
                            cmdHoan.ExecuteNonQuery();
                        }

                        // Cập nhật phiếu chi tiết & Tiền phạt
                        using (SqlCommand cmdCapNhat = new SqlCommand(queryCapNhat, conn, transaction))
                        {
                            cmdCapNhat.Parameters.AddWithValue("@NgayTra", DateTime.Now);
                            cmdCapNhat.Parameters.AddWithValue("@MaPhieu", maPhieu);
                            cmdCapNhat.Parameters.AddWithValue("@MaSach", chiTiet.MaSach);
                            cmdCapNhat.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Lỗi trả sách: " + ex.Message);
                    return false;
                }
            }
        }

        // ==========================================
        // 3. LẤY DANH SÁCH PHIẾU MƯỢN (Dùng JOIN lấy Tên Độc Giả)
        // ==========================================
        public List<PhieuMuonDTO> LayDanhSachPhieuMuon()
        {
            List<PhieuMuonDTO> list = new List<PhieuMuonDTO>();
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT pm.MaPhieu, pm.MaDocGia, dg.TenDocGia, pm.NgayMuon, pm.HanTra 
                    FROM PhieuMuon pm 
                    JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhieuMuonDTO pm = new PhieuMuonDTO();
                            pm.MaPhieu = reader["MaPhieu"].ToString();
                            pm.MaDG = reader["MaDocGia"].ToString();

                            // Đòi hỏi bạn phải thêm public string TenDocGia { get; set; } vào PhieuMuonDTO.cs
                            pm.TenDocGia = reader["TenDocGia"].ToString();

                            pm.NgayMuon = Convert.ToDateTime(reader["NgayMuon"]);
                            pm.HanTra = Convert.ToDateTime(reader["HanTra"]);
                            list.Add(pm);
                        }
                    }
                }
            }
            return list;
        }

        // ==========================================
        // 4. LẤY CHI TIẾT 1 PHIẾU MƯỢN (Dùng JOIN lấy Tên Sách)
        // ==========================================
        public List<ChiTietPhieuMuonDTO> LayChiTietCuaPhieu(string maPhieu)
        {
            List<ChiTietPhieuMuonDTO> list = new List<ChiTietPhieuMuonDTO>();
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT ct.MaPhieu, ct.MaSach, s.TenSach, ct.SoLuongMuon, ct.TrangThai, ct.NgayTra 
                    FROM ChiTietPhieuMuon ct 
                    JOIN Sach s ON ct.MaSach = s.MaSach 
                    WHERE ct.MaPhieu = @MaPhieu";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChiTietPhieuMuonDTO ct = new ChiTietPhieuMuonDTO();
                            ct.MaPhieu = reader["MaPhieu"].ToString();
                            ct.MaSach = reader["MaSach"].ToString();

                            // Đòi hỏi bạn phải thêm public string TenSach { get; set; } vào ChiTietPhieuMuonDTO.cs
                            ct.TenSach = reader["TenSach"].ToString();

                            ct.SoLuongMuon = Convert.ToInt32(reader["SoLuongMuon"]);
                            ct.TrangThai = reader["TrangThai"].ToString();

                            if (reader["NgayTra"] != DBNull.Value)
                            {
                                ct.NgayTra = Convert.ToDateTime(reader["NgayTra"]);
                            }

                            list.Add(ct);
                        }
                    }
                }
            }
            return list;
        }
        // ==========================================
        // 5. ĐẾM SỐ SÁCH ĐỘC GIẢ ĐANG MƯỢN (chưa trả)
        // ==========================================
        public int DemSachDangMuonCuaDocGia(string maDocGia)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT ISNULL(SUM(ct.SoLuongMuon), 0)
                        FROM ChiTietPhieuMuon ct
                        JOIN PhieuMuon pm ON ct.MaPhieu = pm.MaPhieu
                        WHERE pm.MaDocGia = @MaDocGia AND ct.TrangThai = N'Đang mượn'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đếm sách đang mượn: " + ex.Message);
                return 0;
            }
        }
    }
}