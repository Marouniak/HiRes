/*
 * FILE:		PrintingTypesTabNavCtl.cs
 * PROJECT:		HiRes PrintingsSite. Common Controls
 * 
 * ABSTRACT:	Printing Types Tab-navigation control
 * 
 * LEGAL:		
 * 
 */
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

	/// <summary>
	/// </summary>
	public class PrintingTypesTabNavCtl : PrintingTypeNavigatorCtl {
		protected System.Web.UI.WebControls.Label lblPrnTypesMenu;
		private int _selectedPrintingTypeID;
		private int _tabsCount;

		#region events

		protected override void Render(HtmlTextWriter output) {
			output.Write(RenderNavigator());
		}
		#endregion

		#region Generation HTML

		public override String RenderNavigator() {
			String html; // html string containing lines in which does not go into an active unit
			String htmlActive = ""; // html string for active unit in the line
			PrintingTypeElement[] prntgs = this.Printings;
			int count = 1; // count units on the line
			int col = 1; // count all units
			int line = 1; // count all lines
			int activeLine = (int) Math.Ceiling(((double) (AdequacyIDSPTID(prntgs)) / TabsCount)); // get active line by ID element

			// Testing parameters
			// ---------------------
			// Response.Write("lines="+lines+" activeLine="+activeLine+">>");
			// ----------------------

			// Creating HTML
			html = "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\">";
			html += "<tr><td><table cellpadding=0 cellspacing=0 border=0 width=100%><tr>";
			foreach(PrintingTypeElement p in prntgs) {
				// If a beginning of string
				if (count == 1) { 
					if (SelectedPrintingTypeID == p.PrintingTypeID) {
						if (line != activeLine) {
							html +=	"<td width=8 height=26><img src=\""+this.UrlBase+"/"+this.UrlBase+"/images/d1.gif\" width=8 height=26></td>";
						} else {
							htmlActive += "<td width=8 height=26><img src=\""+this.UrlBase+"/images/d1.gif\" width=8 height=26></td>";
						}
					
					} else {
						if (line != activeLine) {
							html += "<td width=8 height=26><img src=\""+this.UrlBase+"/images/l1l.gif\" width=8 height=26></td>";
						} else { 
							htmlActive += "<td width=8 height=26><img src=\""+this.UrlBase+"/images/l1l.gif\" width=8 height=26></td>";
						}
					}
				}
					
				// CONTENT
				if (p.PrintingTypeID == SelectedPrintingTypeID) {
					if (line != activeLine) {
						html += "<td bgcolor=\"#999999\" align=\"center\" width=100%><table cellpadding=0 cellspacing=0 border=0 height=26 width=100%><tr><td valign=top height='6'><img src='"+this.UrlBase+"/images/yt_l_fon.gif' height='6' width='100%'></td></tr><tr><td valign=top width=100%><small><small><b><a href='"+baseRedirectUrl+"?PrTypeId="+p.PrintingTypeID+"' class='nPricesLink'>"+TrimReplace(p.PrintingTypeName.ToUpper(),false)+"</a></b></small></small></td></tr></table></td>";
					} else {
						htmlActive += "<td bgcolor=\"#999999\" align=\"center\" width=100%><table cellpadding=0 cellspacing=0 border=0 height=26 width=100%><tr><td valign=top height='6'><img src='"+this.UrlBase+"/images/yt_l_fon.gif' height='6' width='100%'></td></tr><tr><td valign=top width=100%><small><small><b><a href='"+baseRedirectUrl+"?PrTypeId="+p.PrintingTypeID+"' class='nPricesLink'>"+TrimReplace(p.PrintingTypeName.ToUpper(),false)+"</a></b></small></small></td></tr></table></td>";
					}
				} else {
					if (line != activeLine) {
						html += "<td  bgCOLOR=\"#CCCCCC\" align=\"center\" width=100%><table cellpadding=0 cellspacing=0 border=0 height=26 width=100%><tr><td valign=top height='6'><img src='"+this.UrlBase+"/images/yt_f_fon.gif' height='6' width='100%'></td></tr><tr><td valign=top width=100%><small><small><b><a href='"+baseRedirectUrl+"?PrTypeId="+p.PrintingTypeID+"' class='nPricesLink'>"+TrimReplace(p.PrintingTypeName.ToUpper(),true)+"</a></b></small></small></td></tr><tr><td valign='bottom' height='5'><img src='"+this.UrlBase+"/images/yb_fon.gif' width='100%' height='5'></td></tr></table></td>";
					} else {
						htmlActive += "<td  bgCOLOR=\"#CCCCCC\" align=\"center\" width=100%><table cellpadding=0 cellspacing=0 border=0 height=26 width=100%><tr><td valign=top height='6'><img src='"+this.UrlBase+"/images/yt_f_fon.gif' height='6' width='100%'></td></tr><tr><td valign=top width=100%><small><small><b><a href='"+baseRedirectUrl+"?PrTypeId="+p.PrintingTypeID+"' class='nPricesLink'>"+TrimReplace(p.PrintingTypeName.ToUpper(),true)+"</a></b></small></small></td></tr><tr><td valign='bottom' height='5'><img src='"+this.UrlBase+"/images/yb_fon.gif' width='100%' height='5'></td></tr></table></td>";
					}
				}
				
				// If the end of string
				if (p.PrintingTypeID != prntgs.Length+(prntgs[prntgs.Length-1].PrintingTypeID-prntgs.Length) && count != TabsCount) {
					if ((prntgs[col].PrintingTypeID) == SelectedPrintingTypeID) {
						if (line != activeLine) {
							html += "<td width=21 height=26><img src=\""+this.UrlBase+"/images/l_d.gif\" width=21 height=26></td>";
						} else {
							htmlActive += "<td width=21 height=26><img src=\""+this.UrlBase+"/images/l_d.gif\" width=21 height=26></td>";
						}
					} else {
						if (p.PrintingTypeID == SelectedPrintingTypeID) {
							if (line != activeLine) {
								html += "<td width=26><img src=\""+this.UrlBase+"/images/d_l.gif\" width=28></td>";
							} else {
								htmlActive += "<td width=26><img src=\""+this.UrlBase+"/images/d_l.gif\" width=28></td>";
							}
						} else {
							if (line != activeLine) {
								html += "<td width=26><img src=\""+this.UrlBase+"/images/l_l.gif\" width=28></td>";
							} else {
								htmlActive += "<td width=26><img src=\""+this.UrlBase+"/images/l_l.gif\" width=28></td>";
							}

						}
					}
				} else {
					if (p.PrintingTypeID == SelectedPrintingTypeID) {
						if (line != activeLine) {
							html += "<td><img src=\""+this.UrlBase+"/images/d_2.gif\"></td>";
						} else {
							htmlActive += "<td><img src=\""+this.UrlBase+"/images/d_2.gif\"></td>";
						}
					}
					else {
						if (line != activeLine) {
							html += "<td><img src=\""+this.UrlBase+"/images/l_1.gif\"></td>";
						} else {
							htmlActive += "<td><img src=\""+this.UrlBase+"/images/l_1.gif\"></td>";
						}
					}
					if (p.PrintingTypeID != prntgs.Length+(prntgs[prntgs.Length-1].PrintingTypeID-prntgs.Length)) {
						if (line != activeLine) {
							html += "</tr></table></td></tr><tr><td><table cellpadding=0 cellspacing=0 border=0 width='100%'><tr>";
						} 
					}
				}

				// Inspection of lines
				if (count == TabsCount) {
					count=0;
					line++; // next line
				}
				count++; // next element on the line
				col++; // next element
			}
			html += "</tr></table></td></tr>";
			html += "<tr valign='bottom'><td><table cellpadding=0 cellspacing=0 border=0 width='100%'><tr>"+htmlActive;
			html += "</tr></table></td></tr>";
			/*html += "<tr>"+
						"<td background='"+this.UrlBase+"/images/tab_grey.gif' valign='top'><img src=\""+this.UrlBase+"/images/tab_grey.gif\" width=\"100%\"></td>"+
					"</tr>";
					*/
			html +=	"</table>";
			return html;
		}
		#region functions
		// Replace in the string of a " " to "_"
		private String TrimReplace(String val,Boolean p) {
			int i=0;
			while (i!=-1) {
				i = val.IndexOf(" ",i);
				if (i!=-1) {
					if (p) {
						val=val.Substring(0,i)+"<font color='#CCCCCC'>_</font>"+val.Substring(i+1);
					} else {
						val=val.Substring(0,i)+"<font COLOR='#999999'>_</font>"+val.Substring(i+1);
					}
					i+=29; // goto before font 
				}
			}
			return val;
		}

		// Adequacy ID element with SelectedPrintingTypeID
		private int AdequacyIDSPTID(PrintingTypeElement[] prntgs) {
			int id = 1;
			foreach(PrintingTypeElement p in prntgs) {
				if (p.PrintingTypeID == SelectedPrintingTypeID) {
					break;
				}
				id++;
			}
			return id;
		}

		#endregion
		#endregion
	
		#region Public parameters

		public int SelectedPrintingTypeID {
			get { return _selectedPrintingTypeID; }
			set { _selectedPrintingTypeID = value; }
		}

		public int TabsCount {
			get { return _tabsCount; }
			set { _tabsCount = value; }
		}

		#endregion


	}
}
