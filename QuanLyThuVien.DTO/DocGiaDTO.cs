namespace QuanLyThuVien.DTO
{
    public class DocGiaDTO
    {
        public string MaDocGia { get; set; } = string.Empty; // NOT NULL
        public string TenDocGia { get; set; } = string.Empty; // NOT NULL
        public string? SoDienThoai { get; set; } // NULL
        public string? DiaChi { get; set; } // NULL
        public bool IsDeleted { get; set; } // NOT NULL
    }
}