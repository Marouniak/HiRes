using System;

using HiRes.Common;
using HiRes.BusinessFacade;
using HiRes.Web.Common;
using HiRes.Web.Common.Components;

namespace HiRes.Web.Common.Controls {
	/// <summary>
	/// Summary description for OrderDesignPartControlBase.
	/// </summary>
	public /*abstract*/ class OrderDesignPartsControlBase : OrderInfoControlBaseCommon {

		protected String GetPartName(int partId) {
			OrderInfo orderInfo = GetOrderInfo();
			/*if (orderInfo==null) {
				orderInfo = CurrentOrder();
			}*/
			return ((PrintingTypePart)CacheManager.PrintingParts(orderInfo.OrderJob.PrintingTypeID)[partId]).PartName;
		}
		protected string PartDesignFileUrl(PartDesign pd, PartDesignFileCategory uploadFileCategory ) {
			UriBuilder ub = new UriBuilder(UrlBase);

			ub.Path += "/files/"+pd.FileName;
			string qs = "category="+FileRequestHandler.PARTDESIGN;
			qs += "&partDesignFileCategory="+((int)uploadFileCategory).ToString();
			qs += "&OrderId="+OrderId;
			qs += "&PartId="+pd.PartId;
			ub.Query = qs;
			return ub.Uri.ToString();
		}
	}
}
