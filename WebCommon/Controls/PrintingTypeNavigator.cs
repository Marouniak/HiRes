/*
 * FILE:		PrintingTypeNavigator.cs
 * PROJECT:		HiRes PrintingsSite
 * 
 * ABSTRACT:	Abstract class that used to be a base class for any controls that serves 
 * for any kind of navigation through printing types.
 * 
 * LEGAL:		Copyright (c) Eurosoft International Inc., 2001
 * 
 * Revision history:
 * 
 * 25-Apr-2002 Gleb Novodran
 * Initial implementation
 * 
 */

namespace HiRes.Web.Common.Controls {
	using System;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using HiRes.BusinessFacade;
	using HiRes.Common;

	/// <summary>
	/// Summary description for PrintingTypeNavigator.
	/// </summary>
	public abstract class PrintingTypeNavigatorCtl : ControlBase {
		
		private string _baseRedirectUrl;
		private string _additionalUrlParams;

		public PrintingTypeNavigatorCtl() {

		}

		public virtual String RenderNavigator() {
			
			StringBuilder sb = new StringBuilder();
			PrintingTypeElement[] prntgs = this.Printings;
			
			foreach(PrintingTypeElement p in prntgs) {
				sb.Append("<img src=\""+UrlBase+"/images/_menu1.gif\" border=\"0\"  name=\"pic"+p.PrintingTypeID+"\" width=\"10\" height=\"7\">&nbsp;<a href='"+UrlBase+"/"+baseRedirectUrl+"?PrTypeId="+p.PrintingTypeID+"' onMouseOut=\"MM_swapImgRestore()\" onMouseOver=\"MM_swapImage('pic"+p.PrintingTypeID+"','','"+UrlBase+"/images/_menu2.gif',1)\">"+p.PrintingTypeName+"</a><br>");
			}

			return sb.ToString();
		}

		public string baseRedirectUrl {

			get { return _baseRedirectUrl; }
			set { _baseRedirectUrl = value; }
		}
		
		public string AdditionalUrlParams {

			get { return _additionalUrlParams; }
			set { _additionalUrlParams = value; }
		}

		public PrintingTypeElement[] Printings {
			get {
				if (Cache["PrintingNames"]==null) {
					PrintingTypeElement[] pnames = (new PrintingsFacade()).GetPrintingTypesNames();
					Cache["PrintingNames"] = pnames;
					return pnames;
				} else {
					try {
						return (PrintingTypeElement[]) Cache["PrintingNames"];
					} catch {
						return null;// FIXME: Consider exception handling
					}
				}
			}
		}

	}
}
