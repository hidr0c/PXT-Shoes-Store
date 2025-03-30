using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinaShoseShop.Models
{
	public class Hangsanxuat
	{
		[Key]
		public int Mahang { get; set; }

		public string Tenhang { get; set; }

		public virtual ICollection<Sanpham> Sanphams { get; set; }
	}
}