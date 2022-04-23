using System;

namespace HiRes.Common {

	public enum TxnFilterType : short {
		None = 0,
		BaseNonCaptured = 1,
		BaseCaptured = 2
	}

	/// <summary>
	/// </summary>
	public class PaymentTxnHelper {

		public static FilterExpression GetEmptyTransactionFilter() {
			FilterExpression filter = new FilterExpression(typeof(PaymentTransactionFields));
			return filter;
		}

		public static FilterExpression GetBaseTransactionFilter() {
			FilterExpression filter = new FilterExpression(typeof(PaymentTransactionFields));
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Authrozation);
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Sale);
			return filter;
		}

		public static FilterExpression GetBaseCapturedTransactionFilter() {
			FilterExpression filter = new FilterExpression(typeof(PaymentTransactionFields));
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Authrozation);
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Sale);
			filter.Add(PaymentTransactionFields.TxnState, (short) TransactionState.Captured);
			filter.Add(PaymentTransactionFields.TxnState, (short) TransactionState.Sold);
			return filter;
		}

		public static FilterExpression GetBase2BCapturedTransactionFilter() {
			FilterExpression filter = new FilterExpression(typeof(PaymentTransactionFields));
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Authrozation);
			filter.Add(PaymentTransactionFields.TxnState, (short) TransactionState.Authorized);
			return filter;
		}
	}
}
