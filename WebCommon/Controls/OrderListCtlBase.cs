using System;
using System.Text;
using HiRes.Common;
using HiRes.Web.Common.Components;

namespace HiRes.Web.Common.Controls {
	/// <summary>
	/// Summary description for OrderListCtlBase.
	/// </summary>
	public class OrderListCtlBase : ControlBase {
		
		protected const string MSG_Failed ="Sorry. Service is temorary unavailable";

		private const string PICKUP_SHORT_DESC = "Pick up";
		private const string SHIPPING_SHORT_DESC = "Shipping";

		/// <summary>
		/// Order filtering expression to use to retrieve orders list
		/// </summary>
		public virtual FilterExpression FilterExpression {
			get {
				return (FilterExpression)ViewState["OrdersFilterExpression"];
			}
			set {
				ViewState["OrdersFilterExpression"] = value;
			}
		}

		/// <summary>
		/// Sort field
		/// </summary>
		public OrderFields OrderBy {
			get {
				if(ViewState["OrderBy"] == null) {
					return OrderFields.NONE;
				} else {
					return (OrderFields) ViewState["OrderBy"];
				}
			}
			set {
				ViewState["OrderBy"] = value;
			}
		}

		/// <summary>
		/// Sort order (ascending or descending)
		/// </summary>
		public bool OrderAsc {
			get {
				if(ViewState["OrderAsc"] == null) {
					return false;
				} else {
					return (bool) ViewState["OrderAsc"];
				}
			}
			set {
				ViewState["OrderAsc"] = value;
			}
		}

		//protected virtual string GetPaperTypeName(
		protected virtual string GetJobDescription(OrderInfo orderInfo) {
			try {
				StringBuilder sb = new StringBuilder(orderInfo.OrderJob.Quantity.ToString());
				sb.Append(" ");
				sb.Append(CacheManager.PrintingTypesNames[orderInfo.OrderJob.PrintingTypeID.ToString()]);
				sb.Append(" ");
				if (orderInfo.OrderJob.IsCustomPaperSize) {
					sb.Append(orderInfo.OrderJob.CustomPaperSize);
				} else {
					sb.Append(((PaperSizeInfo)CacheManager.PaperSizes(orderInfo.OrderJob.PrintingTypeID)[orderInfo.OrderJob.PaperSizeID]).PaperSizeName);
				}
				/*sb.Append("<br><b>");
				switch (orderInfo.OrderJob.JobType) {
					case JobType.DesignAndPrinting:
						sb.Append("Design is ordered");
						break;
				}
				sb.Append("</b>");*/

				return sb.ToString();
			} catch (Exception ex) {
				return string.Empty; // development-time
			}
		}
		/// <summary>
		/// TODO: change descriptions 
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <returns></returns>
		protected virtual string GetOrderStatusDescription(OrderInfo orderInfo) {
			String res = "";
			switch (orderInfo.Status) {
				/*case OrderStatus.New: res = "New"; break;*/
					/*				case OrderStatus.New_Ordering: break;
									case OrderStatus.New_Ordering_IncompleteInfo: break;*/
				case OrderStatus.New_WaitingUpload:
					res = "Pending Upload";
					break;
				case OrderStatus.New_DesignIsUploaded:
					res = "Design is Uploaded";
					break;

				case OrderStatus.WaitingForProof: 
					res = "Pending Proof";
					break;
				case OrderStatus.Approved: 
					res = "Approved";
					break;
				case OrderStatus.InDesign: 
					res = "In Design";
					break;

				case OrderStatus.InPrint: 
					res = "In print";
					break;
				case OrderStatus.Printed: 
					res = "Printed";
					break;
/*				
				case OrderStatus.Shipping: 
					res = "Shipping";
					break;
				case OrderStatus.WaitingForPickup: 
					res = "Pick Up";
					break;
				case OrderStatus.Shipped: 
					res = "Shipped";
					break;
*/
				case OrderStatus.Shipped_WaitingPickUp: 
					if (orderInfo.DeliveryDetails.PickUpOrder) {
						res = "Waiting for pick up";
					} else { res = "Shipped"; }
					break;

				case OrderStatus.Delivered_PickedUp: 
					if (orderInfo.DeliveryDetails.PickUpOrder) {
						res = "Picked Up";
					} else { res = "Delivered"; }
					break;

				case OrderStatus.Cancelled: 
					res = "Cancelled";
					break;				
				case OrderStatus.Closed: 
					res = "Closed";
					break;
				default: break;
			}
			return res;
		}

		protected String GetArtworkStatusHtml(OrderInfo orderInfo) {
			String res = "";
			switch (orderInfo.Status) {
				case OrderStatus.New_WaitingUpload: 
					res += "<a href='"+UrlBase+"/UploadDesign.aspx?OrderID="+orderInfo.OrderId+"'>Upload it now</a>";
					break;
				case OrderStatus.New_DesignIsUploaded:
					res += "Uploaded";
					break;
				case OrderStatus.InDesign:
					res += "In design";
					break;
				case OrderStatus.WaitingForProof:
					res += "Proof it now";
					break;
				case OrderStatus.Approved:
					res += "Approved";
					break;
				default: 
					res += "";
					break;
			}
			return res;
		}

		protected virtual string GetShipmentShortDescription(OrderInfo orderInfo) {
			if (orderInfo.DeliveryDetails.PickUpOrder) {
				return PICKUP_SHORT_DESC;
			} else {
				return SHIPPING_SHORT_DESC;
			}
		}		

	}
}
