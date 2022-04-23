using System;
using System.Collections;
using HiRes.Common;
using HiRes.BusinessRules;

namespace HiRes.BusinessFacade {
	/// <summary>
	/// Summary description for PackagingFacade.
	/// </summary>
	public class PackagingFacade {
		
		/// <summary>
		/// Adds new packaging info
		/// </summary>
		/// <param name="pInfo"></param>
		public bool AddPackagingInfo(PackagingInfo pInfo) {
			Packaging packaging = new Packaging();
			return packaging.AddPackagingInfo(pInfo);
		}

		/// <summary>
		/// Updates packaging info. If packaging info doesn't exist, adds
		/// </summary>
		/// <param name="pInfo"></param>
		public bool UpdatePackagingInfo(PackagingInfo pInfo) {
			Packaging packaging = new Packaging();
			return packaging.UpdatePackagingInfo(pInfo);
		}

		public void Determine(ref OrderInfo orderinfo) {
			orderinfo.DeliveryDetails.Packaging = Determine(orderinfo.DeliveryDetails.Carrier, orderinfo.OrderJob);
		}

		public PackagingInfo Determine(PostalCarrier carrier, OrderInfo.JobInfo jobInfo) {
			return (new Packaging()).Determine(carrier, jobInfo);
		}
		
		public PackagingInfo GetPackagingInfo(PostalCarrier carrier, OrderInfo.JobInfo jobInfo) {
			return (new Packaging()).GetPackagingInfo(carrier, jobInfo);
		}

		public PackagingInfo[] GetPackagings(int printingTypeID, int paperSizeID, int paperTypeID, int carrierID, int quantity, OrderExpression orderBy) {
			PackagingInfo[] res;
			(new Packaging()).GetPackagings(printingTypeID, paperSizeID, paperTypeID, carrierID, quantity, orderBy, out res);
			return res;
		}

		public Hashtable GetPackagings(int printingTypeID, int paperSizeID, int paperTypeID, int carrierID, int quantity/*, OrderExpression orderBy*/) {
			Hashtable packagings;
			(new Packaging()).GetPackagings(printingTypeID, paperSizeID, paperTypeID, carrierID, quantity, out packagings);
			return packagings;
		}
	}
}
