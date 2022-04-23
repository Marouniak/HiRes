using System;
using System.Collections;
using System.Collections.Specialized;

namespace HiRes.Common {
	[Serializable()]
	public class FieldMultiValueContainer {

		protected Type enumType;
		protected Object[] fieldValues;
		protected Hashtable _container;
		
		public FieldMultiValueContainer(Type enumType) {
			this.enumType = enumType;

			Array fields = Enum.GetValues(this.enumType);
			int capacity = fields.Length;
			_container = new Hashtable(capacity);
			foreach(object obj in fields) {
				_container[obj] = new ArrayList();
			}
		}

		/// <summary>
		/// Indexer property to access the specific filter field
		/// </summary>
		public virtual object this[object key] {
			get {
				return _container[key];
			}
			set {
				if(!Enum.IsDefined(enumType, (int)key)) {
					throw(new ArgumentOutOfRangeException("index", "Not a valid " + enumType.ToString()));
				}
				/*
				// don't process empty strings
				if((value != null) && (value.ToString() == "")) {
					_container[key] = null;
				} else {
					_container[key] = value;
				}*/
				if (!value.GetType().Equals(typeof(ArrayList))) {
					throw new ArgumentException("value type should be ArrayList");
				} else { _container[key] = value; }
			}
		}

		public virtual void Add(object key, object val) {
			if(!Enum.IsDefined(enumType, (int)key)) {
				throw(new ArgumentOutOfRangeException("key", "Not a valid " + enumType.ToString()));
			}
			ArrayList arr = (ArrayList)this[key];
			arr.Add(val);
			//((ArrayList)this[key]).Add(val);
		}
		
		public virtual void Clear() {
			foreach (ArrayList item in _container) {
				item.Clear();
			}
		}

		public virtual void Clear(object key) {
			((ArrayList)this[key]).Clear();
		}

		/// <summary>
		/// Returns enum type of current filter
		/// </summary>
		public Type EnumType {
			get {
				return enumType;
			}
		}

		public override String ToString() {
			return null;
		}

		public int FilterParamsCount {
			get {
				int num = 0;
				foreach(ArrayList paramvalues in _container.Values) {
					num += paramvalues.Count;
				}
				return num;
			}
		}

	}

	/// <summary>
	/// Summary description for FieldValueCollection.
	/// </summary>
	[Serializable()]
	public class FieldValueCollection {

		protected Type enumType;
		protected Object[] fieldValues;

		public FieldValueCollection(Type enumType) {
			this.enumType = enumType;
			fieldValues = new Object[Enum.GetValues(this.enumType).Length];
		}

		/// <summary>
		/// Indexer property to access the specific filter field
		/// </summary>
		public virtual object this[int index] {
			get {
				return fieldValues[index];
			}
			set {
				if(!Enum.IsDefined(enumType, index)) {
					throw(new ArgumentOutOfRangeException("index", "Not a valid " + enumType.ToString()));
				}
				// don't process empty strings
				if((value != null) && (value.ToString() == "")) value = null;
				fieldValues[index] = value;
			}
		}

		/// <summary>
		/// Returns enum type of current filter
		/// </summary>
		public Type EnumType {
			get {
				return enumType;
			}
		}

		/// <summary>
		/// Clears current filter value
		/// </summary>
		public virtual void Clear() {
			for(int i = 0; i < fieldValues.Length; i++) {
				fieldValues[i] = null;
			}
		}

		public override String ToString() {
			return null;
		}

	}
	[Serializable()]
	public class FilterExpression : FieldMultiValueContainer {

		public FilterExpression(Type enumType) : base(enumType) {
		}

