using System;
using System.Collections;
using System.Data;
using System.Drawing;

using System.Web;

using HiRes.BusinessFacade;
using HiRes.Common;

using WebSupergoo.ABCUpload5;

namespace HiRes.Web.Common.Components {
	/// <summary>
	/// Summary description for ProcessUploadHelper.
	/// </summary>
	public class ProcessUploadHelper {
		public const /*static*/ string AUXFILE = "AUX";
		public const /*static*/ string PARTDESIGN = "PARTDESIGN";
		protected string UploadDir;

		public ProcessUploadHelper(string uploadDir) {
			this.UploadDir = uploadDir;
		}

		/*protected OrderInfo GetOrderInfo(int orderId) {
			if (_orderInfo==null)
				_orderInfo = (new OrderFacade()).GetOrderInfo(orderId);

			return _orderInfo;
		}*/

		public bool ProcessPartDesignUpload(HttpContext context, int orderId, PartDesignFileCategory partDesignFileCategory, bool saveToDb,string inputCtlPrefix ) {
			//try {
				Upload theUpload = new Upload();
				UploadedFile theFile;
				//PartDesign[] parts;
				//int orderId = Int32.Parse(context.Request["orderId"]);
				if (orderId==PersistentBusinessEntity.ID_EMPTY) {
					throw new ArgumentOutOfRangeException("orderId");
				}
				if (inputCtlPrefix==null) {
					throw new ArgumentNullException("inputCtlPrefix");
				}
				OrderFacade ofacade = new OrderFacade();
				OrderInfo orderInfo = ofacade.GetOrderInfo(orderId);

				PartDesign[] parts = null;
				switch (partDesignFileCategory) {
					case PartDesignFileCategory.DesignPreview:
						ofacade.LoadPartDesignPreviews(ref orderInfo);
						parts = orderInfo.Design.PartPreviews;
						break;
					case PartDesignFileCategory.CompletedDesign:
						ofacade.LoadPartDesign(ref orderInfo);
						parts = orderInfo.Design.Parts;
						break;
				}

				EnsureUploadDir(); 
				//string uploadPath = UploadVirtualPath(context,orderInfo.CustomerID, orderInfo.OrderId);
				foreach (PartDesign part in parts) {
					theFile = theUpload.Files[inputCtlPrefix+part.PartId];
					if ((theFile != null) && (theFile.ContentLength > 0)) {
						if ((theFile.TempFile == "") || (theFile.MacBinary == true)) // save using URL

							theFile.SaveAs(UploadDir+"/"+theFile.WinSafeFileName);
						else { // faster to move than copy

							string thePath = UploadDir+"/" + theFile.WinSafeFileName;
							if (System.IO.File.Exists(thePath))
								System.IO.File.Delete(thePath);
							System.IO.File.Move(theFile.Detach(), thePath);
						}
						part.FileCategory = partDesignFileCategory;
						part.FileName = theFile.WinSafeFileName;
						part.IsModified = true; 
					} else {
						if (part.IsEmpty) {
							part.IsModified = true;
						} else {
							part.IsModified = false;
						}
					}
				}
				if (saveToDb) {
					if (UpdateData(context,orderInfo,partDesignFileCategory,PARTDESIGN)) {
						return true;
					} else { return false; }
				} else { return true; }
			/*} catch {
				return false;
			}*/
		}

		private bool UpdateData(HttpContext context, OrderInfo orderInfo, PartDesignFileCategory partDesignFileCategory, string category) {
			//OrderInfo orderInfo = GetOrderInfo(orderId);
			// TODO: consider to precide UploadVirtualPath with UrlBase
			string filesDir = UploadDir;//context.Server.MapPath(/*UrlBase+"/"+*/UploadVirtualPath);
			if (category.Equals(PARTDESIGN)) {
				switch (partDesignFileCategory) {
					case PartDesignFileCategory.DesignPreview:
						return (new OrderFacade()).SavePartDesignPreviews(ref orderInfo, filesDir);
					case PartDesignFileCategory.CompletedDesign:
						return (new OrderFacade()).SavePartsDesign(ref orderInfo, filesDir);
				}
				return false;
			} else if (category.Equals(AUXFILE)) {
				throw new NotImplementedException();
			} else { return false; }
		}

		protected virtual void EnsureUploadDir() {
			//string abspath = context.Server.MapPath(UploadVirtualPath);
			if (System.IO.Directory.Exists(UploadDir)) { 
				return;
			} else {
				System.IO.Directory.CreateDirectory(UploadDir);
			}
		}
//		protected virtual String UploadVirtualPath(HttpContext context,string customerId, int orderId) {
//			return DirectoryManager.GetOrderDirVirtualPath(customerId, orderId);
//		}
	}
}
