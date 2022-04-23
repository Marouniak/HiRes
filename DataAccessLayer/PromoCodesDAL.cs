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
	/// Summary description for PromoCodesDAL.
	/// </summary>
	public class PromoCodesDAL : System.ComponentModel.Component {
		
		private const string CustomerId = "@customerID";
		private const string SiteId = "@siteID";

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		#region SQLCommands
		private SqlCommand getCodeInfoCommand;
		private SqlCommand addCodeCommand;
		private SqlCommand removeCodeCommand;
		private SqlCommand setCodeStateCommand;
		private SqlCommand loadPromoCodesCmd;
		private SqlCommand getPromoCodeUsedNumCmd;
		private SqlCommand getUpdateUsageInfoCmd;
		#endregion

		#region Constructors
		public PromoCodesDAL(System.ComponentModel.IContainer container) {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public PromoCodesDAL() {
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
				if (getCodeInfoCommand!= null) 
					getCodeInfoCommand.Dispose();
				if (addCodeCommand != null) 
					addCodeCommand.Dispose();
				if (removeCodeCommand != null) 
					removeCodeCommand.Dispose();
				if (setCodeStateCommand != null)
					setCodeStateCommand.Dispose();
				if (loadPromoCodesCmd != null)
					loadPromoCodesCmd.Dispose();
				if (getPromoCodeUsedNumCmd != null)
					getPromoCodeUsedNumCmd.Dispose();
				if (getUpdateUsageInfoCmd != null)
					getUpdateUsageInfoCmd.Dispose();
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters
		private SqlCommand GetInfoCommand() {
			if ( getCodeInfoCommand == null ) {
				getCodeInfoCommand = new SqlCommand("HiResAdmin.getPromoCode");
				getCodeInfoCommand.CommandType = CommandType.StoredProcedure;
				getCodeInfoCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 64));
			}
			return getCodeInfoCommand;
		}

		private SqlCommand GetAddCodeCommand() {
			if (addCodeCommand == null ) {
				addCodeCommand = new SqlCommand("HiResAdmin.addPromoCode");
				addCodeCommand.CommandType = CommandType.StoredProcedure;
				addCodeCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 64));
				addCodeCommand.Parameters.Add(new SqlParameter("@SiteID", SqlDbType.Int));
				addCodeCommand.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.NVarChar, 30));
				addCodeCommand.Parameters.Add(new SqlParameter("@CreatedTS", SqlDbType.DateTime));
				addCodeCommand.Parameters.Add(new SqlParameter("@CodeType", SqlDbType.Int));
				addCodeCommand.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Decimal));
				addCodeCommand.Parameters.Add(new SqlParameter("@AmountType", SqlDbType.Int));
				addCodeCommand.Parameters.Add(new SqlParameter("@CodeState", SqlDbType.Int));
				addCodeCommand.Parameters.Add(new SqlParameter("@ValidFrom", SqlDbType.DateTime));
				addCodeCommand.Parameters.Add(new SqlParameter("@ValidTo", SqlDbType.DateTime));
				addCodeCommand.Parameters.Add(new SqlParameter("@IssuerId", SqlDbType.VarChar, 20));
				addCodeCommand.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255));
				addCodeCommand.Parameters.Add(new SqlParameter("@IsCooperative", SqlDbType.Bit));
				addCodeCommand.Parameters.Add(new SqlParameter("@IsWholesale", SqlDbType.Bit));

				addCodeCommand.Parameters.Add(new SqlParameter("@MaxUseNumber", SqlDbType.Int));
			}
			return addCodeCommand;
		}

		private SqlCommand GetRemoveCodeCommand() {
			if ( removeCodeCommand == null ) {
				removeCodeCommand = new SqlCommand("HiResAdmin.removePromoCode");
				removeCodeCommand.CommandType = CommandType.StoredProcedure;
				removeCodeCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 64));
			}
			return removeCodeCommand;
		}

		private SqlCommand GetSetPromoCodeStateCommand() {
			if ( setCodeStateCommand == null ) {
				setCodeStateCommand = new SqlCommand("HiResAdmin.updatePromoCodeState");
				setCodeStateCommand.CommandType = CommandType.StoredProcedure;
				setCodeStateCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 64));
				setCodeStateCommand.Parameters.Add(new SqlParameter("@CodeState", SqlDbType.Int));
			}
			return setCodeStateCommand;
		}

		private SqlCommand GetLoadPromoCodesCommand() {
			if ( loadPromoCodesCmd == null ) {
				loadPromoCodesCmd = new SqlCommand("HiResAdmin.getPromoCodes");
				loadPromoCodesCmd.CommandType = CommandType.StoredProcedure;
				loadPromoCodesCmd.Parameters.Add(new SqlParameter("@filterClause", SqlDbType.VarChar, 1000));
				loadPromoCodesCmd.Parameters.Add(new SqlParameter("@orderClause", SqlDbType.VarChar, 1000));
			}
			return loadPromoCodesCmd;
		}

		private SqlCommand GetPromoCodeUsedNumCommand() {
			if ( getPromoCodeUsedNumCmd == null ) {
				getPromoCodeUsedNumCmd = new SqlCommand("HiResAdmin.getPromoCodeUsedNum");
				getPromoCodeUsedNumCmd.CommandType = CommandType.StoredProcedure;
				getPromoCodeUsedNumCmd.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 64));
				getPromoCodeUsedNumCmd.Parameters.Add(new SqlParameter("@SiteId", SqlDbType.Int));
				getPromoCodeUsedNumCmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.NVarChar, 30));
			}
			return getPromoCodeUsedNumCmd;
		}

		private SqlCommand GetUpdateUsageInfoCommand() {
			if ( getUpdateUsageInfoCmd == null ) {
				getUpdateUsageInfoCmd = new SqlCommand("HiResAdmin.updatePromoCodeUsageInfo");
				getUpdateUsageInfoCmd.CommandType = CommandType.StoredProcedure;
				getUpdateUsageInfoCmd.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 64));
				getUpdateUsageInfoCmd.Parameters.Add(new SqlParameter("@SiteId", SqlDbType.Int));
				getUpdateUsageInfoCmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.NVarChar, 30));
				getUpdateUsageInfoCmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int));
			}
			return getUpdateUsageInfoCmd;
		}

		

		#endregion

		public PromoCodeInfo GetCodeInfo(string code) {
			SqlCommand cmd = GetInfoCommand();
			cmd.Parameters["@Code"].Value = code;

			PromoCodeInfo pcInfo = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					pcInfo = new PromoCodeInfo();

					pcInfo.Code			= (string) reader["Code"];
					pcInfo.SiteId		= (int) reader["SiteID"];
					pcInfo.CustomerUID	= (string) reader["CustomerID"];
					pcInfo.CreatedTS	= (DateTime) reader["CreatedTS"];
					pcInfo.CodeType		= (PromoCodeInfo.PromoCodeType) reader["CodeType"];
					pcInfo.Discount		= new DiscountInfo((decimal) reader["Amount"], (DiscountAmountType) reader["AmountType"]);
					if (reader["Description"] != DBNull.Value)
						pcInfo.Description	= (string) reader["Description"];
					pcInfo.CodeState	= (PromoCodeInfo.PromoCodeState) reader["CodeState"];
					if (reader["ValidFrom"] != DBNull.Value)
						pcInfo.ValidFrom	= (DateTime) reader["ValidFrom"];
					if (reader["ValidTo"] != DBNull.Value)
						pcInfo.ValidTo		= (DateTime) reader["ValidTo"];
					if (reader["IssuerId"] != DBNull.Value)
						pcInfo.IssuedBy		= (string) reader["IssuerId"];
					if (reader["Description"] != DBNull.Value)
						pcInfo.Description	= (string) reader["Description"];
					pcInfo.IsCooperative= (bool) reader["IsCooperative"];
					pcInfo.IsWholesale= (bool) reader["IsWholesale"];
					
					pcInfo.UsageConditions.MaxUseNumber	= (int) reader["MaxUseNumber"];
				}
				reader.Close();
			} catch (Exception ex) {
				AppLog.LogError("DAL: Error while getting promotion code info.",ex);
				throw new DALException("DAL: Error while getting promotion code info",ex);
			} finally {
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
			return pcInfo;
		}

		public bool Add(PromoCodeInfo pcInfo) {
			SqlCommand cmd = GetAddCodeCommand();
			cmd.Parameters["@Code"].Value		= pcInfo.Code;
			cmd.Parameters["@SiteID"].Value		= pcInfo.SiteId;
			cmd.Parameters["@CustomerID"].Value = pcInfo.CustomerUID;
			cmd.Parameters["@CreatedTS"].Value	= pcInfo.CreatedTS;
			cmd.Parameters["@CodeType"].Value	= (int) pcInfo.CodeType;
			cmd.Parameters["@Amount"].Value		= pcInfo.Discount.Amount;
			cmd.Parameters["@AmountType"].Value = pcInfo.Discount.AmountType;
			cmd.Parameters["@CodeState"].Value	= (int) pcInfo.CodeState;
			cmd.Parameters["@ValidFrom"].Value	= pcInfo.ValidFrom;
			cmd.Parameters["@ValidTo"].Value	= pcInfo.ValidTo;
			if (pcInfo.IssuedBy == String.Empty)
				cmd.Parameters["@IssuerId"].Value	= DBNull.Value;
			else
				cmd.Parameters["@IssuerId"].Value	= pcInfo.IssuedBy;

			if (pcInfo.Description == String.Empty)
				cmd.Parameters["@Description"].Value = DBNull.Value;
			else
				cmd.Parameters["@Description"].Value = pcInfo.Description;

			cmd.Parameters["@IsCooperative"].Value = pcInfo.IsCooperative;
			cmd.Parameters["@IsWholesale"].Value = pcInfo.IsWholesale;
			
			cmd.Parameters["@MaxUseNumber"].Value = pcInfo.UsageConditions.MaxUseNumber;

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

		public void Delete(string code) {
			if ((code==null)||(code.Equals(String.Empty))) {
				throw new ArgumentNullException("code");
			}

			SqlCommand cmd = GetRemoveCodeCommand();
			cmd.Parameters["@Code"].Value = code;

			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("DAL: Error while deleting promotion code.",ex);
				throw new DALException("DAL: Error while adding promotion code",ex);//return false;
			} finally {
				if (conn != null) conn.Close();
			}
			//return (rowsAffected > 0);
		}

		public void SetCodeState(string code, PromoCodeInfo.PromoCodeState cs) {

			if ((code==null)||(code.Equals(String.Empty))) {
				throw new ArgumentNullException("code");
			}

			SqlCommand cmd = GetSetPromoCodeStateCommand();
			cmd.Parameters["@Code"].Value = code;
			cmd.Parameters["@CodeState"].Value = (int) cs;

			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("DAL: Error while deleting promotion code.",ex);
				throw new DALException("DAL: Error while adding promotion code",ex);
				//return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return;// (rowsAffected > 0);
		}

		public PromoCodeInfo[] GetPromoCodes(FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadPromoCodesCommand();
			cmd.Parameters["@filterClause"].Value = filter  == null ? "" : filter.ToString();;
			cmd.Parameters["@orderClause"].Value  = orderBy == null ? "" : orderBy.ToString();

			ArrayList codesList = new ArrayList();
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();

				while(reader.Read()) {
					PromoCodeInfo pcInfo = new PromoCodeInfo();
					
					pcInfo.Code			= (string) reader["Code"];
					pcInfo.SiteId		= (int) reader["SiteID"];
					pcInfo.CustomerUID	= (string) reader["CustomerID"];
					pcInfo.CreatedTS	= (DateTime) reader["CreatedTS"];
					pcInfo.CodeType		= (PromoCodeInfo.PromoCodeType) reader["CodeType"];
					pcInfo.Discount		= new DiscountInfo((decimal) reader["Amount"], (DiscountAmountType) reader["AmountType"]);
					if (reader["Description"] != DBNull.Value)
						pcInfo.Description	= (string) reader["Description"];
					pcInfo.CodeState	= (PromoCodeInfo.PromoCodeState) reader["CodeState"];
					if (reader["ValidFrom"] != DBNull.Value)
						pcInfo.ValidFrom	= (DateTime) reader["ValidFrom"];
					if (reader["ValidTo"] != DBNull.Value)
						pcInfo.ValidTo		= (DateTime) reader["ValidTo"];
					if (reader["IssuerId"] != DBNull.Value)
						pcInfo.IssuedBy		= (string) reader["IssuerId"];
					if (reader["Description"] != DBNull.Value)
						pcInfo.Description	= (string) reader["Description"];
					pcInfo.IsCooperative = (bool) reader["IsCooperative"];
					pcInfo.IsWholesale = (bool) reader["IsWholesale"];
					pcInfo.UsageConditions.MaxUseNumber	= (int) reader["MaxUseNumber"];
					codesList.Add(pcInfo);
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
	
			PromoCodeInfo[] codes = new PromoCodeInfo[codesList.Count];
			codesList.CopyTo(codes);
			return codes;
		}


		#region code usage

		public int GetTimesUsedByCustomer(string code, int siteId, string CustomerId) {
			SqlCommand cmd = GetPromoCodeUsedNumCommand();
			cmd.Parameters["@Code"].Value = code;
			cmd.Parameters["@SiteId"].Value = siteId;
			cmd.Parameters["@CustomerId"].Value = CustomerId;

			SqlConnection conn = null;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					return (int) reader["UsedNum"];
				} else 
					return 0;
			} catch  {
				return -1;
			} finally {
				if (reader != null) reader.Close();
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
		}

		public void AddCodeUsage(string code, int siteId, string CustomerId, int orderId) {
			AddCodeUsage(code,siteId,CustomerId, orderId, null);
		}
		
		public void AddCodeUsage(string code, int siteId, string CustomerId, int orderId, SqlTransaction transaction) {

			SqlCommand cmd = GetUpdateUsageInfoCommand();

			cmd.Parameters["@Code"].Value		= code;
			cmd.Parameters["@SiteID"].Value		= siteId;
			cmd.Parameters["@CustomerID"].Value = CustomerId;
			cmd.Parameters["@OrderId"].Value	= orderId;

			SqlConnection conn = null;

			int rowsAffected = 0;
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
				/*if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}*/
			} catch(Exception ex) {
				AppLog.LogError("DAL: Error while updating promotion code info.",ex);
				throw new DALException("DAL: Error while updating promotion code info",ex);
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}

/*			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("DAL: Error while updating promotion code info.",ex);
				throw new DALException("DAL: Error while updating promotion code info",ex);
			} finally {
				if (conn != null) conn.Close();
			}*/
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
