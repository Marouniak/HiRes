using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;
using System.Threading;

namespace HiRes.ShipmentManager.UPS {

	/// <summary>
	/// RatingServiceSelectionRequest used for getting information about 
	/// charges for any kind of Ups Services in dependence of 
	/// shipment addresses and package features.
	/// For additional information read rates_service.pdf.
	/// </summary>
	public class RatingServiceSelectionRequest {
		private AccessRequest accessInfo;
		public PickupTypes PickupType;
		public Address ShipFromAddress;
		public Address ShipToAddress;
		public bool SaturdayPickup = false;
		public bool SaturdayDelivery = false;
		private ArrayList packages = new ArrayList();
		//private Error error;

		public RatingServiceSelectionRequest(AccessRequest accessInfo) {
			this.accessInfo = accessInfo;
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		public IList Packages {
			get { return packages; }
		}

		/// <summary>
		/// Collect all information about internal fields and build xml for request.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		private String ConvertToXml(RequestOption option, UpsServiceCodes service) {
			XmlDocument document = new XmlDocument();
			document.LoadXml("<RatingServiceSelectionRequest/>");
			XmlElement RatingServiceSelectionRequest = document.DocumentElement;
			XmlElement Request = document.CreateElement("Request");
			XmlElement RequestAction = document.CreateElement("RequestAction");
			XmlElement RequestOption = document.CreateElement("RequestOption");
			XmlElement TransactionReference = document.CreateElement("TransactionReference");
			XmlElement CustomerContext = document.CreateElement("CustomerContext");
			XmlElement XpciVersion = document.CreateElement("XpciVersion");

			RequestAction.InnerText = "Rate";
			RequestOption.InnerText = option.ToString();
			CustomerContext.InnerText = "Rating and service";
			XpciVersion.InnerText = "1.0001";
			TransactionReference.AppendChild(CustomerContext);
			TransactionReference.AppendChild(XpciVersion);
			
			Request.AppendChild(RequestAction);
			Request.AppendChild(RequestOption);
			Request.AppendChild(TransactionReference);
			RatingServiceSelectionRequest.AppendChild(Request);
			XmlElement PickupType = document.CreateElement("PickupType");
			XmlElement Code = document.CreateElement("Code");
			Code.InnerText = ((int)this.PickupType).ToString("00");
			PickupType.AppendChild(Code);
			RatingServiceSelectionRequest.AppendChild(PickupType);
			
			XmlElement CustomerClassification = document.CreateElement("CustomerClassification");
			Code = document.CreateElement("Code");
			Code.InnerText = ShipmentManagerConfigHandler.UpsCustomerClassificationCode;
			CustomerClassification.AppendChild(Code);
			RatingServiceSelectionRequest.AppendChild(CustomerClassification);

			XmlElement Shipment = document.CreateElement("Shipment");
			XmlElement Shipper = document.CreateElement("Shipper");
			XmlElement ShipTo = document.CreateElement("ShipTo");
			Shipper.AppendChild(this.ShipFromAddress.GetNode(document));
			ShipTo.AppendChild(this.ShipToAddress.GetNode(document));
			Shipment.AppendChild(Shipper);
			Shipment.AppendChild(ShipTo);
			XmlElement Service = document.CreateElement("Service");
			Code = document.CreateElement("Code");
			Code.InnerText = ((int)service).ToString("00");
			Service.AppendChild(Code);
			Shipment.AppendChild(Service);
			for(int i=0; i<this.packages.Count; i++) {
				if (packages[i].GetType() == typeof(Package)) {
					Shipment.AppendChild(((Package)this.packages[i]).GetNode(document));
				}
			}
			if (this.SaturdayDelivery || this.SaturdayPickup) {
				XmlElement ShipmentServiceOptions = document.CreateElement("ShipmentServiceOptions");
				if (this.SaturdayDelivery) {
					XmlElement SaturdayDelivery = document.CreateElement("SaturdayDelivery");
					SaturdayDelivery.InnerText = (this.SaturdayDelivery == false) ? "0" : "1";
					ShipmentServiceOptions.AppendChild(SaturdayDelivery);
				}
				if (this.SaturdayPickup) {
					XmlElement SaturdayPickup = document.CreateElement("SaturdayPickup");
					SaturdayPickup.InnerText = (this.SaturdayPickup == false) ? "0" : "1";
					ShipmentServiceOptions.AppendChild(SaturdayPickup);
				}
				Shipment.AppendChild(ShipmentServiceOptions);
			}
			RatingServiceSelectionRequest.AppendChild(Shipment);
			return "<?xml version=\"1.0\"?>" + document.InnerXml;
		}

