using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;

using HiRes.BusinessRules;
using HiRes.Common;
using HiRes.Common.Security;

namespace HiRes.Web.Common {
	/// <summary>
	/// 
	/// </summary>
	public class FileRequestHandler : IHttpHandler {

		public struct FileRequestParams {
			public string Category;
			public PartDesignFileCategory PartDesignFileCategory;
			public int OrderId;
			public int PartId;
			public int AuxFileId;
			public int MailListId;
		}
		
		public const string CATEGORY_PARAM = "category";
		public const string PDFCategory_PARAM = "partDesignFileCategory";
		public const string ORDERID_PARAM = "orderId";
		public const string PARTID_PARAM = "partId";
		
		public const string AUXFILEID_PARAM = "auxFileId";
		public const string MAILLISTID_PARAM = "listId";

		public const string AUXFILE = "AUX";
		public const string PARTDESIGN = "PARTDESIGN";
		public const string MAILLIST = "MAILLIST";

		#region request params
		/*		private string category;
				private PartDesignFileCategory partDesignFileCategory;
				private int orderId = PersistentBusinessEntity.ID_EMPTY;
				private int partId = PersistentBusinessEntity.ID_EMPTY;
		
				private int auxFileId = PersistentBusinessEntity.ID_EMPTY;
		*/
		#endregion
		
		public bool IsReusable {
			get {
				return true;
			}
		}

		static FileRequestHandler() {
			AppConfig.OnApplicationStart();
		}

		public void ProcessRequest (HttpContext context) {
			if (!IsRequestValid(context)) {
				context.Response.StatusCode = 403; // forbidden
				return;
			}
			FileRequestParams requestParams;
			if (!CheckParams(context,out requestParams)) {
				context.Response.StatusCode = 400; //bad request
				return;
			}
			
		
			if (requestParams.Category.ToUpper().Equals(PARTDESIGN)) {
				ProcessPartDesignRequest(context,requestParams);
			} else if (requestParams.Category.ToUpper().Equals(AUXFILE)) {
				//TODO: implement this clause
				ProcessAuxFileRequest(context,requestParams);
			} else {
				if (requestParams.Category.ToUpper().Equals(MAILLIST)) {
					ProcessMailListRequest(context,requestParams);
				} else {
					context.Response.StatusCode = 400; //bad request
					context.Response.End();
					return;
				}
			}
		
		}
		protected void ProcessMailListRequest(HttpContext context, FileRequestParams requestParams) {
			MailingListInfo mli = new MailingList().GetInfo(requestParams.MailListId);
			if ((mli==null)||(mli.MailingListBlob==null)) {
				context.Response.End();
				return;
			}
			context.Response.ContentType = (mli.FileContentType==null)||(mli.FileContentType.Length==0)?"application/octet-stream":mli.FileContentType;
	
			context.Response.OutputStream.Write(mli.MailingListBlob,0,mli.MailingListBlob.Length);
			context.Response.Flush();
			context.Response.End();
		}

		protected void ProcessPartDesignRequest(HttpContext context, FileRequestParams requestParams) {

			byte[] buffer = null;
			switch (requestParams.PartDesignFileCategory) {
				case PartDesignFileCategory.DesignPreview:
					buffer = new Order().GetPreviewPartDesignFile(requestParams.OrderId,requestParams.PartId);
					break;
				case PartDesignFileCategory.CompletedDesign:
					buffer = new Order().GetCompletedPartDesignFile(requestParams.OrderId,requestParams.PartId);
					break;
			}

			
			if (buffer==null) {
				context.Response.Write("Sorry, the requested design is currently unavailable.");
				context.Response.End();
				return;
			}
			context.Response.ContentType = "application/octet-stream";
	
			context.Response.OutputStream.Write(buffer,0,buffer.Length);
			context.Response.Flush();
			context.Response.End();
		}

		protected void ProcessAuxFileRequest(HttpContext context, FileRequestParams requestParams) {
			//byte[] buffer = null;
			AuxFile file = new Order().GetAuxFile(requestParams.AuxFileId);
			if ((file==null)||(file.Blob==null)) {
				context.Response.StatusCode = 404;
				context.Response.End();
				return;
			}
			/*			context.Response.AddHeader("content-disposition",
							"attachement; filename=x.doc");
			  */
			context.Response.ContentType = file.FileContentType;
			context.Response.AddHeader("content-disposition", "inline; filename="+file.FileName);

			context.Response.OutputStream.Write(file.Blob,0,file.Blob.Length);
			context.Response.Flush();
			context.Response.End();
			//TODO: consider adding a permission checking here
		}

		protected bool IsRequestValid(HttpContext context) {
			if (context.User.Identity.IsAuthenticated) {
				if ((context.User.IsInRole(UserRoles.Admin.ToString()))||(context.User.IsInRole(UserRoles.DDEmployee.ToString()))||(context.User.IsInRole(UserRoles.ODEmployee.ToString()))||(context.User.IsInRole(UserRoles.PDEmployee.ToString()))) {
					return true;
				} else {
					//TODO: add checking if customer is authorized to access the file
					// i.e. whether requested order belongs to customer orders
					return true;
				}
				
			} else {
				context.Response.StatusCode = 403;
				context.Response.Write("Whoaaaaa! Show me your ID!");
				return false;
			}
		}

		protected bool CheckParams(HttpContext context, out FileRequestParams requestParams) {
			//requestParams.Category;
			requestParams.PartDesignFileCategory = PartDesignFileCategory.CompletedDesign;
			requestParams.OrderId = PersistentBusinessEntity.ID_EMPTY;
			requestParams.PartId = PersistentBusinessEntity.ID_EMPTY;
			requestParams.AuxFileId = PersistentBusinessEntity.ID_EMPTY;
			requestParams.MailListId = PersistentBusinessEntity.ID_EMPTY;

			requestParams.Category = context.Request[CATEGORY_PARAM];
			if (requestParams.Category==null) {
				return false;
			}

			if (requestParams.Category.Equals(PARTDESIGN)) {
				try {
					requestParams.OrderId = Int32.Parse(context.Request[ORDERID_PARAM]);
					requestParams.PartId =  Int32.Parse(context.Request[PARTID_PARAM]);
				} catch {
					return false;
				}
				try {
					requestParams.PartDesignFileCategory = (PartDesignFileCategory)Enum.Parse(typeof(PartDesignFileCategory),context.Request["partDesignFileCategory"]);
					return true;
				} catch {
					return false;
				}
			} else if (requestParams.Category.Equals(AUXFILE)) {
				//TODO: here should be aux file id
				try {
					//orderId = Int32.Parse(context.Request[ORDERID_PARAM]);
					requestParams.AuxFileId =  Int32.Parse(context.Request[AUXFILEID_PARAM]);
				} catch {
					return false;
				}
	
				return true;
			} else {
				if (requestParams.Category.Equals(MAILLIST)) {
					try {
						requestParams.MailListId =  Int32.Parse(context.Request[MAILLISTID_PARAM]);
						return true;
					} catch {
						return false;
					}

				} else { return false; }
			}
		}


	}
}
