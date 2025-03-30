using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VinaShoseShop.Models
{
	public class Phanquyen
	{
		[Key]
		public int IDQuyen { get; set; }

		public string TenQuyen { get; set; }
	}
}