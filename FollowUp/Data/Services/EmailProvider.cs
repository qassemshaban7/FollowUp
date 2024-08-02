using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;

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

        public async Task<int> SendMail( int attendId, int tableId, string UserId, string Value, int? minut)
        {
            var attendance = await _context.Attendances.FindAsync(attendId);

            var user = await _context.ApplicationUsers.FindAsync(UserId);
            if (user.Email == null) return 0;

            var table = await _context.Tables
                .Include(y => y.Build)
                .Include(y => y.Course)
                .FirstOrDefaultAsync(x => x.Id == tableId);

            if (Value == "غائب")
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(MailboxAddress.Parse(_config["stmp:Email"]));
                mimeMessage.To.Add(MailboxAddress.Parse(user.Email));
                mimeMessage.Subject = "تقرير عدم حضور";

                string templatePath = Directory.GetCurrentDirectory() + "/wwwroot/Email.html";
                string htmlTemplate = System.IO.File.ReadAllText(templatePath);

                htmlTemplate = htmlTemplate.Replace("MessEMf", user.UserFullName);
                htmlTemplate = htmlTemplate.Replace("MessEMg", table.Course.Name);
                htmlTemplate = htmlTemplate.Replace("MessEMa", table.Day);
                htmlTemplate = htmlTemplate.Replace("MessEMb", attendance.HijriDate);
                htmlTemplate = htmlTemplate.Replace("MessEMc", attendance.Value);
                htmlTemplate = htmlTemplate.Replace("MessEMd", table.ContactHours.ToString());
                htmlTemplate = htmlTemplate.Replace("MessEMe", table.Time);

                var builder = new BodyBuilder();
                builder.HtmlBody = htmlTemplate;
                mimeMessage.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_config["stmp:Host"],
                    int.Parse(_config["stmp:Port"]),
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["stmp:Email"], _config["stmp:Pass"]);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);

                return 0;
            }
            else
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(MailboxAddress.Parse(_config["stmp:Email"]));
                mimeMessage.To.Add(MailboxAddress.Parse(user.Email));
                mimeMessage.Subject = "تقرير تأخير";

                string templatePath = Directory.GetCurrentDirectory() + "/wwwroot/Email2.html";
                string htmlTemplate = System.IO.File.ReadAllText(templatePath);

                htmlTemplate = htmlTemplate.Replace("MessEMf", user.UserFullName);
                htmlTemplate = htmlTemplate.Replace("MessEMg", table.Course.Name);
                htmlTemplate = htmlTemplate.Replace("MessEMa", table.Day);
                htmlTemplate = htmlTemplate.Replace("MessEMb", attendance.HijriDate);
                htmlTemplate = htmlTemplate.Replace("MessEMc", attendance.Value);
                htmlTemplate = htmlTemplate.Replace("MessEMt", attendance.Minutes.ToString());
                htmlTemplate = htmlTemplate.Replace("MessEMd", table.ContactHours.ToString());
                htmlTemplate = htmlTemplate.Replace("MessEMe", table.Time);

                var builder = new BodyBuilder();
                builder.HtmlBody = htmlTemplate;
                mimeMessage.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_config["stmp:Host"],
                    int.Parse(_config["stmp:Port"]),
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["stmp:Email"], _config["stmp:Pass"]);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);

                return 0;
            }
        }
    }
}