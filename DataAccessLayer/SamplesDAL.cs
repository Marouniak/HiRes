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
	/// Summary description for SamplesDAL.
	/// </summary>
	public class SamplesDAL : System.ComponentModel.Component {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Constants for SQLCommand parameters

		private const string SampleRequestId = "@sampleRequestId";
		private const string SiteId = "@siteID";
		private const string SelectedPrintingTypes = "@selectedPrintingTypes";
		private const string Status = "@status";
		private const string PaymentLevel = "@paymentLevel";
		private const string FirstName = "@firstName";
		private const string LastName = "@lastName";
		private const string ContactEmail = "@contactEmail";
		private const string Address2 = "@address2";
		private const string Address1 = "@address1";
		private const string Country = "@country";
		private const string City = "@city";
		private const string State = "@state";
		private const string ZipCode = "@zipcode";
		private const string CompanyName = "@companyName";
		private const string ContactPhone = "@contactPhone";
		private const string ContactPhone2 = "@contactPhone2";
		private const string ContactFax = "@contactFax";
		private const string Industry = "@industry";
		private const string PlacedTS = "@placedTS";

		private const String Filter = "@filterClause";
		private const String Order = "@orderClause";
		#endregion

		#region SQLCommands
		private SqlCommand loadCommand;
		private SqlCommand insertCommand;
		private SqlCommand updateCommand;
		private SqlCommand removeCommand;
		private SqlCommand loadSampleRequestsCommand;

	
		#endregion

		#region Constructors
		public SamplesDAL(System.ComponentModel.IContainer container) {
			container.Add(this);
			InitializeComponent();
		}

		public SamplesDAL()	{
			InitializeComponent();
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
				if (loadCommand != null) 
					loadCommand.Dispose();
				if (insertCommand != null) 
					insertCommand.Dispose();
				if (updateCommand != null) 
					updateCommand.Dispose();
				if (removeCommand != null) 
					removeCommand.Dispose();
				if (loadSampleRequestsCommand != null)
					loadSampleRequestsCommand.Dispose();
			} 
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters
		/// <summary>
		/// Load sample request command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetLoadCommand() {
			if ( loadCommand == null ) {
				loadCommand = new SqlCommand("HiResAdmin.getSampleRequest");
				loadCommand.CommandType = CommandType.StoredProcedure;
				loadCommand.Parameters.Add(new SqlParameter(SampleRequestId, SqlDbType.Int));
				loadCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return loadCommand;
		}


		/// <summary>
		/// Add sample request command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetInsertCommand() {
			if ( insertCommand == null ) {
				insertCommand = new SqlCommand("HiResAdmin.addSampleRequest");
				insertCommand.CommandType = CommandType.StoredProcedure;
				insertCommand.Parameters.Add(new SqlParameter(SampleRequestId, SqlDbType.Int));
				insertCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				insertCommand.Parameters.Add(new SqlParameter(SelectedPrintingTypes, SqlDbType.Char, 255));
				insertCommand.Parameters.Add(new SqlParameter(Status, SqlDbType.SmallInt));
				insertCommand.Parameters.Add(new SqlParameter(PaymentLevel, SqlDbType.SmallInt));
				insertCommand.Parameters.Add(new SqlParameter(FirstName, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(LastName, SqlDbType.NVarChar, 70));
				insertCommand.Parameters.Add(new SqlParameter(ContactEmail, SqlDbType.NVarChar, 100));
				insertCommand.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(City, SqlDbType.VarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(State, SqlDbType.VarChar, 20));
				insertCommand.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.VarChar, 10));
				insertCommand.Parameters.Add(new SqlParameter(ContactPhone, SqlDbType.VarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactPhone2, SqlDbType.VarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactFax, SqlDbType.VarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(CompanyName, SqlDbType.VarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(Industry, SqlDbType.VarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(PlacedTS, SqlDbType.DateTime));
				insertCommand.Parameters[SampleRequestId].Direction = ParameterDirection.Output;
			}
			return insertCommand;
		}


		/// <summary>
		/// Update existing sample request command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetUpdateCommand() {
			if ( updateCommand == null ) {
				updateCommand = new SqlCommand("HiResAdmin.updateSampleRequest");
				updateCommand.CommandType = CommandType.StoredProcedure;
				updateCommand.Parameters.Add(new SqlParameter(SampleRequestId, SqlDbType.Int));
				updateCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				updateCommand.Parameters.Add(new SqlParameter(SelectedPrintingTypes, SqlDbType.Char, 255));
				updateCommand.Parameters.Add(new SqlParameter(Status, SqlDbType.SmallInt));
				updateCommand.Parameters.Add(new SqlParameter(PaymentLevel, SqlDbType.SmallInt));
				updateCommand.Parameters.Add(new SqlParameter(FirstName, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(LastName, SqlDbType.NVarChar, 70));
				updateCommand.Parameters.Add(new SqlParameter(ContactEmail, SqlDbType.NVarChar, 100));
				updateCommand.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				updateCommand.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				updateCommand.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(City, SqlDbType.VarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(State, SqlDbType.VarChar, 20));
				updateCommand.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.VarChar, 10));
				updateCommand.Parameters.Add(new SqlParameter(ContactPhone, SqlDbType.VarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(ContactPhone2, SqlDbType.VarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(ContactFax, SqlDbType.VarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(CompanyName, SqlDbType.VarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(Industry, SqlDbType.VarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(PlacedTS, SqlDbType.DateTime));
			}
			return updateCommand;
		}

		
		/// <summary>
		/// Remove sample request command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetRemoveCommand() 
		{
			if ( removeCommand== null ) {
				removeCommand = new SqlCommand("HiResAdmin.removeSampleRequest");
				removeCommand.CommandType = CommandType.StoredProcedure;
				removeCommand.Parameters.Add(new SqlParameter(SampleRequestId, SqlDbType.Int));
				removeCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return removeCommand;
		}

		private SqlCommand GetSampleRequestsCommand() {
			if ( loadSampleRequestsCommand == null ) {
				loadSampleRequestsCommand = new SqlCommand("HiResAdmin.getSampleRequests");
				loadSampleRequestsCommand.CommandType = CommandType.StoredProcedure;
				loadSampleRequestsCommand.Parameters.Add(new SqlParameter(Filter, SqlDbType.NVarChar,1000));
				loadSampleRequestsCommand.Parameters.Add(new SqlParameter(Order, SqlDbType.NVarChar,1000));
			}
			return loadSampleRequestsCommand;
		}

		#endregion

		#region Get, Add, Update, Remove sample request functions
		
		/// <summary>
		/// Load information about sample request
		/// </summary>
		/// <param name="sampleRequestId">Sample Request ID</param>
		/// <param name="siteId">Site ID</param>
		/// <returns>Sample request info, if it was found and null otherwise.</returns>
		public SampleRequestInfo GetInfo(int sampleRequestID, int siteId) {
			SqlCommand cmd = GetLoadCommand();
			cmd.Parameters[SampleRequestId].Value = sampleRequestID;
			cmd.Parameters[SiteId].Value = siteId;
			SampleRequestInfo sample = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					sample = new SampleRequestInfo();
					if (reader["SelectedPrintingTypes"] != DBNull.Value)
						sample.SetPrintingsFromString((string)reader["SelectedPrintingTypes"]);
					if (reader["Status"] != DBNull.Value)
						sample.Status = (SampleRequestState)((short) reader["Status"]);
					if (reader["PaymentLevel"] != DBNull.Value)
						sample.PaymentLevel = (PaymentLevel)((short) reader["PaymentLevel"]);

					if (reader["FirstName"] != DBNull.Value)
						sample.Contact.FirstName = (String)reader["FirstName"];
					if (reader["LastName"] != DBNull.Value)
						sample.Contact.LastName = (String)reader["LastName"];
					if (reader["ContactEmail"] != DBNull.Value)
						sample.Contact.ContactEmail = (String)reader["ContactEmail"];
					if (reader["Address1"] != DBNull.Value)
						sample.Contact.Address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						sample.Contact.Address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						sample.Contact.Address.Country = (String)reader["Country"];
					if (reader["State"] != DBNull.Value)
						sample.Contact.Address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						sample.Contact.Address.ZipCode = (String)reader["ZipCode"];
					if (reader["City"] != DBNull.Value)
						sample.Contact.Address.City = (String)reader["City"];
					if (reader["companyName"] != DBNull.Value)
						sample.Contact.CompanyName = (String)reader["companyName"];
					if (reader["contactPhone"] != DBNull.Value)
						sample.Contact.ContactPhone = (String)reader["contactPhone"];
					if (reader["contactPhone2"] != DBNull.Value)
						sample.Contact.ContactPhone2 = (String)reader["contactPhone2"];
					if (reader["contactFax"] != DBNull.Value)
						sample.Contact.ContactFax = (String)reader["contactFax"];
					if (reader["Industry"] != DBNull.Value)
						sample.Industry = (String)reader["Industry"];
					if (reader["PlacedTS"] != DBNull.Value)
						sample.PlacedTS = (DateTime)reader["PlacedTS"];
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
			return sample;
		}
		

		/// <summary>
		/// Register new sample request
		/// </summary>
		/// <param name="sr">Sample request information</param>
		/// <returns>true, if sample request was succesfully registered, and false otherwise.</returns>
		public bool Insert(ref SampleRequestInfo sr, int siteId) {
			SqlCommand cmd = GetInsertCommand();
			cmd.Parameters[SampleRequestId].Value = sr.SampleRequestId;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[SelectedPrintingTypes].Value = (sr.SelectedPrintingTypes == null) ? DBNull.Value : (Object)sr.GetStringFromPrintings();
			cmd.Parameters[Status].Value = (short)sr.Status;
			cmd.Parameters[PaymentLevel].Value = (short)sr.PaymentLevel;
			cmd.Parameters[FirstName].Value = (sr.Contact.FirstName == null) ? DBNull.Value : (Object)sr.Contact.FirstName ;
			cmd.Parameters[LastName].Value = (sr.Contact.LastName == null) ? DBNull.Value : (Object)sr.Contact.LastName;
			cmd.Parameters[ContactEmail].Value = (sr.Contact.ContactEmail == null) ? DBNull.Value : (Object)sr.Contact.ContactEmail;
			cmd.Parameters[Address2].Value = (sr.Contact.Address.Address2 == null) ? DBNull.Value : (Object)sr.Contact.Address.Address2;
			cmd.Parameters[Address1].Value = (sr.Contact.Address.Address1 == null) ? DBNull.Value : (Object)sr.Contact.Address.Address1;
			cmd.Parameters[Country].Value = (sr.Contact.Address.Country == null) ? DBNull.Value : (Object)sr.Contact.Address.Country;
			cmd.Parameters[City].Value = (sr.Contact.Address.City == null) ? DBNull.Value : (Object)sr.Contact.Address.City;
			cmd.Parameters[State].Value = (sr.Contact.Address.State == null) ? DBNull.Value : (Object)sr.Contact.Address.State;
			cmd.Parameters[ZipCode].Value = (sr.Contact.Address.ZipCode == null) ? DBNull.Value : (Object)sr.Contact.Address.ZipCode;
			cmd.Parameters[ContactPhone].Value = (sr.Contact.ContactPhone == null) ? DBNull.Value : (Object)sr.Contact.ContactPhone; 
			cmd.Parameters[ContactPhone2].Value = (sr.Contact.ContactPhone2 == null) ? DBNull.Value : (Object)sr.Contact.ContactPhone2; 
			cmd.Parameters[ContactFax].Value = (sr.Contact.ContactFax == null) ? DBNull.Value : (Object)sr.Contact.ContactFax; 
			cmd.Parameters[CompanyName].Value = (sr.Contact.CompanyName == null) ? DBNull.Value : (Object)sr.Contact.CompanyName; 
			cmd.Parameters[Industry].Value = (sr.Industry == null) ? DBNull.Value : (object)sr.Industry;
			cmd.Parameters[PlacedTS].Value = sr.PlacedTS;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				sr.SampleRequestId = (int)cmd.Parameters[SampleRequestId].Value;
			} catch	(Exception ex){
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		/// <summary>
		/// Update information about sample request
		/// </summary>
		/// <param name="sr">New sample request</param>
		/// <returns>true, if sample request info was succesfully updated, and false if sample request not exist.</returns>
		public bool Update(SampleRequestInfo sr, int siteId) {
			SqlCommand cmd = GetUpdateCommand();
			cmd.Parameters[SampleRequestId].Value = sr.SampleRequestId;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[SelectedPrintingTypes].Value = (sr.SelectedPrintingTypes == null) ? DBNull.Value : (Object)sr.GetStringFromPrintings();
			cmd.Parameters[Status].Value = (short)sr.Status;
			cmd.Parameters[PaymentLevel].Value = (short)sr.PaymentLevel;
			cmd.Parameters[FirstName].Value = (sr.Contact.FirstName == null) ? DBNull.Value : (Object)sr.Contact.FirstName ;
			cmd.Parameters[LastName].Value = (sr.Contact.LastName == null) ? DBNull.Value : (Object)sr.Contact.LastName;
			cmd.Parameters[ContactEmail].Value = (sr.Contact.ContactEmail == null) ? DBNull.Value : (Object)sr.Contact.ContactEmail;
			cmd.Parameters[Address2].Value = (sr.Contact.Address.Address2 == null) ? DBNull.Value : (Object)sr.Contact.Address.Address2;
			cmd.Parameters[Address1].Value = (sr.Contact.Address.Address1 == null) ? DBNull.Value : (Object)sr.Contact.Address.Address1;
			cmd.Parameters[Country].Value = (sr.Contact.Address.Country == null) ? DBNull.Value : (Object)sr.Contact.Address.Country;
			cmd.Parameters[City].Value = (sr.Contact.Address.City == null) ? DBNull.Value : (Object)sr.Contact.Address.City;
			cmd.Parameters[State].Value = (sr.Contact.Address.State == null) ? DBNull.Value : (Object)sr.Contact.Address.State;
			cmd.Parameters[ZipCode].Value = (sr.Contact.Address.ZipCode == null) ? DBNull.Value : (Object)sr.Contact.Address.ZipCode;
			cmd.Parameters[ContactPhone].Value = (sr.Contact.ContactPhone == null) ? DBNull.Value : (Object)sr.Contact.ContactPhone; 
			cmd.Parameters[ContactPhone2].Value = (sr.Contact.ContactPhone2 == null) ? DBNull.Value : (Object)sr.Contact.ContactPhone2; 
			cmd.Parameters[ContactFax].Value = (sr.Contact.ContactFax == null) ? DBNull.Value : (Object)sr.Contact.ContactFax; 
			cmd.Parameters[CompanyName].Value = (sr.Contact.CompanyName == null) ? DBNull.Value : (Object)sr.Contact.CompanyName; 
			cmd.Parameters[Industry].Value = (sr.Industry == null) ? DBNull.Value : (Object)sr.Industry;
			cmd.Parameters[PlacedTS].Value = sr.PlacedTS;

			int rowsAffected = 0;
			SqlConnection conn = null;
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


		/// <summary>
		/// Remove sample request from database
		/// </summary>
		/// <param name="sr"></param>
		/// <returns>true, if sample request was removed, and false, if sample request not exist.</returns>
		public bool Delete(int sampleRequestID, int siteID) {
			SqlCommand cmd = GetRemoveCommand();
			cmd.Parameters[SampleRequestId].Value = sampleRequestID;
			cmd.Parameters[SiteId].Value = siteID;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		
		/// <summary>
		/// Load sample requests
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="filter"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public SampleRequestInfo[] GetSampleRequests(FilterExpression filter, OrderExpression order) {
			SqlCommand cmd = GetSampleRequestsCommand();
			cmd.Parameters[Filter].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters[Order].Value = order == null ? "" : order.ToString();
			//AppLog.LogError(order.ToString());
			SampleRequestInfo[] samplesList = null;
			ArrayList array = new ArrayList();

			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					SampleRequestInfo sample = new SampleRequestInfo();
					sample.SampleRequestId = (int)reader["SampleRequestID"];
					if (reader["SelectedPrintingTypes"] != DBNull.Value)
						sample.SetPrintingsFromString((string)reader["SelectedPrintingTypes"]);
					if (reader["Status"] != DBNull.Value)
						sample.Status = (SampleRequestState)((short) reader["Status"]);
					if (reader["PaymentLevel"] != DBNull.Value)
						sample.PaymentLevel = (PaymentLevel)((short) reader["PaymentLevel"]);
					if (reader["FirstName"] != DBNull.Value)
						sample.Contact.FirstName = (String)reader["FirstName"];
					if (reader["LastName"] != DBNull.Value)
						sample.Contact.LastName = (String)reader["LastName"];
					if (reader["ContactEmail"] != DBNull.Value)
						sample.Contact.ContactEmail = (String)reader["ContactEmail"];
					if (reader["Address1"] != DBNull.Value)
						sample.Contact.Address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						sample.Contact.Address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						sample.Contact.Address.Country = (String)reader["Country"];
					if (reader["State"] != DBNull.Value)
						sample.Contact.Address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						sample.Contact.Address.ZipCode = (String)reader["ZipCode"];
					if (reader["City"] != DBNull.Value)
						sample.Contact.Address.City = (String)reader["City"];
					if (reader["companyName"] != DBNull.Value)
						sample.Contact.CompanyName = (String)reader["companyName"];
					if (reader["contactPhone"] != DBNull.Value)
						sample.Contact.ContactPhone = (String)reader["contactPhone"];
					if (reader["contactPhone2"] != DBNull.Value)
						sample.Contact.ContactPhone2 = (String)reader["contactPhone2"];
					if (reader["contactFax"] != DBNull.Value)
						sample.Contact.ContactFax = (String)reader["contactFax"];
					if (reader["Industry"] != DBNull.Value)
						sample.Industry = (String)reader["Industry"];
					if (reader["PlacedTS"] != DBNull.Value)
						sample.PlacedTS = (DateTime)reader["PlacedTS"];

					array.Add(sample);
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
			if (array.Count > 0) {
				samplesList = new SampleRequestInfo[array.Count];
				array.CopyTo(samplesList);
			}
			return samplesList;
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
