using System;
using System.Xml;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// Error object is returning for any failed request to UPS.
	/// Detailed description in rates_service.pdf, Rates & Service Selection Error Conditions.
	/// </summary>
	public class Error {

		private String errorSeverity;
		private String errorCode;
		private String errorDescription;

		public Error(XmlDocument document) {
			errorSeverity = document.GetElementsByTagName("ErrorSeverity")[0].InnerText;
			errorCode = document.GetElementsByTagName("ErrorCode")[0].InnerText;
			errorDescription = document.GetElementsByTagName("ErrorDescription")[0].InnerText;
		}

		public string ErrorSeverity {
			get {
				return errorSeverity;
			}
		}

		public string ErrorCode {
			get {
				return errorCode;
			}
		}

		public string ErrorDescription {
			get {
				return errorDescription;
			}
		}

		public override String ToString() {
			return errorCode + "(" + errorSeverity + "): " + errorDescription;
		}

	}
}
