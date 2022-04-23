using System;
using System.Collections;
using System.Runtime.Serialization;

namespace HiRes.Common {
	/// <summary>
	/// 
	/// </summary>
	[Obsolete("",false)]
	public class ExtraInfo {

		public int ExtraID;
		public String ExtraName;
		public String ExtraDescription;
		public int PrintingTypeID;
		public ExtraPrice PriceInfo;
		//bool IsSelected;
	}
	
	[Obsolete("",false)]
	public struct ExtraPrice {
		public int ExtraID;
		public int SiteID;
		public decimal Price;
		public bool IsSpecial;
	}

	public enum ExtraFields {
		NONE = 0,
		ExtraID,
		ExtraName,
		ExtraDescription
	}


	public class FullExtraInfo {
		public const int MAX_EXTRA_ATTR_NUM = 4;
		public int ExtraID;
		public String ExtraName;
		public String ExtraDescription;
		
		public ExtraAttribute[] Attributes;

		public FullExtraInfo() {
			Attributes = new ExtraAttribute[MAX_EXTRA_ATTR_NUM];
		}
	}

	public class FullExtraPrice {
		private FullExtraInfo _extraInfo;
		private decimal _price;
		private bool _isPricePerM;
		private bool _isSpecial;

		public FullExtraInfo ExtraInfo {
			get { return _extraInfo; }
			set {
				if (_extraInfo==null) {
					throw new ArgumentNullException();
				}
				_extraInfo = value;
			}
		}

		public int ExtraID {
			get {
				return this.ExtraInfo.ExtraID;
			}
		}

		public decimal Price {
			get { return _price; }
			set { _price = value; }
		}
		public bool IsPricePerM {
			get { return _isPricePerM; }
			set { _isPricePerM = value; }
		}
		public bool IsSpecial {
			get { return _isSpecial; }
			set { _isSpecial = value; }
		}

		public FullExtraPrice() {
			_extraInfo = new FullExtraInfo();
		}
		public FullExtraPrice(FullExtraInfo extraInfo) {
			_extraInfo = extraInfo;
		}
	}
	
	[Serializable()]
	public class SelectedExtraInfo : ISerializable {
		
		private int _extraID;
		private decimal _price;
		private bool _isPricePerM;
		private int _quantityToApply;
		private decimal _totalExtraAmount;
		private ExtraAttribute[] _attributesValues;
		
		#region properties

		public int ExtraID {
			get { return _extraID; }
			set { _extraID = value; }
		}

		public decimal Price {
			get { return _price; }
			set { _price = value; }
		}

		public bool IsPricePerM {
			get { return _isPricePerM; }
			set { _isPricePerM = value; }
		}
		
		public int QuantityToApply {
			get { return _quantityToApply; }
			set { _quantityToApply = value; }
		}

		public decimal TotalExtraAmount {
			get { return _totalExtraAmount; }
			set { _totalExtraAmount = value; }
		}

		public ExtraAttribute[] AttributesValues {
			get {
				return _attributesValues;
			}
			set {
				if (value!=null) {
					_attributesValues = value;
				} else {
					throw new ArgumentNullException();
				}
			}
		}
		#endregion

		public SelectedExtraInfo(params ExtraAttribute[] attrValues) {
			// Currently we can have only 4 atributes per extra maximum
			if (attrValues.Length>FullExtraInfo.MAX_EXTRA_ATTR_NUM) {
				throw new ArgumentException("Too many values passed");
			}
			ExtraID = PersistentBusinessEntity.ID_EMPTY;
			Price = 0.00m;
			IsPricePerM = true;
			QuantityToApply = 0;
			_totalExtraAmount = 0.00m;
			AttributesValues = attrValues;

		}

		public SelectedExtraInfo() {
			ExtraID = PersistentBusinessEntity.ID_EMPTY;
			Price = 0.00m;
			IsPricePerM = true;
			QuantityToApply = 0;
			_totalExtraAmount = 0.00m;
			AttributesValues = new ExtraAttribute[FullExtraInfo.MAX_EXTRA_ATTR_NUM];
		}
		
		#region Serialization

