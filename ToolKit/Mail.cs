using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit
{
	public class Mail
	{
		private static string qqmailAddress = "1***@qq.com";
		public static string wangyimailAddress = "d***@163.com";
		public static void SendQQMail(string receiver, string message, string title)
		{
			SmtpClient client = new SmtpClient();
			MailMessage mail = new MailMessage();
			mail.From = new MailAddress(qqmailAddress, "my qq mail");
			mail.To.Add(receiver);
			mail.Subject = title;
			mail.BodyEncoding = System.Text.Encoding.UTF8;
			mail.IsBodyHtml = true;
			mail.Body = message;
			client.Host = "smtp.qq.com";
			client.Credentials = new System.Net.NetworkCredential(qqmailAddress, "fflnbfpxfhvqbafh");
			client.EnableSsl = false;
			client.Send(mail);
		}
		public static void SendWangyiMail()
		{
			SmtpClient client = new SmtpClient("smtp.163.com", 25);
			client.EnableSsl = true;
			client.Credentials = new System.Net.NetworkCredential("收件箱账号@163.com", "你的ssh for 163");
			MailMessage mail = new MailMessage();
			mail.From = new MailAddress("你的163号@163.com");
			mail.To.Add(new MailAddress("收件箱账号@qq.com"));
			mail.IsBodyHtml = true;
			mail.Priority = MailPriority.Normal;
			mail.Subject = "这是主题";
			mail.Body = "这是内容";
			client.Send(mail);


		}
	}
}
