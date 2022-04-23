namespace HiRes.Web.Common.Controls {

	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using HiRes.Common;
	using HiRes.Web.Common.Components;

	public class OrderingItemsNavigatorBase : ControlBase {
		protected HiRes.Web.Common.Controls.PrintingTypesTabNavCtl tabNavCtl;
		protected HiRes.Web.Common.Controls.OrderingItemsCtl itemsCtl;
		protected HiRes.Web.Common.Controls.PaperTypeNavigatorCtl ctlPaperTypesNav;
		
		private String _targetURL;

		#region request params
		protected int _printingTypeId = PersistentBusinessEntity.ID_EMPTY;
		protected int _paperTypeId = PersistentBusinessEntity.ID_EMPTY;
		protected int _paperSizeId = PersistentBusinessEntity.ID_EMPTY;
		protected int _quantity = PersistentBusinessEntity.ID_EMPTY;
		#endregion

		public String TargetURL {
			get { return _targetURL; }
			set { _targetURL = value; }
		}

		private String _target;

		public String Target {
			get { return _target; }
			set { _target = value; }
		}

		private String _windowProperty;

		public String WindowProperty {
			get { return _windowProperty; }
			set { _windowProperty = value; }
		}



		protected virtual void Page_Load(object sender, System.EventArgs e) {

			itemsCtl.Target=this.Target;
			itemsCtl.TargetURL=this.TargetURL;
			itemsCtl.WindowProperty=this.WindowProperty;

			tabNavCtl.baseRedirectUrl = Page.Request.Url.ToString();
			tabNavCtl.TabsCount = tabNavCtl.Printings.Length;

			if (_printingTypeId == PersistentBusinessEntity.ID_EMPTY) {
				if ((tabNavCtl.Printings!=null)&&(tabNavCtl.Printings.Length>0)) {
					ctlPaperTypesNav.SelectedPrintingTypeID = tabNavCtl.Printings[0].PrintingTypeID; //FIXME: remove hardcoding
					itemsCtl.PrintingTypeId = tabNavCtl.Printings[0].PrintingTypeID;//FIXME: remove hardcoding
					tabNavCtl.SelectedPrintingTypeID = tabNavCtl.Printings[0].PrintingTypeID; //FIXME: remove hardcoding
					_printingTypeId = tabNavCtl.Printings[0].PrintingTypeID; //FIXME: remove hardcoding
				}
			} else {
				UriBuilder uribuilder = new UriBuilder(Page.Request.Url);
				uribuilder.Query = "";
				tabNavCtl.baseRedirectUrl = uribuilder.ToString();
				ctlPaperTypesNav.BaseRedirectUrl = uribuilder.ToString();

				ctlPaperTypesNav.SelectedPrintingTypeID = itemsCtl.PrintingTypeId = _printingTypeId;
				tabNavCtl.SelectedPrintingTypeID = itemsCtl.PrintingTypeId = _printingTypeId;
			}

			if (_paperTypeId == PersistentBusinessEntity.ID_EMPTY) {
				//FIXME: remove this hardcoding - make both controls handling PersistentBusinessEntity.ID_EMPTY param value
				//IEnumerator enumerator = CacheManager.AllPaperTypes(_printingTypeId).Keys.GetEnumerator();
				//enumerator.MoveNext();

				IDictionaryEnumerator enumerator = CacheManager.AllPaperTypes(_printingTypeId).GetEnumerator();
				enumerator.MoveNext();
				ctlPaperTypesNav.SelectedPaperTypeID = itemsCtl.SelectedPaperTypeID = ((PaperTypeInfo)enumerator.Value).PaperTypeID;//((PaperTypeInfo)CacheManager.AllPaperTypes(_printingTypeId).GetEnumerator().Value).PaperTypeID;
				
			} else {
				itemsCtl.SelectedPaperTypeID = _paperTypeId;
				ctlPaperTypesNav.SelectedPaperTypeID = _paperTypeId;
			}

		}

		protected virtual bool CheckParams() {
			try {
				if (Request.QueryString[QSParams.PMT_PrintingTypeId]!=null) {
					_printingTypeId = Int32.Parse(Request.QueryString[QSParams.PMT_PrintingTypeId]);
				}
				if (Request.QueryString[QSParams.PMT_PaperTypeId]!=null) {
					_paperTypeId = Int32.Parse(Request.QueryString[QSParams.PMT_PaperTypeId]);
				}
				/*				if (Request.QueryString[QSParams.PMT_PaperSizeId]!=null) {
									_paperSizeId = Int32.Parse(Request.QueryString[QSParams.PMT_PaperSizeId]);
								}
								if (Request.QueryString[QSParams.PMT_quantity]!=null) {
									_quantity = Int32.Parse(Request.QueryString[QSParams.PMT_quantity]);
								}
				*/				
				
				return true;
			} catch {
				return false;
			}
		}

/*		protected override void OnLoad(EventArgs e) {
			this.CheckIfUserAuthorizedToOperateWithOrder(e);
			// TODO: add checking if customer is authorized to work with order (if OrderId is set).
			base.OnLoad(e);
		}
*/
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
