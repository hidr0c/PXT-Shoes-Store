using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinaShoseShop.Models
{
	public class Donhang
	{
		[Key]
		public int Madon { get; set; }

		public DateTime Ngaydat { get; set; }
		public int? Tinhtrang { get; set; }
		public int? Manguoidung { get; set; }
		public string TenSanPham { get; set; }
		public decimal GiaTien { get; set; }
		public int SoLuong { get; set; }
		public string Size { get; set; }
		public string TenNguoiDat { get; set; }
		public string Email { get; set; }
		public string DiaChi { get; set; }
		public string SoDienThoai { get; set; }

		[ForeignKey("Manguoidung")]
		public virtual Nguoidung Nguoidung { get; set; }
	}
}