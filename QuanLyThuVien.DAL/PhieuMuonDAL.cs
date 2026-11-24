using System;
using System.Collections.Generic;
using System.Data;
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
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // SỬA: bổ sung cột TrangThai theo đúng schema PhieuMuon
                        string queryPhieu = @"
                            INSERT INTO PhieuMuon (MaPhieu, MaDocGia, NgayMuon, HanTra, TrangThai)
                            VALUES (@MaPhieu, @MaDocGia, @NgayMuon, @HanTra, @TrangThai)";
                        using (SqlCommand cmdPhieu = new SqlCommand(queryPhieu, conn, transaction))
                        {
                            cmdPhieu.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 10).Value = phieuMuon.MaPhieu;
                            // SỬA: MaDG -> MaDocGia, xử lý NULL (cột cho phép NULL)
                            cmdPhieu.Parameters.Add("@MaDocGia", SqlDbType.VarChar, 10).Value =
                                (object)phieuMuon.MaDocGia ?? DBNull.Value;
                            // SỬA: NgayMuon / HanTra là DateTime? -> cần DBNull.Value
                            cmdPhieu.Parameters.Add("@NgayMuon", SqlDbType.DateTime).Value =
                                (object)phieuMuon.NgayMuon ?? (object)DBNull.Value;
                            cmdPhieu.Parameters.Add("@HanTra", SqlDbType.DateTime).Value =
                                (object)phieuMuon.HanTra ?? (object)DBNull.Value;
                            cmdPhieu.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 50).Value =
                                (object)phieuMuon.TrangThai ?? "Đang mượn";
                            cmdPhieu.ExecuteNonQuery();
                        }

                        string queryChiTiet = @"
                            INSERT INTO ChiTietPhieuMuon (MaPhieu, MaSach, SoLuongMuon, TrangThai)
                            VALUES (@MaPhieu, @MaSach, @SoLuongMuon, N'Đang mượn')";
                        string queryTruSach = @"
                            UPDATE Sach SET SoLuongTon = SoLuongTon - @SoLuongMuon
                            WHERE MaSach = @MaSach AND SoLuongTon >= @SoLuongMuon";

                        foreach (var chiTiet in danhSachChiTiet)
                        {
                            using (SqlCommand cmdChiTiet = new SqlCommand(queryChiTiet, conn, transaction))
                            {
                                cmdChiTiet.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 10).Value = phieuMuon.MaPhieu;
                                cmdChiTiet.Parameters.Add("@MaSach", SqlDbType.VarChar, 10).Value = chiTiet.MaSach;
                                cmdChiTiet.Parameters.Add("@SoLuongMuon", SqlDbType.Int).Value = chiTiet.SoLuongMuon;
                                cmdChiTiet.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdTruSach = new SqlCommand(queryTruSach, conn, transaction))
                            {
                                cmdTruSach.Parameters.Add("@SoLuongMuon", SqlDbType.Int).Value = chiTiet.SoLuongMuon;
                                cmdTruSach.Parameters.Add("@MaSach", SqlDbType.VarChar, 10).Value = chiTiet.MaSach;

                                if (cmdTruSach.ExecuteNonQuery() == 0)
                                    throw new Exception($"Sách mã {chiTiet.MaSach} không đủ số lượng tồn kho!");
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
        }

        // ==========================================
        // 2. TRẢ SÁCH (Đã tích hợp tự động tính Tiền Phạt)
        // ==========================================
        public bool TraSach(string maPhieu)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Lấy chi tiết sách đang mượn
                        List<ChiTietPhieuMuonDTO> danhSachChiTiet = new List<ChiTietPhieuMuonDTO>();
                        string queryLayChiTiet = @"
                            SELECT MaSach, SoLuongMuon
                            FROM ChiTietPhieuMuon
                            WHERE MaPhieu = @MaPhieu AND TrangThai = N'Đang mượn'";
                        using (SqlCommand cmdLay = new SqlCommand(queryLayChiTiet, conn, transaction))
                        {
                            cmdLay.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 10).Value = maPhieu;
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

                        // THÊM: chặn trường hợp phiếu không còn gì để trả
                        if (danhSachChiTiet.Count == 0)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Không có sách nào đang mượn cho phiếu {maPhieu}.");
                            return false;
                        }

                        string queryHoanSach = "UPDATE Sach SET SoLuongTon = SoLuongTon + @SoLuongMuon WHERE MaSach = @MaSach";

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
                            using (SqlCommand cmdHoan = new SqlCommand(queryHoanSach, conn, transaction))
                            {
                                cmdHoan.Parameters.Add("@SoLuongMuon", SqlDbType.Int).Value = chiTiet.SoLuongMuon;
                                cmdHoan.Parameters.Add("@MaSach", SqlDbType.VarChar, 10).Value = chiTiet.MaSach;
                                cmdHoan.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdCapNhat = new SqlCommand(queryCapNhat, conn, transaction))
                            {
                                cmdCapNhat.Parameters.Add("@NgayTra", SqlDbType.DateTime).Value = DateTime.Now;
                                cmdCapNhat.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 10).Value = maPhieu;
                                cmdCapNhat.Parameters.Add("@MaSach", SqlDbType.VarChar, 10).Value = chiTiet.MaSach;
                                cmdCapNhat.ExecuteNonQuery();
                            }
                        }

                        // THÊM: đóng phiếu mượn khi mọi chi tiết đã trả (bản cũ thiếu hoàn toàn)
                        string queryDongPhieu = @"
                            UPDATE PhieuMuon SET TrangThai = N'Đã trả'
                            WHERE MaPhieu = @MaPhieu
                              AND NOT EXISTS (
                                  SELECT 1 FROM ChiTietPhieuMuon
                                  WHERE MaPhieu = @MaPhieu AND TrangThai <> N'Đã trả'
                              )";
                        using (SqlCommand cmdDong = new SqlCommand(queryDongPhieu, conn, transaction))
                        {
                            cmdDong.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 10).Value = maPhieu;
                            cmdDong.ExecuteNonQuery();
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
        }

        // ==========================================
        // 3. LẤY DANH SÁCH PHIẾU MƯỢN (JOIN lấy Tên Độc Giả)
        // ==========================================
        public List<PhieuMuonDTO> LayDanhSachPhieuMuon()
        {
            List<PhieuMuonDTO> list = new List<PhieuMuonDTO>();
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                // SỬA: LEFT JOIN vì MaDocGia cho phép NULL; lấy thêm TrangThai + DiaChi
                string query = @"
                    SELECT pm.MaPhieu, pm.MaDocGia, dg.TenDocGia, dg.DiaChi,
                           pm.NgayMuon, pm.HanTra, pm.TrangThai
                    FROM PhieuMuon pm
                    LEFT JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhieuMuonDTO pm = new PhieuMuonDTO();
                        pm.MaPhieu = reader["MaPhieu"].ToString();
                        // SỬA: MaDG -> MaDocGia + xử lý DBNull
                        pm.MaDocGia = reader["MaDocGia"] == DBNull.Value ? null : reader["MaDocGia"].ToString();
                        pm.TenDocGia = reader["TenDocGia"] == DBNull.Value ? null : reader["TenDocGia"].ToString();
                        pm.DiaChi = reader["DiaChi"] == DBNull.Value ? null : reader["DiaChi"].ToString();
                        // SỬA: NgayMuon / HanTra NULLABLE
                        pm.NgayMuon = reader["NgayMuon"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayMuon"]);
                        pm.HanTra = reader["HanTra"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["HanTra"]);
                        pm.TrangThai = reader["TrangThai"] == DBNull.Value ? null : reader["TrangThai"].ToString();
                        list.Add(pm);
                    }
                }
            }
            return list;
        }

        // ==========================================
        // 4. LẤY CHI TIẾT 1 PHIẾU MƯỢN (JOIN lấy Tên Sách)
        // ==========================================
        public List<ChiTietPhieuMuonDTO> LayChiTietCuaPhieu(string maPhieu)
        {
            List<ChiTietPhieuMuonDTO> list = new List<ChiTietPhieuMuonDTO>();
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                // SỬA: bổ sung TienPhat + TinhTrangKhiTra theo đúng schema
                string query = @"
                    SELECT ct.MaPhieu, ct.MaSach, s.TenSach, ct.SoLuongMuon,
                           ct.TrangThai, ct.NgayTra, ct.TienPhat, ct.TinhTrangKhiTra
                    FROM ChiTietPhieuMuon ct
                    JOIN Sach s ON ct.MaSach = s.MaSach
                    WHERE ct.MaPhieu = @MaPhieu";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@MaPhieu", SqlDbType.VarChar, 10).Value = maPhieu;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChiTietPhieuMuonDTO ct = new ChiTietPhieuMuonDTO();
                            ct.MaPhieu = reader["MaPhieu"].ToString();
                            ct.MaSach = reader["MaSach"].ToString();
                            ct.TenSach = reader["TenSach"] == DBNull.Value ? null : reader["TenSach"].ToString();
                            ct.SoLuongMuon = Convert.ToInt32(reader["SoLuongMuon"]);
                            ct.TrangThai = reader["TrangThai"].ToString();

                            if (reader["NgayTra"] != DBNull.Value)
                                ct.NgayTra = Convert.ToDateTime(reader["NgayTra"]);
                            if (reader["TienPhat"] != DBNull.Value)
                                ct.TienPhat = Convert.ToDecimal(reader["TienPhat"]);
                            if (reader["TinhTrangKhiTra"] != DBNull.Value)
                                ct.TinhTrangKhiTra = reader["TinhTrangKhiTra"].ToString();

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
                    // SỬA: thêm JOIN DocGia để lọc IsDeleted = 0
                    string query = @"
                        SELECT ISNULL(SUM(ct.SoLuongMuon), 0)
                        FROM ChiTietPhieuMuon ct
                        JOIN PhieuMuon pm ON ct.MaPhieu = pm.MaPhieu
                        JOIN DocGia    dg ON pm.MaDocGia = dg.MaDocGia
                        WHERE pm.MaDocGia = @MaDocGia
                          AND ct.TrangThai = N'Đang mượn'
                          AND dg.IsDeleted = 0";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@MaDocGia", SqlDbType.VarChar, 10).Value = maDocGia;
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
        // 6. ĐẾM SỐ LƯỢNG ĐANG MƯỢN CỦA 1 CUỐN SÁCH (Dùng cho SachBLL)
        public int DemSoLuongDangMuonCuaSach(string maSach)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT ISNULL(SUM(SoLuongMuon), 0)
                        FROM ChiTietPhieuMuon
                        WHERE MaSach = @MaSach AND TrangThai = N'Đang mượn'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@MaSach", SqlDbType.VarChar, 10).Value = maSach;
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đếm sách đang mượn theo sách: " + ex.Message);
                return 0;
            }
        }
    }
}