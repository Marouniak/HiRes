using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace HiRes.Common {

	public enum CustomerFields {
		NONE = 0,
		CustomerID, 
		FirstName, 
		LastName, 
		ContactEmail, 
		Passwd, 
		Address2, 
		Address1, 
		Country, 
		City, 
		State, 
		ZipCode,
		SiteId,
		CompanyName,
		ContactPhone,
		ContactPhone2,
		ContactFax,
		PaymentLevel,
		Industry
	}
	public enum PaymentLevel : short {
		Retail = 0,
		Wholesale = 1
	}
	/// <summary>
	/// Customer Info class contains information about customer
	/// </summary>
	/// FIXME: Inherit it from PersistentBusinessEntity
	public class CustomerInfo {

		#region private members
		private String _customerID = null;
		private int _siteID;
		private String _firstName = null;
		private String _lastName = null;
		private String _contactEmail = null;
		private String _passwd = null;
		private String _passwdHash = null;
		private String _companyName = null;
		private String _contactPhone = null;
		private String _contactPhone2 = null;
		private String _contactFax = null;
		private short _paymentLevel = 0;
		private String _industry = null;
		/*
		private String _address2 = null;
		private String _address1 = null;
		private String _country = null;
		private String _state = null;
		private String _city = null;
		private String _zipcode = null;
		*/
		private CustomerAddressInfo _address;
		private ContactInfo _billingContact;
		#endregion

		#region constructors
		public CustomerInfo() {
			_address = new CustomerAddressInfo();
			_billingContact = new ContactInfo();
		}

		public CustomerInfo(int siteId) : this() {
			_siteID = siteId;
		}
		#endregion

		#region properties
		public String CustomerID {
			set { _customerID = value; }
			get { return _customerID; }
		}
		public int SiteID {
			get { return _siteID; }
		}
		public String FirstName {
			set { _firstName = value; }
			get { return _firstName; }
		}
		public String LastName {
			set { _lastName = value; }
			get { return _lastName; }
		}
		public String ContactEmail {
			set { _contactEmail = value; }
			get { return _contactEmail; }
		}
		public String Passwd {
			set {
				_passwd = value;
				if (_passwd != null) {
					MD5 md5 = new MD5CryptoServiceProvider();
					ASCIIEncoding enc = new ASCIIEncoding();
					byte[] data = enc.GetBytes(_passwd);
					data = md5.ComputeHash(data);
					_passwdHash = enc.GetString(data);
				}
			}
			get { return _passwd; }
		}
		public String PasswdHash {
			set {
				_passwdHash = value;
			}
			get {
				return _passwdHash;
			}
		}
		
		/*
		public String Address2 {
			set { _address.Address2 = value; }
			get { return _address.Address2; }
		}
		public String Address1 {
			set { _address.Address1 = value; }
			get { return _address.Address1; }
		}
		public String Country {
			set { _address.Country = value; }
			get { return _address.Country; }
		}
		public String State {
			set { _address.State = value; }
			get { return _address.State; }
		}
		public String City {
			set { _address.City = value; }
			get { return _address.City; }
		}
		public String ZipCode {
			set { _address.ZipCode = value; }
			get { return _address.ZipCode; }
		}
		*/
		public CustomerAddressInfo Address {
			set { _address = value; }
			get { return _address; }
		}
		public String CompanyName {
			set { _companyName = value; }
			get { return _companyName; }
		}

		public String ContactPhone {
			set { _contactPhone = value; }
			get { return _contactPhone; }
		}

		public String ContactPhone2 {
			set { _contactPhone2 = value; }
			get { return _contactPhone2; }
		}

		public String ContactFax {
			set { _contactFax = value; }
			get { return _contactFax; }
		}
		
		public string FullName {
			get {
				StringBuilder sb;
				if (_companyName!=null) {
					sb = new StringBuilder(_companyName, _companyName.Length + _firstName.Length + _lastName.Length + 2);
					sb.Append(" ");
				} else {
					sb = new StringBuilder( _firstName.Length + _lastName.Length + 1);
				}
				sb.Append(_firstName);
				sb.Append(" ");
				sb.Append(_lastName);
				return sb.ToString();
			}
		}

		public short PaymentLevel {
			get { return _paymentLevel; }
			set { _paymentLevel = value;}
		}

		public String Industry{
			get { return _industry; }
			set { _industry = value; }
		}

		public ContactInfo BillingContact {
			get { return _billingContact; }
			set { _billingContact = value; }
		}
		#endregion

	}
	
}
