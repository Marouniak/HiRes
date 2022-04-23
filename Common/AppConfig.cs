namespace HiRes.Common {
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Configuration;
	using System.Xml;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for AppConfig.
	/// </summary>
	public class AppConfig : IConfigurationSectionHandler {

		private const String DATAACCESS_CONNECTIONSTRING   = "HiRes.DataAccess.ConnectionString";
		private const String PRITINGSITE_SITEID = "HiRes.PrintingsSite.siteId";
		private const String UPLOAD_DIRECTORY = "UploadDirectory";
		private const String SMTP_SERVER = "SMTPServer";
		private const String EMAIL_SENDER = "EmailSender";
		private const String DEMO_MODE = "DemoMode";
		private static bool isConfigLoaded = false;
		private static bool demoMode = false;

		private const String DATAACCESS_CONNECTIONSTRING_DEFAULT   = "server=localhost; User ID=HiResAdmin;database=HiRes";
		private const int PRITINGSITE_SITEID_DEFAULT   = 1;
		private const String UPLOAD_DIRECTORY_DEFAULT = "upload";
		private const String SMTP_SERVER_DEFAULT = "localhost";
		private const String EMAIL_SENDER_DEFAULT = "sales@hiresstudios.com";
		private const bool DEMO_MODE_DEFAULT = false;
		public static String dbConnString;// = "Password=morkovka3000;Persist Security Info=True;User ID=HiResAdmin;Initial Catalog=HiRes;Data Source=MAXIM;Packet Size=4096;Connect Timeout=45;";
		public static int siteId;
		public static String UploadDirectory;
		public static string SmtpServer;
		public static string EmailSender;

		public static bool DemoMode {
			get { return demoMode; }
		}

		public AppConfig() {
		}

		public Object Create(Object parent, object configContext, XmlNode section) {
            
			NameValueCollection settings;
            
			try {
				NameValueSectionHandler baseHandler = new NameValueSectionHandler();
				settings = (NameValueCollection)baseHandler.Create(parent, configContext, section);
			} catch {
				settings = null;
			}
            
			if ( settings == null ) {
				/*dbConnString = DATAACCESS_CONNECTIONSTRING_DEFAULT;
				siteId = PRITINGSITE_SITEID_DEFAULT;
				*/
				throw new ConfigurationException("Incorrect or absent application configuration section");
			} else {
				dbConnString = AppConfig.ReadSetting(settings, DATAACCESS_CONNECTIONSTRING, DATAACCESS_CONNECTIONSTRING_DEFAULT);
				siteId = AppConfig.ReadSetting(settings, PRITINGSITE_SITEID, PRITINGSITE_SITEID_DEFAULT);
				UploadDirectory = AppConfig.ReadSetting(settings, UPLOAD_DIRECTORY, UPLOAD_DIRECTORY_DEFAULT);
				SmtpServer = AppConfig.ReadSetting(settings, SMTP_SERVER, SMTP_SERVER_DEFAULT);
				EmailSender = AppConfig.ReadSetting(settings, EMAIL_SENDER, EMAIL_SENDER_DEFAULT);
				demoMode = AppConfig.ReadSetting(settings, DEMO_MODE, DEMO_MODE_DEFAULT);
			}
			return settings;
		}
		
		public static String ReadSetting(NameValueCollection logSetings, String key, String defaultValue) {
			try {
				Object setting = logSetings[key];
				return (setting == null) ? defaultValue : (String)setting;
			} catch {
				return defaultValue;
			}
		}

		/// <summary>
		///     int version of ReadSetting.
		/// </summary>
		public static int ReadSetting(NameValueCollection logSetings, String key, int defaultValue) {
			try {
				Object setting = logSetings[key];
                
				return (setting == null) ? defaultValue : Convert.ToInt32((String)setting);
			} catch {
				return defaultValue;
			}
		}

		/// <summary>
		///     bool version of ReadSetting.
		/// </summary>
		public static bool ReadSetting(NameValueCollection logSetings, String key, bool defaultValue) {
			try {
				Object setting = logSetings[key];
                
				return (setting == null) ? defaultValue : Convert.ToBoolean((String)setting);
			} catch {
				return defaultValue;
			}
		}
		public static void OnApplicationStart() {
			if (!isConfigLoaded) {
				lock(typeof(AppConfig)) {
					System.Configuration.ConfigurationSettings.GetConfig("AppConfiguration");
					isConfigLoaded = true;
				}
			}
		}

	}
}
