using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FollowUp.Data.Services
{
    public class EmailProvider : IEmailProvider
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public EmailProvider(IConfiguration config, ApplicationDbContext context)
        {
            _context = context;
            _config = config;
        }

        public async Task<int> SendMail(int attendId, int tableId, string UserId, string Value, int? minut)
        {
            var attendance = await _context.Attendances.FindAsync(attendId);

            var user = await _context.ApplicationUsers.FindAsync(UserId);
            if (user.Email == null) return 0;

            var table = await _context.Tables
                .Include(y => y.Build)
                .Include(y => y.Course)
                .FirstOrDefaultAsync(x => x.Id == tableId);

            string subject;
            string templatePath;

            if (Value == "غائب")
            {
                subject = "تقرير عدم حضور";
                templatePath = Directory.GetCurrentDirectory() + "/wwwroot/Email.html";
            }
            else
            {
                subject = "تقرير تأخير";
                templatePath = Directory.GetCurrentDirectory() + "/wwwroot/Email2.html";
            }

            string htmlTemplate = System.IO.File.ReadAllText(templatePath);

            htmlTemplate = htmlTemplate.Replace("MessEMf", user.UserFullName);
            htmlTemplate = htmlTemplate.Replace("MessEMg", table.Course.Name);
            htmlTemplate = htmlTemplate.Replace("MessEMa", table.Day);
            htmlTemplate = htmlTemplate.Replace("MessEMb", attendance.HijriDate);
            htmlTemplate = htmlTemplate.Replace("MessEMc", attendance.Value);
            htmlTemplate = htmlTemplate.Replace("MessEMd", table.ContactHours.ToString());
            htmlTemplate = htmlTemplate.Replace("MessEMe", table.Time);
            if (Value != "غائب")
            {
                htmlTemplate = htmlTemplate.Replace("MessEMt", attendance.Minutes.ToString());
            }

            var sender = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == "Admin");

            var message = new MailMessage();
            message.From = new MailAddress( sender.Email);
            message.To.Add(new MailAddress(user.Email));
            message.Subject = subject;
            message.Body = htmlTemplate;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient(_config["stmp:Host"], int.Parse(_config["stmp:Port"])))
            {
                smtp.Credentials = new NetworkCredential(sender.Email, sender.UserFullName);
                smtp.EnableSsl = true;

                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }

            return 0;
        }


    }
}