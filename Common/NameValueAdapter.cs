using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace HiRes.Common {

	/// <summary>
	/// Class NameValueAdapter construct NameValueCollection from adaptee
	/// </summary>
	public class IdNameAdapter {
		private NameValueCollection _adaptedCollection;
		private Hashtable  _adaptedHashtable;

		private bool _recalculate;

		private ICollection _adaptee;
		private String _idField;
		private String _nameField;

		public IdNameAdapter (ICollection adaptee, string IDField, string NameField) {
			if (adaptee==null) {
				throw new NullReferenceException("Adaptee can't be null");
			}
			_adaptee = adaptee;
			_idField = IDField;
			_nameField = NameField;
			_recalculate = true;
		}

		public ICollection Adaptee {
			get { return _adaptee; }
			set { 
				_adaptee = value;
				_recalculate = true;
			}
		}

		public String IdField{
			get { return _idField; }
			set { 
				_idField = value;
				_recalculate = true;
			}
		}

		public String NameField{
			get { return _nameField; }
			set { 
				_nameField = value;
				_recalculate = true;
			}
		}

		public NameValueCollection AdaptedCollection {
			get {
				if (_recalculate) {
					_adaptedCollection = GetAdaptedCollection();
					_recalculate = false;
				}
				return _adaptedCollection;
			}
		}

		private NameValueCollection GetAdaptedCollection() {
			NameValueCollection adaptedCollection = new NameValueCollection();
			Type type;
			FieldInfo fieldInfo;
			String name, val;

			foreach (Object o in _adaptee) {
				type = o.GetType();
				
				fieldInfo = type.GetField(_idField);
				if (fieldInfo==null) {
					throw new ArgumentException("Field "+_idField+" not found in "+ type.ToString(),_idField);
				}
				name = fieldInfo.GetValue(o).ToString();
				fieldInfo = type.GetField(_nameField);
				if (fieldInfo==null) {
					throw new ArgumentException("Field "+_nameField+" not found in "+ type.ToString(),_nameField);
				}
                val = fieldInfo.GetValue(o).ToString();
				adaptedCollection.Add(name,val);
			}
			return adaptedCollection;
		}

		public Hashtable AdaptedHashtable {
			get {
				if (_recalculate) {
					_adaptedHashtable = GetAdaptedHashtable();
					_recalculate = false;
				}
				return _adaptedHashtable;
			}
		}

		public Hashtable GetAdaptedHashtable() {
			Hashtable adaptedHashtable = new Hashtable();
			Type type;
			FieldInfo fieldInfo;
			object id, val;
			foreach (Object o in _adaptee) {
				type = o.GetType();
				
				fieldInfo = type.GetField(_idField);
				if (fieldInfo==null) {
					throw new ArgumentException("Field "+_idField+" not found in "+ type.ToString(),_idField);
				}
				id = fieldInfo.GetValue(o);
				fieldInfo = type.GetField(_nameField);
				if (fieldInfo==null) {
					throw new ArgumentException("Field "+_nameField+" not found in "+ type.ToString(),_nameField);
				}
				val = fieldInfo.GetValue(o);
				adaptedHashtable.Add(id,val);
			}
			return adaptedHashtable;
		}

	}

	/// <summary>
	/// TODO: change it the way it determine if key is a field or a property
	/// </summary>
	public class IdObjectAdapter {
		private Hashtable _adaptedCollection;
		private bool _recalculate;

		private ICollection _adaptee;
		private String _idField;
		

		public IdObjectAdapter (ICollection adaptee, string IDField) {
			if (adaptee==null) {
				throw new NullReferenceException("Adaptee can't be null");
			}
			_adaptee = adaptee;
			_idField = IDField;
			_recalculate = true;
		}

		public ICollection Adaptee {
			get { return _adaptee; }
			set { 
				_adaptee = value;
				_recalculate = true;
			}
		}

		public String IdField{
			get { return _idField; }
			set { 
				_idField = value;
				_recalculate = true;
			}
		}


		public Hashtable AdaptedCollection {
			get {
				if (_recalculate) {
					_adaptedCollection = GetAdaptedCollection();
					_recalculate = false;
				}
				return _adaptedCollection;
			}
		}

		private Hashtable GetAdaptedCollection() {
			Hashtable adaptedCollection = new Hashtable();
			Type type;
			FieldInfo fieldInfo;
			PropertyInfo propertyInfo;
			String name, val;

			foreach (Object o in _adaptee) {
				type = o.GetType();
				
				fieldInfo = type.GetField(_idField);
				
				if (fieldInfo==null) {
					propertyInfo = type.GetProperty(_idField);
					if (propertyInfo==null) {
						throw new ArgumentException("Field or non-indexed property "+_idField+" not found in "+ type.ToString(),_idField);
					}
					adaptedCollection.Add(propertyInfo.GetValue(o,null),o);
				} else {
					//name = fieldInfo.GetValue(o).ToString();
					adaptedCollection.Add(fieldInfo.GetValue(o),o);
				}
			}
			return adaptedCollection;
		}
	}

	/// <summary>
	/// Enum Adapter to NameValue and Hashtable collections
	/// </summary>
	public class IdNameEnumAdapter {
		private NameValueCollection _adaptedCollection;
		private Hashtable  _adaptedHashtable;
		private bool _recalculate;
		private bool _storeIdAsString;
		private bool _addLeadZero;
		
		private Type _adaptee;

		public IdNameEnumAdapter (Type adaptee) {
			if (adaptee==null) {
				throw new NullReferenceException("Adaptee can't be null");
			}
			_adaptee = adaptee;
			_recalculate = true;
			_storeIdAsString = true;
			_addLeadZero = false;
		}
		/// <summary>
		/// Determine whether id should be stored as string
		/// </summary>
		/// <remarks>It affects only AdaptedHashtable</remarks>
		public bool StoreIdAsString {
			get { return _storeIdAsString; }
			set { _storeIdAsString = value; }
		}

		public bool AddLeadZero {
			get { return _addLeadZero; }
			set { _addLeadZero = value; }
		}

		public NameValueCollection AdaptedCollection {
			get {
				if (_recalculate) {
					_adaptedCollection = new NameValueCollection();
					Array values = Enum.GetValues(_adaptee);
					string[] names = Enum.GetNames(_adaptee);
			
					int i = 0;
					foreach( int v in values) {
						_adaptedCollection.Add(v.ToString(),names[i]);
						i++;
					}
					_recalculate = false;
				}
				return _adaptedCollection;
			}
		}
		public Hashtable AdaptedHashtable {
			get {
				if (_recalculate) {
					_adaptedHashtable = new Hashtable();
					Array keys = Enum.GetValues(_adaptee);
					string[] names = Enum.GetNames(_adaptee);
					
					int i = 0;
					if (_storeIdAsString) {
						string skey;
						foreach( int key in keys) {
							skey = key.ToString();
							if (_addLeadZero) {
								if (skey.Length==1) { skey = "0"+skey; }
							}
							_adaptedHashtable.Add(skey,names[i]);
							i++;
						}
					} else {
						foreach( int key in keys) {
							_adaptedHashtable.Add(key,names[i]);
							i++;
						}
					}

					_recalculate = false;
				}
				return _adaptedHashtable;
			}
		}




	}
}
