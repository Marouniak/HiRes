using System;
using System.Xml;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// AccessRequest contains autentification information 
	/// and must be included in all requests.
	/// </summary>
	public struct AccessRequest {
		public String AccessLicenseNumber;
		public String UserId;
		public String Password;

		public AccessRequest(String accessLicenseNumber, String userId, String password) {
			this.AccessLicenseNumber = accessLicenseNumber;
			this.UserId = userId;
			this.Password = password;
		}

		/// <summary>
		/// Convert AccessRequest class in xml representation.
		/// </summary>
		/// <returns>String, that contain AccessRequest xml document.</returns>
		public String ConvertToXml() {
			XmlDocument document = new XmlDocument();
			document.LoadXml("<AccessRequest/>");
			XmlElement AccessRequest = document.DocumentElement;
			XmlElement AccessLicenseNumber = document.CreateElement("AccessLicenseNumber");
			XmlElement UserId = document.CreateElement("UserId");
			XmlElement Password = document.CreateElement("Password");
			AccessLicenseNumber.InnerText = this.AccessLicenseNumber;
			UserId.InnerText = this.UserId;
			Password.InnerText = this.Password;
			AccessRequest.AppendChild(AccessLicenseNumber);
			AccessRequest.AppendChild(UserId);
			AccessRequest.AppendChild(Password);
			return "<?xml version=\"1.0\"?>" + document.InnerXml;
		}

	}
}
