using System;

namespace HiRes.Common {

	public interface SelfValidator {
		bool IsValid {
			get ;
		}
	}
	public abstract class BusinessEntity : SelfValidator {
		/// <summary>
		/// Check whether the entity is valid.
		/// </summary>
		public bool IsValid {
			get { return true; }
		}
	}
	/// <summary>
	/// TODO: add serialization features
	/// </summary>
	public abstract class PersistentBusinessEntity : BusinessEntity {
		
		public const int ID_EMPTY = -1;
		public const string SID_EMPTY = null;

		protected DateTime _createdTS;
		private DateTime _lastModifiedTS;

		public PersistentBusinessEntity() {
			this._createdTS = _lastModifiedTS = DateTime.Now ;//TODO: consider removing this
		}

		/// <summary>
		/// Indicate is this a new entity or loaded from persistent storage.
		/// </summary>
		public abstract bool IsNew {
			get;
		}
		
		public virtual DateTime CreatedTS {
			get { return _createdTS; }
			set { _createdTS = value;}
		}

		public DateTime LastModifiedTS {
			get { return _lastModifiedTS; }
			set { _lastModifiedTS = value;}
		}
	}
}
