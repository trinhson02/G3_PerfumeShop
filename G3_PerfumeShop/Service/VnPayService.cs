using G3_PerfumeShop.ViewModels;

namespace CodeMegaVNPay.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService( IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //Tạo thanh toán cho ứng dụng vnpay
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);//Dòng này lấy thông tin múi giờ từ cấu hình (key "TimeZoneId"),
                                                                                                 //thường là múi giờ của hệ thống.
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);//Chuyển đổi thời gian hiện tại từ UTC thành giờ địa phương
                                                                                         //dựa trên múi giờ lấy từ cấu hình
            var tick = DateTime.Now.Ticks.ToString();//tick là số ticks hiện tại (tính bằng 100 nano-giây kể từ 01/01/0001),
                                                     //được sử dụng để tạo mã giao dịch duy nhất cho mỗi lần thanh toán.
            var pay = new VnPayLibrary();//Khởi tạo một đối tượng VnPayLibrary,
                                         //đây là một thư viện hoặc lớp tự xây dựng để xử lý việc giao tiếp với VNPay
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];//Lấy URL callback  từ cấu hình.

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);//Phiên bản của API VNPay, lấy từ cấu hình.
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);//Lệnh thực thi (thường là pay cho giao dịch thanh toán).
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);//Mã Terminal của merchant.
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());//Số tiền thanh toán, chuyển đổi sang đơn vị VNĐ và nhân với 100 (VNPay yêu cầu số tiền là VNĐ * 100).
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));//Thời gian tạo yêu cầu thanh toán theo định dạng yyyyMMddHHmmss.
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);//Mã tiền tệ (thường là VND), lấy từ cấu hình.
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));//Địa chỉ IP của người dùng lấy từ ngữ cảnh HTTP.
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);//Ngôn ngữ hiển thị của trang thanh toán, lấy từ cấu hình.
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");//Thông tin đơn hàng (tên, mô tả đơn hàng, số tiền).
            pay.AddRequestData("vnp_OrderType", model.OrderType);//Loại đơn hàng, lấy từ model 
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);//URL callback, nơi VNPay chuyển hướng sau khi thanh toán xong.
            pay.AddRequestData("vnp_TxnRef", tick);//Mã tham chiếu giao dịch, được tạo từ tick để đảm bảo tính duy nhất.

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);//Hàm CreateRequestUrl của pay tạo URL yêu cầu thanh toán từ các tham số đã được thiết lập và mã hóa theo yêu cầu của VNPay,
                                                                                                            // sử dụng BaseUrl và HashSecret từ cấu hình.

            return paymentUrl;//Trả về URL thanh toán đã tạo để người dùng có thể truy cập và thực hiện giao dịch.
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)// nhận kết quả trả về từ VNPay qua tham số collections.
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);//Phân tích và xác minh dữ liệu phản hồi từ VNPay,
                                                                                                    //sử dụng HashSecret để đảm bảo tính toàn vẹn của dữ liệu.

            return response;//Trả về một đối tượng PaymentResponseModel chứa kết quả giao dịch (thành công hoặc thất bại).
        }
    }
}
