using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using HiRes.Common;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

using HiRes.SystemFramework.Logging;

namespace HiRes.DAL {
	/// <summary>
	/// Summary description for EmployeeDAL.
	/// </summary>
	public class EmployeeDAL : System.ComponentModel.Component {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Constants for SQLCommand parameters
		private const string UID = "@UID";
		private const string FirstName = "@firstName";
		private const string LastName = "@lastName";
		private const string ContactEmail = "@contactEmail";
		private const string Passwd = "@passwd";
		private const string Roles = "@roles";
		private const string AccountBlocked = "@accountBlocked";

		private const string Address1 = "@address1";
		private const string Address2 = "@address2";
		private const string Country = "@country";
		private const string City = "@city";
		private const string State = "@state";
		private const string ZipCode = "@zipcode";
		private const string CompanyName = "@companyName";
		private const string ContactPhone = "@contactPhone";
		private const string ContactPhone2 = "@contactPhone2";
		private const string ContactFax = "@contactFax";
		private const string HourlyRate = "@hourlyRate";

		private const string Filter = "@filterClause";
		private const string Order = "@orderClause";
		#endregion

		#region SQLCommands
		private SqlCommand insertCommand;
		private SqlCommand removeCommand;
		private SqlCommand updateInfoCommand;
		private SqlCommand updatePwdCommand;
		private SqlCommand loadCommand;
		private SqlCommand loadEmployeesCommand;
		private SqlCommand checkLoginCommand;
		#endregion

		#region Constructors
		public EmployeeDAL(System.ComponentModel.IContainer container) {
			container.Add(this);
			InitializeComponent();
		}
		public EmployeeDAL() {
			InitializeComponent();
		}
		#endregion

		#region Destructors
		public new void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
			//GC.SuppressFinalize(true);
		}

