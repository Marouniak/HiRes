using System;
using System.Collections;
using System.Text;
using System.Web.Caching;
using System.Web.UI;

using HiRes.BusinessFacade;
using HiRes.Common;

namespace HiRes.Web.Common.Controls {
	/// <summary>
	/// Summary description for PriceBaseCtl.
	/// </summary>
	public /*abstract*/ class BaseOrderingItemCtl : OrderInfoControlBaseCommon /*ControlBase*/ {
		
		public const String KEY_CACHED_PRINTINGTYPE = "PrintingType:PrintingTypeID:";
		public const String KEY_CACHED_PRICE = "PrintingTypes:Price:PrintingTypeID:";
		
		protected int _printingTypeId = PersistentBusinessEntity.ID_EMPTY;
		protected int _paperTypeId = PersistentBusinessEntity.ID_EMPTY;

		private String _cacheKey;
		private String _cachedPriceKey;

		private void Page_Load(object sender, System.EventArgs e) {
			// Put user code to initialize the page here
		}

		private void EnsurePrintingTypeInfoIsCached() {
			if (Cache[CacheKey]==null) {
				Cache[CacheKey] = (new PrintingsFacade()).GetPrintingTypeInfo(_printingTypeId);
			}
		}

		#region properties

		private string CacheKey {
			get {
				if (_cacheKey==null) {
					if (PrintingTypeId==PersistentBusinessEntity.ID_EMPTY) {
						throw new ArgumentException("PrintingTypeId should be set before getting printing info", "PrintingTypeId");
					}
					_cacheKey = ((new StringBuilder(KEY_CACHED_PRINTINGTYPE,KEY_CACHED_PRINTINGTYPE.Length+10)).Append(PrintingTypeId)).ToString();
				}
				return _cacheKey;
			}
		}
		private string CachedPriceKey {
			get {
				if (_cachedPriceKey==null) {
					if (PrintingTypeId==PersistentBusinessEntity.ID_EMPTY) {
						throw new ArgumentException("PrintingTypeId should be set before getting printing info", "PrintingTypeId");
					}
					_cachedPriceKey = ((new StringBuilder(KEY_CACHED_PRICE,KEY_CACHED_PRINTINGTYPE.Length+10)).Append(PrintingTypeId)).ToString();
				}
				return _cachedPriceKey;
			}
		}

		public int PrintingTypeId {
			get { return _printingTypeId; }
			set { 
				_printingTypeId = value;
/*				_cacheKey = ((new StringBuilder(KEY_CACHED_PRINTINGTYPE,KEY_CACHED_PRINTINGTYPE.Length+10)).Append(value)).ToString();
				_cachedPriceKey = ((new StringBuilder(KEY_CACHED_PRICE,KEY_CACHED_PRINTINGTYPE.Length+10)).Append(value)).ToString();
*/				
				EnsurePrintingTypeInfoIsCached();
				if ((PrintingType!=null)&&(PrintingType.PaperSizes!=null)&&(PrintingType.PaperSizes[0].PaperTypes!=null)) {
					_paperTypeId = PrintingType.PaperSizes[0].PaperTypes[0].PaperTypeID;
				} else { _paperTypeId = PersistentBusinessEntity.ID_EMPTY;}
			}
		}

		public int PaperTypeId {
			get { return _paperTypeId; }
			set { _paperTypeId = value; }
		}
		public PrintingTypeInfo PrintingType {
			get {
				EnsurePrintingTypeInfoIsCached();
				return (PrintingTypeInfo)Cache[_cacheKey];
			}
		}


		public PaperSizeInfo[] PaperSizes {
			get {
				EnsurePrintingTypeInfoIsCached();
				return ((PrintingTypeInfo)Cache[CacheKey]).PaperSizes;
			}
		} 

		public PaperTypeInfo[] PaperTypes {
			get {
				//FIXME:
				if ((this.PaperSizes==null)||(this.PaperSizes[0]==null)) return null;
				return this.PaperSizes[0].PaperTypes;
				//throw new Exception("Outdated method calling");
			}
		}

		public Hashtable/*ExtraInfo[]*/ Extras {
			get {
				EnsurePrintingTypeInfoIsCached();
				return ((PrintingTypeInfo)Cache[CacheKey]).Extras;
			}		
		}

		public OrderQuantityInfo[] Quantities {
			get {
				EnsurePrintingTypeInfoIsCached();
				return ((PrintingTypeInfo)Cache[CacheKey]).OrderQuantities;
			}		
		}

		public virtual PrintingPrice[] Price {
			get {
				if (Cache[CachedPriceKey]==null) {
					PrintingPrice[] price = (new PrintingsFacade()).GetOrderedPrices(_printingTypeId);
					if (price!=null) {
						//Cache.Add(CachedPriceKey,price,new CacheDependency(
						Cache[CachedPriceKey] = price;
					}
					return price;
				} else {
					return (PrintingPrice[])Cache[CachedPriceKey];
				}
			}		
		}
	
		#endregion

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
