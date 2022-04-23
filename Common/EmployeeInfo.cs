using System;

namespace HiRes.Common {
	public enum EmployeeFields {
		NONE = 0,
		UID, 
		FirstName, 
		LastName, 
		ContactEmail, 
		Roles, 
		AccountBlocked 
	}


	/// <summary>
	/// Summary description for EmployeeInfo.
	/// </summary>
	public class EmployeeInfo {

		#region private members
		private string _uid;
		private string _roles; // comma-separated roles
		private bool _accountBlocked;
		private decimal _hourlyRate;

		private ContactInfo _contact;

		#endregion

		#region constructors
		public EmployeeInfo() {
			_contact = new ContactInfo();
		}
		#endregion

		#region properties
		public string UID {
			set { _uid = value; }
			get { return _uid; }
		}

		public string Roles{
			set { _roles = value; }
			get { return _roles; }
		}

		public decimal HourlyRate{
			set { _hourlyRate = value; }
			get { return _hourlyRate; }
		}

		public bool AccountBlocked {
			set { _accountBlocked = value; }
			get { return _accountBlocked; }
		}

		public ContactInfo Contact {
			get { return _contact; }
			set {
				if (value==null) {
					throw new ArgumentNullException();
				}
				_contact = value;
			}
		}
		

		#endregion
	}
}
