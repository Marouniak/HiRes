using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace HiRes.Common {
	public class ParamStringBuilder	{
		private NameValueCollection container;
		private string delimiter;

		public string Delimiter {
			get {return delimiter;}
			set {delimiter = value;}
		}
		
		public ParamStringBuilder() {
			container = new NameValueCollection();
			delimiter = "&";
		}

		public ParamStringBuilder(string _delimiter) {
			container = new NameValueCollection();
			delimiter = _delimiter;
		}

		public void AddParam(string paramName, string paramValue) {
			container[paramName] = paramValue; 
		}

		public void RemoveParam(string paramName) {
			container.Remove(paramName);
		}

		public void AddParams(Hashtable parameters) {
			foreach (object key in parameters.Keys) {
                container.Add(key.ToString(),parameters[key].ToString());				
			}
		}

		public void AddParams(NameValueCollection parameters) {
			container.Add(parameters);
		}

		public override string ToString() {
			string s = String.Empty;
			StringBuilder sb = new StringBuilder();
			foreach(string key in container.Keys) {
				if (sb.Length>0)
					sb.Append(Delimiter);
				sb.Append(key);
				sb.Append("=");
				sb.Append(container[key]);
			}
			return sb.ToString();
		}
	}
}
