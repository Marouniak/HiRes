using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

using HiRes.Common;
using HiRes.Common.ShippingDefs;
using HiRes.DAL;
using HiRes.ShipmentManager;
//using HiRes.ShipmentManager.FedEx;
//using HiRes.ShipmentManager.UPS;

namespace HiRes.BusinessRules {
	/*public enum ShippingResCode {
		OK = 0,
		ShipMangerError = 1,
		DBError = 2
	}*/
	/// <summary>
	/// </summary>
	public class Shipping {
		public string GetTrackInfo(PostalCarrier carrier, DateTime shipmentDate, string trackingNumber) {
			HiRes.ShipmentManager.ShipManager shipManager = new ShipManager();
			return shipManager.TrackShipment(carrier, shipmentDate, trackingNumber);
		}

		/// <summary>
		/// Sends a ship-a-package request to postal carrier web server
		/// </summary>
		/// <exception cref="ShipManagerException"/>
		ShipPackageResCode ProcessShipping(PostalCarrierInfo carrier, ShippingServiceInfo service, ContactInfo origin, ContactInfo destination, 
			PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue) {

			//throw new NotImplementedException();
			ShipManager shipManager = new ShipManager();
			try {
				ShippingResponseInfo res = shipManager.ShipPackage((PostalCarrier)Enum.Parse(typeof(PostalCarrier),carrier.Id), service.Id,	origin, destination,
					packageInfo, orderShippingDate, declaredValue);
			
				/*if (!new Packaging().AddPackagingInfo(packageInfo)) {
					return ShipPackageResCode.ERR_DB;	
				}*/
				return ShipPackageResCode.OK;
			} catch (ShipmentManagerException) {
				return ShipPackageResCode.ERR_ShipAPI;
			}
			//return new ShippingResponseInfo();
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
			using (PackagingDAL pdal = new PackagingDAL()) {
				return pdal.GetShippedPackageInfo(shippedPackageId);
			}
		}

		public ArrayList GetOrderShippedPackages(int orderId) {
			using (PackagingDAL pdal = new PackagingDAL()) {
				return pdal.GetOrderShippedPackages(orderId);
			}
		}

		public bool AddShippedPackageInfo (ref ShippedPackageInfo pi) { 
			using (PackagingDAL pdal = new PackagingDAL()) {
				return pdal.AddShippedPackageInfo(ref pi,null);
			}
		}


//		public ShipPackageResCode ShipOrderPackages (ref OrderInfo orderInfo) {
//
//			if (orderInfo==null) {
//				throw new ArgumentNullException();
//			}
//			if (orderInfo.DeliveryDetails.PickUpOrder) {
//				return ShipPackageResCode.ERR_Order_Is_For_PickUp;
//			}
//			//TODO: get from config file info if carrier has ability to order shipments online
//			if (orderInfo.DeliveryDetails.Carrier == PostalCarrier.FedEx) {
//				//TODO: ordering through an integration API goes here
//			}
//
//			orderInfo.ShippedPackages = ShippedPackageInfo.Create(orderInfo);
//
//			using (IDbTransaction transaction = DbTransactionFactory.BeginTransaction()) {
//				using (PackagingDAL pdal = new PackagingDAL()) {
//					//IDbTransaction transaction = DbTransactionFactory.BeginTransaction();
//					for(int i=0; i<boxNum; i++) {
//						ShippedPackageInfo spi = (ShippedPackageInfo)orderInfo.ShippedPackages[i];
//						if (!pdal.AddShippedPackageInfo(ref spi,transaction)) {
//							transaction.Rollback();
//							IDbConnection conn = transaction.Connection;
//							//transaction.Connection.Close();
//							return ShipPackageResCode.ERR_DB;
//						}
//					}
//					transaction.Commit();
//				}
//				IDbConnection conn1 = transaction.Connection;//transaction.Connection.Close();
//			}
//			return ShipPackageResCode.OK;
//
//		}

