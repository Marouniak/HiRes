/**
 * FILE: FedTrackRequest.cs
 * 
 * PROJECT: -
 * 
 * ABSTRACT: Receives the tracking info on specified shipment
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		27-Apr-2002	Maxim Lysenko
 *			Initial implementation
 */
using System;
using System.Text;

namespace HiRes.ShipmentManager.FedEx {
	public class FedExTrackResponseInfo {
		public string RecipientCity;			// 15
		public string RecipientState;			// 16 
		// Converted to Datetime from format YYYYMMDD 
		public DateTime ShipDate;				// 24
		public string TrackingNumber;			// 29
		public string TrackStatus;				// 1701
		public string ServiceTypeDescription;	// 1704
		public string DeliverTo;				// 1705
		public string SignedFor;				// 1706
		// Converted to Datetime from format YYYYMMDD HHMM 
		public DateTime DeliveryTime;			// 1707 
		public int TrackActivities;				// 1715
		public string[] TrackActivityLines;		// 1721-1735	

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("Track status: ").Append(this.TrackStatus).Append("\n")
				.Append("Deliver to: ").Append(this.DeliverTo).Append("\n")
				.Append("Signed for: ").Append(this.SignedFor).Append("\n")
				.Append("Delivery time: ").Append(this.DeliveryTime).Append("\n");
			sb.Append("History: \n");
			for (int i=0; i <= this.TrackActivities-1; i++) {
				sb.Append(TrackActivityLines[i]).Append("\n");
			}
			return sb.ToString();
		}
		
		public FedExTrackResponseInfo() {
		}
	}

	/// <summary>
	/// Summary description for FedExTrackRequest.
	/// </summary>
	public class FedExTrackRequest {
		private FedExAPI fedExAPI;
		public FedExTrackRequest(FedExAPI fedExAPI) {
			this.fedExAPI = fedExAPI;
		}

		// XXX: Check. It may not work for FDXE shipments without specifying ShipDate (24)
		public FedExTrackResponseInfo SendRequest(FedExAccessInfo accessInfo, string trackingNumber) {
			return this.SendRequest(accessInfo, trackingNumber, DateTime.MinValue, string.Empty);
		}

		public FedExTrackResponseInfo SendRequest(FedExAccessInfo accessInfo, string trackingNumber, DateTime shipmentDate) {
			return this.SendRequest(accessInfo, trackingNumber, shipmentDate, string.Empty);
		}

		public FedExTrackResponseInfo SendRequest(FedExAccessInfo accessInfo, string trackingNumber, DateTime shipmentDate, string customTxnId) {
			string shipmentDateStr = string.Empty;
			if (shipmentDate != DateTime.MaxValue) {
				shipmentDateStr = shipmentDate.ToString("yyyyMMdd");
			}
			FedExRequestString request = new FedExRequestString();
			request.Append("0", "402")
				.Append("1", customTxnId)
				.Append("29", trackingNumber)
				.Append("498", accessInfo.MeterNumber)
				.Append("10", accessInfo.AccountNumber)
				.Append("24", shipmentDateStr);
	
			string responseString;
			this.fedExAPI.ProcessTransaction(UniversalTransactionIdentifier.ALL_TRACK_PACKAGE, request.ToString(), out responseString);

			FedExRequestString response = new FedExRequestString(responseString);

			if (response.ErrorMessage != String.Empty) {
				throw new FedExTxnException(response.ErrorMessage);
			} 

			FedExTrackResponseInfo trackInfo = new FedExTrackResponseInfo();
			string s;

			trackInfo.RecipientCity = response["15"];
			trackInfo.RecipientState = response["16"];
			s = response["24"];
			trackInfo.ShipDate = new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)));
			trackInfo.TrackingNumber = response["29"];
			trackInfo.TrackStatus = response["1701"];
			trackInfo.ServiceTypeDescription = response["1704"];
			trackInfo.DeliverTo = response["1705"];
			trackInfo.SignedFor = response["1706"];
			s = response["1707"];
			trackInfo.DeliveryTime = new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)), int.Parse(s.Substring(9, 2)), int.Parse(s.Substring(11, 2)), int.Parse(s.Substring(13, 2)));
			trackInfo.TrackActivities = int.Parse(response["1715"]);
			trackInfo.TrackActivityLines = new string[trackInfo.TrackActivities];
			for (int i=0; i < trackInfo.TrackActivities; i++) {
				trackInfo.TrackActivityLines[i] = response[(1721 + i).ToString()];
			}

			return trackInfo;
		}
	}
}
