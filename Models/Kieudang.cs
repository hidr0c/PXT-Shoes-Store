using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinaShoseShop.Models
{
	public class Kieudang
	{
		[Key]
		public int Makieudang { get; set; }

		public string Tenkieudang { get; set; }

		public virtual ICollection<Sanpham> Sanphams { get; set; }
	}
}