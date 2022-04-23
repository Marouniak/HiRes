/*
 * 
 * */
using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using HiRes.Common;

namespace HiRes.DAL {
	/// <summary>
	/// Summary description for PaymentsDAL.
	/// </summary>
	public class PaymentsDAL : System.ComponentModel.Component {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public const int TXNID_SIZE = 20;

		#region Private Members: SQL Commands
		private SqlCommand loadPaymentsCmd;
		private SqlCommand orderPaymentsCmd;
		private SqlCommand addPaymentCmd;
		private SqlCommand cancelPaymentCmd;

		private SqlCommand loadTransactionInfoCmd;
		private SqlCommand addTransactionCmd;
		private SqlCommand setTransactionStateCmd;
		private SqlCommand setCapturedAmountCmd;

		private SqlCommand setPaymentAmountsCmd;
		private SqlCommand loadTransactionsCmd;
		private SqlCommand checkTransactionIdCmd;
		#endregion
		
		#region Constructors
		public PaymentsDAL(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public PaymentsDAL()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#endregion

		#region Destructors
		public new void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
			//GC.SuppressFinalize(true);
		}

		/// <summary>
		///		Free the instance variables of this object.
		/// </summary>
		protected new virtual void Dispose(bool disposing) {
			
			if (! disposing)
				return; // we're being collected, so let the GC take care of this object
			try {
				if (loadPaymentsCmd != null)
					loadPaymentsCmd.Dispose();
				if (orderPaymentsCmd != null)
					orderPaymentsCmd.Dispose();
				if (addPaymentCmd != null)
					addPaymentCmd.Dispose();
				if (cancelPaymentCmd!= null)
					cancelPaymentCmd.Dispose();
				if (loadTransactionInfoCmd!= null) 
					loadTransactionInfoCmd.Dispose();
				if (addTransactionCmd!= null) 
					addTransactionCmd.Dispose();
				if (setTransactionStateCmd!= null)
					setTransactionStateCmd.Dispose();
				if (setCapturedAmountCmd!= null)
					setCapturedAmountCmd.Dispose();
				
				if (setPaymentAmountsCmd!= null)
					setPaymentAmountsCmd.Dispose();
				if (loadTransactionsCmd!= null)
					loadTransactionsCmd.Dispose();
				if (checkTransactionIdCmd!=null)
					checkTransactionIdCmd.Dispose();
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters

		private SqlCommand GetLoadPaymentsCmd() {
			if ( loadPaymentsCmd == null ) {
				loadPaymentsCmd = new SqlCommand("HiResAdmin.getPayments");
				loadPaymentsCmd.CommandType = CommandType.StoredProcedure;
				loadPaymentsCmd.Parameters.Add(new SqlParameter("@filterClause", SqlDbType.NVarChar));
				loadPaymentsCmd.Parameters.Add(new SqlParameter("@orderClause", SqlDbType.NVarChar));
//				loadPaymentsCmd.Parameters.Add(new SqlParameter("@txnId", SqlDbType.NVarChar, 20));
			}
			return loadPaymentsCmd;
		}

		private SqlCommand GetOrderPaymentsCmd() {
			if ( orderPaymentsCmd == null ) {
				orderPaymentsCmd = new SqlCommand("HiResAdmin.getOrderPayments");
				orderPaymentsCmd.CommandType = CommandType.StoredProcedure;
				orderPaymentsCmd.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
			}
			return orderPaymentsCmd;
		}

		private SqlCommand GetAddPaymentCmd() {
			if ( addPaymentCmd == null ) {
				addPaymentCmd = new SqlCommand("HiResAdmin.addPayment");
				addPaymentCmd.CommandType = CommandType.StoredProcedure;
				addPaymentCmd.Parameters.Add(new SqlParameter("@txnReferenceID", SqlDbType.VarChar, TXNID_SIZE));
				addPaymentCmd.Parameters.Add(new SqlParameter("@paymentDate", SqlDbType.DateTime));
				addPaymentCmd.Parameters.Add(new SqlParameter("@amount", SqlDbType.Decimal));
				addPaymentCmd.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
				addPaymentCmd.Parameters.Add(new SqlParameter("@paymentId", SqlDbType.Int));
				addPaymentCmd.Parameters["@paymentId"].Direction = ParameterDirection.Output;
			}
			return addPaymentCmd;
		}

		private SqlCommand GetCancelPaymentCmd() {
			if ( cancelPaymentCmd== null ) {
				cancelPaymentCmd = new SqlCommand("HiResAdmin.cancelPayment");
				cancelPaymentCmd.CommandType = CommandType.StoredProcedure;
				cancelPaymentCmd.Parameters.Add(new SqlParameter("@paymentId", SqlDbType.Int));
			}
			return cancelPaymentCmd;
		}

		private SqlCommand GetAddTransactionCmd() {
			if ( addTransactionCmd== null ) {
				addTransactionCmd = new SqlCommand("HiResAdmin.addPaymentTransactionInfo");
				addTransactionCmd.CommandType = CommandType.StoredProcedure;
				addTransactionCmd.Parameters.Add(new SqlParameter("@TxnReferenceId", SqlDbType.VarChar, TXNID_SIZE));
				addTransactionCmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.NVarChar, 30));
				
				addTransactionCmd.Parameters.Add(new SqlParameter("@TxnAmount", SqlDbType.Money));
				addTransactionCmd.Parameters.Add(new SqlParameter("@TxnDate", SqlDbType.DateTime));
				addTransactionCmd.Parameters.Add(new SqlParameter("@TxnType", SqlDbType.SmallInt));
				addTransactionCmd.Parameters.Add(new SqlParameter("@TxnState", SqlDbType.SmallInt));
				addTransactionCmd.Parameters.Add(new SqlParameter("@PaymentInstrument", SqlDbType.SmallInt));
				addTransactionCmd.Parameters.Add(new SqlParameter("@PaymentInstrumentType", SqlDbType.SmallInt));
				
				addTransactionCmd.Parameters.Add(new SqlParameter("@OriginTxnId", SqlDbType.NVarChar, TXNID_SIZE));
				addTransactionCmd.Parameters.Add(new SqlParameter("@TxnOriginatedFrom", SqlDbType.SmallInt));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingFirstName", SqlDbType.NVarChar, 50));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingLastName", SqlDbType.NVarChar, 70));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingContactEmail", SqlDbType.NVarChar, 100));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingAddress1", SqlDbType.NVarChar, 255));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingAddress2", SqlDbType.NVarChar, 255));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingCountry", SqlDbType.NVarChar, 50));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingState", SqlDbType.NVarChar, 20));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingZipcode", SqlDbType.NVarChar, 10));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingCity", SqlDbType.VarChar, 20));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingContactPhone", SqlDbType.VarChar, 20));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingContactFax", SqlDbType.VarChar, 20));
				addTransactionCmd.Parameters.Add(new SqlParameter("@billingCompanyName", SqlDbType.VarChar, 20));
			}
			return addTransactionCmd;
		}

		private SqlCommand GetLoadTransactionCmd() {
			if ( loadTransactionInfoCmd== null ) {
				loadTransactionInfoCmd = new SqlCommand("HiResAdmin.getPaymentTransactionInfo");
				loadTransactionInfoCmd.CommandType = CommandType.StoredProcedure;
				//FIXME:!!!
				loadTransactionInfoCmd.Parameters.Add(new SqlParameter("@txnId", SqlDbType.VarChar,TXNID_SIZE));
			}
			return loadTransactionInfoCmd;
		}

		private SqlCommand GetLoadTransactionsCmd() {
			if ( loadTransactionsCmd == null ) {
				loadTransactionsCmd = new SqlCommand("HiResAdmin.getTransactions");
				loadTransactionsCmd.CommandType = CommandType.StoredProcedure;
				loadTransactionsCmd.Parameters.Add(new SqlParameter("@filterClause", SqlDbType.NVarChar));
				loadTransactionsCmd.Parameters.Add(new SqlParameter("@orderClause", SqlDbType.NVarChar));
			}
			return loadTransactionsCmd;
		}

		private SqlCommand GetSetTransactionStateCmd() {
			if (setTransactionStateCmd == null ) {
				setTransactionStateCmd = new SqlCommand("HiResAdmin.SetTransactionState");
				setTransactionStateCmd.CommandType = CommandType.StoredProcedure;
				setTransactionStateCmd.Parameters.Add(new SqlParameter("@TxnReferenceId", SqlDbType.VarChar, TXNID_SIZE));
				setTransactionStateCmd.Parameters.Add(new SqlParameter("@TxnState", SqlDbType.SmallInt));
			}
			return setTransactionStateCmd;
		}

		private SqlCommand GetSetCapturedAmountCmd() {
			if (setCapturedAmountCmd == null ) {
				setCapturedAmountCmd = new SqlCommand("HiResAdmin.SetCapturedAmount");
				setCapturedAmountCmd.CommandType = CommandType.StoredProcedure;
				setCapturedAmountCmd.Parameters.Add(new SqlParameter("@TxnReferenceId", SqlDbType.VarChar, TXNID_SIZE));
				setCapturedAmountCmd.Parameters.Add(new SqlParameter("@amountCaptured", SqlDbType.Money));
			}
			return setCapturedAmountCmd;
		}

		private SqlCommand GetSetPaymentAmounts() {
			if (setPaymentAmountsCmd == null ) {
				setPaymentAmountsCmd = new SqlCommand("HiResAdmin.SetPaymentAmounts");
				setPaymentAmountsCmd.CommandType = CommandType.StoredProcedure;
				setPaymentAmountsCmd.Parameters.Add(new SqlParameter("@TxnReferenceId", SqlDbType.VarChar, TXNID_SIZE));
				setPaymentAmountsCmd.Parameters.Add(new SqlParameter("@paramString", SqlDbType.VarChar, 255));
			}
			return setTransactionStateCmd;
		}

		private SqlCommand GetCheckTransactionIdCmd() {
			if (checkTransactionIdCmd == null) {
				checkTransactionIdCmd = new SqlCommand("HiResAdmin.checkTransactionId");
				checkTransactionIdCmd.CommandType = CommandType.StoredProcedure;
				checkTransactionIdCmd.Parameters.Add(new SqlParameter("@TxnReferenceId", SqlDbType.VarChar, TXNID_SIZE));
				checkTransactionIdCmd.Parameters.Add(new SqlParameter("@ValidTxn", SqlDbType.SmallInt));
				checkTransactionIdCmd.Parameters["@ValidTxn"].Direction = ParameterDirection.Output;
			}
			return checkTransactionIdCmd;
		}
		#endregion


		#region Payment Commands Implementation

		public ArrayList GetOrderPayments(int orderID) {
			SqlCommand cmd = GetOrderPaymentsCmd();
			SqlConnection conn = null;
			ArrayList array = new ArrayList();
			cmd.Parameters["@orderID"].Value = orderID;

			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					PaymentInfo paymentInfo = new PaymentInfo();
					paymentInfo.PaymentId = (int)reader["PaymentId"];
					if (reader["TxnReferenceId"] != DBNull.Value)
						paymentInfo.TxnReferenceId = (String)reader["TxnReferenceId"];
					if (reader["PaymentDate"] != DBNull.Value)
						paymentInfo.PaymentDate = (DateTime)reader["PaymentDate"];
					if (reader["Amount"] != DBNull.Value)
						paymentInfo.Amount = (decimal)reader["Amount"];
					paymentInfo.OrderId = (int)reader["OrderId"];
					paymentInfo.PaymentState = (TransactionState)((short)reader["TxnState"]);
					paymentInfo.IsCancelled = (bool)reader["IsCancelled"];
					array.Add(paymentInfo);
				}
				reader.Close();
			} catch (Exception ex) {
				throw new DALException("",ex);
				
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return array;
		}

		public PaymentInfo[] GetPayments(FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadPaymentsCmd();
			cmd.Parameters["@filterClause"].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters["@orderClause"].Value = orderBy == null ? "" : orderBy.ToString();
			PaymentInfo[] payments = null;
			SqlConnection conn = null;
			ArrayList array = new ArrayList();
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					PaymentInfo paymentInfo = new PaymentInfo();
					paymentInfo.PaymentId = (int)reader["PaymentId"];
					if (reader["TxnReferenceId"] != DBNull.Value)
						paymentInfo.TxnReferenceId = (String)reader["TxnReferenceId"];
					if (reader["PaymentDate"] != DBNull.Value)
						paymentInfo.PaymentDate = (DateTime)reader["PaymentDate"];
					if (reader["Amount"] != DBNull.Value)
						paymentInfo.Amount = (decimal)reader["Amount"];
					paymentInfo.OrderId = (int)reader["OrderId"];
					
					paymentInfo.IsCancelled = (bool)reader["IsCancelled"];
					array.Add(paymentInfo);
				}
				reader.Close();
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			if (array.Count != 0) {
				payments = new PaymentInfo[array.Count];
				array.CopyTo(payments);
			}
			return payments;
		}


		public /*ArrayList*/ PaymentTransactionInfo[] GetTransactions(FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadTransactionsCmd();
			cmd.Parameters["@filterClause"].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters["@orderClause"].Value = orderBy == null ? "" : orderBy.ToString();
			PaymentTransactionInfo[] transactions = null;
			SqlConnection conn = null;
			SqlDataReader reader = null;
			ArrayList array = new ArrayList();
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader();
				while(reader.Read()) {
					PaymentTransactionInfo pt = new PaymentTransactionInfo();
					pt.TxnReferenceId	= (string) reader["TxnReferenceId"];
					if (reader["CustomerID"] != DBNull.Value) {
						pt.CustomerID	= (string) reader["CustomerID"];
					} else {
						pt.CustomerID	= string.Empty;
					}
					pt.Amount			= (decimal) reader["TxnAmount"];
					pt.CreatedTS = (DateTime) reader["TxnDate"];
					short ptType = (short)reader["TxnType"];
					pt.TxnType = (PaymentTransactionType)Enum.Parse(typeof(PaymentTransactionType),ptType.ToString());
					//pt.TxnType			= (PaymentTransactionType) ((ushort) reader["TxnType"]);
					pt.TxnState	= (TransactionState) ((short)reader["TxnState"]);
					pt.PaymentSource.PaymentInstrument = (PaymentInstrument) ((short)reader["PaymentInstrument"]);
					if (reader["OriginTxnId"] != DBNull.Value)
						pt.OriginTxnId = (string) reader["OriginTxnId"]; 
					else pt.OriginTxnId = "";
	
					if (reader["TxnOriginatedFrom"] != DBNull.Value)
						pt.TxnOriginatedFrom = (TransactionOrigin) ((short)reader["TxnOriginatedFrom"]); 
					else pt.TxnOriginatedFrom = TransactionOrigin.OrderingSiteInternetPayment;

					if (reader["BillingFirstName"] != DBNull.Value)
						pt.BillTo.FirstName = (string) reader["BillingFirstName"]; 
					else pt.BillTo.FirstName = "";

					if (reader["BillingLastName"] != DBNull.Value)
						pt.BillTo.LastName = (string) reader["BillingLastName"]; 
					else pt.BillTo.LastName = "";

					if (reader["BillingContactEmail"] != DBNull.Value)
						pt.BillTo.ContactEmail = (string) reader["BillingContactEmail"]; 
					else pt.BillTo.ContactEmail = "";

					if (reader["BillingAddress1"] != DBNull.Value)
						pt.BillTo.Address.Address1 = (string) reader["BillingAddress1"]; 
					else pt.BillTo.Address.Address1 = "";

					if (reader["BillingAddress2"] != DBNull.Value)
						pt.BillTo.Address.Address2 = (string) reader["BillingAddress2"]; 
					else pt.BillTo.Address.Address2 = "";

					if (reader["BillingCountry"] != DBNull.Value)
						pt.BillTo.Address.Country = (string) reader["BillingCountry"]; 
					else pt.BillTo.Address.Country = "";

					if (reader["BillingState"] != DBNull.Value)
						pt.BillTo.Address.State = (string) reader["BillingState"]; 
					else pt.BillTo.Address.State = "";

					if (reader["BillingZipcode"] != DBNull.Value)
						pt.BillTo.Address.ZipCode = (string) reader["BillingZipcode"]; 
					else pt.BillTo.Address.ZipCode = "";

					if (reader["BillingCity"] != DBNull.Value)
						pt.BillTo.Address.City = (string) reader["BillingCity"]; 
					else pt.BillTo.Address.City = "";

					if (reader["BillingContactPhone"] != DBNull.Value)
						pt.BillTo.ContactPhone = (string) reader["BillingContactPhone"]; 
					else pt.BillTo.ContactPhone = "";

					if (reader["BillingContactFax"] != DBNull.Value)
						pt.BillTo.ContactFax = (string) reader["BillingContactFax"]; 
					else pt.BillTo.ContactFax = "";

					if (reader["BillingCompanyName"] != DBNull.Value)
						pt.BillTo.CompanyName = (string) reader["BillingCompanyName"]; 
					else pt.BillTo.CompanyName = "";

					array.Add(pt);
				}
				reader.Close();
				if (array.Count != 0) {
					transactions = new PaymentTransactionInfo[array.Count];
					array.CopyTo(transactions);
					return transactions;
				} else {
					return new PaymentTransactionInfo[0];
				}
				//return array;
			} catch (Exception ex) {
				throw new DALException("",ex);
				//return transactions;
			} finally {
				if (!reader.IsClosed) { reader.Close(); }
				if ((conn != null)&&(conn.State!=ConnectionState.Closed)) {
					conn.Close(); 
				}
				cmd.Connection = null;
			}
			/*if (array.Count != 0) {
				transactions = new PaymentTransactionInfo[array.Count];
				array.CopyTo(transactions);
			}*/
			//return transactions;
		}


		#region payments
		internal/*public*/ int AddPayment(PaymentInfo payment) {
			return AddPayment(payment, null);
		}

		internal/*public*/ int AddPayment(PaymentInfo payment, SqlTransaction transaction) {
			SqlCommand cmd = GetAddPaymentCmd();
			cmd.Parameters["@txnReferenceID"].Value = (payment.TxnReferenceId == null) ? DBNull.Value : (Object)payment.TxnReferenceId ;
			cmd.Parameters["@paymentDate"].Value = payment.PaymentDate;
			cmd.Parameters["@amount"].Value = payment.Amount;
			cmd.Parameters["@orderID"].Value = payment.OrderId;

			SqlConnection conn = null;
			int rowsAffected = 0;
			int id = PersistentBusinessEntity.ID_EMPTY;
			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = transaction;
				}
				cmd.Connection = conn;
				//conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0) {
					id = (int)cmd.Parameters["@paymentId"].Value;
				}
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
			return id;
		}

		/*public bool CancelPayment(int paymentId) {
			SqlCommand cmd = GetCancelPaymentCmd();
			cmd.Parameters["@paymentId"].Value = paymentId;
			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}*/

		public bool CancelPayment(int paymentId, IDbTransaction dbTrans) {
			SqlCommand cmd = GetCancelPaymentCmd();
			cmd.Parameters["@paymentId"].Value = paymentId;

			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)dbTrans);

			return (rowsAffected>0);
		}

		#endregion

		public PaymentTransactionInfo GetTransactionInfo(string txnId) {
			SqlCommand cmd = GetLoadTransactionCmd();
			cmd.Parameters["@txnId"].Value = txnId;
			PaymentTransactionInfo pt = null;
			SqlConnection conn = null;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader();
				if (reader.Read()) {
					pt = new PaymentTransactionInfo(true);
					pt.TxnReferenceId	= (string) reader["TxnReferenceId"];
					if (reader["CustomerID"] != DBNull.Value) {
						pt.CustomerID	= (string) reader["CustomerID"];
					} else {
						pt.CustomerID	= string.Empty;
					}
					pt.Amount			= (decimal) reader["TxnAmount"];
					pt.CreatedTS		= (DateTime) reader["TxnDate"];
					short ptType = (short)reader["TxnType"];
					pt.TxnType = (PaymentTransactionType)Enum.Parse(typeof(PaymentTransactionType),ptType.ToString());
					//pt.TxnType			= (PaymentTransactionType) ((ushort) reader["TxnType"]);
					pt.TxnState			= (TransactionState) ((short)reader["TxnState"]);
					pt.PaymentSource.PaymentInstrument = (PaymentInstrument) ((short)reader["PaymentInstrument"]);
					//					pt.PaymentSource.PaymentInstrumentType = (int) reader["PaymentInstrumentType"];
					if (reader["OriginTxnId"] != DBNull.Value)
						pt.OriginTxnId = (string) reader["OriginTxnId"]; 
					else pt.OriginTxnId = "";
	
					if (reader["TxnOriginatedFrom"] != DBNull.Value)
						pt.TxnOriginatedFrom = (TransactionOrigin) ((short)reader["TxnOriginatedFrom"]); 
					else pt.TxnOriginatedFrom = TransactionOrigin.OrderingSiteInternetPayment;

					if (reader["BillingFirstName"] != DBNull.Value)
						pt.BillTo.FirstName = (string) reader["BillingFirstName"]; 
					else pt.BillTo.FirstName = "";

					if (reader["BillingLastName"] != DBNull.Value)
						pt.BillTo.LastName = (string) reader["BillingLastName"]; 
					else pt.BillTo.LastName = "";

					if (reader["BillingContactEmail"] != DBNull.Value)
						pt.BillTo.ContactEmail = (string) reader["BillingContactEmail"]; 
					else pt.BillTo.ContactEmail = "";

					if (reader["BillingAddress1"] != DBNull.Value)
						pt.BillTo.Address.Address1 = (string) reader["BillingAddress1"]; 
					else pt.BillTo.Address.Address1 = "";

					if (reader["BillingAddress2"] != DBNull.Value)
						pt.BillTo.Address.Address2 = (string) reader["BillingAddress2"]; 
					else pt.BillTo.Address.Address2 = "";

					if (reader["BillingCountry"] != DBNull.Value)
						pt.BillTo.Address.Country = (string) reader["BillingCountry"]; 
					else pt.BillTo.Address.Country = "";

					if (reader["BillingState"] != DBNull.Value)
						pt.BillTo.Address.State = (string) reader["BillingState"]; 
					else pt.BillTo.Address.State = "";

					if (reader["BillingZipcode"] != DBNull.Value)
						pt.BillTo.Address.ZipCode = (string) reader["BillingZipcode"]; 
					else pt.BillTo.Address.ZipCode = "";

					if (reader["BillingCity"] != DBNull.Value)
						pt.BillTo.Address.City = (string) reader["BillingCity"]; 
					else pt.BillTo.Address.City = "";

					if (reader["BillingContactPhone"] != DBNull.Value)
						pt.BillTo.ContactPhone = (string) reader["BillingContactPhone"]; 
					else pt.BillTo.ContactPhone = "";

					if (reader["BillingContactFax"] != DBNull.Value)
						pt.BillTo.ContactFax = (string) reader["BillingContactFax"]; 
					else pt.BillTo.ContactFax = "";

					if (reader["BillingCompanyName"] != DBNull.Value)
						pt.BillTo.CompanyName = (string) reader["BillingCompanyName"]; 
					else pt.BillTo.CompanyName = "";
				}
			} catch (Exception ex) {
				throw new DALException("",ex);
			} finally {
				if (!reader.IsClosed) { reader.Close(); }
				if ((conn != null)&&(conn.State!=ConnectionState.Closed)) {
					conn.Close(); 
				}
				cmd.Connection = null;
			}
			return pt;
		}

		
		#region AddCaptureTransaction

		public bool AddCaptureTransaction (PaymentTransactionInfo txnInfo) {
			return AddCaptureTransaction(txnInfo, null, null);	
		}

		public bool AddCaptureTransaction (PaymentTransactionInfo txnInfo,Hashtable amountDistribution) {
			return AddCaptureTransaction(txnInfo, amountDistribution, null);	
		}

		public bool AddCaptureTransaction (PaymentTransactionInfo txnInfo,Hashtable amountDistribution, SqlTransaction transaction) {
			if (txnInfo.TxnType!=PaymentTransactionType.DelayedCapture) {
				throw new ArgumentException("Wrong Transaction Type");
			}
			
			//AddTransaction(txnInfo, transaction);
			bool isSQLTxnParamNull = (transaction==null);
			SqlConnection conn = null;
			if (isSQLTxnParamNull) {
				try {				
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
					transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);

				} catch {
					return false;
				} finally {
					if (transaction!=null) {
						transaction.Dispose();
					}
					if (conn != null) conn.Close();
				}
			}

			try {
				AddTransaction(txnInfo, transaction);
				SetTransactionState(txnInfo.OriginTxnId,TransactionState.Captured, transaction);
				if ((amountDistribution!=null)&&(amountDistribution.Count>0)) {
					SetPaymentAmounts(txnInfo.TxnReferenceId, amountDistribution, transaction);
				}
				//TODO: 1) set captured amount for reference txn ( )
				// 2) change payment amounts according to the amountDistribution (+)
				if (isSQLTxnParamNull)
					transaction.Commit();

				return true;
			} catch /*(Exception ex)*/ {
				//TODO: consider logging
				if (isSQLTxnParamNull)
					transaction.Rollback();
				return false;
			} finally {
				if (isSQLTxnParamNull) {
					if (transaction!=null) {
						transaction.Dispose();
					}
				}
				if (conn != null) conn.Close();
			}
		}


		#endregion

		#region AddVoidTransaction

		public bool AddVoidTransaction (PaymentTransactionInfo txnInfo) {
			return AddVoidTransaction(txnInfo, null);
		}

		public bool AddVoidTransaction (PaymentTransactionInfo txnInfo, IDbTransaction transaction) {

			if (txnInfo.TxnType!=PaymentTransactionType.Void) {
				throw new ArgumentException("Wrong Transaction Type");
			}
			
			//AddTransaction(txnInfo, transaction);
			bool isSQLTxnParamNull = (transaction==null);
			SqlConnection conn = null;

			if (isSQLTxnParamNull) {
				try {				
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
					transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);

				} catch {
					if (isSQLTxnParamNull) {
						if (transaction!=null) {
							transaction.Dispose();
						}
						if (conn != null) conn.Close();
					}
					return false;
				} finally {
					/*if (transaction!=null) {
						transaction.Dispose();
					}
					if (conn != null) conn.Close();*/
				}
			}

			try {
				AddTransaction(txnInfo, transaction);
				SetTransactionState(txnInfo.OriginTxnId,TransactionState.Void, (SqlTransaction)transaction);

				if (isSQLTxnParamNull)
					transaction.Commit();

				return true;
			} catch /*(Exception ex)*/ {
				//TODO: consider logging
				if (isSQLTxnParamNull)
					transaction.Rollback();
				return false;
			} finally {
				if (isSQLTxnParamNull) {
					if (transaction!=null) {
						transaction.Dispose();
					}
				}
				if (conn != null) conn.Close();
			}
			


		} 

		#endregion

		
		public bool AddBaseTransaction (PaymentTransactionInfo txnInfo, IDbTransaction transaction) {
			
			if ((txnInfo.TxnType!=PaymentTransactionType.Sale)&&(txnInfo.TxnType!=PaymentTransactionType.Authrozation)) {
				throw new ArgumentException("Wrong Transaction Type");
			}
			return AddTransaction(txnInfo, transaction);
		}

		#region commented
	
		/*public bool AddCorrectiveTransaction (PaymentTransactionInfo txnInfo) {
            return AddCorrectiveTransaction(txnInfo,null);
		}
 
		public bool AddCorrectiveTransaction (PaymentTransactionInfo txnInfo, SqlTransaction transaction) {
			SqlConnection conn = null;
			if (transaction==null) {
				try {				
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
					transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
					AddTransaction(txnInfo, transaction);
					//FIXME: txnstate - should be set (either in bussines ruiles or facade).
					SetTransactionState(txnInfo.OriginTxnId, txnInfo.TxnState);
					transaction.Commit();
				} catch {
					transaction.Rollback();
					return false;
				} finally {
					if (transaction!=null) {
						transaction.Dispose();
					}
					if (conn != null) conn.Close();
				}
			}

			try {
				//TODO: implement
				throw new NotImplementedException();
			} catch 
				//TODO: consider logging
				transaction.Rollback();
				return false;
			} finally {
				if (transaction!=null) {
					transaction.Dispose();
				}
				if (conn != null) conn.Close();
			}
		}


		*/
		#endregion

		#region AddAuthorizeTransaction
		

		#endregion
		#region AddTransaction
		public bool AddTransaction (PaymentTransactionInfo txnInfo) {
			return AddTransaction(txnInfo, null);
		}


		public bool AddTransaction (PaymentTransactionInfo txnInfo, IDbTransaction transaction) {
			SqlCommand cmd = GetAddTransactionCmd();

			#region params
			cmd.Parameters["@TxnReferenceId"].Value = txnInfo.TxnReferenceId;
			if (txnInfo.CustomerID!=PersistentBusinessEntity.SID_EMPTY) {
				cmd.Parameters["@CustomerID"].Value = txnInfo.CustomerID;
			} else { cmd.Parameters["@CustomerID"].Value = DBNull.Value; }
			cmd.Parameters["@TxnAmount"].Value = txnInfo.Amount;
			cmd.Parameters["@TxnDate"].Value = txnInfo.CreatedTS;
			cmd.Parameters["@TxnType"].Value = txnInfo.TxnType;
			cmd.Parameters["@TxnState"].Value = txnInfo.TxnState;
			cmd.Parameters["@PaymentInstrument"].Value = txnInfo.PaymentSource.PaymentInstrument;
			cmd.Parameters["@PaymentInstrumentType"].Value = txnInfo.PaymentSource.PaymentInstrumentType;
			
			cmd.Parameters["@OriginTxnId"].Value = txnInfo.OriginTxnId;
			cmd.Parameters["@TxnOriginatedFrom"].Value = txnInfo.TxnOriginatedFrom;
			cmd.Parameters["@billingFirstName"].Value = txnInfo.BillTo.FirstName;
			cmd.Parameters["@billingLastName"].Value = txnInfo.BillTo.LastName;
			cmd.Parameters["@billingContactEmail"].Value = txnInfo.BillTo.ContactEmail;
			cmd.Parameters["@billingAddress1"].Value = txnInfo.BillTo.Address.Address1;
			cmd.Parameters["@billingAddress2"].Value = txnInfo.BillTo.Address.Address2;
			cmd.Parameters["@billingCountry"].Value = txnInfo.BillTo.Address.Country;
			cmd.Parameters["@billingState"].Value = txnInfo.BillTo.Address.State;
			cmd.Parameters["@billingZipcode"].Value = txnInfo.BillTo.Address.ZipCode;
			cmd.Parameters["@billingCity"].Value = txnInfo.BillTo.Address.City;
			cmd.Parameters["@billingContactPhone"].Value = txnInfo.BillTo.ContactPhone;
			cmd.Parameters["@billingContactFax"].Value = txnInfo.BillTo.ContactFax;
			cmd.Parameters["@billingCompanyName"].Value = (txnInfo.BillTo.CompanyName==null?DBNull.Value:(object)txnInfo.BillTo.CompanyName);
			#endregion 

			IDbConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = (SqlTransaction)transaction;
				}
				cmd.Connection = (SqlConnection)conn;
				
				int rowsAffected = cmd.ExecuteNonQuery(); 

				foreach(PaymentInfo payment in txnInfo.Payments) {
					AddPayment(payment,(SqlTransaction)transaction);
				}
				
				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}
			} catch(Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
		}

		#endregion

		public bool SetTransactionState (string txnId, TransactionState txnState) {
			return SetTransactionState (txnId, txnState, null);
		}


		public bool SetTransactionState (string txnId, TransactionState txnState, IDbTransaction transaction) {
			SqlCommand cmd = GetSetTransactionStateCmd();

			#region params
			cmd.Parameters["@TxnReferenceId"].Value = txnId;
			cmd.Parameters["@TxnState"].Value = (int) txnState;
			#endregion 

			IDbConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = (SqlTransaction)transaction;
				}
	
				cmd.Connection = (SqlConnection)conn;
				
				int rowsAffected = cmd.ExecuteNonQuery(); 

				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}

			} catch (Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}

		}


		public bool SetTxnCapturedAmount(string txnId, decimal amountCaptured) {
			return SetTxnCapturedAmount (txnId, amountCaptured, null);
		}


		public bool SetTxnCapturedAmount(string txnId, decimal amountCaptured, IDbTransaction transaction) {
			SqlCommand cmd = GetSetCapturedAmountCmd();

			#region params
			cmd.Parameters["@TxnReferenceId"].Value = txnId;
			cmd.Parameters["@amountCaptured"].Value = amountCaptured;
			#endregion 

			IDbConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = (SqlTransaction)transaction;
				}
	
				cmd.Connection = (SqlConnection)conn;
				
				int rowsAffected = cmd.ExecuteNonQuery(); 

				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}

			} catch (Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}

		}

		/// <summary>
		/// Set new payment amounts values.
		/// </summary>
		/// <param name="txnId"></param>
		/// <param name="amountsDistribution">Contains PaymentId/NewAmountToSet pair</param>
		/// <returns></returns>
		internal bool SetPaymentAmounts (string txnId, Hashtable amountsDistribution) {
			return SetPaymentAmounts (txnId, amountsDistribution, null);
		}
		public bool SetPaymentAmounts (string txnId, Hashtable amountsDistribution, IDbTransaction transaction) {
			if (amountsDistribution==null) {
				throw new ArgumentNullException("amountsDistribution");
			}
			if (amountsDistribution.Count==0) {
				throw new ArgumentException("amountsDistribution should contain atleast 1 item","amountsDistribution");
			}

			SqlCommand cmd = GetSetPaymentAmounts();
			
			ParamStringBuilder paramBuilder = new ParamStringBuilder(",");
			paramBuilder.AddParams(amountsDistribution);
			#region params
			cmd.Parameters["@TxnReferenceId"].Value = txnId;
			cmd.Parameters["@paramString"].Value = paramBuilder.ToString();
			#endregion 

			SqlConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = (SqlConnection)(transaction.Connection);
					cmd.Transaction = (SqlTransaction)transaction;
				}
	
				cmd.Connection = conn;
				
				int rowsAffected = cmd.ExecuteNonQuery(); 

				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}

			} catch (Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}

		}
		public bool IsTxnExists (string txnId) {
			SqlCommand cmd = GetCheckTransactionIdCmd();

			cmd.Parameters["@TxnReferenceId"].Value = txnId;
//			cmd.Parameters["@ValidTxn"].Value = ;

			SqlConnection conn = null;

			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();

				cmd.ExecuteNonQuery(); 

				return ((int)cmd.Parameters["@ValidTxn"].Value == 1)?true:false;

			} catch (Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}

		}

		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
