using System;
using System.Xml;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// Address contains all significant fields for shipper and shipTo addresses.
	/// This class used in Rate Requests. If country has Postal Code, all 
	/// other fields is not important, otherwise location must be defined by 
	/// city, country code and state province code.
	/// For detail information read rates_service.pdf Table 1: Rates & Service Selection Request Input
	/// </summary>
	public struct Address {
		
		public String City;
		public String StateProvinceCode;
		public String PostalCode;
		public String CountryCode;
		public bool ResidentialAddress;

		public Address(String City, String StateProvinceCode, 
						String PostalCode, String CountryCode) {
			this.City = City;
			this.StateProvinceCode = StateProvinceCode;
			this.PostalCode = PostalCode;
			this.CountryCode = CountryCode;
			this.ResidentialAddress = false;
		}

		/// <summary>
		/// GetNode convert all internal fields into XMLElement.
		/// </summary>
		/// <param name="document">document used for "request xml" creation.</param>
		/// <returns>Node, representing Address element.</returns>
		public XmlNode GetNode(XmlDocument document) {
			if (document == null) throw new Exception("XmlDocument is not exist.");
			XmlElement Address = document.CreateElement("Address");
			if (this.City != null) {
				XmlElement City = document.CreateElement("City");
				City.InnerText = this.City;
				Address.AppendChild(City);
			}
			if (this.StateProvinceCode != null) {
				XmlElement StateProvinceCode = document.CreateElement("StateProvinceCode");
				StateProvinceCode.InnerText = this.StateProvinceCode;
				Address.AppendChild(StateProvinceCode);
			}
			if (this.PostalCode != null) {
				XmlElement PostalCode = document.CreateElement("PostalCode");
				PostalCode.InnerText = this.PostalCode;
				Address.AppendChild(PostalCode);
			}
			if (this.CountryCode != null) {
				XmlElement CountryCode = document.CreateElement("CountryCode");
				CountryCode.InnerText = this.CountryCode;
				Address.AppendChild(CountryCode);			
			}
			if (this.ResidentialAddress) {
				XmlElement ResidentialAddress = document.CreateElement("ResidentialAddress");
				ResidentialAddress.InnerText = (this.ResidentialAddress == false) ? "0" : "1";
				Address.AppendChild(ResidentialAddress);

				XmlElement ResidentialAddressIndicator = document.CreateElement("ResidentialAddressIndicator");
				ResidentialAddressIndicator.InnerText = "1";
				Address.AppendChild(ResidentialAddressIndicator);
			}

			return Address;
		}

	}
}
