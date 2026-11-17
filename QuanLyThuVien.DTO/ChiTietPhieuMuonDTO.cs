using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyThuVien.DTO
{
    public class ChiTietPhieuMuonDTO
    {
        public string MaPhieu { get; set; }
        public string MaSach { get; set; }
        public int SoLuongMuon { get; set; }
        public DateTime? NgayTra { get; set; }
        public decimal TienPhat { get; set; }
        public string TinhTrangKhiTra { get; set; }
        public string TenSach { get; set; }
        public string TrangThai { get; set; }
    }
}
