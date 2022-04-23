using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

using System.Data;
using System.Data.SqlClient;

using HiRes.Common;
using HiRes.SystemFramework.Logging;

namespace HiRes.DAL {
	/// <summary>
	/// Summary description for MailingListDAL.
	/// </summary>
	public class MailingListDAL : System.ComponentModel.Component {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region const
		const string PMT_MailingListId = "@mailingListId";
		const string PMT_OrderId = "@orderId";
		const string PMT_ListCost = "@listCost";
		const string PMT_ListQuantity = "@listQuantity";
		const string PMT_FileName = "@fileName";
		const string PMT_FileContentType = "@fileContentType";
		const string PMT_FileBlob = "@mailingList";

		const string FLD_MailingListId = "MailingListId";
		const string FLD_OrderId ="OrderId";
		const string FLD_ListCost = "ListCost";
		const string FLD_ListQuantity = "ListQuantity";
		const string FLD_FileName = "FileName";
		const string FLD_FileContentType = "FileContentType";
		const string FLD_FileBlob = "MailingList";

		#endregion

		#region Private Members: SQL Commands
		
		private SqlCommand loadMailingListsCmd;
		private SqlCommand loadMailingInfoCmd;
		private SqlCommand removeMailListCmd;
		private SqlCommand addMailListCmd;
		private SqlCommand updateMailListCmd;

		#endregion

		#region Constructors
		public MailingListDAL(System.ComponentModel.IContainer container) {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public MailingListDAL() {
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
				if (loadMailingListsCmd != null)
					loadMailingListsCmd.Dispose();
				if (loadMailingInfoCmd != null)
					loadMailingInfoCmd.Dispose();
				if (removeMailListCmd != null)
					removeMailListCmd.Dispose();
				if (addMailListCmd != null)
					addMailListCmd.Dispose();
				if (updateMailListCmd != null)
					updateMailListCmd.Dispose();
				
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters
		private SqlCommand GetLoadMailingListsCmd() {
			if ( loadMailingListsCmd == null ) {
				loadMailingListsCmd = new SqlCommand("HiResAdmin.GetMailingLists");
				loadMailingListsCmd.CommandType = CommandType.StoredProcedure;
				loadMailingListsCmd.Parameters.Add(new SqlParameter(PMT_OrderId, SqlDbType.Int));
			}
			return loadMailingListsCmd;
		}

		private SqlCommand GetLoadMailingInfoCmd() {
			if ( loadMailingInfoCmd == null ) {
				loadMailingInfoCmd = new SqlCommand("HiResAdmin.GetMailingListInfo");
				loadMailingInfoCmd.CommandType = CommandType.StoredProcedure;
				loadMailingInfoCmd.Parameters.Add(new SqlParameter(PMT_MailingListId, SqlDbType.Int));
			}
			return loadMailingInfoCmd;
		}

		private SqlCommand GetRemoveMailListCmd() {
			if ( removeMailListCmd == null ) {
				removeMailListCmd = new SqlCommand("HiResAdmin.RemoveMailingList");
				removeMailListCmd.CommandType = CommandType.StoredProcedure;
				removeMailListCmd.Parameters.Add(new SqlParameter(PMT_MailingListId, SqlDbType.Int));
			}
			return removeMailListCmd;
		}

		private SqlCommand GetAddMailListCmd() {
			if ( addMailListCmd == null ) {
				addMailListCmd = new SqlCommand("HiResAdmin.AddMailingList");
				addMailListCmd.CommandType = CommandType.StoredProcedure;
				addMailListCmd.Parameters.Add(new SqlParameter(PMT_OrderId, SqlDbType.Int));
				addMailListCmd.Parameters.Add(new SqlParameter(PMT_ListCost, SqlDbType.SmallMoney));
				addMailListCmd.Parameters.Add(new SqlParameter(PMT_ListQuantity, SqlDbType.Int));
				addMailListCmd.Parameters.Add(new SqlParameter(PMT_FileName, SqlDbType.NVarChar,255));
				addMailListCmd.Parameters.Add(new SqlParameter(PMT_FileContentType, SqlDbType.VarChar,50));
				addMailListCmd.Parameters.Add(new SqlParameter(PMT_FileBlob, SqlDbType.Image));

				addMailListCmd.Parameters.Add(new SqlParameter(PMT_MailingListId, SqlDbType.Int));
				addMailListCmd.Parameters[PMT_MailingListId].Direction = ParameterDirection.Output;
			}
			return addMailListCmd;
		}

		private SqlCommand GetUpdateMailListDataCmd() {
			if ( updateMailListCmd == null ) {
				updateMailListCmd = new SqlCommand("HiResAdmin.UpdateMailingListData");
				updateMailListCmd.CommandType = CommandType.StoredProcedure;
				updateMailListCmd.Parameters.Add(new SqlParameter(PMT_OrderId, SqlDbType.Int));
				updateMailListCmd.Parameters.Add(new SqlParameter(PMT_MailingListId, SqlDbType.Int));
				updateMailListCmd.Parameters.Add(new SqlParameter(PMT_ListCost, SqlDbType.SmallMoney));
				updateMailListCmd.Parameters.Add(new SqlParameter(PMT_ListQuantity, SqlDbType.Int));
			}
			return updateMailListCmd;
		}
		#endregion

		#region MailingList public methods

		public /*MailingListInfo[]*/ ArrayList GetMailingLists(int orderId) {
			SqlDataReader reader = null;
			SqlCommand cmd = GetLoadMailingListsCmd();
			cmd.Parameters[PMT_OrderId].Value = orderId;

			//MailingListInfo[] lists = null;
			SqlConnection conn = null;
			ArrayList array = new ArrayList();
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader();
				while(reader.Read()) {
					MailingListInfo mailingListInfo = new MailingListInfo();
					
					mailingListInfo.MailingListId = (int)reader[FLD_MailingListId];
					mailingListInfo.OrderId = orderId;
					mailingListInfo.ListCost = (decimal)reader[FLD_ListCost];
					mailingListInfo.ListQuantity = (int)reader[FLD_ListQuantity];
					if (reader[FLD_FileName] != DBNull.Value)
						mailingListInfo.FileName = (string)reader[FLD_FileName];
					if (reader[FLD_FileContentType] != DBNull.Value)
						mailingListInfo.FileContentType = (string)reader[FLD_FileContentType];
					
					array.Add(mailingListInfo);
				}
				reader.Close();
			} finally {
				if ((reader!=null)&&(!reader.IsClosed)) { reader.Close(); }
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			/*if (array.Count != 0) {
				lists = new MailingListInfo[array.Count];
				array.CopyTo(lists);
			}*/
			return array;

		}

		public MailingListInfo GetInfo(int mailingListId, bool loadFile) {
			SqlDataReader reader = null;
			SqlCommand cmd = GetLoadMailingInfoCmd();
			cmd.Parameters[PMT_MailingListId].Value = mailingListId;
			MailingListInfo mi = null;

			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					mi = new MailingListInfo();
					if (loadFile) {
						mi.MailingListBlob = (byte[]) reader[FLD_FileBlob];
					}
					mi.MailingListId = (int)reader[FLD_MailingListId];
					mi.OrderId = (int)reader[FLD_OrderId];
					mi.ListCost = (decimal)reader[FLD_ListCost];
					mi.ListQuantity = (int)reader[FLD_ListQuantity];
					if (reader[FLD_FileName] != DBNull.Value)
						mi.FileName = (string)reader[FLD_FileName];
					if (reader[FLD_FileContentType] != DBNull.Value)
						mi.FileContentType = (string)reader[FLD_FileContentType];

				}
				reader.Close();
			} catch (Exception ex) {
				return null;
			} finally {
				if ((reader!=null)&&(!reader.IsClosed)) { reader.Close(); }
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}

			return mi;
		}



