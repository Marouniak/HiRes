using System;
using System.Collections;

using HiRes.BusinessRules;
using HiRes.Common;

using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessFacade {
/*
	public enum PaymentTxnResCode {
		OK = 0,
		AUTHORIZATION_FAILED = -1,
		ORDER_DIRECTORY_CREATION_FAILED = -2,
		DB_OPERATION_FAILED = -3,
		NON_AUTHENTICATED_CUSTOMER = -4,
		BILLINGCONTACT_NOTSET = -5
	}
*/
	/// <summary>
	/// Summary description for TransactionFacade.
	/// </summary>
	public class PaymentTransactionFacade {

		public static PaymentTransactionInfo CreateVoidTransaction(string txnIdToVoid) {

			PaymentTransactionInfo txn =  new PaymentTransactionInfo();
			txn.OriginTxnId = txnIdToVoid;
			txn.TxnType = PaymentTransactionType.Void;

			txn.TxnState = TransactionState.None;
			
			txn.SetAsNew(true);

			return txn;
		}

		public static PaymentTransactionInfo CreateAuthorizeTransaction(decimal amount, ContactInfo BillTo, PaymentSourceInfo paymentSourceInfo/*, ref ArrayList orders*/) {
			PaymentTransactionInfo txn =  new PaymentTransactionInfo();
			txn.OriginTxnId = string.Empty;
			txn.TxnType = PaymentTransactionType.Authrozation;
			txn.Amount = amount;
			txn.BillTo = BillTo;
			txn.PaymentSource = paymentSourceInfo;
	/*		if (orders!=null) {
				// TODO: Create Payments
				//txn.Payments =
			}*/
			txn.TxnState = TransactionState.None;
			//txn.PaymentSource.PaymentInstrumentType = 
			txn.SetAsNew(true);

			return txn;
		}
		public static PaymentTransactionInfo CreateSaleTransaction(decimal amount, ContactInfo BillTo, PaymentSourceInfo paymentSourceInfo/*, ref ArrayList orders*/) {
			PaymentTransactionInfo txn =  new PaymentTransactionInfo();
			txn.OriginTxnId = string.Empty;
			txn.TxnType = PaymentTransactionType.Sale;
			txn.Amount = amount;
			txn.BillTo = BillTo;
			txn.PaymentSource = paymentSourceInfo;
			/*		if (orders!=null) {
						// TODO: Create Payments
						//txn.Payments =
					}*/
			txn.TxnState = TransactionState.None;
			//txn.PaymentSource.PaymentInstrumentType = 
			txn.SetAsNew(true);

			return txn;
		}
		public static PaymentTransactionInfo CreateCaptureTransaction(string txnIdToCapture) {
			PaymentTransactionInfo txn =  new PaymentTransactionInfo();
			txn.OriginTxnId = txnIdToCapture;
			txn.TxnType = PaymentTransactionType.DelayedCapture;
			
			txn.Amount = 0.00m;
			txn.Properties = TxnProperties.None;

			txn.TxnState = TransactionState.None;
			txn.SetAsNew(true);

			return txn;			
		}
		
		/*public static PaymentTransactionInfo CreateCaptureTransaction(string txnIdToCapture, Hashtable amountsToCapture) {
		}*/
		public static PaymentTransactionInfo CreateCaptureTransaction(string txnIdToCapture, decimal amountToCapture) {
			PaymentTransactionInfo txn =  new PaymentTransactionInfo();
			txn.OriginTxnId = txnIdToCapture;
			txn.TxnType = PaymentTransactionType.DelayedCapture;
			
			txn.Amount = amountToCapture;
			//TODO: Consider verifying if amount's really changed
			txn.Properties = TxnProperties.TxnAmountChanged;

			txn.TxnState = TransactionState.None;
			txn.SetAsNew(true);

			return txn;
		}

		public static PaymentTransactionInfo CreateNewPaymentTransaction(decimal amount, ContactInfo billTo, PaymentSourceInfo paymentSource) {
			return CreateNewPaymentTransaction(amount, billTo, paymentSource, PersistentBusinessEntity.SID_EMPTY);
		}
		/// <summary>
		/// This method is used to create a new payment transaction based on payment source type.
		/// Card and ECheck payments should be performed through authorize transaction.
		/// For other payment sources (cash, money order, check) we use sale transaction and dont pass them through Verisign payment gateway.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="billTo"></param>
		/// <param name="paymentSource"></param>
		/// <returns></returns>
		public static PaymentTransactionInfo CreateNewPaymentTransaction(decimal amount, ContactInfo billTo, PaymentSourceInfo paymentSource, string customerId) {
			switch (paymentSource.PaymentInstrumentType) {
				case PaymentInstrumentType.Card:
					return CreateAuthorizeTransaction(amount,billTo,paymentSource);
				case PaymentInstrumentType.ECheck:
					return CreateAuthorizeTransaction(amount,billTo,paymentSource);
				default:
					return CreateSaleTransaction(amount,billTo,paymentSource);
			}
		}

		public PaymentTransactionInfo GetFullTransactionInfo(string txnId) {
			PaymentTransaction pt = new PaymentTransaction();
			PaymentTransactionInfo txn = pt.GetTransactionInfo(txnId);
			txn.Payments = new ArrayList(pt.GetPayments(txnId));
			return txn;
		}

		public PaymentTransactionInfo[] GetTransactions(FilterExpression filter, OrderExpression orderBy) {
			return new PaymentTransaction().GetTransactions(filter,orderBy);
		}

		public PaymentTransactionInfo[] GetBaseTransactions(FilterExpression filter, OrderExpression orderBy) {
			//throw new NotImplementedException();
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Authrozation);
			filter.Add(PaymentTransactionFields.TxnType, (ushort) PaymentTransactionType.Sale);
			return new PaymentTransaction().GetTransactions(filter,orderBy);
		}

		public ArrayList GetOrderPayments(int orderID) {
			return new PaymentTransaction().GetOrderPayments(orderID);
		}

		public PaymentTxnResCode PlaceBaseTransaction(PaymentTransactionInfo txn) {
			PaymentTxnResCode rescode = PaymentTxnResCode.OK;
			
			if ((txn.TxnType!=PaymentTransactionType.Sale)&&(txn.TxnType!=PaymentTransactionType.Authrozation)) {
				throw new ArgumentException("txn shouyld be 'Sale' or 'Authorize'");
			}
			switch (txn.TxnType) {
				case PaymentTransactionType.Authrozation:
					new PaymentTransaction().AuthoriseTransaction(txn, out rescode);
					break;
				case PaymentTransactionType.Sale:
					new PaymentTransaction().SaleTransaction(txn, out rescode);
					break;
			}

			return rescode;
		}

		public PaymentTxnResCode CaptureFullAmount(string txnIdToCapture) {
			PaymentTxnResCode rescode = PaymentTxnResCode.OK;
			PaymentTransactionInfo captureTxn = CreateCaptureTransaction(txnIdToCapture);

			PaymentTransaction paymentTransaction = new PaymentTransaction();
			PaymentTransactionInfo origTxn = paymentTransaction.GetTransactionInfo(txnIdToCapture);
			if (origTxn == null) {
				rescode = PaymentTxnResCode.TXNID_NOT_FOUND;
				return rescode;
			}
			if (!origTxn.CanBeCaptured) {
				rescode = PaymentTxnResCode.CAPTURE_OPERATION_CANT_BE_PERFORMED;
				return rescode;
			}
			captureTxn.Amount = origTxn.Amount;
			paymentTransaction.CaptureTransaction(captureTxn,out rescode);

			return rescode;
		}
	}
}
