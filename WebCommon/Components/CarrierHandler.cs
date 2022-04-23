using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

using HiRes.Common; 
using HiRes.Common.ShippingDefs;
using HiRes.ShipmentManager.FedEx;
using HiRes.ShipmentManager.UPS;

namespace HiRes.Web.Common.Components {
	public class CarrierHandler : IConfigurationSectionHandler	{
		const string xmlCarrier   = "carrier";
		const string xmlMethod    = "method";
		const string xmlID        = "ID";
		const string xmlName      = "name";
		const string xmlDispLabel = "dispLabel";

		public CarrierHandler() {}

		private static Hashtable _carriers;

		public object Create ( object parent, object configContext, XmlNode section ) {
			if (!section.HasChildNodes) {
				throw new ConfigurationException("Wrong application configuration. Section has no child nodes");
			}

			if (!section.FirstChild.Name.Equals(xmlCarrier)) {
				throw new ConfigurationException("Wrong application configuration.");
			}
			
			_carriers = ProcessServices(section);
			return _carriers;
		}

		private Hashtable ProcessServices(XmlNode carrierSection) {
			
			Hashtable carriers = new Hashtable();

			#region cycle
			foreach (XmlNode carrierNode in carrierSection.ChildNodes) {
				if (carrierNode.Name.Equals(xmlCarrier)) { 
					PostalCarrierInfo carrier = new PostalCarrierInfo();
					string _ID, _Name, _Key;
					if (carrierNode.Attributes[xmlID]!=null) {
						_Key = carrierNode.Attributes[xmlID].Value;
					} else {
						if (carrierNode.Attributes[xmlName]!=null) {
							_Key  = carrierNode.Attributes[xmlName].Value;
						} else {
							throw new ConfigurationException("Carrier name or ID can't be empty");
						}
					}

					// Check if ID or Name in PostalCarrier enum
					PostalCarrier _postalCarrier;
					try {
						_postalCarrier = (PostalCarrier) Enum.Parse(typeof(PostalCarrier),_Key,true);
						_ID = Enum.Format(typeof(PostalCarrier), _postalCarrier, "d");
						_Name = Enum.Format(typeof(PostalCarrier), _postalCarrier, "g");;
					} catch {
						throw new ConfigurationException("Wrong Carrier name or ID");
					}

					carrier.Id = _ID;
					carrier.Name = _Name;
					carrier.DispLabel = carrierNode.Attributes[xmlDispLabel].Value;
					if (carrierNode.Attributes[xmlDispLabel].Value!=null) {
						carrier.DispLabel = carrierNode.Attributes[xmlDispLabel].Value;
					} else {
						carrier.DispLabel = "";
					}

					switch (_postalCarrier) {
						case PostalCarrier.FedEx:
							ProcessMetods(carrierNode,typeof(FedExShippingService),carrier);
							break;
						case PostalCarrier.UPS:
							ProcessMetods(carrierNode,typeof(UpsServiceCodes),carrier);
							break;
					}
					carriers.Add(carrier.Id, carrier);
				}
			}
			#endregion
			if (carriers.Count==0) { 
				return null; 
			} else { return carriers; }
		}
		
		private void ProcessMetods(XmlNode methodSection, Type EnumType, PostalCarrierInfo carrier) {
			foreach (XmlNode methodNode in methodSection.ChildNodes) {
				ShippingServiceInfo method = new ShippingServiceInfo();
				if (methodNode.Name.Equals(xmlMethod)) { 
					string _ID, _Name, _Key;
					if (methodNode.Attributes[xmlID]!=null) {
						_Key = methodNode.Attributes[xmlID].Value;
					} else {
						if (methodNode.Attributes[xmlName]!=null) {
							_Key  = methodNode.Attributes[xmlName].Value;
						} else {
							throw new ConfigurationException("Method name or ID can't be empty");
						}
					}

					// Check if ID or Name in EnumType
					try {
						Enum _enum = (Enum) Enum.Parse(EnumType,_Key,true);
						if (EnumType==typeof(UpsServiceCodes)) {
							_ID = ((int)((UpsServiceCodes)_enum)).ToString("00");
						} else {
							_ID = Enum.Format(EnumType,_enum, "d");
						}
						_Name = Enum.Format(EnumType,_enum, "g");
					} catch (Exception ex) {
						throw new ConfigurationException("Wrong method name or ID");
					}

					method.Id = _ID;
					method.Name = _Name;
					if (methodNode.Attributes[xmlDispLabel].Value!=null) {
						method.DispLabel = methodNode.Attributes[xmlDispLabel].Value;
					} else {
						method.DispLabel = "";
					}
					carrier.AddService(method);     
				}
			}
		}

		public static void OnApplicationStart() {
			System.Configuration.ConfigurationSettings.GetConfig("CarrierConfiguration");
		}

		public static Hashtable AvailableCarriers {
			get {
				return _carriers;
			}
		}
	}

}
