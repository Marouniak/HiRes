/**
 * FILE: FedExCommonData.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: Common enumerations and data types
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
	/// This strucutre is needed to perform any type of FedEx transaction
	/// It have to be initialized by FedExSubscribeRequest during Application initialization, and must be available
	/// from Application variable
	/// </summary>
	public struct FedExAccessInfo {
		public const string FEDEX_ACCOUNT_NUMBER = "234915916";

		public string AccountNumber;
		public string MeterNumber;
		
		public FedExAccessInfo(string accountNumber, string meterNumber) {
			this.AccountNumber = accountNumber;
			this.MeterNumber = meterNumber;
		}
	}

	/// <summary>
	/// Contains address of location
	/// </summary>
	public struct Address {
		public string State;
		public string PostalCode; 
		public string CountryCode;

		public Address(string state, string postalCode, string countryCode) {
			this.State = state;
			this.PostalCode = postalCode;
			this.CountryCode = countryCode;
		}
	}

	/// <summary>
	/// Weight units
	/// </summary>
	public enum WeightUnits {
		LBS,	// Libras 
		KGS		// KGS
	}

	/// <summary>
	/// Dim units
	/// </summary>
	public enum DimUnits {
		I,		// Inches
		C		// Centimeters
	}


	/// <summary>
	/// FedEx services
	/// </summary>
	public enum FedExCarierCode {
		ALL,	// All carriers (FedEx Express and FedEx Ground)
		FDXE,	// FedEx Express only
		FDXG,	// FedEx Ground only
		INTL,	// Applicable to export shipments
		USD		// Applicable to US domestic shipments (US to US)
	}

	/// <summary>
	/// Specifies how the package will be delivered by one of FedEx's services
	/// <seealso cref="FedExCarierCode"/>
	/// </summary>
	/// <remarks>
	/// FDXE - FedEx Express shipping
	/// FDXG - FedEx Ground shupping
	/// </remarks>
	public enum FedExShippingService {
		// FedEx USA Domestic delivery
		USAPriority = 01,				// FDXE
		USA2day = 03,					// FDXE
		USAStandardOvernight = 05,		// FDXE
		USAFirstOvernight = 06,			// FDXE
		USAExpressSaver = 20,			// FDXE
		USAOvernightFreight = 70,		// FDXE
		USA2dayFreight = 80,			// FDXE
		USAExpressSaverFreight = 83,	// FDXE
		USAHomeDelivery = 90,			// FDXG
		USAGroundService = 92,			// FDXG
		/*
				// FedEx International delivery
				InternationalPriority = 01,		// FDXE
				InternationalEconomy = 03,		// FDXE
				InternationalFirst = 06,		// FDXE
				InternationalGroundService = 92	// FDXG
		*/
	}

	/// <summary>
	/// Available FedEx shipping packages
	/// </summary>
	public enum FedExPackagingType {
		// FedEx Express shipping packaging types are:
		OtherPackaging = 01,
		CustomPackaging = 01,
		FedExPak = 02,
		FedExBox = 03,
		FedExTube = 04,
		FedExEnvelope = 06,
		FedEx10kgBox = 15,				// International only
		FedEx25kgBox = 25				// International only
	}

	/// <summary>
	/// Package info
	/// </summary>
	public struct FedExPackageInfo {
		private decimal _DimWidth;
		private decimal _DimHeight;
		private decimal _DimLength;
		private DimUnits _DimUnits;

		public decimal DimWidth {
			get {return (this.PackagingType == FedExPackagingType.CustomPackaging)?_DimWidth:0; }
			set {_DimWidth = value; }
		}
		public decimal DimHeight {
			get {return (this.PackagingType == FedExPackagingType.CustomPackaging)?_DimHeight:0; }
			set {_DimHeight = value; }
		}
		public decimal DimLength {
			get {return (this.PackagingType == FedExPackagingType.CustomPackaging)?_DimLength:0; }
			set {_DimLength = value; }
		}
		public DimUnits DimUnits {			
			get {return (this.PackagingType == FedExPackagingType.CustomPackaging)?_DimUnits:0; }
			set {_DimUnits = value; }
		}
		public string DimUnitsStr {			
			get {return (this.PackagingType == FedExPackagingType.CustomPackaging)?_DimUnits.ToString():string.Empty; }
		}
		public FedExPackagingType PackagingType;
	}

	public enum FedExLabelPrinterType {
		PlainPaper = 01,
		EltronOrion = 02,
		EltronEclipse = 03
	}

	public enum FedExLabelMediaType {
		ThermalWithDocTab = 03,
		ThermalWithoutDocTab = 04,
		PlainPaperPNG = 05
	}

	/// <summary>
	/// Label info
	/// </summary>
	public struct FedExLabelInfo {
		private int _LabelType;
		private FedExLabelPrinterType _PrinterType;
		private FedExLabelMediaType _MediaType;

		public int LabelType {
			get {return 1; }
		}

		public FedExLabelPrinterType PrinterType {
			get {return this._PrinterType; }
			set {this._PrinterType = value; }
		}

		public FedExLabelMediaType MediaType {
			get {return this._MediaType; }
			set {this._MediaType = value; }
		}
	}


	/// <summary>
	/// </summary>
	/// <remarks>
	/// FDXE shipment drop off types
	/// </remarks>
	public enum FedExShipmentDropOffType {
		RegularPickup = 1,
		RequestCourier = 2,
		DropBox = 3,
		DropAtBSC = 4,
		DropAtStation = 5
	}

	/// <summary>
	/// FedEx Cash On Delivery (COD) types
	/// </summary>
	public enum FedExCODCollectionType {
		AnyPaymentType = 1,
		GuaranteedFunds = 2,
		Currency = 3
	}

	/// <summary>
	/// FedEx home delivery types
	/// </summary>
	public enum FedExHomeDeliveryType {
		DateCertain = 1,
		Evening= 2,
		Appointment = 3
	}	

	/// <summary>
	/// FedEx special services flags
	/// </summary>
	/// <remarks>
	/// FDXE - FedEx Express shipping
	/// FDXG - FedEx Ground shupping
	/// USD - USA Domestic delivery
	/// </remarks>
	public struct FedExSpecialServiceFlags {
		public bool COD;				// 27, FDXE, FDXG - shipment is COD or ECOD (Cash On Delivery)
		public bool ResidentialDelivery;// 440, USD - shipment is Residential Delivery. If Recipient Address is in a rural area (defined by table lookup), additional charge will be applied. This field is not applicable to the FedEx Home Delivery service.
		public bool InsidePickup;		// 1120, FDXE - shipment is Inside Pickup
		public bool InsideDelivery;		// 1121, FDXE - inside Delivery
		public bool BSO;				// 1174, FDXE - shipment is BSO (Broker Select Option)
		public bool HAL;				// 1200, FDXE - shipment is HAL (Hold at Location)
		public bool SaturdayDelivery;	// 1266, FDXE - shipment is targeted for Saturday delivery
		public bool SaturdayPickup;		// 1267, FDXE - shipment is targeted for Saturday pickup
		public bool DryIce;				// 1268, FDXE - shipment contains dry ice
		public bool DangerousGoods;		// 1331, FDXE - shipment contains dangerous goods (accessible or inaccessible)
		public FedExShipmentDropOffType DropOffType;	//1333 FDXE - shipment drop off type
		public float CODCollectAmount;	// 1409, FDXG - COD collect amount required if the COD or ECOD Flag is set to Y
		public FedExCODCollectionType CODCollectionType;	// 3000, FDXG - COD	Collection Type required field if the COD or ECOD Flag is set to Y
		public bool Autopod;			// 3008, FDXG - shipment is targeted for Autopod. Must have a contractual agreement in order to receive Autopod
		public bool NonstandardContainer;	// 3018, FDXG - Y if nonstandard container
		public bool HomeSignatureDelivery;	// 3019, FDXG USD - Y if shipment is for home delivery and a signature is required
		public FedExHomeDeliveryType HomeDeliveryType;	// 3020, FDXG USD - required field if shipping FedEx Home Delivery
	}
}
