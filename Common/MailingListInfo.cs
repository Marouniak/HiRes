using System;
using System.Collections;

namespace HiRes.Common {

	/// <summary>
	/// </summary>
	public sealed class MailingListInfo : PersistentBusinessEntity {
		int _mailingListId;
		private int _orderId;

		decimal _listCost;
		int _listQuantity;

		private string _fileContentType;
		private string _fileName;
		byte[] _mailingListBlob;

		public int MailingListId {
			get { return _mailingListId; }
			set { _mailingListId = value; }
		}

		public int OrderId {
			get { return _orderId; }
			set { _orderId = value; }
		}

		public decimal ListCost {
			get { return _listCost; }
			set { _listCost = value; }
		}

		public int ListQuantity {
			get { return _listQuantity; }
			set { _listQuantity = value; }
		}
		
		public string FileContentType {
			get { return _fileContentType; }
			set { _fileContentType = value; }
		}
		public string FileName {
			get { return _fileName; }
			set { _fileName = value; }
		}

		public byte[] MailingListBlob {
			get { return _mailingListBlob; }
			set { _mailingListBlob = value; }
		}

		public override bool IsNew {
			get {
				return (_mailingListId == PersistentBusinessEntity.ID_EMPTY);
			}
		}
		public bool IsEmpty {
			get {
				if ((this.FileName==null)||(this.FileName==String.Empty)) {
					return true;
				} else {
					return false;
				}
			}
		}
		//public bool IsListLoaded
		public MailingListInfo() : base() {
			_mailingListId = PersistentBusinessEntity.ID_EMPTY;
			_orderId = PersistentBusinessEntity.ID_EMPTY;
			_listCost = 0.00m;
			_listQuantity = 0;
		}

	}
}
