using System;

namespace HiRes.Common {

	/// <summary>
	/// 
	/// </summary>
	public class TxnFSM	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="txn">Origin payment transaction determine new state for</param>
		/// <param name="actionType">Type of the related transaction(capturing, voiding ) that's going to be performed</param>
		/// <param name="newState">New state that origin transaction comes to in case of successful 
		/// completeing of the related transaction.
		/// </param>
		/// <returns>
		/// <code>false</code>if action can't be performed on the given transaction
		/// <code>true</code>if action can be performed on the given transaction.
		/// </returns>
		[Obsolete("",true)]
		public static bool GetNewState(PaymentTransactionInfo txn, PaymentTransactionType actionType, out TransactionState newState) {
			newState = TransactionState.None;
			
			if (actionType==PaymentTransactionType.Inquiry) {
				newState = txn.TxnState;
				return true;
			}
			if (actionType==PaymentTransactionType.Credit) {
				//TODO: implement Credit
				throw new NotImplementedException();
			}

			switch (txn.TxnState) {
				case TransactionState.None:
					switch (actionType) {
						case PaymentTransactionType.Authrozation:
							newState = TransactionState.Authorized;
							return true;
						case PaymentTransactionType.Sale:
							newState = TransactionState.Sold;
							return true;
						default:
							return false;
					}
					break;
				case TransactionState.Authorized:
					switch (actionType) {
						case PaymentTransactionType.DelayedCapture:
							newState = TransactionState.Captured;
							return true;
						case PaymentTransactionType.Void:
							newState = TransactionState.Void;
							return true;
						default:
							return false;
					}
					break;
				case TransactionState.Captured:
					switch (actionType) {
						case PaymentTransactionType.Void:
							newState = TransactionState.Void;
							return true;
						default:
							return false;
					}					
					break;
				case TransactionState.RequireVoiceAuthorization:
					switch (actionType) {
						case PaymentTransactionType.VoiceAuthorization:
							newState = TransactionState.Authorized;
							return true;
						case PaymentTransactionType.Void:
							newState = TransactionState.Void;
							return true;
						default:
							return false;
					}
					break;
				case TransactionState.Sold:
					switch (actionType) {
						case PaymentTransactionType.Void:
							newState = TransactionState.Void;
							return true;
						default:
							return false;
					}
					break;
				default:
					return false;
			}
		}


		public static bool GetNewState(TransactionState curState, PaymentTransactionType actionType, out TransactionState newState) {
			newState = TransactionState.None;
			
			if (actionType==PaymentTransactionType.Inquiry) {
				newState = curState;
				return true;
			}
			if (actionType==PaymentTransactionType.Credit) {
				//TODO: implement Credit
				throw new NotImplementedException();
			}

			switch (curState) {
				case TransactionState.None:
				switch (actionType) {
					case PaymentTransactionType.Authrozation:
						newState = TransactionState.Authorized;
						return true;
					case PaymentTransactionType.Sale:
						newState = TransactionState.Sold;
						return true;
					default:
						return false;
				}
					break;
				case TransactionState.Authorized:
				switch (actionType) {
					case PaymentTransactionType.DelayedCapture:
						newState = TransactionState.Captured;
						return true;
					case PaymentTransactionType.Void:
						newState = TransactionState.Void;
						return true;
					default:
						return false;
				}
					break;
				case TransactionState.Captured:
				switch (actionType) {
					case PaymentTransactionType.Void:
						newState = TransactionState.Void;
						return true;
					default:
						return false;
				}					
					break;
				case TransactionState.RequireVoiceAuthorization:
				switch (actionType) {
					case PaymentTransactionType.VoiceAuthorization:
						newState = TransactionState.Authorized;
						return true;
					case PaymentTransactionType.Void:
						newState = TransactionState.Void;
						return true;
					default:
						return false;
				}
					break;
				case TransactionState.Sold:
				switch (actionType) {
					case PaymentTransactionType.Void:
						newState = TransactionState.Void;
						return true;
					default:
						return false;
				}
					break;
				default:
					return false;
			}

		}
	}
}
