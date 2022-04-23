namespace HiRes.Web.Common.Controls {
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using HiRes.BusinessFacade;
	using HiRes.Common;
	using HiRes.Web.Common.Components;

/*	
	public class OrderingItemRenderEventArgs : EventArgs {
		public OrderingItemInfo DataItem;
		public OrderingItemRenderEventArgs(OrderingItemInfo dataItem) {
			this.DataItem = dataItem;
		}
	}

	[Serializable]
	public delegate void OrderingItemRenderEventHandler(
		object sender,
		OrderingItemRenderEventArgs e
	);
*/
	/// <summary>
	///		Summary description for AltPrintingTypePrices.
	/// </summary>
	public class OrderingItemsCtl : BaseOrderingItemCtl {

//		public event OrderingItemRenderEventHandler OrderingItemRender;

		protected System.Web.UI.WebControls.Label outputLabel;
		protected System.Web.UI.WebControls.Label debugLabel;
		//protected System.Web.UI.WebControls.DropDownList drPaperTypes;

		#region properties
		
		private PrintingPrice[] _price; // use this member to store pricing info in it once per page loading
		
		private int _selectedPaperTypeID;

		public int SelectedPaperTypeID {
			get { return _selectedPaperTypeID; }
			set { _selectedPaperTypeID = value; }
		}
		
		private String _targetURL;

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
		/// <summary>
		/// Getter is overrided to do not retrieve pricing info from cache but directly from db.
		/// </summary>
		public override PrintingPrice[] Price {
			get {
				if (_price==null) {
					// if "WholesaleCode" property is set then return the corresponding wholesale prices
					if (DisplayWholesalePrices) {
						if ((_wholesaleCode==null)||(_wholesaleCode.Equals(String.Empty))) {
							_price = (new PrintingsFacade()).GetOrderedCommonWholesalePrices(_printingTypeId);
						} else {
							_price = (new PrintingsFacade()).GetOrderedWholesalePrices(_wholesaleCode,_printingTypeId);
						}

					} else {
						_price = (new PrintingsFacade()).GetOrderedPrices(_printingTypeId);
					}
				}
				return _price;
			}
		}

		private string _wholesaleCode;

		public string WholesaleCode {
			get { return _wholesaleCode;}
			set { _wholesaleCode = value; }
		}
		private bool _displayWholesalePrices;
		public bool DisplayWholesalePrices {
			get { return _displayWholesalePrices; }
			set { _displayWholesalePrices = value; }		
		}
		#endregion
		/// <summary>
		/// Render prices for given printing type and paper type.
		/// It's assumed that the price info in Price property is sorted by Quantity, PaperSizeId and PaperTypeId
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output) {
			this.PaperTypeId = SelectedPaperTypeID;//(SelectedPaperTypeID!=PersistentBusinessEntity.ID_EMPTY?SelectedPaperTypeID:CacheManager.PaperTypes(PrintingTypeId,PaperTypes[0].);
			int psX = 0;
			bool isNetscape = Request.Browser.Browser.ToString()=="Netscape"?true:false;
			String html;
			if ((Price==null)||(PrintingType==null)||(PaperSizes==null)||(this.Quantities==null)) {
				html ="<strong>";
				html += "Info is temporarily unavailable.";
				html += "</strong>";
			} else {
				
				html = "<table width=\"100%\" class=\"tablePrintingTypePrices\" cellpadding=\"3\" cellspacing=\"0\">";
				if (isNetscape) {
					html = "<table width=\"100%\" border=\"3\" class=\"nTable\" bordercolor=\"black\" cellpadding=\"3\" cellspacing=\"0\">";
				}
				if (!isNetscape){
					html +="<tr>"+
						"<td bgcolor=\"EEECE3\" align=\"left\" width=\"128\" class='tdPrintingTypePricesTitle'><strong>Quantity</strong></td>";
				}
				else {
					html +="<tr>"+
						"<td bgcolor=\"EEECE3\" class='tdPrintingTypePricesTitleN4' align=\"left\" width=\"128\"><strong>Quantity</strong></td>";
				}

				foreach (PaperSizeInfo papersize in PaperSizes) {
					html += "<td bgcolor=\"EEECE3\" align=\"center\" class='tdPrintingTypePricesTitleEl'><strong>"+papersize.PaperSizeName+"</strong></td>";
					psX++;
				}
				//----------------------

				int i;
				// skip paper types till find 
				for (i=0;(Price[i].PaperTypeID!=PaperTypeId)&&(i<Price.Length);i++);
				if (i==Price.Length) {
					return; //FIXME
				}
				int sizeIndex;
				foreach ( OrderQuantityInfo q in Quantities) {

					html += "<tr>";
					if (!isNetscape) html += "<td bgcolor=\"EEECE3\" align=\"left\" width=\"128\" class='tdPrintingTypePricesNo'><table cellpadding=0 cellspacing=0 border=0 width=128><tr><td>"+q.Quantity+"</td></tr></table></td>";
					else html += "<td bgcolor=\"EEECE3\" align=\"left\" width=\"128\" ><table cellpadding=0 cellspacing=0 border=0 width=128><tr><td>"+q.Quantity+"</td></tr></table></td>";
					
					foreach (PaperSizeInfo papersize in PaperSizes) {
						for (;(Price[i].PaperTypeID!=PaperTypeId)&&(i<Price.Length);i++);		
						sizeIndex=0;
						if (Price[i].Quantity != q.Quantity) {
							if (!isNetscape) html += "<td align=\"center\" class='tdPrintingTypePrices'> N/A</td>";
							else html += "<td align=\"center\"> N/A</td>";
						} else {
							if (Price[i].PaperSizeID == papersize.PaperSizeID) {
								if (Price[i].Price != -1) {
									if (Price[i].IsSpecial) {
										if (!isNetscape) html += "<td align=\"center\" class='tdPrintingTypePrices'><a href='#' class='aPrintingTypePricesSep' onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a>*</td>";
										else html += "<td align=\"center\" class='tdPrintingTypePricesN4'><a href='#' class='aPrintingTypePricesSep' onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a>*</td>";
									} else {
										if (!isNetscape) html += "<td align=\"center\" valign=middle class='tdPrintingTypePrices'><a class='aPrintingTypePrices' href=\"#\" onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a></td>";
										else html += "<td align=\"center\" valign=middle class='tdPrintingTypePricesN4'><a class='aPrintingTypePrices' href=\"#\" onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a></td>";
										

									}
								} else {
									if (!isNetscape) html += "<td align=\"center\" class='tdPrintingTypePrices'>Call us</td>";
									else html += "<td align=\"center\">Call us</td>";
								}
								i++;
							} else {
								//						html += "<td colspan='"+(PaperSizes.Length-sizeIndex).ToString()+"'>&nbsp;</td>";
								if (!isNetscape) html += "<td class='tdPrintingTypePrices'>N/A"+Price[i].ToString()+"</td>";
								else html += "<td>N/A"+Price[i].ToString()+"</td>";
							}
							sizeIndex++;
						}
					}
					html += "</tr>";
				}
				//----------------------				
				html += "</table>";
			}
			
			//outputLabel.Text = html;
			output.Write(html);
			//output.Write(html);
			this.RenderChildren(output);
			JSReg();

		}

		public virtual void JSReg() {
			String JS;
			JS =  @"
				<script language=""javascript"">
				<!--
					function goURL(prTypeId,quantity,paperSizeId,paperTypeId) {
							var _target = '"+Target+@"';
							win = open('"+TargetURL+@"?"+QSParams.PMT_PrintingTypeId+@"='+prTypeId+'&"+QSParams.PMT_PaperTypeId+@"='+paperTypeId+'&"+QSParams.PMT_quantity+@"='+quantity+'&"+QSParams.PMT_PaperSizeId+@"='+paperSizeId,'"+Target+@"','"+WindowProperty+@"');
							if (_target == '_blank') {
								if (win != null && win.opener == null){
									win.opener = window;
								}
							}
					}
				// -->
				</script>
				";
			Page.RegisterStartupScript("goUrl",JS);
		}
	}
}
