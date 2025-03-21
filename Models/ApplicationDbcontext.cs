using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace VinaShoseShop.Models
{
	public class ApplicationDbcontext : DbContext
	{
		public ApplicationDbcontext() : base("DefaultConnection")
		{

		}

		public DbSet<Chitietdonhang> Chitietdonhangs { get; set; }
		public DbSet<Donhang> Donhangs { get; set; }
		public DbSet<Hangsanxuat> Hangsanxuats { get; set; }
		public DbSet<Kieudang> Kieudangs { get; set; }
		public DbSet<Nguoidung> Nguoidungs { get; set; }
		public DbSet<Phanquyen> Phanquyens { get; set; }
		public DbSet<Sanpham> Sanphams { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}





	}
}