using System;
using System.Collections;
using System.Data;
using System.Text;

using HiRes.Common;
using HiRes.DAL;
using HiRes.PaymentFramework;
using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessRules {
	/// <summary>
	/// Business operation related to manipulating payment transactions
	/// </summary>
	public class PaymentTransaction {

		protected const string TXNID_PREFIX = "@";
		/// <summary>
		/// This method generates transaction id for the cash, money order etc. transactions
		/// (that was not passed through the payment gateway and hence was not acquired transaction id from this gateway)
		/// </summary>
		/// <returns></returns>
		public static string GenerateTxnId() {
			StringBuilder sb = new StringBuilder(TXNID_PREFIX);
			sb.Append(DateTime.UtcNow.Ticks);
			return sb.ToString();
		}
		/// <summary>
		/// Return full transaction info (with related payments info)
		/// </summary>
		/// <param name="txnId"></param>
		/// <returns></returns>
		public PaymentTransactionInfo GetTransactionInfo(string txnId) {
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				return pDAL.GetTransactionInfo(txnId);
			}
		}

		public PaymentTransactionInfo[] GetTransactions(FilterExpression filter, OrderExpression orderBy) {
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				return pDAL.GetTransactions(filter,orderBy);
			}
		}


		public PaymentInfo[] GetPayments(string txnId) {
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				FilterExpression filter = new FilterExpression(typeof(PaymentFields));
				filter.Add(PaymentFields.TxnReferenceId,txnId);
				return pDAL.GetPayments(filter,null);
			}
		}

		public ArrayList GetOrderPayments(int orderID) {
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				return pDAL.GetOrderPayments(orderID);
			}
		}

		public bool AddTransaction(PaymentTransactionInfo txn) {
			///1) determine what type is the transaction of
			///2) if txn type is one of the following:
			///		- sale
			///		- authorize
			///	then store transaction info and payments
			///	if tx type is :
			///		- delayed capture
			///		- void
			///
			throw new NotImplementedException();
		}

		public void SetTransactionState(string txnId, TransactionState state) {
			throw new NotImplementedException();
		}

		private PaymentTransactionResponse ProcessTransaction(PaymentTransactionInfo txn) {
			if (AppConfig.DemoMode) {
				return DemoModeHelper.GetFakedTransactionResponse();
			}
			PaymentProcessor payment = new PaymentProcessor();
			//HACK: PaymentProcessor requires expiration date to be in "mmyy" format;
			string expiration = txn.PaymentSource.ValidToMonthYear;
			//return payment.ProcessTransaction(txn.PaymentSource.AccountNumber, expiration, txn.Amount);
			return payment.ProcessTransaction(txn);

		}

		/*public bool MakeBaseTransaction(PaymentTransactionInfo txn, out PaymentTxnResCode resCode) {
			switch (txn.TxnType) {

			}
		}*/

		public bool AuthoriseTransaction(PaymentTransactionInfo txn, out PaymentTxnResCode resCode) {
			//throw new NotImplementedException();
			if (txn.TxnType!=PaymentTransactionType.Authrozation) {
				throw new ArgumentException("Wrong Transaction Type");
			}
			if ((txn.PaymentSource.PaymentInstrumentType != PaymentInstrumentType.Card)&&(txn.PaymentSource.PaymentInstrumentType != PaymentInstrumentType.ECheck)) {
				throw new ArgumentException("Wrong payment instrument. Only cards and echecks can be authorised.");
			}

			PaymentTransactionResponse ptr = ProcessTransaction(txn);

			if (!ptr.isApproved) {
				resCode = PaymentTxnResCode.AUTHORIZATION_FAILED;
				return false;
			}
			txn.CreatedTS = DateTime.Now;
			txn.TxnReferenceId = ptr.PNREF;

			TransactionState newState;
			TxnFSM.GetNewState(txn.TxnState,PaymentTransactionType.Authrozation,out newState);
			txn.TxnState = newState;

			bool res = false;
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				//FIXME: Consider calling the specialized method (Like Add)
				res = pDAL.AddTransaction(txn);
			}

			if (!res) {
				resCode = PaymentTxnResCode.DB_OPERATION_FAILED;
				//TODO: consider sending email to admin
				AppLog.LogError("Attention! Transaction passed through transactions gateway, but was not stored to the db");
				return false;
			}

			resCode = PaymentTxnResCode.OK;
			return true;


		}

		public bool CaptureTransaction(PaymentTransactionInfo txn, Hashtable amountsDistribution, out PaymentTxnResCode resCode) {
			// TODO: get number of payments assoc. with the transaction.
			// if it's different from the amountsDistribution items number return false and CAPTURE_WRONG_AMOUNT_DISTRIBUTION
			if (txn.TxnType!=PaymentTransactionType.DelayedCapture) {
				throw new ArgumentException("Wrong Transaction Type");
			}
			if ((txn.OriginTxnId==null)||(txn.OriginTxnId.Length==0)) {
				throw new ArgumentException("OriginTxnId should be specified");
			}

			PaymentTransactionResponse ptr = ProcessTransaction(txn);
						
			if (!ptr.isApproved) {
				AppLog.LogTrace(ptr.RESPMSG);
				resCode = PaymentTxnResCode.TXN_FAILED_GATEWAY;
				return false; 
			}

			txn.TxnReferenceId = ptr.PNREF;

			bool res = false;
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				//FIXME: Consider calling the specialized method (Like AddBaseTransaction)
				res = pDAL.AddTransaction(txn/*,amountsDistribution*/);
			}

			if (!res) {
				resCode = PaymentTxnResCode.DB_OPERATION_FAILED;
				//TODO: consider sending email to admin
				AppLog.LogError("Attention! Transaction passed through transactions gateway, but was not stored to the db");
				return false;
			}

			resCode = PaymentTxnResCode.OK;
			return true;


		}

		public bool CaptureTransaction(PaymentTransactionInfo txn, out PaymentTxnResCode resCode) {
			if (txn.TxnType!=PaymentTransactionType.DelayedCapture) {
				throw new ArgumentException("Wrong Transaction Type");
			}
			if ((txn.OriginTxnId==null)||(txn.OriginTxnId.Length==0)) {
				throw new ArgumentException("OriginTxnId should be specified");
			}

			PaymentTransactionResponse ptr = ProcessTransaction(txn);
						
			if (!ptr.isApproved) {
				AppLog.LogTrace(ptr.RESPMSG);
				resCode = PaymentTxnResCode.TXN_FAILED_GATEWAY;
				return false; 
			}

			txn.TxnReferenceId = ptr.PNREF;

			TransactionState newState;
			TxnFSM.GetNewState(txn.TxnState,PaymentTransactionType.DelayedCapture,out newState);
			txn.TxnState = newState;

			//txn.TxnState = TransactionState.Captured;

			bool res = false;
			using (IDbTransaction dbTrans = DbTransactionFactory.BeginTransaction()) {
				using (PaymentsDAL pDAL = new PaymentsDAL()) {
					//FIXME: Consider calling the specialized method (Like AddBaseTransaction)
					res = pDAL.AddTransaction(txn,dbTrans);
					if (!res) {
						dbTrans.Rollback();
						resCode = PaymentTxnResCode.DB_OPERATION_FAILED;
						txn.TxnState = TransactionState.None;
						return false;
					}
					res = pDAL.SetTransactionState(txn.OriginTxnId,TransactionState.Captured,dbTrans);
					if (!res) {
						dbTrans.Rollback();
						resCode = PaymentTxnResCode.DB_OPERATION_FAILED;
						txn.TxnState = TransactionState.None;
						AppLog.LogError("Attention! Transaction passed through transactions gateway, but was not stored to the db");
						return false;//TODO: consider sending email to admin
					}

					res = pDAL.SetTxnCapturedAmount(txn.OriginTxnId,txn.Amount,dbTrans);
				}

				if (!res) {

					dbTrans.Rollback();
					resCode = PaymentTxnResCode.DB_OPERATION_FAILED;
					txn.TxnState = TransactionState.None;
					AppLog.LogError("Attention! Transaction passed through transactions gateway, but was not stored to the db");
					return false;//TODO: consider sending email to admin
				} else {
					dbTrans.Commit();
					resCode = PaymentTxnResCode.OK;
					return true;
				}
			}

		}

		public bool SaleTransaction(PaymentTransactionInfo txn, out PaymentTxnResCode resCode) {
			if (txn.TxnType!=PaymentTransactionType.Sale) {
				throw new ArgumentException("Wrong Transaction Type");
			}

			switch(txn.PaymentSource.PaymentInstrumentType) {
				case PaymentInstrumentType.Card:
					PaymentTransactionResponse ptr = ProcessTransaction(txn);			

					if (!ptr.isApproved) {
						AppLog.LogTrace(ptr.RESPMSG);
						resCode = PaymentTxnResCode.TXN_FAILED_GATEWAY;
						return false; 
					}

					txn.TxnReferenceId = ptr.PNREF;
					//txn.TxnState = TransactionState.Sold;

					break;
				case PaymentInstrumentType.Cash:
					//TODO: manually create txn id
					txn.TxnReferenceId = GenerateTxnId();
					//txn.TxnState = TransactionState.Sold;
					break;
				case PaymentInstrumentType.MoneyOrder:
					txn.TxnReferenceId = GenerateTxnId();
					break;
				default:
					throw new ArgumentException("Wrong payment instrument. Only cards and cash transactions required.");

				//TODO: consider implemeting
			}
			TransactionState newState;
			TxnFSM.GetNewState(txn.TxnState,PaymentTransactionType.Sale,out newState);
			txn.TxnState = newState;
			/*if (TxnFSM.GetNewState(txn,PaymentTransactionType.Sale,out newState)) {
				txn.TxnState = newState;
			} else {
				resCode = WRONG_TRANSACTION_TYPE;
				return false;
			}*/
			
			bool res = false;
			using (PaymentsDAL pDAL = new PaymentsDAL()) {
				//FIXME: Consider calling the specialized method (Like AddBaseTransaction)
				res = pDAL.AddTransaction(txn);
			}

			if (!res) {
				resCode = PaymentTxnResCode.DB_OPERATION_FAILED;
				//TODO: consider sending email to admin
				AppLog.LogError("Attention! Transaction passed through transactions gateway, but was not stored to the db");
				return false;
			}

			resCode = PaymentTxnResCode.OK;
			return true;


		}

	}
}
