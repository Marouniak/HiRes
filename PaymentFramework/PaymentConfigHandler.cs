using System;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;

namespace HiRes.PaymentFramework
{
	/// <summary>
	/// Summary description for PaymentConfigHandler.
	/// </summary>
	public class PaymentConfigHandler : IConfigurationSectionHandler {

		private static String hostAddress;
		private static String hostPort;
		private static String timeOut;
		private static String proxyAddress;
		private static String proxyPort;
		private static String proxyLogon;
		private static String proxyPassword;
		private static String partner;
		private static String vendor;
		private static String user;
		private static String pwd;
		
		public PaymentConfigHandler() {
		}

		public Object Create(Object parent, object configContext, XmlNode section) {
			NameValueSectionHandler baseHandler = new NameValueSectionHandler();
			NameValueCollection configSettings = (NameValueCollection)baseHandler.Create(parent, configContext, section);
			
			hostAddress = ReadString(configSettings, "HostAddress");
			hostPort = ReadString(configSettings, "HostPort");
			timeOut = ReadString(configSettings, "TimeOut");
			proxyAddress = ReadString(configSettings, "ProxyAddress");
			proxyPort = ReadString(configSettings, "ProxyPort");
			proxyLogon = ReadString(configSettings, "ProxyLogon");
			proxyPassword = ReadString(configSettings, "ProxyPassword");
			partner = ReadString(configSettings, "Partner");
			vendor = ReadString(configSettings, "Vendor");
			user = ReadString(configSettings, "User");
			pwd = ReadString(configSettings, "PWD");

			return configSettings;
		}

		public static void OnApplicationStart() {
			System.Configuration.ConfigurationSettings.GetConfig("PayFlowProConfiguration");
		}

		public static String ReadString(NameValueCollection logSetings, String key) {
			Object keyValue = logSetings[key];
			if(keyValue != null) {
				return (String)keyValue;
			} else {
				throw new ConfigurationException("Key: " + key + " was not found in configuration.");
			}
		}

		public static String HostAddress {
			get { return hostAddress; }
		}

		public static String HostPort {
			get { return hostPort; }
		}

		public static String TimeOut {
			get { return timeOut; }
		}

		public static String ProxyAddress {
			get { return proxyAddress; }
		}

		public static String ProxyPort {
			get { return proxyPort; }
		}

		public static String ProxyLogon {
			get { return proxyLogon; }
		}

		public static String ProxyPassword {
			get { return proxyPassword; }
		}

		public static String Partner {
			get { return partner; }
		}

		public static String Vendor {
			get { return vendor; }
		}

		public static String User {
			get { return user; }
		}

		public static String Pwd {
			get { return pwd; }
		}

	}
}
