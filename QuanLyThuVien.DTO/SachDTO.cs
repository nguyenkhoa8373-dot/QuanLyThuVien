namespace QuanLyThuVien.DTO
{
    public class SachDTO
    {
        public string MaSach { get; set; } = string.Empty; // NOT NULL
        public string TenSach { get; set; } = string.Empty; // NOT NULL
        public string? TheLoai { get; set; } // NULL
        public int? SoLuongTon { get; set; } // NULL
        public bool? IsDeleted { get; set; } // NULL
    }
}