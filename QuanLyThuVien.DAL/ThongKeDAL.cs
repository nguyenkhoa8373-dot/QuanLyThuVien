using QuanLyThuVien.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyThuVien.DAL
{
    public class ThongKeDAL
    {
        // Hàm 1: Lấy danh sách độc giả quá hạn (đã sửa từ trước)
        public List<PhieuMuonDTO> LayDanhSachDocGiaQuaHan()
        {
            var list = new List<PhieuMuonDTO>();
            const string sql = @"
SELECT pm.MaPhieu, pm.MaDocGia, pm.NgayMuon, pm.HanTra, pm.TrangThai,
       dg.TenDocGia, dg.DiaChi
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
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PhieuMuonDTO
                        {
                            MaPhieu = rd["MaPhieu"] == DBNull.Value ? string.Empty : rd["MaPhieu"].ToString(),
                            MaDocGia = rd["MaDocGia"] == DBNull.Value ? null : rd["MaDocGia"].ToString(),
                            NgayMuon = rd["NgayMuon"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rd["NgayMuon"]),
                            HanTra = rd["HanTra"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rd["HanTra"]),
                            TrangThai = rd["TrangThai"] == DBNull.Value ? null : rd["TrangThai"].ToString(),
                            TenDocGia = rd["TenDocGia"] == DBNull.Value ? null : rd["TenDocGia"].ToString(),
                            DiaChi = rd["DiaChi"] == DBNull.Value ? null : rd["DiaChi"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Hàm 2: Đếm số sách đang mượn (Chính là hàm trong ảnh của bạn)
        public Dictionary<string, int> DemSachDangMuonTheoDocGia()
        {
            var result = new Dictionary<string, int>();
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
                        // Xử lý triệt để cảnh báo CS8600
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