/**
 * FILE: FedExAPI.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: A wrapper around FedEx client API.
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		27-Apr-2002	Maxim Lysenko
 *			Initial implementation
 */

using System;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;

namespace HiRes.ShipmentManager.FedEx {
	// FedEx API function result codes
	public enum FedExTxnState {
		API_OK = 0,
		API_SUCCESS = -1,
		API_NOT_INIT_ERROR = -8,				// FedExAPIClient.dll was not initialized.
		API_INIT_ERROR = -24,					// Error trying to initialize FedExAPIClient.dll
		API_UNKNOWN_HOST_EXCEPTION = -2201,		// Invalid IP Address. Insure the IP Address for ATOM is correct.
		API_UNABLE_TO_OPEN_SOCKET = -2202,		// Invalid IP Address or port for the ATOM you are trying to connect with, or the ATOM you are trying to connect with is not running.
		API_SET_TIMEOUT_FAILED = -2203,			// Setting the read timeout you requested failed. Check your timeout value.
		API_UNABLE_TO_OPEN_OUTPUTSTREAM = -2204,// Unable to obtain resources necessary for communicating with the server. Try closing some applications.
		API_UNABLE_TO_OPEN_INPUTSTREAM = -2205,	// Unable to obtain resources necessary for communicating with the server. Try closing some applications.
		API_ERROR_READING_REPLY = -2206,		// The connection to FedEx timed out before receiving all of the reply.
		API_ERROR_READING_HEADER = -2207,		// The connection to FedEx closed before receiving any of the reply. This could also result from a timeout.
		API_ERROR_READING_INPUT = -2208,		// Contact FedEx.
		API_ENCODING_EXCEPTION = -2209			// Contact FedEx.
	}

	// The Universal Transaction Identifier (UTI) is a unique code that 
	// has been assigned to a given transaction type
	// FDXG - FedEx ground service FDXE - FedEx express service 
	public enum UniversalTransactionIdentifier {
		FDXG_END_OF_DAY_CLOSE = 1002,
		FDXE_DELETE_PACKAGE = 1005,
		FDXE_SHIP_PACKAGE = 2016,
		FDXE_RATE_PACKAGE = 2017,
		FDXE_SERVICE_AVAILABILITY = 2018,
		FDXG_SHIP_PACKAGE = 3000,
		FDXG_DELETE_PACKAGE = 3001,
		ALL_SUBSCRIPTION = 3003,
		FDXG_RATE_PACKAGE = 3004,
		ALL_TRACK_PACKAGE = 5000,
		ALL_SIGNATURE_PROOF_OF_DELIVERY = 5001
	}

	/// <summary>
	/// FedEx API Exception class
	/// </summary>
	/// <exception cref="System.Exception"> Thrown when FedExAPI functions failed </exception>
	public class FedExApiException : Exception {
		public FedExApiException() {
		}
		public FedExApiException(string message) : base(message) {
		}
		public FedExApiException(string message, Exception inner) : base(message, inner) {
		}
	}

	/// <summary>
	/// FedEx Transaction Failed Exception class
	/// </summary>
	/// <exception cref="System.Exception"> Thrown when FedEx transaction failed. It can happen due logical errors in program like invalid zip code or incorrect account number</exception>
	public class FedExTxnException : Exception {
		public FedExTxnException() {
		}
		public FedExTxnException(string message) : base(message) {
		}
		public FedExTxnException(string message, Exception inner) : base(message, inner) {
		}
	}

	/// <summary>
	/// FedEx API dll wrapper
	/// </summary>
	public class FedExAPI : IDisposable {
		#region Private member variables
		private string host;
		private int port;
		#endregion
		/// <summary>
		/// Default constructor
		/// </summary>
		public FedExAPI() : this("127.0.0.1", 8190 ) {
		}

