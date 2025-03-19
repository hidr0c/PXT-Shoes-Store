using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ApplicationDbcontext db = new ApplicationDbcontext();

        // Lấy giỏ hàng từ Session
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

		// Thêm sản phẩm vào giỏ hàng
		public ActionResult ThemGioHang(int iMasp, string size, string strURL, int quantity)
		{
			int soluong = quantity;
			Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMasp);
			if (sp == null)
			{
				Response.StatusCode = 404;
				return null;
			}

			List<GioHang> lstGioHang = LayGioHang();
			GioHang sanpham = lstGioHang.Find(n => n.iMasp == iMasp && n.Size == size);

			if (sanpham == null)
			{
				sanpham = new GioHang(iMasp, size);
				if (sp.GiaSale > 0 && sp.Sale == true)
				{
					sanpham.dDonGia = Convert.ToDouble(sp.GiaSale);
				}
				else
				{
					sanpham.dDonGia = Convert.ToDouble(sp.Giatien);
				}
				sanpham.iSoLuong = soluong;
				sanpham.GiaGocSp = sp.GiaGoc;
				lstGioHang.Add(sanpham);
			}
			else
			{
				if (sp.GiaSale > 0 && sp.Sale == true)
				{
					sanpham.dDonGia = Convert.ToDouble(sp.GiaSale);
				}
				else
				{
					sanpham.dDonGia = Convert.ToDouble(sp.Giatien);
				}
				sanpham.GiaGocSp = sp.GiaGoc;
				sanpham.iSoLuong += soluong;
			}

			Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng

			return Redirect(strURL);
		}

		// Cập nhật giỏ hàng: cập nhật số lượng và kích cỡ sản phẩm
		[HttpPost]
        public ActionResult CapNhatGioHang(int iMaSP, string size, FormCollection f)
        {
            string sizemoi = f["txtSize"];

            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP && n.Size == size);

            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"]);

                // Lấy size mới từ form collection và kiểm tra nếu nằm trong khoảng 36-45
                string txtSize = f["txtSize"];
                if (KiemTraSizeHopLe(txtSize))
                {
                    sanpham.Size = txtSize; // Lưu size mới vào sản phẩm
                }
                else
                {
                    // Nếu size không hợp lệ, bạn có thể xử lý theo nhu cầu, ví dụ thông báo lỗi
                    ViewBag.ErrorMessage = "Kích cỡ sản phẩm không hợp lệ!";
                    // Hoặc giữ nguyên size cũ
                    // sanpham.Size = size;
                }
            }

            Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng

            return RedirectToAction("GioHang");
        }




        // Hàm kiểm tra kích cỡ sản phẩm có hợp lệ (trong khoảng 36-45)
        private bool KiemTraSizeHopLe(string size)
        {
            int parsedSize;
            return int.TryParse(size, out parsedSize) && parsedSize >= 36 && parsedSize <= 45;
        }

        // Xóa sản phẩm khỏi giỏ hàng
   /*     public ActionResult XoaGioHang(int iMaSP, string size)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP && n.Size == size);

            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP && n.Size == size);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng
			}

*//*            Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng
*//*
            return RedirectToAction("GioHang");
        }
*/

        public ActionResult XoaGioHang(int iMaSP)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP );

            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng
            }

            /*            Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng
            */
            return RedirectToAction("GioHang");
        }

        // Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }

        // Tính tổng số lượng sản phẩm trong giỏ hàng
        public int TongSoLuong()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            return lstGioHang?.Sum(n => n.iSoLuong) ?? 0;
        }

        // Tính tổng tiền của giỏ hàng
        private double TongTien()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            return lstGioHang?.Sum(n => n.ThanhTien) ?? 0;
        }

        // Tạo partial view cho giỏ hàng
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        // Xây dựng view cho người dùng chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }

		public ActionResult DatHangThanhCong()
		{
			return View();
		}

		// Chức năng đặt hàng
		[HttpPost]
		public ActionResult DatHang()
		{
			if (Session["use"] == null || string.IsNullOrEmpty(Session["use"].ToString()))
			{
				return RedirectToAction("Dangnhap", "User");
			}

			if (Session["GioHang"] == null)
			{
				return RedirectToAction("Index", "Home");
			}

			Nguoidung model = ((Nguoidung)Session["use"]);

			Donhang ddh = new Donhang
			{
				Manguoidung = model.Manguoidung,
				Ngaydat = DateTime.Now,
				Tinhtrang = 1,
				GiaTien = Convert.ToDecimal(TongTien()),
				DiaChi = model.Diachi,
				SoDienThoai = model.Dienthoai,
				TenNguoiDat = model.Hoten,
				Email = model.Email
			};

			db.Donhangs.Add(ddh);
			db.SaveChanges();

			List<GioHang> gh = LayGioHang();
			string productDetails = string.Empty;

			foreach (var item in gh)
			{
				var sanPham = db.Sanphams.SingleOrDefault(sp => sp.Masp == item.iMasp);

				if (sanPham != null)
				{
					Chitietdonhang ctDH = new Chitietdonhang
					{
						Madon = ddh.Madon,
						Masp = item.iMasp,
						Soluong = item.iSoLuong,
						Dongia = (decimal)item.dDonGia,
						GiaGocSp = item.GiaGocSp,
						size = item.Size,
						Sanpham = sanPham // Gán thông tin sản phẩm đã lấy từ cơ sở dữ liệu
					};

					db.Chitietdonhangs.Add(ctDH);
					db.SaveChanges();

					// Xây dựng nội dung sản phẩm
					productDetails += $@"
                <tr>
                    <td>{sanPham.Tensp}</td>
                    <td>{item.iSoLuong}</td>
                    <td>{item.dDonGia.ToString()}</td>
                </tr>";
				}
			}

			// Tạo luồng gửi email
			Thread t = new Thread(() =>
			{
				string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/templatesendmail/send2.html"));
				contentCustomer = contentCustomer.Replace("{{MaDon}}", ddh.Madon.ToString());
				contentCustomer = contentCustomer.Replace("{{SanPham}}", productDetails);
				contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
				contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", ddh.TenNguoiDat);
				contentCustomer = contentCustomer.Replace("{{Phone}}", ddh.SoDienThoai);
				contentCustomer = contentCustomer.Replace("{{Email}}", ddh.Email);
				contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", ddh.DiaChi);
				contentCustomer = contentCustomer.Replace("{{TongTien}}", ddh.GiaTien.ToString("N0"));
				GuiMail.SendMail("Shopbangiay", "Đơn hàng #" + ddh.Madon.ToString(), contentCustomer.ToString(), ddh.Email);// gmail nhận thông tin đơn hàng của gamil người dùng

				string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/templatesendmail/send1.html"));
				contentAdmin = contentAdmin.Replace("{{MaDon}}", ddh.Madon.ToString());
				contentAdmin = contentAdmin.Replace("{{SanPham}}", productDetails);
				contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
				contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", ddh.TenNguoiDat);
				contentAdmin = contentAdmin.Replace("{{Phone}}", ddh.SoDienThoai);
				contentAdmin = contentAdmin.Replace("{{Email}}", ddh.Email);
				contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", ddh.DiaChi);
				contentAdmin = contentAdmin.Replace("{{TongTien}}", ddh.GiaTien.ToString("N0"));
				GuiMail.SendMail("Shopbangiay", "Đơn hàng mới #" + ddh.Madon.ToString(), contentAdmin.ToString(), "seafoodtavern@gmail.com");// gmail Nhận thông tin đơn hàng admin
            });

			t.Start();

			// Thêm thông báo đặt hàng thành công
			TempData["SuccessMessage"] = "Đặt hàng thành công!";

			// Xóa giỏ hàng sau khi đặt hàng thành công
			Session["GioHang"] = null;

			return View("DatHangThanhCong");
		}


		[HttpPost]
		public ActionResult DatHangOnline()
		{
			if (Session["use"] == null || string.IsNullOrEmpty(Session["use"].ToString()))
			{
				return RedirectToAction("Dangnhap", "User");
			}

			if (Session["GioHang"] == null)
			{
				return RedirectToAction("Index", "Home");
			}

			Nguoidung model = ((Nguoidung)Session["use"]);

			Donhang ddh = new Donhang
			{
				Manguoidung = model.Manguoidung,
				Ngaydat = DateTime.Now,
				Tinhtrang = 1,
				GiaTien = Convert.ToDecimal(TongTien()),
				DiaChi = model.Diachi,
				SoDienThoai = model.Dienthoai,
				TenNguoiDat = model.Hoten,
				Email = model.Email
			};

			db.Donhangs.Add(ddh);
			db.SaveChanges();

			List<GioHang> gh = LayGioHang();
			string productDetails = string.Empty;

			foreach (var item in gh)
			{
				var sanPham = db.Sanphams.SingleOrDefault(sp => sp.Masp == item.iMasp);

				if (sanPham != null)
				{
					Chitietdonhang ctDH = new Chitietdonhang
					{
						Madon = ddh.Madon,
						Masp = item.iMasp,
						Soluong = item.iSoLuong,
						Dongia = (decimal)item.dDonGia,
						GiaGocSp = item.GiaGocSp,
						size = item.Size,
						Sanpham = sanPham // Gán thông tin sản phẩm đã lấy từ cơ sở dữ liệu
					};

					db.Chitietdonhangs.Add(ctDH);
					db.SaveChanges();

					// Xây dựng nội dung sản phẩm
					productDetails += $@"
                <tr>
                    <td>{sanPham.Tensp}</td>
                    <td>{item.iSoLuong}</td>
                    <td>{item.dDonGia.ToString()}</td>
                </tr>";
				}
			}

			// Tạo luồng gửi email
			Thread t = new Thread(() =>
			{
				string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/templatesendmail/send2.html"));
				contentCustomer = contentCustomer.Replace("{{MaDon}}", ddh.Madon.ToString());
				contentCustomer = contentCustomer.Replace("{{SanPham}}", productDetails);
				contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
				contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", ddh.TenNguoiDat);
				contentCustomer = contentCustomer.Replace("{{Phone}}", ddh.SoDienThoai);
				contentCustomer = contentCustomer.Replace("{{Email}}", ddh.Email);
				contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", ddh.DiaChi);
				contentCustomer = contentCustomer.Replace("{{TongTien}}", ddh.GiaTien.ToString("N0"));
				GuiMail.SendMail("Shopbangiay", "Đơn hàng #" + ddh.Madon.ToString(), contentCustomer.ToString(), ddh.Email);// sẽ gửi gmail tới ng dùng

				string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/templatesendmail/send1.html"));
				contentAdmin = contentAdmin.Replace("{{MaDon}}", ddh.Madon.ToString());
				contentAdmin = contentAdmin.Replace("{{SanPham}}", productDetails);
				contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
				contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", ddh.TenNguoiDat);
				contentAdmin = contentAdmin.Replace("{{Phone}}", ddh.SoDienThoai);
				contentAdmin = contentAdmin.Replace("{{Email}}", ddh.Email);
				contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", ddh.DiaChi);
				contentAdmin = contentAdmin.Replace("{{TongTien}}", ddh.GiaTien.ToString("N0"));
				GuiMail.SendMail("Shopbangiay", "Đơn hàng mới #" + ddh.Madon.ToString(), contentAdmin.ToString(), "seafoodtavern@gmail.com");// này là gmail do admin quản lý
			});

			t.Start();


			// Thêm thông báo đặt hàng thành công
			TempData["SuccessMessage"] = "Đặt hàng thành công!";

			// Xóa giỏ hàng sau khi đặt hàng thành công
			Session["GioHang"] = null;

			var url = VnpayPayment(ddh.Madon);

			return Redirect(url);  
		}

		// Xây dựng trang chủ
		public ActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }

		public string VnpayPayment(int madh)
		{

			int TypePaymentVN = 0;

			var urlPayment = "";
			var order = db.Donhangs.FirstOrDefault(x => x.Madon == madh);
			var chitietdh = db.Chitietdonhangs.Where(x => x.Madon == order.Madon);
			//Get Config Info
			string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
			string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
			string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
			string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key
																						//Build URL for VNPAY
			VnPayLibrary vnpay = new VnPayLibrary();
			var Price = (long)order.GiaTien * 100;

			vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
			vnpay.AddRequestData("vnp_Command", "pay");
			vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
			vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
			if (TypePaymentVN == 1)
			{
				vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
			}
			else if (TypePaymentVN == 2)
			{
				vnpay.AddRequestData("vnp_BankCode", "VNBANK");
			}
			else if (TypePaymentVN == 3)
			{
				vnpay.AddRequestData("vnp_BankCode", "INTCARD");
			}
			vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", "VND");
			vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
			vnpay.AddRequestData("vnp_Locale", "vn");
			vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Madon.ToString());
			vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
			vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
			vnpay.AddRequestData("vnp_TxnRef", order.Madon.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
															//Add Params of 2.1.0 Version
															//Billing
			urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
			//log.InfoFormat("VNPAY URL: {0}", paymentUrl);
			return urlPayment;
		}

		// Action để nhận kết quả từ VNPAY
		public ActionResult VnpayReturn()
		{
			if (Request.QueryString.Count > 0)
			{
				string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
				var vnpayData = Request.QueryString;
				VnPayLibrary vnpay = new VnPayLibrary();
				foreach (string s in vnpayData)
				{
					//get all querystring data
					if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
					{
						vnpay.AddResponseData(s, vnpayData[s]);
					}
				}
				string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
				long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
				string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
				string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
				String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
				String TerminalID = Request.QueryString["vnp_TmnCode"];
				long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
				String bankCode = Request.QueryString["vnp_BankCode"];
				bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
				if (checkSignature)
				{
					if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
					{
						var itemOrder = db.Donhangs.FirstOrDefault();
						if (itemOrder != null)
						{
							itemOrder.Tinhtrang = 2;
							db.Donhangs.Attach(itemOrder);
							db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
							db.SaveChanges();
						}
						//Thanh toan thanh cong
						ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
						//log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
					}
					else
					{
						//Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
						ViewBag.InnerText = "Giao dịch bị hủy.Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
						//log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
					}
					//displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
					//displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
					//displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
					ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
					//displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
				}
			}
			return View();
		}


	}
}
