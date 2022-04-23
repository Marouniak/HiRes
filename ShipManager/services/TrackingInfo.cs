using System;

using System.Xml;
using System.Xml.XPath;

namespace HiRes.ShipmentManager.services
{
	/// <summary>
	/// Summary description for TrackingInfo.
	/// </summary>
	public class TrackingInfo {

		public string TrackingNumber;
		public string SignedFor;
		public string ActivityStatus;
		public DateTime ActivityTime;
		public string ActivityCity;
		public string ActivityState;
		public string ActivityCountryCode;
		
		public TrackingInfo() {
		}

		public string ToXmlString() {
			XmlDocument document = new XmlDocument();
			document.LoadXml("<TrackingInfo/>");
			XmlElement TrackingInfo = document.DocumentElement;

			XmlElement xeTrackingNumber = document.CreateElement("TrackingNumber");
			XmlElement xeSignedFor = document.CreateElement("SignedFor");
			XmlElement xeActivityStatus = document.CreateElement("ActivityStatus");
			XmlElement xeActivityDate = document.CreateElement("ActivityDate");
			XmlElement xeActivityTime = document.CreateElement("ActivityTime");
			XmlElement xeActivityCity = document.CreateElement("ActivityCity");
			XmlElement xeActivityState = document.CreateElement("ActivityState");
			XmlElement xeActivityCountryCode = document.CreateElement("ActivityCountryCode");

			xeTrackingNumber.InnerText = TrackingNumber;
			xeSignedFor.InnerText = SignedFor;
			xeActivityStatus.InnerText = ActivityStatus;
			xeActivityTime.InnerText = ActivityTime.ToString();
			xeActivityCity.InnerText = ActivityCity;
			xeActivityState.InnerText = ActivityState;
			xeActivityCountryCode.InnerText = ActivityCountryCode;

			TrackingInfo.AppendChild(xeTrackingNumber);
			TrackingInfo.AppendChild(xeSignedFor);
			TrackingInfo.AppendChild(xeActivityStatus);
			TrackingInfo.AppendChild(xeActivityTime);
			TrackingInfo.AppendChild(xeActivityCity);
			TrackingInfo.AppendChild(xeActivityState);
			TrackingInfo.AppendChild(xeActivityCountryCode);

			return "<?xml version=\"1.0\"?>" + document.InnerXml;
		}
	}

}
