using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using HiRes.Common;
using HiRes.Web.Common.Components;

namespace HiRes.Web.Common.Controls {

	public class OrderingItemRenderEventArgs : EventArgs {
		public OrderingItemInfo DataItem;
		public HtmlTextWriter output;
		public OrderingItemRenderEventArgs(OrderingItemInfo dataItem, HtmlTextWriter output) {
			this.DataItem = dataItem;
			this.output = output;
		}
	}
	
	[Serializable]
	public delegate void OrderingItemRenderEventHandler(
	object sender,
	OrderingItemRenderEventArgs e
	);

	
	/// <summary>
	/// 
	/// </summary>
	public class OrderingItemsAdvCtl : OrderingItemsCtl {

		protected OrderingItemRenderEventHandler _itemEvent;

		public event OrderingItemRenderEventHandler OrderingItemRender {
			add {
				_itemEvent += value;
				Console.WriteLine("in add accessor");
			}
			remove {
				_itemEvent -= value;
				Console.WriteLine("in remove accessor");
			}
		}

		//public event OrderingItemRenderEventHandler OrderingItemRender;

		protected override void Render(HtmlTextWriter output) {

			if ((PrintingType==null)||(PaperSizes==null)||(this.Quantities==null)) {
				output.Write("<strong>");
				output.Write("Info is temporarily unavailable.");
				output.Write("</strong>");
				return;
			}
			
			
			this.PaperTypeId = SelectedPaperTypeID;
			int psX = 0;
			bool isNetscape = Request.Browser.Browser.ToString()=="Netscape"?true:false;
			

			output.Write("<table width=\"100%\" class=\"tablePrintingTypePrices\" cellpadding=\"3\" cellspacing=\"0\">");
			
			if (isNetscape) {
				output.Write("<table width=\"100%\" border=\"3\" class=\"nTable\" bordercolor=\"black\" cellpadding=\"3\" cellspacing=\"0\">");
			}

			if (!isNetscape){
				output.Write("<tr><td bgcolor=\"EEECE3\" align=\"left\" width=\"128\" class='tdPrintingTypePricesTitle'><strong>Quantity</strong></td>");
			}
			else {
				output.Write("<tr><td bgcolor=\"EEECE3\" class='tdPrintingTypePricesTitleN4' align=\"left\" width=\"128\"><strong>Quantity</strong></td>");
			}
			foreach (PaperSizeInfo papersize in PaperSizes) {
				output.Write("<td bgcolor=\"EEECE3\" align=\"center\" class='tdPrintingTypePricesTitleEl'><strong>"+papersize.PaperSizeName+"</strong></td>");
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

				output.Write("<tr>");
				if (!isNetscape) output.Write("<td bgcolor=\"EEECE3\" align=\"left\" width=\"128\" class='tdPrintingTypePricesNo'><table cellpadding=0 cellspacing=0 border=0 width=128><tr><td>"+q.Quantity+"</td></tr></table></td>");
				else output.Write("<td bgcolor=\"EEECE3\" align=\"left\" width=\"128\" ><table cellpadding=0 cellspacing=0 border=0 width=128><tr><td>"+q.Quantity+"</td></tr></table></td>");

				
				foreach (PaperSizeInfo papersize in PaperSizes) {
					for (;(Price[i].PaperTypeID!=PaperTypeId)&&(i<Price.Length);i++);		
					sizeIndex=0;
					if (Price[i].Quantity != q.Quantity) {
						if (!isNetscape) {
							output.Write("<td align=\"center\" class='tdPrintingTypePrices'>");
							OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
							_itemEvent(this,args);
							output.Write("</td>");
						}
						else {
							output.Write("<td align=\"center\">");
							OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
							_itemEvent(this,args);
							output.Write("</td>");
						}
					} else {
						if (Price[i].PaperSizeID == papersize.PaperSizeID) {
							if (Price[i].Price != -1) {
								if (Price[i].IsSpecial) {
									if (!isNetscape) {
										output.Write("<td align=\"center\" class='tdPrintingTypePrices'>");
										
										OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
										_itemEvent(this,args);
										output.Write("</td>");
									}
									else {
										//output.Write("<td align=\"center\" class='tdPrintingTypePricesN4'><a href='#' class='aPrintingTypePricesSep' onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a>*</td>");
										output.Write("<td align=\"center\" class='tdPrintingTypePricesN4'>");
										OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
										_itemEvent(this,args);
										output.Write("</td>");
									}
								} else {
									if (!isNetscape) {
										// output.Write("<td align=\"center\" valign=middle class='tdPrintingTypePrices'><a class='aPrintingTypePrices' href=\"#\" onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a></td>");
										output.Write("<td align=\"center\" valign=middle class='tdPrintingTypePrices'>");
										OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
										_itemEvent(this,args);
										output.Write("</td>");
									}
									else{
										//output.Write("<td align=\"center\" valign=middle class='tdPrintingTypePricesN4'><a class='aPrintingTypePrices' href=\"#\" onClick=\"goURL('"+Price[i].PrintingTypeID+"','"+Price[i].Quantity+"','"+Price[i].PaperSizeID+"','"+Price[i].PaperTypeID+"');\">$"+Price[i].Price+"</a></td>");
										output.Write("<td align=\"center\" valign=middle class='tdPrintingTypePricesN4'>");
										OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
										_itemEvent(this,args);
										output.Write("</td>");
									}
										

								}
								
								
							} else {
								if (!isNetscape) {
									output.Write("<td align=\"center\" class='tdPrintingTypePrices'>");
									OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
									_itemEvent(this,args);
									output.Write("</td>");
								}
								else { 

									output.Write("<td align=\"center\">");
									OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
									_itemEvent(this,args);
									output.Write("</td>");
									
								}
							}
							i++;
						} else {
							//						html += "<td colspan='"+(PaperSizes.Length-sizeIndex).ToString()+"'>&nbsp;</td>";
							if (!isNetscape) {
								output.Write("<td class='tdPrintingTypePrices'  align=center>");
								OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
								_itemEvent(this,args);
								output.Write("</td>");
							}
							else {
								output.Write("<td align=center>");
								OrderingItemRenderEventArgs args = new OrderingItemRenderEventArgs(new OrderingItemInfo(Price[i].PrintingTypeID,Price[i].PaperTypeID,Price[i].PaperSizeID,Price[i].Quantity),output);
								_itemEvent(this,args);
								output.Write("</td>");
							}
						}
						sizeIndex++;
					}
				}
				
				output.Write("</tr>");
			}
			//----------------------				
			output.Write("</table>");
			
			
			
		}

	}
}
