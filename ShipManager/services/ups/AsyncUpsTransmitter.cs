using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;

namespace HiRes.ShipmentManager.UPS {	
	/// <summary>
	/// RequestState deliver to asynchronous methods and  
	/// contain objects, that can be used by this methods.
	/// </summary>
	public class RequestState {
		public WebRequest request;
		public String requestData;
		public RequestState() {}
	}

	/// <summary>
	/// AsyncUpsTransmitter is analogue to UpsTransmitter. 
	/// It is used for asynchronous data transferring to UPS.
	/// This class was created when "data transfer timeout" 
	/// error was received in thread testing of UpsTransmitter.
	/// </summary>
	public class AsyncUpsTransmitter {
		
		// this event is ON, when all data received from UPS
		private ManualResetEvent allDone = new ManualResetEvent(false);
		// data, received from UPS
		private String responseData;

		public AsyncUpsTransmitter() {
		}

		/// <summary>
		/// Transmit request to UPS and return response.
		/// </summary>
		/// <param name="requestData">request string</param>
		/// <param name="tool">Rate or Track</param>
		/// <returns>response string</returns>
		public String Transmit(String requestData, UpsTools tool) {
			//WebRequest request = WebRequest.Create("https://www.ups.com/ups.app/xml/" + tool.ToString());
			WebRequest request = WebRequest.Create(ShipmentManagerConfigHandler.UpsAppUrl + tool.ToString());
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = requestData.Length;
			request.Method = "POST";
			
			RequestState rs = new RequestState();
			rs.request = request;
			rs.requestData = requestData;
			// start asynchronous request
			request.BeginGetRequestStream(new AsyncCallback(RequestStreamCallBack), rs);
			// wait while response receiving
			allDone.WaitOne();
			return responseData;
		}

		/// <summary>
		/// Called when WebRequest request stream is available.
		/// </summary>
		/// <param name="ar"></param>
		private void RequestStreamCallBack(IAsyncResult ar) {
			// receive request state
			RequestState rs = (RequestState) ar.AsyncState;
			// get request and request data, previously stored in RequestState
			WebRequest request = rs.request;
			String requestData = rs.requestData;
			// get request stream
			Stream requestStream = request.EndGetRequestStream(ar);
			ASCIIEncoding enc = new ASCIIEncoding();
			// write request to UPS
			requestStream.Write(enc.GetBytes(requestData), 0, requestData.Length);
			requestStream.Flush();

			// get response stream from UPS
			WebResponse response = request.GetResponse();
			Stream responseStream = response.GetResponseStream();
			StringBuilder sb = new StringBuilder();
			int d;
			// read data
			while ((d = responseStream.ReadByte()) != -1) {
				sb.Append((char)d);
			}
			// Abort method is needed, because next request will fail
			request.Abort();
			responseStream.Close();
			response.Close();
			// save received data in responseData
			responseData = sb.ToString();
			// all done
			allDone.Set();
		}

	}
}
