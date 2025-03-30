using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using VinaShoseShop.Models;  // Thêm dòng này

namespace VinaShoseShop.Controllers
{
    public class UserController : Controller
    {
		ApplicationDbcontext db = new ApplicationDbcontext();

        public ActionResult dangky()
        {
            Nguoidung nguoidung = new Nguoidung();  
            return View(nguoidung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult dangky(Nguoidung nguoidung)
        {
            // Kiểm tra họ tên: không được để trống, không được chứa số, không chứa ký tự đặc biệt, và có độ dài tối thiểu 3 ký tự
            if (string.IsNullOrEmpty(nguoidung.Hoten))
            {
                ModelState.AddModelError("Hoten", "Họ tên không được để trống.");
            }
            else if (nguoidung.Hoten.Any(char.IsDigit))
            {
                ModelState.AddModelError("Hoten", "Họ tên không được chứa số.");
            }
            else if (nguoidung.Hoten.Length < 3)
            {
                ModelState.AddModelError("Hoten", "Họ tên phải có độ dài tối thiểu 3 ký tự.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(nguoidung.Hoten, @"^[a-zA-Z\s]+$"))
            {
                ModelState.AddModelError("Hoten", "Họ tên chỉ được chứa chữ cái và khoảng trắng, không bao gồm ký tự đặc biệt.");
            }

            // Kiểm tra email: không được để trống và phải đúng định dạng Gmail
            if (string.IsNullOrEmpty(nguoidung.Email))
            {
                ModelState.AddModelError("Email", "Email không được để trống.");
            }
            else if (!nguoidung.Email.EndsWith("@gmail.com"))
            {
                ModelState.AddModelError("Email", "Email phải đúng định dạng Gmail (@gmail.com).");
            }

            // Kiểm tra số điện thoại: không được để trống, phải có đúng 10 chữ số
            if (string.IsNullOrEmpty(nguoidung.Dienthoai))
            {
                ModelState.AddModelError("Dienthoai", "Số điện thoại không được để trống.");
            }
            else if (nguoidung.Dienthoai.Length != 10 || !nguoidung.Dienthoai.All(char.IsDigit))
            {
                ModelState.AddModelError("Dienthoai", "Số điện thoại phải có đúng 10 chữ số và không có ký tự và chữ cái.");
            }
            else if (!nguoidung.Dienthoai.StartsWith("0"))
            {
                ModelState.AddModelError("Dienthoai", "Số điện thoại phải bắt đầu bằng số 0.");
            }


            // Kiểm tra mật khẩu: không được để trống và phải có ít nhất 5 ký tự
            if (string.IsNullOrEmpty(nguoidung.Matkhau))
            {
                ModelState.AddModelError("Matkhau", "Mật khẩu không được để trống.");
            }
            else if (nguoidung.Matkhau.Length < 5)
            {
                ModelState.AddModelError("Matkhau", "Mật khẩu phải có ít nhất 5 ký tự.");
            }
            if (string.IsNullOrEmpty(nguoidung.Diachi))
            {
                ModelState.AddModelError("Diachi", "Địa chỉ không được bỏ trống");
            }

            // Nếu tất cả các điều kiện đều thỏa mãn, lưu người dùng
            if (ModelState.IsValid)
            {
                try
                {
                    nguoidung.Matkhau = GetMD5(nguoidung.Matkhau);  // Mã hóa mật khẩu trước khi lưu
                    db.Nguoidungs.Add(nguoidung);
                    db.SaveChanges();
                    return RedirectToAction("Dangnhap");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Đã có lỗi xảy ra: " + ex.Message;
                }
            }

            // Nếu có lỗi, trả về view cùng với các thông báo lỗi
            return View("Dangky");
        }

        public ActionResult Dangnhap()
        {
			LoginModel loginModel = new LoginModel();   
            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dangnhap(FormCollection userlog)
        {
            LoginModel model = new LoginModel();
            if (ModelState.IsValid)
            {
                string userMail = userlog["userMail"];
                string password = userlog["password"];
                // Kiểm tra trường email và password không được để trống
                if (string.IsNullOrEmpty(userMail))
                {
                    ModelState.AddModelError("userMail", "Email không được để trống.");
                }
                if (string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("password", "Mật khẩu không được để trống.");
                }

                // Nếu các điều kiện không thỏa mãn
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //Mã hóa mật khẩu
                var f_password = GetMD5(password);
                //Kiểm tra thông tin đăng nhập/ nếu trùng tài khoản gmail sẽ báo lỗi ở đây
                var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(userMail) && x.Matkhau.Equals(f_password));
                if (islogin != null)
                {
                    Session["use"] = islogin;
                    //Kiểm tra nếu là admin
                    if (userMail == "admin@gmail.com" || userMail == "admin2@gmail.com")
                    {
                        return RedirectToAction("Index", "Admin/Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Chophep = false;
                    ViewBag.Fail = "Đăng nhập thất bại: Tài khoản và mật khẩu của bạn không chính xác xin vui lòng thử lại!";
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Thongtin()
        {
            var user = Session["use"] as Nguoidung;
            if (user == null)
            {
                return RedirectToAction("Dangnhap", "User");
            }
            return View(user);
        }

        public ActionResult Edit()
        {
            var user = Session["use"] as Nguoidung;
            if (user == null)
            {
                return RedirectToAction("Dangnhap", "User");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Nguoidung user)
        {
            // Kiểm tra họ tên: không được để trống, không chứa số, không chứa ký tự đặc biệt, và có độ dài tối thiểu 3 ký tự
            if (string.IsNullOrEmpty(user.Hoten))
            {
                ModelState.AddModelError("Hoten", "Họ tên không được để trống.");
            }
            else if (user.Hoten.Any(char.IsDigit))
            {
                ModelState.AddModelError("Hoten", "Họ tên không được chứa số.");
            }
            else if (user.Hoten.Length < 3)
            {
                ModelState.AddModelError("Hoten", "Họ tên phải có độ dài tối thiểu 3 ký tự.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(user.Hoten, @"^[a-zA-Z\s]+$"))
            {
                ModelState.AddModelError("Hoten", "Họ tên chỉ được chứa chữ cái và khoảng trắng, không bao gồm ký tự đặc biệt.");
            }

            // Kiểm tra email: không được để trống và phải đúng định dạng Gmail
            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "Email không được để trống.");
            }
            else if (!user.Email.EndsWith("@gmail.com"))
            {
                ModelState.AddModelError("Email", "Email phải đúng định dạng Gmail (@gmail.com).");
            }

            // Kiểm tra số điện thoại: không được để trống, phải có đúng 10 chữ số, không chứa ký tự lạ, và bắt đầu bằng số 0
            if (string.IsNullOrEmpty(user.Dienthoai))
            {
                ModelState.AddModelError("Dienthoai", "Số điện thoại không được để trống.");
            }
            else if (user.Dienthoai.Length != 10 || !user.Dienthoai.All(char.IsDigit))
            {
                ModelState.AddModelError("Dienthoai", "Số điện thoại phải có đúng 10 chữ số và không có kí tự,dấu,chữ cái .");
            }
            else if (!user.Dienthoai.StartsWith("0"))
            {
                ModelState.AddModelError("Dienthoai", "Số điện thoại phải bắt đầu bằng số 0.");
            }

            // Kiểm tra mật khẩu (nếu có chỉnh sửa mật khẩu): không được để trống và phải có ít nhất 5 ký tự
            if (!string.IsNullOrEmpty(user.Matkhau) && user.Matkhau.Length < 5)
            {
                ModelState.AddModelError("Matkhau", "Mật khẩu phải có ít nhất 5 ký tự.");
            }

            // Kiểm tra địa chỉ: không được để trống
            if (string.IsNullOrEmpty(user.Diachi))
            {
                ModelState.AddModelError("Diachi", "Địa chỉ không được để trống.");
            }

            // Nếu tất cả các điều kiện đều thỏa mãn, cập nhật thông tin người dùng
            if (ModelState.IsValid)
            {
                var existingUser = db.Nguoidungs.FirstOrDefault(u => u.Manguoidung == user.Manguoidung);
                if (existingUser != null)
                {
                    existingUser.Hoten = user.Hoten;
                    existingUser.Email = user.Email;
                    existingUser.Dienthoai = user.Dienthoai;
                    existingUser.Diachi = user.Diachi;

                    db.SaveChanges();
                    Session["use"] = existingUser;
                    return RedirectToAction("Thongtin");
                }
            }

            // Nếu có lỗi, trả về view cùng với các thông báo lỗi
            return View(user);
        }

        public ActionResult Donhang()
        {
            var user = Session["use"] as Nguoidung;
            if (user == null)
            {
                return RedirectToAction("Dangnhap", "User");
            }
			var orders = db.Donhangs.Where(d => d.Manguoidung == user.Manguoidung).ToList();

			var userList = new List<Nguoidung>();
			foreach (var u in orders)
			{
				var nguoiDung = db.Nguoidungs.FirstOrDefault(
					p => p.Manguoidung == u.Manguoidung);
				userList.Add(nguoiDung);
			}

			ViewBag.UserList = userList;

            return View(orders);
        }

        [HttpPost]
        public ActionResult NhanHang(int id)
        {
            var order = db.Donhangs.FirstOrDefault(d => d.Madon == id);
            if (order != null && order.Tinhtrang == 2)
            {
                order.Tinhtrang = 3;
                db.SaveChanges();
            }
            return RedirectToAction("Donhang");
        }

        [HttpPost]
        public ActionResult Huydon(int id)
        {
            var order = db.Donhangs.FirstOrDefault(d => d.Madon == id);
            if (order != null && order.Tinhtrang == 1)
            {
                order.Tinhtrang = 4;
                db.SaveChanges();
            }
            return RedirectToAction("Donhang");
        }

        private string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

		public ActionResult Detaildonhang(int? Madon)
		{
			var ctDonhangsQuery = db.Chitietdonhangs.AsQueryable();

			// Kiểm tra nếu Madon được truyền từ DonhangsController
			if (Madon != null)
			{
				ctDonhangsQuery = ctDonhangsQuery.Where(x => x.Madon == Madon);
			}

			// Order and include related entities
			var ctDonhangs = ctDonhangsQuery
							 .OrderBy(x => x.Madon)
							 .Include(x => x.Sanpham)
							 .ToList();

			// Extract the products from the loaded Chitietdonhangs
			var products = ctDonhangs.Select(ct => ct.Sanpham).Where(sp => sp != null).ToList();

			ViewBag.SanPham = products;
			return View(ctDonhangs);
		}




	}
}
