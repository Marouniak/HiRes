/**
 * FILE: ShipmentManagerConfigHandler.cs
 * 
 * PROJECT: HiRes studios web application
 * 
 * ABSTRACT: Loads ShipManager information from config file
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		18-May-2002	Maxim Lysenko
 *			Initial implementation
 */

using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

using HiRes.Common;
using HiRes.ShipmentManager.FedEx;
using HiRes.ShipmentManager.UPS;

namespace HiRes.ShipmentManager {

	/// <summary>
	/// The information about host and port where FedEx ATOM application is located
	/// </summary>
	public struct FedExATOMInfo {
		public string Host;
		public int Port;
	}

	/// <summary>
	///     Configuration settings for shipment manager
	///     <remarks> 
	///         The OnApplicationStart function in this class must be called
	///         from the Application_OnStart event in Global.asax. It will read configuration section and
	///         load configuration setting. 
	///     </remarks>
	/// </summary>
	public class ShipmentManagerConfigHandler : IConfigurationSectionHandler {
		private static AddressInfo		defaultSenderAddress;
		private static string			companyName;
		private static ShippingServicePrinterType printerType;
		private static ShippingServiceMediaType mediaType;
		private static FedExAccessInfo  fedExAccessInfo;
		private static FedExATOMInfo    fedExATOMInfo;
		private static AccessRequest    upsAccessRequest;
		private static PickupTypes		upsPickupType;
		private static FedExShipmentDropOffType fedExPickupType;
		private static string			upsAppUrl;
		private static string			upsCustomerClassificationCode;
		private static string			defaultSenderContactPhone;

		/// <summary>
		///		The method derived from IConfigurationSectionHanler 
		///     Called from OnApplicationStart to initialize settings from the Web.Config file(s). 
		///     <param name="parent">An object created by processing a section with this name in a Config.Web file in a parent directory.</param>
		///     <param name="configContext">The config's context.</param>
		///     <param name="section">The section to be read.</param>
		/// </summary>
		public Object Create(Object parent, object configContext, XmlNode section) {
			NameValueSectionHandler baseHandler = new NameValueSectionHandler();
			NameValueCollection configSettings = (NameValueCollection)baseHandler.Create(parent, configContext, section);
			// read FedEx authentication info
			fedExAccessInfo.AccountNumber = ReadString(configSettings, "FedExAccessInfo.AccountNumber");			// "234915916" 
			fedExAccessInfo.MeterNumber = ReadString(configSettings, "FedExAccessInfo.MeterNumber");				// "1014712" 
			// read FedEx ATOM configuration
			fedExATOMInfo.Host = ReadString(configSettings, "FedExATOMInfo.Host");									// "127.0.0.1" 
			fedExATOMInfo.Port = int.Parse(ReadString(configSettings, "FedExATOMInfo.Port"));						// "8190" 
			// read FedEx pickup type
			int pickupType =  int.Parse(ReadString(configSettings, "FedExPickupType"));
			fedExPickupType = (FedExShipmentDropOffType) pickupType;

			// read UPS authentication info
			upsAccessRequest.AccessLicenseNumber = ReadString(configSettings, "UpsAccessInfo.AccessLicenseNumber");	// "DB773CE24F789D68"
			upsAccessRequest.UserId = ReadString(configSettings, "UpsAccessInfo.UserId");							// "hiresups01" 
			upsAccessRequest.Password = ReadString(configSettings, "UpsAccessInfo.Password");						// "ship987" 
			// read UPS pickup type
			pickupType = int.Parse(ReadString(configSettings, "UpsPickupType"));
			upsPickupType = (PickupTypes) pickupType;

			// read the printer and media info
			int i = int.Parse(ReadString(configSettings, "LabelPrinterType"));
			printerType = (ShippingServicePrinterType)i;
			i = int.Parse(ReadString(configSettings, "LabelMediaType"));
			mediaType = (ShippingServiceMediaType)i;
		
			// read the default sender address (address of Hi-Res studios)
			defaultSenderAddress = new AddressInfo();
			defaultSenderAddress.Country = ReadString(configSettings, "DefaultSenderAddress.Country");		// "US"
			defaultSenderAddress.State = ReadString(configSettings, "DefaultSenderAddress.State");			// "DC"
			defaultSenderAddress.ZipCode = ReadString(configSettings, "DefaultSenderAddress.ZipCode");		// "20016"
			defaultSenderAddress.Address1 = ReadString(configSettings, "DefaultSenderAddress.Address1");	// "20016"
			defaultSenderAddress.Address2 = ReadString(configSettings, "DefaultSenderAddress.Address2");	// "20016"
			defaultSenderAddress.City = ReadString(configSettings, "DefaultSenderAddress.City");			// "20016"
			companyName = ReadString(configSettings, "DefaultSenderAddress.CompanyName");

			defaultSenderContactPhone = ReadString(configSettings, "DefaultSender.ContactPhone");

			// read ups.app address
			upsAppUrl = ReadString(configSettings, "UpsAppUrl");
			
			// read UPS Customer Classification Code
			upsCustomerClassificationCode = ReadString(configSettings, "upsCustomerClassificationCode");

			return configSettings;
		}
		
		public ShipmentManagerConfigHandler() {
		}

		/// <summary>
		///     Call this function from Application_OnStart event handler
		/// </summary>
		public static void OnApplicationStart() {
			System.Configuration.ConfigurationSettings.GetConfig("ShipManagerConfiguration");
		}

		/// <summary>
		///		Reads a parameter from a hashtable. If there is no value associated with key function throws ConfigurationException
		///     <param name="logSetings">The Hashtable to read from.</param>
		///     <param name="key">A key for the value in the Hashtable.</param>
		/// </summary>
		public static String ReadString(NameValueCollection logSetings, String key) {
			Object keyValue = logSetings[key];
			if(keyValue != null) {
				return (String)keyValue;
			} else {
				throw new ConfigurationException("Key: " + key + " was not found in configuration.");
			}
		}

		public static FedExAccessInfo FedExAccessInfo {
			get {
				return fedExAccessInfo;
			}
		}

		public static FedExATOMInfo FedExATOMInfo {
			get {
				return fedExATOMInfo;
			}
		}

		public static FedExShipmentDropOffType FedExPickupType {
			get { return fedExPickupType; }
		}

		public static AccessRequest UpsAccessInfo {
			get { return upsAccessRequest; }
		}

		public static AddressInfo DefaultSenderAddress {
			get { return defaultSenderAddress; }
		}

		public static string CompanyName {
			get { return companyName; }
		}

		public static string DefaultSenderContactPhone {
			get { return defaultSenderContactPhone; }
		}
		
		public static PickupTypes UpsPickupType {
			get {
				return upsPickupType;
			}
		}

		public static string UpsAppUrl {
			get {
				return upsAppUrl;
			}
		}

		public static string UpsCustomerClassificationCode {
			get {return upsCustomerClassificationCode; }
		}

		public static ShippingServicePrinterType PrinterType {
			get {return printerType;}
		}

		public static ShippingServiceMediaType MediaType {
			get {return mediaType;}
		}
	}
}
