using System;
using System.Text;
using System.Web.Mail;

using HiRes.Common;

namespace HiRes.MailingSystem {
	/// <summary>
	/// Summary description for Class1.
	/// </summary>

	public class MassMailer {

		public string FromEmail;

		public MailMessage CreateMailMessage(string toEmail, string strMessage, bool isHtml) {
			MailMessage msg = new MailMessage();
			msg.From = FromEmail==null?AppConfig.EmailSender:FromEmail;
			msg.To = toEmail;
			msg.Body = strMessage;
			if (isHtml) {
				msg.BodyFormat = MailFormat.Html;
			} else { msg.BodyFormat = MailFormat.Text; }
			return msg;
		}

		public void SendMessage(MailMessage msg) {
			SmtpMail.Send(msg);
		}

		public void SendMessage(string toEmail, string strMessage, bool isHtml) {
			//TODO: implement it the way to send mail asynchroniously			
			MailMessage msg = CreateMailMessage(toEmail, strMessage, isHtml);
			//SmtpMail.Send(CreateMailMessage(toEmail, strMessage, isHtml));

			SmtpMail.SmtpServer = AppConfig.SmtpServer;
			SmtpMail.Send(msg);

			//SmtpMail.Send(FromEmail==null?AppConfig.EmailSender:FromEmail,toEmail,"test",strMessage);
				
		}

/*		public void SendMessage(ICollection emails, string strMessage, bool isHtml) {

		}
*/
		public void OnApplicationStart() {
			SmtpMail.SmtpServer = AppConfig.SmtpServer;
		}

	}


	public class MailGenerator {

		public static string GeneratePromoCodeMsg(PromoCodeInfo codeInfo) {
			//TODO: change this
			return GeneratePromoCodeMsg(codeInfo,null);
		}

		public static string GeneratePromoCodeMsg(PromoCodeInfo codeInfo, CustomerInfo customerInfo) {
			if (codeInfo==null) {
				throw new ArgumentNullException("codeInfo");
			}
			// TODO: Use xml template for building the email
			StringBuilder sb = new StringBuilder();
			sb.Append("Dear customer, <br>");
			sb.Append("Promotion code: ");
			sb.Append(codeInfo.Code);
			sb.Append("<BR>");
			return sb.ToString();
		}
	}
}
