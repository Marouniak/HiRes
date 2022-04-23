using System;
using System.Xml;

namespace HiRes.ShipmentManager.UPS {

	/// <summary>
	/// Package is used in RatingServiceSelectionRequest.
	/// Package represents one package in shipment and keep 
	/// Packaging Type, Package Weight and Dimensions.
	/// Detailed description in rates_service.pdf, Table 1: Rates & Service Selection Request Input
	/// </summary>
	public struct Package {
		
		public PackageTypeCodes PackagingType;
		private String unitOfDimensionMeasurement;
		public float Length, Width, Height;
		private String unitOfWeightMeasurement;
		public float Weight;
		public bool AdditionalHandling;
		public float InsuredValue;

		public const int EMailNotification = 3;
		public const int FAXNotification = 4;
		public int ShipmentNotification;

		public String UnitOfDimensionMeasurement {
			get { 
				if (unitOfDimensionMeasurement == null)
					return "IN";
				return unitOfDimensionMeasurement;
			}
			set {
				if (value == "CM") unitOfDimensionMeasurement = value;
				else unitOfDimensionMeasurement = "IN";
			}
		}

		public String UnitOfWeightMeasurement {
			get { 
				if (unitOfWeightMeasurement == null)
					return "LBS";
				return unitOfWeightMeasurement; 
			}
			set {
				if (value == "KGS") unitOfWeightMeasurement = value;
				else unitOfWeightMeasurement = "LBS";
			}
		}

		/// <summary>
		/// GetNode convert all internal fields into XMLElement.
		/// </summary>
		/// <param name="document">document used for "request xml" creation</param>
		/// <returns>Node, representing Package element.</returns>
		public XmlNode GetNode(XmlDocument document) {
			if (document == null) throw new Exception("XmlDocument is not exist.");
			XmlElement Package = document.CreateElement("Package");
			XmlElement PackagingType = document.CreateElement("PackagingType");
			XmlElement Code = document.CreateElement("Code");
			Code.InnerText = ((int)this.PackagingType).ToString("00");
			PackagingType.AppendChild(Code);
			Package.AppendChild(PackagingType);
			if ((this.Width>=1) && (this.Height>=1) && (this.Length>=1)) {
				XmlElement Dimensions = document.CreateElement("Dimensions");
				XmlElement UnitOfMeasurement = document.CreateElement("UnitOfMeasurement");
				Code = document.CreateElement("Code");
				Code.InnerText = this.UnitOfDimensionMeasurement;
				UnitOfMeasurement.AppendChild(Code);
				Dimensions.AppendChild(UnitOfMeasurement);
				XmlElement Length = document.CreateElement("Length");
				XmlElement Width = document.CreateElement("Width");
				XmlElement Height = document.CreateElement("Height");
				Length.InnerText = this.Length.ToString("F");
				Width.InnerText = this.Width.ToString("F");
				Height.InnerText = this.Height.ToString("F");
				Dimensions.AppendChild(Length);
				Dimensions.AppendChild(Width);
				Dimensions.AppendChild(Height);
				Package.AppendChild(Dimensions);
			}
			if (this.Weight>0) {
				XmlElement PackageWeight = document.CreateElement("PackageWeight");
				XmlElement UnitOfMeasurement = document.CreateElement("UnitOfMeasurement");
				Code = document.CreateElement("Code");
				Code.InnerText = this.UnitOfWeightMeasurement;
				UnitOfMeasurement.AppendChild(Code);
				PackageWeight.AppendChild(UnitOfMeasurement);
				XmlElement Weight = document.CreateElement("Weight");
				Weight.InnerText = this.Weight.ToString("F");
				PackageWeight.AppendChild(Weight);
				Package.AppendChild(PackageWeight);
			}
			if (this.InsuredValue > 0) {
				XmlElement PackageServiceOptions = document.CreateElement("PackageServiceOptions");
				XmlElement InsuredValue = document.CreateElement("InsuredValue");
				XmlElement CurrencyCode = document.CreateElement("CurrencyCode");
				XmlElement MonetaryValue = document.CreateElement("MonetaryValue");
				CurrencyCode.InnerText = "USD";
				MonetaryValue.InnerText = this.InsuredValue.ToString("F");
				InsuredValue.AppendChild(CurrencyCode);
				InsuredValue.AppendChild(MonetaryValue);
				PackageServiceOptions.AppendChild(InsuredValue);
				Package.AppendChild(PackageServiceOptions);
			}
			if (this.AdditionalHandling) {
				XmlElement AdditionalHandling = document.CreateElement("AdditionalHandling");
				AdditionalHandling.InnerText = (this.AdditionalHandling == false) ? "0" : "1";
				Package.AppendChild(AdditionalHandling);
			}
			if (this.ShipmentNotification == EMailNotification || this.ShipmentNotification == FAXNotification) {
				XmlElement ShipmentNotification = document.CreateElement("ShipmentNotification");
				XmlElement NotificationCode = document.CreateElement("NotificationCode");
				NotificationCode.InnerText = this.ShipmentNotification.ToString();
				ShipmentNotification.AppendChild(NotificationCode);
				Package.AppendChild(ShipmentNotification);
				if (this.ShipmentNotification == FAXNotification) {
					XmlElement AttentionName = document.CreateElement("AttentionName");
					AttentionName.InnerText = "Test";
					ShipmentNotification.AppendChild(AttentionName);

					XmlElement FaxDestination = document.CreateElement("FaxDestination");
					XmlElement FaxNumber = document.CreateElement("FaxNumber");
					FaxNumber.InnerText = "111111111111111";
					FaxDestination.AppendChild(FaxNumber);
					ShipmentNotification.AppendChild(FaxDestination);

					XmlElement PhoneNumber = document.CreateElement("PhoneNumber");
					PhoneNumber.InnerText = "111111111111111";
					ShipmentNotification.AppendChild(PhoneNumber);
				}
			}
			return Package;
		}
		
	}
}
