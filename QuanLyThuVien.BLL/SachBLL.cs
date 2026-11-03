using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;

namespace QuanLyThuVien.BLL
{
	public class SachBLL
	{
		private readonly SachDAL sachDAL = new SachDAL();

		public List<SachDTO> LayDanhSachSach()
		{
			return sachDAL.LayDanhSachSach();
		}

		public List<SachDTO> TimSach(string tuKhoa)
		{
			if (string.IsNullOrWhiteSpace(tuKhoa))
			{
				return LayDanhSachSach();
			}

			tuKhoa = tuKhoa.Trim();

			return LayDanhSachSach()
				.Where(sach =>
					(sach.MaSach?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
					(sach.TenSach?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false) ||
					(sach.TheLoai?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ?? false))
				.ToList();
		}

		public bool ThemSach(SachDTO sach, out string thongBao)
		{
			if (!KiemTraSach(sach, out thongBao))
			{
				return false;
			}

			bool daTonTai = LayDanhSachSach()
			.Any(s => string.Equals(s.MaSach, sach.MaSach, StringComparison.OrdinalIgnoreCase));

			if (daTonTai)
			{
				thongBao = "Mã sách đã tồn tại.";
				return false;
			}

			bool ketQua = sachDAL.ThemSach(sach);
			thongBao = ketQua ? "Thêm sách thành công." : "Không thể thêm sách.";
			return ketQua;
		}

		public bool SuaSach(SachDTO sach, out string thongBao)
		{
			if (!KiemTraSach(sach, out thongBao))
			{
				return false;
			}

			bool daTonTai = LayDanhSachSach()
			.Any(s => string.Equals(s.MaSach, sach.MaSach, StringComparison.OrdinalIgnoreCase));

			if (!daTonTai)
			{
				thongBao = "Không tìm thấy mã sách cần sửa.";
				return false;
			}

			bool ketQua = sachDAL.SuaSach(sach);
			thongBao = ketQua ? "Cập nhật sách thành công." : "Không thể cập nhật sách.";
			return ketQua;
		}

		public bool XoaSach(string maSach, out string thongBao)
		{
			if (string.IsNullOrWhiteSpace(maSach))
			{
				thongBao = "Mã sách không được để trống.";
				return false;
			}

			bool daTonTai = LayDanhSachSach()
			.Any(s => string.Equals(s.MaSach, maSach.Trim(), StringComparison.OrdinalIgnoreCase));

			if (!daTonTai)
			{
				thongBao = "Không tìm thấy mã sách cần xóa.";
				return false;
			}

			bool ketQua = sachDAL.XoaSach(maSach.Trim());
			thongBao = ketQua ? "Xóa sách thành công." : "Không thể xóa sách.";
			return ketQua;
		}

		private bool KiemTraSach(SachDTO sach, out string thongBao)
		{
			if (sach == null)
			{
				thongBao = "Thông tin sách không được để trống.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(sach.MaSach))
			{
				thongBao = "Mã sách không được để trống.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(sach.TenSach))
			{
				thongBao = "Tên sách không được để trống.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(sach.TheLoai))
			{
				thongBao = "Thể loại không được để trống.";
				return false;
			}

			if (sach.SoLuongTon < 0)
			{
				thongBao = "Số lượng tồn không được nhỏ hơn 0.";
				return false;
			}

			thongBao = string.Empty;
			return true;
		}
	}
}