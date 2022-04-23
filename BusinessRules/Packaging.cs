using System;
using System.Collections;

using HiRes.Common;
using HiRes.DAL;
using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessRules {

	public class PackagingException : Exception {
		public PackagingException (string msg) : base(msg) {}
	}
	/// <summary>
	/// Summary description for Packaging.
	/// </summary>
	public class Packaging {
		public const int BASE_PACKAGING_QUANTITY = 5000;

		public Packaging() {
		}
		
		/// <summary>
		/// Outdated: - if no corresponding info in the db then it tries to determine the packaging based on base quantity info
		/// </summary>
		/// <param name="carrier"></param>
		/// <param name="jobInfo"></param>
		/// <returns></returns>
		/// <remarks>For use only by HiResPrintings Site. Consider removing it somewhere.</remarks>

		public PackagingInfo Determine(PostalCarrier carrier, OrderInfo.JobInfo jobInfo) {
			using (PackagingDAL PDal = new PackagingDAL()) {
				PackagingInfo packaging = PDal.GetPackagingInfo(
							jobInfo.PrintingTypeID, jobInfo.PaperSizeID, 
							jobInfo.PaperTypeID, carrier, jobInfo.Quantity);
				if (packaging==null) {
					if (jobInfo.Quantity != BASE_PACKAGING_QUANTITY) {
							
						packaging = PDal.GetPackagingInfo (
							jobInfo.PrintingTypeID, jobInfo.PaperSizeID, 
							jobInfo.PaperTypeID, carrier, BASE_PACKAGING_QUANTITY); 
						
						if (packaging != null) {
							if (jobInfo.Quantity > BASE_PACKAGING_QUANTITY) {
								packaging.BoxesNumber = jobInfo.Quantity / BASE_PACKAGING_QUANTITY;
								if ((jobInfo.Quantity % BASE_PACKAGING_QUANTITY)>0) {
									//FIXME: consider this case e.g. base=5000 quantity = 7500
									packaging.BoxesNumber++;
								}
							} else {
								packaging.BoxesNumber = 1;
							}
						}
					}
				}
				// if packaging still == null then something went wrong
				if (packaging==null) {
					AppLog.LogError("Can't determine what packaging to use.");
					throw new PackagingException("Can't determine what packaging to use.");
				}
				return packaging;
			}
		}
		
		public PackagingInfo GetPackagingInfo (PostalCarrier carrier, OrderInfo.JobInfo jobInfo) {
			using (PackagingDAL PDal = new PackagingDAL()) {
				PackagingInfo packaging = PDal.GetPackagingInfo(
					jobInfo.PrintingTypeID, jobInfo.PaperSizeID, 
					jobInfo.PaperTypeID, carrier, jobInfo.Quantity);
				return packaging;
			}
		}
		public bool AddPackagingInfo(PackagingInfo pInfo) {
			bool res;
			using (PackagingDAL pDal = new PackagingDAL()){
				res = pDal.AddPackagingInfo(pInfo);
			}
			return res;
	}


		public bool UpdatePackagingInfo(PackagingInfo pInfo) {
			bool res;
			using (PackagingDAL pDal = new PackagingDAL()){
				res = pDal.UpdatePackagingInfo(pInfo);
			}
			return res;
		}



		internal ArrayList GetPackagings(int printingTypeID, int paperSizeID, int paperTypeID, int carrierID, int quantity, OrderExpression orderBy) {
			using (PackagingDAL packagingDAL = new PackagingDAL()) {
				FilterExpression fexp = null;
				if ((printingTypeID!=PrintingPrice.ALL_ITEMS)||(paperTypeID!=PrintingPrice.ALL_ITEMS)||(paperSizeID!=PrintingPrice.ALL_ITEMS)||(carrierID!=PrintingPrice.ALL_ITEMS)||(quantity!=PrintingPrice.ALL_ITEMS)) {
					fexp = new FilterExpression(typeof(PackagingsFields));
					if (printingTypeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PackagingsFields.PrintingTypeID,printingTypeID);
					}
					if (paperSizeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PackagingsFields.PaperSizeID,paperSizeID);
					}
					if (paperTypeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PackagingsFields.PaperTypeID,paperTypeID);
					}
					if (carrierID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PackagingsFields.CarrierID,carrierID);
					}
					if (quantity!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PackagingsFields.Quantity,quantity);
					}
				}
				return packagingDAL.GetPackagings(fexp, orderBy);
			}	
		}

		public void GetPackagings(int printingTypeID, int paperSizeID, int paperTypeID, int carrierID, int quantity, OrderExpression orderBy, out PackagingInfo[] packagings) {
			//PackagingInfo[] res;
			packagings = null;
			ArrayList packagingsList = GetPackagings(printingTypeID, paperSizeID, paperTypeID, carrierID, quantity, orderBy);
			packagingsList.CopyTo(packagings);
			//return res;
		}

		public void GetPackagings(int printingTypeID, int paperSizeID, int paperTypeID, int carrierID, int quantity, out Hashtable packagings) {
			//PackagingInfo[] res;
			packagings = new Hashtable();
			ArrayList packagingsList = GetPackagings(printingTypeID, paperSizeID, paperTypeID, carrierID, quantity, null);
			//packagingsList.CopyTo(packagings);
			for ( int i=0; i<packagingsList.Count; i++) {
				PackagingInfo pi = (PackagingInfo)packagingsList[i];
				packagings.Add(PackagingInfo.GetKey(pi),pi);
			}

			//return res;
		}

	}
}
