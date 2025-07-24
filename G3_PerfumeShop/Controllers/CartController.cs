using Amazon.S3.Model;
using G3_PerfumeShop.ViewModels;
using CodeMegaVNPay.Services;
using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using X.PagedList;

namespace G3_PerfumeShop.Controllers
{
    public class CartController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        private readonly VnPayService _vnPayService;

        public CartController(G3_PerfumeShopDB_Iter3Context context,
            VnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;

        }

        // Action method to display the Cart page
        public IActionResult Index(int? page)//Hiển thị trang giỏ hàng
        {
            // Lấy roleId và userId từ session
            var roleId = HttpContext.Session.GetInt32("RoleId");//Lấy RoleId từ session để kiểm tra quyền truy cập.
            var userId = HttpContext.Session.GetInt32("UserId");//Lấy UserId từ session để xác định người dùng hiện tại.

            // Kiểm tra quyền truy cập
            if (userId == null)//Nếu userId là null, chuyển hướng đến trang đăng nhập.
            {
                return RedirectToAction("Index", "Login");
            }
            if (roleId != 4 || userId == null) // Kiểm tra quyền truy cập
            {
                return RedirectToAction("UnauthorizedAccess", "Home"); // Chuyển hướng tới UnauthorizedAccess view
            }

            // Tìm đơn hàng (giỏ hàng) của người dùng, bao gồm chi tiết đơn hàng và thông tin sản phẩm.
            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSizePricing)
                .ThenInclude(psp => psp.Product)
                .FirstOrDefault(o => o.UserId == userId && o.Status == true);

            // Nếu không có giỏ hàng, hiển thị trang "EmptyCart".
            if (cart == null)
            {
                return View("EmptyCart");
            }

            // Thiết lập phân trang cho 5 sản phẩm mỗi trang.
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pagedOrderDetails = cart.OrderDetails.ToPagedList(pageNumber, pageSize);

