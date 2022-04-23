using System;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// RateAndService class keep one item of RatingServiceSelection 
	/// response (service and total charge of shipment delivery with 
	/// help of this service).
	/// </summary>
	public struct RateAndService {
		
		public UpsServiceCodes Service;
		public decimal Charge;

		public RateAndService(UpsServiceCodes Service, decimal Charge) {
			this.Service = Service;
			this.Charge = Charge;
		}

	}
}
