using System;

namespace QuanLyThuVien.DTO
{
    public class ChiTietPhieuMuonDTO
    {
        // Khóa chính phức hợp (NOT NULL) -> Khởi tạo string.Empty để hết warning CS8618
        public string MaPhieu { get; set; } = string.Empty;
        public string MaSach { get; set; } = string.Empty;

        // NOT NULL
        public int SoLuongMuon { get; set; }
        public string TrangThai { get; set; } = "Đang mượn";

        // NULLABLE
        public DateTime? NgayTra { get; set; }
        public decimal? TienPhat { get; set; }
        public string? TinhTrangKhiTra { get; set; } // Thêm dấu ? vì cột SQL cho phép NULL

        // --- Thuộc tính JOIN (Không có trong bảng CSDL) ---
        public string? TenSach { get; set; } // Khắc phục lỗi CS1061
    }
}