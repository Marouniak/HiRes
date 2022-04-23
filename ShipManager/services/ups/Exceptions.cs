using System;

namespace HiRes.ShipmentManager.UPS {

	public class UpsRateException : Exception {
		public UpsRateException() {
		}
		public UpsRateException(string message) : base(message) {
		}
		public UpsRateException(string message, Exception inner) : base(message, inner) {
		}
	}

	public class UpsTrackException : Exception {
		public UpsTrackException() {
		}
		public UpsTrackException(string message) : base(message) {
		}
		public UpsTrackException(string message, Exception inner) : base(message, inner) {
		}
	}

}
