using System.Text.RegularExpressions;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    public class PhieuMuonBLL
    {
        private const string LoiCSDL = "Lỗi truy cập cơ sở dữ liệu: ";
        private const int DoDaiMaToiDa = 10;

        // Quy định mượn trả
        public const int SoNgayMuonToiDa = 30;
        public const int SoSachToiDaMoiDocGia = 5;
        public const decimal TienPhatMoiNgay = 5000m;

        private readonly PhieuMuonDAL phieuMuonDAL;
        private readonly DocGiaDAL docGiaDAL;
        private readonly SachDAL sachDAL;

        public PhieuMuonBLL() : this(new PhieuMuonDAL(), new DocGiaDAL(), new SachDAL())
        {
        }

        public PhieuMuonBLL(PhieuMuonDAL phieuMuonDAL, DocGiaDAL docGiaDAL, SachDAL sachDAL)
        {
            this.phieuMuonDAL = phieuMuonDAL;
            this.docGiaDAL = docGiaDAL;
            this.sachDAL = sachDAL;
        }

        // ================= TRUY VẤN =================

        public List<PhieuMuonDTO> LayDanhSachPhieuMuon()
        {
            return phieuMuonDAL.LayDanhSachPhieuMuon();
        }

        public List<ChiTietPhieuMuonDTO> LayChiTietCuaPhieu(string maPhieu)
        {
            if (string.IsNullOrWhiteSpace(maPhieu))
            {
                return new List<ChiTietPhieuMuonDTO>();
            }

            return phieuMuonDAL.LayChiTietCuaPhieu(maPhieu.Trim());
        }

        /// <summary>Trạng thái tổng của phiếu: "Đang mượn", "Quá hạn" hoặc "Đã trả".</summary>
        public string LayTrangThaiPhieu(PhieuMuonDTO phieu)
        {
            var chiTiet = LayChiTietCuaPhieu(phieu.MaPhieu);
            bool conSachChuaTra = chiTiet.Any(TruyVanMuon.LaDangMuon);

            if (!conSachChuaTra)
            {
                return TruyVanMuon.DaTra;
            }

            return (phieu.HanTra.HasValue && phieu.HanTra.Value.Date < DateTime.Today)
                ? "Quá hạn"
                : TruyVanMuon.DangMuon;
        }

        // ================= MƯỢN SÁCH =================

        public bool TaoPhieuMuon(PhieuMuonDTO phieu, List<ChiTietPhieuMuonDTO> danhSachChiTiet, out string thongBao)
        {
            if (!KiemTraPhieu(phieu, danhSachChiTiet, out thongBao))
            {
                return false;
            }

            try
            {
                // 1. Độc giả phải tồn tại (và chưa bị xóa)
                bool docGiaTonTai = docGiaDAL.LayDanhSachDocGia()
                    .Any(d => string.Equals((d.MaDocGia ?? string.Empty).Trim(), phieu.MaDocGia, StringComparison.OrdinalIgnoreCase));

                if (!docGiaTonTai)
                {
                    thongBao = "Không tìm thấy độc giả.";
                    return false;
                }

                // 2. Sách phải tồn tại và đủ số lượng
                var dsSach = sachDAL.LayDanhSachSach();

                foreach (var ct in danhSachChiTiet)
                {
                    var sach = dsSach.FirstOrDefault(s =>
                        string.Equals((s.MaSach ?? string.Empty).Trim(), ct.MaSach, StringComparison.OrdinalIgnoreCase));

                    if (sach == null)
                    {
                        thongBao = $"Sách mã {ct.MaSach} không tồn tại.";
                        return false;
                    }

                    int tonKho = sach.SoLuongTon ?? 0;
                    if (tonKho < ct.SoLuongMuon)
                    {
                        thongBao = $"Sách \"{sach.TenSach}\" chỉ còn {tonKho} cuốn, không đủ để mượn {ct.SoLuongMuon} cuốn.";
                        return false;
                    }
                }

                // 3. Độc giả đang có sách quá hạn thì không được mượn thêm
                var quaHan = TruyVanMuon.LayChiTietDangMuon(phieuMuonDAL, phieu.MaDocGia)
                    .FirstOrDefault(x => x.Phieu.HanTra.HasValue && x.Phieu.HanTra.Value.Date < DateTime.Today);

                if (quaHan.Phieu != null)
                {
                    thongBao = $"Độc giả đang có sách quá hạn chưa trả (phiếu {quaHan.Phieu.MaPhieu}), không thể mượn thêm.";
                    return false;
                }

                // 4. Tổng số sách đang mượn + sách mới không vượt quá hạn mức
                int dangMuon = TruyVanMuon.DemSachDangMuonCuaDocGia(phieuMuonDAL, phieu.MaDocGia!);
                int muonMoi = danhSachChiTiet.Sum(x => x.SoLuongMuon);

                if (dangMuon + muonMoi > SoSachToiDaMoiDocGia)
                {
                    thongBao = $"Độc giả đang mượn {dangMuon} cuốn. Chỉ được mượn tối đa {SoSachToiDaMoiDocGia} cuốn cùng lúc (hiện chọn thêm {muonMoi} cuốn).";
                    return false;
                }

                // 5. Mã phiếu
                var dsPhieu = phieuMuonDAL.LayDanhSachPhieuMuon();

                if (string.IsNullOrEmpty(phieu.MaPhieu))
                {
                    phieu.MaPhieu = SinhMaPhieu(dsPhieu);
                }
                else if (dsPhieu.Any(p => string.Equals((p.MaPhieu ?? string.Empty).Trim(), phieu.MaPhieu, StringComparison.OrdinalIgnoreCase)))
                {
                    thongBao = "Mã phiếu mượn đã tồn tại.";
                    return false;
                }

                phieu.TrangThai = TruyVanMuon.DangMuon;

                // 6. Lưu (DAL dùng transaction: tạo phiếu + chi tiết + trừ kho)
                bool ketQua = phieuMuonDAL.TaoPhieuMuon(phieu, danhSachChiTiet);
                thongBao = ketQua
                    ? $"Tạo phiếu mượn {phieu.MaPhieu} thành công."
                    : "Không thể tạo phiếu mượn.";
                return ketQua;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        // ================= TRẢ SÁCH =================

        public bool TraSach(string maPhieu, out string thongBao)
        {
            if (string.IsNullOrWhiteSpace(maPhieu))
            {
                thongBao = "Mã phiếu không được để trống.";
                return false;
            }

            maPhieu = maPhieu.Trim();

            try
            {
                var phieu = phieuMuonDAL.LayDanhSachPhieuMuon()
                    .FirstOrDefault(p => string.Equals((p.MaPhieu ?? string.Empty).Trim(), maPhieu, StringComparison.OrdinalIgnoreCase));

                if (phieu == null)
                {
                    thongBao = "Không tìm thấy phiếu mượn.";
                    return false;
                }

                var chiTietTruocTra = phieuMuonDAL.LayChiTietCuaPhieu(phieu.MaPhieu);
                var maSachChuaTra = chiTietTruocTra
                    .Where(TruyVanMuon.LaDangMuon)
                    .Select(x => x.MaSach)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (maSachChuaTra.Count == 0)
                {
                    thongBao = "Phiếu này đã trả hết sách.";
                    return false;
                }

                bool ketQua = phieuMuonDAL.TraSach(phieu.MaPhieu);

                if (!ketQua)
                {
                    thongBao = "Không thể trả sách.";
                    return false;
                }

                // Đọc lại tiền phạt thực tế đã được SQL Server tính và lưu
                var chiTietSauTra = phieuMuonDAL.LayChiTietCuaPhieu(phieu.MaPhieu);
                decimal tongTienPhat = chiTietSauTra
                    .Where(ct => maSachChuaTra.Contains(ct.MaSach))
                    .Sum(ct => ct.TienPhat ?? 0m);

                int soNgayTre = phieu.HanTra.HasValue
                    ? Math.Max(0, (DateTime.Today - phieu.HanTra.Value.Date).Days)
                    : 0;

                if (tongTienPhat > 0 || soNgayTre > 0)
                {
                    thongBao = $"Trả sách thành công. Trễ {soNgayTre} ngày, tiền phạt: {tongTienPhat:N0}đ.";
                }
                else
                {
                    thongBao = "Trả sách thành công.";
                }

                return true;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        // ================= HÀM PHỤ =================

        private static bool KiemTraPhieu(PhieuMuonDTO phieu, List<ChiTietPhieuMuonDTO> danhSachChiTiet, out string thongBao)
        {
            if (phieu == null)
            {
                thongBao = "Thông tin phiếu mượn không được để trống.";
                return false;
            }

            phieu.MaPhieu = (phieu.MaPhieu ?? string.Empty).Trim();
            phieu.MaDocGia = (phieu.MaDocGia ?? string.Empty).Trim();

            if (phieu.MaPhieu.Length > DoDaiMaToiDa)
            {
                thongBao = $"Mã phiếu mượn không được vượt quá {DoDaiMaToiDa} ký tự.";
                return false;
            }

            if (phieu.MaDocGia.Length == 0)
            {
                thongBao = "Vui lòng chọn độc giả.";
                return false;
            }

            if (!phieu.NgayMuon.HasValue || phieu.NgayMuon.Value == default)
            {
                phieu.NgayMuon = DateTime.Now;
            }

            if (!phieu.HanTra.HasValue)
            {
                thongBao = "Vui lòng chọn hạn trả.";
                return false;
            }

            if (phieu.HanTra.Value.Date <= phieu.NgayMuon.Value.Date)
            {
                thongBao = "Hạn trả phải sau ngày mượn.";
                return false;
            }

            if ((phieu.HanTra.Value.Date - phieu.NgayMuon.Value.Date).Days > SoNgayMuonToiDa)
            {
                thongBao = $"Thời hạn mượn tối đa là {SoNgayMuonToiDa} ngày.";
                return false;
            }

            if (danhSachChiTiet == null || danhSachChiTiet.Count == 0)
            {
                thongBao = "Phiếu mượn phải có ít nhất một cuốn sách.";
                return false;
            }

            var daCo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int tongSoCuon = 0;

            foreach (var ct in danhSachChiTiet)
            {
                if (ct == null)
                {
                    thongBao = "Chi tiết phiếu mượn không hợp lệ.";
                    return false;
                }

                ct.MaSach = (ct.MaSach ?? string.Empty).Trim();

                if (ct.MaSach.Length == 0)
                {
                    thongBao = "Mã sách trong phiếu không được để trống.";
                    return false;
                }

                if (ct.SoLuongMuon <= 0)
                {
                    thongBao = $"Số lượng mượn của sách {ct.MaSach} phải lớn hơn 0.";
                    return false;
                }

                if (!daCo.Add(ct.MaSach))
                {
                    thongBao = $"Sách {ct.MaSach} bị lặp trong phiếu. Hãy gộp lại thành một dòng.";
                    return false;
                }

                tongSoCuon += ct.SoLuongMuon;
            }

            if (tongSoCuon > SoSachToiDaMoiDocGia)
            {
                thongBao = $"Mỗi lần chỉ được mượn tối đa {SoSachToiDaMoiDocGia} cuốn (hiện đang chọn {tongSoCuon}).";
                return false;
            }

            thongBao = string.Empty;
            return true;
        }

        private static string SinhMaPhieu(List<PhieuMuonDTO> danhSachPhieu)
        {
            int max = 0;

            foreach (var p in danhSachPhieu)
            {
                var m = Regex.Match(p.MaPhieu ?? string.Empty, @"^PM(\d+)$", RegexOptions.IgnoreCase);

                if (m.Success && int.TryParse(m.Groups[1].Value, out int so) && so > max)
                {
                    max = so;
                }
            }

            return "PM" + (max + 1).ToString("D3");
        }
    }
}