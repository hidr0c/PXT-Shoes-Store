using Microsoft.Ajax.Utilities;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using TuongShoeShop.Models;
using VinaShoseShop.Models;

namespace VinaShoseShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ApplicationDbcontext db = new ApplicationDbcontext();

        // Mock coupon data (in a real application, this would come from a database)
        private readonly Dictionary<string, decimal> coupons = new Dictionary<string, decimal>
        {
            { "SALE10", 0.10m }, // 10% discount
            { "SALE20", 0.20m }, // 20% discount
            { "SALE50", 0.50m }  // 50% discount
        };

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
                string txtSize = f["txtSize"];
                if (KiemTraSizeHopLe(txtSize))
                {
                    sanpham.Size = txtSize; // Lưu size mới vào sản phẩm
                }
                else
                {
                    ViewBag.ErrorMessage = "Kích cỡ sản phẩm không hợp lệ!";
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
        public ActionResult XoaGioHang(int iMaSP)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);

            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                Session["GioHang"] = lstGioHang; // Cập nhật session sau khi thay đổi giỏ hàng
            }

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
            ViewBag.AppliedCoupon = Session["AppliedCoupon"]?.ToString();
            return View(lstGioHang);
        }

        // Tính tổng số lượng sản phẩm trong giỏ hàng
        public int TongSoLuong()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            return lstGioHang?.Sum(n => n.iSoLuong) ?? 0;
        }

        // Tính tổng tiền của giỏ hàng (bao gồm giảm giá nếu có)
        private double TongTien()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            double subtotal = lstGioHang?.Sum(n => n.ThanhTien) ?? 0;

            // Apply discount if a valid coupon is applied
            string appliedCoupon = Session["AppliedCoupon"]?.ToString();
            decimal discountRate = 0m;
            if (!string.IsNullOrEmpty(appliedCoupon) && coupons.ContainsKey(appliedCoupon))
            {
                discountRate = coupons[appliedCoupon];
            }

            double discountAmount = subtotal * (double)discountRate;
            double discountedSubtotal = subtotal - discountAmount;

            // Add 10% VAT on the discounted subtotal
            double thueVAT = discountedSubtotal * 0.10;
            double finalTotal = discountedSubtotal + thueVAT;

            return finalTotal;
        }

        // Tính tổng tiền trước khi áp dụng giảm giá (dùng để hiển thị tổng tiền hàng)
        private double TongTienHang()
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

        // Action to apply a coupon
        [HttpPost]
        public ActionResult ApplyCoupon(string couponCode)
        {
            List<GioHang> lstGioHang = LayGioHang();

            // Validate the coupon
            if (!string.IsNullOrEmpty(couponCode) && coupons.ContainsKey(couponCode.ToUpper()))
            {
                // Coupon is valid, store it in Session
                Session["AppliedCoupon"] = couponCode.ToUpper();
                TempData["CouponWarning"] = null; // Clear any previous warning
            }
            else
            {
                // Coupon is invalid, set a warning message
                TempData["CouponWarning"] = "Mã giảm giá không hợp lệ.";
                Session["AppliedCoupon"] = null; // Clear any previous coupon
            }

            return RedirectToAction("GioHang");
        }

        public ActionResult DatHangThanhCong()
        {
            return View();
        }

        public ActionResult DatHangThatBai()
        {
            return View();
        }

        // Chức năng đặt hàng (COD)
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
                GiaTien = Convert.ToDecimal(TongTien()), // Use the discounted total
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
                        Sanpham = sanPham
                    };

                    db.Chitietdonhangs.Add(ctDH);
                    db.SaveChanges();

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
                GuiMail.SendMail("Shopbangiay", "Đơn hàng #" + ddh.Madon.ToString(), contentCustomer.ToString(), ddh.Email);

                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/templatesendmail/send1.html"));
                contentAdmin = contentAdmin.Replace("{{MaDon}}", ddh.Madon.ToString());
                contentAdmin = contentAdmin.Replace("{{SanPham}}", productDetails);
                contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", ddh.TenNguoiDat);
                contentAdmin = contentAdmin.Replace("{{Phone}}", ddh.SoDienThoai);
                contentAdmin = contentAdmin.Replace("{{Email}}", ddh.Email);
                contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", ddh.DiaChi);
                contentAdmin = contentAdmin.Replace("{{TongTien}}", ddh.GiaTien.ToString("N0"));
                GuiMail.SendMail("Shopbangiay", "Đơn hàng mới #" + ddh.Madon.ToString(), contentAdmin.ToString(), "seafoodtavern@gmail.com");
            });

            t.Start();

            // Clear the cart and applied coupon after successful order
            Session["GioHang"] = null;
            Session["AppliedCoupon"] = null;

            return View("DatHangThanhCong");
        }

        [HttpPost]
        public ActionResult DatHangVNPay()
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
                GiaTien = Convert.ToDecimal(TongTien()), // Use the discounted total
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
                        Sanpham = sanPham
                    };

                    db.Chitietdonhangs.Add(ctDH);
                    db.SaveChanges();

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
                GuiMail.SendMail("Shopbangiay", "Đơn hàng #" + ddh.Madon.ToString(), contentCustomer.ToString(), ddh.Email);

                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/templatesendmail/send1.html"));
                contentAdmin = contentAdmin.Replace("{{MaDon}}", ddh.Madon.ToString());
                contentAdmin = contentAdmin.Replace("{{SanPham}}", productDetails);
                contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", ddh.TenNguoiDat);
                contentAdmin = contentAdmin.Replace("{{Phone}}", ddh.SoDienThoai);
                contentAdmin = contentAdmin.Replace("{{Email}}", ddh.Email);
                contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", ddh.DiaChi);
                contentAdmin = contentAdmin.Replace("{{TongTien}}", ddh.GiaTien.ToString("N0"));
                GuiMail.SendMail("Shopbangiay", "Đơn hàng mới #" + ddh.Madon.ToString(), contentAdmin.ToString(), "seafoodtavern@gmail.com");
            });

            t.Start();

            var url = VnpayPayment(ddh.Madon);
            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            return Redirect(url);
        }

        public string VnpayPayment(int madh)
        {
            int TypePaymentVN = 0;
            var urlPayment = "";
            var order = db.Donhangs.FirstOrDefault(x => x.Madon == madh);
            var chitietdh = db.Chitietdonhangs.Where(x => x.Madon == order.Madon);

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.GiaTien * 100;

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString());
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
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Madon.ToString());

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return urlPayment;
        }

        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (string s in vnpayData)
                {
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
                        var itemOrder = db.Donhangs.FirstOrDefault(x => x.Madon.ToString() == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.Tinhtrang = 2;
                            db.Donhangs.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ViewBag.InnerText = "Giỏ hàng không có sản phẩm, không thể thực hiện thanh toán";
                            return RedirectToAction("DatHangThatBai");
                        }
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                        Session["GioHang"] = null;
                        Session["AppliedCoupon"] = null; // Clear the applied coupon
                        return RedirectToAction("DatHangThanhCong");
                    }
                    else
                    {
                        var itemOrder = db.Donhangs.FirstOrDefault(x => x.Madon.ToString() == orderCode);
                        itemOrder.Tinhtrang = 4;
                        db.Donhangs.Attach(itemOrder);
                        db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.InnerText = "Giao dịch bị hủy. Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                        return RedirectToAction("DatHangThatBai");
                    }
                }
            }
            return View();
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/GioHang/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                    // Clear the cart and applied coupon after successful payment
                    Session["GioHang"] = null;
                    Session["AppliedCoupon"] = null;
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            // Add items from the cart to the PayPal item list
            List<GioHang> lstGioHang = LayGioHang();
            foreach (var item in lstGioHang)
            {
                itemList.items.Add(new Item()
                {
                    name = item.sTensp,
                    currency = "USD",
                    price = (item.dDonGia / 23000).ToString("F2"), // Convert VND to USD (approximate rate)
                    quantity = item.iSoLuong.ToString(),
                    sku = item.iMasp.ToString()
                });
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = (TongTien() / 23000).ToString("F2") // Convert VND to USD
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = (TongTien() / 23000).ToString("F2"), // Use the discounted total
                details = details
            };

            var transactionList = new List<Transaction>();
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }

        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }

        public ActionResult ThanhToanThatBai()
        {
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }
    }
}