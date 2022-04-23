using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using HiRes.Common;

namespace HiRes.DAL {
	/// <summary>
	/// Summary description for PackagingDAL.
	/// </summary>
	public class PackagingDAL : System.ComponentModel.Component	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private const string PMT_OrderID = "@orderID";

		#region Private Members: SQL Commands
		private SqlCommand loadPackagingsCmd;
		private SqlCommand loadPackagingInfoCmd;
		private SqlCommand addPackagingInfoCmd;
		private SqlCommand updatePackagingInfoCmd;

		private SqlCommand loadOrderShippedPackagesCmd;
		private SqlCommand loadShippedPackageInfoCmd;
		private SqlCommand addShippedPackageInfoCmd;
		
		private SqlCommand storePackageLabelCmd;
		private SqlCommand getPackageLabelCmd;
		#endregion

		#region Constructors
		public PackagingDAL(System.ComponentModel.IContainer container)
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

		public PackagingDAL()
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
				if (loadPackagingInfoCmd != null) 
					loadPackagingInfoCmd.Dispose();
				if (addPackagingInfoCmd != null)
					addPackagingInfoCmd.Dispose();
				if (updatePackagingInfoCmd != null)
					updatePackagingInfoCmd.Dispose();
				if (loadPackagingsCmd != null)
					loadPackagingsCmd.Dispose();

				if (loadOrderShippedPackagesCmd != null)
					loadOrderShippedPackagesCmd.Dispose();
				if (loadShippedPackageInfoCmd != null)
					loadShippedPackageInfoCmd.Dispose();
				if (addShippedPackageInfoCmd != null)
					addShippedPackageInfoCmd.Dispose();