		public ShipPackageResCode AddOrderShippedPackagesInfo (ref OrderInfo orderInfo) {
			//throw new NotImplementedException();
			if (orderInfo==null) {
				throw new ArgumentNullException();
			}
			if (orderInfo.DeliveryDetails.PickUpOrder) {
				return ShipPackageResCode.ERR_Order_Is_For_PickUp;
			}

			ArrayList shippedPackages = ShippedPackageInfo.Create(orderInfo);
			ArrayList labels = null;
			int numShipped = 0;
			//TODO: get from config file info if carrier has ability to order shipments online
			if (orderInfo.DeliveryDetails.Carrier == PostalCarrier.FedEx) {
				
				ShipManager shipManager = new ShipManager();
				//ArrayList shippedPackages = null;
				
				decimal shipAmount = 0m;
				try {
					ContactInfo origin = new ContactInfo();
					origin.Address = ShipmentManagerConfigHandler.DefaultSenderAddress;
					origin.CompanyName = ShipmentManagerConfigHandler.CompanyName;
					origin.ContactPhone = ShipmentManagerConfigHandler.DefaultSenderContactPhone;
					CustomerInfo customer = new Customer().GetInfo(orderInfo.CustomerID,orderInfo.SiteID);

					ContactInfo destination = new ContactInfo();
					destination.FirstName = customer.FirstName;
					destination.LastName = customer.LastName;
					destination.Address = orderInfo.DeliveryDetails.ShipAddress;
					
					//shippedPackages = ShippedPackageInfo.Create(orderInfo);

					PackagingInfo packaging = (PackagingInfo)orderInfo.DeliveryDetails.Packaging.Clone();
					//HACK: Place a separate order for each package
					packaging.BoxesNumber = 1;
					labels = new ArrayList();
					for(numShipped=0;numShipped<orderInfo.DeliveryDetails.Packaging.BoxesNumber;numShipped++) {
						if (AppConfig.DemoMode) {
							((ShippedPackageInfo)shippedPackages[numShipped]).TrackingId = DemoModeHelper.GetFakedTrackingNo();
							continue;
						}

						ShippingResponseInfo res = shipManager.ShipPackage(
							orderInfo.DeliveryDetails.Carrier,
							orderInfo.DeliveryDetails.ShipMethod,
							origin, destination,
							packaging, DateTime.Now, orderInfo.Amounts.DeclaredAmount);
						
						((ShippedPackageInfo)shippedPackages[numShipped]).TrackingId = res.TrackingNumber;
						//RFU: shipAmount
						//shipAmount += res.ChargeAmount;
						labels.Add(res.LabelDataBuffer);
					}
			
					orderInfo.ShippedPackages = shippedPackages;
				} catch (ShipmentManagerException ex) {
					return ShipPackageResCode.ERR_ShipAPI;
				}
			}
			try {
				using (IDbTransaction transaction = DbTransactionFactory.BeginTransaction()) {
					using (PackagingDAL pdal = new PackagingDAL()) {
						//IDbTransaction transaction = DbTransactionFactory.BeginTransaction();
						for(int i=0; i<orderInfo.DeliveryDetails.Packaging.BoxesNumber; i++) {
							ShippedPackageInfo spi = (ShippedPackageInfo)shippedPackages[i];
							if (!pdal.AddShippedPackageInfo(ref spi,transaction)) {
								transaction.Rollback();
								return ShipPackageResCode.ERR_DB;
							}
							if (labels.Count==numShipped) {
								if (!pdal.StorePackageLabel(spi.ShippedPackageId,(byte[])labels[i],transaction)) {
									transaction.Rollback();
									return ShipPackageResCode.ERR_DB;
								} else {
									labels[i] = null;
								}
							}
						}
						transaction.Commit();
						orderInfo.ShippedPackages = shippedPackages;
					}

				}

				return ShipPackageResCode.OK;

			} catch (Exception) {
				return ShipPackageResCode.ERR_DB;
			}

			

			return ShipPackageResCode.OK;
		}

		public byte[] GetPackageLabel(int shippedPackageId) {
			using (PackagingDAL pdal = new PackagingDAL()) {
				return pdal.GetPackageLabel(shippedPackageId);
			}
		}

		public bool StorePackageLabel(int shippedPackageId,byte[] label) {
			using (PackagingDAL pdal = new PackagingDAL()) {
				return pdal.StorePackageLabel(shippedPackageId,label,null);
			}
		}
	}
}
