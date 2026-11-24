using System.Data;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    public class ThongKeBLL
    {
        private const string LoiCSDL = "Lỗi truy cập cơ sở dữ liệu: ";
        private const int TopToiDa = 100;

        private readonly ThongKeDAL thongKeDAL;

        public ThongKeBLL() : this(new ThongKeDAL())
        {
        }

        public ThongKeBLL(ThongKeDAL thongKeDAL)
        {
            this.thongKeDAL = thongKeDAL;
        }

        public bool LayTopSachMuonNhieuNhat(int soLuongTop, out List<SachMuonNhieuDTO> ketQua, out string thongBao)
        {
            ketQua = new List<SachMuonNhieuDTO>();

            if (soLuongTop <= 0 || soLuongTop > TopToiDa)
            {
                thongBao = $"Số lượng top phải từ 1 đến {TopToiDa}.";
                return false;
            }

            try
            {
                DataTable dt = thongKeDAL.LayTopSachMuonNhieuNhat(soLuongTop);

                foreach (DataRow row in dt.Rows)
                {
                    ketQua.Add(new SachMuonNhieuDTO
                    {
                        MaSach = row["MaSach"].ToString() ?? string.Empty,
                        TenSach = row["TenSach"].ToString() ?? string.Empty,
                        TheLoai = row["TheLoai"].ToString() ?? string.Empty,
                        TongLuotMuon = Convert.ToInt32(row["TongLuotMuon"])
                    });
                }

                thongBao = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool LayDanhSachDocGiaQuaHan(out List<DocGiaQuaHanDTO> ketQua, out string thongBao)
        {
            ketQua = new List<DocGiaQuaHanDTO>();

            try
            {
                DataTable dt = thongKeDAL.LayDanhSachDocGiaQuaHan();

                foreach (DataRow row in dt.Rows)
                {
                    ketQua.Add(new DocGiaQuaHanDTO
                    {
                        MaDocGia = row["MaDocGia"].ToString() ?? string.Empty,
                        TenDocGia = row["TenDocGia"].ToString() ?? string.Empty,
                        MaPhieu = row["MaPhieu"].ToString() ?? string.Empty,
                        HanTra = Convert.ToDateTime(row["HanTra"]),
                        SoNgayTre = Convert.ToInt32(row["SoNgayTre"]),
                        TienPhatDuKien = Convert.ToDecimal(row["TienPhatDuKien"])
                    });
                }

                thongBao = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }

        public bool TinhTongTienPhatThangNay(out decimal tongTien, out string thongBao)
        {
            tongTien = 0;

            try
            {
                tongTien = thongKeDAL.TinhTongTienPhatThangNay();
                thongBao = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                thongBao = LoiCSDL + ex.Message;
                return false;
            }
        }
    }
}
