using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyThuVien.DAL
{
    public class ThongKeDAL
    {
        // 1. THỐNG KÊ TOP SÁCH ĐƯỢC MƯỢN NHIỀU NHẤT
        public DataTable LayTopSachMuonNhieuNhat(int soLuongTop)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT TOP (@Top) s.MaSach, s.TenSach, s.TheLoai, SUM(ct.SoLuongMuon) AS TongLuotMuon
                    FROM ChiTietPhieuMuon ct
                    JOIN Sach s ON ct.MaSach = s.MaSach
                    GROUP BY s.MaSach, s.TenSach, s.TheLoai
                    ORDER BY TongLuotMuon DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Top", soLuongTop);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // 2. LẤY DANH SÁCH ĐỘC GIẢ ĐANG QUÁ HẠN CHƯA TRẢ (Để nhắc nhở)[cite: 9]
        public DataTable LayDanhSachDocGiaQuaHan()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                // Tìm phiếu mượn có Hạn trả < Ngày hiện tại và Trạng thái vẫn là 'Đang mượn'
                string query = @"
                    SELECT dg.MaDocGia, dg.TenDocGia, pm.MaPhieu, pm.HanTra, 
                           DATEDIFF(day, pm.HanTra, GETDATE()) AS SoNgayTre,
                           (DATEDIFF(day, pm.HanTra, GETDATE()) * 5000) AS TienPhatDuKien
                    FROM PhieuMuon pm
                    JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                    JOIN ChiTietPhieuMuon ct ON pm.MaPhieu = ct.MaPhieu
                    WHERE ct.TrangThai = N'Đang mượn' AND pm.HanTra < GETDATE()
                    GROUP BY dg.MaDocGia, dg.TenDocGia, pm.MaPhieu, pm.HanTra";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // 3. TÍNH TỔNG TIỀN PHẠT THU ĐƯỢC TRONG THÁNG[cite: 9]
        public decimal TinhTongTienPhatThangNay()
        {
            decimal tongTien = 0;
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT ISNULL(SUM(TienPhat), 0) 
                    FROM ChiTietPhieuMuon 
                    WHERE MONTH(NgayTra) = MONTH(GETDATE()) AND YEAR(NgayTra) = YEAR(GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    tongTien = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            return tongTien;
        }
    }
}