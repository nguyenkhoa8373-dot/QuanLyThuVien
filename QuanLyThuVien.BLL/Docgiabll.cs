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

        private readonly DocGiaDAL docGiaDAL;
        private readonly PhieuMuonDAL phieuMuonDAL;

        public DocGiaBLL() : this(new DocGiaDAL(), new PhieuMuonDAL())
        {
        }

        public DocGiaBLL(DocGiaDAL docGiaDAL, PhieuMuonDAL phieuMuonDAL)
        {
            this.docGiaDAL = docGiaDAL;
            this.phieuMuonDAL = phieuMuonDAL;
        }

        public List<DocGiaDTO> LayDanhSachDocGia()
        {
            return docGiaDAL.LayDanhSachDocGia();
        }

        public DocGiaDTO? LayTheoMa(string maDocGia)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
            {
                return null;
            }

            maDocGia = maDocGia.Trim();

            return docGiaDAL.LayDanhSachDocGia()
                .FirstOrDefault(d => string.Equals((d.MaDocGia ?? string.Empty).Trim(), maDocGia, StringComparison.OrdinalIgnoreCase));
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
                    (dg.MaDocGia?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (dg.TenDocGia?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
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
                if (LayTheoMa(docGia.MaDocGia) != null)
                {
                    thongBao = "Mã độc giả đã tồn tại.";
                    return false;
                }

                bool ketQua = docGiaDAL.ThemDocGia(docGia);

                // DAL không ném lỗi mà chỉ trả false, nên không phân biệt được nguyên nhân.
                // Trường hợp hay gặp: mã này từng thuộc một độc giả đã bị xóa (xóa mềm) nên vẫn chiếm khóa chính.
                thongBao = ketQua
                    ? "Thêm độc giả thành công."
                    : "Không thể thêm độc giả. Mã có thể đã từng được sử dụng (độc giả đã xóa) hoặc lỗi kết nối CSDL.";
                return ketQua;
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
                if (LayTheoMa(docGia.MaDocGia) == null)
                {
                    thongBao = "Không tìm thấy mã độc giả cần sửa.";
                    return false;
                }

                bool ketQua = docGiaDAL.SuaDocGia(docGia);
                thongBao = ketQua ? "Cập nhật độc giả thành công." : "Không thể cập nhật độc giả.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool XoaDocGia(string maDocGia, out string thongBao)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
            {
                thongBao = "Mã độc giả không được để trống.";
                return false;
            }

            maDocGia = maDocGia.Trim();

            try
            {
                if (LayTheoMa(maDocGia) == null)
                {
                    thongBao = "Không tìm thấy mã độc giả cần xóa.";
                    return false;
                }

                int dangMuon = TruyVanMuon.DemSachDangMuonCuaDocGia(phieuMuonDAL, maDocGia);
                if (dangMuon > 0)
                {
                    thongBao = $"Độc giả còn {dangMuon} cuốn sách chưa trả, không thể xóa.";
                    return false;
                }

                bool ketQua = docGiaDAL.XoaDocGia(maDocGia);
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

        private bool KiemTraDocGia(DocGiaDTO docGia, out string thongBao)
        {
            if (docGia == null)
            {
                thongBao = "Thông tin độc giả không được để trống.";
                return false;
            }

            docGia.MaDocGia = (docGia.MaDocGia ?? string.Empty).Trim();
            docGia.TenDocGia = (docGia.TenDocGia ?? string.Empty).Trim();
            docGia.SoDienThoai = (docGia.SoDienThoai ?? string.Empty).Trim();
            // DiaChi null sẽ làm AddWithValue lỗi ở DAL, nên luôn chuẩn hóa về chuỗi rỗng
            docGia.DiaChi = (docGia.DiaChi ?? string.Empty).Trim();

            if (docGia.MaDocGia.Length == 0)
            {
                thongBao = "Mã độc giả không được để trống.";
                return false;
            }

            if (docGia.TenDocGia.Length == 0)
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
