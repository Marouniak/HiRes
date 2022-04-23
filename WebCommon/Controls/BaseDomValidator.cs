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
	DefaultProperty("ErrorMessage"),
	]
	public abstract class BaseDomValidator : Label, IValidator {

		// Note: these script-enabled controls have two sets
		// of client-side script. 
		// There is a fixed set in a script library called 
		// DomValidation.js. This goes  at the top of the page
		// and is put on the page using RegisterClientScriptBlock.
		// The second is the dynamic block 
		// below that contains some inline script that 
		// should be executed at the end of the page load.
		// This is declared using RegisterStartupScript.

		private const string ValidatorFileName = "DomValidation.js";
		private const string ValidatorIncludeScriptKey = "DomValidatorIncludeScript";
		private const string ValidatorStartupScript = @"
<script language=""javascript"">
<!--

var Page_ValidationActive = false;
if (typeof(Page_DomValidationVer) == ""undefined"")
    alert(""{0}"");
else
    ValidatorOnLoad();

function ValidatorOnSubmit() {{
    if (Page_ValidationActive) {{
        return ValidatorCommonOnSubmit();
    }}
}}

// -->
</script>
        ";
		private const string IncludeScriptFormat = @"
<script language=""{0}"" src=""{1}{2}""></script>";

		private bool preRenderCalled;
		private bool isValid;
		private bool propertiesChecked;
		private bool propertiesValid;
		private bool renderUplevel;

		protected BaseDomValidator() {
			isValid = true;
			propertiesChecked = false;
			propertiesValid = true;
			renderUplevel = false;

			// Default forecolor for validators is Red.
			ForeColor = Color.Red;
		}

		[
		DefaultValue(typeof(Color), "Red")
		]
		public override Color ForeColor {
			get {
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;
			}
		}        

		[
		Category("Behavior"),
		DefaultValue(""),
		Description("The control to validate."),
		TypeConverter(typeof(ValidatedControlConverter))
		]                                         
		public string ControlToValidate {
			get { 
				object o = ViewState["ControlToValidate"];
				return((o == null) ? String.Empty : (string)o);
			}
			set {
				ViewState["ControlToValidate"] = value;
			}
		}

		[
		Bindable(true),
		Category("Appearance"),
		DefaultValue(""),
		Description("Error Message")
		]
		public string ErrorMessage {
			get {
				object o = ViewState["ErrorMessage"];
				return((o == null) ? String.Empty : (string)o);
			}
			set {
				ViewState["ErrorMessage"] = value;
			}
		}

		[
		Category("Behavior"),
		DefaultValue(true),
		Description("Enable Client Script")
		]
		public bool EnableClientScript {
			get {
				object o = ViewState["EnableClientScript"];
				return((o == null) ? true : (bool)o);
			}
			set {
				ViewState["EnableClientScript"] = value;
			}
		}

		public override bool Enabled {
			get {
				return base.Enabled;
			}
			set {
				base.Enabled= value;
				// When a validator is disabled, 
				// generally, the intent is not to
				// make the page invalid for that round trip.
				if (!value) {
					isValid = true;
				}
			}
		}        

		[
		Browsable(false),
		Category("Behavior"),
		DefaultValue(true),
		Description("Is Valid"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsValid {
			get {
				return isValid;
			}
			set {
				isValid = value;
			}
		}

		protected bool PropertiesValid {
			get {
				if (!propertiesChecked) {
					propertiesValid = ControlPropertiesValid();
					propertiesChecked = true;
				}
				return propertiesValid;
			}
		}

		protected bool RenderUplevel {
			get {
				return renderUplevel;
			}
		}

		[
		Bindable(true),
		Category("Appearance"),
		DefaultValue(ValidatorDisplay.Static),
		Description("Display"),
		]
		public ValidatorDisplay Display {
			get {
				object o = ViewState["Display"];
				return((o == null) ? ValidatorDisplay.Static : (ValidatorDisplay)o);
			}
			set {
				if (value < ValidatorDisplay.None || value > ValidatorDisplay.Dynamic) {
					throw new ArgumentException();
				}
				ViewState["Display"] = value;
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			// Validators do not render the disabled attribute; 
			// instead, they are invisible when they are disabled.
			bool disabled = !Enabled;
			if (disabled) {
				Enabled = true;
			}
			base.AddAttributesToRender(writer);

			if (RenderUplevel) {
				// A validator must have an id on the client, so if  it is null, write it here.
				// Otherwise, base.RenderAttributes takes care of it.
				if (ID == null) {
					writer.AddAttribute("id", ClientID);
				}

				if (ControlToValidate.Length > 0) {
					writer.AddAttribute("controltovalidate", GetControlRenderID(ControlToValidate));
				}
				if (ErrorMessage.Length > 0) {
					writer.AddAttribute("errormessage", ErrorMessage, true);
				}
				ValidatorDisplay display = Display;
				if (display != ValidatorDisplay.Static) {
					writer.AddAttribute("display", PropertyConverter.EnumToString(typeof(ValidatorDisplay), display));
				}
				if (!IsValid) {
					writer.AddAttribute("isvalid", "False");
				}
				if (disabled) {
					writer.AddAttribute("enabled", "False");
				}
			}
			if (disabled) {
				Enabled = false;
			}
		}

		protected void CheckControlValidationProperty(string name, string propertyName) {
			// Get the control using the relative name.
			Control c = NamingContainer.FindControl(name);            
			if (c == null) {
				throw new HttpException("Control not found.");
			}

			// Get  the control's validation property.
			PropertyDescriptor prop = GetValidationProperty(c);
			if (prop == null) {
				throw new HttpException("Control cannot be validated.");                          
			}

		}

		protected virtual bool ControlPropertiesValid() {
			// Determine whether the control to validate is blank.
			string controlToValidate = ControlToValidate;
			if (controlToValidate.Length == 0) {
				throw new HttpException("ControlToValidate cannot be blank.");
			}

			// Check that the property points to a valid control (if not, an exception is thrown). 
           
			CheckControlValidationProperty(controlToValidate, "ControlToValidate");

			return true;
		}                     

		protected abstract bool EvaluateIsValid();    

		protected string GetControlRenderID(string name) {

			// Get the control using the relative name.
			Control c = FindControl(name);            
			if (c == null) {
				Debug.Fail("We should have already checked for the presence of this");
				return "";
			}
			return c.ClientID;
		}


		protected string GetControlValidationValue(string name) {

			// Get the control using the relative name.
			Control c = NamingContainer.FindControl(name);            
			if (c == null) {
				return null;
			}

			// Get  the control's validation property.
			PropertyDescriptor prop = GetValidationProperty(c);
			if (prop == null) {
				return null;
			}

			// Get its value as a string.
			object value = prop.GetValue(c);
			if (value is ListItem) {
				return((ListItem) value).Value;
			}
			else if (value != null) {
				return value.ToString();
			}
			else {
				return string.Empty;
			}
		}                

		public static PropertyDescriptor GetValidationProperty(object component) {
			ValidationPropertyAttribute valProp = (ValidationPropertyAttribute)TypeDescriptor.GetAttributes(component)[typeof(ValidationPropertyAttribute)];
			if (valProp != null && valProp.Name != null) {
				return TypeDescriptor.GetProperties(component, null)[valProp.Name];
			}
			return null;
		}

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			Page.Validators.Add(this);
		}        

		protected override void OnUnload(EventArgs e) {
			if (Page != null) {
				Page.Validators.Remove(this);
			}
			base.OnUnload(e);
		}        

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			preRenderCalled = true;

			// Force a re-query of properties for render.
			propertiesChecked = false;                       

			// Work out uplevelness now.
			renderUplevel = DetermineRenderUplevel();

			if (renderUplevel) {
				RegisterValidatorCommonScript();
			}
		}

		protected virtual bool DetermineRenderUplevel() {

			// Must be on a page.
			
			Page page = Page;
			if (page == null || page.Request == null) {
				return false;
			}

			// Check the browser capabilities. 
			// This is how you can get automatic fallback to server-side 
			// behavior. These validation controls need 
			// the W3C DOM level 1 for control manipulation
			// and need at least ECMAScript 1.2 for the 
			// regular expressions.
			
				return true;
			/*
			return (EnableClientScript 
				&& page.Request.Browser.W3CDomVersion.Major >= 1
				&& page.Request.Browser.EcmaScriptVersion.CompareTo(new Version(1, 2)) >= 0);
			*/	
		}

		protected void RegisterValidatorCommonScript() {
            
			string location = null;

			if (!Page.IsClientScriptBlockRegistered(ValidatorIncludeScriptKey)) {
				// Provide the location of the script file.
				// When using a script library, deployment can be 
				// a problem  because the runtime is
				// tied to a specific version of the script file. 
				// This sample takes the easy way out and insists that
				// the file be placed in the /script subdirectory 
				// of the application.
				// In other cases, you should place it  where it
				// can be shared by multiple applications and is placed 
				// in a separate directory so that different versions 
				// of a control library can run  side by side.
				// The recommended pattern is to put script files  in the 
				// path /aspnet_client/<assembly name>/<assembly version>/".
				location = Page.Request.ApplicationPath + "/script/";
				//location = "http://localhost/testproject/script/";

				// Create the client script block.
				string includeScript = String.Format(IncludeScriptFormat, "javascript", location, ValidatorFileName);
				Page.RegisterClientScriptBlock(ValidatorIncludeScriptKey, includeScript);
			}

			if (!Page.IsStartupScriptRegistered(ValidatorIncludeScriptKey)) {

				if (location == null) location = Page.Request.ApplicationPath + "/script/";

				// Provide an error message, which is localized.
				string missingScriptMessage = "Validation script is missing '" + location + ValidatorFileName + "'";

				// Create the startup script block.
				string startupScript = String.Format(ValidatorStartupScript, new object [] {missingScriptMessage, });
				Page.RegisterStartupScript(ValidatorIncludeScriptKey, startupScript);
			}

			Page.RegisterOnSubmitStatement("ValidatorOnSubmit", "return ValidatorOnSubmit();");
		}

		protected virtual void RegisterValidatorDeclaration() {
			string element = "document.getElementById(\"" + ClientID + "\")";
			Page.RegisterArrayDeclaration("Page_Validators", element);
		}

		protected override void Render(HtmlTextWriter writer) {
			bool shouldBeVisible;

			if (preRenderCalled == false) {
				// This is for design time. 
				// In this case you  do not 
				// want any expandos or property checks
				// and always want the control to be visible in the designer.
				propertiesChecked = true;
				propertiesValid = true;
				renderUplevel = false;
				shouldBeVisible = true;
			}
			else {
				shouldBeVisible = Enabled && !IsValid;
			}

			// Do not render if there are errors.
			if (!PropertiesValid) {
				return;
			}


			//  Specify what to display.
			ValidatorDisplay display = Display;
			bool displayContents;
			bool displayTags;
			if (RenderUplevel) {
				displayTags = true;
				displayContents = (display != ValidatorDisplay.None);
			}
			else {
				displayContents = (display != ValidatorDisplay.None && shouldBeVisible);
				displayTags = displayContents;
			}

			if (displayTags && RenderUplevel) {

				// Add this validator to the array 
				// of validators  emitted in the client script. 
				RegisterValidatorDeclaration();

				// Set extra uplevel styles.
				if (display == ValidatorDisplay.None
					|| (!shouldBeVisible && display == ValidatorDisplay.Dynamic)) {
					Style["display"] = "none";
				}
				else if (!shouldBeVisible) {
					Debug.Assert(display == ValidatorDisplay.Static, "Unknown Display Type");
					Style["visibility"] = "hidden";
				}
			}

			// Display the contents.
			if (displayTags) {
				RenderBeginTag(writer);
			}
			if (displayContents) {
				if (Text.Trim().Length > 0) {
					RenderContents(writer);
				}
				else {
					writer.Write(ErrorMessage);
				}
			}
			else if (!RenderUplevel && display == ValidatorDisplay.Static) {
				// For downlevel browsers in static mode, render a space so 
				// that table cells do not render as empty.
				writer.Write("&nbsp;");
			}
			if (displayTags) {
				RenderEndTag(writer);
			}
		}


		public void Validate() {
			if (!Visible || !Enabled) {
				IsValid = true;
				return;
			}
			// Check  whether the container is invisible.
			Control parent = Parent;
			while (parent != null) {
				if (!parent.Visible) {
					IsValid = true;
					return;
				}
				parent = parent.Parent;
			}
			propertiesChecked = false;
			if (!PropertiesValid) {
				IsValid = true;
				return;
			}
			IsValid = EvaluateIsValid();
		}                                                  

	}

}
