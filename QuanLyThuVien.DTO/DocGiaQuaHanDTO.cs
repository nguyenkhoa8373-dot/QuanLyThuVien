using System;
namespace QuanLyThuVien.DTO
{
    public class DocGiaQuaHanDTO
    {
        public string MaDocGia { get; set; } = string.Empty;
        public string TenDocGia { get; set; } = string.Empty;
        public string MaPhieu { get; set; } = string.Empty;
        public DateTime HanTra { get; set; }
        public int SoNgayTre { get; set; }
        public decimal TienPhatDuKien { get; set; }
    }
}