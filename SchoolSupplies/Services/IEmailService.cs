using System.Net;
using System.Net.Mail;

namespace SchoolSupplies.Services
{
    public interface IEmailService
    {
        Task SendForgotPasswordEmailAsync(string email, string callbackUrl);
    }

    public class EmailService : IEmailService
    {
        public async Task SendForgotPasswordEmailAsync(string email, string callbackUrl)
        {
            var subject = "Reset your password";
            var message = $"Please reset your password by clicking this link: <a href='{callbackUrl}'>link</a>";
            var fromEmail = "tannarzo23@outlook.com"; // Cập nhật email của bạn
            var fromPassword = "NhatTaN00444$"; // Mật khẩu ứng dụng hoặc mật khẩu email của bạn

            try
            {
                using (var client = new SmtpClient("smtp.office365.com", 587)) // Sử dụng máy chủ SMTP của Microsoft (Outlook)
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false; // Đảm bảo không sử dụng thông tin xác thực mặc định
                    client.Credentials = new NetworkCredential(fromEmail, fromPassword);

                    var mailMessage = new MailMessage(fromEmail, email, subject, message)
                    {
                        IsBodyHtml = true
                    };

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                // Ghi lại thông báo lỗi chi tiết
                Console.WriteLine($"SMTP Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Ghi lại các ngoại lệ khác
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }
    }
}