				if (storePackageLabelCmd != null)
					storePackageLabelCmd.Dispose();
				if (getPackageLabelCmd != null)
					getPackageLabelCmd.Dispose();
				
			} finally {
				base.Dispose(disposing);
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

		#region SQL Command Getters
		private SqlCommand GetLoadPackagingInfoCommand() {
			if ( loadPackagingInfoCmd == null ) {
				loadPackagingInfoCmd = new SqlCommand("HiResAdmin.getPackagingInfo");
				loadPackagingInfoCmd.CommandType = CommandType.StoredProcedure;
				loadPackagingInfoCmd.Parameters.Add(new SqlParameter("@printingTypeId", SqlDbType.Int));
				loadPackagingInfoCmd.Parameters.Add(new SqlParameter("@paperSizeId", SqlDbType.Int));
				loadPackagingInfoCmd.Parameters.Add(new SqlParameter("@paperTypeId", SqlDbType.Int));
				loadPackagingInfoCmd.Parameters.Add(new SqlParameter("@carrierId", SqlDbType.Int));
				loadPackagingInfoCmd.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Int));
			}
			return loadPackagingInfoCmd;
		}

		private SqlCommand GetUpdatePackagingInfoCommand() {
			if ( updatePackagingInfoCmd == null ) {
				updatePackagingInfoCmd = new SqlCommand("HiResAdmin.updatePackagingInfo");
				updatePackagingInfoCmd.CommandType = CommandType.StoredProcedure;
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@PrintingTypeId", SqlDbType.Int));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@PaperSizeId", SqlDbType.Int));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@PaperTypeId", SqlDbType.Int));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@CarrierId", SqlDbType.Int));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@Height", SqlDbType.Decimal));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@Width", SqlDbType.Decimal));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@Length", SqlDbType.Decimal));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@CarrierPackagingTypeID", SqlDbType.VarChar, 20));
				updatePackagingInfoCmd.Parameters.Add(new SqlParameter("@BoxesNumber", SqlDbType.Int));
			}
			return updatePackagingInfoCmd;
		}

		private SqlCommand GetAddPackagingInfoCommand() {
			if ( addPackagingInfoCmd == null ) {
				addPackagingInfoCmd = new SqlCommand("HiResAdmin.addPackagingInfo");
				addPackagingInfoCmd.CommandType = CommandType.StoredProcedure;
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@PrintingTypeId", SqlDbType.Int));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@PaperSizeId", SqlDbType.Int));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@PaperTypeId", SqlDbType.Int));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@CarrierId", SqlDbType.Int));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@Height", SqlDbType.Decimal));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@Width", SqlDbType.Decimal));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@Length", SqlDbType.Decimal));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@CarrierPackagingTypeID", SqlDbType.VarChar, 20));
				addPackagingInfoCmd.Parameters.Add(new SqlParameter("@BoxesNumber", SqlDbType.Int));
			}
			return addPackagingInfoCmd;
		}

		private SqlCommand GetLoadPackagingsCommand() {
			if ( loadPackagingsCmd == null ) {
				loadPackagingsCmd = new SqlCommand("HiResAdmin.getPackagings");
				loadPackagingsCmd.CommandType = CommandType.StoredProcedure;
				loadPackagingsCmd.Parameters.Add(new SqlParameter("@filterClause", SqlDbType.VarChar, 1000));
				loadPackagingsCmd.Parameters.Add(new SqlParameter("@orderClause", SqlDbType.VarChar, 1000));
			}
			return loadPackagingsCmd;
		}

		private SqlCommand GetLoadOrderShippedPackagesCmd() {
			if ( loadOrderShippedPackagesCmd == null ) {
				loadOrderShippedPackagesCmd = new SqlCommand("HiResAdmin.GetShippedPackages");
				loadOrderShippedPackagesCmd.CommandType = CommandType.StoredProcedure;
				loadOrderShippedPackagesCmd.Parameters.Add(new SqlParameter(PMT_OrderID, SqlDbType.Int));
			}
			return loadOrderShippedPackagesCmd;
		}

		private SqlCommand GetLoadShippedPackageInfoCmd() {
			if ( loadShippedPackageInfoCmd == null ) {
				loadShippedPackageInfoCmd = new SqlCommand("HiResAdmin.GetShippedPackageInfo");
				loadShippedPackageInfoCmd.CommandType = CommandType.StoredProcedure;
				loadShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@ShippedPackageId", SqlDbType.Int));
			}
			return loadShippedPackageInfoCmd;
		}

		private SqlCommand GetAddShippedPackageInfoCmd() {
			if ( addShippedPackageInfoCmd == null ) {
				addShippedPackageInfoCmd = new SqlCommand("HiResAdmin.AddShippedPackage");
				addShippedPackageInfoCmd.CommandType = CommandType.StoredProcedure;
				
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter(PMT_OrderID, SqlDbType.Int));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@CarrierID", SqlDbType.Int));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@ShippingMethodID", SqlDbType.VarChar,20));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@Height", SqlDbType.Decimal));

				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@Width", SqlDbType.Decimal));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@Length", SqlDbType.Decimal));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@CarrierPackagingTypeID", SqlDbType.VarChar, 20));
				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@TrackingId", SqlDbType.VarChar, 30));

				addShippedPackageInfoCmd.Parameters.Add(new SqlParameter("@ShippedPackageId", SqlDbType.Int));
				addShippedPackageInfoCmd.Parameters["@ShippedPackageId"].Direction = ParameterDirection.Output;

			}
			return addShippedPackageInfoCmd;
		}

		private SqlCommand GetLoadPackageLabelCmd() {
			if ( getPackageLabelCmd == null ) {
				getPackageLabelCmd = new SqlCommand("HiResAdmin.GetPackageLabel");
				getPackageLabelCmd.CommandType = CommandType.StoredProcedure;
				getPackageLabelCmd.Parameters.Add(new SqlParameter("@ShippedPackageId", SqlDbType.Int));
			}
			return getPackageLabelCmd;
		}

		private SqlCommand GetStorePackageLabelCmd() {
			if ( storePackageLabelCmd == null ) {
				storePackageLabelCmd = new SqlCommand("HiResAdmin.StorePackageLabel");
				storePackageLabelCmd.CommandType = CommandType.StoredProcedure;
				storePackageLabelCmd.Parameters.Add(new SqlParameter("@ShippedPackageId", SqlDbType.Int));
				storePackageLabelCmd.Parameters.Add(new SqlParameter("@label", SqlDbType.VarBinary,7000));
			}
			return storePackageLabelCmd;
		}
		#endregion		


		public PackagingInfo GetPackagingInfo(int printingTypeId, int paperSizeId, 
			int paperTypeId, PostalCarrier carrier, int quantity) {
			SqlCommand cmd = GetLoadPackagingInfoCommand();
			cmd.Parameters["@printingTypeId"].Value = printingTypeId;
			cmd.Parameters["@paperSizeId"].Value = paperSizeId;
			cmd.Parameters["@paperTypeId"].Value = paperTypeId;
			cmd.Parameters["@carrierId"].Value = carrier;
			cmd.Parameters["@quantity"].Value = quantity;
			PackagingInfo packaging = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					packaging = new PackagingInfo();
					packaging.Carrier = carrier;
					packaging.PaperSizeId = paperSizeId;
					packaging.PaperTypeId = printingTypeId;
					packaging.PrintingTypeId = printingTypeId;
					packaging.Quantity = quantity;

					packaging.Height = (decimal)reader["Height"];
					packaging.Width  = (decimal)reader["Width"];
					packaging.Length = (decimal)reader["Length"];
					packaging.Weight = (decimal)reader["Weight"];
					if (reader["CarrierPackagingTypeID"] != DBNull.Value) 
						packaging.CarrierPackageTypeId = (String)reader["CarrierPackagingTypeID"];
					packaging.BoxesNumber = (int)reader["BoxesNumber"];
				}
				reader.Close();
			} catch (Exception e){
				throw e;
			} finally {
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
			return packaging;
		}


		public bool AddPackagingInfo(PackagingInfo packaging) {
			SqlCommand cmd = GetAddPackagingInfoCommand();
			cmd.Parameters["@PrintingTypeId"].Value = packaging.PrintingTypeId;
			cmd.Parameters["@PaperSizeId"].Value = packaging.PaperSizeId;
			cmd.Parameters["@PaperTypeId"].Value = packaging.PaperTypeId;
			cmd.Parameters["@CarrierId"].Value = (int)packaging.Carrier;
			cmd.Parameters["@Quantity"].Value = packaging.Quantity;
			cmd.Parameters["@Height"].Value = packaging.Height;
			cmd.Parameters["@Width"].Value = packaging.Width;
			cmd.Parameters["@Length"].Value = packaging.Length;
			cmd.Parameters["@Weight"].Value = packaging.Weight;
			if (packaging.CarrierPackageTypeId == String.Empty)
				cmd.Parameters["@CarrierPackagingTypeID"].Value = DBNull.Value;
			else
				cmd.Parameters["@CarrierPackagingTypeID"].Value = packaging.CarrierPackageTypeId;
			cmd.Parameters["@BoxesNumber"].Value = packaging.BoxesNumber;

			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public bool UpdatePackagingInfo(PackagingInfo packaging) {
			SqlCommand cmd = GetUpdatePackagingInfoCommand();
			cmd.Parameters["@PrintingTypeId"].Value = packaging.PrintingTypeId;
			cmd.Parameters["@PaperSizeId"].Value = packaging.PaperSizeId;
			cmd.Parameters["@PaperTypeId"].Value = packaging.PaperTypeId;
			cmd.Parameters["@CarrierId"].Value = (int)packaging.Carrier;
			cmd.Parameters["@Quantity"].Value = packaging.Quantity;
			cmd.Parameters["@Height"].Value = packaging.Height;
			cmd.Parameters["@Width"].Value = packaging.Width;
			cmd.Parameters["@Length"].Value = packaging.Length;
			cmd.Parameters["@Weight"].Value = packaging.Weight;
			if (packaging.CarrierPackageTypeId == String.Empty)
				cmd.Parameters["@CarrierPackagingTypeID"].Value = DBNull.Value;
			else
				cmd.Parameters["@CarrierPackagingTypeID"].Value = packaging.CarrierPackageTypeId;
			cmd.Parameters["@BoxesNumber"].Value = packaging.BoxesNumber;

			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		public /*PackagingInfo[]*/ ArrayList GetPackagings(FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadPackagingsCommand();
			cmd.Parameters["@filterClause"].Value = filter  == null ? "" : filter.ToString();;
			cmd.Parameters["@orderClause"].Value  = orderBy == null ? "" : orderBy.ToString();

			ArrayList packagingList = new ArrayList();
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();

				while(reader.Read()) {
					PackagingInfo pi = new PackagingInfo();
					pi.PrintingTypeId = (int)reader["PrintingTypeId"];
					pi.PaperTypeId = (int)reader["PaperTypeId"];
					pi.PaperSizeId = (int)reader["PaperSizeId"];
					pi.Carrier     = (PostalCarrier)reader["CarrierId"];
					pi.Quantity    = (int)reader["Quantity"];
					pi.PaperTypeId = (int)reader["PaperTypeId"];
					pi.Height      = (decimal)reader["Height"];
					pi.Width       = (decimal)reader["Width"];
					pi.Length      = (decimal)reader["Length"];
					pi.Weight      = (decimal)reader["Weight"];
					if (reader["CarrierPackagingTypeID"] != DBNull.Value) 
						pi.CarrierPackageTypeId = (String)reader["CarrierPackagingTypeID"];
					pi.BoxesNumber = (int)reader["BoxesNumber"];
					packagingList.Add(pi);
				}
				reader.Close();
				return packagingList;
			} catch {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
	
			/*PackagingInfo[] packagings = new PackagingInfo[packagingList.Count];
			for (int i=0;i<packagingList.Count;i++)
				packagings[i] = (PackagingInfo) packagingList[i];

			return packagings;*/
		}

		public ShippedPackageInfo GetShippedPackageInfo(int shippedPackageId) {
			SqlCommand cmd = GetLoadShippedPackageInfoCmd();
			cmd.Parameters["@ShippedPackageId"].Value = shippedPackageId;

			ShippedPackageInfo packageInfo = null;
			SqlConnection conn = null;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					packageInfo = new ShippedPackageInfo();
					packageInfo.ShippedPackageId = (int)reader["ShippedPackageId"];
					packageInfo.Carrier = (PostalCarrier)reader["CarrierID"];
					packageInfo.ShipMethod = (string)reader["ShippingMethodID"];
					packageInfo.TrackingId = (string)reader["TrackingId"];

					packageInfo.OrderId = (int)reader["OrderId"];
					packageInfo.Height = (decimal)reader["Height"];
					packageInfo.Width  = (decimal)reader["Width"];
					packageInfo.Length = (decimal)reader["Length"];
					packageInfo.Weight = (decimal)reader["Weight"];
					if (reader["CarrierPackagingTypeID"] != DBNull.Value) 
						packageInfo.CarrierPackageTypeId = (string)reader["CarrierPackagingTypeID"];
					
				}
				return packageInfo;
			} catch (Exception e){
				throw e;
			} finally {
				if (!reader.IsClosed)
					reader.Close();
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
			
		}

		public ArrayList GetOrderShippedPackages(int orderId) {
			SqlCommand cmd = GetLoadOrderShippedPackagesCmd();
			cmd.Parameters[PMT_OrderID].Value = orderId;

			ArrayList packagesList = new ArrayList();
			SqlConnection conn = null;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader();

				while(reader.Read()) {
					ShippedPackageInfo pi = new ShippedPackageInfo();
					pi.Carrier     = (PostalCarrier)reader["CarrierId"];
					pi.ShipMethod  = (string)reader["ShippingMethodID"];
					pi.Height      = (decimal)reader["Height"];
					pi.Width       = (decimal)reader["Width"];
					pi.Length      = (decimal)reader["Length"];
					pi.Weight      = (decimal)reader["Weight"];

					if (reader["CarrierPackagingTypeID"] != DBNull.Value) 
						pi.CarrierPackageTypeId = (String)reader["CarrierPackagingTypeID"];

					pi.OrderId      = orderId;//(int)reader["OrderId"];
					pi.ShippedPackageId      = (int)reader["ShippedPackageId"];
					pi.TrackingId      = (string)reader["TrackingId"];

					packagesList.Add(pi);
				}
				//reader.Close();
				return packagesList;
			} catch (Exception ex) {
				//return packagesList;
				throw new DALException("Error getting order packages",ex);
			} finally {
				if (!reader.IsClosed) { reader.Close(); }
				if (conn != null) conn.Close();
			}
	
		}


		public bool AddShippedPackageInfo(ref ShippedPackageInfo pi, IDbTransaction transaction) {
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			SqlCommand cmd = GetAddShippedPackageInfoCmd();
			//SqlConnection conn = null;
			int rowsAffected = 0;
			//bool res = true;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = (SqlConnection)transaction.Connection;
					cmd.Transaction = (SqlTransaction)transaction;
					
				}

				cmd.Parameters[PMT_OrderID].Value = pi.OrderId;
				cmd.Parameters["@CarrierID"].Value = (int)pi.Carrier;
				cmd.Parameters["@ShippingMethodID"].Value = pi.ShipMethod;
								
				cmd.Parameters["@Height"].Value = pi.Height;
				cmd.Parameters["@Width"].Value = pi.Width;
				cmd.Parameters["@Length"].Value = pi.Length;
				cmd.Parameters["@Weight"].Value = pi.Weight;
				if (!pi.IsCarrierPackaging)
					cmd.Parameters["@CarrierPackagingTypeID"].Value = DBNull.Value;
				else
					cmd.Parameters["@CarrierPackagingTypeID"].Value = pi.CarrierPackageTypeId;
				//cmd.Parameters["@BoxesNumber"].Value = packaging.BoxesNumber;

				cmd.Parameters["@TrackingId"].Value = pi.TrackingId;


				/*conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();*/
				rowsAffected = cmd.ExecuteNonQuery();
				pi.ShippedPackageId = (int)cmd.Parameters["@ShippedPackageId"].Value;
				return (rowsAffected > 0);
			} catch (Exception ex){
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}
		}

		public byte[] GetPackageLabel(int shippedPackageId) {
			SqlCommand cmd = GetLoadPackageLabelCmd();
			cmd.Parameters["@ShippedPackageId"].Value = shippedPackageId;

			SqlConnection conn = null;
			
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				object obj = cmd.ExecuteScalar();
				if (obj.Equals(DBNull.Value)) {
					return new byte[0];
				}
				byte[] res = (byte[])obj;
				//SqlDataReader reader = cmdExecuteReader(CommandBehavior.SingleRow);
				//if(reader.Read()) {
				//}
				return res;
				//reader.Close();

			} catch (Exception ex) {
				return new byte[0];
			} finally {
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}

		}

		public bool StorePackageLabel(int shippedPackageId, byte[] label) {
			SqlCommand cmd = GetStorePackageLabelCmd();
			cmd.Parameters["@ShippedPackageId"].Value = shippedPackageId;
			cmd.Parameters["@label"].Value = label;

			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex) {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
		}
		
		public bool StorePackageLabel(int shippedPackageId, byte[] label, IDbTransaction transaction) {
			SqlCommand cmd = GetStorePackageLabelCmd();
			cmd.Parameters["@ShippedPackageId"].Value = shippedPackageId;
			cmd.Parameters["@label"].Value = label;
			
			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			return (rowsAffected > 0);
		}

	}
}
