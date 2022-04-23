using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Text;

using HiRes.Common;
namespace HiRes.DAL {
	/// <summary>
	/// 
	/// </summary>
	public class DBHelper	{
		public static int ExecuteSafeNonQueryTransactional(SqlCommand cmd, SqlTransaction transaction) {

			int rowsAffected = 0;

			SqlConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = transaction;
				}
				cmd.Connection = conn;
				
				rowsAffected = cmd.ExecuteNonQuery(); 

				return rowsAffected;

			} catch(Exception ex) {
				return 0;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}

		}
	}
}