		protected new virtual void Dispose(bool disposing) {
			
			if (! disposing)
				return; // we're being collected, so let the GC take care of this object
			try {
				if (loadCommand != null) 
					loadCommand.Dispose();
				if (insertCommand != null) 
					insertCommand.Dispose();
				if (updateInfoCommand != null) 
					updateInfoCommand.Dispose();
				if (updatePwdCommand != null) 
					updatePwdCommand.Dispose();
				if (removeCommand != null) 
					removeCommand.Dispose();
				if (loadEmployeesCommand != null) 
					loadEmployeesCommand.Dispose();
				if (checkLoginCommand != null) 
					checkLoginCommand.Dispose();
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters
		/// <summary>
		/// Register new employee command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetInsertCommand() {
			if ( insertCommand == null ) {
				insertCommand = new SqlCommand("HiResAdmin.addEmployee");
				insertCommand.CommandType = CommandType.StoredProcedure;
				insertCommand.Parameters.Add(new SqlParameter(UID, SqlDbType.NVarChar, 20));
				insertCommand.Parameters.Add(new SqlParameter(FirstName, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(LastName, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactEmail, SqlDbType.NVarChar, 100));
				insertCommand.Parameters.Add(new SqlParameter(Passwd, SqlDbType.NVarChar, 30));
				insertCommand.Parameters.Add(new SqlParameter(Roles, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(AccountBlocked, SqlDbType.Bit));

				insertCommand.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				insertCommand.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(City, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(State, SqlDbType.NVarChar, 20));
				insertCommand.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.NVarChar, 10));
				insertCommand.Parameters.Add(new SqlParameter(ContactPhone, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactPhone2, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(ContactFax, SqlDbType.NVarChar, 50));
				insertCommand.Parameters.Add(new SqlParameter(HourlyRate, SqlDbType.SmallMoney));
			}
			return insertCommand;
		}

		/// <summary>
		/// Remove employee command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetRemoveCommand() {
			if ( removeCommand== null ) {
				removeCommand = new SqlCommand("HiResAdmin.removeEmployee");
				removeCommand.CommandType = CommandType.StoredProcedure;
				removeCommand.Parameters.Add(new SqlParameter(UID, SqlDbType.NVarChar, 20));
			}
			return removeCommand;
		}

		/// <summary>
		/// Update existing employee command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetUpdateInfoCommand() {
			if ( updateInfoCommand == null ) {
				updateInfoCommand = new SqlCommand("HiResAdmin.updateEmployeeInfo");
				updateInfoCommand.CommandType = CommandType.StoredProcedure;
				updateInfoCommand.Parameters.Add(new SqlParameter(UID, SqlDbType.NVarChar, 20));
				updateInfoCommand.Parameters.Add(new SqlParameter(FirstName, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(LastName, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(ContactEmail, SqlDbType.NVarChar, 100));
				updateInfoCommand.Parameters.Add(new SqlParameter(Roles, SqlDbType.NVarChar, 255));
				updateInfoCommand.Parameters.Add(new SqlParameter(AccountBlocked, SqlDbType.Bit));

				updateInfoCommand.Parameters.Add(new SqlParameter(Address2, SqlDbType.NVarChar, 255));
				updateInfoCommand.Parameters.Add(new SqlParameter(Address1, SqlDbType.NVarChar, 255));
				updateInfoCommand.Parameters.Add(new SqlParameter(Country, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(City, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(State, SqlDbType.NVarChar, 20));
				updateInfoCommand.Parameters.Add(new SqlParameter(ZipCode, SqlDbType.NVarChar, 10));
				updateInfoCommand.Parameters.Add(new SqlParameter(ContactPhone, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(ContactPhone2, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(ContactFax, SqlDbType.NVarChar, 50));
				updateInfoCommand.Parameters.Add(new SqlParameter(HourlyRate, SqlDbType.SmallMoney));
			}
			return updateInfoCommand;
		}

		/// <summary>
		/// Update existing employee password
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetUpdatePwdCommand() {
			if ( updatePwdCommand == null ) {
				updatePwdCommand = new SqlCommand("HiResAdmin.updateEmployeePassword");
				updatePwdCommand.CommandType = CommandType.StoredProcedure;
				updatePwdCommand.Parameters.Add(new SqlParameter(UID, SqlDbType.NVarChar, 20));
				updatePwdCommand.Parameters.Add(new SqlParameter(Passwd, SqlDbType.NVarChar, 30));
			}
			return updatePwdCommand;
		}

		/// <summary>
		/// Load employee info command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetLoadCommand() {
			if ( loadCommand == null ) {
				loadCommand = new SqlCommand("HiResAdmin.GetEmployeeInfo");
				loadCommand.CommandType = CommandType.StoredProcedure;
				loadCommand.Parameters.Add(new SqlParameter(UID, SqlDbType.NVarChar, 20));
			}
			return loadCommand;
		}

		/// <summary>
		/// Load employees command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetEmployeesCommand() {
			if ( loadEmployeesCommand == null ) {
				loadEmployeesCommand = new SqlCommand("HiResAdmin.getEmployees");
				loadEmployeesCommand.CommandType = CommandType.StoredProcedure;
				loadEmployeesCommand.Parameters.Add(new SqlParameter(Filter, SqlDbType.NVarChar));
				loadEmployeesCommand.Parameters.Add(new SqlParameter(Order, SqlDbType.NVarChar));
			}
			return loadEmployeesCommand;
		}

		#endregion

		#region Get, Add, Update, Load employees functions
		/// <summary>
		/// Load information about employee
		/// </summary>
		/// <param name="uid">UID</param>
		/// <returns>Employee info, if employee was found and null otherwise.</returns>
		public EmployeeInfo GetInfo(string uid){
			SqlCommand cmd = GetLoadCommand();
			cmd.Parameters[UID].Value = uid;
			EmployeeInfo employee = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					employee = new EmployeeInfo();
					employee.UID = (string)reader["UID"];
					if (reader["FirstName"] != DBNull.Value)
						employee.Contact.FirstName = (string)reader["FirstName"];
					if (reader["LastName"] != DBNull.Value)
						employee.Contact.LastName = (string)reader["LastName"];
					if (reader["ContactEmail"] != DBNull.Value)
						employee.Contact.ContactEmail = (string)reader["ContactEmail"];
					if (reader["Roles"] != DBNull.Value)
						employee.Roles = (string)reader["Roles"];
					if (reader["AccountBlocked"] != DBNull.Value)
						employee.AccountBlocked = (bool)reader["AccountBlocked"];

					if (reader["Address1"] != DBNull.Value)
						employee.Contact.Address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						employee.Contact.Address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						employee.Contact.Address.Country = (String)reader["Country"];
					if (reader["City"] != DBNull.Value)
						employee.Contact.Address.City = (String)reader["City"];
					if (reader["State"] != DBNull.Value)
						employee.Contact.Address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						employee.Contact.Address.ZipCode = (String)reader["ZipCode"];
					if (reader["contactPhone"] != DBNull.Value)
						employee.Contact.ContactPhone = (String)reader["contactPhone"];
					if (reader["contactPhone2"] != DBNull.Value)
						employee.Contact.ContactPhone2 = (String)reader["contactPhone2"];
					if (reader["contactFax"] != DBNull.Value)
						employee.Contact.ContactFax = (String)reader["contactFax"];
					if (reader["hourlyRate"] != DBNull.Value)
						employee.HourlyRate = (Decimal)reader["hourlyRate"];
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
			return employee;
		}
		
		/// <summary>
		/// Register new employee
		/// </summary>
		/// <param name="ei">Employee information</param>
		/// <returns>true, if employee was succesfully registered, and false otherwise.</returns>
		public bool Add(EmployeeInfo ei, string passwd) {
			SqlCommand cmd = GetInsertCommand();
			cmd.Parameters[UID].Value            = ei.UID;
			cmd.Parameters[FirstName].Value      = (ei.Contact.FirstName == null) ? DBNull.Value : (Object)ei.Contact.FirstName ;
			cmd.Parameters[LastName].Value       = (ei.Contact.LastName == null) ? DBNull.Value : (Object)ei.Contact.LastName;
			cmd.Parameters[ContactEmail].Value   = (ei.Contact.ContactEmail == null) ? DBNull.Value : (Object)ei.Contact.ContactEmail;
			cmd.Parameters[Passwd].Value         = GetPasswordHash(passwd);
			cmd.Parameters[Roles].Value          = (ei.Roles == null) ? DBNull.Value : (Object)ei.Roles;
			cmd.Parameters[AccountBlocked].Value = (Object)ei.AccountBlocked;

			cmd.Parameters[Address2].Value = (ei.Contact.Address.Address2 == null) ? DBNull.Value : (Object)ei.Contact.Address.Address2;
			cmd.Parameters[Address1].Value = (ei.Contact.Address.Address1 == null) ? DBNull.Value : (Object)ei.Contact.Address.Address1;
			cmd.Parameters[Country].Value = (ei.Contact.Address.Country == null) ? DBNull.Value : (Object)ei.Contact.Address.Country;
			cmd.Parameters[City].Value = (ei.Contact.Address.City == null) ? DBNull.Value : (Object)ei.Contact.Address.City;
			cmd.Parameters[State].Value = (ei.Contact.Address.State == null) ? DBNull.Value : (Object)ei.Contact.Address.State;
			cmd.Parameters[ZipCode].Value = (ei.Contact.Address.ZipCode == null) ? DBNull.Value : (Object)ei.Contact.Address.ZipCode;
			cmd.Parameters[ContactPhone].Value = (ei.Contact.ContactPhone == null) ? DBNull.Value : (Object)ei.Contact.ContactPhone; 
			cmd.Parameters[ContactPhone2].Value = (ei.Contact.ContactPhone2 == null) ? DBNull.Value : (Object)ei.Contact.ContactPhone2; 
			cmd.Parameters[ContactFax].Value = (ei.Contact.ContactFax == null) ? DBNull.Value : (Object)ei.Contact.ContactFax; 
			cmd.Parameters[HourlyRate].Value = (Object)ei.HourlyRate; 

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("Error while adding employee",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Update information about existing employee
		/// </summary>
		/// <param name="ei">New employee information</param>
		/// <returns>true, if employee info was succesfully updated, and false if such user not exist.</returns>
		public bool UpdateInfo(EmployeeInfo ei) {
			SqlCommand cmd = GetUpdateInfoCommand();
			cmd.Parameters[UID].Value            = ei.UID;
			cmd.Parameters[FirstName].Value      = (ei.Contact.FirstName == null) ? DBNull.Value : (Object)ei.Contact.FirstName ;
			cmd.Parameters[LastName].Value       = (ei.Contact.LastName == null) ? DBNull.Value : (Object)ei.Contact.LastName;
			cmd.Parameters[ContactEmail].Value   = (ei.Contact.ContactEmail == null) ? DBNull.Value : (Object)ei.Contact.ContactEmail;
			cmd.Parameters[Roles].Value          = (ei.Roles == null) ? DBNull.Value : (Object)ei.Roles;
			cmd.Parameters[AccountBlocked].Value = (Object)ei.AccountBlocked;

			cmd.Parameters[Address2].Value = (ei.Contact.Address.Address2 == null) ? DBNull.Value : (Object)ei.Contact.Address.Address2;
			cmd.Parameters[Address1].Value = (ei.Contact.Address.Address1 == null) ? DBNull.Value : (Object)ei.Contact.Address.Address1;
			cmd.Parameters[Country].Value = (ei.Contact.Address.Country == null) ? DBNull.Value : (Object)ei.Contact.Address.Country;
			cmd.Parameters[City].Value = (ei.Contact.Address.City == null) ? DBNull.Value : (Object)ei.Contact.Address.City;
			cmd.Parameters[State].Value = (ei.Contact.Address.State == null) ? DBNull.Value : (Object)ei.Contact.Address.State;
			cmd.Parameters[ZipCode].Value = (ei.Contact.Address.ZipCode == null) ? DBNull.Value : (Object)ei.Contact.Address.ZipCode;
			
			cmd.Parameters[ContactPhone].Value = (ei.Contact.ContactPhone == null) ? DBNull.Value : (Object)ei.Contact.ContactPhone; 
			cmd.Parameters[ContactPhone2].Value = (ei.Contact.ContactPhone2 == null) ? DBNull.Value : (Object)ei.Contact.ContactPhone2; 
			cmd.Parameters[ContactFax].Value = (ei.Contact.ContactFax == null) ? DBNull.Value : (Object)ei.Contact.ContactFax; 
			cmd.Parameters[HourlyRate].Value = (Object)ei.HourlyRate; 

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				AppLog.LogError("Error while updating employee info",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Update existing employee password
		/// </summary>
		/// <param name="uid">employee UID</param>
		/// <param name="pwd">New employee password</param>
		/// <returns>true, if employee password was succesfully updated, and false if such user not exist.</returns>
		public bool UpdatePassword(string uid, string pwd) {
			SqlCommand cmd = GetUpdatePwdCommand();
			cmd.Parameters[UID].Value    = uid;
			cmd.Parameters[Passwd].Value = GetPasswordHash(pwd);
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch  {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Remove employee from database
		/// </summary>
		/// <param name="uid">employee UID</param>
		/// <returns>true, if employee was removed, and false, if employee not exist.</returns>
		public bool Remove(string uid) {
			SqlCommand cmd = GetRemoveCommand();
			cmd.Parameters[UID].Value = uid;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch  {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Load employees
		/// </summary>
		/// <returns></returns>
		public ArrayList GetEmployees(FilterExpression filter, OrderExpression order) {
			SqlCommand cmd = GetEmployeesCommand();
			cmd.Parameters[Filter].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters[Order].Value = order == null ? "" : order.ToString();
			ArrayList employeeList = new ArrayList();
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					EmployeeInfo employee = new EmployeeInfo();
					employee.UID = (string)reader["UID"];
					if (reader["FirstName"] != DBNull.Value)
						employee.Contact.FirstName = (string)reader["FirstName"];
					if (reader["LastName"] != DBNull.Value)
						employee.Contact.LastName = (string)reader["LastName"];
					if (reader["ContactEmail"] != DBNull.Value)
						employee.Contact.ContactEmail = (string)reader["ContactEmail"];
					if (reader["Roles"] != DBNull.Value)
						employee.Roles = (string)reader["Roles"];
					if (reader["AccountBlocked"] != DBNull.Value)
						employee.AccountBlocked = (bool)reader["AccountBlocked"];

					if (reader["Address1"] != DBNull.Value)
						employee.Contact.Address.Address1 = (String)reader["Address1"];
					if (reader["Address2"] != DBNull.Value)
						employee.Contact.Address.Address2 = (String)reader["Address2"];
					if (reader["Country"] != DBNull.Value)
						employee.Contact.Address.Country = (String)reader["Country"];
					if (reader["City"] != DBNull.Value)
						employee.Contact.Address.City = (String)reader["City"];
					if (reader["State"] != DBNull.Value)
						employee.Contact.Address.State = (String)reader["State"];
					if (reader["ZipCode"] != DBNull.Value)
						employee.Contact.Address.ZipCode = (String)reader["ZipCode"];
					if (reader["contactPhone"] != DBNull.Value)
						employee.Contact.ContactPhone = (String)reader["contactPhone"];
					if (reader["contactPhone2"] != DBNull.Value)
						employee.Contact.ContactPhone2 = (String)reader["contactPhone2"];
					if (reader["contactFax"] != DBNull.Value)
						employee.Contact.ContactFax = (String)reader["contactFax"];
					if (reader["hourlyRate"] != DBNull.Value)
						employee.HourlyRate = (Decimal)reader["hourlyRate"];

					employeeList.Add(employee);
				}
				reader.Close();
			} catch (Exception ex) {
				return null;
			} finally {
				if (conn != null) conn.Close();
			}
			return employeeList;
		}

		/// <summary>
		/// Check login command
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetCheckLoginCommand() {
			if ( checkLoginCommand == null ) {
				checkLoginCommand = new SqlCommand("HiResAdmin.checkEmployeeLogin");
				checkLoginCommand.CommandType = CommandType.StoredProcedure;
				checkLoginCommand.Parameters.Add(new SqlParameter(UID, SqlDbType.NVarChar, 20));
				checkLoginCommand.Parameters.Add(new SqlParameter(Passwd, SqlDbType.NVarChar, 30));
				checkLoginCommand.Parameters.Add(new SqlParameter("@logged", SqlDbType.Int));
				checkLoginCommand.Parameters["@logged"].Direction = ParameterDirection.Output;
			}
			return checkLoginCommand;
		}


		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion


		/// <summary>
		/// Check employee existance and his password.
		/// </summary>
		/// <param name="UID"></param>
		/// <param name="passwd"></param>
		/// <returns>true, if password accepted, false, if password incorrect, 
		/// throws exception, if employee not exist or any database error occurs.</returns>
		public bool CheckLogin(string uid, string passwd) {
			SqlCommand cmd = GetCheckLoginCommand();
			cmd.Parameters[UID].Value = uid;
			/* compute password hash and
			   transmit hashed password as parameter to SQL procedure */
			//cmd.Parameters[Passwd].Value = GetPasswordHash(passwd);
			cmd.Parameters[Passwd].Value = passwd;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				cmd.ExecuteNonQuery();
			} catch {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			int logged = (int)cmd.Parameters["@logged"].Value;
			if ((logged == -2) || (logged == -1)) 
				return false;
			return (logged == 1);
		}

		private String GetPasswordHash(String pwd) {
			return pwd;
//			MD5 md5 = new MD5CryptoServiceProvider();
//			ASCIIEncoding enc = new ASCIIEncoding();
//			byte[] data = enc.GetBytes(pwd);
//			data = md5.ComputeHash(data);
//			return enc.GetString(data);
		}


	}
}
