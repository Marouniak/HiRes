using System;
using System.Drawing;
using System.Text;
using System.Web.UI.WebControls;

using HiRes.BusinessFacade;
using HiRes.Common;
using HiRes.Web.Common.Components;

namespace HiRes.Web.Common.Controls {
	/// <summary>
	/// Summary description for OrderInfoControlBase.
	/// </summary>
	public /*abstract*/ class OrderInfoControlBaseCommon : ControlBase, IOrderInfoManagerBase {
		protected const string MSG_INFOSAVED = "Information was successfully updated.";
		protected const string MSG_ERROR = "Error while updating info.";
		//protected System.Web.UI.WebControls.Label lblMessage;

		//protected OrderInfo _orderInfo;
		protected OrderInfoManagerBase orderManager;
	
		public OrderInfoControlBaseCommon():base() {
			orderManager = new OrderInfoManagerBase(this.Context);
		}

		public virtual int OrderId {
			get { 
				return orderManager.OrderId;
			}
			set { orderManager.OrderId = value; }
		}

		/// <summary>
		/// Determine whether the control has an order info bind to it
		/// </summary>
		public bool HasOrderInfo {
			get {
				return orderManager.HasOrderInfo;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Store order info to session variable only if /*obsolete: it is already persisted
		/// (has OrderId) and */ a transactional operation should be performed that involve several pages
		/// </remarks>
		public virtual bool StoreOrderInfoInSessionVariable {
			get {
				return orderManager.StoreOrderInfoInSessionVariable;
			}
			set {
				orderManager.StoreOrderInfoInSessionVariable = value;
			}
		}

		public virtual OrderInfo OrderInfoFromSessionVar {
			get {
				return orderManager.OrderInfoFromSessionVar;
			}
			set {
				orderManager.OrderInfoFromSessionVar = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>HACK: Hack for controls derived from <code>OrderInfoControlBaseCommon</code>
		/// and contained by another control derived from <code>OrderInfoControlBaseCommon</code>
		/// to avoid loading the same order info again
		/// </remarks>
		/// <param name="orderInfo"></param>
		public virtual void AssignOrderInfo(ref OrderInfo orderInfo) {
			orderManager.AssignOrderInfo(ref orderInfo);
		}

		[Obsolete("Use 'ReplicateOrderInfo'",true)]
		protected virtual void ReplicateOrderInfoToChildControl(ref HiRes.Web.Common.Controls.OrderInfoControlBaseCommon ctl) {
			
		}

		protected virtual void ReplicateOrderInfo(ref IOrderInfoManagerBase ctl) {
			orderManager.ReplicateOrderInfo(ref ctl);
		}
		
		protected virtual void ReplicateOrderInfo(ref HiRes.Web.Common.Controls.OrderInfoControlBaseCommon ctl) {
			IOrderInfoManagerBase iManager = (IOrderInfoManagerBase)ctl;
			orderManager.ReplicateOrderInfo(ref iManager);
		}
		
		public virtual OrderInfo GetOrderInfo() {
			return orderManager.GetOrderInfo();
		}

		public virtual void InvalidateOrderInfo() {
			orderManager.InvalidateOrderInfo();
		}
		/*protected override String UploadVirtualPath {
			get {
				throw new NotImplementedException();
			}
		}*/
		
		protected virtual string AuxFileUploadVirtualPath {
			get {
				//TODO: implement this
/*				OrderInfo orderInfo = GetOrderInfo();
				if (orderInfo.OrderId == PersistentBusinessEntity.ID_EMPTY) {
					// if order has not persisted yet return path to the temporary file storage
					return DirectoryManager.GetEmployeeUploadDestinationVirtualPath(Customer.CustomerID);
				} else {
					return DirectoryManager.GetAuxFilesVirtualPath(orderInfo.CustomerID,orderInfo.OrderId);
				}
*/
				throw new NotImplementedException();
				/*
				OrderInfo orderInfo = GetOrderInfo();
				if (orderInfo.OrderId == PersistentBusinessEntity.ID_EMPTY) {
					// if order has not persisted yet return path to the temporary file storage
					return DirectoryManager.GetCustomerUploadDestinationVirtualPath(Customer.CustomerID);
				} else {
					return DirectoryManager.GetAuxFilesVirtualPath(orderInfo.CustomerID,orderInfo.OrderId);
				}
				*/
			}
		}
		
		protected virtual string GetJobDescription(OrderInfo orderInfo) {
			StringBuilder sb = new StringBuilder(orderInfo.OrderJob.Quantity.ToString());
			sb.Append(" ");
			sb.Append(CacheManager.PrintingTypesNames[orderInfo.OrderJob.PrintingTypeID.ToString()]);
			sb.Append(" ");

			if (orderInfo.OrderJob.IsCustomPaperSize) {
				sb.Append(orderInfo.OrderJob.CustomPaperSize);
			} else {
				sb.Append(((PaperSizeInfo)CacheManager.PaperSizes(orderInfo.OrderJob.PrintingTypeID)[orderInfo.OrderJob.PaperSizeID]).PaperSizeName);
			}
			return sb.ToString();
		}

		/*protected virtual string GetShortJobDescription(OrderInfo orderInfo) {
			StringBuilder sb = new StringBuilder(orderInfo.OrderJob.Quantity.ToString());
			sb.Append(" ");
			sb.Append(CacheManager.PrintingTypesNames[orderInfo.OrderJob.PrintingTypeID.ToString()]);
			sb.Append(" ");
			sb.Append(((PaperSizeInfo)CacheManager.PaperSizes(orderInfo.OrderJob.PrintingTypeID)[orderInfo.OrderJob.PaperSizeID]).PaperSizeName);
			sb.Append("<br><b>");
			switch (orderInfo.OrderJob.JobType) {
				case JobType.DesignAndPrinting:
					sb.Append("Design is ordered");
					break;
			}
			sb.Append("</b>");

			return sb.ToString();
		}*/
		#region component initialization		

		protected virtual void CheckIfUserAuthorizedToOperateWithOrder(object sender, System.EventArgs e) {
			if (!IsPostBack) {
				if (OrderId!=PersistentBusinessEntity.ID_EMPTY) {
					//TODO: add checking here	
				}
			}
		}

		override protected void OnInit(EventArgs e) {
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent() {
			this.Load += new System.EventHandler(this.CheckIfUserAuthorizedToOperateWithOrder);
		}

		#endregion 

//		protected virtual Label MessageLabel {
//			get { throw new NotImplementedException(); }
//			set { throw new NotImplementedException(); }
//		}
//		protected void SetMessage(Label lblMessage, string msg, bool isErrorNotification) {
//			lblMessage.Text = msg;
//			lblMessage.ForeColor = (isErrorNotification?lblMessage.ForeColor = Color.Red:Color.Black);
//		}
//
//		protected void SetMessage(string msg, bool isErrorNotification) {
//			Label lblMessage = MessageLabel;
//			lblMessage.Text = msg;
//			lblMessage.ForeColor = (isErrorNotification?lblMessage.ForeColor = Color.Red:Color.Black);
//		}

	}
}
