using System;
using System.Collections;

namespace HiRes.Common {

	public enum PaymentTransactionFields {
		NONE = 0,
		TxnReferenceId,
		CustomerID,
		TxnDate,
		TxnType,
		TxnState,
		TxnOriginatedFrom,
		OriginTxnId,
		PaymentInstrumentType
	}

	public enum PaymentTransactionType : ushort {
		Sale = 'S',
		Credit = 'C',
		Authrozation = 'A',
		DelayedCapture = 'D',
		Void = 'V',
		VoiceAuthorization = 'F',
		Inquiry = 'I'
	}

	public enum TransactionState : short {
		None = -1,
		Authorized = 0,
		RequireVoiceAuthorization = 1,
		Captured = 2,
		Void = 3,
		Sold = 4
	}

	public enum TransactionOrigin : short {
		OrderingSiteInternetPayment = 0,
		ManuallyAdded = 1
	}
	
	[Flags]
	public enum TxnProperties {
		None = 0x0000,
		TxnAmountChanged = 0x0001
	}
	
	/// <summary>
	/// <code>PaymentTransactionInfo</code> represents a payment transaction 
	/// that can be either made through the site or added manually through 
	/// the Internal system. It serves as a base info for creating <code>PaymentInfo</code> objects.
	/// <see cref="PaymentInfo"/>
	/// </summary>
	public class PaymentTransactionInfo : PersistentBusinessEntity {

		private string txnReferenceId = PersistentBusinessEntity.SID_EMPTY;
		private string _customerId = PersistentBusinessEntity.SID_EMPTY;
		public decimal Amount = 0.00m;

		private ContactInfo _billTo;
		//public DateTime TransactionTS;
		private PaymentTransactionType _txnType;
		private TransactionState _txnState;

		public TransactionOrigin TxnOriginatedFrom;
		public TxnProperties Properties = TxnProperties.None;

		/// <summary>
		/// The VeriSign Reference ID (PNREF) which is returned for all transactions.
		/// The OriginTxnId is used when referencing a previous transaction.
		/// </summary>
		public string OriginTxnId = PersistentBusinessEntity.SID_EMPTY;
		//public bool IsAmountChanged;
		public PaymentSourceInfo PaymentSource;
		
		private bool _isNew = true;

		private ArrayList _payments;

		public ArrayList Payments {
			get { return _payments; }
			set {
				if (value != null) {
					_payments = value;
				}
			}
		}
		public string TxnReferenceId {
			get { return txnReferenceId; }
			set {
				txnReferenceId = value;

				if ((Payments!=null)&&(Payments.Count>0))
					for(int i=0; i<Payments.Count; i++) {
						((PaymentInfo)Payments[i]).TxnReferenceId = value;
					}
			}
		}
		public string CustomerID {
			get { return _customerId; }
			set { _customerId = value; }
		}
		public override DateTime CreatedTS {
			get { return _createdTS; }
			set {
				_createdTS = value;
				if ((Payments!=null)&&(Payments.Count>0))
					for(int i=0; i<Payments.Count; i++) {
						((PaymentInfo)Payments[i]).PaymentDate = value;
					}

			}
		}

		public ContactInfo BillTo {
			get { return _billTo; }
			set {
				if (value!=null) {
					_billTo = value;
				} else {
					throw new ArgumentNullException();
				}
			}
		}
		
		public PaymentTransactionType TxnType {
			get { return _txnType; }
			set {
				_txnType = value;
				/*if (Payments!=null) {
					for(int i=0;i<Payments.Count;i++) {
						((PaymentInfo)Payments[i]) = value;
					}
				}*/
			}
		}

		public bool CanBeCaptured {
			get {
				return ((TxnType == PaymentTransactionType.Authrozation)||(TxnType == PaymentTransactionType.VoiceAuthorization))&&(TxnState == TransactionState.Authorized);
			}
		}
		public TransactionState TxnState {
			get { return _txnState; }
			set {
				_txnState = value;
				if (Payments!=null) {
					for(int i=0;i<Payments.Count;i++) {
						((PaymentInfo)Payments[i]).PaymentState = value;
					}
				}
			}
		}

		public PaymentTransactionInfo() : this(true) {
			//BillTo = new ContactInfo();
		}
		
		public PaymentTransactionInfo(bool forceParts) : base() {
			if (forceParts) {
				BillTo = new ContactInfo();
				Payments = new ArrayList();
			}
		}

		public override bool IsNew {
			get { return _isNew; }
			//set { _isNew = value; }
		}

		public void SetAsNew(bool val) {
			_isNew = val; 
		}
		
		public bool IsValid {
			get {
				throw new NotImplementedException();
				//return true;
			}
		}

		public decimal AmountAvailableToCapture {
			get {
				if (_payments==null) {
					return 0.00m;
				}
				decimal amt = 0.00m;
				for(int i=0;i<Payments.Count;i++) {
					PaymentInfo payment = (PaymentInfo)Payments[i];
					amt += payment.IsCancelled?0.00m:payment.Amount;
				}
				return amt;
			}
		}

	}
}
