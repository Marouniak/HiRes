using System;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Threading;
using System.Web;

namespace /*Com.Eurosoftusa.Utils*/HiRes.Common {

	public class MIMEType {

		private static NameValueCollection namValue;
		private static bool isInitialized = false;
		private const string MappingFile = "mime.xml";
		
		public static bool Init(string filePath) {
			bool res = false;
			lock (typeof(MIMEType)) {
				isInitialized = false;
				namValue.Clear();
				try {
					XmlTextReader reader = new XmlTextReader(filePath);
					string key = "";
					string val = "";

					while (reader.Read()){
						if (reader.NodeType == XmlNodeType.Element) {
							if (reader.Name.ToUpper() != "MAP") 
								continue;
							key = "";
							val = "";

							if (reader.MoveToFirstAttribute()) 
								key = reader.Value;
							if (reader.MoveToNextAttribute()) 
								val = reader.Value;
							namValue.Add(key.ToUpper(),val);
						}
					}
					reader.Close();
					isInitialized = true;
				} catch (Exception ex) {
					isInitialized = false;
				}
				res = isInitialized;
			}
			return res;

		}
		public static string GetMIMEType(string key) {
			if (isInitialized)	
				return namValue[key.ToUpper()];
			else throw new InvalidOperationException("Class were not successfully initialized");
		}

		static MIMEType() {
			namValue = new NameValueCollection();
			string filepath; 
			if (HttpContext.Current!=null) {
				
				filepath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath)+"\\bin\\"+MappingFile;
			} else {
				filepath = MappingFile;
			}
			Init(filepath);//TODO change this - get xml from resources
		}
	}
}
