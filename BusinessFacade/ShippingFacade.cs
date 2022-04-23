using System;
using System.Collections;

using HiRes.Common;
using HiRes.Common.ShippingDefs;

using HiRes.BusinessRules;
using HiRes.ShipmentManager;
using HiRes.SystemFramework;
using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessFacade {
	/// <summary>
	/// Summary description for ShippingFacade.
	/// </summary>
	public class ShippingFacade {

		public ShippingServicePrice[] CalculateShippingPrices(PostalCarrierInfo carrier, AddressInfo origin, 
			AddressInfo destination, PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue) {
			//PostalCarrier postalCarrier = PostalCarrier.FedEx;
			PostalCarrier postalCarrier = (PostalCarrier)Enum.Parse(typeof(PostalCarrier),carrier.Id);
			
			// return faked prices if operates in Demo mode
			if (AppConfig.DemoMode) {
				return DemoModeHelper.GetFakedShippingPrices(postalCarrier);
			}
			

			// ups remark:
			// if u want to get prices for all shiping services, u can only once call method
			// HiRes.ShipmentManager.UPS.RatingServiceSelectionRequest.GetRatesAndServices()

			ShipManager shipManager = new ShipManager();
			int itemsNum = carrier.AvailableServices.Count;
			ShippingServicePrice[] prices = new ShippingServicePrice[itemsNum];
			for(int i=0; i<itemsNum; i++) {
				prices[i].CarrierId = carrier.Id;
				ShippingServiceInfo service = (ShippingServiceInfo)carrier.AvailableServices[i];
					
				prices[i].ServiceId = service.Id;
				prices[i].ServiceDispLabel = service.DispLabel;
				try {
					//FIXME : change shipManager.RatePackage the way it accepts decimal amount
					prices[i].Price = shipManager.RatePackage(
						postalCarrier,
						service.Id,
						origin,
						destination,
						packageInfo,
						orderShippingDate,
						declaredValue);
					prices[i].IsAvailable = true;
				} catch (Exception ex) {
					prices[i].Price = -1.00m;
					prices[i].IsAvailable = false;
				}
			}
			return prices;		
		}

		public string GetTrackInfo(PostalCarrier carrier, DateTime shipmentDate, string trackingNumber) {
			HiRes.ShipmentManager.ShipManager shipManager = new ShipManager();
			return shipManager.TrackShipment(carrier, shipmentDate, trackingNumber);
		}

		
		 
		/// <summary>
		/// Sends a ship-a-package request to postal carrier web server
		/// </summary>
		/// <exception cref="ShipManagerException"/>
		[Obsolete("",true)]
		ShipPackageResCode PlaceOrderShippings(ref OrderInfo orderInfo) {

			if (orderInfo.DeliveryDetails.Carrier == PostalCarrier.FedEx) {
				
			} else {
				if (orderInfo.DeliveryDetails.Carrier == PostalCarrier.UPS) {
					new Shipping().AddOrderShippedPackagesInfo (ref orderInfo);
				}
			}

			throw new NotImplementedException();
			/*ShipManager shipManager = new ShipManager();
			try {
				ShippingResponseInfo res = shipManager.ShipPackage((PostalCarrier)Enum.Parse(typeof(PostalCarrier),carrier.Id), service.Id,	origin, destination,
					packageInfo, orderShippingDate, (double)declaredValue);
			
				new Packaging().AddPackagingInfo(packageInfo);
				return true;
			} catch (ShipmentManagerException ex) {
				return false;
			}
			//return new ShippingResponseInfo();*/
		}


		public byte[] GetShippingLabel(PostalCarrier carrier, String trackingNumber) {
			throw new NotImplementedException();
			// Maxim codeblock
			// ...
		}

		public void OrderShipping() {
			throw new NotImplementedException();
			// Maxim codeblock
			// ...
		}

		public ShippedPackageInfo GetShippedPackageInfo(int shippedPackageId) {
			return new Shipping().GetShippedPackageInfo(shippedPackageId);
		}

		public ArrayList GetOrderShippedPackages(int orderId) {
			return new Shipping().GetOrderShippedPackages(orderId);
		}

		public bool AddShippedPackageInfo (ref ShippedPackageInfo pi) { 
			return new Shipping().AddShippedPackageInfo(ref pi);
		}

		public ShipPackageResCode AddOrderShippedPackagesInfo (ref OrderInfo orderInfo) {
			return new Shipping().AddOrderShippedPackagesInfo (ref orderInfo);
		}

		public byte[] GetPackageLabel(int shippedPackageId) {
			return new Shipping().GetPackageLabel(shippedPackageId);
		}

		public bool StorePackageLabel(int shippedPackageId,byte[] label) {
			return new Shipping().StorePackageLabel(shippedPackageId,label);
		}

	}

}
