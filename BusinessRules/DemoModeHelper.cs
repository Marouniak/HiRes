using System;
using HiRes.Common;
using HiRes.Common.ShippingDefs;
using HiRes.PaymentFramework;
using HiRes.ShipmentManager;

namespace HiRes.BusinessRules {
	/// <summary>
	/// </summary>
	public class DemoModeHelper {

		public static PaymentTransactionResponse GetFakedTransactionResponse() {
			PaymentTransactionResponse response = new PaymentTransactionResponse();
			response.PNREF = PaymentTransaction.GenerateTxnId();;
			response.RESPMSG = "Ok";
			response.RESULT = 0;
			response.AuthCode = "120F23";
			response.AVSAddr = '1';
			response.AVSZip = '1';

			return response;
		}
		public static string GetFakedTrackingNo() {
			return "67490384923";
		}
		public static ShippingServicePrice[] GetFakedShippingPrices(PostalCarrier carrier) {
			ShippingServicePrice[] prices = null;
			switch (carrier) {
				case PostalCarrier.FedEx:
					prices = new ShippingServicePrice[4];
					for(int i=0;i<4;i++) {
						prices[i].CarrierId = PostalCarrier.FedEx.ToString("d");
					}

					prices[0].Price = 12.25m;
					prices[0].ServiceId ="1";
					prices[0].ServiceDispLabel ="USA Priority";
					prices[0].IsAvailable = true;

					prices[1].Price = 35.00m;
					prices[1].ServiceId ="5";
					prices[1].ServiceDispLabel ="USA Standard Overnight";
					prices[1].IsAvailable = true;

					prices[2].Price = 32.27m;
					prices[2].ServiceId ="20";
					prices[2].ServiceDispLabel ="USA ExpressSaver";
					prices[2].IsAvailable = true;

					prices[3].Price = 10.00m;
					prices[3].ServiceId ="3";
					prices[3].ServiceDispLabel ="USA 2day";
					prices[3].IsAvailable = true;
					break;
				case PostalCarrier.UPS:
					prices = new ShippingServicePrice[3];
					for(int i=0; i<3;i++) {
						prices[i].CarrierId = PostalCarrier.UPS.ToString("d");
					}

					prices[0].Price = 35.25m;
					prices[0].ServiceId ="0";
					prices[0].ServiceDispLabel ="Next Day Air";
					prices[0].IsAvailable = true;

					prices[1].Price = 27.00m;
					prices[1].ServiceId ="02";
					prices[1].ServiceDispLabel ="2nd Day Air";
					prices[1].IsAvailable = true;

					prices[2].Price = 10.00m;
					prices[2].ServiceId ="03";
					prices[2].ServiceDispLabel ="Ground";
					prices[2].IsAvailable = true;

					break;
			}

			return prices;
			
		}
	}
}
