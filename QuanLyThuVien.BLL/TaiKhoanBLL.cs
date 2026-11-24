using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
    public class TaiKhoanBLL
    {
        public const string QuyenAdmin = "Admin";

        private readonly TaiKhoanDAL taiKhoanDAL;

        public TaiKhoanBLL() : this(new TaiKhoanDAL())
        {
        }

        public TaiKhoanBLL(TaiKhoanDAL taiKhoanDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
        }

        public bool DangNhap(string tenDangNhap, string matKhau, out TaiKhoanDTO? taiKhoan, out string thongBao)
        {
            taiKhoan = null;

            tenDangNhap = (tenDangNhap ?? string.Empty).Trim();

            if (tenDangNhap.Length == 0)
            {
                thongBao = "Tên đăng nhập không được để trống.";
                return false;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                thongBao = "Mật khẩu không được để trống.";
                return false;
            }

            try
            {
                taiKhoan = taiKhoanDAL.DangNhap(tenDangNhap, matKhau);
            }
            catch (Exception ex)
            {
                thongBao = "Lỗi truy cập cơ sở dữ liệu: " + ex.Message;
                return false;
            }

            if (taiKhoan == null)
            {
                thongBao = "Sai tên đăng nhập hoặc mật khẩu.";
                return false;
            }

            thongBao = string.Empty;
            return true;
        }

        public bool LaAdmin(TaiKhoanDTO? taiKhoan)
        {
            return taiKhoan != null &&
                   string.Equals((taiKhoan.Quyen ?? string.Empty).Trim(), QuyenAdmin, StringComparison.OrdinalIgnoreCase);
        }
    }
}