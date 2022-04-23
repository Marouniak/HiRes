

namespace HiRes.Web.Common.Controls
{

	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Globalization;
	using System;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Text.RegularExpressions;
	using System.Text;	
	/// <summary>
	/// Summary description for DateDomValidator.
	/// </summary>
	public class DateDomValidator : CustomDomValidator
	{
		
		private bool _isFullDateFormat;
		private static string _dateTimeFormatInfo;
		private bool isRequired;
		private bool isPosteriorOnly;
		public bool EnableClientScript = true;

		public bool isFullDateFormat {
			set { _isFullDateFormat = value; }
			get { return _isFullDateFormat; }
		}

		public string DateTimeFormatInfo	{
			set { _dateTimeFormatInfo = value; }
			get { return _dateTimeFormatInfo; }
		}
		
		public bool IsRequired {
			get { return isRequired; }
			set { isRequired = value;}
		}

		public bool IsPosteriorOnly {
			get { return isPosteriorOnly; }
			set { isPosteriorOnly = value;}
		}

		private void outputScript() {
			
			if (DateTimeFormatInfo == null ) {
				DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToString();
			}

			string _afterDate = "";
			if (IsPosteriorOnly) {
				_afterDate = DateTime.Now.ToShortDateString();
			}

			string dateClientValidator= @"
			<script language=""javascript"">
				function checkDate(Obj,stringObj){
					str = stringObj.Value;
					str1 = stringObj.Value;
					format = '"+DateTimeFormatInfo+@"';
					_afterDate ='"+_afterDate+@"';
					separator = format.charAt(format.search(/[^a-z]/i));
					var sep = '?./+*';
					isUnspecSym = false;
					for (i=0;i<sep.length;i++) {
						tmp= '\\'+sep.charAt(i);
						if (separator == sep.charAt(i)) {
							if (eval(""str.search(/""+tmp+""/gi,'-')"") != -1) {
								str= eval(""str.replace(/""+tmp+""/gi,'-')"");
							} else {
								stringObj.IsValid = false;
								return stringObj.IsValid;
							}
							separator = '-';
						}
					}
					
					if ((sepStr = eval(""str.match(/""+separator+""/g)""))!= -1 && sepStr) {
						if (sepStr.length<2){
							 stringObj.IsValid = false;
							 return stringObj.IsValid;
						}
					} else {
							stringObj.IsValid = false;
							return false;
					}
					str		=	str.replace(/\D/gi,'-');
					format	=	format.replace(/[^a-z]/gi,'-');
					format	=	format.replace(/mm/gi,'([0][1-9]|[1-9]|1[0-2])');
					format	=	format.replace(/m/gi,'([0][1-9]|[1-9]|1[0-2])');
					format	=	format.replace(/y{1,4}/gi,'([0-9]{2}|[0-9]{4})');
					format	=	format.replace(/dd/gi,'([0][1-9]|[1-9]|[12][0-9]|3[01])');
					format	=	format.replace(/d/gi,'([0][1-9]|[1-9]|[12][0-9]|3[01])');
					if (eval(""str.search(/^""+format+""$/)"") == -1) {
						stringObj.IsValid = false;
					} else {
						stringObj.IsValid = true;
					}";
			if (IsPosteriorOnly) {
					dateClientValidator +=@"	
						if (document.getElementById) {
							if (stringObj.IsValid == true) {
								try {
									var afterDate = new Date(_afterDate);
									var nowDate = new Date(str1);
									if (afterDate>nowDate) stringObj.IsValid = false ;
								} catch (er) {
									stringObj.IsValid = true;
								}
							}
						}
					";
			}
			dateClientValidator +=@"	
					return stringObj.IsValid;
				}
			</script>
			";
			this.Page.RegisterClientScriptBlock("dateClientValidator",dateClientValidator);
		}


		
		protected override bool EvaluateIsValid() 
		{

			string val = GetControlValidationValue(ControlToValidate);
			if ((val == null)&&(this.isRequired)) 
			{
				return false;
			}
			try 
			{
				DateTime.Parse(val);
				return true;
			} 
			catch 
			{
				return false;
			}
		}

		private void Page_Load(object sender, System.EventArgs e) {
			
		}

		#region component initialization
		override protected void OnInit(EventArgs e) 
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//

			InitializeComponent();
			base.OnInit(e);
			if (this.EnableClientScript) {
				outputScript();
				this.clientvalidationfunction = "checkDate";
			}
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

	}
}
