using System;

namespace HiRes.Common {
	
	public enum PromoCodeFields {
		NONE = 0,
		Code,
		SiteID,
		CustomerID,
		CreatedTS,
		CodeType,
		Amount,
		AmountType,
		CodeState,
		ValidFrom,
		ValidTo,
		IssuerId,
		Description,
		IsCooperative,
		MaxUseNumber,
		IsWholesale
	}

	/// <summary>
	/// Promotion code information container.
	/// </summary>
	public class PromoCodeInfo : PersistentBusinessEntity {
		public const string COMMON_CODE = "";

		public enum PromoCodeType {
			Disposable = 0,
			Permanent = 1
		}

		public enum PromoCodeState {
			Active = 0,
			Inactive = 1,
			Suspended = 2
		}

		public class Conditions {
			private int _maxUseNumber = 0;
			private bool _firstTimeCustomerOnly = false;
			
			public int MaxUseNumber {
				get { return _maxUseNumber; }
				set { _maxUseNumber = value; }
			}
			public bool FirstTimeCustomerOnly {
				get { return _firstTimeCustomerOnly;}
				set { _firstTimeCustomerOnly = value; }
			}
		}

		private int _siteId = PersistentBusinessEntity.ID_EMPTY;
		private string _customerUID;
		private string _issuer;
		private DateTime _validTo;
		private DateTime _validFrom;
		private PromoCodeType _codeType;
		
		public DiscountInfo Discount;
		
		private string _code;
		private PromoCodeState _codeState;

		//private int _maxUseNumber = 0;
		private string _description;

		private bool _isCooperative = false;

		private bool _isWholesale = false;

		public Conditions UsageConditions;
		
		#region constructor

		public PromoCodeInfo() {
			UsageConditions = new Conditions();
			_codeState = PromoCodeState.Active;
		}

		#endregion

		#region properties

		public override bool IsNew {
			get { 
				throw new NotImplementedException();
				/*if ((_code==null)||(_code==String.Empty)) {
					return false;
				} else {
					return true;
				}*/
			}
		}

		public int SiteId {
			get { return _siteId; }
			set { _siteId = value; }
		}

		public string CustomerUID {
			get { return _customerUID; }
			set { _customerUID = value; }
		}
		
		public string IssuedBy {
			get { return _issuer; }
			set { _issuer = value; }
		}

		public DateTime ValidTo { 
			get { return _validTo; }
			set { _validTo = value; }
		}

		public DateTime ValidFrom { 
			get { return _validFrom; }
			set { _validFrom = value; }
		}

		public PromoCodeType CodeType {
			get { return _codeType; }
			set { _codeType = value; }
		}

		public string Code {
			get { return _code;}
			set {_code = value;}
		}

		public PromoCodeState CodeState {
			get { return _codeState; }
			set { _codeState = value; }
		}
/*
		public int MaxUseNumber {
			get { return _maxUseNumber; }
			set { _maxUseNumber = value; }
		}
*/
		public string Description {
			get { return _description;}
			set {_description = value;}
		}
		
		/// <summary>
		/// Idicates if the code can be used together with other promotion codes for the same order.
		/// </summary>
		public bool IsCooperative {
			get { return _isCooperative; }
			set { _isCooperative = value; }
		}

		public bool IsWholesale {
			get { return _isWholesale; }
			set { _isWholesale = value; }
		}

		#endregion
	}

	public struct DiscountInfo {

		public decimal Amount;
		public DiscountAmountType AmountType;

		public DiscountInfo(decimal amount, DiscountAmountType amountType) {
			this.Amount = amount;
			this.AmountType = amountType;
		}
	}

	public enum DiscountAmountType {
		Money = 0,
		Persentages = 1
	}
	
}
