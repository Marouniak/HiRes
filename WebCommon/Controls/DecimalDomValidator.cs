

namespace HiRes.Web.Common.Controls {
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
	/// Summary description for DecimalDomValidator.
	/// </summary>
	/// 
	
	public class DecimalDomValidator : CustomDomValidator {

		private string _decimalSeparator;
		private bool isRequired;
		public bool EnableClientScript = true;
	
		public string DecimalSeparator {
			get { return _decimalSeparator; }
			set { _decimalSeparator = value;}
		}

		public bool IsRequired
		{
			get { return isRequired; }
			set { isRequired = value;}
		}


		private void outputScript() {

			if (DecimalSeparator == null ) {
				DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

				/*string ds = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
				ds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
				ds = new NumberFormatInfo().NumberDecimalSeparator;*/
			}

			string decimalClientScript = @"
				<script languages=""javascript"">
					function decimalClientScript(Obj,stringObj){
						str = stringObj.Value;
						separator ='"+DecimalSeparator+@"';
						if(str.search(/^(-|\+|)\d+(\"+DecimalSeparator+@"\d+)?$/) != -1 ) {
							stringObj.IsValid = true;
						} else {
							if(str.search(/^(-|\+|)\d+(,\d+)?$/) != -1 && separator == ',') {
								stringObj.IsValid = true;
							} else {
								stringObj.IsValid = false;
							}
						}
						return stringObj.IsValid;
					}
				</script>
			";
			this.Page.RegisterClientScriptBlock("decimalClientScript",decimalClientScript);
		}
		

		protected override bool EvaluateIsValid() {

			string val = GetControlValidationValue(ControlToValidate);
			if ((val == null)&&(this.isRequired)) {
				return false;
			}
			try {
				decimal res =  decimal.Parse(val);
				return true;
			} catch {
				return false;
			}
		}
	
		private void Page_Load(object sender, System.EventArgs e) 
		{
			
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
				this.clientvalidationfunction = "decimalClientScript";
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