		public override String ToString() {
			String filterString = String.Empty;
			String[] enumNames = Enum.GetNames(enumType);
			foreach(String fieldName in enumNames) { 
				object field = Enum.Parse(enumType, fieldName, true);
				// Add filter value to SQL string
				ArrayList values = (ArrayList)_container[field];
				if((values != null)&&(values.Count>0)/* && (values != "NONE")*/) {
					if(filterString != String.Empty) filterString += " and ";
					filterString += " ( ";
					bool firstValue = true;
					foreach ( object val in values ) {
						if (val==null) {
							if (!firstValue) {
								filterString += " or ";
							} else { firstValue = false; }
							filterString += fieldName + " = null ";
							continue;
						}
						if((val != null) && (!val.ToString().Equals("NONE"))) {
							/*if (!firstValue) {
								filterString += " or ";
							} else { firstValue = false; }*/
							if(Type.GetTypeCode(val.GetType()) != TypeCode.String) {
//								if(filterString != String.Empty) filterString += " and ";
								if (!firstValue) {
									filterString += " or ";
								} else { firstValue = false; }
								filterString += fieldName + " = ";
								if(Type.GetTypeCode(val.GetType()) == TypeCode.Boolean) {
									bool bVal = (bool) val/*_container[field]*/;
									if(bVal) {
										filterString += "1";
									} else {
										filterString += "0";
									}
								} else {
									filterString += val.ToString();
								}
								//TODO: add DateTime rountines
							} else {
								// replace ' character in string to ''
								String s = /*_container[field]*/val.ToString();
								s = s.Replace("'", "''");
								//if(filterString != String.Empty) filterString += " and ";
								if (!firstValue) {
									//filterString += " and "; 
									filterString += " or "; 
								} else {
									firstValue = false;
								}
								filterString += fieldName + " = '" + s + "'";
							}
						}
					}
					filterString += " ) ";
				}
			}
			return filterString;
		}

	}
/*
	/// <summary>
	/// Summary description for FilterExpression.
	/// 
	/// Sample of using FilterExpression class
	/// FilterExpression filterExp = new FilterExpression(typeof(OrderFields));
	/// filterExp[(int)OrderFields.SiteID] = 1;
	/// filterExp[(int)OrderFields.CustomerID] = "xomok";
	/// String filterStr = filterExp.ToString();
	/// 
	/// </summary>
	[Serializable()]
	public class FilterExpression : FieldValueCollection {

		public FilterExpression(Type enumType) : base(enumType) {
		}

		public override String ToString() {
			String filterString = String.Empty;
			String[] enumNames = Enum.GetNames(enumType);
			foreach(String fieldName in enumNames) { 
				int field = (int)Enum.Parse(enumType, fieldName, true);
				// Add filter value to SQL string
				if((fieldValues[field] != null) && (fieldName != "NONE")) {
					if(Type.GetTypeCode(fieldValues[field].GetType()) != TypeCode.String) {
						if(filterString != String.Empty) filterString += " and ";
						filterString += fieldName + " = ";
						if(Type.GetTypeCode(fieldValues[field].GetType()) == TypeCode.Boolean) {
							bool bVal = (bool) fieldValues[field];
							if(bVal) {
								filterString += "1";
							} else {
								filterString += "0";
							}
						} else {
							filterString += fieldValues[field].ToString();
						}
					} else {
						// replace ' character in string to ''
						String s = fieldValues[field].ToString();
						s = s.Replace("'", "''");
						if(filterString != String.Empty) filterString += " and ";
						filterString += fieldName + " = '" + s + "'";
					}
				}
			}
			return filterString;
		}

	}
*/
	public enum Order_By_Expression {
		ASC,
		DESC
	}

	/// <summary>
	/// Summary description for OrderExpression.
	/// 
	/// Sample of using OrderExpression class
	/// OrderExpression orderExp = new OrderExpression(typeof(CustomerFields));
	/// orderExp[(int)CustomerFields.CustomerID] = Order_By_Expression.ASC;
	/// orderExp[(int)CustomerFields.ContactEmail] = Order_By_Expression.Desc;
	/// String orderByStr = orderExp.ToString();
	/// 
	/// </summary>
	public class OrderExpression : FieldValueCollection {
		
		private int orderIndex;
		private int[] order;

		public OrderExpression(Type enumType) : base(enumType) {
			order = new int[Enum.GetValues(this.enumType).Length];
			ResetOrder();
		}

		public override Object this[int index] {
			get {
				return base[index];
			}
			set {
				if (value.GetType() != typeof(Order_By_Expression))
					throw new FormatException("Invalid value type: " + value.GetType().ToString());
				base[index] = value;
				if (Array.IndexOf(order, index, 0, orderIndex) == -1) {
					order[orderIndex] = index;
					orderIndex++;
				}
			}
		}

		public override String ToString() {
			String filterString = String.Empty;
			String[] enumNames = Enum.GetNames(enumType);
			for (int i=0; i<orderIndex; i++) {
				int field = order[i];
				String fieldName = Enum.Parse(enumType, field.ToString(), true).ToString();
				if((fieldValues[field] != null) && (fieldName != "NONE")) {
					filterString += fieldName + " " + fieldValues[field] + ",";
				}
			}
			if (filterString != String.Empty)
				return filterString.Substring(0, filterString.Length-1);
			return filterString;
		}

		public override void Clear() {
			base.Clear();
			ResetOrder();
		}

		private void ResetOrder() {
			for (int i=0; i<order.Length; i++) {
				order[i] = -1;
			}
			orderIndex = 0;
		}

	}

}
