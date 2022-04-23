using System;
using System.Collections;

using HiRes.Common;
using HiRes.DAL;

namespace HiRes.BusinessRules {
	/// <summary>
	/// </summary>
	public class MailingList {
		//public MailingList() {}

		public ArrayList GetMailingLists(int orderId) {
			using (MailingListDAL mdal = new MailingListDAL()) {
				return mdal.GetMailingLists(orderId);
			}
		}

		public MailingListInfo GetInfo(int mailingListId) {
			return GetInfo(mailingListId,true);
			/*using (MailingListDAL mdal = new MailingListDAL()) {
				return mdal.GetInfo(mailingListId);
			}*/
		}
		public MailingListInfo GetInfo(int mailingListId, bool loadFile) {
			using (MailingListDAL mdal = new MailingListDAL()) {
				return mdal.GetInfo(mailingListId,loadFile);
			}			
		}
		public bool Add(MailingListInfo listInfo, out int mailingListId) {
			using (MailingListDAL mdal = new MailingListDAL()) {
				return mdal.Add(listInfo, out mailingListId);
			}
		}
		public bool Add(ref MailingListInfo listInfo) {
			using (MailingListDAL mdal = new MailingListDAL()) {
				int mailingListId;// = PersistentBusinessEntity.ID_EMPTY;
				bool res = mdal.Add(listInfo, out mailingListId);
				listInfo.MailingListId = mailingListId;
				return res;
			}
		}

		public bool UpdateListData(MailingListInfo listInfo) {
			using (MailingListDAL mdal = new MailingListDAL()) {
				return mdal.UpdateListData(listInfo);
			}
		}
		public bool Remove(int mailingListId) {
			using (MailingListDAL mdal = new MailingListDAL()) {
				return mdal.Remove(mailingListId);
			}
		}
	}
}
