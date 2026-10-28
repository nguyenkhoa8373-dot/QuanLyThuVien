using System;

namespace QuanLyThuVien.DTO
{
    // Đã đổi thành public để mở khóa cho DAL nhìn thấy
    public class SachDTO
    {
        // Phải có các thuộc tính này thì DAL mới có chỗ để cất dữ liệu
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TheLoai { get; set; }
        public int SoLuongTon { get; set; }
        public bool IsDeleted { get; set; }
    }
}