		public bool Add(MailingListInfo listInfo, out int mailinglistId) {
			SqlCommand cmd = GetAddMailListCmd();
			/*cmd.Parameters[PMT_MailingListId].Value	= listInfo.MailingListId;

			cmd.Parameters[PMT_MailingListId].Value	= listInfo.MailingListId;*/
			cmd.Parameters[PMT_OrderId].Value	= listInfo.OrderId;
			cmd.Parameters[PMT_ListCost].Value	= listInfo.ListCost;
			cmd.Parameters[PMT_ListQuantity].Value	= listInfo.ListQuantity;
			/*cmd.Parameters[PMT_FileName].Value	= listInfo.FileName;
			cmd.Parameters[PMT_FileContentType].Value	= listInfo.FileContentType;
			cmd.Parameters[PMT_FileBlob].Value	= listInfo.MailingListBlob;*/

			if ((listInfo.FileName == null)||(listInfo.FileName.Length==0))
				cmd.Parameters[PMT_FileName].Value	= DBNull.Value;
			else
				cmd.Parameters[PMT_FileName].Value	= listInfo.FileName;

			if ((listInfo.FileContentType == null))
				cmd.Parameters[PMT_FileContentType].Value	= DBNull.Value;
			else
				cmd.Parameters[PMT_FileContentType].Value	= listInfo.FileContentType;

			if ((listInfo.MailingListBlob == null))
				cmd.Parameters[PMT_FileBlob].Value	= DBNull.Value;
			else
				cmd.Parameters[PMT_FileBlob].Value	= listInfo.MailingListBlob;

			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				mailinglistId = (int)cmd.Parameters[PMT_MailingListId].Value;
			} catch (Exception ex) {
				AppLog.LogError("DAL: Error while adding promotion code.",ex);
				mailinglistId = PersistentBusinessEntity.ID_EMPTY;
				return false;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}
		public bool UpdateListData(MailingListInfo listInfo) {
			SqlCommand cmd = GetUpdateMailListDataCmd();
			cmd.Parameters[PMT_MailingListId].Value	= listInfo.MailingListId;

			cmd.Parameters[PMT_MailingListId].Value	= listInfo.MailingListId;
			cmd.Parameters[PMT_OrderId].Value	= listInfo.OrderId;
			cmd.Parameters[PMT_ListCost].Value	= listInfo.ListCost;
			cmd.Parameters[PMT_ListQuantity].Value	= listInfo.ListQuantity;


			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("DAL: Error while adding promotion code.",ex);
				return false;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		public bool Remove(int mailingListId) {
			SqlCommand cmd = GetRemoveMailListCmd();
			cmd.Parameters[PMT_MailingListId].Value = mailingListId;
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
