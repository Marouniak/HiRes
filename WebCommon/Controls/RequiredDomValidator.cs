namespace HiRes.Web.Common.Controls {

	using System.ComponentModel;
	using System.Diagnostics;
	using System;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	[
	ToolboxData("<{0}:RequiredDomValidator runat=server ErrorMessage=\"RequiredDomValidator\"></{0}:RequiredDomValidator>")
	]
	public class RequiredDomValidator : BaseDomValidator {

		[
		Bindable(true),
		Category("Behavior"),
		DefaultValue(""),
		Description("Initial Value")
		]                                         
		public string InitialValue {
			get { 
				object o = ViewState["InitialValue"];
				return((o == null) ? String.Empty : (string)o);
			}
			set {
				ViewState["InitialValue"] = value;
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			if (RenderUplevel) {
				writer.AddAttribute("evaluationfunction", "RequiredFieldValidatorEvaluateIsValid");
				writer.AddAttribute("initialvalue", InitialValue);
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
			return(!controlValue.Trim().Equals(InitialValue.Trim()));
		}                
	}
}
