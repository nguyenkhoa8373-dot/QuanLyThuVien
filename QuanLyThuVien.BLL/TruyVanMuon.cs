using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    /// <summary>
    /// Các truy vấn dùng chung: tìm sách đang được mượn, đếm số cuốn đang mượn...
    /// Viết trên các hàm DAL sẵn có nên không cần sửa DAL.
    /// </summary>
    internal static class TruyVanMuon
    {
        public const string DangMuon = "Đang mượn";
        public const string DaTra = "Đã trả";

        public static bool LaDangMuon(ChiTietPhieuMuonDTO ct)
        {
            return string.Equals((ct.TrangThai ?? string.Empty).Trim(), DangMuon, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Lấy các dòng chi tiết đang mượn (kèm phiếu chứa nó).
        /// maDG != null thì chỉ xét phiếu của độc giả đó.
        /// </summary>
        public static List<(PhieuMuonDTO Phieu, ChiTietPhieuMuonDTO ChiTiet)> LayChiTietDangMuon(
            PhieuMuonDAL phieuMuonDAL, string? maDG = null)
        {
            var ketQua = new List<(PhieuMuonDTO, ChiTietPhieuMuonDTO)>();

            foreach (var phieu in phieuMuonDAL.LayDanhSachPhieuMuon())
            {
                if (maDG != null &&
                    !string.Equals((phieu.MaDG ?? string.Empty).Trim(), maDG, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var ct in phieuMuonDAL.LayChiTietCuaPhieu(phieu.MaPhieu))
                {
                    if (LaDangMuon(ct))
                    {
                        ketQua.Add((phieu, ct));
                    }
                }
            }

            return ketQua;
        }

        /// <summary>Tổng số cuốn độc giả đang mượn mà chưa trả.</summary>
        public static int DemSachDangMuonCuaDocGia(PhieuMuonDAL phieuMuonDAL, string maDG)
        {
            return LayChiTietDangMuon(phieuMuonDAL, maDG).Sum(x => x.ChiTiet.SoLuongMuon);
        }

        /// <summary>Tổng số cuốn của một đầu sách đang được mượn mà chưa trả.</summary>
        public static int DemSachDangMuon(PhieuMuonDAL phieuMuonDAL, string maSach)
        {
            return LayChiTietDangMuon(phieuMuonDAL)
                .Where(x => string.Equals((x.ChiTiet.MaSach ?? string.Empty).Trim(), maSach, StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.ChiTiet.SoLuongMuon);
        }
    }
}