		public SelectedExtraInfo(SerializationInfo info, StreamingContext context) {
			_extraID = (int)info.GetValue("_extraID",typeof(int));
			_price = (decimal)info.GetValue("_price",typeof(decimal));
			_isPricePerM = (bool)info.GetValue("_isPricePerM",typeof(bool));
			_quantityToApply = (int)info.GetValue("_quantityToApply",typeof(int));
			_totalExtraAmount = (decimal)info.GetValue("_totalExtraAmount",typeof(decimal));
			
			_attributesValues = new ExtraAttribute[FullExtraInfo.MAX_EXTRA_ATTR_NUM];
			for (int i=0;i<FullExtraInfo.MAX_EXTRA_ATTR_NUM;i++) {
				_attributesValues[i] = (ExtraAttribute)info.GetValue(i.ToString(),typeof(ExtraAttribute));
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("_extraID",_extraID);
			info.AddValue("_price",_price);
			info.AddValue("_isPricePerM",_isPricePerM);
			info.AddValue("_quantityToApply",_quantityToApply);
			info.AddValue("_totalExtraAmount",_totalExtraAmount);

			for (int i=0;i<FullExtraInfo.MAX_EXTRA_ATTR_NUM;i++) {
				//if (_attributesValues[i]==null) { continue; }
				info.AddValue(i.ToString(),_attributesValues[i],typeof(ExtraAttribute));
			}
		}
		#endregion

		public void RecalculateTotalExtraAmount() {
			_totalExtraAmount = (IsPricePerM?Price*(QuantityToApply/1000m):Price/*x1*/);
		}
	}

	public enum ExtraAttributeType : byte {
		None = 0,
		Informational = 1,
		Amount = 2
	}

	[Serializable()]
	public struct ExtraAttribute : ISerializable {
		private ExtraAttributeType _type;
		private object _value;
		public string _name;

		public ExtraAttributeType Type {
			get {
				return _type;
			}
		}

		public bool IsEmpty {
			get {
				return _type == ExtraAttributeType.None;
			}
		}
		
		public string Name {
			get { return _name; }
			set { _name = value; }
		}

		public object Value {
			get {
				return _value;
			}
			set {
				Type valuetype = value.GetType();
				if (_type==ExtraAttributeType.Amount) {
					if (valuetype !=typeof(decimal)) {
						throw new ArgumentException("value should be 'decimal'");
						/*try {
							_value = (decimal)value;
						} catch {
							throw new ArgumentException("value should be 'decimal'");
						}*/
					}
				} else {
					if (valuetype != typeof(string)) {
						throw new ArgumentException("value should be 'string'");
					}
				}
				_value = value;
			}
		}
		

		public decimal GetAmountValue() {
			if (_type!=ExtraAttributeType.Amount) {
				throw new ArgumentException();
			}
			return (decimal)_value;
		}
		
		public string GetInformationalValue() {
			if (_type==ExtraAttributeType.Amount) {
				throw new ArgumentException();
			}
			return (string)_value;
		}

		public void ParseStringValue (string val) {
			if (_type==ExtraAttributeType.Amount) {
				_value = decimal.Parse(val);
			} else {
				_value = val;
			}
		}

		/*public ExtraAttribute() {
			this._type = ExtraAttributeType.None;
			this._value = null;
			this.Name = string.Empty;
		}*/

		public ExtraAttribute(ExtraAttributeType type) {
			this._type = type;
			this._value = null;
			this._name = string.Empty;
		}

		public ExtraAttribute(ExtraAttributeType type, string name/*, string val*/) {
			this._type = type;
			this._value = null;
			this._name = name;
		}

		#region Serialization

		public ExtraAttribute(SerializationInfo info, StreamingContext context) {
/*
			private ExtraAttributeType _type;
			private object _value;
			public string _name;
*/
			_type = (ExtraAttributeType)info.GetValue("_type",typeof(byte));
			if (_type==ExtraAttributeType.Amount) {
				_value = (decimal)info.GetValue("_value",typeof(decimal));
			} else {
				_value = (string)info.GetValue("_value",typeof(string));
			}
			_name = (string)info.GetValue("_name",typeof(string));

		}

		public void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("_type",_type);
			info.AddValue("_value",_value);
			info.AddValue("_name",_name);
		}
		#endregion

	}
}
