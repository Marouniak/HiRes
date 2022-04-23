using System;
using System.Collections;
using System.Text;
using System.Web.UI;

using HiRes.BusinessFacade;
using HiRes.Common;
using HiRes.Web.Common.Components;

namespace HiRes.Web.Common.Controls {
	/// <summary>
	/// Summary description for PaperTypeNavigatorCtl.
	/// </summary>
	public class PaperTypeNavigatorCtl : BaseOrderingItemCtl {
		
		private String _baseRedirectUrl;
		private int _selectedPrintingTypeID;
		private int _selectedPaperTypeID;

		#region properties

		public int SelectedPrintingTypeID {
			get { return _selectedPrintingTypeID; }
			set { _selectedPrintingTypeID = value; }
		}

		public int SelectedPaperTypeID {
			get { return _selectedPaperTypeID; }
			set { _selectedPaperTypeID = value; }
		}

		public String BaseRedirectUrl {
			get { return _baseRedirectUrl; }
			set { _baseRedirectUrl = value; }
		}

		#endregion

		protected override void Render(HtmlTextWriter output) {
				
			StringBuilder sb = new StringBuilder();
			this.PrintingTypeId = SelectedPrintingTypeID;
			
			int count = 1; // count units on the line
			
			sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
			sb.Append("<tr><td><table cellpadding=0 cellspacing=0 border=0><tr>");

			ICollection allPaperTypes = CacheManager.AllPaperTypes(_printingTypeId).Values;
			int len = allPaperTypes.Count;
			//int len = this.PaperTypes.Length;
			foreach(PaperTypeInfo item in allPaperTypes) {
			//foreach(PaperTypeInfo item in this.PaperTypes) {
				if (item.PaperTypeID == SelectedPaperTypeID)  sb.Append("<td class='aSubTab'>");
				else sb.Append("<td class='SubTab' height=100%>");

					if (item.PaperTypeID == SelectedPaperTypeID) sb.Append("<small><small><b><i>"+item.PaperTypeName.ToUpper()+"</i></b></small></small>");
					else sb.Append("<small><small><b><a class='aPaperTypeAlt' href='"+/*NavigationManager.URL_Pricing*/BaseRedirectUrl+"?PrTypeId="+PrintingTypeId+"&PaperTypeId="+item.PaperTypeID+"'><i>"+item.PaperTypeName.ToUpper()+"</i></a></b></small></small>");

				sb.Append("</td>");
				sb.Append("<td class=PaperSizetdEmpty>&nbsp;</td>");
				count++;
			}				
			sb.Append("</tr></table></td><td valign='bottom'>&nbsp;</td></tr></table>");
			output.Write(sb.ToString());
			this.RenderChildren(output);
			
		}

		private void Page_Load(object sender, System.EventArgs e) {
		}
		#region component initialization
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
