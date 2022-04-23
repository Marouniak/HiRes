using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace HiRes.ShipmentManager.UPS {
	/// <summary>
	/// UPS Tools. Now available Rate and Track.
	/// </summary>
	public enum UpsTools {
		Rate,
		Track
	}

	/// <summary>
	/// UpsTransmitter used for sending request and getting response from UPS.
	/// </summary>
	public class UpsTransmitter {
		
		public UpsTransmitter()	{
		}

		/// <summary>
		/// Transmit request to UPS and return response.
		/// </summary>
		/// <param name="requestData">request string</param>
		/// <param name="tool">Rate or Track</param>
		/// <returns>response string</returns>
		public String Transmit(String requestData, UpsTools tool) {
			WebRequest request = WebRequest.Create("https://www.ups.com/ups.app/xml/" + tool.ToString());
			request.Timeout = 5 * 60 * 1000;
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = requestData.Length;
			request.Method = "POST";
			// send request
			ASCIIEncoding enc = new ASCIIEncoding();
			request.GetRequestStream().Write(enc.GetBytes(requestData), 0, requestData.Length);
			request.GetRequestStream().Flush();
			// get response
			WebResponse response = request.GetResponse();
			Stream responseStream = response.GetResponseStream();
			StringBuilder sb = new StringBuilder();
			int d;
			// read response bytes
			while ((d = responseStream.ReadByte()) != -1) {
				sb.Append((char)d);
			}
			request.Abort();
			responseStream.Close();
			response.Close();
			return sb.ToString();
		}

	}
}
