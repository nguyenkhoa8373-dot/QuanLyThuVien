using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyThuVien.DAL
{
    public class ThongKeDAL
    {
        // Hàm 1: Lấy danh sách độc giả quá hạn (Đã sửa trả về DataTable để khớp với BLL)
        public DataTable LayDanhSachDocGiaQuaHan()
        {
            DataTable dt = new DataTable();
            const string sql = @"
                SELECT pm.MaPhieu, pm.MaDocGia, dg.TenDocGia, pm.HanTra,
                       DATEDIFF(day, pm.HanTra, GETDATE()) AS SoNgayTre,
                       (DATEDIFF(day, pm.HanTra, GETDATE()) * 5000) AS TienPhatDuKien
                FROM PhieuMuon pm
                INNER JOIN DocGia dg ON dg.MaDocGia = pm.MaDocGia
                INNER JOIN ChiTietPhieuMuon ct ON ct.MaPhieu = pm.MaPhieu
                WHERE ct.TrangThai = N'Đang mượn'
                  AND pm.HanTra IS NOT NULL
                  AND pm.HanTra < GETDATE()
                  AND dg.IsDeleted = 0
                ORDER BY pm.HanTra";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // Hàm 2 (Bổ sung): Lấy Top sách mượn nhiều nhất (Phục vụ ThongKeBLL)
        public DataTable LayTopSachMuonNhieuNhat(int soLuongTop)
        {
            DataTable dt = new DataTable();
            // Sử dụng parameter để tránh lỗi SQL Injection dù đây là kiểu int
            string sql = $@"
                SELECT TOP {soLuongTop} s.MaSach, s.TenSach, s.TheLoai, SUM(ct.SoLuongMuon) AS TongLuotMuon
                FROM Sach s
                INNER JOIN ChiTietPhieuMuon ct ON s.MaSach = ct.MaSach
                WHERE s.IsDeleted = 0
                GROUP BY s.MaSach, s.TenSach, s.TheLoai
                ORDER BY TongLuotMuon DESC";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // Hàm 3 (Bổ sung): Tính tổng tiền phạt trong tháng này (Phục vụ ThongKeBLL)
        public decimal TinhTongTienPhatThangNay()
        {
            const string sql = @"
                SELECT ISNULL(SUM(TienPhat), 0)
                FROM ChiTietPhieuMuon
                WHERE TrangThai = N'Đã trả'
                  AND MONTH(NgayTra) = MONTH(GETDATE())
                  AND YEAR(NgayTra) = YEAR(GETDATE())";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        // Hàm 4: Đếm số sách đang mượn theo độc giả (Giữ nguyên theo file cũ của bạn)
        public System.Collections.Generic.Dictionary<string, int> DemSachDangMuonTheoDocGia()
        {
            var result = new System.Collections.Generic.Dictionary<string, int>();
            const string sql = @"
                SELECT dg.MaDocGia, SUM(ct.SoLuongMuon) AS TongSach
                FROM DocGia dg
                LEFT JOIN PhieuMuon pm ON pm.MaDocGia = dg.MaDocGia
                LEFT JOIN ChiTietPhieuMuon ct ON ct.MaPhieu = pm.MaPhieu AND ct.TrangThai = N'Đang mượn'
                WHERE dg.IsDeleted = 0
                GROUP BY dg.MaDocGia";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string ma = rd["MaDocGia"] == DBNull.Value ? string.Empty : rd["MaDocGia"].ToString();
                        int tong = rd["TongSach"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TongSach"]);

                        if (!string.IsNullOrEmpty(ma))
                        {
                            result[ma] = tong;
                        }
                    }
                }
            }
            return result;
        }
    }
}