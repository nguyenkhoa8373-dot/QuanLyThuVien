using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    internal static class TruyVanMuon
    {
        public const string DangMuon = "Đang mượn";
        public const string DaTra = "Đã trả";

        public static bool LaDangMuon(ChiTietPhieuMuonDTO ct)
        {
            return string.Equals((ct.TrangThai ?? string.Empty).Trim(), DangMuon, StringComparison.OrdinalIgnoreCase);
        }

        public static List<(PhieuMuonDTO Phieu, ChiTietPhieuMuonDTO ChiTiet)> LayChiTietDangMuon(
            PhieuMuonDAL phieuMuonDAL, string? maDocGia = null)
        {
            var ketQua = new List<(PhieuMuonDTO, ChiTietPhieuMuonDTO)>();

            foreach (var phieu in phieuMuonDAL.LayDanhSachPhieuMuon())
            {
                if (maDocGia != null &&
                    !string.Equals((phieu.MaDocGia ?? string.Empty).Trim(), maDocGia, StringComparison.OrdinalIgnoreCase))
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

        public static int DemSachDangMuonCuaDocGia(PhieuMuonDAL phieuMuonDAL, string maDocGia)
        {
            return phieuMuonDAL.DemSachDangMuonCuaDocGia(maDocGia);
        }

        public static int DemSachDangMuon(PhieuMuonDAL phieuMuonDAL, string maSach)
        {
            return phieuMuonDAL.DemSoLuongDangMuonCuaSach(maSach);
        }
    }
}