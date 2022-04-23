using System;
using System.Xml;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// PackageShipment is used in response to TrackRequest.
	/// </summary>
	public struct PackageShipment {

		public String ShipmentIdentificationNumber;
		public String TrackingNumber;
		public ActivityStatusType DeliveryStatusType;
		public DateTime DeliveryTime;

		public PackageShipment(
						String ShipmentIdentificationNumber,
						String TrackingNumber,
						ActivityStatusType DeliveryStatusType,
						DateTime DeliveryTime) {
			this.ShipmentIdentificationNumber = ShipmentIdentificationNumber;
			this.TrackingNumber = TrackingNumber;
			this.DeliveryStatusType = DeliveryStatusType;
			this.DeliveryTime = DeliveryTime;
		}

		public override String ToString() {
			return "Package:" + "\r\n" +
				"TrackingNumber: " + TrackingNumber + "\r\n" +
				"ShipmentIdentificationNumber: " + ShipmentIdentificationNumber + "\r\n" +
				"DeliveryStatus: " + UpsEnumDescription.getDescription(DeliveryStatusType) + "\r\n" +
				"DeliveryTime: " + DeliveryTime + "\r\n";
		}
	}
}
