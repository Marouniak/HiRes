using System;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;
using System.Threading;

using HiRes.ShipmentManager.services;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// TrackRequest used for tracking package or shipment, 
	/// identified by Tracking or Shipment Number.
	/// For additional info read tracking.pdf
	/// </summary>
	public class TrackRequest {

		private AccessRequest accessRequest;

		public TrackRequest(AccessRequest accessRequest) {
			this.accessRequest = accessRequest;
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		/// <summary>
		/// Get info about package delivery. 
		/// Package represented by Tracking Number.
		/// </summary>
		/// <param name="trackingNumber"></param>
		/// <returns></returns>
		public TrackingInfo TrackPackage(String trackingNumber) {
			String requestData = this.accessRequest.ConvertToXml() + ConvertToXml(trackingNumber, null);
			AsyncUpsTransmitter transmitter = new AsyncUpsTransmitter();
			String responseData = transmitter.Transmit(requestData, UpsTools.Track);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(responseData);
			String responseStatusCode = doc.GetElementsByTagName("ResponseStatusCode")[0].InnerText;
			if (responseStatusCode != "1") {
				String errorSeverity = doc.GetElementsByTagName("ErrorSeverity")[0].InnerText;
				String errorCode = doc.GetElementsByTagName("ErrorCode")[0].InnerText;
				String errorDescription = doc.GetElementsByTagName("ErrorDescription")[0].InnerText;
				throw new UpsTrackException(errorCode + "(" + errorSeverity + "): " + errorDescription);
			}
			TrackingInfo result = new TrackingInfo();
			result.TrackingNumber = doc.GetElementsByTagName("TrackingNumber")[0].InnerText;
			XPathNavigator nav = doc.CreateNavigator();
			XPathExpression expStatusTypes = nav.Compile("//TrackResponse/Shipment/Package/Activity/Status/StatusType/Code/text()");
			XPathNodeIterator iStatusTypes = nav.Select(expStatusTypes);
			XPathExpression expDate = nav.Compile("//TrackResponse/Shipment/Package/Activity/Date/text()");
			XPathNodeIterator iDate = nav.Select(expDate);
			XPathExpression expTime = nav.Compile("//TrackResponse/Shipment/Package/Activity/Time/text()");
			XPathNodeIterator iTime = nav.Select(expTime);
			XPathExpression expCity = nav.Compile("//TrackResponse/Shipment/Package/Activity/ActivityLocation/Address/City/text()");
			XPathNodeIterator iCity = nav.Select(expCity);
			XPathExpression expStateProvinceCode = nav.Compile("//TrackResponse/Shipment/Package/Activity/ActivityLocation/Address/StateProvinceCode/text()");
			XPathNodeIterator iStateProvinceCode = nav.Select(expStateProvinceCode);
			XPathExpression expCountryCode = nav.Compile("//TrackResponse/Shipment/Package/Activity/ActivityLocation/Address/CountryCode/text()");
			XPathNodeIterator iCountryCode = nav.Select(expCountryCode);
			XPathExpression expSignedForByName = nav.Compile("//TrackResponse/Shipment/Package/Activity/ActivityLocation/SignedForByName/text()");
			XPathNodeIterator iSignedForByName = nav.Select(expSignedForByName);

			iStatusTypes.MoveNext();
			iDate.MoveNext();
			iTime.MoveNext();
			iCity.MoveNext();
			iStateProvinceCode.MoveNext();
			iCountryCode.MoveNext();
			if (iSignedForByName.Count > 0)
				iSignedForByName.MoveNext();
			
			ActivityStatusType ast = (ActivityStatusType)Enum.Parse(typeof(ActivityStatusType), iStatusTypes.Current.Value, true);
			result.ActivityStatus = UpsEnumDescription.getDescription(ast);

			String date = iDate.Current.Value;
			String time = iTime.Current.Value;
			result.ActivityTime = new DateTime(
				Int32.Parse(date.Substring(0, 4)), 
				Int32.Parse(date.Substring(4, 2)), 
				Int32.Parse(date.Substring(6, 2)), 
				Int32.Parse(time.Substring(0, 2)), 
				Int32.Parse(time.Substring(2, 2)), 
				Int32.Parse(time.Substring(4, 2)));

			result.ActivityCity = iCity.Current.Value;
			result.ActivityState = iStateProvinceCode.Current.Value;
			result.ActivityCountryCode = iCountryCode.Current.Value;
			if (iSignedForByName.Count > 0)
				result.SignedFor = iSignedForByName.Current.Value;
			return result;
		}
/*
		public PackageShipment TrackPackage(String trackingNumber) {
			//error = null;
			String requestData = this.accessRequest.ConvertToXml() + ConvertToXml(trackingNumber, null);
			//UpsTransmitter transmitter = new UpsTransmitter();
			AsyncUpsTransmitter transmitter = new AsyncUpsTransmitter();
			String responseData = transmitter.Transmit(requestData, UpsTools.Track);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(responseData);
			String responseStatusCode = doc.GetElementsByTagName("ResponseStatusCode")[0].InnerText;
			if (responseStatusCode != "1") {
				String errorSeverity = doc.GetElementsByTagName("ErrorSeverity")[0].InnerText;
				String errorCode = doc.GetElementsByTagName("ErrorCode")[0].InnerText;
				String errorDescription = doc.GetElementsByTagName("ErrorDescription")[0].InnerText;
				throw new UpsTrackException(errorCode + "(" + errorSeverity + "): " + errorDescription);
			}
			PackageShipment result;
			result.ShipmentIdentificationNumber = doc.GetElementsByTagName("ShipmentIdentificationNumber")[0].InnerText;
			result.TrackingNumber = doc.GetElementsByTagName("TrackingNumber")[0].InnerText;
			XPathNavigator nav = doc.CreateNavigator();
			XPathExpression expStatusTypes = nav.Compile("//TrackResponse/Shipment/Package/Activity/Status/StatusType/Code/text()");
			XPathNodeIterator iStatusTypes = nav.Select(expStatusTypes);
			XPathExpression expDate = nav.Compile("//TrackResponse/Shipment/Package/Activity/Date/text()");
			XPathNodeIterator iDate = nav.Select(expDate);
			XPathExpression expTime = nav.Compile("//TrackResponse/Shipment/Package/Activity/Time/text()");
			XPathNodeIterator iTime = nav.Select(expTime);
			iStatusTypes.MoveNext();
			iDate.MoveNext();
			iTime.MoveNext();
			result.DeliveryStatusType = (ActivityStatusType)Enum.Parse(typeof(ActivityStatusType), iStatusTypes.Current.Value, true);
			String date = iDate.Current.Value;
			String time = iTime.Current.Value;
			result.DeliveryTime = new DateTime(
				Int32.Parse(date.Substring(0, 4)), 
				Int32.Parse(date.Substring(4, 2)), 
				Int32.Parse(date.Substring(6, 2)), 
				Int32.Parse(time.Substring(0, 2)), 
				Int32.Parse(time.Substring(2, 2)), 
				Int32.Parse(time.Substring(4, 2)));
			return result;
		}
*/

		/// <summary>
		/// Get info about shipment, represented by Shipment Number.
		/// </summary>
		/// <param name="shipmentNumber"></param>
		/// <returns>Array of packages in shipment. 
		/// I think, that shipment delivery is successful, 
		/// if all packages has deliveryStatus=DELIVERED.</returns>
/*
		public PackageShipment[] TrackShipment(String shipmentNumber) {
			//error = null;
			String requestData = this.accessRequest.ConvertToXml() + ConvertToXml(null, shipmentNumber);
			//UpsTransmitter transmitter = new UpsTransmitter();
			AsyncUpsTransmitter transmitter = new AsyncUpsTransmitter();
			String responseData = transmitter.Transmit(requestData, UpsTools.Track);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(responseData);
			String responseStatusCode = doc.GetElementsByTagName("ResponseStatusCode")[0].InnerText;
			if (responseStatusCode != "1") {
				String errorSeverity = doc.GetElementsByTagName("ErrorSeverity")[0].InnerText;
				String errorCode = doc.GetElementsByTagName("ErrorCode")[0].InnerText;
				String errorDescription = doc.GetElementsByTagName("ErrorDescription")[0].InnerText;
				throw new UpsTrackException(errorCode + "(" + errorSeverity + "): " + errorDescription);
			}
			String shipmentIdentificationNumber = doc.GetElementsByTagName("ShipmentIdentificationNumber")[0].InnerText;
			XPathNavigator nav = doc.CreateNavigator();
			XPathExpression expTrackNum = nav.Compile("//TrackResponse/Shipment/Package/TrackingNumber/text()");
			XPathNodeIterator iTrackNum = nav.Select(expTrackNum);
			XPathExpression expStatusTypes = nav.Compile("//TrackResponse/Shipment/Package/Activity/Status/StatusType/Code/text()");
			XPathNodeIterator iStatusTypes = nav.Select(expStatusTypes);
			XPathExpression expDate = nav.Compile("//TrackResponse/Shipment/Package/Activity/Date/text()");
			XPathNodeIterator iDate = nav.Select(expDate);
			XPathExpression expTime = nav.Compile("//TrackResponse/Shipment/Package/Activity/Time/text()");
			XPathNodeIterator iTime = nav.Select(expTime);
			PackageShipment[] pss = new PackageShipment[iTrackNum.Count];
			for (int i=0; i<iTrackNum.Count; i++) {
				iTrackNum.MoveNext();
				iStatusTypes.MoveNext();
				iDate.MoveNext();
				iTime.MoveNext();
				pss[i].ShipmentIdentificationNumber = shipmentIdentificationNumber;
				pss[i].TrackingNumber = iTrackNum.Current.Value;
				pss[i].DeliveryStatusType = (ActivityStatusType)Enum.Parse(typeof(ActivityStatusType), iStatusTypes.Current.Value, true);
				String date = iDate.Current.Value;
				String time = iTime.Current.Value;
				pss[i].DeliveryTime = new DateTime(
					Int32.Parse(date.Substring(0, 4)), 
					Int32.Parse(date.Substring(4, 2)), 
					Int32.Parse(date.Substring(6, 2)), 
					Int32.Parse(time.Substring(0, 2)), 
					Int32.Parse(time.Substring(2, 2)), 
					Int32.Parse(time.Substring(4, 2)));
			}
			return pss;
		}
*/

		/// <summary>
		/// Transform Request structure in xml string.
		/// </summary>
		/// <param name="trackingNumber"></param>
		/// <param name="shipmentNumber"></param>
		/// <returns></returns>
		private String ConvertToXml(String trackingNumber, String shipmentNumber) {
			XmlDocument document = new XmlDocument();
			document.LoadXml("<TrackRequest/>");
			XmlElement TrackRequest = document.DocumentElement;
			XmlElement Request = document.CreateElement("Request");
			XmlElement RequestAction = document.CreateElement("RequestAction");
			XmlElement RequestOption = document.CreateElement("RequestOption");
			RequestAction.InnerText = "Track";
			RequestOption.InnerText = "none";
			Request.AppendChild(RequestAction);
			Request.AppendChild(RequestOption);
			TrackRequest.AppendChild(Request);
			if (trackingNumber != null) {
				XmlElement TrackingNumber = document.CreateElement("TrackingNumber");
				TrackingNumber.InnerText = trackingNumber;
				TrackRequest.AppendChild(TrackingNumber);
			}
			if (shipmentNumber != null) {
				XmlElement ShipmentIdentificationNumber = document.CreateElement("ShipmentIdentificationNumber");
				ShipmentIdentificationNumber.InnerText = shipmentNumber;
				TrackRequest.AppendChild(ShipmentIdentificationNumber);
			}
			return "<?xml version=\"1.0\"?>" + document.InnerXml;
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
