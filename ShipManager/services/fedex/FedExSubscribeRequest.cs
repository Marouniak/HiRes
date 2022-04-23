/**
 * FILE: FedExSubscribeRequest.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: Subscribes for FedEx services
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		27-Apr-2002	Maxim Lysenko
 *			Initial implementation
 */

using System;

namespace HiRes.ShipmentManager.FedEx {
	public struct FedExContactInfo {
		public string ContactName;
		public string CompanyName;
		public string AddressLine1;
		public string AddressLine2;
		public string City;
		public string State;
		public string PostalCode;
		public string CountryCode;
		public string PhoneNumber;
		public FedExAccessInfo AccessInfo;
		
		public FedExContactInfo(string accountNumber) : this("", "", "", "", "", "", "", "", "", accountNumber) {
		}

		public FedExContactInfo(string contactName, string companyName, string addressLine1,
			string addressLine2, string city, string stateCode, string postalCode, 
			string countryCode, string phoneNumber, string accountNumber
			) {
			this.ContactName = contactName;
			this.CompanyName = companyName;
			this.AddressLine1 = addressLine1;
			this.AddressLine2 = addressLine2;
			this.City = city;
			this.State = stateCode;
			this.PostalCode = postalCode;
			this.CountryCode = countryCode;
			this.PhoneNumber = phoneNumber;
			this.AccessInfo = new FedExAccessInfo(accountNumber, String.Empty);
		}
	}

	/// <summary>
	/// Summary description for FedExSubscribeRequest.
	/// </summary>
	public class FedExSubscribeRequest {
		private FedExAPI fedExAPI;

		public FedExSubscribeRequest(FedExAPI fedExAPI) {
			this.fedExAPI = fedExAPI;
		}

		/// <summary>
		/// Sends subscribe request to Fedex server. The server returns string like this
		/// the "0,"311"498,"1014502"4021-1,"FedEx Express Shipping"4021-2,"FedEx Ground Shipping"99,"""
		/// </summary>
		/// <param name="contactInfo"></param>
		/// <returns>FedEx meter number in FedExAccessInfo structure</returns>
		public FedExAccessInfo SendRequest(ref FedExContactInfo contactInfo) {
			FedExRequestString request = new FedExRequestString();
			request.Append("0", "211")
				.Append("4003", contactInfo.ContactName)
				.Append("4007", contactInfo.CompanyName)
				.Append("4008", contactInfo.AddressLine1)
				.Append("4009", contactInfo.AddressLine2)
				.Append("4011", contactInfo.City)
				.Append("4012", contactInfo.State)
				.Append("4013", contactInfo.PostalCode)
				.Append("4014", contactInfo.CountryCode)
				.Append("4015", contactInfo.PhoneNumber)
				.Append("10",   contactInfo.AccessInfo.AccountNumber);
			
			string responseString;
			this.fedExAPI.ProcessTransaction(UniversalTransactionIdentifier.ALL_SUBSCRIPTION, request.ToString(), out responseString);

			FedExRequestString response = new FedExRequestString(responseString);
			
			if (response.ErrorMessage != String.Empty) {
				throw new FedExTxnException(response.ErrorMessage);
			} 

			contactInfo.AccessInfo.MeterNumber = response["498"];

			return contactInfo.AccessInfo;
		}
	}
}
