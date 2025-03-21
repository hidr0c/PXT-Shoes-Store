using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinaShoseShop.Models
{
	public class Sanpham
	{
		[Key]
		public int Masp { get; set; }

		public string Tensp { get; set; }
		[Required(ErrorMessage = "Không được để trống")]
		[Range(1000, double.MaxValue, ErrorMessage = "Giá trị bé nhất phải là 1000")]
		public decimal? Giatien { get; set; }
		[Required(ErrorMessage = "Không được để trống")]
		[Range(1000, double.MaxValue, ErrorMessage = "Giá trị bé nhất phải là 1000")]
		public decimal? GiaGoc { get; set; }
		[Required(ErrorMessage = "Không được để trống")]
		[Range(1000, double.MaxValue, ErrorMessage = "Giá trị bé nhất phải là 1000")]
		public decimal? GiaSale { get; set; }
		public bool Sale { get; set; }
		public int? Soluong { get; set; }
		public string Mota { get; set; }
		public string Mausac { get; set; }
		public int? Kichco { get; set; }
		public bool? Sanphammoi { get; set; }
		public string Anhbia { get; set; }

		public int? Mahang { get; set; }
		public int? Makieudang { get; set; }

		[ForeignKey("Mahang")]
		public virtual Hangsanxuat Hangsanxuat { get; set; }

		[ForeignKey("Makieudang")]
		public virtual Kieudang Kieudang { get; set; }
	}
}