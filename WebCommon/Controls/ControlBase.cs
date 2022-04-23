using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using HiRes.Common;

namespace HiRes.Web.Common.Controls {
	
	/// <summary>
	/// All controls that perform operations on shopping cart 
	/// should derive from this class.
	/// </summary>
	public /*abstract*/ class ControlBase : UserControl {
		/*public enum ControlMode {
			WIZARD = 0,
			NONWIZARD
		}*/
		public const String KEY_CACHEDCART = "ShoppingCart";
		public const String KEY_CURRENTORDER = "CurrentOrder";
		public const String KEY_CUSTOMER = "CustomerInfo";

		public const String KEY_EMPLOYEE = "EmployeeInfo";

		private String pageSecureUrlBase;
		private String pageUrlBase;
		private Uri pageUriBase;
		private String urlSuffix;
		private String _backUrl;
		private string _continueUrl;

		/// <summary>
		///     Constructor for PageBase. 
		///     <remarks>Initialize private members.</remarks>
		/// </summary>
		public ControlBase () {
			try {
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

		/*public ControlMode Mode {
			get { 
				try {
					return (ControlMode)ViewState["ControlMode"];
				} catch {
					return ControlMode.NONWIZARD;//design-time hack to allow to view the derrived control in editor in "design" mode
				}
			}
			set {
				ViewState["ControlMode"] = value;
			}
		}    */    
        
		/// <value>
		///     Property SecureUrlBase is used to get the prefix for URLs in the Secure directory.
		/// </value>
		public String SecureUrlBase {
			get {
				if (pageSecureUrlBase == null ) {
					pageSecureUrlBase = @"https://" + urlSuffix;
				}
				return pageSecureUrlBase; 
			}
		}

		/// <value>
		///     Property UrlBase is used to get the prefix for URLs.
		/// </value>
		public String UrlBase {
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
		/// Url to use by 'back' button if there's such a button in control.
		/// </summary>
		public String BackUrl {
			get {
				return _backUrl;
			}
			set {
				_backUrl = value;
			}
		}

		public String ContinueUrl {
			get { return _continueUrl; }
			set { _continueUrl = value; }
		}

		protected string OperationResultMsg {
			get { 
				string res = (string)Session["OpResultMsg"];
				Session["OpResultMsg"] = null;
				return res;
			}
			set { Session["OpResultMsg"] = value; }
		}
		
		#region Employee
		public EmployeeInfo Employee {
			get {
				try {
					return (EmployeeInfo)Session[KEY_EMPLOYEE];
				}
				catch {
					return null;  // for design time
				}
			}
			set { Session[KEY_EMPLOYEE] = value; }
		}

		#endregion

		protected virtual String UploadVirtualPath {
			get {
				return AppConfig.UploadDirectory;
				//return String.Empty;
			}
		}

		protected virtual void EnsureUploadVirtualPath() {
			string abspath = Server.MapPath(UploadVirtualPath);
			if (System.IO.Directory.Exists(abspath )) { 
				return;
			} else {
				System.IO.Directory.CreateDirectory(abspath);
			}
		}

		protected virtual void Continue() {
			//TODO: implement it
			return;
		}

		protected virtual void Back() {
			//TODO: implement it
			return;
		}
		
		public const string SESSION_RESMSGID = "SessionResultMsg";
		public const string SESSION_ISRESMSGWARN = "SESSION_ISRESMSGWARN";

		public string ResultMessage {
			get {
				try {
					string res = (string)Session[SESSION_RESMSGID];
					Session[SESSION_RESMSGID] = "";
					return res;
				} catch {
					return string.Empty; //design-time: remove from the final version
				}
			}
			set { Session[SESSION_RESMSGID] = value ; }
		}

		public bool IsResultMessageWarning {
			get {
				try {
					object res = Session[SESSION_ISRESMSGWARN];
					//bool boolRes = false;
					if (res==null) {
						Session[SESSION_ISRESMSGWARN] = false;
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
