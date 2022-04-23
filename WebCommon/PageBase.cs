namespace HiRes.Web.Common {
	using System;
	using System.Drawing;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.ComponentModel;
	
	using HiRes.Common;
	using HiRes.SystemFramework.Logging;
	using HiRes.Web.Common.Controls;
	/// <summary>
	/// Summary description for PageBase.
	/// </summary>
	public class PageBase : System.Web.UI.Page {
		
		protected const string MSG_INFOSAVED = "Information was successfully updated.";
		protected const string MSG_ERROR = "Error while updating info.";

		private String pageSecureUrlBase;
		private String pageUrlBase;
		private Uri pageUriBase;
		private String urlSuffix;
		private String _redirectUrl;
		private String _backUrl;
		/// <summary>
		///     Constructor for PageBase. 
		///     <remarks>Initialize private members.</remarks>
		/// </summary>
		public PageBase () {
			try {
				//check if non-default port is used 
				if (Context.Request.Url.Port==80) {
					urlSuffix =  Context.Request.Url.Host + Context.Request.ApplicationPath;
				} else {
					urlSuffix =  Context.Request.Url.Host +":"+ Context.Request.Url.Port +Context.Request.ApplicationPath;
				}
				
				pageUrlBase = @"http://" + urlSuffix;
			}
			catch {
				// for design time
			}            
		}
        
		/// <value>
		///     Property SecureUrlBase is used to get the prefix for URLs in the Secure directory.
		/// </value>
		public /*static*/virtual String SecureUrlBase {
			get {
				if (pageSecureUrlBase == null) {
					pageSecureUrlBase = @"https://" + urlSuffix;
				}
				return pageSecureUrlBase; 
			}
		}

		/// <value>
		///     Property UrlBase is used to get the prefix for URLs.
		/// </value>
		public /*static*/ String UrlBase {
			get {
				return pageUrlBase; 
			}
		}
		
		public Uri UriBase {
			get {
				try {
					if (pageUriBase==null) {
						pageUriBase = new Uri(pageUrlBase);
					}
					return pageUriBase;
				} catch {
					// design-time
					return null;
				}
			}
		}

		
		public string GetAbsoluteUrl(string relativeUrl) {
			return GetAbsoluteUrl(relativeUrl,null);
		}

		public string GetAbsoluteUrl(string relativeUrl, string queryString) {
			if (relativeUrl==null) { return pageUrlBase; }
			/*int len = relativeUrl.Length;
			if (queryString!=null) { len+=queryString.Length+1; }
			StringBuilder sb = new StringBuilder(*/
			UriBuilder ub = new UriBuilder(pageUrlBase);
			ub.Path += "/"+relativeUrl;
			ub.Query = queryString;
			return ub.Uri.ToString();
		}
		/// <summary>
		/// Url to redirect user after page action are succesfully performed
		/// </summary>
		public String RedirectUrl {
			get {
				return _redirectUrl;
			}
			set {
				_redirectUrl = value;
			}
		}
		/// <summary>
		/// Url to use by 'back' button if there's such a button on the page.
		/// </summary>
		public String BackUrl {
			get {
				return _backUrl;
			}
			set {
				_backUrl = value;
			}
		}

		#region Employee
		public EmployeeInfo Employee {
			get {
				try {
					return (EmployeeInfo)Session[ControlBase.KEY_EMPLOYEE];
				}
				catch {
					return null;  // for design time
				}
			}
			set { Session[ControlBase.KEY_EMPLOYEE] = value; }
		}

		#endregion

		
		/// <summary>
		///     Handles errors that may be encountered when displaying this page.
		///     <param name="e">An EventArgs that contains the event data.</param>
		/// </summary>
		protected override void OnError(EventArgs e) {
			AppLog.LogError("Unhandled Exception:",Server.GetLastError());
			base.OnError(e);
		}

		public string ResultMessage {
			get {
				try {
					string res = (string)Session[ControlBase.SESSION_RESMSGID];
					Session[ControlBase.SESSION_RESMSGID] = "";
					return res;
				} catch {
					return string.Empty; //design-time: remove from the final version
				}
			}
			set { Session[ControlBase.SESSION_RESMSGID] = value ; }
		}

		public bool IsResultMessageWarning {
			get {
				try {
					object res = Session[ControlBase.SESSION_ISRESMSGWARN];
					//bool boolRes = false;
					if (res==null) {
						Session[ControlBase.SESSION_ISRESMSGWARN] = false;
						return false;
					} else {
						return (bool)res;
					}
				} catch {
					return false; //design-time: remove from the final version
				}
			}
			set { Session[ControlBase.SESSION_RESMSGID] = value ; }
		}

		protected virtual Label MessageLabel {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		protected void SetMessage(Label lblMessage, string msg, bool isErrorNotification) {
			lblMessage.Text = msg;
			lblMessage.ForeColor = (isErrorNotification?lblMessage.ForeColor = Color.Red:Color.Black);
		}

		protected void SetMessage(string msg, bool isErrorNotification) {
			Label lblMessage = MessageLabel;
			lblMessage.Text = msg;
			lblMessage.ForeColor = (isErrorNotification?lblMessage.ForeColor = Color.Red:Color.Black);
		}
	}
}
