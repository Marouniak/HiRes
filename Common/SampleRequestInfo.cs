using System;
using System.Collections;

namespace HiRes.Common {

	public enum SampleRequestFields {
		NONE = 0,
		SampleRequestID, 
		FirstName, 
		LastName, 
		Status,
		PlacedTS
	}

	public enum SampleRequestState {
		New = 0,
		Sent = 1
	}
	/// <summary>
	/// </summary>
	public class SampleRequestInfo : PersistentBusinessEntity {
		
		private ContactInfo _contact;
		private SampleRequestState _status;
		private PaymentLevel _paymentLevel;
		private Hashtable _selectedPrintingTypes;
		private String _industry = null;
		private int _sampleRequestId;


		public SampleRequestInfo() {
			_contact = new ContactInfo();
			_selectedPrintingTypes = new Hashtable();
			this.CreatedTS = this.LastModifiedTS = DateTime.Now;
		}

		public SampleRequestInfo(Hashtable printingTypes) {
			_contact = new ContactInfo();
			_selectedPrintingTypes = new Hashtable();
			this.CreatedTS = this.LastModifiedTS = DateTime.Now;
//			SetPrintingsFromString("6;5;4;3;2;1");
			IDictionaryEnumerator ptEnumerator = printingTypes.GetEnumerator();
			while (ptEnumerator.MoveNext()) 
				_selectedPrintingTypes.Add(ptEnumerator.Key, ptEnumerator.Value);	                                                                                                                                                                                                                                       
		}

		public int SampleRequestId 	{
			get { return _sampleRequestId; }
			set { _sampleRequestId = value; }
		}

		public override bool IsNew {
			get { return (_sampleRequestId==ID_EMPTY); }
		}

		public ContactInfo Contact 
		{
			get { return _contact; }
			set {
				if (value==null) {
					throw new ArgumentNullException();
				}
				_contact = value;
			}
		}
		
		public DateTime PlacedTS {
			get { return CreatedTS;/*._placedTS;*/ }
			set { CreatedTS = value; }
		}

		public SampleRequestState Status {
			get { return _status; }
			set { _status = value; }
		}

		public PaymentLevel PaymentLevel {
			get { return _paymentLevel; }
			set { _paymentLevel = value; }
		}

		public Hashtable SelectedPrintingTypes {
			get { return _selectedPrintingTypes; }
			set { _selectedPrintingTypes = value; }
		}

		public String Industry {
			get { return _industry; }
			set { _industry = value; }
		}

		public bool SetPrintingsFromString(string printings) {
			if(SelectedPrintingTypes == null)
				SelectedPrintingTypes = new Hashtable();
			if (printings == null)
				return false;
			SelectedPrintingTypes.Clear();
			
			string[] prints = printings.Split(new Char[] {';'});
			//for(int i=0; i<prints.Length; i++) {
			foreach(string print in prints) {
//				int ind=print.IndexOf('='); 
//				if(ind==-1) 
//					return false;
//				string printID = print.Substring(0,ind);
//				bool printValue = print.Substring(ind+1,1)=="1" ? true : false;
				try {
					SelectedPrintingTypes.Add(Int32.Parse(print),null);
				} catch {
					//
				}
			}
			return true;
		}
		

		public string GetStringFromPrintings() {
			if (SelectedPrintingTypes == null)
				return String.Empty;
			string res = String.Empty;
			
			int i=0;
			IDictionaryEnumerator printEnumerator = SelectedPrintingTypes.GetEnumerator();
			while ( printEnumerator.MoveNext() ) {
				if(i>0)
					res += ";";
					//res += printEnumerator.Key.ToString() + "=" + ((bool)printEnumerator.Value ? 1 : 0).ToString()+";";
					res += printEnumerator.Key.ToString();
				i++;
			}
			return res;
		}
	}
}
