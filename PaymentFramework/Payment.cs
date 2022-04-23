using System;
using System.Globalization;
using System.Text;
using System.Collections.Specialized;

using HiRes.Common;

namespace HiRes.PaymentFramework {
/*
	public enum PaymentTransactionType {
		Sale = 'S',
		Credit = 'C',
		Authrozation = 'A',
		DelayedCapture = 'D',
		Void = 'V',
		VoiceAuthorization = 'F',
		Inquiry = 'I'
	}
*/
	/// <summary>
	/// Summary description for Payment.
	/// </summary>
	public class PaymentProcessor {

/*		public PaymentProcessor() {
		}
*/
		/// <summary>
		/// Call this method to process payment transaction
		/// </summary>
		/// <param name="creditCardNumber">For test purposes use this one: 5105105105105100</param>
		/// <param name="expirationDate">For test purposes use this one: 0804</param>
		/// <param name="amount">For test purposes use values lower than 99$</param>
		/// <returns></returns>
		[Obsolete("Use ProcessTransaction(PaymentTransactionInfo txn)",true)]
		public PaymentTransactionResponse ProcessTransaction (
				String creditCardNumber,
				String expirationDate,	// The format is MMYY
				decimal amount) {
			StringBuilder paramList = new StringBuilder();
			paramList.Append("TRXTYPE=S");
			paramList.Append("&TENDER=C");
			paramList.Append("&PARTNER="); paramList.Append(PaymentConfigHandler.Partner);
			paramList.Append("&VENDOR="); paramList.Append(PaymentConfigHandler.Vendor);
			paramList.Append("&USER="); paramList.Append(PaymentConfigHandler.User);
			paramList.Append("&PWD="); paramList.Append(PaymentConfigHandler.Pwd);
			paramList.Append("&ACCT="); paramList.Append(creditCardNumber);
			paramList.Append("&EXPDATE="); paramList.Append(expirationDate);
			
			paramList.Append("&AMT="); paramList.Append(amount.ToString("F",NumberFormatInfo.InvariantInfo));

			string strResponse = SubmitTransaction(paramList.ToString());
			NameValueCollection response = ParseResponse(strResponse);
			PaymentTransactionResponse ptr = new PaymentTransactionResponse();
			ptr.PNREF = response["PNREF"];
			ptr.RESPMSG = response["RESPMSG"];
			ptr.RESULT = Int32.Parse(response["RESULT"]);
			return ptr;
		}
		
		/*private PaymentTransactionResponse VoidTrnasaction(string txnRefId) {
		}*/
		public PaymentTransactionResponse ProcessTransaction(PaymentTransactionInfo txn) {

			string paramString = BuildParamString(txn);

			string strResponse = SubmitTransaction(paramString);

			NameValueCollection response = ParseResponse(strResponse);
			PaymentTransactionResponse ptr = new PaymentTransactionResponse();
			ptr.PNREF = response["PNREF"];
			ptr.RESPMSG = response["RESPMSG"];
			ptr.RESULT = Int32.Parse(response["RESULT"]);
			//if (response["AUTHCODE"]!=null) {}
			ptr.AuthCode = response["AUTHCODE"];
			ptr.AVSAddr = (response["AVSADDR"]!=null)?response["AVSADDR"][0]:'X';
			ptr.AVSZip = (response["AVSZIP"]!=null)?response["AVSZIP"][0]:'X';
			return ptr;

		}
		
		private string BuildParamString(PaymentTransactionInfo txn) {
			return BuildParamString(txn,false);
		}