		/// <summary>
		/// Initializes a new instance of FedExAPI class
		/// </summary>
		public FedExAPI(string host, int port) {
			this.host = host;
			this.port = port;
			FedExTxnState result = (FedExTxnState) FedExAPIInit();
			if (result != FedExTxnState.API_OK) {
				throw new FedExApiException("Initializing exception " + result.ToString());
			}
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		/// <summary>
		/// Processes FedEx transaction
		/// </summary>
		/// <param name="uti">Universal Transaction Identifier <seealso cref="UniversalTransactionIdentifier"/> </param>
		/// <param name="txnRequest">Request</param>
		/// <param name="txnResponse">Response</param>
		/// <returns>API_SUCCESFULL if ok, or error code</returns>
		public void ProcessTransaction(UniversalTransactionIdentifier uti, string txnRequest, out string txnResponse) {
			byte[] receiveBuf = new byte[10*1024]; // Default receive buffer size is 10K
			int bytesReceived = 0;
			txnResponse = "";
			FedExTxnState result = (FedExTxnState) FedExAPITransaction(this.host, this.port, txnRequest.ToCharArray(), txnRequest.Length, (int)uti, receiveBuf, receiveBuf.Length, ref bytesReceived);
			if (result == FedExTxnState.API_OK) {
				StringBuilder sb = new StringBuilder(bytesReceived, bytesReceived);
				for(int i = 0; i <  bytesReceived; i++) {
					sb.Append((char)receiveBuf[i]);
				}
				txnResponse = sb.ToString();
			} else {
				throw new FedExApiException("FedEx API exception: " + result.ToString());
			}
		}

		/// <summary>
		/// Processes FedEx transaction
		/// </summary>
		/// <param name="uti">Universal Transaction Identifier <seealso cref="UniversalTransactionIdentifier"/> </param>
		/// <param name="txnRequest">Request</param>
		/// <param name="txnResponse">Response</param>
		/// <returns>API_SUCCESFULL if ok, or error code</returns>
		public void ProcessTransaction(UniversalTransactionIdentifier uti, string txnRequest, out int bytesReceived, out byte[] txnResponse) {
			txnResponse = new byte[10*1024]; // Default receive buffer size is 10K
			bytesReceived = 0;
			FedExTxnState result = (FedExTxnState) FedExAPITransaction(this.host, this.port, txnRequest.ToCharArray(), txnRequest.Length, (int)uti, txnResponse, txnResponse.Length, ref bytesReceived);
			if (result != FedExTxnState.API_OK) {
				throw new FedExApiException("FedEx API exception: " + result.ToString());
			}
		}

		/// <summary>
		/// Free unmanaged resources imported from FedExAPIClient.dll Derived from IDisposable interface
		/// </summary>
		public void Dispose() {
			FedExAPIRelease();
		}
		
		#region Imported unmanaged code from FedExApiClient.dll
		[DllImport("fedexapiclient.dll")]
		private static extern int FedExAPIInit();

		[DllImport("fedexapiclient.dll")]
		private static extern void FedExAPIRelease();

		/// <summary>
		/// Manager API transaction to the FedEx Internet Server
		/// </summary>
		/// <param name="host">Host name or IP address of your system that is running ATOM</param>
		/// <param name="port">The TCP/IP port that you have configured ATOM to listen	usually 8190</param>
		/// <param name="sendBuf">Pointer to a user's buffer containing the request fields for a transaction</param>
		/// <param name="sendBufLen">Length of data in user's buffer containing the request fields for a transaction</param>
		/// <param name="universalTransCode">Value that represents the Universal Transaction Identifier</param>
		/// <param name="receiveBuf">Pointer to a user's buffer to receive the request fields for a transaction</param>
		/// <param name="receiveBufLen">Length of the user's receive buffer which will receive the request fields for a transaction</param>
		/// <param name="actualReceiveLen">An integer to receive the actual byte count of the size of the data received</param>
		/// <returns></returns>
		[DllImport("fedexapiclient.dll")]
		private static extern int FedExAPITransaction (string host, int port, char[] sendBuf, int sendBufLen, int universalTransCode, byte[] receiveBuf, int receiveBufLen, ref int actualReceiveLen);
		#endregion
	}
}
