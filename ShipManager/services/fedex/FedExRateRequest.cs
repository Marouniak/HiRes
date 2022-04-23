/**
 * FILE: FedRateRequest.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: Receives the rate info on specified shipment
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		27-Apr-2002	Maxim Lysenko
 *			Initial implementation
 */
using System;

namespace HiRes.ShipmentManager.FedEx {
	/// <summary>
	/// Send this request to receive the rate of shipping from Origin to Destination
	/// </summary>
	public class FedExRateRequestInfo {
		public Address Origin;					
		public Address Destination;				
		public DateTime ShipmentDateTime;		
		public int PayType { 
			get { return 1; }					/// PayType must be "1" Bill Sender
		}
		public FedExPackageInfo PackageInfo;	
		public int PackageTotal;				/// The number of packages in shipment
		public WeightUnits WeightUnits;			/// Default value - LBS
		public decimal TotalPackageWeight;		/// Weight of package
		public FedExShippingService ServiceType;
		public decimal DeclaredValue;			/// Declared/Carriage value
		public string DeclaredValueCurrencyType {
			get { return "USD";/*(DeclaredValue > 0)?"USD":string.Empty; */}
		}
		public FedExSpecialServiceFlags Flags;	

		public FedExRateRequestInfo() {
		}
	}

	public class FedExRateResponseInfo {
		public bool DimFlag;		// 431  FDXE - Y if Dim Applies.
		public string RateScaleCode;// 1089 FDXE USD
		public string RateCurrency;	// 1090 Rate currency type returned upon successful transaction reply
		public string RateZone;		// 1092 FDXE USD
		public decimal BillWeight;	// 1402 FDXE - Bill Weight = Dim if Dim applies
		public decimal DimWeight;	// 1403 FDXE
		public decimal BaseRateAmount;	// 1416 Base rate amount returned upon successful transaction reply
		public decimal TotalSurchargeAmount;	// 1417 Total surcharge amount returned upon successful transaction reply
		public decimal TotalDiscountAmount;	// 1418 Total discount amount returned upon successful transaction reply
		public decimal NetChargeAmount;		// 1419 Net charge amount returned upon successful transaction reply
		public decimal TotalRebateAmount;	// 1420 FDXE - Total rebate amount return upon successful transaction reply
		public string OversizeFlag;	// 3010 FDXG - Returned upon successful transaction reply

		public FedExRateResponseInfo() {
		}
	}

	/// <summary>
	/// Performs Rate transaction on Fedex server
	/// </summary>
	public class FedExRateRequest {
		private FedExAPI fedExAPI;

		public FedExRateRequest(FedExAPI fedExAPI) {
			this.fedExAPI = fedExAPI;
		}

		public FedExRateResponseInfo SendRequest(FedExAccessInfo accessInfo, FedExCarierCode carrierCode, FedExRateRequestInfo requestInfo) {		
			return this.SendRequest(accessInfo, carrierCode, requestInfo, string.Empty);
		}

		public FedExRateResponseInfo SendRequest(FedExAccessInfo accessInfo, FedExCarierCode carrierCode, FedExRateRequestInfo requestInfo, string customTxnId) {
			string shipmentDateTimeStr = requestInfo.ShipmentDateTime.ToString("yyyyMMdd");
			FedExRequestString request = new FedExRequestString();
			request.Append("0", "022")
				.Append("1", customTxnId)
				.Append("10", accessInfo.AccountNumber)
				.Append("498", accessInfo.MeterNumber)
				.Append("3025", carrierCode.ToString())
				.Append("8", requestInfo.Origin.State)
				.Append("9", requestInfo.Origin.PostalCode)
				.Append("117", requestInfo.Origin.CountryCode)
				.Append("16", requestInfo.Destination.State)
				.Append("17", requestInfo.Destination.PostalCode)
				.Append("50", requestInfo.Destination.CountryCode)
				.Append("24", shipmentDateTimeStr)
				.Append("23", requestInfo.PayType.ToString())	
				.Append("57", requestInfo.PackageInfo.DimHeight)
				.Append("58", requestInfo.PackageInfo.DimWidth)
				.Append("59", requestInfo.PackageInfo.DimLength)
				.Append("68", requestInfo.DeclaredValueCurrencyType)
				.Append("75", requestInfo.WeightUnits.ToString())
				.Append("116", requestInfo.PackageTotal)
				.Append("1116", requestInfo.PackageInfo.DimUnitsStr)
				.Append("1273", requestInfo.PackageInfo.PackagingType)
				.Append("1274", requestInfo.ServiceType)
				.Append("1401", requestInfo.TotalPackageWeight.ToString("F1"))
				.Append("1415", requestInfo.DeclaredValue.ToString("F2"))
				.Append("1333", (int)requestInfo.Flags.DropOffType)

				// TODO: Add additional FedEx special service flags processing
				.Append("27", requestInfo.Flags.COD)
				.Append("440", requestInfo.Flags.ResidentialDelivery)
				.Append("1120", requestInfo.Flags.InsidePickup)
				.Append("1121", requestInfo.Flags.InsideDelivery)
				.Append("1174", requestInfo.Flags.BSO)
				.Append("1200", requestInfo.Flags.HAL)
				.Append("1266", requestInfo.Flags.SaturdayDelivery)
				.Append("1267", requestInfo.Flags.SaturdayPickup)
				.Append("1268", requestInfo.Flags.DryIce)
				.Append("1331", requestInfo.Flags.DangerousGoods)
				.Append("1266", requestInfo.Flags.SaturdayDelivery);
	
			string responseString;
			this.fedExAPI.ProcessTransaction(UniversalTransactionIdentifier.FDXE_RATE_PACKAGE, request.ToString(), out responseString);

			FedExRequestString response = new FedExRequestString(responseString);
			
			if (response.ErrorMessage != String.Empty) {
				throw new FedExTxnException(response.ErrorMessage);
			} 

			FedExRateResponseInfo rateResponseInfo = new FedExRateResponseInfo();
			rateResponseInfo.DimFlag = response.getBool("431");
			rateResponseInfo.RateScaleCode = response["1089"];
			rateResponseInfo.RateCurrency = response["1090"];
			rateResponseInfo.RateZone = response["1092"];
			rateResponseInfo.BillWeight = response.getDecimal("1402");
			rateResponseInfo.DimWeight = response.getDecimal("1403");
			rateResponseInfo.BaseRateAmount = response.getDecimal("1416");
			rateResponseInfo.TotalSurchargeAmount = response.getDecimal("1417");
			rateResponseInfo.TotalDiscountAmount = response.getDecimal("1418");
			rateResponseInfo.NetChargeAmount = response.getDecimal("1419");
			rateResponseInfo.TotalRebateAmount = response.getDecimal("1420");
			rateResponseInfo.OversizeFlag = response["3010"];

			return rateResponseInfo;
		}
	}
}
