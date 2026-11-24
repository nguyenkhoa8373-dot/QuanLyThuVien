using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    public class SachBLL
    {
        private readonly SachDAL sachDAL = new SachDAL();
        private readonly PhieuMuonDAL phieuMuonDAL = new PhieuMuonDAL();

        public List<SachDTO> LayDanhSachSach()
        {
            return sachDAL.LayDanhSachSach();
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

            if (TonTai(sach.MaSach))
            {
                thongBao = "Mã sách đã tồn tại.";
                return false;
            }

            bool ketQua = sachDAL.ThemSach(sach);
            thongBao = ketQua ? "Thêm sách thành công." : "Không thể thêm sách (mã có thể đã được dùng trước đây).";
            return ketQua;
        }

        public bool SuaSach(SachDTO sach, out string thongBao)
        {
            if (!KiemTraSach(sach, out thongBao))
            {
                return false;
            }

            if (!TonTai(sach.MaSach))
            {
                thongBao = "Không tìm thấy mã sách cần sửa.";
                return false;
            }

            bool ketQua = sachDAL.SuaSach(sach);
            thongBao = ketQua ? "Cập nhật sách thành công." : "Không thể cập nhật sách.";
            return ketQua;
        }

        public bool XoaSach(string maSach, out string thongBao)
        {
            if (string.IsNullOrWhiteSpace(maSach))
            {
                thongBao = "Mã sách không được để trống.";
                return false;
            }

            maSach = maSach.Trim();

            if (!TonTai(maSach))
            {
                thongBao = "Không tìm thấy mã sách cần xóa.";
                return false;
            }

            int dangMuon = phieuMuonDAL.DemSoLuongDangMuonCuaSach(maSach);
            if (dangMuon > 0)
            {
                thongBao = $"Sách đang được mượn ({dangMuon} cuốn chưa trả), không thể xóa.";
                return false;
            }

            bool ketQua = sachDAL.XoaSach(maSach);
            thongBao = ketQua ? "Xóa sách thành công." : "Không thể xóa sách.";
            return ketQua;
        }

        // ================= HÀM PHỤ =================

        private bool TonTai(string maSach)
        {
            return LayDanhSachSach().Any(s =>
                string.Equals((s.MaSach ?? string.Empty).Trim(), maSach, StringComparison.OrdinalIgnoreCase));
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

            if (sach.MaSach.Length > 10)
            {
                thongBao = "Mã sách tối đa 10 ký tự.";
                return false;
            }

            if (sach.TenSach.Length > 100)
            {
                thongBao = "Tên sách tối đa 100 ký tự.";
                return false;
            }

            if (sach.TheLoai.Length > 50)
            {
                thongBao = "Thể loại tối đa 50 ký tự.";
                return false;
            }

            if (sach.SoLuongTon < 0)
            {
                thongBao = "Số lượng tồn không được nhỏ hơn 0.";
                return false;
            }

            thongBao = string.Empty;
            return true;
        }
    }
}