		private string BuildParamString(PaymentTransactionInfo txn, bool modifyAmount) {
			StringBuilder paramList = new StringBuilder();
			paramList.Append("TRXTYPE=");paramList.Append((char)((ushort)txn.TxnType));
			paramList.Append("&TENDER=C");
			//---------------------------------------------------------------------------
			paramList.Append("&PARTNER="); paramList.Append(PaymentConfigHandler.Partner);
			paramList.Append("&VENDOR="); paramList.Append(PaymentConfigHandler.Vendor);
			paramList.Append("&USER="); paramList.Append(PaymentConfigHandler.User);
			paramList.Append("&PWD="); paramList.Append(PaymentConfigHandler.Pwd);
			//---------------------------------------------------------------------------
			switch (txn.TxnType) {
				case PaymentTransactionType.Void: 
					paramList.Append("&ORIGID="); paramList.Append(txn.OriginTxnId);
					break;
				case PaymentTransactionType.Sale:
					paramList.Append("&ACCT="); paramList.Append(txn.PaymentSource.AccountNumber);
					paramList.Append("&EXPDATE="); paramList.Append(txn.PaymentSource.ValidToMonthYear);
					paramList.Append("&AMT="); paramList.Append(txn.Amount.ToString("F",NumberFormatInfo.InvariantInfo));
					break;
				case PaymentTransactionType.Authrozation:
					paramList.Append("&ACCT="); paramList.Append(txn.PaymentSource.AccountNumber);
					paramList.Append("&EXPDATE="); paramList.Append(txn.PaymentSource.ValidToMonthYear);
					paramList.Append("&AMT="); paramList.Append(txn.Amount.ToString("F",NumberFormatInfo.InvariantInfo));
					break;
				case PaymentTransactionType.Credit:
					paramList.Append("&ORIGID="); paramList.Append(txn.OriginTxnId);
					if (modifyAmount) {
						paramList.Append("&AMT="); paramList.Append(txn.Amount.ToString("F",NumberFormatInfo.InvariantInfo));
					}
					break;
				case PaymentTransactionType.DelayedCapture:
					paramList.Append("&ORIGID="); paramList.Append(txn.OriginTxnId);
					if (modifyAmount) {
						paramList.Append("&AMT="); paramList.Append(txn.Amount.ToString("F",NumberFormatInfo.InvariantInfo));
					}
					break;
				case PaymentTransactionType.Inquiry:
					paramList.Append("&ORIGID="); paramList.Append(txn.OriginTxnId);
					break;
				default:
					throw new NotSupportedException("Not supported transaction type.");
			}
			/*paramList.Append("&ACCT="); paramList.Append(creditCardNumber);
			paramList.Append("&EXPDATE="); paramList.Append(expirationDate);*/
			return paramList.ToString();

		}

		private string SubmitTransaction(string paramList) {
			PFProCOMLib.IPFProCOM pfPro = new PFProCOMLib.PNComClass();
			pfPro.HostAddress = PaymentConfigHandler.HostAddress;
			pfPro.HostPort = PaymentConfigHandler.HostPort;
			pfPro.TimeOut = PaymentConfigHandler.TimeOut;
			if (PaymentConfigHandler.ProxyAddress != "")
				pfPro.ProxyAddress = PaymentConfigHandler.ProxyAddress;
			if (PaymentConfigHandler.ProxyPort != "")
				pfPro.ProxyPort = PaymentConfigHandler.ProxyPort;
			if (PaymentConfigHandler.ProxyLogon != "")
				pfPro.ProxyLogon = PaymentConfigHandler.ProxyLogon;
			if (PaymentConfigHandler.ProxyPassword != "")
				pfPro.ProxyPassword = PaymentConfigHandler.ProxyPassword;

			pfPro.ParmList = paramList;
			pfPro.ProcessTransaction();
			return pfPro.Response;
		}
		/// <summary>
		/// Parse response and transform it to name-value collection.
		/// Original response is a name-value pairs separated by "&".
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private NameValueCollection ParseResponse(String response) {
			String[] items = response.Split(new Char[] {'=', '&'});
			NameValueCollection values = new NameValueCollection(items.Length / 2);
			for (int i=0; i<items.Length / 2; i++) {
				values.Add(items[i*2], items[i*2+1]);
			}
			return values;
		}

		/*private PaymentTransactionResponse ParseResponse(string response) {
			throw new NotImplementedException();
		}*/
	}

	public class PaymentTransactionResponse {
		
		private string pnref;
		private int result = -1000;
		private string respmsg;
		private string _authCode;

		private char avsAddr;
		private char avsZip;

		/*public PaymentTransactionResponse() {
			pnref = PersistentBusinessEntity.SID_EMPTY;
			result = -1000;
			respmsg = null;
			_authCode = null;
			avsAddr= '';
			avsZip='';
		}*/
		public bool isApproved {
			get { return (result==0); }
		}

		public string PNREF {
			get { return pnref; }
			set { pnref = value; }
		}

		public int RESULT {
			get { return result; }
			set { result = value; }
		}

		public string RESPMSG {
			get { return respmsg; }
			set { respmsg = value; }
		}

		public string AuthCode {
			get { return _authCode; }
			set { _authCode=value; }
		}

		public char AVSAddr {
			get { return avsAddr; }
			set { avsAddr = value; }
		}

		public char AVSZip {
			get { return avsZip; }
			set { avsZip=value; }
		}

	}

}
