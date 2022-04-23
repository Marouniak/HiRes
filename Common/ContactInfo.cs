using System;
using System.Runtime.Serialization;
using System.Text;

namespace HiRes.Common {

	[Serializable()]
	public class AddressInfo : BusinessEntity, ISerializable  {
		public const string USA_CODE = "US";
		public AddressInfo() {
			_country = USA_CODE;
		}

		public AddressInfo(SerializationInfo info, StreamingContext context) {
			_address1 = (String)info.GetValue("_address1",typeof(String));
			_address2 = (String)info.GetValue("_address2",typeof(String));
			_country = (String)info.GetValue("_country",typeof(String));
			_state = (String)info.GetValue("_state",typeof(String));
			_city = (String)info.GetValue("_city",typeof(String));
			_zipcode = (String)info.GetValue("_zipcode",typeof(String));
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("_address1",_address1);
			info.AddValue("_address2",_address2);
			info.AddValue("_country",_country);
			info.AddValue("_state",_state);
			info.AddValue("_city",_city);
			info.AddValue("_zipcode",_zipcode);
		}
		#region private

		private String _country;
		private String _state;
		private String _city;
		private String _zipcode;
		private String _address1;
		private String _address2;

		#endregion

		#region properties

		public String Address2 {
			set { _address2 = value; }
			get { return _address2; }
		}
		public String Address1 {
			set { _address1 = value; }
			get { return _address1; }
		}
		public String Country {
			set { _country = value; }
			get { return _country; }
		}
		public String State {
			set { _state = value; }
			get { return _state; }
		}
		public String City {
			set { _city = value; }
			get { return _city; }
		}
		public String ZipCode {
			set { _zipcode = value; }
			get { return _zipcode; }
		}

		public bool IsValid {
			// FIXME: add address validation
			get { return true; }
		}
		public String FullAddressInfo {
			get {
				return this.ToString();
			}
		}
		#endregion

		public override String ToString() {
			//return Address1+"&nbsp;"+Address2+"&nbsp;"+City+", "+State+" "+ZipCode+"&nbsp;"+Country;
			return ToString(true);
		}
		
		public String ToString(bool isShortForm) {
			string addr =  Address1+"&nbsp;"+Address2+"&nbsp;"+City+", "+State+" "+ZipCode;
			if (!isShortForm) {
				addr+="&nbsp;"+Country;
			}
			return addr;
		}
	}

	/*
	[Serializable()]
	
	public class BillingAddressInfo : AddressInfo {

		#region private
		private string _firstName;
		private string _lastName;
		#endregion

		#region properties
		public string FirstName {
			get { return _firstName; }
			set { _firstName = value; }
		}
		public string LastName {
			get { return _lastName;}
			set { _lastName = value; }
		}
		#endregion

		public BillingAddressInfo() : base() {
		}

		public BillingAddressInfo(SerializationInfo info, StreamingContext context) : base(info,context) {
			this._firstName = (String)info.GetValue("_firstName",typeof(String));
			this._lastName = (String)info.GetValue("_lastName",typeof(String));
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info,context);
			info.AddValue("_firstName",_firstName);
			info.AddValue("_lastName",_lastName);
		}

		public override String ToString() {
			return FirstName+" "+LastName+" "+Address1+"&nbsp;"+Address2+"&nbsp;"+City+", "+State+" "+ZipCode+"&nbsp;"+Country;
		}
	}

	*/
	/// <summary>
	/// Summary description for ContactInfo.
	/// </summary>
	[Serializable()]
	public class ContactInfo : BusinessEntity, ISerializable {
		
		#region private

		private string _firstName;
		private string _lastName;
		private String _contactEmail = null;
		private String _companyName = null;
		private String _contactPhone = null;
		private String _contactFax = null;
		private String _contactPhone2 = null;

		private AddressInfo _addressInfo;

		#endregion

		#region properties

		public string FirstName {
			get { return _firstName; }
			set { _firstName = value; }
		}
		public string LastName {
			get { return _lastName;}
			set { _lastName = value; }
		}

		public String ContactEmail {
			set { _contactEmail = value; }
			get { return _contactEmail; }
		}

		public AddressInfo Address {
			get { return _addressInfo;}
			set { _addressInfo = value; }
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
					int lenght = (_companyName!=null?_companyName.Length:0) + (_firstName!=null?_firstName.Length:0) + (_lastName!=null?_lastName.Length:0) + 2;
					sb = new StringBuilder(_companyName, lenght);
				if (_companyName!=null) {
//					int lenght = (_companyName!=null?_companyName.Length:0) + (_firstName!=null?_firstName.Length:0) + (_lastName!=null?_lastName.Length:0) + 2;
//					sb = new StringBuilder(_companyName, lenght);
					sb.Append(" ");
				}/* else {
					sb = new StringBuilder( _firstName.Length + _lastName.Length + 1);
				}*/
				sb.Append(_firstName==null?"":_firstName);
				sb.Append(" ");
				sb.Append(_lastName==null?"":_lastName);
				return sb.ToString();
			}
		}
		#endregion

		public ContactInfo() {
			_addressInfo = new AddressInfo();
		}

		public ContactInfo(SerializationInfo info, StreamingContext	context) {
			_addressInfo = new AddressInfo(info,context);
			_firstName	= (String)info.GetValue("_firstName",typeof(String));
			_lastName = (String)info.GetValue("_lastName",typeof(String));
			_contactEmail = (String)info.GetValue("_contactEmail",typeof(String));
			_companyName = (String)info.GetValue("_companyName",typeof(String));
			_contactPhone = (String)info.GetValue("_contactPhone",typeof(String));
			_contactFax = (String)info.GetValue("_contactFax",typeof(String));
		}

		public void	GetObjectData(SerializationInfo	info, StreamingContext context)	{
			_addressInfo.GetObjectData(info,context);
			info.AddValue("_firstName",_firstName);
			info.AddValue("_lastName",_lastName);
			info.AddValue("_contactEmail",_contactEmail);
			info.AddValue("_companyName",_companyName);
			info.AddValue("_contactPhone",_contactPhone);
			info.AddValue("_contactFax",_contactFax);
		}

		/*public override	String ToString() {
			return FirstName+" "+LastName+"	"+Address1+"&nbsp;"+Address2+"&nbsp;"+City+", "+State+"	"+ZipCode+"&nbsp;"+Country;
		}*/

	}

	[Serializable()]
	public class CustomerAddressInfo : AddressInfo, ISerializable {
		
		private int _customerAddressID;

		public int CustomerAddressID {
			set { _customerAddressID = value; }
			get { return _customerAddressID; }
		}

		public CustomerAddressInfo() {
			_customerAddressID = PersistentBusinessEntity.ID_EMPTY;
		}

		public CustomerAddressInfo(SerializationInfo info, StreamingContext context): base(info,context) {
			_customerAddressID = (int)info.GetValue("_customerAddressID",typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info,context);
			info.AddValue("_customerAddressID",_customerAddressID);
		}

	}

}
