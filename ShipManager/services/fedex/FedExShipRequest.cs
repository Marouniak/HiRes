/**
 * FILE: FedShipRequest.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: Receives the rate info on specified shipment
 * 
 * LEGAL: (c)2003 Eurosoft International Inc.
 * 
 * Revision history:
 *		20-May-2003	Maxim I. Lysenko
 *			Initial implementation
 */
using System;
using System.Globalization;

namespace HiRes.ShipmentManager.FedEx {
	/// <summary>
	/// Send this request to receive the rate of shipping from Origin to Destination
	/// </summary>
	public class FedExShipRequestInfo {
		public FedExContactInfo Origin;					
		public FedExContactInfo Destination;				
		public DateTime ShipmentDateTime;		
		public int PayType { 
			get { return 1; }					/// For HiRes the PayType is "1" Bill Sender
		}
		public FedExLabelInfo LabelInfo;		/// The printer label info
		public FedExPackageInfo PackageInfo;	
		public string ReferenceInforamtion;		/// This field allows customer-defined notes to print on the shipping label.
		public int PackageTotal;				/// The number of packages in shipment
		public WeightUnits WeightUnits;			/// Default value - LBS
		public decimal TotalPackageWeight;		/// Weight of package
		public FedExShippingService ServiceType;
		public decimal DeclaredValue;			/// Declared/Carriage value
		public string DeclaredValueCurrencyType {
			get { return "USD";/*(DeclaredValue > 0)?"USD":string.Empty; */}
		}
		public FedExSpecialServiceFlags Flags;	

		public FedExShipRequestInfo() {
		}
	}

	public class FedExShipResponseInfo {
		public string TrackingNumber;	// 29 - Tracking Number
		
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

		public byte[] LabelDataBuffer; // 188 - Label Buffer Data stream

		public FedExShipResponseInfo() {
		}
	}

	/// <summary>
	/// Performs Ship a package request on Fedex server
	/// </summary>
	public class FedExShipRequest {
		private FedExAPI fedExAPI;

		protected byte[] DecodeLabelData(string labelData) {
			int arrayLength = labelData.Length;
			for (int i = 0; i < labelData.Length; i++) {
				if (labelData[i] == '%') {
					arrayLength -= 2;		// in input sequence some ASCII characters may be replaced by their codes %XX
					// -2 because %XX (3 bytes) sequence will be replaced by C (1 byte) character where C is the ASCII character with the hex code %XX
				}
			}

			byte[] decodedBuf = new byte[arrayLength];
			int bufPosition = 0;
			
			for (int i = 0; i < labelData.Length; i++) {
				byte b = (byte)labelData[i];
				if (b == '%') {
					string hexCode = labelData.Substring(i + 1, 2);
					b = Byte.Parse(hexCode, NumberStyles.HexNumber);
					i += 2;
				}

				decodedBuf[bufPosition] = b;
				bufPosition++;
			}

			return decodedBuf;
		}

		public FedExShipRequest(FedExAPI fedExAPI) {
			this.fedExAPI = fedExAPI;
		}

		public FedExShipResponseInfo SendRequest(FedExAccessInfo accessInfo, FedExCarierCode carrierCode, FedExShipRequestInfo requestInfo) {		
			return this.SendRequest(accessInfo, carrierCode, requestInfo, string.Empty);
		}

