
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

	[
	ToolboxData("<{0}:CustomDomValidator runat=server ErrorMessage=\"CustomDomValidator\"></{0}:CustomDomValidator>")
	]
	public class CustomDomValidator : BaseDomValidator {
		[
		Bindable(true),
		Category("Behavior"),
		DefaultValue(""),
		Description("clientvalidationfunction")
		] 

		public string clientvalidationfunction {
			get { 
				object o = ViewState["clientvalidationfunction"];
				return((o == null) ? String.Empty : (string)o);
			}
			set {
				ViewState["clientvalidationfunction"] = value;
			}
		}
		
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			if (RenderUplevel && this.EnableClientScript) {
				writer.AddAttribute("evaluationfunction", "CustomValidatorEvaluateIsValid");
				writer.AddAttribute("clientvalidationfunction", clientvalidationfunction);
			} else {
				writer.AddAttribute("evaluationfunction", "CustomValidatorEvaluateIsValid");
				writer.AddAttribute("clientvalidationfunction", "return true");
			}
		}    

		protected override bool EvaluateIsValid() {

			// Get the control value; return true if it is not found.
			string controlValue = GetControlValidationValue(ControlToValidate);
			if (controlValue == null) {
				Debug.Fail("Should have been caught by PropertiesValid check.");
				return true;
			}

			// See if the control has changed.
			return(!controlValue.Trim().Equals(clientvalidationfunction.Trim()));
		}  
	}
}
