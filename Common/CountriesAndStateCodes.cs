using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace HiRes.Common {
	public class CodeName {
		private String code;
		private String name;

		public CodeName() {
		}

		public CodeName(String code, String name) {
			this.code = code;
			this.name = name;
		}

		public String Code {
			get { return code; }
			set { code = value; }
		}

		public String Name {
			get { return name; }
			set { name = value; }
		}
	}

	/// <summary>
	/// Summary description for Countries.
	/// </summary>
	public class CountriesAndStateCodes {
		
		//private CodeName[] codes;
		private string _filename;

		public CountriesAndStateCodes(String fileName) {
			_filename = fileName;
		}

		public ArrayList GetCodeNames() {
			XmlDocument document = new XmlDocument();
			document.Load(_filename);
			XmlNodeList nlCountries = document.GetElementsByTagName("item");
			ArrayList res = new ArrayList();
//			codes = new CodeName[nlCountries.Count];
			for (int i=0; i<nlCountries.Count; i++) {
				XmlElement elCountry = (XmlElement)nlCountries[i];
				String code = elCountry.GetElementsByTagName("code")[0].InnerText;
				String name = elCountry.GetElementsByTagName("name")[0].InnerText;
				res.Add(new CodeName(code, name));
//				codes[i] = new CodeName(code, name);
			}			
			return res;
		}

	}
}