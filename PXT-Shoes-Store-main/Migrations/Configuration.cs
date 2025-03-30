namespace VinaShoseShop.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<VinaShoseShop.Models.ApplicationDbcontext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VinaShoseShop.Models.ApplicationDbcontext context)
        {

            // Phương thức này sẽ được gọi sau khi chuyển sang phiên bản mới nhất.

            // để tránh tạo dữ liệu trùng lặp
        }
    }
}
