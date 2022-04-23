using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

using System.Security.Cryptography;
using System.Text;

using HiRes.Common;
using HiRes.SystemFramework.Logging;


namespace HiRes.DAL {
	/// <summary>
	/// Summary description for Component1.
	/// </summary>
	public class CustomerDAL : System.ComponentModel.Component {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
	
		#region Constants for SQLCommand parameters
		private const string CustomerId = "@customerID";
		private const string SiteId = "@siteID";
		private const string FirstName = "@firstName";
		private const string LastName = "@lastName";
		private const string ContactEmail = "@contactEmail";
		private const string Passwd = "@passwd";
		private const string Address2 = "@address2";
		private const string Address1 = "@address1";
		private const string CustomerAddressId = "@CustomerAddressId";
		private const string Country = "@country";
		private const string City = "@city";
		private const string State = "@state";
		private const string ZipCode = "@zipcode";
		private const string CompanyName = "@companyName";
		private const string ContactPhone = "@contactPhone";
		private const string ContactPhone2 = "@contactPhone2";
		private const string ContactFax = "@contactFax";
		private const string BillingFirstName = "@billingFirstName";
		private const string BillingLastName = "@billingLastName";
		private const string BillingContactEmail = "@billingContactEmail";
		private const string BillingAddress1 = "@billingAddress1";
		private const string BillingAddress2 = "@billingAddress2";
		private const string BillingCountry = "@billingCountry";
		private const string BillingState = "@billingState";
		private const string BillingZipcode = "@billingZipcode";
		private const string BillingCity = "@billingCity";
		private const string BillingContactPhone = "@billingContactPhone";
		private const string BillingContactFax = "@billingContactFax";
		private const string BillingCompanyName = "@billingCompanyName";
		private const string Industry = "@industry";
		private const string PaymentLevel = "@paymentLevel";

		private const String Filter = "@filterClause";
		private const String Order = "@orderClause";
		#endregion

		#region SQLCommands
		private SqlCommand loadCommand;
		private SqlCommand insertCommand;
		private SqlCommand updateCommand;
		private SqlCommand removeCommand;
		private SqlCommand checkLoginCommand;
		private SqlCommand loadCustomersCommand;
		
		private SqlCommand getCustomerAddressCmd;
		private SqlCommand addCustomerAddressCmd;
		private SqlCommand updateCustomerAddressCmd;
		private SqlCommand removeCustomerAddressCmd;
		private SqlCommand getCustomerAddressesCmd;
		#endregion
		
		#region Constructors
		public CustomerDAL(System.ComponentModel.IContainer container) {
			container.Add(this);
			InitializeComponent();
		}

