using System;
using System.Data;
using System.Data.SqlClient;

using HiRes.Common;
using HiRes.SystemFramework.Logging;

namespace HiRes.DAL {
	public class DbTransactionFactoryException : Exception {
		public DbTransactionFactoryException() : base() {}
		public DbTransactionFactoryException(string desc, Exception ex) : base(desc,ex) {}
	}
	/// <summary>
	/// Auxiliary class intended for use by business layer to create db transaction.
	/// </summary>
	/// <remarks>Using this class in business layer will allow easier changing of backend database</remarks>
	public class DbTransactionFactory {
		public static IDbTransaction BeginTransaction(){
			return BeginTransaction(AppConfig.dbConnString);
		}
		public static IDbTransaction BeginTransaction(string connString) {
			try {
				SqlConnection conn = new SqlConnection(connString);
				conn.Open();
				IDbTransaction transaction =  conn.BeginTransaction();
				
				return transaction;
			} catch (Exception ex) {
				throw new DbTransactionFactoryException("Exception occured during the transaction starting",ex);
			}
		}
	}
}