		/// <summary>
		/// Get total charge of delivery for specified Ups Service.
		/// </summary>
		/// <param name="service">Service, requested for shipment delivery.</param>
		/// <returns>total charge of delivery</returns>
		public decimal GetRates(UpsServiceCodes service) {
			//error = null;
			String requestData = accessInfo.ConvertToXml() + ConvertToXml(RequestOption.rate, service);
			//UpsTransmitter transmitter = new UpsTransmitter();
			AsyncUpsTransmitter transmitter = new AsyncUpsTransmitter();
			String responseData = transmitter.Transmit(requestData, UpsTools.Rate);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(responseData);
			String responseStatusCode = doc.GetElementsByTagName("ResponseStatusCode")[0].InnerText;
			if (responseStatusCode != "1") {
				String errorSeverity = doc.GetElementsByTagName("ErrorSeverity")[0].InnerText;
				String errorCode = doc.GetElementsByTagName("ErrorCode")[0].InnerText;
				String errorDescription = doc.GetElementsByTagName("ErrorDescription")[0].InnerText;
				throw new UpsRateException(errorCode + "(" + errorSeverity + "): " + errorDescription);
			}
			XPathNavigator nav = doc.CreateNavigator();
			XPathExpression expr = nav.Compile("//RatingServiceSelectionResponse/RatedShipment/TotalCharges/MonetaryValue/text()");
			XPathNodeIterator iterator = nav.Select(expr);
			iterator.MoveNext();
			return decimal.Parse(iterator.Current.Value);
		}

		/// <summary>
		/// Get Service and Charge information for specified shipment.
		/// </summary>
		/// <returns></returns>
		public RateAndService[] GetRatesAndServices() {
			//error = null;
			String requestData = accessInfo.ConvertToXml() + this.ConvertToXml(RequestOption.shop, UpsServiceCodes._Standard);
			//UpsTransmitter transmitter = new UpsTransmitter();
			AsyncUpsTransmitter transmitter = new AsyncUpsTransmitter();
			String responseData = transmitter.Transmit(requestData, UpsTools.Rate);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(responseData);
			String responseStatusCode = doc.GetElementsByTagName("ResponseStatusCode")[0].InnerText;
			if (responseStatusCode != "1") {
				String errorSeverity = doc.GetElementsByTagName("ErrorSeverity")[0].InnerText;
				String errorCode = doc.GetElementsByTagName("ErrorCode")[0].InnerText;
				String errorDescription = doc.GetElementsByTagName("ErrorDescription")[0].InnerText;
				throw new UpsRateException(errorCode + "(" + errorSeverity + "): " + errorDescription);
			}
			XPathNavigator nav = doc.CreateNavigator();
			XPathExpression expService = nav.Compile("//RatingServiceSelectionResponse/RatedShipment/Service/Code/text()");
			XPathExpression expCharge = nav.Compile("//RatingServiceSelectionResponse/RatedShipment/TotalCharges/MonetaryValue/text()");
			XPathNodeIterator iServices = nav.Select(expService);
			XPathNodeIterator iCharges = nav.Select(expCharge);
			RateAndService[] ras = new RateAndService[iCharges.Count];
			for (int i=0; i<iCharges.Count; i++) {
				iCharges.MoveNext();
				iServices.MoveNext();
				ras[i] = new RateAndService((UpsServiceCodes)Int32.Parse(iServices.Current.Value), decimal.Parse(iCharges.Current.Value));
			}
			return ras;
		}

		/*
		/// <summary>
		/// If request has been failed, GetError method can 
		/// returned information about failure.
		/// </summary>
		/// <returns>error string</returns>
		public String GetErrorInfo() {
			if (error == null) return null;
			return error.ToString();
		}
		*/

	}
}
