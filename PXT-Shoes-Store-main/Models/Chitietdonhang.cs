using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinaShoseShop.Models
{
	public class Chitietdonhang
	{
		/*	[Key, Column(Order = 0)]
			public int Madon { get; set; }

			[Key, Column(Order = 1)]
			public int Masp { get; set; }

			public int? Soluong { get; set; }

			public decimal? Dongia { get; set; }

			public decimal? GiaGocSp { get; set; }

			public string size { get; set; }

			[ForeignKey("Masp")]
			public virtual Sanpham Sanpham { get; set; }*/


		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Column(Order = 0)]
		public int Madon { get; set; }

		[Column(Order = 1)]
		public int Masp { get; set; }

		public int? Soluong { get; set; }

		public decimal? Dongia { get; set; }

		public decimal? GiaGocSp { get; set; }

		public string size { get; set; }

		[ForeignKey("Masp")]
		public virtual Sanpham Sanpham { get; set; }

	}
}
