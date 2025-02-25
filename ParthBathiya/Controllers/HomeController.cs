using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ParthBathiya.Google;

namespace ParthBathiya.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult SendMessage(string name, string email, string mobile, string message)
        {
            try
            {
                #region SMTP client setup
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(AppSettingGoogleModel.SystemEmail, AppSettingGoogleModel.SystemPassword),
                    EnableSsl = true
                };
                #endregion

                #region Create the email body
                string body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Inquiry</h2>
            <p><strong>Name:</strong> {name}</p>
            <p><strong>Email:</strong> {email}</p>
            <p><strong>Mobile:</strong> {mobile}</p>
            <p><strong>Message:</strong>{message}</p>
        </body>
        </html>";
                #endregion

                #region Create the email
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(AppSettingGoogleModel.SystemEmail),
                    Subject = "Inquery",
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(AppSettingGoogleModel.ToEmail);
                #endregion

                #region Send email
                smtpClient.Send(mailMessage);

                TempData["AlertMsg"] = "Message sent successfully.";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index");

                #endregion
            }
            catch (Exception)
            {
                TempData["AlertMsg"] = "Message sending failed.";
                TempData["AlertType"] = "error";

                return RedirectToAction("Index");
            }
        }
    }
}
