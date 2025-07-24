using G3_PerfumeShop.ViewModels;
using System.Security.Policy;

namespace CodeMegaVNPay.Services;
public interface IVnPayService
{
    //Thanh toán bằng VNPay
    //Tạo URL thanh toán.
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);//PaymentInformationModel model Chứa thông tin về giao dịch cần thanh toán như số tiền,
                                                                                //mô tả đơn hàng, loại đơn hàng, v.v.
                                                                                //HttpContext context Chứa thông tin về ngữ cảnh HTTP của yêu cầu hiện tại 
                                                                                //phương thức này sẽ tạo ra URL cho trang thanh toán VNPay
                                                                                //dựa trên thông tin đơn hàng và ngữ cảnh người dùng
    //Xử lý kết quả thanh toán.
    PaymentResponseModel PaymentExecute(IQueryCollection collections);
}