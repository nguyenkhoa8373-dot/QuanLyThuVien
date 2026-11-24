using System;

namespace QuanLyThuVien.DTO
{
    public class PhieuMuonDTO
    {
        // NOT NULL (Khóa chính)
        public string MaPhieu { get; set; } = string.Empty;

        // NULLABLE (Theo schema SQL)
        public string? MaDocGia { get; set; }
        public DateTime? NgayMuon { get; set; }
        public DateTime? HanTra { get; set; }
        public string? TrangThai { get; set; }

        // --- Thuộc tính JOIN ---
        public string? TenDocGia { get; set; }
        public string? DiaChi { get; set; }
    }
}