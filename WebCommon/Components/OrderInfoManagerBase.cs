using System;
using System.Web;

using HiRes.BusinessFacade;
using HiRes.Common;
using HiRes.Web.Common;
namespace HiRes.Web.Common.Components {
	/// <summary>
	/// OrderInfoManager provides facade for pages and controls for getting order information.
	/// </summary>
	public class OrderInfoManagerBase : IOrderInfoManagerBase {

		public const String KEY_CURRENTORDER = "CurrentOrder";

		protected OrderInfo _orderInfo;

		protected int _orderId = PersistentBusinessEntity.ID_EMPTY;

		//protected HiRes.Web.PrintingsSite.PageBase page;
		protected HttpContext context;
		
		public OrderInfoManagerBase(HttpContext context) {
			this.context = context;
		}
		
		public int OrderId {
			get { 
				return _orderId;
			}
			set { _orderId = value; }
		}
		
/*		public OrderInfo CurrentOrder() {
			try {
				if (context.Session[PageBase.KEY_CURRENTORDER]!=null) {
					return (OrderInfo)context.Session[PageBase.KEY_CURRENTORDER];
				} else { return null; }
			} catch {
				return null; // FIXME: design time
			}
		}
*/
		/// <summary>
		/// Determine whether the control has an order info bind to it
		/// </summary>
		public virtual bool HasOrderInfo {
			get {
				try {
					if (OrderId==PersistentBusinessEntity.ID_EMPTY) {
						return false;
					} else { return true; }
				} catch {
					return false; // TODO: design time - remove in final version
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Store order info to session variable only if it is already persisted
		/// (has OrderId) and a transactional operation should be performed that involve several pages
		/// </remarks>
		private bool _storeOrderInfoInSessionVariable = false;
		public virtual bool StoreOrderInfoInSessionVariable {
			get { 
				//return false;
				
				
				try {
					//return (bool)context.Session["StoreOrderInfoInSession"]; 
					return _storeOrderInfoInSessionVariable;
				} catch {
					return false; //TODO: this is a design-time construction, remove this
				}
			}
			set {
				//context.Session["StoreOrderInfoInSession"] = value;
				_storeOrderInfoInSessionVariable = value;
			}
		}

		public OrderInfo OrderInfoFromSessionVar {
			get {
				try {
					return (OrderInfo)context.Session["CurrentOrderInfo"];
				} catch { return null; }
			}
			set { 
				context.Session["CurrentOrderInfo"] = value;
				/*if (context.Session["CurrentOrderInfo"]!=null) {
					StoreOrderInfoInSessionVariable = true;
				} else {
					StoreOrderInfoInSessionVariable = false;
				}*/
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>HACK: Hack for controls those are contained by another control 
		/// derived from <code>OrderInfoControlBase</code> to avoid loading 
		/// the same order info again
		/// </remarks>
		/// <param name="orderInfo"></param>
		public void AssignOrderInfo(ref OrderInfo orderInfo) {
			if (orderInfo==null) {
				throw new ArgumentNullException("orderInfo");
			} else {
				this.OrderId = orderInfo.OrderId;
				this._orderInfo = orderInfo;
			}
		}

		public void ReplicateOrderInfo(ref IOrderInfoManagerBase ctl) {
			if (this.OrderId==PersistentBusinessEntity.ID_EMPTY) {
				return; //nothing to do in this case
			} else {
				ctl.StoreOrderInfoInSessionVariable = this.StoreOrderInfoInSessionVariable;
				if (!this.StoreOrderInfoInSessionVariable) {
					if (_orderInfo!=null) {
						ctl.AssignOrderInfo(ref this._orderInfo);
					}
				}
			}
		}


		public OrderInfo GetOrderInfo() {
			if (StoreOrderInfoInSessionVariable) {
				_orderInfo = OrderInfoFromSessionVar;
			} else {

				if (_orderInfo==null) {
					if (OrderId==PersistentBusinessEntity.ID_EMPTY) {
						//_orderInfo = null;
					} else {
						// if order info stored to the session get it from the session variable
						/*if (StoreOrderInfoInSessionVariable) {
							_orderInfo = OrderInfoFromSessionVar;
						} else {*/
							// else load it from db 
							_orderInfo = (new OrderFacade()).GetOrderInfo(OrderId);
						//}
					}
				}
			}
			return _orderInfo;
		}

		public void InvalidateOrderInfo() {
			_orderInfo = null;
		}
	}
}