		public CustomerDAL() {
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
				if (checkLoginCommand != null) 
					checkLoginCommand.Dispose();
				if (loadCustomersCommand != null) 
					loadCustomersCommand.Dispose();

				if (getCustomerAddressCmd != null) 
					getCustomerAddressCmd.Dispose();
				if (addCustomerAddressCmd != null) 
					addCustomerAddressCmd.Dispose();
				if (updateCustomerAddressCmd != null) 
					updateCustomerAddressCmd.Dispose();
				if (removeCustomerAddressCmd != null) 
					removeCustomerAddressCmd.Dispose();
				if (getCustomerAddressesCmd != null) 
					getCustomerAddressesCmd.Dispose();
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters

		/// <summary>
		/// Load customer info command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetLoadCommand() {
			if ( loadCommand == null ) {
				loadCommand = new SqlCommand("HiResAdmin.getCustomerInfo");
				loadCommand.CommandType = CommandType.StoredProcedure;
				loadCommand.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				loadCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return loadCommand;
		}

		/// <summary>
		/// Register new customer command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetInsertCommand() {
			if ( insertCommand == null ) {
				insertCommand = new SqlCommand("HiResAdmin.addCustomer");
				insertCommand.CommandType = CommandType.StoredProcedure;
				insertCommand.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				insertCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				insertCommand.Parameters.Add(new SqlParameter(FirstName, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(LastName, SqlDbType.NVarChar, 70));
				insertCommand.Parameters.Add(new SqlParameter(ContactEmail, SqlDbType.NVarChar, 100));
				insertCommand.Parameters.Add(new SqlParameter(Passwd, SqlDbType.NVarChar, 30));
				insertCommand.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(City, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(State, SqlDbType.NVarChar, 20));
				insertCommand.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.NVarChar, 10));
				insertCommand.Parameters.Add(new SqlParameter(CompanyName, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactPhone, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactPhone2, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactFax, SqlDbType.NVarChar, 50));
				
				insertCommand.Parameters.Add(new SqlParameter(BillingFirstName, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(BillingLastName, SqlDbType.NVarChar, 70));
				insertCommand.Parameters.Add(new SqlParameter(BillingContactEmail, SqlDbType.NVarChar, 100));
				insertCommand.Parameters.Add(new SqlParameter(BillingAddress1, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(BillingAddress2, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(BillingCountry, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(BillingState, SqlDbType.NVarChar, 20));
				insertCommand.Parameters.Add(new SqlParameter(BillingZipcode, SqlDbType.NVarChar, 10));
				insertCommand.Parameters.Add(new SqlParameter(BillingCity, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(BillingContactPhone, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(BillingContactFax, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(BillingCompanyName, SqlDbType.NVarChar, 50));

				insertCommand.Parameters.Add(new SqlParameter(PaymentLevel, SqlDbType.SmallInt));
				insertCommand.Parameters.Add(new SqlParameter(Industry, SqlDbType.VarChar, 50));
			}
			return insertCommand;
		}

		/// <summary>
		/// Update existing customer command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetUpdateCommand() {
			if ( updateCommand == null ) {
				updateCommand = new SqlCommand("HiResAdmin.updateCustomer");
				updateCommand.CommandType = CommandType.StoredProcedure;
				updateCommand.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				updateCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				updateCommand.Parameters.Add(new SqlParameter(FirstName, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(LastName, SqlDbType.NVarChar, 70));
				updateCommand.Parameters.Add(new SqlParameter(ContactEmail, SqlDbType.NVarChar, 100));
				updateCommand.Parameters.Add(new SqlParameter(Passwd, SqlDbType.NVarChar, 30));
				updateCommand.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				updateCommand.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				updateCommand.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(City, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(State, SqlDbType.NVarChar, 20));
				updateCommand.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.NVarChar, 10));
				updateCommand.Parameters.Add(new SqlParameter(CompanyName, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(ContactPhone, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(ContactPhone2, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(ContactFax, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(BillingFirstName, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(BillingLastName, SqlDbType.NVarChar, 70));
				updateCommand.Parameters.Add(new SqlParameter(BillingContactEmail, SqlDbType.NVarChar, 100));
				updateCommand.Parameters.Add(new SqlParameter(BillingAddress1, SqlDbType.NVarChar, 255));
				updateCommand.Parameters.Add(new SqlParameter(BillingAddress2, SqlDbType.NVarChar, 255));
				updateCommand.Parameters.Add(new SqlParameter(BillingCountry, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(BillingState, SqlDbType.NVarChar, 20));
				updateCommand.Parameters.Add(new SqlParameter(BillingZipcode, SqlDbType.NVarChar, 10));
				updateCommand.Parameters.Add(new SqlParameter(BillingCity, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(BillingContactPhone, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(BillingContactFax, SqlDbType.NVarChar, 50));
				updateCommand.Parameters.Add(new SqlParameter(BillingCompanyName, SqlDbType.NVarChar, 50));

				updateCommand.Parameters.Add(new SqlParameter(PaymentLevel, SqlDbType.SmallInt));
				updateCommand.Parameters.Add(new SqlParameter(Industry, SqlDbType.VarChar, 50));

			}
			return updateCommand;
		}

		/// <summary>
		/// Remove customer command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetRemoveCommand() {
			if ( removeCommand== null ) {
				removeCommand = new SqlCommand("HiResAdmin.removeCustomer");
				removeCommand.CommandType = CommandType.StoredProcedure;
				removeCommand.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				removeCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return removeCommand;
		}

		/// <summary>
		/// Check login command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetCheckLoginCommand() {
			if ( checkLoginCommand == null ) {
				checkLoginCommand = new SqlCommand("HiResAdmin.checkLogin");
				checkLoginCommand.CommandType = CommandType.StoredProcedure;
				checkLoginCommand.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				checkLoginCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				checkLoginCommand.Parameters.Add(new SqlParameter(Passwd, SqlDbType.NVarChar, 30));
				checkLoginCommand.Parameters.Add(new SqlParameter("@logged", SqlDbType.Int));
				checkLoginCommand.Parameters["@logged"].Direction = ParameterDirection.Output;
			}
			return checkLoginCommand;
		}

		/// <summary>
		/// Load customers command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetCustomersCommand() {
			if ( loadCustomersCommand == null ) {
				loadCustomersCommand = new SqlCommand("HiResAdmin.getCustomers");
				loadCustomersCommand.CommandType = CommandType.StoredProcedure;
				loadCustomersCommand.Parameters.Add(new SqlParameter(Filter, SqlDbType.NVarChar,1000));
				loadCustomersCommand.Parameters.Add(new SqlParameter(Order, SqlDbType.NVarChar,1000));
			}
			return loadCustomersCommand;
		}

		#region Customer Address SQL Command Getters
		
		private SqlCommand GetCustomerAddressCmd() {
			if ( getCustomerAddressCmd == null ) {
				getCustomerAddressCmd = new SqlCommand("HiResAdmin.getCustomerAddressInfo");
				getCustomerAddressCmd.CommandType = CommandType.StoredProcedure;
				getCustomerAddressCmd.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				getCustomerAddressCmd.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				getCustomerAddressCmd.Parameters.Add(new SqlParameter(CustomerAddressId, SqlDbType.Int));
			}
			return getCustomerAddressCmd;
		}

		private SqlCommand GetAddCustomerAddressCmd() {
			if ( addCustomerAddressCmd == null ) {
				addCustomerAddressCmd = new SqlCommand("HiResAdmin.addCustomerAddress");
				addCustomerAddressCmd.CommandType = CommandType.StoredProcedure;
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(City, SqlDbType.NVarChar, 50));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(State, SqlDbType.NVarChar, 20));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.NVarChar, 10));
				addCustomerAddressCmd.Parameters.Add(new SqlParameter("@customerAddressId", SqlDbType.Int));
				addCustomerAddressCmd.Parameters["@customerAddressId"].Direction = ParameterDirection.Output;
			}
			return addCustomerAddressCmd;
		}

		private SqlCommand GetUpdateCustomerAddressCmd() {
			if ( updateCustomerAddressCmd == null ) {
				updateCustomerAddressCmd = new SqlCommand("HiResAdmin.updateCustomerAddress");
				updateCustomerAddressCmd.CommandType = CommandType.StoredProcedure;
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter("@customerAddressId", SqlDbType.Int));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(City, SqlDbType.NVarChar, 50));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(State, SqlDbType.NVarChar, 20));
				updateCustomerAddressCmd.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.NVarChar, 10));
			}
			return updateCustomerAddressCmd;
		}

		private SqlCommand GetRemoveCustomerAddressCmd() {
			if ( removeCustomerAddressCmd== null ) {
				removeCustomerAddressCmd = new SqlCommand("HiResAdmin.removeCustomerAddress");
				removeCustomerAddressCmd.CommandType = CommandType.StoredProcedure;
				removeCustomerAddressCmd.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar, 30));
				removeCustomerAddressCmd.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				removeCustomerAddressCmd.Parameters.Add(new SqlParameter("@customerAddressId", SqlDbType.Int));
			}
			return removeCustomerAddressCmd;
		}

