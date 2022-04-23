using System;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// rates_service.pdf, Appendix C, Table 3: Pickup Types
	/// </summary>
	public enum PickupTypes {
		_Daily_Pickup = 1,
		_Customer_Counter = 3,
		_OneTimePickup = 6,
		_On_Call_Air = 7,
		_Letter_Center = 19,
		_Air_Service_Center = 20
	}

	/// <summary>
	/// rates_service.pdf, Appendix C, Table 4: UPS Service Codes
	/// </summary>
	public enum UpsServiceCodes {
		_Next_Day_Air = 01,
		_2nd_Day_Air = 02,
		_Ground = 03,
		_Worldwide_Express = 07,
		_Worldwide_Expedited = 08,
		_Standard = 11,
		_3Day_Select = 12,
		_Next_Day_Air_Saver = 13,
		_Next_Day_Air_Early_AM = 14,
		_Worldwide_Express_Plus = 54,
		_2nd_Day_Air_AM = 59,
		_Express_Saver = 65
	}

	/// <summary>
	/// rates_service.pdf, Appendix C, Table 5: Package Type Codes
	/// </summary>
	public enum PackageTypeCodes {
		_Unknown = 00,
		_UPS_letter = 01,
		_Package = 02,
		_UPS_Tube = 03, 
		_UPS_Pak = 04,
		_UPS_Express_Box = 21,
		_UPS_25KG_Box = 24,
		_UPS_10KG_Box = 25
	}

	/// <summary>
	/// rates_service.pdf, Table 1: Rates & Service Selection Request Input
	/// </summary>
	public class UnitOfMeasurement {
		public const String IN = "IN";
		public const String CM = "CM";
		public const String LBS = "LBS";
		public const String KGS = "KGS";
	}

	/// <summary>
	/// RequestOption define two constants, rate and shop 
	/// used in RatingServiceSelectionRequest. If RequestOption=rate, 
	/// than response will contains TotalCharge for one Service, otherwise
	/// charges for all available services will be included in response.
	/// 
	/// rates_service.pdf Table 1: Rates & Service Selection Request Input
	/// </summary>
	enum RequestOption {
		rate, shop
	}

	/// <summary>
	/// ActivityStatusType is used in TrackRequest.
	/// This value defines Package Delivery Status.
	/// </summary>
	public enum ActivityStatusType {
		I,	// In Transit
		D,	// Delivered
		X,	// Exception
		P,	// Pickup
		M	// Manifest pickup
	}

	/// <summary>
	/// UpsEnumDescription used for conversion of UpsLib\Enums into string description.
	/// </summary>
	public class UpsEnumDescription {
		public static String getDescription(Object val) {
			if (val.GetType() == typeof(PickupTypes)) {
				switch ((int)val) {
					case 01: return "Daily Pickup";
					case 03: return "Customer Counter";
					case 06: return "One Time Pickup";
					case 07: return "On Call Air";
					case 19: return "Letter Center";
					case 20: return "Air Service Center";
					default: return null;
				}
			} else if (val.GetType() == typeof(UpsServiceCodes)) {
				switch ((int)val) {
					case 01: return "Next Day Air";
					case 02: return "2nd Day Air";
					case 03: return "Ground";
					case 07: return "Worldwide Express";
					case 08: return "Worldwide Expedited";
					case 11: return "Standard";
					case 12: return "3-Day Select";
					case 13: return "Next Day Air Saver";
					case 14: return "Next Day Air Early AM";
					case 54: return "Worldwide Express Plus";
					case 59: return "2nd Day Air AM";
					case 65: return "Express Saver";
					default: return null;
				}
			} else if (val.GetType() == typeof(PackageTypeCodes)) {
				switch ((int)val) {
					case 00: return "Unknown";
					case 01: return "UPS letter";
					case 02: return "Package";
					case 03: return "UPS Tube";
					case 04: return "UPS Pak";
					case 21: return "UPS Express Box";
					case 24: return "UPS 25KG Box";
					case 25: return "UPS 10KG Box";
					default: return null;
				}
			} else if (val.GetType() == typeof(ActivityStatusType)) {
				switch (((ActivityStatusType)val).ToString()) {
					case "I": return "In Transit";
					case "D": return "Delivered";
					case "X": return "Exception";
					case "P": return "Pickup";
					case "M": return "Manifest pickup";
					default: return null;
				}
			}
			return null;
		}
	}

}
