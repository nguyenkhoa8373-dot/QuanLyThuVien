using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    public class SachBLL
    {
        private const string LoiCSDL = "Lỗi truy cập cơ sở dữ liệu: ";
        private const int SoLuongToiDa = 100000;

        private readonly SachDAL sachDAL;
        private readonly PhieuMuonDAL phieuMuonDAL;

        public SachBLL() : this(new SachDAL(), new PhieuMuonDAL())
        {
        }

        public SachBLL(SachDAL sachDAL, PhieuMuonDAL phieuMuonDAL)
        {
            this.sachDAL = sachDAL;
            this.phieuMuonDAL = phieuMuonDAL;
        }

        public List<SachDTO> LayDanhSachSach()
        {
            return sachDAL.LayDanhSachSach();
        }

        public SachDTO? LayTheoMa(string maSach)
        {
            if (string.IsNullOrWhiteSpace(maSach))
            {
                return null;
            }

            maSach = maSach.Trim();

            return sachDAL.LayDanhSachSach()
                .FirstOrDefault(s => string.Equals((s.MaSach ?? string.Empty).Trim(), maSach, StringComparison.OrdinalIgnoreCase));
        }

        public List<SachDTO> TimSach(string tuKhoa)
        {
            var danhSach = LayDanhSachSach();

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                return danhSach;
            }

            tuKhoa = tuKhoa.Trim();

            return danhSach
                .Where(sach =>
                    (sach.MaSach?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (sach.TenSach?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (sach.TheLoai?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        public bool ThemSach(SachDTO sach, out string thongBao)
        {
            if (!KiemTraSach(sach, out thongBao))
            {
                return false;
            }

            try
            {
                if (LayTheoMa(sach.MaSach) != null)
                {
                    thongBao = "Mã sách đã tồn tại.";
                    return false;
                }

                bool ketQua = sachDAL.ThemSach(sach);

                // DAL chỉ trả false, không cho biết nguyên nhân (ví dụ mã của sách đã xóa mềm vẫn chiếm khóa chính)
                thongBao = ketQua
                    ? "Thêm sách thành công."
                    : "Không thể thêm sách. Mã có thể đã từng được sử dụng (sách đã xóa) hoặc lỗi kết nối CSDL.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool SuaSach(SachDTO sach, out string thongBao)
        {
            if (!KiemTraSach(sach, out thongBao))
            {
                return false;
            }

            try
            {
                if (LayTheoMa(sach.MaSach) == null)
                {
                    thongBao = "Không tìm thấy mã sách cần sửa.";
                    return false;
                }

                // Lưu ý: SoLuongTon là số cuốn CÒN TRONG KHO (đã trừ số đang cho mượn),
                // nên sửa trực tiếp ở đây là điều chỉnh tồn kho.
                bool ketQua = sachDAL.SuaSach(sach);
                thongBao = ketQua ? "Cập nhật sách thành công." : "Không thể cập nhật sách.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool XoaSach(string maSach, out string thongBao)
        {
            if (string.IsNullOrWhiteSpace(maSach))
            {
                thongBao = "Mã sách không được để trống.";
                return false;
            }

            maSach = maSach.Trim();

            try
            {
                if (LayTheoMa(maSach) == null)
                {
                    thongBao = "Không tìm thấy mã sách cần xóa.";
                    return false;
                }

                int dangMuon = TruyVanMuon.DemSachDangMuon(phieuMuonDAL, maSach);
                if (dangMuon > 0)
                {
                    thongBao = $"Sách đang được mượn {dangMuon} cuốn chưa trả, không thể xóa.";
                    return false;
                }

                bool ketQua = sachDAL.XoaSach(maSach);
                thongBao = ketQua ? "Xóa sách thành công." : "Không thể xóa sách.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        private bool KiemTraSach(SachDTO sach, out string thongBao)
        {
            if (sach == null)
            {
                thongBao = "Thông tin sách không được để trống.";
                return false;
            }

            sach.MaSach = (sach.MaSach ?? string.Empty).Trim();
            sach.TenSach = (sach.TenSach ?? string.Empty).Trim();
            sach.TheLoai = (sach.TheLoai ?? string.Empty).Trim();

            if (sach.MaSach.Length == 0)
            {
                thongBao = "Mã sách không được để trống.";
                return false;
            }

            if (sach.TenSach.Length == 0)
            {
                thongBao = "Tên sách không được để trống.";
                return false;
            }

            if (sach.TheLoai.Length == 0)
            {
                thongBao = "Thể loại không được để trống.";
                return false;
            }

            if (sach.SoLuongTon < 0)
            {
                thongBao = "Số lượng tồn không được nhỏ hơn 0.";
                return false;
            }

            if (sach.SoLuongTon > SoLuongToiDa)
            {
                thongBao = $"Số lượng tồn không được lớn hơn {SoLuongToiDa}.";
                return false;
            }

            thongBao = string.Empty;
            return true;
        }
    }
}
