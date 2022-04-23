namespace HiRes.Web.Common.Controls {

	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Diagnostics;
	using System.Text.RegularExpressions;
	using System.Drawing.Design;
	using System;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	[
	ToolboxData("<{0}:RegexDomValidator runat=server ErrorMessage=\"RegexDomValidator\"></{0}:RegexDomValidator>")
	]
	public class RegexDomValidator : BaseDomValidator {

		[
		Bindable(true),
		Category("Behavior"),
		DefaultValue(""),
		Editor("System.Web.UI.Design.WebControls.RegexTypeEditor,System.Design", typeof(UITypeEditor)),
		Description("ValidationExpression")
		]                                         

		private bool isRequired;
		public bool EnableClientScript = true;

		public bool IsRequired 
		{
			get { return isRequired; }
			set { isRequired = value;}
		}

		public string ValidationExpression {
			get { 
				object o = ViewState["ValidationExpression"];
				return((o == null) ? String.Empty : (string)o);
			}
			set {
				try {
						Regex.IsMatch("", value);
				}
				catch (Exception e) {
					//Throw new HttpException.
					//                       HttpRuntime.FormatResourceString(SR.Validator_bad_regex, value), e);
					throw new HttpException("Bad Expression", e);
				}
				ViewState["ValidationExpression"] = value;
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			if (RenderUplevel) {
				writer.AddAttribute("evaluationfunction", "RegularExpressionValidatorEvaluateIsValid");
				if (ValidationExpression.Length > 0 && this.EnableClientScript) {
					writer.AddAttribute("validationexpression", ValidationExpression);
				} else {
					writer.AddAttribute("validationexpression", "*");
				}
			}
		}            

		protected override bool EvaluateIsValid() {

			// Validation always succeeds if input is empty or value was not found.
			string controlValue = GetControlValidationValue(ControlToValidate);
			Debug.Assert(controlValue != null, "Should have already been checked");
			if (controlValue == null || controlValue.Length == 0) {
				if (!IsRequired){
					return true;
				} 
				else {
					return false;
				}
			}

			try {
				// Looking for an exact match, not just a search hit.
				Match m = Regex.Match(controlValue, ValidationExpression);
				return(m.Success && m.Index == 0 && m.Length == controlValue.Length);
			}
			catch {
				Debug.Fail("Regex error should have been caught in property setter.");
				return true;
			}
		}

	}
}