            return View(pagedOrderDetails);//Trả về view chứa danh sách sản phẩm trong giỏ hàng.
        }
        [HttpPost]
        public IActionResult UpdateCart(int orderDetailId, int quantity) //Phương thức POST để cập nhật số lượng sản phẩm trong giỏ hàng.
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập.
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm giỏ hàng đang hoạt động của người dùng.
            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.UserId == userId && o.Status == true);

            if (cart == null)
            {
                // If no cart exists, redirect back to the cart page
                return RedirectToAction("Index", "Cart");
            }

            // Tìm sản phẩm cần cập nhật trong giỏ hàng.
            var existingOrderDetail = cart.OrderDetails
                .FirstOrDefault(od => od.Id == orderDetailId);

            if (existingOrderDetail != null)
            {
                if (quantity > 0)//Nếu số lượng lớn hơn 0, cập nhật số lượng sản phẩm.
                {
                    // Update the quantity if it's greater than 0
                    existingOrderDetail.Quantity = quantity;
                    _context.OrderDetails.Update(existingOrderDetail);
                }
                else
                {
                    // Remove the product from the cart if quantity is 0
                    _context.OrderDetails.Remove(existingOrderDetail);
                }

                // Save changes
                _context.SaveChanges();
            }

            // Redirect to the cart page
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        //Xóa sản phẩm từ cart
        public IActionResult RemoveFromCart(int orderDetailId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Check if user is logged in
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Find the user's active cart (Order)
            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.UserId == userId && o.Status == true);

            if (cart == null)
            {
                // If no active cart exists, redirect back to the cart page
                return RedirectToAction("Index", "Cart");
            }

            // Find the specific order detail by orderDetailId
            var orderDetail = cart.OrderDetails
                .FirstOrDefault(od => od.Id == orderDetailId);

            if (orderDetail != null)
            {
                // Remove the item from the cart
                _context.OrderDetails.Remove(orderDetail);

                // Save changes to the database
                _context.SaveChanges();
            }

            // Redirect to the cart page after removing the item
            return RedirectToAction("Index", "Cart");
        }

        // Action method to add items to the cart
        [HttpPost]
        //Add thêm sản phẩm vào cart
        public IActionResult AddToCart(int productSizePricingId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Check if user is logged in
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Find the user's active cart (Order)
            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.UserId == userId && o.Status == true);

            if (cart == null)
            {
                // Create a new cart if it doesn't exist
                cart = new Order
                {
                    UserId = (int)userId,
                    CreatedAt = DateTime.Now,
                    Status = true,
                };

                _context.Orders.Add(cart);
                _context.SaveChanges(); // Save to get the OrderId
            }

            // Check if the product is already in the cart
            var existingOrderDetail = cart.OrderDetails
                .FirstOrDefault(od => od.ProductSizePricingId == productSizePricingId);

            if (existingOrderDetail != null)
            {
                // Update the quantity if the product already exists in the cart
                existingOrderDetail.Quantity += quantity;
                _context.OrderDetails.Update(existingOrderDetail);
            }
            else
            {
                var maxId = _context.OrderDetails.Max(od => od.Id);
                // Add a new product to the cart
                var orderDetail = new OrderDetail
                {
                    Id = maxId + 1,
                    ProductSizePricingId = productSizePricingId,
                    Quantity = quantity,
                    OrderId = cart.Id
                };



                _context.OrderDetails.Add(orderDetail);
            }

            // Save changes
            _context.SaveChanges();

            // Redirect to the cart page
            return RedirectToAction("Index", "Cart");
        }

        // Action method to handle reviewing the order, with the option to change the address
        [HttpPost]
        public IActionResult ReviewOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //Hiện thông tin 
            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSizePricing)
                .ThenInclude(psp => psp.Product)
                .FirstOrDefault(o => o.UserId == userId && o.Status == true);

            //Trường hợp cart không có sản phẩm nào
            if (cart == null || !cart.OrderDetails.Any())
            {
                return View("EmptyCart");
            }

            // Lấy thông tin người dùng từ bảng User
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.Id, u.FirstName, u.LastName, u.Phone, u.Address })  // Truy vấn từ bảng User
                .ToList();

            ViewBag.Users = user;  // Đẩy danh sách người dùng vào ViewBag.Users

            return View(cart);
        }

        //Hiện ra trang checkout
        [HttpPost]
        
        public async Task<IActionResult> CompleteCheckout(Checkoucs checkout)
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Tìm đơn hàng (giỏ hàng) của người dùng, bao gồm chi tiết đơn hàng và thông tin sản phẩm.
            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSizePricing)
                .ThenInclude(psp => psp.Product)
                .FirstOrDefault(o => o.UserId == userId && o.Status == true);
            cart.ShippingAddress = String.Join('|', checkout.Address);

            if (cart == null || !cart.OrderDetails.Any())
            {
                return View("EmptyCart");
            }

            // Lưu thông tin địa chỉ giao hàng vào trường ShippingAddress
            //     cart.ShippingAddress = $"{FirstName} {LastName}, {Phone}, {Address + "$" + Address2}";

            // Đánh dấu đơn hàng đã hoàn tất
            cart.Status = false;
            await SendCartCompletionEmailAsync(cart);
            _context.SaveChanges();

            // Điều hướng tới trang cảm ơn
            return RedirectToAction("ThankYou", new { orderId = cart.Id });
        }


        public async Task SendCartCompletionEmailAsync(Order? cart)
        {
            // Đọc nội dung từ file HTML
            string templatePath = "wwwroot/back/mailTemplate/cartCompletion.html";
            string emailBody = System.IO.File.ReadAllText(templatePath);
            string orderItemsHtml = "";
            foreach (var item in cart.OrderDetails)
            {
                var product = item.ProductSizePricing.Product;
                orderItemsHtml += $@"
        <tr>
            <td>{product.Name}</td>
            <td>{item.ProductSizePricing.Size}</td>
            <td>{item.ProductSizePricing.Price:C}</td>
            <td>{item.Quantity}</td>
            <td>{(item.ProductSizePricing.Price * item.Quantity):C}</td>
        </tr>";
            }

            // Sau đó thay thế vào template
            emailBody = emailBody
                .Replace("{UserName}", cart.User.Username)
                .Replace("{ShippingAddress}", cart.ShippingAddress.Replace("|", ", "))
                .Replace("{OrderItems}", orderItemsHtml)
                .Replace("{TotalAmount}", cart.OrderDetails.Sum(od => od.ProductSizePricing.Price * od.Quantity).ToString("C"));

            // Khởi tạo HttpClient và gửi yêu cầu
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("9db8722fc724b9831c22ff62:"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("from", "support@g3perfume.click"),
            new KeyValuePair<string, string>("to", cart.ShippingEmail),
            new KeyValuePair<string, string>("subject", "Yêu cầu thay đổi mật khẩu"),
            new KeyValuePair<string, string>("html", emailBody),
            new KeyValuePair<string, string>("sender", "Hỗ trợ từ G3 Perfume")
        });

                var response = await httpClient.PostAsync("https://api.forwardemail.net/v1/emails", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Email sent successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to send email. Status Code: " + response.StatusCode);
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error Details: " + errorDetails);
                }
            }
        }


        // Action to display Thank You page after checkout
        public IActionResult ThankYou(int orderId)//Tìm kiếm đơn hàng theo orderId và lấy đầy đủ chi tiết sản phẩm trong đơn hàng, bao gồm OrderDetails, ProductSizePricing, và Product.
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSizePricing)
                .ThenInclude(psp => psp.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)//Nếu đơn hàng không tồn tại, trả về lỗi NotFound.
            {
                return NotFound();
            }

            // Hiển thị địa chỉ giao hàng
            //           ViewBag.ShippingAddress = order.ShippingAddress;
            return View(order);//Trả về trang ThankYou và truyền thông tin đơn hàng (order) vào view.
        }
        public IActionResult CreatePaymentUrl(Checkoucs model) //Phương thức CreatePaymentUrl trả về một đối tượng IActionResult.
                                                               //Nó nhận tham số model là một đối tượng Checkoucs,
                                                               //chứa thông tin về đơn hàng và người mua
        {
            var userId = HttpContext.Session.GetInt32("UserId");    //Lấy userId từ session

            if (userId == null)//Nếu người dùng chưa đăng nhập, chuyển hướng về trang đăng nhập.
            {
                return RedirectToAction("Index", "Login");
            }
            var cart = _context.Orders//Truy vấn đơn hàng của người dùng từ cơ sở dữ liệu.
              .Include(o => o.OrderDetails)
              .ThenInclude(od => od.ProductSizePricing)
              .ThenInclude(psp => psp.Product)
              .FirstOrDefault(o => o.UserId == userId && o.Status == true);

            //Trường hợp cart rỗng
            if (cart == null || !cart.OrderDetails.Any())
            {
                return View("EmptyCart");
            }
            cart.ShippingAddress = String.Join('|', model.Address);//Địa chỉ giao hàng được gán từ model.Address.
            cart.LastName = model.LastName;//Họ và tên của người mua từ model.
            cart.FirstName = model.LastName;
            cart.ShippingEmail = String.Join('|', model.Email);//Email người mua, được ghép nối từ model.Email.
            cart.Phone = model.Phone;//Số điện thoại của người mua từ model.Phone.

            _context.SaveChanges();//Lưu các thay đổi vào cơ sở dữ liệu.

            PaymentInformationModel payment = new PaymentInformationModel();//Tạo đối tượng payment để chứa thông tin thanh toán.
            payment.OrderDescription = "Đơn hàng từ website xxx.xxx";//Mô tả đơn hàng, ở đây ghi là "Đơn hàng từ website xxx.xxx".
            payment.Name = "Đơn hàng #" + cart.Id;//Tên của đơn hàng, gồm chữ "Đơn hàng #" và mã đơn hàng (cart.Id).
            payment.OrderType = "Perfume";//Loại sản phẩm, ở đây là "Perfume".
            payment.Amount = (double)cart.OrderDetails.Sum(x => x.ProductSizePricing.Price * x.Quantity);//Tính tổng giá trị đơn hàng.
            var url = _vnPayService.CreatePaymentUrl(payment, HttpContext);//Gọi phương thức CreatePaymentUrl từ dịch vụ VNPay để tạo URL thanh toán.

            return Redirect(url);//Sau khi URL thanh toán được tạo, người dùng sẽ được chuyển hướng
                                 //tới URL thanh toán này để tiến hành thanh toán qua VNPay.
        }

        //Hiện ra thông tin thanh toán
        public IActionResult PaymentCallback()//hành động phản hồi khi người dùng hoàn thành thanh toán và VNPay
        {
            var response = _vnPayService.PaymentExecute(Request.Query);//Sử dụng VNPay để xử lý kết quả thanh toán thông qua phương thức PaymentExecute,
                                                                       //lấy dữ liệu từ Request.Query (chứa các tham số phản hồi từ VNPay).
            if (response.Success)//Kiểm tra nếu thanh toán thành công.
            {
                var userId = HttpContext.Session.GetInt32("UserId");//Lấy userId từ session

                if (userId == null)//Nếu userId không tồn tại
                {
                    return RedirectToAction("Index", "Login");
                }

                //Tìm đơn hàng đang hoạt động của người dùng hiện tại.
                var cart = _context.Orders  //Truy vấn bảng đơn hàng (Orders).
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductSizePricing)
                    .ThenInclude(psp => psp.Product)
                    .FirstOrDefault(o => o.UserId == userId && o.Status == true);

                //Nếu giỏ hàng trống hoặc không có sản phẩm, hiển thị trang EmptyCart.
                if (cart == null || !cart.OrderDetails.Any())
                {
                    return View("EmptyCart");
                }

                cart.Status = false;//Nếu giỏ hàng có sản phẩm, cập nhật trạng thái của giỏ hàng (Status = false)
                                    //để đánh dấu rằng đơn hàng đã được thanh toán xong.
                _context.SaveChanges();//lưu các thay đổi vào cơ sở dữ liệu bằng phương thức _context.SaveChanges()

                // Điều hướng tới trang cảm ơn
                return RedirectToAction("ThankYou", new { orderId = cart.Id });//chuyển hướng người dùng tới trang "ThankYou" và
                                                                               //truyền orderId của đơn hàng
                                                                               //vừa được thanh toán để hiển thị thông tin chi tiết.
            }
            else
            {
                return RedirectToAction("cart");//Nếu thanh toán không thành công (response.Success là false),
                                                //người dùng được chuyển hướng trở lại trang giỏ hàng (cart).

            }
        }
    }
    //Tk test : 9704198526191432198
    //NGUYEN VAN A
    //07/15
}
