using System;
using System.Collections;

namespace HiRes.Common {

	public enum ShipPackageResCode {
		OK = 0,
		ERR_Order_Is_For_PickUp = 1,
		ERR_ShipAPI = 2,
		ERR_DB = 3
	}
	/// <summary>
	/// </summary>
	public class ShippedPackageInfo {

		public int ShippedPackageId;
		public int OrderId;
		public PostalCarrier Carrier;

		private string _shipMethod;
		public decimal Width;
		public decimal Height;
		public decimal Length;
		public decimal Weight;
		
		public string TrackingId;

		private String _carrierPackageTypeId;

		public String CarrierPackageTypeId {
			get { return _carrierPackageTypeId; }
			set { _carrierPackageTypeId = value; }
		}
		public bool IsCarrierPackaging {
			get {
				return (_carrierPackageTypeId!=null);
			}
		}
		public String ShipMethod {
			get { return _shipMethod; }
			set { _shipMethod = value; }
		}

		public ShippedPackageInfo() {
			ShippedPackageId = PersistentBusinessEntity.ID_EMPTY;
			TrackingId = string.Empty;
			OrderId = PersistentBusinessEntity.ID_EMPTY;
		}

		public static ShippedPackageInfo Create(PackagingInfo pi) {
			ShippedPackageInfo spi = new ShippedPackageInfo();
			spi._carrierPackageTypeId = pi.CarrierPackageTypeId;
			//+spi._shipMethod = ;
			//+spi.Carrier;
			spi.Height = pi.Height;
			spi.Length = pi.Length;
			spi.Weight = pi.Weight;
			spi.Width = pi.Width;
			return spi;
			//+spi.OrderId;
			//+spi.ShippedPackageId;
			//+spi.TrackingId;
		}

		public static ArrayList Create(OrderInfo orderInfo) {
			ArrayList res = new ArrayList();
			int boxNum = orderInfo.DeliveryDetails.Packaging.BoxesNumber;
			for(int i=0; i<boxNum; i++) {
				ShippedPackageInfo spi = ShippedPackageInfo.Create(orderInfo.DeliveryDetails.Packaging);
				spi.OrderId = orderInfo.OrderId;
				spi.Carrier = orderInfo.DeliveryDetails.Carrier;
				spi.ShipMethod = orderInfo.DeliveryDetails.ShipMethod;

				res.Add(spi);
			}
			return res;
		}

	}
}
