using System;
using System.Collections;

using HiRes.BusinessRules;
using HiRes.Common;
using HiRes.MailingSystem;
using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessFacade {
	/// <summary>
	/// Summary description for PromoCodeFacade.
	/// </summary>
	public class PromoCodeFacade {
		// result codes for CanBeUsedByCustomer method
		public const int ERR_OK = 0;
		public const int ERR_FIRSTIMERSONLY = -1;
		public const int ERR_NOTCOOPERATIVE = -2;
		public const int ERR_OUTDATED = -3;
		public const int ERR_USAGELIMITUSEDUP = -4;


		public PromoCodeInfo GetCodeInfo (string code) {
			return new PromoCode().GetCodeInfo(code);
		}

		public bool AddCode(PromoCodeInfo codeInfo) {
			return new PromoCode().AddCode(codeInfo);
		}

		public void RemoveCode(string code) {
			PromoCode promoCode = new PromoCode();
			PromoCodeInfo codeInfo = promoCode.GetCodeInfo(code);
			if (promoCode.GetTotalTimesUsed(code)>0) {
				promoCode.SuspendCode(code);// if code was used 
			} else {
				promoCode.DeleteCode(code);// if code were not used yet then remove it from database
			}
			
		}

		/// <summary>
		/// This method perform all neccesary checkings to determine whether the given
		/// promotion code can be used by customer. The value of AppConfig.siteId is used 
		/// </summary>
		/// <param name="code">promotion code</param>
		/// <param name="CustomerId">customer id</param>
		/// <param name="useAsSupplementary">if true checks if code can be used as supplementary</param>
		/// <returns><code>true</code> if code can be used by customer.</returns>
		public bool CanBeUsedByCustomer(string code, string CustomerId, bool useAsSupplementary, out int resCode) {
			PromoCode promoCode = new PromoCode();
			
			PromoCodeInfo codeInfo = promoCode.GetCodeInfo(code);
			
			if (codeInfo.UsageConditions.FirstTimeCustomerOnly) {
				// check if orders has been placed
				if (new Customer().GetPlacedOrdersNum(AppConfig.siteId,CustomerId)>0) {
					resCode = ERR_FIRSTIMERSONLY;
					return false;
				}
			}

			if (useAsSupplementary) {
				if (!codeInfo.IsCooperative) {
					resCode = ERR_NOTCOOPERATIVE;
					return false;
				}
			}
			if ((codeInfo.ValidFrom>DateTime.Now)||(codeInfo.ValidTo<DateTime.Now)) {
				resCode = ERR_OUTDATED;
				return false;
			}
			if (codeInfo.CodeType==PromoCodeInfo.PromoCodeType.Disposable) {
				if ((codeInfo.UsageConditions.MaxUseNumber - promoCode.GetTimesUsedByCustomer(code, AppConfig.siteId, CustomerId))<=0) {
					resCode = ERR_USAGELIMITUSEDUP;
					return false;
				}
			}
			resCode = ERR_OK;
			return true;
		}

		
		public bool SendCode(string code) {
			if ((code==null)||(code.Equals(String.Empty))) {
				throw new ArgumentNullException("code");
			}
			PromoCodeInfo codeInfo = new PromoCode().GetCodeInfo(code);
			if (codeInfo==null) {
				return false;
			}
			SendCode(codeInfo);
			return true;
		}

		public bool SendCode(PromoCodeInfo codeInfo) {
            return SendCode(codeInfo, null, false);
		}

		public bool SendCode(PromoCodeInfo codeInfo, string message, bool isHtml) {
			if (codeInfo==null) {
				throw new ArgumentNullException("codeInfo");
			}
			if (codeInfo.UsageConditions.FirstTimeCustomerOnly) {
				throw new ArgumentException("Code info shouldn't be for 'first time customer'","codeInfo");
			}
			
			try {
				string emailBody;
			
/*				if (message==null) {
					emailBody = MailGenerator.GeneratePromoCodeMsg(codeInfo,recipient);
				} else { emailBody = message; }
*/
				PromoCode promoCode = new PromoCode();
				// get list of customers that can use the code
				ArrayList recipients = promoCode.GetCustomerRecipients(codeInfo);
			
				if (recipients.Count == 0) return true;
			
				MassMailer mailer = new MassMailer();

				emailBody = message;
				for (int i=0; i < recipients.Count ; i++) {
					CustomerInfo recipient = (CustomerInfo)recipients[i];
					if (message==null) {
						emailBody = MailGenerator.GeneratePromoCodeMsg(codeInfo,recipient);
					}
					mailer.SendMessage(recipient.ContactEmail,emailBody,isHtml);
					//				
				}
				return true;
			} catch (Exception ex) {
				AppLog.LogError("Error while sending promotion code",ex);
				return false;
			}
		}
		public PromoCodeInfo[] GetPromoCodes(FilterExpression filter, OrderExpression orderBy) {
			return new PromoCode().GetPromoCodes(filter,orderBy);
		}

	}
}
