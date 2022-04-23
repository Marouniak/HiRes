/**
 * FILE: IFedExRequest.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: FedExRequest class and interface for FedExRequest classes. 
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		27-Apr-2002	Maxim Lysenko
 *			Initial implementation
 */
using System;
using System.Text;

namespace HiRes.ShipmentManager.FedEx {
	/// <summary>
	/// Provides convinient way to work with FedEx transactions
	/// </summary>
	public class FedExRequestString {
		private StringBuilder request;

		public const int DEFAULT_REQUEST_SIZE = 4096;

		public string this[string fieldName] {
			get {
				string searchString = request.ToString();
				fieldName += ",\"";
				int index =	searchString.IndexOf(fieldName);
				string result = String.Empty;
				if ((index > 0) && (searchString.Substring(index - 1, 1 ) == "\"")) {
					int startIndex = index + fieldName.Length;
					int endIndex = searchString.IndexOf("\"", startIndex);
					if (endIndex > 0) {
						result = searchString.Substring(startIndex, endIndex - startIndex);
					}
				}
				return result;
			}
		}

		public string ErrorMessage {
			get {
				if (this["2"] != string.Empty) {
					return "Code: " + this["2"] + " Message: " + this["3"];
				} else {
					return String.Empty;
				}
			}
		}

		public FedExRequestString() {
			request = new StringBuilder(DEFAULT_REQUEST_SIZE);
		}

		public FedExRequestString(string value) {
			request = new StringBuilder(value, DEFAULT_REQUEST_SIZE);
		}

		public FedExRequestString Append(string fieldName, string fieldValue) {
			if (fieldValue != string.Empty) {
				request.Append(fieldName).Append(",\"").Append(fieldValue).Append("\"");
			}
			return this;
		}

		public FedExRequestString Append(string fieldName, int fieldValue) {
			if (fieldValue > 0) {
				request.Append(fieldName).Append(",\"").Append(fieldValue).Append("\"");
			}
			return this;
		}

		public FedExRequestString Append(string fieldName, double fieldValue) {
			if (fieldValue > 0) {
				request.Append(fieldName).Append(",\"").Append(fieldValue).Append("\"");
			}
			return this;
		}

		public FedExRequestString Append(string fieldName, decimal fieldValue) {
			if (fieldValue > 0) {
				request.Append(fieldName).Append(",\"").Append(fieldValue).Append("\"");
			}
			return this;
		}

		public FedExRequestString Append(string fieldName, bool fieldValue) {
			if (fieldValue) {
				request.Append(fieldName).Append(",\"Y\"");
			}
			return this;
		}

		public FedExRequestString Append(string fieldName, FedExPackagingType fieldValue) {
			if ((int)fieldValue > 0) {
				request.Append(fieldName).Append(",\"").Append(((int)fieldValue).ToString("D2")).Append("\"");
			}
			return this;
		}

		public FedExRequestString Append(string fieldName, FedExShippingService fieldValue) {
			if ((int)fieldValue > 0) {
				request.Append(fieldName).Append(",\"").Append(((int)fieldValue).ToString("D2")).Append("\"");
			}
			return this;
		}

		public string getString(string fieldName) {
			return this[fieldName];
		}

		public bool getBool(string fieldName) {
			if (this[fieldName] == string.Empty) {
				return false;
			} else {
				return this[fieldName] == "Y";
			}
		}

		public int getInt(string fieldName) {
			if (this[fieldName] == string.Empty) {
				return 0;
			} else {
				return int.Parse(this[fieldName]);
			}
		}

		public double getDouble(string fieldName) {
			if (this[fieldName] == string.Empty) {
				return 0.0;
			} else {
				return double.Parse(this[fieldName]);
			}
		}
		
		public decimal getDecimal(string fieldName) {
			if (this[fieldName] == string.Empty) {
				return 0.0m;
			} else {
				return decimal.Parse(this[fieldName]);
			}
		}

		public override string ToString() {
			request.Append("99,\"\"");
			return request.ToString();
		}
	}

	/// <summary>
	/// Summary description for IFedExRequest.
	/// </summary>
	public interface IFedExRequest {
		void SendRequest();
	}
}
