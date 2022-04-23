using System;
using HiRes.Common;
namespace HiRes.Web.Common {
	/// <summary>
	/// Summary description for IOrderInfoManagerCommon.
	/// </summary>

	public interface IOrderInfoManagerBase {

		int OrderId {
			get;
			set;
		}
		
		//OrderInfo CurrentOrder();

		bool HasOrderInfo {
			get;
		}
		
		bool StoreOrderInfoInSessionVariable {
			get;
			set;
		}

		OrderInfo OrderInfoFromSessionVar {
			get;
			set;
		}

		void AssignOrderInfo(ref OrderInfo orderInfo);

		OrderInfo GetOrderInfo();

		void InvalidateOrderInfo();
	}
}
