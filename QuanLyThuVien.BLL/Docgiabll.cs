using System.Data.SqlClient;
using System.Text.RegularExpressions;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    public class DocGiaBLL
    {
        private const string LoiCSDL = "Lỗi truy cập cơ sở dữ liệu: ";

        // Số điện thoại VN: 10 chữ số, bắt đầu bằng 0
        private static readonly Regex MauSoDienThoai = new Regex(@"^0\d{9}$");

        private readonly IDocGiaRepository docGiaRepo;

        // Tùy chọn: có thì XoaDocGia sẽ kiểm tra độc giả còn sách chưa trả.
        private readonly IPhieuMuonRepository? phieuMuonRepo;

        public DocGiaBLL(IDocGiaRepository docGiaRepo, IPhieuMuonRepository? phieuMuonRepo = null)
        {
            this.docGiaRepo = docGiaRepo;
            this.phieuMuonRepo = phieuMuonRepo;
        }

        public List<DocGiaDTO> LayDanhSachDocGia()
        {
            return docGiaRepo.LayDanhSachDocGia();
        }

        public List<DocGiaDTO> TimDocGia(string tuKhoa)
        {
            var danhSach = LayDanhSachDocGia();

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                return danhSach;
            }

            tuKhoa = tuKhoa.Trim();

            return danhSach
                .Where(dg =>
                    (dg.MaDG?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (dg.TenDG?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (dg.SoDienThoai?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        public bool ThemDocGia(DocGiaDTO docGia, out string thongBao)
        {
            if (!KiemTraDocGia(docGia, out thongBao))
            {
                return false;
            }

            try
            {
                if (TimTheoMa(docGia.MaDG) != null)
                {
                    thongBao = "Mã độc giả đã tồn tại.";
                    return false;
                }

                bool ketQua = docGiaRepo.ThemDocGia(docGia);
                thongBao = ketQua ? "Thêm độc giả thành công." : "Không thể thêm độc giả.";
                return ketQua;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                thongBao = "Mã độc giả này đã được dùng trước đây. Vui lòng dùng mã khác.";
                return false;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool SuaDocGia(DocGiaDTO docGia, out string thongBao)
        {
            if (!KiemTraDocGia(docGia, out thongBao))
            {
                return false;
            }

            try
            {
                if (TimTheoMa(docGia.MaDG) == null)
                {
                    thongBao = "Không tìm thấy mã độc giả cần sửa.";
                    return false;
                }

                bool ketQua = docGiaRepo.SuaDocGia(docGia);
                thongBao = ketQua ? "Cập nhật độc giả thành công." : "Không thể cập nhật độc giả.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool XoaDocGia(string maDG, out string thongBao)
        {
            if (string.IsNullOrWhiteSpace(maDG))
            {
                thongBao = "Mã độc giả không được để trống.";
                return false;
            }

            maDG = maDG.Trim();

            try
            {
                if (TimTheoMa(maDG) == null)
                {
                    thongBao = "Không tìm thấy mã độc giả cần xóa.";
                    return false;
                }

                if (phieuMuonRepo != null)
                {
                    int dangMuon = TruyVanMuon.DemSachDangMuonCuaDocGia(phieuMuonRepo, maDG);
                    if (dangMuon > 0)
                    {
                        thongBao = $"Độc giả còn {dangMuon} cuốn sách chưa trả, không thể xóa.";
                        return false;
                    }
                }

                bool ketQua = docGiaRepo.XoaDocGia(maDG);
                thongBao = ketQua ? "Xóa độc giả thành công." : "Không thể xóa độc giả.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        // ================= HÀM PHỤ =================

        private DocGiaDTO? TimTheoMa(string maDG)
        {
            return docGiaRepo.LayDanhSachDocGia()
                .FirstOrDefault(d => string.Equals((d.MaDG ?? string.Empty).Trim(), maDG, StringComparison.OrdinalIgnoreCase));
        }

        private bool KiemTraDocGia(DocGiaDTO docGia, out string thongBao)
        {
            if (docGia == null)
            {
                thongBao = "Thông tin độc giả không được để trống.";
                return false;
            }

            docGia.MaDG = (docGia.MaDG ?? string.Empty).Trim();
            docGia.TenDG = (docGia.TenDG ?? string.Empty).Trim();
            docGia.SoDienThoai = (docGia.SoDienThoai ?? string.Empty).Trim();

            if (docGia.MaDG.Length == 0)
            {
                thongBao = "Mã độc giả không được để trống.";
                return false;
            }

            if (docGia.TenDG.Length == 0)
            {
                thongBao = "Tên độc giả không được để trống.";
                return false;
            }

            if (docGia.SoDienThoai.Length == 0)
            {
                thongBao = "Số điện thoại không được để trống.";
                return false;
            }

            if (!MauSoDienThoai.IsMatch(docGia.SoDienThoai))
            {
                thongBao = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.";
                return false;
            }

            thongBao = string.Empty;
            return true;
        }
    }
}