		private SqlCommand GetCustomerAddressesCmd() {
			if ( getCustomerAddressesCmd == null ) {
				getCustomerAddressesCmd = new SqlCommand("HiResAdmin.getCustomerAddresses");
				getCustomerAddressesCmd.CommandType = CommandType.StoredProcedure;
				getCustomerAddressesCmd.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				getCustomerAddressesCmd.Parameters.Add(new SqlParameter(CustomerId, SqlDbType.NVarChar));
			}
			return getCustomerAddressesCmd;
		}
		#endregion
		#endregion

		#region Get, Add, Update, Remove, Load customers functions
		
		/// <summary>
		/// Load information about customer
		/// </summary>
		/// <param name="customerId">Customer ID</param>
		/// <param name="siteId">Site ID</param>
		/// <returns>Customer info, if user was found and null otherwise.</returns>
		public CustomerInfo GetInfo(String customerId, int siteId) {
			SqlCommand cmd = GetLoadCommand();
			cmd.Parameters[CustomerId].Value = customerId;
			cmd.Parameters[SiteId].Value = siteId;
			CustomerInfo customer = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					customer = new CustomerInfo(siteId);
					customer.CustomerID = (String)reader["CustomerId"];
					if (reader["FirstName"] != DBNull.Value)
						customer.FirstName = (String)reader["FirstName"];
					if (reader["LastName"] != DBNull.Value)
						customer.LastName = (String)reader["LastName"];
					if (reader["ContactEmail"] != DBNull.Value)
						customer.ContactEmail = (String)reader["ContactEmail"];
					customer.PasswdHash = (String)reader["passwd"];
					//customer.Address = new AddressInfo();
					if (reader["Address1"] != DBNull.Value)
						customer.Address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						customer.Address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						customer.Address.Country = (String)reader["Country"];
					if (reader["City"] != DBNull.Value)
						customer.Address.City = (String)reader["City"];
					if (reader["State"] != DBNull.Value)
						customer.Address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						customer.Address.ZipCode = (String)reader["ZipCode"];
					if (reader["companyName"] != DBNull.Value)
						customer.CompanyName = (String)reader["companyName"];
					if (reader["contactPhone"] != DBNull.Value)
						customer.ContactPhone = (String)reader["contactPhone"];
					if (reader["contactPhone2"] != DBNull.Value)
						customer.ContactPhone2 = (String)reader["contactPhone2"];
					if (reader["contactFax"] != DBNull.Value)
						customer.ContactFax = (String)reader["contactFax"];

					if (reader["BillingFirstName"] != DBNull.Value)
						customer.BillingContact.FirstName = (String)reader["BillingFirstName"];
					if (reader["BillingLastName"] != DBNull.Value)
						customer.BillingContact.LastName = (String)reader["BillingLastName"];
					if (reader["BillingContactEmail"] != DBNull.Value)
						customer.BillingContact.ContactEmail = (String)reader["BillingContactEmail"];
					if (reader["BillingAddress1"] != DBNull.Value)
						customer.BillingContact.Address.Address1 = (String)reader["BillingAddress1"];
					if (reader["BillingAddress2"] != DBNull.Value)
						customer.BillingContact.Address.Address2 = (String)reader["BillingAddress2"];
					if (reader["BillingCountry"] != DBNull.Value)
						customer.BillingContact.Address.Country= (String)reader["BillingCountry"];
					if (reader["BillingState"] != DBNull.Value)
						customer.BillingContact.Address.State = (String)reader["BillingState"];
					if (reader["BillingZipcode"] != DBNull.Value)
						customer.BillingContact.Address.ZipCode = (String)reader["BillingZipcode"];
					if (reader["BillingCity"] != DBNull.Value)
						customer.BillingContact.Address.City = (String)reader["BillingCity"];
					if (reader["BillingContactPhone"] != DBNull.Value)
						customer.BillingContact.ContactPhone = (String)reader["BillingContactPhone"];
					if (reader["BillingContactFax"] != DBNull.Value)
						customer.BillingContact.ContactFax = (String)reader["BillingContactFax"];
					if (reader["BillingCompanyName"] != DBNull.Value)
						customer.BillingContact.CompanyName = (String)reader["BillingCompanyName"];
					if (reader["PaymentLevel"] != DBNull.Value)
						customer.PaymentLevel = (short) reader["PaymentLevel"];
					if (reader["Industry"] != DBNull.Value)
						customer.Industry = (String)reader["Industry"];

					//customer.IsNew = false;
				}
				reader.Close();
			} catch (Exception ex) {
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			return customer;
		}
		
		/// <summary>
		/// Register new customer.
		/// </summary>
		/// <param name="ci">Customer information</param>
		/// <returns>true, if customer was succesfully registered, and false otherwise.</returns>
		public bool Insert(CustomerInfo ci) {
			SqlCommand cmd = GetInsertCommand();
			cmd.Parameters[CustomerId].Value = ci.CustomerID;
			cmd.Parameters[SiteId].Value = ci.SiteID;
			cmd.Parameters[FirstName].Value = (ci.FirstName == null) ? DBNull.Value : (Object)ci.FirstName ;
			cmd.Parameters[LastName].Value = (ci.LastName == null) ? DBNull.Value : (Object)ci.LastName;
			cmd.Parameters[ContactEmail].Value = (ci.ContactEmail == null) ? DBNull.Value : (Object)ci.ContactEmail;
			cmd.Parameters[Passwd].Value = ci.PasswdHash;
			cmd.Parameters[Address2].Value = (ci.Address.Address2 == null) ? DBNull.Value : (Object)ci.Address.Address2;
			cmd.Parameters[Address1].Value = (ci.Address.Address1 == null) ? DBNull.Value : (Object)ci.Address.Address1;
			cmd.Parameters[Country].Value = (ci.Address.Country == null) ? DBNull.Value : (Object)ci.Address.Country;
			cmd.Parameters[City].Value = (ci.Address.City == null) ? DBNull.Value : (Object)ci.Address.City;
			cmd.Parameters[State].Value = (ci.Address.State == null) ? DBNull.Value : (Object)ci.Address.State;
			cmd.Parameters[ZipCode].Value = (ci.Address.ZipCode == null) ? DBNull.Value : (Object)ci.Address.ZipCode;
			cmd.Parameters[CompanyName].Value = (ci.CompanyName == null) ? DBNull.Value : (Object)ci.CompanyName; 
			cmd.Parameters[ContactPhone].Value = (ci.ContactPhone == null) ? DBNull.Value : (Object)ci.ContactPhone; 
			cmd.Parameters[ContactPhone2].Value = (ci.ContactPhone2 == null) ? DBNull.Value : (Object)ci.ContactPhone2; 
			cmd.Parameters[ContactFax].Value = (ci.ContactFax == null) ? DBNull.Value : (Object)ci.ContactFax; 
			
			cmd.Parameters[BillingFirstName].Value = (ci.BillingContact.FirstName == null) ? DBNull.Value : (Object)ci.BillingContact.FirstName;
			cmd.Parameters[BillingLastName].Value = (ci.BillingContact.LastName == null) ? DBNull.Value : (Object)ci.BillingContact.LastName;
			cmd.Parameters[BillingContactEmail].Value = (ci.BillingContact.ContactEmail == null) ? DBNull.Value : (Object)ci.BillingContact.ContactEmail;
			cmd.Parameters[BillingAddress1].Value = (ci.BillingContact.Address.Address1 == null) ? DBNull.Value : (Object)ci.BillingContact.Address.Address1;
			cmd.Parameters[BillingAddress2].Value = (ci.BillingContact.Address.Address2 == null) ? DBNull.Value : (Object)ci.BillingContact.Address.Address2;
			cmd.Parameters[BillingCountry].Value = (ci.BillingContact.Address.Country == null) ? DBNull.Value : (Object)ci.BillingContact.Address.Country;
			cmd.Parameters[BillingState].Value = (ci.BillingContact.Address.State == null) ? DBNull.Value : (Object)ci.BillingContact.Address.State;
			cmd.Parameters[BillingZipcode].Value = (ci.BillingContact.Address.ZipCode == null) ? DBNull.Value : (Object)ci.BillingContact.Address.ZipCode;
			cmd.Parameters[BillingCity].Value = (ci.BillingContact.Address.City == null) ? DBNull.Value : (Object)ci.BillingContact.Address.City;
			cmd.Parameters[BillingContactPhone].Value = (ci.BillingContact.ContactPhone == null) ? DBNull.Value : (Object)ci.BillingContact.ContactPhone;
			cmd.Parameters[BillingContactFax].Value = (ci.BillingContact.ContactFax == null) ? DBNull.Value : (Object)ci.BillingContact.ContactFax;
			cmd.Parameters[BillingCompanyName].Value = (ci.BillingContact.CompanyName == null) ? DBNull.Value : (Object)ci.BillingContact.CompanyName;
			cmd.Parameters[PaymentLevel].Value = ci.PaymentLevel;
			cmd.Parameters[Industry].Value = (ci.Industry == null) ? DBNull.Value : (object)ci.Industry;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("Error while registering new customer",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Update information about existing customer.
		/// </summary>
		/// <param name="ci">New customer information</param>
		/// <returns>true, if customer info was succesfully updated, and false if such user not exist.</returns>
		public bool Update(CustomerInfo ci) {
			SqlCommand cmd = GetUpdateCommand();
			cmd.Parameters[CustomerId].Value = ci.CustomerID;
			cmd.Parameters[SiteId].Value = ci.SiteID;
			cmd.Parameters[FirstName].Value = (ci.FirstName == null) ? DBNull.Value : (Object)ci.FirstName ;
			cmd.Parameters[LastName].Value = (ci.LastName == null) ? DBNull.Value : (Object)ci.LastName;
			cmd.Parameters[ContactEmail].Value = (ci.ContactEmail == null) ? DBNull.Value : (Object)ci.ContactEmail;
			cmd.Parameters[Passwd].Value = ci.PasswdHash;
			cmd.Parameters[Address2].Value = (ci.Address.Address2 == null) ? DBNull.Value : (Object)ci.Address.Address2;
			cmd.Parameters[Address1].Value = (ci.Address.Address1 == null) ? DBNull.Value : (Object)ci.Address.Address1;
			cmd.Parameters[Country].Value = (ci.Address.Country == null) ? DBNull.Value : (Object)ci.Address.Country;
			cmd.Parameters[City].Value = (ci.Address.City == null) ? DBNull.Value : (Object)ci.Address.City;
			cmd.Parameters[State].Value = (ci.Address.State == null) ? DBNull.Value : (Object)ci.Address.State;
			cmd.Parameters[ZipCode].Value = (ci.Address.ZipCode == null) ? DBNull.Value : (Object)ci.Address.ZipCode;
			cmd.Parameters[CompanyName].Value = (ci.CompanyName == null) ? DBNull.Value : (Object)ci.CompanyName; 
			cmd.Parameters[ContactPhone].Value = (ci.ContactPhone == null) ? DBNull.Value : (Object)ci.ContactPhone; 
			cmd.Parameters[ContactPhone2].Value = (ci.ContactPhone2 == null) ? DBNull.Value : (Object)ci.ContactPhone2; 
			cmd.Parameters[ContactFax].Value = (ci.ContactFax == null) ? DBNull.Value : (Object)ci.ContactFax; 

			cmd.Parameters[BillingFirstName].Value = (ci.BillingContact.FirstName == null) ? DBNull.Value : (Object)ci.BillingContact.FirstName;
			cmd.Parameters[BillingLastName].Value = (ci.BillingContact.LastName == null) ? DBNull.Value : (Object)ci.BillingContact.LastName;
			cmd.Parameters[BillingContactEmail].Value = (ci.BillingContact.ContactEmail == null) ? DBNull.Value : (Object)ci.BillingContact.ContactEmail;
			cmd.Parameters[BillingAddress1].Value = (ci.BillingContact.Address.Address1 == null) ? DBNull.Value : (Object)ci.BillingContact.Address.Address1;
			cmd.Parameters[BillingAddress2].Value = (ci.BillingContact.Address.Address2 == null) ? DBNull.Value : (Object)ci.BillingContact.Address.Address2;
			cmd.Parameters[BillingCountry].Value = (ci.BillingContact.Address.Country == null) ? DBNull.Value : (Object)ci.BillingContact.Address.Country;
			cmd.Parameters[BillingState].Value = (ci.BillingContact.Address.State == null) ? DBNull.Value : (Object)ci.BillingContact.Address.State;
			cmd.Parameters[BillingZipcode].Value = (ci.BillingContact.Address.ZipCode == null) ? DBNull.Value : (Object)ci.BillingContact.Address.ZipCode;
			cmd.Parameters[BillingCity].Value = (ci.BillingContact.Address.City == null) ? DBNull.Value : (Object)ci.BillingContact.Address.City;
			cmd.Parameters[BillingContactPhone].Value = (ci.BillingContact.ContactPhone == null) ? DBNull.Value : (Object)ci.BillingContact.ContactPhone;
			cmd.Parameters[BillingContactFax].Value = (ci.BillingContact.ContactFax == null) ? DBNull.Value : (Object)ci.BillingContact.ContactFax;
			cmd.Parameters[BillingCompanyName].Value = (ci.BillingContact.CompanyName == null) ? DBNull.Value : (Object)ci.BillingContact.CompanyName;
			cmd.Parameters[PaymentLevel].Value = ci.PaymentLevel;
			cmd.Parameters[Industry].Value = (ci.Industry == null) ? DBNull.Value : (object)ci.Industry;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex)  {
				AppLog.LogError("Error while updating customer info",ex);
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Remove customer from database
		/// </summary>
		/// <param name="ci"></param>
		/// <returns>true, if customer was removed, and false, if customer not exist.</returns>
		public bool Delete(CustomerInfo ci) {
			SqlCommand cmd = GetRemoveCommand();
			cmd.Parameters[CustomerId].Value = ci.CustomerID;
			cmd.Parameters[SiteId].Value = ci.SiteID;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch  {
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Load customers
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="filter"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public CustomerInfo[] GetCustomers(FilterExpression filter, OrderExpression order) {
			SqlCommand cmd = GetCustomersCommand();
//			string filterString = filter.ToString();
//			string orderString = order.ToString();
//			cmd.Parameters[Filter].Value = ((filter == null)||(filterString.Equals(String.Empty)) )? DBNull.Value : (object)filter.ToString();
//			cmd.Parameters[Order].Value = ((order == null)||(orderString.Equals(String.Empty))) ? DBNull.Value : (object)order.ToString();
			cmd.Parameters[Filter].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters[Order].Value = order == null ? "" : order.ToString();
			CustomerInfo[] customers = null;
			ArrayList array = new ArrayList();
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					CustomerInfo customer = new CustomerInfo((int)reader["SiteId"]);
					customer.CustomerID = (String)reader["CustomerId"];
					if (reader["FirstName"] != DBNull.Value)
						customer.FirstName = (String)reader["FirstName"];
					if (reader["LastName"] != DBNull.Value)
						customer.LastName = (String)reader["LastName"];
					if (reader["ContactEmail"] != DBNull.Value)
						customer.ContactEmail = (String)reader["ContactEmail"];
					customer.PasswdHash = (String)reader["passwd"];
					//customer.Address = new AddressInfo();
					if (reader["Address1"] != DBNull.Value)
						customer.Address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						customer.Address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						customer.Address.Country = (String)reader["Country"];
					if (reader["City"] != DBNull.Value)
						customer.Address.City = (String)reader["City"];
					if (reader["State"] != DBNull.Value)
						customer.Address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						customer.Address.ZipCode = (String)reader["ZipCode"];
					if (reader["companyName"] != DBNull.Value)
						customer.CompanyName = (String)reader["companyName"];
					if (reader["contactPhone"] != DBNull.Value)
						customer.ContactPhone = (String)reader["contactPhone"];
					if (reader["contactPhone2"] != DBNull.Value)
						customer.ContactPhone2 = (String)reader["contactPhone2"];
					if (reader["contactFax"] != DBNull.Value)
						customer.ContactFax = (String)reader["contactFax"];
					customer.PaymentLevel = (short) reader["PaymentLevel"];
					if (reader["Industry"] != DBNull.Value)
						customer.Industry = (String)reader["Industry"];
					array.Add(customer);
				}
				reader.Close();
			} catch (Exception ex) {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
			if (array.Count > 0) {
				customers = new CustomerInfo[array.Count];
				array.CopyTo(customers);
			}
			return customers;
		}

		#endregion

		/// <summary>
		/// Check customer existance and his password.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="siteId"></param>
		/// <param name="passwd"></param>
		/// <returns>true, if password accepted, false, if password incorrect, 
		/// throws exception, if customer not exist or any database error occurs.</returns>
		public bool CheckLogin(String customerId, int siteId, String passwd) {
			SqlCommand cmd = GetCheckLoginCommand();
			cmd.Parameters[CustomerId].Value = customerId;
			cmd.Parameters[SiteId].Value = siteId;
			/* compute password hash and
			   transmit hashed password as parameter to SQL procedure */
			cmd.Parameters[Passwd].Value = GetPasswordHash(passwd);
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			int logged = (int)cmd.Parameters["@logged"].Value;
			if ((logged == -2) || (logged == -1)) {
				//throw new Exception("User not exist.");
				return false;
			}

			return (logged == 1);
		}

		private String GetPasswordHash(String pwd) {
			MD5 md5 = new MD5CryptoServiceProvider();
			ASCIIEncoding enc = new ASCIIEncoding();
			byte[] data = enc.GetBytes(pwd);
			data = md5.ComputeHash(data);
			return enc.GetString(data);
		}

		#region Get, Add, Update, Remove, Load CustomerAddresses

		public CustomerAddressInfo GetCustomerAddressInfo(String customerId, int siteId, int customerAddressId) {
			SqlCommand cmd = GetCustomerAddressCmd();
			cmd.Parameters[CustomerId].Value = customerId;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[CustomerAddressId].Value = customerAddressId;
			CustomerAddressInfo address = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					address = new CustomerAddressInfo();
					address.CustomerAddressID = customerAddressId;
					if (reader["Address1"] != DBNull.Value)
						address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						address.Country = (String)reader["Country"];
					if (reader["City"] != DBNull.Value)
						address.City = (String)reader["City"];
					if (reader["State"] != DBNull.Value)
						address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						address.ZipCode = (String)reader["ZipCode"];
				}
				reader.Close();
			} finally {
				if (conn != null) conn.Close();
			}
			return address;
		}
		
		public int AddCustomerAddressInfo(String customerId, int siteId, CustomerAddressInfo address) {
			SqlCommand cmd = GetAddCustomerAddressCmd();
			cmd.Parameters[CustomerId].Value = customerId;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[Address2].Value = (address.Address2 == null) ? DBNull.Value : (Object)address.Address2;
			cmd.Parameters[Address1].Value = (address.Address1 == null) ? DBNull.Value : (Object)address.Address1;
			cmd.Parameters[Country].Value = (address.Country == null) ? DBNull.Value : (Object)address.Country;
			cmd.Parameters[City].Value = (address.City == null) ? DBNull.Value : (Object)address.City;
			cmd.Parameters[State].Value = (address.State == null) ? DBNull.Value : (Object)address.State;
			cmd.Parameters[ZipCode].Value = (address.ZipCode == null) ? DBNull.Value : (Object)address.ZipCode;
			int rowsAffected = 0;
			int id = -1;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				id = (int)cmd.Parameters["@customerAddressId"].Value;
			} finally {
				if (conn != null) conn.Close();
			}
			address.CustomerAddressID = id;
			return id;
		}

		public bool UpdateCustomerAddress(String customerId, int siteId, CustomerAddressInfo address) {
			SqlCommand cmd = GetUpdateCustomerAddressCmd();
			cmd.Parameters[CustomerId].Value = customerId;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[CustomerAddressId].Value = address.CustomerAddressID;
			cmd.Parameters[Address2].Value = (address.Address2 == null) ? DBNull.Value : (Object)address.Address2;
			cmd.Parameters[Address1].Value = (address.Address1 == null) ? DBNull.Value : (Object)address.Address1;
			cmd.Parameters[Country].Value = (address.Country == null) ? DBNull.Value : (Object)address.Country;
			cmd.Parameters[City].Value = (address.City == null) ? DBNull.Value : (Object)address.City;
			cmd.Parameters[State].Value = (address.State == null) ? DBNull.Value : (Object)address.State;
			cmd.Parameters[ZipCode].Value = (address.ZipCode == null) ? DBNull.Value : (Object)address.ZipCode;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		public bool RemoveCustomerAddress(String customerId, int siteId, int customerAddressId) {
			SqlCommand cmd = GetRemoveCustomerAddressCmd();
			cmd.Parameters[CustomerId].Value = customerId;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters["@customerAddressId"].Value = customerAddressId;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		public CustomerAddressInfo[] GetCustomerAddresses(int siteId, String customerId) {
			SqlCommand cmd = GetCustomerAddressesCmd();
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[CustomerId].Value = customerId;
			CustomerAddressInfo[] addresses = null;
			ArrayList array = new ArrayList();
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					CustomerAddressInfo address = new CustomerAddressInfo();
					if (reader["Address1"] != DBNull.Value)
						address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						address.Country = (String)reader["Country"];
					if (reader["City"] != DBNull.Value)
						address.City = (String)reader["City"];
					if (reader["State"] != DBNull.Value)
						address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						address.ZipCode = (String)reader["ZipCode"];
					if (reader["CustomerAddressID"] != DBNull.Value)
						address.CustomerAddressID = (int)reader["CustomerAddressID"];
					array.Add(address);
				}
				reader.Close();
			} catch {
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			if (array.Count > 0) {
				addresses = new CustomerAddressInfo[array.Count];
				array.CopyTo(addresses);
			}
			return addresses;
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
