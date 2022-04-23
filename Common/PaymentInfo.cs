using System;

namespace HiRes.Common {

	public enum OrderPaymentType {
		Downpayment = 0,
		DesignAmount = 1,
		RemainingAmoount = 2
	}

	/// <summary>
	/// <code>PaymentInfo</code> contains info regarding order payment.
	/// It's used to be composed by distributing the payment transaction amount between
	/// orders the customer pays for.
	/// <see cref="PaymentTransactionInfo"/>
	/// </summary>
	public class PaymentInfo : SelfValidator {
		
		private int paymentId;
		private String txnReferenceId;
		private DateTime paymentDate = DateTime.Now;
		private decimal amount;
		private int orderId;

		private OrderPaymentType _paymentType;
		private TransactionState _paymentState;
		private bool _isCancelled;

		public PaymentInfo() {
		}

		public int PaymentId {
			get { return paymentId; }
			set { paymentId = value; }
		}

		public String TxnReferenceId {
			get { return txnReferenceId; }
			set { txnReferenceId = value; }
		}

		public DateTime PaymentDate {
			get { return paymentDate; }
			set { paymentDate = value; }
		}

		public decimal Amount {
			get { return amount; }
			set { amount = value; }
		}

		public int OrderId {
			get { return orderId; }
			set { orderId = value; }
		}

		public OrderPaymentType PaymentType {
			get { return _paymentType; }
			set { _paymentType = value;}
		}
		
		public TransactionState PaymentState {
			get { return _paymentState; }
			set { _paymentState = value;}
		}

		public bool IsCancelled {
			get { return _isCancelled; }
			set { _isCancelled = value; }
		}
		public bool IsValid {
			get {
				throw new NotImplementedException();
				//return true;
			}
		}
	}

	public enum PaymentFields {
		NONE = 0,
		PaymentId,
		TxnReferenceId,
		PaymentDate,
		Amount,
		OrderId
	}

}