		public FedExShipResponseInfo SendRequest(FedExAccessInfo accessInfo, FedExCarierCode carrierCode, FedExShipRequestInfo requestInfo, string customTxnId) {
			string shipmentDateTimeStr = requestInfo.ShipmentDateTime.ToString("yyyyMMdd");
			FedExRequestString request = new FedExRequestString();
			request.Append("0", "021")					// Ship-a-package request
				.Append("1", customTxnId)
				.Append("10", accessInfo.AccountNumber)
				.Append("498", accessInfo.MeterNumber)
				.Append("3025", carrierCode.ToString())
				
				// Origin
				.Append("4", requestInfo.Origin.CompanyName)
				.Append("5", requestInfo.Origin.AddressLine1)
				.Append("6", requestInfo.Origin.AddressLine2)
				.Append("7", requestInfo.Origin.City)
				.Append("8", requestInfo.Origin.State)
				.Append("9", requestInfo.Origin.PostalCode)
				.Append("32", requestInfo.Origin.ContactName)
				.Append("117", requestInfo.Origin.CountryCode)
				.Append("183", requestInfo.Origin.PhoneNumber)
				
				// Destination
				.Append("11", requestInfo.Origin.CompanyName)
				.Append("13", requestInfo.Origin.AddressLine1)
				.Append("14", requestInfo.Origin.AddressLine2)
				.Append("15", requestInfo.Origin.City)
				.Append("16", requestInfo.Origin.State)
				.Append("17", requestInfo.Origin.PostalCode)
				.Append("12", requestInfo.Origin.ContactName)
				.Append("50", requestInfo.Origin.CountryCode)
				.Append("18", requestInfo.Origin.PhoneNumber)
				
				// Shipment date/time stamp
				.Append("24", shipmentDateTimeStr)
				//TODO: It is possible to shedule the shipment date on the future date (less then 10 days in advance)
				//.Append("119", futureDate)
				// Payment
				.Append("23", requestInfo.PayType.ToString())	
				.Append("68", requestInfo.DeclaredValueCurrencyType)
				// Multiple Piece Shipping
				.Append("116", requestInfo.PackageTotal)
				.Append("1400", requestInfo.TotalPackageWeight.ToString("F1"))
				// * 1117 Package Sequence - Required field for multi-piece shipping via the Express
				// Server if Package Total (#116) value is greater than 1.
				// * 1123 Master Tracking Number - Required field for multi-piece shipping via 
				// the Express Server if Package Sequence (#1117) value is greater than 1.
				// * 1124 Master Form ID FDXE ALL Required field for multi-piece shipping via the Express
				// Server if Package Sequence (#1117) value is greater than 1.
				
				// Shipment/ Package Information
				.Append("25", requestInfo.ReferenceInforamtion)
				.Append("57", requestInfo.PackageInfo.DimHeight)
				.Append("58", requestInfo.PackageInfo.DimWidth)
				.Append("59", requestInfo.PackageInfo.DimLength)
				.Append("75", requestInfo.WeightUnits.ToString())
				.Append("1116", requestInfo.PackageInfo.DimUnitsStr)
				.Append("1273", requestInfo.PackageInfo.PackagingType)
				.Append("1274", requestInfo.ServiceType)
				.Append("1401", requestInfo.TotalPackageWeight.ToString("F1"))
				.Append("1415", requestInfo.DeclaredValue.ToString("F2"))
				.Append("1333", (int)requestInfo.Flags.DropOffType)

				// Additional FedEx special service flags processing
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
				.Append("1266", requestInfo.Flags.SaturdayDelivery)

				// Forms/Printers
				.Append("1368", (int)requestInfo.LabelInfo.LabelType)	// Label Type: only 1 = standard is currently allowed)
				.Append("1369", (int)requestInfo.LabelInfo.PrinterType)	// Label Printer Type: 1 = plain paper (such as laser), 2 = Eltron Orion or 3 = Eltron Eclipse
				.Append("1370", (int)requestInfo.LabelInfo.MediaType);	// Label Media Type: 3 = 4x6 and 3/4 thermal with doc-tab, 4 = 4x6 and 3/4 thermal without doc-tab and 5 = plain paper PNG

			string responseString = "";
			this.fedExAPI.ProcessTransaction(UniversalTransactionIdentifier.FDXE_SHIP_PACKAGE, request.ToString(), out responseString);

			FedExRequestString response = new FedExRequestString(responseString);
			
			if (response.ErrorMessage != String.Empty) {
				throw new FedExTxnException(response.ErrorMessage);
			} 

			FedExShipResponseInfo rateResponseInfo = new FedExShipResponseInfo();
			rateResponseInfo.TrackingNumber = response["29"];
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

			rateResponseInfo.LabelDataBuffer = DecodeLabelData(response["188"]);

			return rateResponseInfo;
		}
	}
}
