/*
 * FILE:		OrdersDAL.cs
 * 
 * PROJECT:		HiRes PrintingsSite Database Access Layer
 * 
 * ABSTRACT:	"Order" entity related database operations
 * 
 * LEGAL:		
 * 
 * Revision history:
 * 
 * 28-Apr-2002 Gleb Novodran
 * Initial implementation
 * 02-May-2002 Vitaly Homko
 * order info getters
 * 
 * 01-Jun-2002 Gleb Novodran
 * Add new orders finctionality
 * Payment-related functionality moved here from PaymentsDAL.cs
 * 
 * 
 */

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Text;

using HiRes.Common;
using HiRes.SystemFramework.Logging;

namespace HiRes.DAL {

	[Flags]
	public enum OrderInfoParts {
		None = 0x0000,
		MainInfo = 0x0001,
		DeliveryDetails = 0x0002,
		Amounts = 0x0004,
		Payments = 0x0008,
		OrderStatus = 0x0010,
		DesignParts = 0x0020,
		DesignPreview = 0x0040,
		AuxFiles = 0x0080,
		SelectedExtras = 0x0100,
		PDInfo = 0x0200,
		DDInfo = 0x0400,
		MailingInfo = 0x0800
		/*,
		PromoCodesUsage = 0x0100*/
	}

	/// <summary>
	/// Summary description for OrdersDAL.
	/// </summary>
	public class OrdersDAL : System.ComponentModel.Component {


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Constants
		private const String OrderID = "@orderID";
		private const String SiteID = "@siteID";
		private const string AuxFileID = "@auxFileId";
		private const String FileCategory = "@FileCategory";
		private const String PartId = "@partId";
		private const String UPNo = "@UPNo";
		private const String PSNo = "@PSNo";
		private const string PM_PostageType = "@PostageType";
		private const string PM_MailDue = "@MailingDue";

		private const String Filter = "@filterClause";
		private const String Order = "@orderClause";

		private const string BillingFirstName = "@billingFirstName";
		private const string BillingLastName = "@billingLastName";
		private const string BillingContactEmail = "@billingContactEmail";
		private const string BillingAddress1 = "@billingAddress1";
		private const string BillingAddress2 = "@billingAddress2";
		private const string BillingCountry = "@billingCountry";
		private const string BillingState = "@billingState";
		private const string BillingZipcode = "@billingZipcode";
		private const string BillingCity = "@billingCity";
		private const string BillingContactPhone = "@billingContactPhone";
		private const string BillingContactFax = "@billingContactFax";
		private const string BillingCompanyName = "@billingCompanyName";
		private const string JobName = "@JobName";
		#endregion

		#region Private Members: SQL Commands
		private SqlCommand loadOrderCommand;
		private SqlCommand loadDeliveryDetailsCommand;
		private SqlCommand loadSelectedExtras;
		private SqlCommand loadOrdersCommand;
		private SqlCommand updateOrderStatus;
		private SqlCommand updateOrderDeliveryDetails;
		private SqlCommand updateOrderAmounts;

		private SqlCommand loadPartDesignsCmd;
		private SqlCommand addPartDesignCmd;
		private SqlCommand updatePartDesignFileCmd;
		private SqlCommand getPartDesignFileCmd; 
		private SqlCommand updatePartDesignCmd;
		private SqlCommand removePartDesignCmd;
		private SqlCommand addAuxFileCmd;
		private SqlCommand removeAuxFileCmd;
		private SqlCommand loadAuxFilesCmd;

		private SqlCommand addNewOrderCmd;

		private SqlCommand loadPaymentsCmd;
		private SqlCommand addPaymentCmd;
		private SqlCommand removePaymentCmd;

		private SqlCommand addExtrasCmd;
		
		private SqlCommand updateMailingInfoCmd;
		private SqlCommand updatePDInfoCmd;
		private SqlCommand updateDDInfoCmd;

		private SqlCommand updateSelectedExtraCmd;
		private SqlCommand delExtrasCmd;
		#endregion

		#region Constructors
		public OrdersDAL(System.ComponentModel.IContainer container) {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public OrdersDAL() {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		#endregion

		#region Destructors
		public new void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
			//GC.SuppressFinalize(true);
		}

		/// <summary>
		///		Free the instance variables of this object.
		/// </summary>
		protected new virtual void Dispose(bool disposing) {
			if (!disposing)
				return; // we're being collected, so let the GC take care of this object
			try {
				
				for (int i=0; i<components.Components.Count;i++) {
					components.Components[i].Dispose();
				}

				if (loadOrderCommand != null) 
					loadOrderCommand.Dispose();
				if (loadDeliveryDetailsCommand != null) 
					loadDeliveryDetailsCommand.Dispose();
				if (loadSelectedExtras != null) 
					loadSelectedExtras.Dispose();
				if (loadOrdersCommand != null)
					loadOrdersCommand.Dispose();
				if (updateOrderStatus !=null)
					updateOrderStatus.Dispose();
				if (updateOrderDeliveryDetails!=null)
					updateOrderDeliveryDetails.Dispose();
				if (updateOrderAmounts!=null)
					updateOrderAmounts.Dispose();
				if (loadPartDesignsCmd != null)
					loadPartDesignsCmd.Dispose();
				if (addPartDesignCmd != null)
					addPartDesignCmd.Dispose();
				if (updatePartDesignFileCmd != null)
					updatePartDesignFileCmd.Dispose();
				
				if (getPartDesignFileCmd != null)
					getPartDesignFileCmd.Dispose();

				if (updatePartDesignCmd != null)
					updatePartDesignCmd.Dispose();
				if (addAuxFileCmd != null)
					addAuxFileCmd.Dispose();
				if (removePartDesignCmd != null)
					removePartDesignCmd.Dispose();
				if (removeAuxFileCmd != null)
					removeAuxFileCmd.Dispose();
				if (loadAuxFilesCmd != null)
					loadAuxFilesCmd.Dispose();
				
				if (addNewOrderCmd != null)
					addNewOrderCmd.Dispose();

				if (loadPaymentsCmd != null) 
					loadPaymentsCmd.Dispose();
				if (addPaymentCmd != null) 
					addPaymentCmd.Dispose();
				if (removePaymentCmd!= null) 
					removePaymentCmd.Dispose();

				if (addExtrasCmd != null) 
					addExtrasCmd.Dispose();
				
				if (updateMailingInfoCmd != null)
					updateMailingInfoCmd.Dispose();
				if (updatePDInfoCmd != null)
					updatePDInfoCmd.Dispose();
				if (updatePDInfoCmd != null)
					updateDDInfoCmd.Dispose();

				if (updateSelectedExtraCmd != null)
					updateSelectedExtraCmd.Dispose();
				if (delExtrasCmd != null)
					delExtrasCmd.Dispose();
				
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQL Command Getters
		private SqlCommand GetLoadOrderCommand() {
			if ( loadOrderCommand == null ) {
				loadOrderCommand = new SqlCommand("HiResAdmin.getOrderInfo");
				loadOrderCommand.CommandType = CommandType.StoredProcedure;
				loadOrderCommand.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
			}
			return loadOrderCommand;
		}
		private SqlCommand GetLoadDeliveryDetailsCommand() {
			if ( loadDeliveryDetailsCommand == null ) {
				loadDeliveryDetailsCommand = new SqlCommand("HiResAdmin.getDeliveryDetailsInfo");
				loadDeliveryDetailsCommand.CommandType = CommandType.StoredProcedure;
				loadDeliveryDetailsCommand.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
			}
			return loadDeliveryDetailsCommand;
		}

		private SqlCommand GetLoadSelectedExtrasCommand() {
			if ( loadSelectedExtras == null ) {
				loadSelectedExtras = new SqlCommand("HiResAdmin.getSelectedExtrasInfo");
				loadSelectedExtras.CommandType = CommandType.StoredProcedure;
				loadSelectedExtras.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
			}
			return loadSelectedExtras;
		}

		private SqlCommand GetOrdersCommand() {
			if ( loadOrdersCommand == null ) {
				loadOrdersCommand = new SqlCommand("HiResAdmin.getOrders");
				loadOrdersCommand.CommandType = CommandType.StoredProcedure;
				loadOrdersCommand.Parameters.Add(new SqlParameter(Filter, SqlDbType.NVarChar));
				loadOrdersCommand.Parameters.Add(new SqlParameter(Order, SqlDbType.NVarChar));

			}
			return loadOrdersCommand;
		}
		private SqlCommand GetUpdateOrderStatusCommand() {
			if ( updateOrderStatus == null ) {
				updateOrderStatus = new SqlCommand("HiResAdmin.UpdateOrderStatus");
				updateOrderStatus.CommandType = CommandType.StoredProcedure;
				updateOrderStatus.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				updateOrderStatus.Parameters.Add(new SqlParameter("@status", SqlDbType.Int));
			}
			return updateOrderStatus;
		}

		private SqlCommand GetUpdateOrderDeliveryDetails() {
			if (updateOrderDeliveryDetails == null) {
				updateOrderDeliveryDetails = new SqlCommand("HiResAdmin.UpdateDeliveryDetails");
				updateOrderDeliveryDetails.CommandType = CommandType.StoredProcedure;
				
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@PickupOrder", SqlDbType.Bit));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Country", SqlDbType.NVarChar,50));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@State", SqlDbType.NVarChar,20));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar,50));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@zipcode", SqlDbType.VarChar,10));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Address1", SqlDbType.NVarChar,255));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Address2", SqlDbType.NVarChar,255));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@SpecialInstructions", SqlDbType.NVarChar,255));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@CarrierID", SqlDbType.Int));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@ShippingMethodID", SqlDbType.VarChar,20));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Height", SqlDbType.Decimal));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Width", SqlDbType.Decimal));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Length", SqlDbType.Decimal));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@BoxesNumber", SqlDbType.Int));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@CarrierPackagingTypeID", SqlDbType.VarChar,20));
				//-----------------------------------
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.DateTime));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@ShippedDate", SqlDbType.DateTime));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@IsDelivered", SqlDbType.Bit));
				updateOrderDeliveryDetails.Parameters.Add(new SqlParameter("@ShipTrackingID", SqlDbType.VarChar,30));
				
			}

			return updateOrderDeliveryDetails;
		}

		private SqlCommand GetUpdateOrderAmounts() {
			//throw new NotImplementedException();
			if (updateOrderAmounts == null) {
				updateOrderAmounts = new SqlCommand("HiResAdmin.UpdateOrderAmounts");
				updateOrderAmounts.CommandType = CommandType.StoredProcedure;
				
				updateOrderAmounts.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountCalculated", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountExtras", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountShipping", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountDiscount", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountTax", SqlDbType.SmallMoney));
				//updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountDesign", SqlDbType.SmallMoney));

				updateOrderAmounts.Parameters.Add(new SqlParameter("@DesignHours", SqlDbType.Decimal));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@DesignCost", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@IsFixedDesignCost", SqlDbType.Bit));

				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountImages", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountScanning", SqlDbType.SmallMoney));

				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountProof", SqlDbType.SmallMoney));
				
				updateOrderAmounts.Parameters.Add(new SqlParameter("@PostageCostPerPiece", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@PostageQuantity", SqlDbType.Int));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@MailingPrcCostPerM", SqlDbType.SmallMoney));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@MailingPrcQuantity", SqlDbType.Int));
				updateOrderAmounts.Parameters.Add(new SqlParameter("@DiskPrepCost", SqlDbType.SmallMoney));

				updateOrderAmounts.Parameters.Add(new SqlParameter("@AmountTotal", SqlDbType.SmallMoney));
//				updateOrderAmounts.Parameters.Add(new SqlParameter("@RequiredDownpayment", SqlDbType.SmallMoney));
			}
			return updateOrderAmounts;
		}

		private SqlCommand GetLoadPartDesignsCommand() {
			if ( loadPartDesignsCmd == null ) {
				loadPartDesignsCmd = new SqlCommand("HiResAdmin.loadPartsDesigns");
				loadPartDesignsCmd.CommandType = CommandType.StoredProcedure;
				loadPartDesignsCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				loadPartDesignsCmd.Parameters.Add(new SqlParameter(FileCategory, SqlDbType.Int));
			}
			return loadPartDesignsCmd;
		}

		private SqlCommand GetAddPartDesignCommand() {
			if ( addPartDesignCmd == null ) {
				addPartDesignCmd = new SqlCommand("HiResAdmin.addPartDesign");
				addPartDesignCmd.CommandType = CommandType.StoredProcedure;
				addPartDesignCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				addPartDesignCmd.Parameters.Add(new SqlParameter(PartId, SqlDbType.Int));
				addPartDesignCmd.Parameters.Add(new SqlParameter(FileCategory, SqlDbType.Int));
				addPartDesignCmd.Parameters.Add(new SqlParameter("@fileName", SqlDbType.NVarChar));
				addPartDesignCmd.Parameters.Add(new SqlParameter("@specialInstructions", SqlDbType.NText));
			}
			return addPartDesignCmd;
		}

		private SqlCommand GetUpdatePartDesignFile() {
			if ( updatePartDesignFileCmd == null ) {
				updatePartDesignFileCmd = new SqlCommand("HiResAdmin.UpdatePartDesignFile");
				updatePartDesignFileCmd.CommandType = CommandType.StoredProcedure;
				updatePartDesignFileCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				updatePartDesignFileCmd.Parameters.Add(new SqlParameter(PartId, SqlDbType.Int));
				updatePartDesignFileCmd.Parameters.Add(new SqlParameter(FileCategory, SqlDbType.Int));
				updatePartDesignFileCmd.Parameters.Add(new SqlParameter("@blobdata", SqlDbType.Image));
				
			}
			return updatePartDesignFileCmd;
		}

		private SqlCommand GetPartDesignFileGetterCmd() {
			if ( getPartDesignFileCmd == null ) {
				getPartDesignFileCmd = new SqlCommand("HiResAdmin.GetPartDesignFile");
				getPartDesignFileCmd.CommandType = CommandType.StoredProcedure;
				
				getPartDesignFileCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				getPartDesignFileCmd.Parameters.Add(new SqlParameter(PartId, SqlDbType.Int));
				getPartDesignFileCmd.Parameters.Add(new SqlParameter(FileCategory, SqlDbType.Int));

			}
			return getPartDesignFileCmd;
		}

		private SqlCommand GetUpdatePartDesignCommand() {
			if ( updatePartDesignCmd == null ) {
				updatePartDesignCmd = new SqlCommand("HiResAdmin.updatePartDesign");
				updatePartDesignCmd.CommandType = CommandType.StoredProcedure;
				updatePartDesignCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				updatePartDesignCmd.Parameters.Add(new SqlParameter(PartId, SqlDbType.Int));
				updatePartDesignCmd.Parameters.Add(new SqlParameter(FileCategory, SqlDbType.Int));
				
				updatePartDesignCmd.Parameters.Add(new SqlParameter("@fileName", SqlDbType.NVarChar));
				updatePartDesignCmd.Parameters.Add(new SqlParameter("@specialInstructions", SqlDbType.NText));
			}
			return updatePartDesignCmd;
		}

		private SqlCommand GetRemovePartDesignCommand() {
			if ( removePartDesignCmd == null ) {
				removePartDesignCmd = new SqlCommand("HiResAdmin.removePartDesign");
				removePartDesignCmd.CommandType = CommandType.StoredProcedure;
				removePartDesignCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				removePartDesignCmd.Parameters.Add(new SqlParameter(PartId, SqlDbType.Int));
			}
			return removePartDesignCmd;
		}

		#region AuxFile getters
		private SqlCommand GetAddAuxFileCommand() {
			if ( addAuxFileCmd == null ) {
				addAuxFileCmd = new SqlCommand("HiResAdmin.addAuxFile");
				addAuxFileCmd.CommandType = CommandType.StoredProcedure;
				addAuxFileCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				addAuxFileCmd.Parameters.Add(new SqlParameter(PartId, SqlDbType.Int));
				addAuxFileCmd.Parameters.Add(new SqlParameter("@fileName", SqlDbType.NVarChar));
				addAuxFileCmd.Parameters.Add(new SqlParameter("@fileType", SqlDbType.Int));
				addAuxFileCmd.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar));
				addAuxFileCmd.Parameters.Add(new SqlParameter("@fileContentType", SqlDbType.VarChar,50));
				addAuxFileCmd.Parameters.Add(new SqlParameter("@blobdata", SqlDbType.Image));
	
	
				addAuxFileCmd.Parameters.Add(new SqlParameter("@auxFileId", SqlDbType.Int));
				addAuxFileCmd.Parameters["@auxFileId"].Direction = ParameterDirection.Output;
			}
			return addAuxFileCmd;
		}
		
		private SqlCommand GetRemoveAuxFileCommand() {
			if ( removeAuxFileCmd == null ) {
				removeAuxFileCmd = new SqlCommand("HiResAdmin.removeAuxFile");
				removeAuxFileCmd.CommandType = CommandType.StoredProcedure;
				removeAuxFileCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				removeAuxFileCmd.Parameters.Add(new SqlParameter("@auxFileId", SqlDbType.Int));
			}
			return removeAuxFileCmd;
		}
		
		private SqlCommand GetLoadAuxFilesCmd() {
			if (loadAuxFilesCmd == null) {
				loadAuxFilesCmd = new SqlCommand("HiResAdmin.getAuxFiles");
				loadAuxFilesCmd.CommandType = CommandType.StoredProcedure;
				loadAuxFilesCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
			}
			return loadAuxFilesCmd;
		}

		private SqlCommand GetAuxFileGetter() {
			SqlCommand cmd = new SqlCommand("HiResAdmin.getAuxFile");
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@auxFileId", SqlDbType.Int));
			return cmd;
		}
		#endregion
		private SqlCommand GetAddNewOrder() {
			//throw new NotImplementedException();
			if (addNewOrderCmd == null) {
				addNewOrderCmd = new SqlCommand("HiResAdmin.AddNewOrder");
				addNewOrderCmd.CommandType = CommandType.StoredProcedure;
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.NVarChar,30));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@SiteID", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@CreatedTS", SqlDbType.DateTime));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@DesignDue", SqlDbType.DateTime));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@PrintingDue", SqlDbType.DateTime));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@OrderJobType", SqlDbType.Int));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@PrintingTypeID", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@IsCustomQuantity", SqlDbType.Bit));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@PaperSizeID", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@CustomPaperSize", SqlDbType.VarChar, 20));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@PaperTypeID", SqlDbType.Int));
				//addNewOrderCmd.Parameters.Add(new SqlParameter("@HasExtras", SqlDbType.Bit));
				addNewOrderCmd.Parameters.Add(new SqlParameter(JobName, SqlDbType.VarChar, 40));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@Designer", SqlDbType.VarChar, 20));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@ProofType", SqlDbType.SmallInt));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@ProofNumber", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@PostageType", SqlDbType.SmallInt));
	

				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountCalculated", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountExtras", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountShipping", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountDiscount", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountTax", SqlDbType.SmallMoney));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountProof", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@PostageCostPerPiece", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@PostageQuantity", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@MailingPrcCostPerM", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@MailingPrcQuantity", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@DiskPrepCost", SqlDbType.SmallMoney));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@AmountTotal", SqlDbType.SmallMoney));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@RequiredDownpayment", SqlDbType.SmallMoney));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@PickupOrder", SqlDbType.Bit));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Country", SqlDbType.NVarChar,50));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@State", SqlDbType.NVarChar,20));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar,50));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@zipcode", SqlDbType.VarChar,10));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Address1", SqlDbType.NVarChar,255));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Address2", SqlDbType.NVarChar,255));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@SpecialInstructions", SqlDbType.NVarChar,255));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@CarrierID", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@ShippingMethodID", SqlDbType.VarChar,20));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Height", SqlDbType.Decimal));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Width", SqlDbType.Decimal));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Length", SqlDbType.Decimal));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@BoxesNumber", SqlDbType.Int));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@CarrierPackagingTypeID", SqlDbType.VarChar,20));
				
				addNewOrderCmd.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.DateTime));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@ShippedDate", SqlDbType.DateTime));
				addNewOrderCmd.Parameters.Add(new SqlParameter("@IsDelivered", SqlDbType.Bit));
				
				addNewOrderCmd.Parameters.Add(new SqlParameter("@CustomerDesignDescription", SqlDbType.VarChar,1024));

				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingFirstName, SqlDbType.NVarChar, 50));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingLastName, SqlDbType.NVarChar, 70));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingContactEmail, SqlDbType.NVarChar, 100));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingAddress1, SqlDbType.NVarChar, 255));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingAddress2, SqlDbType.NVarChar, 255));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingCountry, SqlDbType.NVarChar, 50));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingState, SqlDbType.NVarChar, 20));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingZipcode, SqlDbType.NVarChar, 10));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingCity, SqlDbType.NVarChar, 50));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingContactPhone, SqlDbType.NVarChar, 50));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingContactFax, SqlDbType.NVarChar, 50));
				addNewOrderCmd.Parameters.Add(new SqlParameter(BillingCompanyName, SqlDbType.NVarChar, 50));

				addNewOrderCmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
				addNewOrderCmd.Parameters["@OrderID"].Direction = ParameterDirection.Output;
			}
			return addNewOrderCmd;
		}
		/*
		private SqlCommand GetUpdateMainOrderInfo() {
			//throw new NotImplementedException();
			if (updateMainOrderInfoCmd == null) {
				updateMainOrderInfoCmd = new SqlCommand("HiResAdmin.AddNewOrder");
				updateMainOrderInfoCmd.CommandType = CommandType.StoredProcedure;
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.NVarChar,30));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@SiteID", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@CreatedTS", SqlDbType.DateTime));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@OrderJobType", SqlDbType.Int));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@PrintingTypeID", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@PaperSizeID", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@PaperTypeID", SqlDbType.Int));
				//updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@HasExtras", SqlDbType.Bit));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(JobName, SqlDbType.VarChar, 40));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountCalculated", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountExtras", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountShipping", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountDiscount", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountProof", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountDesign", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountTax", SqlDbType.SmallMoney));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@AmountTotal", SqlDbType.SmallMoney));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@DesignHours", SqlDbType.Decimal));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@DesignCost", SqlDbType.Decimal));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@RequiredDownpayment", SqlDbType.SmallMoney));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@PickupOrder", SqlDbType.Bit));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Country", SqlDbType.NVarChar,50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@State", SqlDbType.NVarChar,20));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar,50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@zipcode", SqlDbType.VarChar,10));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Address1", SqlDbType.NVarChar,255));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Address2", SqlDbType.NVarChar,255));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@SpecialInstructions", SqlDbType.NVarChar,255));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@CarrierID", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@ShippingMethodID", SqlDbType.VarChar,20));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Height", SqlDbType.Decimal));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Width", SqlDbType.Decimal));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Length", SqlDbType.Decimal));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Decimal));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@BoxesNumber", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@CarrierPackagingTypeID", SqlDbType.VarChar,20));
				
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.DateTime));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@ShippedDate", SqlDbType.DateTime));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@IsDelivered", SqlDbType.Bit));
				
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@CustomerDesignDescription", SqlDbType.VarChar,1024));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingFirstName, SqlDbType.NVarChar, 50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingLastName, SqlDbType.NVarChar, 70));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingContactEmail, SqlDbType.NVarChar, 100));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingAddress1, SqlDbType.NVarChar, 255));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingAddress2, SqlDbType.NVarChar, 255));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingCountry, SqlDbType.NVarChar, 50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingState, SqlDbType.NVarChar, 20));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingZipcode, SqlDbType.NVarChar, 10));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingCity, SqlDbType.NVarChar, 50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingContactPhone, SqlDbType.NVarChar, 50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingContactFax, SqlDbType.NVarChar, 50));
				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter(BillingCompanyName, SqlDbType.NVarChar, 50));

				updateMainOrderInfoCmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
				updateMainOrderInfoCmd.Parameters["@OrderID"].Direction = ParameterDirection.Output;
			}
			return updateMainOrderInfoCmd;
		}
		*/
		
		
		private SqlCommand GetUpdateOrderMailingInfo() {
			if ( updateMailingInfoCmd == null ) {
				updateMailingInfoCmd = new SqlCommand("HiResAdmin.UpdateOrderMailingInfo");
				updateMailingInfoCmd.CommandType = CommandType.StoredProcedure;
				updateMailingInfoCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				updateMailingInfoCmd.Parameters.Add(new SqlParameter(PM_PostageType, SqlDbType.SmallInt));
				updateMailingInfoCmd.Parameters.Add(new SqlParameter(PM_MailDue, SqlDbType.DateTime));
			}
			return updateMailingInfoCmd;
		}

		private SqlCommand GetUpdatePDInfoCommand() {
			if ( updatePDInfoCmd == null ) {
				updatePDInfoCmd = new SqlCommand("HiResAdmin.UpdatePDInfo");
				updatePDInfoCmd.CommandType = CommandType.StoredProcedure;
				updatePDInfoCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				updatePDInfoCmd.Parameters.Add(new SqlParameter(UPNo, SqlDbType.Int));
				updatePDInfoCmd.Parameters.Add(new SqlParameter(PSNo, SqlDbType.Int));
				updatePDInfoCmd.Parameters.Add(new SqlParameter("@ProofType", SqlDbType.SmallInt));
				updatePDInfoCmd.Parameters.Add(new SqlParameter("@ProofNumber", SqlDbType.Int));
				updatePDInfoCmd.Parameters.Add(new SqlParameter("@PrintingDue", SqlDbType.DateTime));
			}
			return updatePDInfoCmd;
		}

		private SqlCommand GetUpdateDDInfoCommand() {
			if ( updateDDInfoCmd == null ) {
				updateDDInfoCmd = new SqlCommand("HiResAdmin.UpdateDDInfo");
				updateDDInfoCmd.CommandType = CommandType.StoredProcedure;
				updateDDInfoCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));

				updateDDInfoCmd.Parameters.Add(new SqlParameter("@ProofType", SqlDbType.SmallInt));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@ProofNumber", SqlDbType.Int));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@ImageSource", SqlDbType.SmallInt));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@ImagesNumber", SqlDbType.Int));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@ScanType", SqlDbType.SmallInt));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@ScanNumber", SqlDbType.Int));

				updateDDInfoCmd.Parameters.Add(new SqlParameter("@Designer", SqlDbType.VarChar,20));

				/*updateDDInfoCmd.Parameters.Add(new SqlParameter("@DesignHours", SqlDbType.Decimal));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@@DesignCost", SqlDbType.SmallMoney));
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@IsFixedDesignCost", SqlDbType.Bit));*/
				updateDDInfoCmd.Parameters.Add(new SqlParameter("@DesignDue", SqlDbType.DateTime));
			}
			return updateDDInfoCmd;

		}

		#region payment commands getters

		private SqlCommand GetLoadPaymentsCmd() {
			if ( loadPaymentsCmd == null ) {
				loadPaymentsCmd = new SqlCommand("HiResAdmin.getPayments");
				loadPaymentsCmd.CommandType = CommandType.StoredProcedure;
				loadPaymentsCmd.Parameters.Add(new SqlParameter("@filterClause", SqlDbType.NVarChar));
				loadPaymentsCmd.Parameters.Add(new SqlParameter("@orderClause", SqlDbType.NVarChar));
			}
			return loadPaymentsCmd;
		}

		private SqlCommand GetAddPaymentCmd() {
			if ( addPaymentCmd == null ) {
				addPaymentCmd = new SqlCommand("HiResAdmin.addPayment");
				addPaymentCmd.CommandType = CommandType.StoredProcedure;
				addPaymentCmd.Parameters.Add(new SqlParameter("@txnReferenceID", SqlDbType.VarChar, 20));
				addPaymentCmd.Parameters.Add(new SqlParameter("@paymentDate", SqlDbType.DateTime));
				addPaymentCmd.Parameters.Add(new SqlParameter("@amount", SqlDbType.SmallMoney));
				addPaymentCmd.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
				addPaymentCmd.Parameters.Add(new SqlParameter("@paymentId", SqlDbType.Int));
				addPaymentCmd.Parameters["@paymentId"].Direction = ParameterDirection.Output;
			}
			return addPaymentCmd;
		}

		private SqlCommand GetRemovePaymentCmd() {
			if ( removePaymentCmd== null ) {
				removePaymentCmd = new SqlCommand("HiResAdmin.removePayment");
				removePaymentCmd.CommandType = CommandType.StoredProcedure;
				removePaymentCmd.Parameters.Add(new SqlParameter("@paymentId", SqlDbType.Int));
			}
			return removePaymentCmd;
		}

		#endregion

		#region Extras command getters
		private SqlCommand GetAddExtrasCmd() {
			if (addExtrasCmd == null) {
				addExtrasCmd = new SqlCommand("HiResAdmin.AddExtras");
				addExtrasCmd.CommandType = CommandType.StoredProcedure;
				addExtrasCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				addExtrasCmd.Parameters.Add(new SqlParameter("@extras", SqlDbType.VarChar,255));
			}
			return addExtrasCmd;
		}
/*
	@ExtraID	int,
	@OrderID int,
	@Price smallmoney,
	@QuantityToApply int,
	@IsPricePerM bit,
	@TotalExtraAmount smallmoney,
	@Attribute1Value varchar(20),
	@Attribute2Value varchar(20),
	@Attribute3Value varchar(20),
	@Attribute4Value varchar(20)
*/
		private SqlCommand GetUpdateSelectedExtraCmd() {

			if (updateSelectedExtraCmd == null) {
				updateSelectedExtraCmd = new SqlCommand("HiResAdmin.UpdateSelectedExtra");
				updateSelectedExtraCmd.CommandType = CommandType.StoredProcedure;
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@ExtraID", SqlDbType.Int));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@Price", SqlDbType.SmallMoney));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@QuantityToApply", SqlDbType.Int));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@IsPricePerM", SqlDbType.Bit));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@TotalExtraAmount", SqlDbType.SmallMoney));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@Attribute1Value", SqlDbType.VarChar,20));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@Attribute2Value", SqlDbType.VarChar,20));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@Attribute3Value", SqlDbType.VarChar,20));
				updateSelectedExtraCmd.Parameters.Add(new SqlParameter("@Attribute4Value", SqlDbType.VarChar,20));
				
			}
			return updateSelectedExtraCmd;

		}
		
		private SqlCommand GetDeleteSelectedExtrasCmd() {
			if (delExtrasCmd == null) {
				delExtrasCmd = new SqlCommand("HiResAdmin.DeleteSelectedExtras");
				delExtrasCmd.CommandType = CommandType.StoredProcedure;
				delExtrasCmd.Parameters.Add(new SqlParameter(OrderID, SqlDbType.Int));
			}
			return delExtrasCmd;
		}

		#endregion

		#region Order logs command getters
		/// <summary>
		/// </summary>
		/// <remarks>command should be disposed after usage because it is not disposed by component destructor</remarks>
		/// <returns></returns>
		private SqlCommand GetAddOrderLogEntryCommand() {
			SqlCommand cmd = new SqlCommand("HiResAdmin.AddOrderLogEntry");
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int));
			cmd.Parameters.Add(new SqlParameter("@eventTimeStamp", SqlDbType.DateTime));
			cmd.Parameters.Add(new SqlParameter("@Category", SqlDbType.VarChar,30));
			cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar,30));
			cmd.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar,1024));
			cmd.Parameters.Add(new SqlParameter("@EmployeeUID", SqlDbType.VarChar,20));
			return cmd;
		}

		/// <summary>
		/// </summary>
		/// <remarks>command should be disposed after usage because it is not disposed by component destructor</remarks>
		/// <returns></returns>
		private SqlCommand GetFullOrderLogCmd() {
			SqlCommand cmd = new SqlCommand("HiResAdmin.GetFullOrderLog");
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int));
			return cmd;
		}

		#endregion

		#endregion

		#region Data Loaders

		public OrderInfo GetOrderInfo(int orderID) {
			SqlCommand cmd = GetLoadOrderCommand();
			cmd.Parameters[OrderID].Value = orderID;
			OrderInfo order = null;
			SqlConnection conn = null;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					order = new OrderInfo();
					order.OrderId = (int)reader["OrderId"];
					order.Amounts = new OrderInfo.PaymentAmounts();
					//if (reader["AmountCalculated"] != DBNull.Value) 
						order.Amounts.CalculatedAmount = (decimal)reader["AmountCalculated"];
					//if (reader["AmountShipping"] != DBNull.Value)
						order.Amounts.ShippingAmount = (decimal)reader["AmountShipping"];
					//if (reader["AmountDiscount"] != DBNull.Value)
						order.Amounts.DiscountAmount = (decimal)reader["AmountDiscount"];
					//if (reader["AmountTax"] != DBNull.Value)
						order.Amounts.TaxAmount = (decimal)reader["AmountTax"];
					//if (reader["AmountTotal"] != DBNull.Value)
						//order.Amounts.TotalAmount = (decimal)reader["AmountTotal"];
					//if (reader["AmountDesign"] != DBNull.Value)
						//order.Amounts.DesignAmount = (decimal)reader["AmountDesign"];
					order.Amounts.DesignHours = (decimal)reader["DesignHours"];
					order.Amounts.DesignCost = (decimal)reader["DesignCost"];
					order.Amounts.IsDesignCostFixed = (bool)reader["IsFixedDesignCost"];

					order.Amounts.ImagesAmount = (decimal)reader["AmountImages"];
					order.Amounts.ScanAmount = (decimal)reader["AmountScanning"];
					order.Amounts.ProofAmount = (decimal)reader["AmountProof"];

					order.Amounts.PostageCostPerPiece = (decimal)reader["PostageCostPerPiece"];
					order.Amounts.PostageQuantity = (int)reader["PostageQuantity"];
					order.Amounts.MailingPrcCostPerM = (decimal)reader["MailingPrcCostPerM"];
					order.Amounts.MailingPrcQuantity = (int)reader["MailingPrcQuantity"];
					order.Amounts.DiskPrepCost = (decimal)reader["DiskPrepCost"];
					order.Amounts.MailingListsTotalCost = (decimal)reader["MailingListsTotalCost"];

					if (reader["RequiredDownpayment"] != DBNull.Value)
						order.DownpaymentRequired = (decimal)reader["RequiredDownpayment"];
					//if (reader["AmountExtras"] != DBNull.Value)
						order.Amounts.ExtrasAmount = (decimal)reader["AmountExtras"];
					
					order.TotalAmountPaid = (decimal)reader["TotalAmountPaid"];

					if (reader["UPNo"] != DBNull.Value)
						order.UPNo = (int)reader["UPNo"];
					if (reader["PSNo"] != DBNull.Value)
						order.PSNo = (int)reader["PSNo"];
					
					if (reader["PlacedTS"] != DBNull.Value)
						order.PlacedTS = (DateTime)reader["PlacedTS"];
					if (reader["LastModifiedTS"] != DBNull.Value)
						order.LastModifiedTS = (DateTime)reader["LastModifiedTS"];
					if (reader["DesignDue"] != DBNull.Value)
						order.DesignDue = (DateTime)reader["DesignDue"];
					if (reader["PrintingDue"] != DBNull.Value)
						order.PrintingDue = (DateTime)reader["PrintingDue"];
					if (reader["MailingDue"] != DBNull.Value)
						order.MailingDue = (DateTime)reader["MailingDue"];
					
					order.Status = (OrderStatus)reader["Status"];
					if (reader["IsCancelled"] != DBNull.Value)
						order.IsCancelled = (bool)reader["IsCancelled"];
					
					//TODO: consider removing
					if (reader["ShipTrackingID"] != DBNull.Value)
						order.ShipTrackingID = (String)reader["ShipTrackingID"];

					if (reader["CustomerID"] != DBNull.Value)
						order.CustomerID = (String)reader["CustomerID"];
					order.OrderJob = new OrderInfo.JobInfo();
					order.OrderJob.PrintingTypeID = (int)reader["PrintingTypeID"];
					if (reader["PaperTypeID"] != DBNull.Value)
						order.OrderJob.PaperTypeID = (int)reader["PaperTypeID"];

					if (reader["PaperSizeID"] != DBNull.Value)
						order.OrderJob.PaperSizeID = (int)reader["PaperSizeID"];
					if (reader["CustomPaperSize"] != DBNull.Value) {
						order.OrderJob.CustomPaperSize = (string)reader["CustomPaperSize"];
					}

					order.OrderJob.IsCustomQuantity = (bool)reader["IsCustomQuantity"];
					order.OrderJob.Quantity = (int)reader["Quantity"];
					
					order.OrderJob.HasExtras = ((int)reader["SelectedExtrasNum"])>0;

					//order.OrderJob.HasExtras = (bool)reader["HasExtras"];

					order.OrderJob.JobType = (JobType)reader["OrderJobType"];
					order.OrderJob.PostageType = (PostageType)((short)reader["PostageType"]);

					if (reader["JobName"] != DBNull.Value) {
						order.OrderJob.JobName = (string)reader["JobName"];
					}
					//short pt = (short)reader["ProofType"];
					order.OrderJob.ProofType = (ProofType)((short)reader["ProofType"]);
					order.OrderJob.ScanType = (ScanType)((short)reader["ScanType"]);
					order.OrderJob.ImageSourceId = (short)reader["ImageSource"];

					order.OrderJob.ProofNumber = (int)reader["ProofNumber"];
					order.OrderJob.ScanNumber = (int)reader["ScanNumber"];
					order.OrderJob.ImagesNumber = (int)reader["ImagesNumber"];

					order.SiteID = (int)reader["SiteID"];
/*					if (reader["OrderLog"] != DBNull.Value)
						order.OrderLog = (String)reader["OrderLog"];*/
					//if (reader["DesignHours"] != DBNull.Value)

					if (reader["Designer"] != DBNull.Value)
						order.Designer = (string)reader["Designer"];
					if (reader["CustomerDesignDescription"] != DBNull.Value) {
						order.Design.CustomerDescription = (string)reader["CustomerDesignDescription"];
					}
					//CustomerDesignDescription
				}
				//reader.Close();
				// load related info
				if (order != null) {
					order.BillTo = new ContactInfo();
					if (reader["BillingCompanyName"] != DBNull.Value)
						order.BillTo.CompanyName = (string)reader["BillingCompanyName"];
					if (reader["BillingFirstName"] != DBNull.Value)
						order.BillTo.FirstName = (string)reader["BillingFirstName"];
					if (reader["BillingLastName"] != DBNull.Value)
						order.BillTo.LastName = (string)reader["BillingLastName"];
					if (reader["BillingAddress1"] != DBNull.Value)
						order.BillTo.Address.Address1 = (string)reader["BillingAddress1"];
					if (reader["BillingAddress2"] != DBNull.Value)
						order.BillTo.Address.Address2 = (string)reader["BillingAddress2"];
					if (reader["BillingCity"] != DBNull.Value)
						order.BillTo.Address.City = (string)reader["BillingCity"];
					if (reader["BillingState"] != DBNull.Value)
						order.BillTo.Address.State = (string)reader["BillingState"];
					if (reader["BillingZipcode"] != DBNull.Value)
						order.BillTo.Address.ZipCode = (string)reader["BillingZipcode"];
					if (reader["BillingContactEmail"] != DBNull.Value)
						order.BillTo.ContactEmail = (string)reader["BillingContactEmail"];
					if (reader["BillingContactFax"] != DBNull.Value)
						order.BillTo.ContactFax = (string)reader["BillingContactFax"];
					if (reader["BillingContactPhone"] != DBNull.Value)
						order.BillTo.ContactPhone = (string)reader["BillingContactPhone"];
					
					order.BillTo.ContactPhone2 = string.Empty;
					
					order.OrderJob.Extras = GetSelectedExtrasFull(order.OrderId);
					order.DeliveryDetails = GetDeliveryDetailsInfo(order.OrderId);
				}
//			} catch (Exception ex) {
//				throw;
			} finally {
				//				cmd.Connection = null;
				if (!reader.IsClosed) { reader.Close(); }
				if ((conn != null)&&(conn.State!=ConnectionState.Closed)) {
					conn.Close(); 
				}
			}
			return order;
		}

		public DeliveryDetailsInfo GetDeliveryDetailsInfo(int orderID) {
			SqlCommand cmd = GetLoadDeliveryDetailsCommand();
			cmd.Parameters[OrderID].Value = orderID;
			DeliveryDetailsInfo delivery = new DeliveryDetailsInfo();
			SqlConnection conn = null;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					//delivery = new DeliveryDetailsInfo();
					//delivery.OrderId = (int)reader["OrderId"];
					delivery.ShipAddress = new AddressInfo();
					delivery.PickUpOrder = (bool)reader["PickupOrder"];
					if (reader["ShipDate"] != DBNull.Value) 
						delivery.ShipDate = (DateTime)reader["ShipDate"];
					if (reader["ShippedDate"] != DBNull.Value) 
						delivery.ShippedDate = (DateTime)reader["ShippedDate"];
					delivery.IsDelivered = (bool)reader["IsDelivered"];

					//if order is not a pickup from hires load other info
					if (!delivery.PickUpOrder) {

						if (reader["Country"] != DBNull.Value) 
							delivery.ShipAddress.Country = (String)reader["Country"];
						if (reader["State"] != DBNull.Value) 
							delivery.ShipAddress.State = (String)reader["State"];
						if (reader["City"] != DBNull.Value) 
							delivery.ShipAddress.City = (String)reader["City"];
						if (reader["ZipCode"] != DBNull.Value) 
							delivery.ShipAddress.ZipCode = (String)reader["zipcode"];
						if (reader["Address1"] != DBNull.Value) 
							delivery.ShipAddress.Address1 = (String)reader["Address1"];
						if (reader["Address2"] != DBNull.Value) 
							delivery.ShipAddress.Address2 = (String)reader["Address2"];
						if (reader["ShipDate"] != DBNull.Value) 
							delivery.ShipDate = (DateTime)reader["ShipDate"];
						if (reader["ShippedDate"] != DBNull.Value) 
							delivery.ShippedDate = (DateTime)reader["ShippedDate"];
						if (reader["SpecialInstructions"] != DBNull.Value) 
							delivery.SpecialInstructions = (String)reader["SpecialInstructions"];
						//if (reader["PickupOrder"] != DBNull.Value) 
						if (reader["CarrierID"] != DBNull.Value) 
							delivery.Carrier = (PostalCarrier)reader["CarrierID"];

						if (reader["ShippingMethodID"] != DBNull.Value) 
							delivery.ShipMethod = (String)reader["ShippingMethodID"];

					
						if (reader["Height"] != DBNull.Value) 
							delivery.Packaging.Height = (decimal)reader["Height"];
						if (reader["Width"] != DBNull.Value) 
							delivery.Packaging.Width  = (decimal)reader["Width"];
						if (reader["Length"] != DBNull.Value) 
							delivery.Packaging.Length = (decimal)reader["Length"];
						if (reader["Weight"] != DBNull.Value) 
							delivery.Packaging.Weight = (decimal)reader["Weight"];
						if (reader["CarrierPackagingTypeID"] != DBNull.Value) 
							delivery.Packaging.CarrierPackageTypeId = (String)reader["CarrierPackagingTypeID"];
						if (reader["BoxesNumber"] != DBNull.Value) 
							delivery.Packaging.BoxesNumber = (int)reader["BoxesNumber"];

						//TODO: consider removing
						if (reader["ShipTrackingID"] != DBNull.Value)
							delivery.ShipTrackingID = (string)reader["ShipTrackingID"];
					}

				}
				//reader.Close();
			} catch {
				throw;
			} finally {
				//cmd.Connection = null;
				if (!reader.IsClosed) { reader.Close(); }
				if (conn != null) conn.Close();
			}
			return delivery;
		}
		[Obsolete("Use 'GetSelectedExtrasFull'",true)]
		public ArrayList GetSelectedExtras(int orderID) {
			SqlCommand cmd = GetLoadSelectedExtrasCommand();
			cmd.Parameters[OrderID].Value = orderID;
				
			SqlConnection conn = null;
			ArrayList list = new ArrayList();
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					list.Add(reader["ExtraID"]);
				}
				reader.Close();
//			} catch {
//				throw;
			} finally {
				//cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return list;
		}

		public /*ArrayList*/Hashtable GetSelectedExtrasFull(int orderID) {
			SqlCommand cmd = GetLoadSelectedExtrasCommand();
			cmd.Parameters[OrderID].Value = orderID;
			SqlConnection conn = null;
			Hashtable extras = new Hashtable();
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				SelectedExtraInfo selectedExtra = null;
				while(reader.Read()) {
					ExtraAttribute[] AttributesValues = new ExtraAttribute[FullExtraInfo.MAX_EXTRA_ATTR_NUM];
					for(int i = 1; i<=FullExtraInfo.MAX_EXTRA_ATTR_NUM;i++) {
						/*ExtraAttributeType type = (ExtraAttributeType)reader["Attribute"+i+"Type"];
						string attrname = (string)reader["Attribute"+i+"Name"];*/
						ExtraAttributeType type = (ExtraAttributeType)/*(byte)*/reader["Attribute"+i+"Type"];
						if (type == ExtraAttributeType.None) {
							AttributesValues[i-1] = new ExtraAttribute(type);
						} else {
							//object attrname = reader["Attribute"+i+"Name"];
							AttributesValues[i-1] = new ExtraAttribute(type, (string)reader["Attribute"+i+"Name"]);
							AttributesValues[i-1].ParseStringValue((string)reader["Attribute"+i+"Value"]);
						}
					}

					selectedExtra = new SelectedExtraInfo(AttributesValues);
					selectedExtra.ExtraID = (int)reader["ExtraID"];
					selectedExtra.Price = (decimal)reader["Price"];
					selectedExtra.IsPricePerM = (bool)reader["IsPricePerM"];
					selectedExtra.QuantityToApply = (int)reader["QuantityToApply"];
					selectedExtra.TotalExtraAmount= (decimal)reader["TotalExtraAmount"];

					extras.Add(selectedExtra.ExtraID,selectedExtra);
				}
				reader.Close();
			} catch (Exception ex) {
				throw;
			} finally {
				//cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return extras;
		
		}

		public OrderInfo[] GetOrders(FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetOrdersCommand();
			cmd.Parameters[Filter].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters[Order].Value = orderBy == null ? "" : orderBy.ToString();
			OrderInfo[] orders = null;
			SqlConnection conn = null;
			ArrayList array = new ArrayList();
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader();
				while(reader.Read()) {
					OrderInfo orderInfo = new OrderInfo();
					orderInfo.OrderId = (int)reader["OrderId"];
					
					orderInfo.Amounts = new OrderInfo.PaymentAmounts();
					//if (reader["AmountCalculated"] != DBNull.Value) 
						orderInfo.Amounts.CalculatedAmount = (decimal)reader["AmountCalculated"];
					//if (reader["AmountShipping"] != DBNull.Value)
						orderInfo.Amounts.ShippingAmount = (decimal)reader["AmountShipping"];
					//if (reader["AmountDiscount"] != DBNull.Value)
						orderInfo.Amounts.DiscountAmount = (decimal)reader["AmountDiscount"];
					//if (reader["AmountDesign"] != DBNull.Value)
						//orderInfo.Amounts.DesignAmount = (decimal)reader["AmountDesign"];
					//if (reader["DesignHours"] != DBNull.Value)
					orderInfo.Amounts.DesignHours = (decimal)reader["DesignHours"];
					orderInfo.Amounts.DesignCost = (decimal)reader["DesignCost"];
					orderInfo.Amounts.IsDesignCostFixed = (bool)reader["IsFixedDesignCost"];

					orderInfo.Amounts.ImagesAmount = (decimal)reader["AmountImages"];
					orderInfo.Amounts.ScanAmount = (decimal)reader["AmountScanning"];

					//if (reader["AmountExtras"] != DBNull.Value)
					orderInfo.Amounts.ExtrasAmount = (decimal)reader["AmountExtras"];
					orderInfo.Amounts.ProofAmount = (decimal)reader["AmountProof"];
					
					orderInfo.Amounts.PostageCostPerPiece = (decimal)reader["PostageCostPerPiece"];
					orderInfo.Amounts.PostageQuantity = (int)reader["PostageQuantity"];
					orderInfo.Amounts.MailingPrcCostPerM = (decimal)reader["MailingPrcCostPerM"];
					orderInfo.Amounts.MailingPrcQuantity = (int)reader["MailingPrcQuantity"];
					orderInfo.Amounts.DiskPrepCost = (decimal)reader["DiskPrepCost"];
					orderInfo.Amounts.MailingListsTotalCost = (decimal)reader["MailingListsTotalCost"];
					
					//orderInfo.Amounts.ProofAmount = (decimal)reader["AmountProof"];

					//if (reader["AmountTax"] != DBNull.Value)
						orderInfo.Amounts.TaxAmount = (decimal)reader["AmountTax"];
					//if (reader["AmountTotal"] != DBNull.Value)
						//orderInfo.Amounts.TotalAmount = (decimal)reader["AmountTotal"];
					if (reader["RequiredDownpayment"] != DBNull.Value)
						orderInfo.DownpaymentRequired = (decimal)reader["RequiredDownpayment"];
					
					if (reader["PlacedTS"] != DBNull.Value)
						orderInfo.PlacedTS = (DateTime)reader["PlacedTS"];
					if (reader["LastModifiedTS"] != DBNull.Value)
						orderInfo.LastModifiedTS = (DateTime)reader["LastModifiedTS"];
					
					if (reader["DesignDue"] != DBNull.Value)
						orderInfo.DesignDue = (DateTime)reader["DesignDue"];
					if (reader["PrintingDue"] != DBNull.Value)
						orderInfo.PrintingDue = (DateTime)reader["PrintingDue"];
					if (reader["MailingDue"] != DBNull.Value)
						orderInfo.MailingDue = (DateTime)reader["MailingDue"];

					orderInfo.Status = (OrderStatus)reader["Status"];
					if (reader["IsCancelled"] != DBNull.Value)
						orderInfo.IsCancelled = (bool)reader["IsCancelled"];
					if (reader["ShipTrackingID"] != DBNull.Value)
						orderInfo.ShipTrackingID = (String)reader["ShipTrackingID"];
					if (reader["CustomerID"] != DBNull.Value)
						orderInfo.CustomerID = (String)reader["CustomerID"];

					orderInfo.OrderJob = new OrderInfo.JobInfo();
					orderInfo.OrderJob.PrintingTypeID = (int)reader["PrintingTypeID"];
					if (reader["PaperTypeID"] != DBNull.Value)
						orderInfo.OrderJob.PaperTypeID = (int)reader["PaperTypeID"];
					if (reader["PaperSizeID"] != DBNull.Value)
						orderInfo.OrderJob.PaperSizeID = (int)reader["PaperSizeID"];
					if (reader["CustomPaperSize"] != DBNull.Value) {
						orderInfo.OrderJob.CustomPaperSize = (string)reader["CustomPaperSize"];
					}

					orderInfo.OrderJob.IsCustomQuantity = (bool)reader["IsCustomQuantity"];
					orderInfo.OrderJob.Quantity = (int)reader["Quantity"];

					if (reader["JobName"] != DBNull.Value) {
						orderInfo.OrderJob.JobName = (string)reader["JobName"];
					}
					
					if (reader["UPNo"] != DBNull.Value)
						orderInfo.UPNo = (int)reader["UPNo"];
					if (reader["PSNo"] != DBNull.Value)
						orderInfo.PSNo = (int)reader["PSNo"];
					
					//orderInfo.OrderJob.HasExtras = (bool)reader["HasExtras"];
					bool hasextras = ((int)reader["SelectedExtrasNum"])>0;
					orderInfo.OrderJob.HasExtras = ((int)reader["SelectedExtrasNum"])>0;

					orderInfo.OrderJob.JobType = (JobType)reader["OrderJobType"];
					orderInfo.OrderJob.PostageType = (PostageType)((short)reader["PostageType"]);

					orderInfo.SiteID = (int)reader["SiteID"];
/*					if (reader["OrderLog"] != DBNull.Value)
						orderInfo.OrderLog = (String)reader["OrderLog"];*/

					if (reader["TotalAmountPaid"] != DBNull.Value){
						orderInfo.TotalAmountPaid = (decimal)reader["TotalAmountPaid"];
					} else {
						orderInfo.TotalAmountPaid = 0.00m;
					}

					orderInfo.DeliveryDetails.PickUpOrder = (bool)reader["PickupOrder"];
					array.Add(orderInfo);
				}
				//reader.Close();
			} catch (Exception ex) {
				throw ex;
			} finally {
				if (!reader.IsClosed) { reader.Close(); }
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			if (array.Count != 0) {
				orders = new OrderInfo[array.Count];
				array.CopyTo(orders);
			}
			return orders;
		}


		#endregion
		


		public bool AddNewOrder(ref OrderInfo orderInfo) {
			return AddNewOrder(ref orderInfo, null);
		}

		public bool AddNewOrder(ref OrderInfo orderInfo, IDbTransaction transaction) {
			bool txnNull = (transaction == null);
		
			SqlConnection conn = null;

			if (txnNull) {
				try {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
					transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
				} catch (Exception ex){
					if (txnNull) {
						if (transaction!=null) {
							transaction.Dispose();
						}
						if (conn != null) conn.Close();
					}
					AppLog.LogError("Error while beginning transaction",ex);
					return false;
				}
			}
			try {
				SqlCommand cmd = GetAddNewOrder();
				cmd.Transaction = (SqlTransaction)transaction;
				cmd.Connection = cmd.Transaction.Connection;

				#region Params setting
				cmd.Parameters["@Status"].Value = orderInfo.Status;
				cmd.Parameters["@CustomerID"].Value = orderInfo.CustomerID;
				cmd.Parameters["@SiteID"].Value = orderInfo.SiteID;
				cmd.Parameters["@CreatedTS"].Value = orderInfo.CreatedTS;

				cmd.Parameters["@DesignDue"].Value = orderInfo.DesignDue;
				cmd.Parameters["@PrintingDue"].Value = orderInfo.PrintingDue;

				cmd.Parameters["@OrderJobType"].Value = orderInfo.OrderJob.JobType;
				cmd.Parameters["@PrintingTypeID"].Value = orderInfo.OrderJob.PrintingTypeID;
				cmd.Parameters["@IsCustomQuantity"].Value = orderInfo.OrderJob.IsCustomQuantity;
				cmd.Parameters["@Quantity"].Value = orderInfo.OrderJob.Quantity;
				cmd.Parameters["@PaperSizeID"].Value = orderInfo.OrderJob.PaperSizeID;
				if (orderInfo.OrderJob.CustomPaperSize!=null) {
					cmd.Parameters["@CustomPaperSize"].Value = orderInfo.OrderJob.CustomPaperSize;
				}
				cmd.Parameters["@PaperTypeID"].Value = orderInfo.OrderJob.PaperTypeID;
				//cmd.Parameters["@HasExtras"].Value = orderInfo.OrderJob.HasExtras;
				cmd.Parameters[JobName].Value = ((orderInfo.OrderJob.JobName==null)||(orderInfo.OrderJob.JobName==String.Empty))?DBNull.Value:(object)orderInfo.OrderJob.JobName;

				if (orderInfo.Designer==null) {
					cmd.Parameters["@Designer"].Value = DBNull.Value;
				} else {
					cmd.Parameters["@Designer"].Value = orderInfo.Designer;
				}
				cmd.Parameters["@ProofType"].Value = (short)orderInfo.OrderJob.ProofType;
				cmd.Parameters["@ProofNumber"].Value = orderInfo.OrderJob.ProofNumber;
				cmd.Parameters["@PostageType"].Value = (short)orderInfo.OrderJob.PostageType;

				cmd.Parameters["@AmountCalculated"].Value = orderInfo.Amounts.CalculatedAmount;
				cmd.Parameters["@AmountExtras"].Value = orderInfo.Amounts.ExtrasAmount;
				cmd.Parameters["@AmountShipping"].Value = orderInfo.Amounts.ShippingAmount;
				cmd.Parameters["@AmountDiscount"].Value = orderInfo.Amounts.DiscountAmount;
				cmd.Parameters["@AmountTax"].Value = orderInfo.Amounts.TaxAmount;

				cmd.Parameters["@AmountProof"].Value = orderInfo.Amounts.ProofAmount;
				cmd.Parameters["@PostageCostPerPiece"].Value = orderInfo.Amounts.PostageCostPerPiece;
				cmd.Parameters["@PostageQuantity"].Value = orderInfo.Amounts.PostageQuantity;
				cmd.Parameters["@MailingPrcCostPerM"].Value = orderInfo.Amounts.MailingPrcCostPerM;
				cmd.Parameters["@MailingPrcQuantity"].Value = orderInfo.Amounts.MailingPrcQuantity;
				cmd.Parameters["@DiskPrepCost"].Value = orderInfo.Amounts.DiskPrepCost;

				cmd.Parameters["@AmountTotal"].Value = orderInfo.Amounts.TotalAmount;
				cmd.Parameters["@RequiredDownpayment"].Value = orderInfo.DownpaymentRequired;
			
				cmd.Parameters["@PickupOrder"].Value = orderInfo.DeliveryDetails.PickUpOrder;
			
				cmd.Parameters["@Country"].Value = ((orderInfo.DeliveryDetails.ShipAddress.Country==null)||(orderInfo.DeliveryDetails.ShipAddress.Country==String.Empty))?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.Country;
				cmd.Parameters["@State"].Value = (orderInfo.DeliveryDetails.ShipAddress.State==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.State;
				cmd.Parameters["@City"].Value = (orderInfo.DeliveryDetails.ShipAddress.City==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.City;
				cmd.Parameters["@zipcode"].Value = (orderInfo.DeliveryDetails.ShipAddress.ZipCode==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.ZipCode;
				cmd.Parameters["@Address1"].Value = (orderInfo.DeliveryDetails.ShipAddress.Address1==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.Address1;
				cmd.Parameters["@Address2"].Value = (orderInfo.DeliveryDetails.ShipAddress.Address2==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.Address2;

				cmd.Parameters["@SpecialInstructions"].Value = (orderInfo.DeliveryDetails.SpecialInstructions==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.SpecialInstructions;
				cmd.Parameters["@CarrierID"].Value = (int)orderInfo.DeliveryDetails.Carrier;
				cmd.Parameters["@ShippingMethodID"].Value = (orderInfo.DeliveryDetails.ShipMethod==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipMethod;

				cmd.Parameters["@Height"].Value = orderInfo.DeliveryDetails.Packaging.Height;
				cmd.Parameters["@Width"].Value = orderInfo.DeliveryDetails.Packaging.Width;
				cmd.Parameters["@Length"].Value = orderInfo.DeliveryDetails.Packaging.Length;
				cmd.Parameters["@Weight"].Value = orderInfo.DeliveryDetails.Packaging.Weight;
				cmd.Parameters["@BoxesNumber"].Value = orderInfo.DeliveryDetails.Packaging.BoxesNumber;
				cmd.Parameters["@CarrierPackagingTypeID"].Value = (orderInfo.DeliveryDetails.Packaging.CarrierPackageTypeId==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.Packaging.CarrierPackageTypeId;
					
				// FIXME: Fill dates with an actual info	
				cmd.Parameters["@ShipDate"].Value = DBNull.Value;
				cmd.Parameters["@ShippedDate"].Value = DBNull.Value;

				cmd.Parameters["@IsDelivered"].Value = orderInfo.DeliveryDetails.IsDelivered;

				//cmd.Parameters["@CustomerDesignDescription"].Value = (orderInfo.Design.CustomerDescription!=null)?orderInfo.Design.CustomerDescription:DBNull.Value;
				if ((orderInfo.Design.CustomerDescription!=null)&&(!orderInfo.Design.CustomerDescription.Equals(String.Empty))) {
					cmd.Parameters["@CustomerDesignDescription"].Value = orderInfo.Design.CustomerDescription;
				} else { cmd.Parameters["@CustomerDesignDescription"].Value = DBNull.Value; }
				cmd.Parameters[BillingFirstName].Value = (orderInfo.BillTo.FirstName == null) ? DBNull.Value : (Object)orderInfo.BillTo.FirstName;
				cmd.Parameters[BillingLastName].Value = (orderInfo.BillTo.LastName == null) ? DBNull.Value : (Object)orderInfo.BillTo.LastName;
				cmd.Parameters[BillingContactEmail].Value = (orderInfo.BillTo.ContactEmail == null) ? DBNull.Value : (Object)orderInfo.BillTo.ContactEmail;
				cmd.Parameters[BillingAddress1].Value = (orderInfo.BillTo.Address.Address1 == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.Address1;
				cmd.Parameters[BillingAddress2].Value = (orderInfo.BillTo.Address.Address2 == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.Address2;
				cmd.Parameters[BillingCountry].Value = (orderInfo.BillTo.Address.Country == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.Country;
				cmd.Parameters[BillingState].Value = (orderInfo.BillTo.Address.State == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.State;
				cmd.Parameters[BillingZipcode].Value = (orderInfo.BillTo.Address.ZipCode == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.ZipCode;
				cmd.Parameters[BillingCity].Value = (orderInfo.BillTo.Address.City == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.City;
				cmd.Parameters[BillingContactPhone].Value = (orderInfo.BillTo.ContactPhone == null) ? DBNull.Value : (Object)orderInfo.BillTo.ContactPhone;
				cmd.Parameters[BillingContactFax].Value = (orderInfo.BillTo.ContactFax == null) ? DBNull.Value : (Object)orderInfo.BillTo.ContactFax;
				cmd.Parameters[BillingCompanyName].Value = (orderInfo.BillTo.CompanyName == null) ? DBNull.Value : (Object)orderInfo.BillTo.CompanyName;

				/*					if (orderInfo.Design.CustomerDescription.Length>0) {
										cmd.Parameters["@CustomerDesignDescription"].Size = 
									}*/
				#endregion

				cmd.ExecuteNonQuery();
				orderInfo.OrderId = (int)cmd.Parameters["@OrderID"].Value;

				switch (orderInfo.OrderJob.JobType) {
					case JobType.PrintingOnly:
						//if (orderInfo.Design.AllPartsUploaded)
						foreach (PartDesign pd in orderInfo.Design.Parts) {
							pd.OrderId = orderInfo.OrderId;
							/*								if (pd.IsEmpty) {
																continue;
															}*/
							if (!AddPartDesign(pd,cmd.Transaction)) {
								if (txnNull) {transaction.Rollback();}
								return false;
							}

							if (pd.IsEmpty) { continue; }

							// FIXME: remove calling DirectoryManger after temporary files will be stored to the SQL server db
							if (!UpdatePartDesignFile(pd,DirectoryManager.GetCustomerUploadDestinationAbsolutePath(orderInfo.CustomerID),cmd.Transaction)) {
								if (txnNull) {transaction.Rollback();}
								return false;
							}
						}
						break;
					case JobType.DesignAndPrinting:
						int auxFileId;
						foreach (ArrayList partFiles in orderInfo.Design.AuxFiles.Values) {
								
							foreach(AuxFile file in partFiles) {
								file.OrderId = orderInfo.OrderId;
								// TODO: add file to the db
								if (!AddAuxFile(file, DirectoryManager.GetCustomerUploadDestinationAbsolutePath(orderInfo.CustomerID),cmd.Transaction, out auxFileId)) {
									if (txnNull) {transaction.Rollback();}
									return false;
								} else {
									file.AuxFileId = auxFileId;
								}
							}
						}
						break;
				}
				// here goes adding extras info
				/*if (!AddExtras(orderInfo.OrderId,orderInfo.OrderJob.Extras, cmd.Transaction)) {
					if (txnNull) {transaction.Rollback();}
					return false;
				}
				*/
				if (!UpdateSelectedExtras(orderInfo.OrderId,orderInfo.OrderJob.Extras, cmd.Transaction)) {
					if (txnNull) {transaction.Rollback();}
					return false;
				}
				
				if (orderInfo.OrderPromoCodes.Codes.Count>0) {
					if (!AddCodesUsage(orderInfo,cmd.Transaction)) {
						if (txnNull) {transaction.Rollback();}
						return false;
					}
				}
				// if IDbTransactionParameter was not passed then commit transaction, else let it to be made by caller method
				if (txnNull) {
					transaction.Commit();
				}
				return true;
			} catch (Exception ex) {
				if (txnNull) {
					// if IDbTransactionParameter was not passed then rollback transaction
					// else let it to be made by caller method
					transaction.Rollback();
				}
				return false;
			} finally {
				if (txnNull) {
					if (transaction!=null) {
						transaction.Dispose();
					}
					if (conn != null) conn.Close();
				}
			}


		}
		#region outdated

		/// <summary>
		/// Adds new orders and downpayments to the database.
		/// </summary>
		/// <remarks>it's used by shopping cart related business logic</remarks>
		/// <param name="orders">Orders to add. Orders should also include downpayment info</param>
		/// <returns>true - if operation is successfully accompished</returns>
		[Obsolete("Don't use AddNewOrders",true)]
		public bool AddNewOrders(ref ArrayList  orders) {
			// DESIGNTIME: check whether downpayment info is present
			foreach(OrderInfo orderInfo in orders) {
				if (orderInfo.Payments.Count!=1) {
					throw new ArgumentException("orderInfo should contain payment info");
				}
			}
			SqlCommand cmd = GetAddNewOrder();

			SqlConnection conn = null;
			SqlTransaction transaction;
			try {

				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = transaction;
//				paymentCmd.Transaction = transaction;

				foreach(OrderInfo orderInfo in orders) {
					#region Params setting
					cmd.Parameters["@Status"].Value = orderInfo.Status;
					cmd.Parameters["@CustomerID"].Value = orderInfo.CustomerID;
					cmd.Parameters["@SiteID"].Value = orderInfo.SiteID;
					cmd.Parameters["@CreatedTS"].Value = orderInfo.CreatedTS;

					cmd.Parameters["@OrderJobType"].Value = orderInfo.OrderJob.JobType;
					cmd.Parameters["@PrintingTypeID"].Value = orderInfo.OrderJob.PrintingTypeID;
					cmd.Parameters["@Quantity"].Value = orderInfo.OrderJob.Quantity;
					cmd.Parameters["@PaperSizeID"].Value = orderInfo.OrderJob.PaperSizeID;
					cmd.Parameters["@PaperTypeID"].Value = orderInfo.OrderJob.PaperTypeID;
					//cmd.Parameters["@HasExtras"].Value = orderInfo.OrderJob.HasExtras;
					cmd.Parameters[JobName].Value = ((orderInfo.OrderJob.JobName==null)||(orderInfo.OrderJob.JobName==String.Empty))?DBNull.Value:(object)orderInfo.OrderJob.JobName;

					cmd.Parameters["@AmountCalculated"].Value = orderInfo.Amounts.CalculatedAmount;
					cmd.Parameters["@AmountExtras"].Value = orderInfo.Amounts.ExtrasAmount;
					cmd.Parameters["@AmountShipping"].Value = orderInfo.Amounts.ShippingAmount;
					cmd.Parameters["@AmountDiscount"].Value = orderInfo.Amounts.DiscountAmount;
					cmd.Parameters["@AmountTax"].Value = orderInfo.Amounts.TaxAmount;
					cmd.Parameters["@AmountTotal"].Value = orderInfo.Amounts.TotalAmount;
					cmd.Parameters["@RequiredDownpayment"].Value = orderInfo.DownpaymentRequired;
			
					cmd.Parameters["@PickupOrder"].Value = orderInfo.DeliveryDetails.PickUpOrder;
			
					cmd.Parameters["@Country"].Value = ((orderInfo.DeliveryDetails.ShipAddress.Country==null)||(orderInfo.DeliveryDetails.ShipAddress.Country==String.Empty))?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.Country;
					cmd.Parameters["@State"].Value = (orderInfo.DeliveryDetails.ShipAddress.State==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.State;
					cmd.Parameters["@City"].Value = (orderInfo.DeliveryDetails.ShipAddress.City==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.City;
					cmd.Parameters["@zipcode"].Value = (orderInfo.DeliveryDetails.ShipAddress.ZipCode==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.ZipCode;
					cmd.Parameters["@Address1"].Value = (orderInfo.DeliveryDetails.ShipAddress.Address1==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.Address1;
					cmd.Parameters["@Address2"].Value = (orderInfo.DeliveryDetails.ShipAddress.Address2==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipAddress.Address2;

					cmd.Parameters["@SpecialInstructions"].Value = (orderInfo.DeliveryDetails.SpecialInstructions==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.SpecialInstructions;
					cmd.Parameters["@CarrierID"].Value = (int)orderInfo.DeliveryDetails.Carrier;
					cmd.Parameters["@ShippingMethodID"].Value = (orderInfo.DeliveryDetails.ShipMethod==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.ShipMethod;

					cmd.Parameters["@Height"].Value = orderInfo.DeliveryDetails.Packaging.Height;
					cmd.Parameters["@Width"].Value = orderInfo.DeliveryDetails.Packaging.Width;
					cmd.Parameters["@Length"].Value = orderInfo.DeliveryDetails.Packaging.Length;
					cmd.Parameters["@Weight"].Value = orderInfo.DeliveryDetails.Packaging.Weight;
					cmd.Parameters["@BoxesNumber"].Value = orderInfo.DeliveryDetails.Packaging.BoxesNumber;
					cmd.Parameters["@CarrierPackagingTypeID"].Value = (orderInfo.DeliveryDetails.Packaging.CarrierPackageTypeId==null)?DBNull.Value:(object)orderInfo.DeliveryDetails.Packaging.CarrierPackageTypeId;
					
					// FIXME: Fill dates with an actual info	
					cmd.Parameters["@ShipDate"].Value = DBNull.Value;
					cmd.Parameters["@ShippedDate"].Value = DBNull.Value;

					cmd.Parameters["@IsDelivered"].Value = orderInfo.DeliveryDetails.IsDelivered;

					//cmd.Parameters["@CustomerDesignDescription"].Value = (orderInfo.Design.CustomerDescription!=null)?orderInfo.Design.CustomerDescription:DBNull.Value;
					if ((orderInfo.Design.CustomerDescription!=null)&&(!orderInfo.Design.CustomerDescription.Equals(String.Empty))) {
						cmd.Parameters["@CustomerDesignDescription"].Value = orderInfo.Design.CustomerDescription;
					} else { cmd.Parameters["@CustomerDesignDescription"].Value = DBNull.Value; }
					cmd.Parameters[BillingFirstName].Value = (orderInfo.BillTo.FirstName == null) ? DBNull.Value : (Object)orderInfo.BillTo.FirstName;
					cmd.Parameters[BillingLastName].Value = (orderInfo.BillTo.LastName == null) ? DBNull.Value : (Object)orderInfo.BillTo.LastName;
					cmd.Parameters[BillingContactEmail].Value = (orderInfo.BillTo.ContactEmail == null) ? DBNull.Value : (Object)orderInfo.BillTo.ContactEmail;
					cmd.Parameters[BillingAddress1].Value = (orderInfo.BillTo.Address.Address1 == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.Address1;
					cmd.Parameters[BillingAddress2].Value = (orderInfo.BillTo.Address.Address2 == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.Address2;
					cmd.Parameters[BillingCountry].Value = (orderInfo.BillTo.Address.Country == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.Country;
					cmd.Parameters[BillingState].Value = (orderInfo.BillTo.Address.State == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.State;
					cmd.Parameters[BillingZipcode].Value = (orderInfo.BillTo.Address.ZipCode == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.ZipCode;
					cmd.Parameters[BillingCity].Value = (orderInfo.BillTo.Address.City == null) ? DBNull.Value : (Object)orderInfo.BillTo.Address.City;
					cmd.Parameters[BillingContactPhone].Value = (orderInfo.BillTo.ContactPhone == null) ? DBNull.Value : (Object)orderInfo.BillTo.ContactPhone;
					cmd.Parameters[BillingContactFax].Value = (orderInfo.BillTo.ContactFax == null) ? DBNull.Value : (Object)orderInfo.BillTo.ContactFax;
					cmd.Parameters[BillingCompanyName].Value = (orderInfo.BillTo.CompanyName == null) ? DBNull.Value : (Object)orderInfo.BillTo.CompanyName;


					#endregion
						
					cmd.ExecuteNonQuery();
					orderInfo.OrderId = (int)cmd.Parameters["@OrderID"].Value;

					int paymentId = PersistentBusinessEntity.ID_EMPTY;
					((PaymentInfo)orderInfo.Payments[0]).OrderId = orderInfo.OrderId;
					if (!AddPayment((PaymentInfo)orderInfo.Payments[0],conn,transaction,out paymentId)) {
						transaction.Rollback();
						return false;
					} else {
						((PaymentInfo)orderInfo.Payments[0]).PaymentId = paymentId;
					}
					// here goes adding design files info
					switch (orderInfo.OrderJob.JobType) {
						case JobType.PrintingOnly:
							//if (orderInfo.Design.AllPartsUploaded)
							foreach (PartDesign pd in orderInfo.Design.Parts) {
								pd.OrderId = orderInfo.OrderId;

								if (!AddPartDesign(pd,transaction)) {
									transaction.Rollback();
									return false;
								}

								if (pd.IsEmpty) { continue; }

								// FIXME: remove calling DirectoryManger after temporary files will be stored to the SQL server db
								if (!UpdatePartDesignFile(pd,DirectoryManager.GetCustomerUploadDestinationAbsolutePath(orderInfo.CustomerID),transaction)) {
									transaction.Rollback();
									return false;
								}
							}
							break;
						case JobType.DesignAndPrinting:
							int auxFileId;
							foreach (ArrayList partFiles in orderInfo.Design.AuxFiles.Values) {
								
								foreach(AuxFile file in partFiles) {
									file.OrderId = orderInfo.OrderId;
									// TODO: add file to the db
									if (!AddAuxFile(file, DirectoryManager.GetCustomerUploadDestinationAbsolutePath(orderInfo.CustomerID),transaction, out auxFileId)) {
										transaction.Rollback();
										return false;
									} else {
										file.AuxFileId = auxFileId;
									}
								}
							}
							break;
					}
					// here goes adding extras info
					if (!AddExtras(orderInfo.OrderId,orderInfo.OrderJob.Extras, transaction)) {
						transaction.Rollback();
						return false;
					}
					
					if (orderInfo.OrderPromoCodes.Codes.Count>0) {
						if (!AddCodesUsage(orderInfo,transaction)) {
							transaction.Rollback();
							return false;
						}
					}

				} //foreach
				//cmd.Transaction.Commit();
				transaction.Commit();
			} catch (Exception ex) {
				//TODO: consider logging
				cmd.Transaction.Rollback();
				return false;
			} finally {
				if (cmd.Transaction!=null) {
					cmd.Transaction.Dispose();
				}
				if (conn != null) conn.Close();
			}

			return true;
			
		
		}
		
		#endregion
		/// <summary>
		/// This routine is used to partial order info update depnding on the <code>partsToUpdate</code> parameter.
		/// </summary>
		/// <param name="orderInfo">Order info</param>
		/// <param name="partsToUpdate">specify exact order info parts to update</param>
		/// <param name="filesDir">Can be null. required only for ypdating part design and aux files</param>
		/// <returns></returns>
		public bool UpdateOrderInfo( OrderInfo orderInfo, OrderInfoParts partsToUpdate, string filesDir) {
			if (partsToUpdate == OrderInfoParts.None) { return true; }
			if (((partsToUpdate&OrderInfoParts.DesignParts)>0)||((partsToUpdate&OrderInfoParts.AuxFiles)>0)) {
				if ((filesDir==null)||(filesDir == String.Empty)) {
					return false;
				}
			}

			SqlConnection conn = null;
			SqlTransaction transaction;

			conn = new SqlConnection(AppConfig.dbConnString);
			conn.Open();

			transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);

			try {

				if ((partsToUpdate&OrderInfoParts.MainInfo)>0) {
					// TODO: main order info updating goes here
				}

				if ((partsToUpdate&OrderInfoParts.DeliveryDetails)>0) {
					if (!UpdateOrderDeliveryDetails(orderInfo.OrderId, orderInfo.DeliveryDetails, transaction)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.Amounts)>0) {
					if (!UpdateOrderAmounts(orderInfo,transaction)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.DesignParts)>0) {
					if (!UpdatePartsDesign(orderInfo.Design.Parts,filesDir)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.DesignPreview)>0) {
					if (!UpdatePartsDesign(orderInfo.Design.PartPreviews,filesDir)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.AuxFiles)>0) {
					throw new NotImplementedException();
					// TODO: Implement this
				}

				if ((partsToUpdate&OrderInfoParts.OrderStatus)>0) {
					if (!UpdateOrderStatus(orderInfo.OrderId,orderInfo.Status,transaction)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.Payments)>0) {
					// TODO: Implement this
					throw new NotImplementedException();
				}
				if ((partsToUpdate&OrderInfoParts.SelectedExtras)>0) {
					if (!this.UpdateSelectedExtras(orderInfo.OrderId,orderInfo.OrderJob.Extras,transaction)) {
						transaction.Rollback();
						return false;
					}

				}
				if ((partsToUpdate&OrderInfoParts.PDInfo)>0) {
					if (!this.UpdatePDInfo(orderInfo,transaction)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.DDInfo)>0) {
					if (!this.UpdateDDInfo(orderInfo,transaction)) {
						transaction.Rollback();
						return false;
					}
				}
				if ((partsToUpdate&OrderInfoParts.MailingInfo)>0) {
					if (!this.UpdateMailingInfo(orderInfo,transaction)) {
						transaction.Rollback();
						return false;
					}
				}				
				transaction.Commit();
				return true;
			} catch (Exception ex) {
				if (transaction!=null)
					transaction.Rollback();
				return false;
			} finally {
				if (transaction !=null) {
					transaction.Dispose();
				}
				if (conn != null) conn.Close();
			}

		}

		#region UpdateOrderStatus
		public bool UpdateOrderStatus(int orderId, OrderStatus status, SqlTransaction transaction, OrderLog.Entry logEntry) {
			SqlCommand cmd = GetUpdateOrderStatusCommand();
			cmd.Parameters[OrderID].Value = orderId;
			cmd.Parameters["@status"].Value = (int)status;

			SqlConnection conn = null;
			bool res = false;
			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = transaction;
				}
				cmd.Connection = conn;

				int rowsAffected = cmd.ExecuteNonQuery();
				res = (rowsAffected > 0);
				if (logEntry!=null) {
					res &= AddOrderLogEntry(logEntry,transaction);
				}
				return res;

			} catch {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
			
		}

		public bool UpdateOrderStatus(int orderId, OrderStatus status) {
			return UpdateOrderStatus(orderId, status, null, null);
		}
		public bool UpdateOrderStatus(int orderId, OrderStatus status, SqlTransaction transaction) {
			return UpdateOrderStatus(orderId, status, transaction, null);
		}		
		public bool UpdateOrderStatus(int orderId, OrderStatus status, OrderLog.Entry logEntry) {
			return UpdateOrderStatus(orderId, status, null, logEntry);
		}
		
		public bool UpdateOrderStatusTxn(int orderId, OrderStatus status, IDbTransaction transaction, OrderLog.Entry logEntry) {
			SqlTransaction sqlTxn = (SqlTransaction)transaction;
			return UpdateOrderStatus(orderId, status, sqlTxn, null);
		}
		#endregion

		public bool UpdateOrderDeliveryDetails(int orderId, DeliveryDetailsInfo deliveryInfo) {
			return UpdateOrderDeliveryDetails(orderId, deliveryInfo, null);
		}

		public bool UpdateOrderDeliveryDetails(int orderId, DeliveryDetailsInfo deliveryInfo, SqlTransaction transaction) {

			SqlCommand cmd = GetUpdateOrderDeliveryDetails();

			#region params
			cmd.Parameters[OrderID].Value = orderId;
			cmd.Parameters["@PickupOrder"].Value = deliveryInfo.PickUpOrder;
			
			cmd.Parameters["@Country"].Value = ((deliveryInfo.ShipAddress.Country==null)||(deliveryInfo.ShipAddress.Country==String.Empty))?DBNull.Value:(object)deliveryInfo.ShipAddress.Country;
			cmd.Parameters["@State"].Value = (deliveryInfo.ShipAddress.State==null)?DBNull.Value:(object)deliveryInfo.ShipAddress.State;
			cmd.Parameters["@City"].Value = (deliveryInfo.ShipAddress.City==null)?DBNull.Value:(object)deliveryInfo.ShipAddress.City;
			cmd.Parameters["@zipcode"].Value = (deliveryInfo.ShipAddress.ZipCode==null)?DBNull.Value:(object)deliveryInfo.ShipAddress.ZipCode;
			cmd.Parameters["@Address1"].Value = (deliveryInfo.ShipAddress.Address1==null)?DBNull.Value:(object)deliveryInfo.ShipAddress.Address1;
			cmd.Parameters["@Address2"].Value = (deliveryInfo.ShipAddress.Address2==null)?DBNull.Value:(object)deliveryInfo.ShipAddress.Address2;

			cmd.Parameters["@SpecialInstructions"].Value = (deliveryInfo.SpecialInstructions==null)?DBNull.Value:(object)deliveryInfo.SpecialInstructions;
			cmd.Parameters["@CarrierID"].Value = (int)deliveryInfo.Carrier;
			//if (deliveryInfo.Carrier in PostalCarrier) {}
			cmd.Parameters["@ShippingMethodID"].Value = (deliveryInfo.ShipMethod==null)?DBNull.Value:(object)deliveryInfo.ShipMethod;

			cmd.Parameters["@Height"].Value = deliveryInfo.Packaging.Height;
			cmd.Parameters["@Width"].Value = deliveryInfo.Packaging.Width;
			cmd.Parameters["@Length"].Value = deliveryInfo.Packaging.Length;
			cmd.Parameters["@Weight"].Value = deliveryInfo.Packaging.Weight;
			cmd.Parameters["@BoxesNumber"].Value = deliveryInfo.Packaging.BoxesNumber;
			cmd.Parameters["@CarrierPackagingTypeID"].Value = (deliveryInfo.Packaging.CarrierPackageTypeId==null)?DBNull.Value:(object)deliveryInfo.Packaging.CarrierPackageTypeId;
			
			// FIXME: Fill dates with an actual info
			
			cmd.Parameters["@ShipDate"].Value = DBNull.Value;
			cmd.Parameters["@ShippedDate"].Value = DBNull.Value;

			cmd.Parameters["@IsDelivered"].Value = deliveryInfo.IsDelivered;
			cmd.Parameters["@ShipTrackingID"].Value = (deliveryInfo.ShipTrackingID==null||deliveryInfo.ShipTrackingID.Equals(String.Empty))?DBNull.Value:(object)deliveryInfo.ShipTrackingID;
			

			#endregion

			SqlConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = transaction;
				}
				cmd.Connection = conn;
				
				int rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}
			} catch(Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
			
		}


		public bool UpdateOrderAmounts(OrderInfo orderInfo) {
			return UpdateOrderAmounts(orderInfo, null);
		}

		public bool UpdateOrderAmounts(OrderInfo orderInfo, SqlTransaction transaction) {

			SqlCommand cmd = GetUpdateOrderAmounts();

			#region params

			cmd.Parameters[OrderID].Value = orderInfo.OrderId;

			cmd.Parameters["@AmountCalculated"].Value = orderInfo.Amounts.CalculatedAmount;
			cmd.Parameters["@AmountExtras"].Value = orderInfo.Amounts.ExtrasAmount;
			cmd.Parameters["@AmountShipping"].Value = orderInfo.Amounts.ShippingAmount;
			cmd.Parameters["@AmountDiscount"].Value = orderInfo.Amounts.DiscountAmount;
			cmd.Parameters["@AmountTax"].Value = orderInfo.Amounts.TaxAmount;
			//cmd.Parameters["@AmountDesign"].Value = orderInfo.Amounts.DesignAmount;
			cmd.Parameters["@DesignHours"].Value = orderInfo.Amounts.DesignHours;
			cmd.Parameters["@DesignCost"].Value = orderInfo.Amounts.DesignCost;
			cmd.Parameters["@IsFixedDesignCost"].Value = orderInfo.Amounts.IsDesignCostFixed;
			cmd.Parameters["@AmountImages"].Value = orderInfo.Amounts.ImagesAmount;
			cmd.Parameters["@AmountScanning"].Value = orderInfo.Amounts.ScanAmount;
			
			cmd.Parameters["@AmountProof"].Value = orderInfo.Amounts.ProofAmount;
			cmd.Parameters["@PostageCostPerPiece"].Value = orderInfo.Amounts.PostageCostPerPiece;
			cmd.Parameters["@PostageQuantity"].Value = orderInfo.Amounts.PostageQuantity;
			cmd.Parameters["@MailingPrcCostPerM"].Value = orderInfo.Amounts.MailingPrcCostPerM;
			cmd.Parameters["@MailingPrcQuantity"].Value = orderInfo.Amounts.MailingPrcQuantity;
			cmd.Parameters["@DiskPrepCost"].Value = orderInfo.Amounts.DiskPrepCost;

			cmd.Parameters["@AmountTotal"].Value = orderInfo.Amounts.TotalAmount;
//			cmd.Parameters["@RequiredDownpayment"].Value = orderInfo.DownpaymentRequired;
	/*@PostageCostPerPiece	smallmoney,
	@PostageQuantity	int,
	@MailingPrcCostPerM	smallmoney,
	@MailingPrcQuantity	int,
	@DiskPrepCost	smallmoney,*/
			#endregion

			SqlConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = transaction.Connection;
					cmd.Transaction = transaction;
				}
				cmd.Connection = conn;
				
				int rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}
			} catch (Exception ex) {
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
			
		}

		[Obsolete]
		public bool UpdatePDInfo(int orderID, int upNo, int psNo) {
			SqlCommand cmd = GetUpdatePDInfoCommand();
			cmd.Parameters[OrderID].Value = orderID;
			cmd.Parameters[UPNo].Value    = upNo;
			cmd.Parameters[PSNo].Value    = psNo;
			
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch  {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		//GetUpdateOrderMailingInfo()
		public bool UpdateMailingInfo(OrderInfo orderInfo,SqlTransaction transaction) {
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			SqlCommand cmd = GetUpdateOrderMailingInfo();
			//bool res = true;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
					
				}
				cmd.Parameters[OrderID].Value = orderInfo.OrderId;
				cmd.Parameters[PM_PostageType].Value = orderInfo.OrderJob.PostageType;
				cmd.Parameters[PM_MailDue].Value = orderInfo.MailingDue;
			
				int rowsAffected = 0;
				//SqlConnection conn = null;
			
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex) {
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}
			
		}
		
//		#region move to stored proc
//		public bool UpdateMailingInfoTmp(OrderInfo orderInfo,IDbTransaction transaction) {
//			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
//				throw new ArgumentException("'transaction.Connection' should be 'open' ");
//			}
//
//			SqlCommand cmd = GetUpdateOrderMailingInfo();
//			//bool res = true;
//			try {
//				if (transaction==null) {
//					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
//					cmd.Connection.Open();
//				} else {
//					cmd.Connection = (SqlConnection)transaction.Connection;
//					cmd.Transaction = (SqlTransaction)transaction;
//					
//				}
//				cmd.Parameters[OrderID].Value = orderInfo.OrderId;
//				cmd.Parameters[PM_PostageType].Value = orderInfo.OrderJob.PostageType;
//				cmd.Parameters[PM_MailDue].Value = orderInfo.MailingDue;
//			
//				int rowsAffected = 0;
//				//SqlConnection conn = null;
//			
//				rowsAffected = cmd.ExecuteNonQuery();
//				return (rowsAffected > 0);
//			} catch (Exception ex) {
//				return false;
//			} finally {
//				if (transaction==null) {
//					cmd.Connection.Close();
//				}
//			}
//			
//		}
//
//		public bool UpdateOrderAmountsTmp(OrderInfo orderInfo, IDbTransaction transaction) {
//
//			SqlCommand cmd = GetUpdateOrderAmounts();
//
//			#region params
//
//			cmd.Parameters[OrderID].Value = orderInfo.OrderId;
//
//			cmd.Parameters["@AmountCalculated"].Value = orderInfo.Amounts.CalculatedAmount;
//			cmd.Parameters["@AmountExtras"].Value = orderInfo.Amounts.ExtrasAmount;
//			cmd.Parameters["@AmountShipping"].Value = orderInfo.Amounts.ShippingAmount;
//			cmd.Parameters["@AmountDiscount"].Value = orderInfo.Amounts.DiscountAmount;
//			cmd.Parameters["@AmountTax"].Value = orderInfo.Amounts.TaxAmount;
//			//cmd.Parameters["@AmountDesign"].Value = orderInfo.Amounts.DesignAmount;
//			cmd.Parameters["@DesignHours"].Value = orderInfo.Amounts.DesignHours;
//			cmd.Parameters["@DesignCost"].Value = orderInfo.Amounts.DesignCost;
//			cmd.Parameters["@IsFixedDesignCost"].Value = orderInfo.Amounts.IsDesignCostFixed;
//			cmd.Parameters["@AmountImages"].Value = orderInfo.Amounts.ImagesAmount;
//			cmd.Parameters["@AmountScanning"].Value = orderInfo.Amounts.ScanAmount;
//			
//			cmd.Parameters["@AmountProof"].Value = orderInfo.Amounts.ProofAmount;
//			cmd.Parameters["@PostageCostPerPiece"].Value = orderInfo.Amounts.PostageCostPerPiece;
//			cmd.Parameters["@PostageQuantity"].Value = orderInfo.Amounts.PostageQuantity;
//			cmd.Parameters["@MailingPrcCostPerM"].Value = orderInfo.Amounts.MailingPrcCostPerM;
//			cmd.Parameters["@MailingPrcQuantity"].Value = orderInfo.Amounts.MailingPrcQuantity;
//			cmd.Parameters["@DiskPrepCost"].Value = orderInfo.Amounts.DiskPrepCost;
//
//			cmd.Parameters["@AmountTotal"].Value = orderInfo.Amounts.TotalAmount;
//			//			cmd.Parameters["@RequiredDownpayment"].Value = orderInfo.DownpaymentRequired;
//			/*@PostageCostPerPiece	smallmoney,
//			@PostageQuantity	int,
//			@MailingPrcCostPerM	smallmoney,
//			@MailingPrcQuantity	int,
//			@DiskPrepCost	smallmoney,*/
//			#endregion
//
//			SqlConnection conn = null;
//
//			try {
//				if (transaction==null) {
//					conn = new SqlConnection(AppConfig.dbConnString);
//					conn.Open();
//				} else {
//					conn = (SqlConnection)transaction.Connection;
//					cmd.Transaction = (SqlTransaction)transaction;
//				}
//				cmd.Connection = conn;
//				
//				int rowsAffected = cmd.ExecuteNonQuery();
//				if (rowsAffected > 0) {
//					return true;
//				} else {
//					return false;
//				}
//			} catch (Exception ex) {
//				return false;
//			} finally {
//				cmd.Connection = null;
//				if (transaction==null) {
//					if (conn != null) conn.Close();
//				}
//			}
//			
//		}
//		#endregion

		public bool UpdatePDInfo(OrderInfo orderInfo,SqlTransaction transaction) {
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			SqlCommand cmd = GetUpdatePDInfoCommand();
			//bool res = true;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
					
				}
				cmd.Parameters[OrderID].Value = orderInfo.OrderId;
				cmd.Parameters[UPNo].Value    = orderInfo.UPNo;
				cmd.Parameters[PSNo].Value    = orderInfo.PSNo;
				cmd.Parameters["@ProofType"].Value = orderInfo.OrderJob.ProofType;
				cmd.Parameters["@ProofNumber"].Value = orderInfo.OrderJob.ProofNumber;
				
				cmd.Parameters["@PrintingDue"].Value = orderInfo.PrintingDue;
			
				int rowsAffected = 0;
				//SqlConnection conn = null;
			
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex) {
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}
			
		}

		public bool UpdateDDInfo(OrderInfo orderInfo,SqlTransaction transaction) {
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			SqlCommand cmd = GetUpdateDDInfoCommand();
			//bool res = true;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
					
				}
		/*				@ProofType smallint,
		@ProofNumber int,
		@ImageSource smallint,
		@ImagesNumber int,
		@ScanType smallint,
		@ScanNumber int,*/
				cmd.Parameters[OrderID].Value = orderInfo.OrderId;

				cmd.Parameters["@ProofType"].Value		= orderInfo.OrderJob.ProofType;
				cmd.Parameters["@ProofNumber"].Value	= orderInfo.OrderJob.ProofNumber;
				cmd.Parameters["@ImageSource"].Value	= orderInfo.OrderJob.ImageSourceId;
				cmd.Parameters["@ImagesNumber"].Value	= orderInfo.OrderJob.ImagesNumber;
				cmd.Parameters["@ScanType"].Value	= orderInfo.OrderJob.ScanType;
				cmd.Parameters["@ScanNumber"].Value	= orderInfo.OrderJob.ScanNumber;

				if (orderInfo.Designer!=null) {
					cmd.Parameters["@Designer"].Value	= orderInfo.Designer;
				} else { cmd.Parameters["@Designer"].Value	= DBNull.Value; }
				
				/*cmd.Parameters["@DesignHours"].Value = orderInfo.Amounts.DesignHours;
				cmd.Parameters["@DesignCost"].Value = orderInfo.Amounts.DesignCost;
				cmd.Parameters["@IsFixedDesignCost"].Value = orderInfo.Amounts.IsDesignCostFixed;*/
				cmd.Parameters["@DesignDue"].Value = orderInfo.DesignDue;

				int rowsAffected = 0;
				//SqlConnection conn = null;
			
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex)  {
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}
			
		}
		#region payments
		public PaymentInfo[] GetPayments(FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadPaymentsCmd();
			cmd.Parameters["@filterClause"].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters["@orderClause"].Value = orderBy == null ? "" : orderBy.ToString();
			PaymentInfo[] payments = null;
			SqlConnection conn = null;
			ArrayList array = new ArrayList();
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					PaymentInfo paymentInfo = new PaymentInfo();
					paymentInfo.PaymentId = (int)reader["PaymentId"];
					if (reader["TxnReferenceId"] != DBNull.Value)
						paymentInfo.TxnReferenceId = (String)reader["TxnReferenceId"];
					if (reader["PaymentDate"] != DBNull.Value)
						paymentInfo.PaymentDate = (DateTime)reader["PaymentDate"];
					if (reader["Amount"] != DBNull.Value)
						paymentInfo.Amount = (decimal)reader["Amount"];
					paymentInfo.OrderId = (int)reader["OrderId"];
					array.Add(paymentInfo);
				}
				reader.Close();
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			if (array.Count != 0) {
				payments = new PaymentInfo[array.Count];
				array.CopyTo(payments);
			}
			return payments;
		}

		public int AddPayment(PaymentInfo payment) {
			SqlCommand cmd = GetAddPaymentCmd();
			cmd.Parameters["@txnReferenceID"].Value = (payment.TxnReferenceId == null) ? DBNull.Value : (Object)payment.TxnReferenceId ;
			cmd.Parameters["@paymentDate"].Value = payment.PaymentDate;
			cmd.Parameters["@amount"].Value = (decimal)payment.Amount;
			cmd.Parameters["@orderID"].Value = payment.OrderId;

			SqlConnection conn = null;
			int rowsAffected = 0;
			int id = PersistentBusinessEntity.ID_EMPTY;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0) {
					id = (int)cmd.Parameters["@paymentId"].Value;
				}
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return id;
		}

		/// <summary>
		/// Adds payment. Use this method in transaction context. Caller is responsible for opening, closing and disposing connection and transaction objects.
		/// </summary>
		/// <param name="payment"></param>
		/// <param name="conn"></param>
		/// <param name="transaction"></param>
		/// <param name="paymentId"></param>
		/// <returns></returns>
		public bool AddPayment(PaymentInfo payment, SqlConnection conn, SqlTransaction transaction, out int paymentId) {

			if (conn==null) {
				throw new ArgumentNullException("'conn' param not found","conn");
			}
			if (transaction==null) {
				throw new ArgumentNullException("'transaction' param not found", "transaction");
			}
			
			SqlCommand cmd = GetAddPaymentCmd();
			cmd.Parameters["@txnReferenceID"].Value = (payment.TxnReferenceId == null) ? DBNull.Value : (Object)payment.TxnReferenceId ;
			cmd.Parameters["@paymentDate"].Value = payment.PaymentDate;
			cmd.Parameters["@amount"].Value = payment.Amount;
			cmd.Parameters["@orderID"].Value = payment.OrderId;

			//SqlConnection conn = null;
			int rowsAffected = 0;
			paymentId = PersistentBusinessEntity.ID_EMPTY;
			try {
				//conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				cmd.Transaction = transaction;
				//if (conn.State = ConnectionState.Closed) {conn.Open();}
				rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0) {
					paymentId = (int)cmd.Parameters["@paymentId"].Value;
					return true;
				} else { return false; }
			} catch {
				return false;
			} finally {
				cmd.Connection = null;
				cmd.Transaction = null;
				//if (conn != null) conn.Close();
			}

		}

		public bool RemovePayment(int paymentId) {
			SqlCommand cmd = GetRemovePaymentCmd();
			cmd.Parameters["@paymentId"].Value = paymentId;
			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		#endregion

		#region PartDesignFiles

		public PartDesign[] LoadPartsDesigns(int orderId,PartDesignFileCategory fileCategory) {
			SqlCommand cmd = GetLoadPartDesignsCommand();
			cmd.Parameters[OrderID].Value = orderId;
			cmd.Parameters[FileCategory].Value = fileCategory;
			SqlConnection conn = null;
			ArrayList list = new ArrayList();
			ArrayList previewList = new ArrayList();
			PartDesign[] jds = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				PartDesign item;
				while(reader.Read()) {
					item = new PartDesign();
					item.OrderId = orderId;
					item.PartId = (int)reader["PartID"];
					//fileCategory = (PartDesignFileCategory)reader["FileCategory"];
					
					item.FileCategory = (PartDesignFileCategory)reader["FileCategory"];
					if (reader["FileName"] != DBNull.Value)
						item.FileName = (String)reader["FileName"];
					if (reader["SpecialInstructions"] != DBNull.Value)
						item.SpecialInstructions = (String)reader["SpecialInstructions"];
					item.IsFilePersisted = (int)reader["PicSize"]>0;
					/*
					item.Part = new PrintingTypePart();
					item.Part.PartId = item.PartId;
					if (reader["PartName"] != DBNull.Value)
						item.Part.PartName = (String)reader["PartName"];
					if (reader["Description"] != DBNull.Value)
						item.Part.Description = (String)reader["Description"];
					*/

					list.Add(item);
				}
				reader.Close();
			} catch (Exception ex){
				//FIXME: logging
				throw;
			} finally {
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
			if (list.Count > 0) {
				jds = new PartDesign[list.Count];
				list.CopyTo(jds);
			}
			return jds;
		}

		public bool AddPartDesign(PartDesign pd) {
			return AddPartDesign(pd, null);
		}
		
		public bool AddPartDesign(PartDesign pd, SqlTransaction transaction) {

			if (pd==null) {
				throw new ArgumentNullException("'pd' param not found","pd");
			}


//			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
			if ((transaction!=null)&&(transaction.Connection == null)) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}
			
			SqlCommand cmd = GetAddPartDesignCommand();
			cmd.Parameters[OrderID].Value = pd.OrderId;
			cmd.Parameters[PartId].Value = pd.PartId;
			cmd.Parameters[FileCategory].Value = pd.FileCategory;

			cmd.Parameters["@fileName"].Value = ((pd.FileName==null)||(pd.FileName==String.Empty))? DBNull.Value : (Object)pd.FileName;
			cmd.Parameters["@specialInstructions"].Value = (pd.SpecialInstructions==null) ? DBNull.Value : (Object)pd.SpecialInstructions;

			int rowsAffected = 0;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
				}
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch(Exception ex) {
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
				cmd.Transaction = null;
			}
		}

		public bool UpdatePartDesign(PartDesign pd, SqlTransaction transaction) {
			if (pd==null) {
				throw new ArgumentNullException("'pd' param not found","pd");
			}
			if ((transaction!=null)&&(transaction.Connection == null)) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			SqlCommand cmd = GetUpdatePartDesignCommand();
			cmd.Parameters[OrderID].Value = pd.OrderId;
			cmd.Parameters[PartId].Value = pd.PartId;
			cmd.Parameters[FileCategory].Value = pd.FileCategory;
			cmd.Parameters["@fileName"].Value = ((pd.FileName==null)||(pd.FileName==String.Empty))? DBNull.Value : (Object)pd.FileName;
			cmd.Parameters["@specialInstructions"].Value = (pd.SpecialInstructions==null) ? DBNull.Value : (Object)pd.SpecialInstructions;

			int rowsAffected = 0;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
				}
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex){
				AppLog.LogError("Error during updating part design info",ex);
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
				cmd.Transaction = null;
			}

		}

		public bool UpdatePartDesign(PartDesign pd) {
			return UpdatePartDesign(pd,null);
		}
/*		
		public bool UpdatePartsDesign(PartDesign[] parts,string fileDir) {
			return UpdatePartsDesign(parts, fileDir, null);
		}
*/
		public bool UpdatePartsDesign(PartDesign[] parts,string fileDir/*, SqlTransaction upperLevelTrans*/) {
			SqlConnection conn = null;
			SqlTransaction transaction = null;
			bool res = true;
			try {

				conn = new SqlConnection(AppConfig.dbConnString);
				conn.Open();
				transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
				
				for (int j=0; j< parts.Length;j++) {
					if (!parts[j].IsModified) continue;
					if (!UpdatePartDesign(parts[j],transaction)) {
						transaction.Rollback();
						return false;
					}
					if (parts[j].IsEmpty) { continue; }

					if(!UpdatePartDesignFile(parts[j],fileDir,transaction)) {
						transaction.Rollback();
						return false;
					}
					
				}
				/*foreach (PartDesign pd in parts) {
					if (!pd.IsModified) continue;
					if (!UpdatePartDesign(pd,transaction)) {
						transaction.Rollback();
						return false;
					}
					if(!UpdatePartDesignFile(pd,fileDir,transaction)) {
						transaction.Rollback();
						return false;
					}
				}*/
				transaction.Commit();
				return true;
			} catch (Exception ex) {
				if (transaction!=null) { transaction.Rollback(); }
				return false;
			} finally {
				if (transaction!=null) {
					transaction.Dispose();
				}
				if (conn != null) conn.Close();

			}
		}
		
		public bool RemovePartDesign(int orderId, int partId) {
			SqlCommand cmd = GetRemovePartDesignCommand();
			cmd.Parameters[OrderID].Value = orderId;
			cmd.Parameters[PartId].Value = partId;
			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
			return (rowsAffected > 0);
		}
		
		public bool UpdatePartDesignFile(PartDesign pd, string fileDir) {
			return UpdatePartDesignFile(pd, fileDir, null);
		}
		
		public bool UpdatePartDesignFile(PartDesign pd, string fileDir, SqlTransaction transaction) {
			// params checkng
			if (!Path.IsPathRooted(fileDir)) {
				throw new ArgumentException("fileDir should contain a rooted path to the directory that contain design file","fileDir");
			}
			if (pd==null) {
				throw new ArgumentNullException("'pd' param not found","pd");
			}

			if (pd.IsEmpty) {
				return true; // nothing to update
				//throw new ArgumentOutOfRangeException("pd","part design shouldn't be empty");
			}
			if ((transaction!=null)&&(transaction.Connection == null)) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			string filePath = Path.Combine(fileDir,pd.FileName);
			
			FileStream fs = new FileStream(filePath,FileMode.Open,FileAccess.Read);
			byte[] fileData = new Byte[fs.Length];
			fs.Read(fileData,0,(int)fs.Length);
			fs.Close();

			SqlCommand cmd = GetUpdatePartDesignFile();
			cmd.Parameters[OrderID].Value = pd.OrderId;
			cmd.Parameters[PartId].Value = pd.PartId;
			cmd.Parameters[FileCategory].Value = pd.FileCategory;
			cmd.Parameters["@blobdata"].Value = fileData;

			int rowsAffected = 0;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
				}
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex) {
				AppLog.LogError("Can't store design file to db",ex);
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
				cmd.Transaction = null;
			}

			
		}

		
		public /*MemoryStream*/byte[] GetPartDesignFile(int orderId, int partId, PartDesignFileCategory fileCategory ) {
			SqlCommand cmd = GetPartDesignFileGetterCmd();
			SqlConnection conn = null;
			try {
				
				cmd.Parameters[OrderID].Value = orderId;
				cmd.Parameters[PartId].Value = partId;
				cmd.Parameters[FileCategory].Value = (int)fileCategory;

				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				cmd.Connection.Open();
				byte[] blob = (byte[])cmd.ExecuteScalar();
//				object blob = cmd.ExecuteScalar();
//				return null;
				return blob;
/*				MemoryStream stream = new MemoryStream(blob);
				return stream;*/
			} catch (Exception ex) {
				//TODO: Consider logging
				return /*(MemoryStream)MemoryStream.Null*/null;
			} finally {
				if (conn != null) conn.Close();
				cmd.Connection = null;
			}
			
		}
		#endregion

		#region AuxFiles

		public AuxFilesContainer GetAuxFiles( int orderId ) {
			AuxFilesContainer files = new AuxFilesContainer();
			ArrayList res = new ArrayList();
			SqlCommand cmd = GetLoadAuxFilesCmd();
			cmd.Parameters[OrderID].Value = orderId;

			SqlConnection conn = null;

			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				AuxFile file;
				while(reader.Read()) {
					file = new AuxFile();
					file.AuxFileId	= (int)reader["AuxFileId"];
					file.OrderId	= (int)reader["OrderID"];
					file.PartId		= (reader["PartID"]==DBNull.Value)?-1:(int)reader["PartID"];//(int)reader["PartID"];
					file.FileName	= (string)reader["FileName"];
					file.FileType	= (AuxFileType)reader["FileType"];
					file.Description= (string)reader["Description"];
					file.FileContentType = (string)reader["FileContentType"];
					//res.Add(file);
					files.AddAuxFile(file);
				}
				reader.Close();
				//cmd.Fill(data);
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return files;
		}


/*
		public AuxFile[] GetAuxFiles( int orderId ) {
			ArrayList res = new ArrayList();
			SqlCommand cmd = GetLoadAuxFilesCmd();
			cmd.Parameters[OrderID].Value = orderId;

			SqlConnection conn = null;
			
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				AuxFile file;
				while(reader.Read()) {
					file = new AuxFile();
					file.AuxFileId	= (int)reader["AuxFileId"];
					file.OrderId	= (int)reader["OrderID"];
					file.PartId		= (int)reader["PartID"];
					file.FileName	= (string)reader["FileName"];
					file.FileType	= (AuxFileType)reader["FileType"];
					file.Description= (string)reader["Description"];
					res.Add(file);
				}
				reader.Close();
				//cmd.Fill(data);
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}			
			if (res.Count>0) {
				AuxFile[] files = new AuxFile[res.Count];
				res.CopyTo(files);
				return files;
			} else { return null; }
		}
*/	
	
/*
		public bool UpdateAuxFileBLOB(AuxFile auxFile, string fileDir) {
			return UpdateAuxFileBLOB(auxFile, fileDir, null);
		}
		public bool UpdateAuxFileBLOB(AuxFile auxFile, string fileDir, SqlTransaction transaction) {
			// params checkng
			if (!Path.IsPathRooted(fileDir)) {
				throw new ArgumentException("fileDir should contain a rooted path to the directory that contain design file","fileDir");
			}
			if (auxFile==null) {
				throw new ArgumentNullException("'pd' param not found","pd");
			}

			if (auxFile.IsEmpty) {
				throw new ArgumentOutOfRangeException("auxFile","auxFile name shouldn't be empty");
			}
			if ((transaction!=null)&&(transaction.Connection == null)) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			string filePath = Path.Combine(fileDir,auxFile.FileName);
			
			FileStream fs = new FileStream(filePath,FileMode.Open,FileAccess.Read);
			byte[] fileData = new Byte[fs.Length];
			fs.Read(fileData,0,(int)fs.Length);
			fs.Close();

			SqlCommand cmd = GetUpdatePartDesignFile();
			cmd.Parameters[OrderID].Value = pd.OrderId;
			cmd.Parameters[PartId].Value = pd.PartId;
			cmd.Parameters[FileCategory].Value = pd.FileCategory;
			cmd.Parameters["@blobdata"].Value = fileData;

			int rowsAffected = 0;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
				}
				rowsAffected = cmd.ExecuteNonQuery();
				return (rowsAffected > 0);
			} catch (Exception ex) {
				AppLog.LogError("Can't store design file to db",ex);
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
				cmd.Transaction = null;
			}


			throw new NotImplementedException();
		}
*/
		/// <summary>
		/// Adds aux file (file that contain descriptions, scetches, logos etc)
		/// </summary>
		/// <param name="auxFile">The id of newly added auxfile</param>
		/// <returns></returns>
		public bool AddAuxFile(AuxFile auxFile, string fileDir, out int auxFileId) {
			return AddAuxFile(auxFile, fileDir, null, out auxFileId);
		}

		public bool AddAuxFile(AuxFile auxFile,string fileDir, SqlTransaction transaction, out int auxFileId) {
			
			auxFileId = PersistentBusinessEntity.ID_EMPTY;

			if (!Path.IsPathRooted(fileDir)) {
				throw new ArgumentException("fileDir should contain a rooted path to the directory that contain design file","fileDir");
			}

			if (auxFile==null) {
				throw new ArgumentNullException("auxFile","'auxFile' param not found");
			}

			if (auxFile.IsEmpty) {
				throw new ArgumentOutOfRangeException("auxFile","auxFile name shouldn't be empty");
			}

			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}
			
			byte[] fileData = null; 
			FileStream fs = null;
			try {
				string filePath = Path.Combine(fileDir,auxFile.FileName);
				// TODO: add try-catch block here
				fs = new FileStream(filePath,FileMode.Open,FileAccess.Read);
				fileData = new Byte[fs.Length];
				fs.Read(fileData,0,(int)fs.Length);
			} catch {
				return false;
			} finally {
				fs.Close();
			}
			
			SqlCommand cmd = GetAddAuxFileCommand();
			cmd.Parameters[OrderID].Value = auxFile.OrderId;
			//cmd.Parameters[PartId].Value = (auxFile.PartId==PersistentBusinessEntity.ID_EMPTY)?DBNull.Value:auxFile.PartId;
/*			if (auxFile.PartId!=PersistentBusinessEntity.ID_EMPTY) {
				cmd.Parameters[PartId].Value = auxFile.PartId;
			} else { cmd.Parameters[PartId].Value = DBNull.Value; }*/// SIC! commented to allow files that are not related to any part
			if (auxFile.PartId==PersistentBusinessEntity.ID_EMPTY) {
				cmd.Parameters[PartId].Value = DBNull.Value;
			} else { cmd.Parameters[PartId].Value = auxFile.PartId; }
			
			cmd.Parameters["@fileName"].Value = auxFile.FileName;
			cmd.Parameters["@fileType"].Value = auxFile.FileType;
			cmd.Parameters["@fileContentType"].Value = (auxFile.FileContentType==null?DBNull.Value:(object)auxFile.FileContentType);
			cmd.Parameters["@description"].Value = (auxFile.Description==null)?DBNull.Value:(object)auxFile.Description;
			cmd.Parameters["@blobdata"].Value = fileData;

			int rowsAffected = 0;
			
			try {
				//conn = new SqlConnection(AppConfig.dbConnString);
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
				}
				rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0) {
					auxFileId = (int)cmd.Parameters["@auxFileId"].Value;
				}
				return true;
			} catch (Exception ex) {
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}
		}


		public bool RemoveAuxFile(int orderId, int auxFileId) {
			SqlCommand cmd = GetRemoveAuxFileCommand();
			cmd.Parameters[OrderID].Value = orderId;
			cmd.Parameters["@auxFileId"].Value = auxFileId;
			SqlConnection conn = null;
			int rowsAffected = 0;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
//			} catch {
//				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public /*MemoryStream*/AuxFile GetAuxFile(int auxFileId) {
			using (SqlCommand cmd = GetAuxFileGetter()) {
				SqlConnection conn = null;
				AuxFile file = null;
				try {
				
					cmd.Parameters[AuxFileID].Value = auxFileId;
					conn = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection = conn;
					cmd.Connection.Open();
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				
					//				byte[] blob = (byte[])cmd.ExecuteReader.ExecuteScalar();
					//				object blob = cmd.ExecuteScalar();
					//				return null;
					while(reader.Read()) {
						file = new AuxFile();
						file.Blob =(byte[]) reader["pic"];
						//file.AuxFileId = auxFileId;
						file.AuxFileId	= (int)reader["AuxFileId"];
						file.OrderId	= (int)reader["OrderID"];
						
						file.PartId		= (reader["PartID"]==DBNull.Value)?-1:(int)reader["PartID"];//(int)reader["PartID"];
						file.FileName	= (string)reader["FileName"];
						file.FileType	= (AuxFileType)reader["FileType"];
						file.Description= (string)reader["Description"];
						file.FileContentType = (string)reader["FileContentType"];
					}
					return file;
				} catch (Exception ex) {
					//TODO: Consider logging
					return /*(MemoryStream)MemoryStream.Null*/null;
				} finally {
					if (conn != null) conn.Close();
					cmd.Connection = null;
				}
			}
			
		}

		#endregion

		#region Extras
		
		[Obsolete("",true)]
		public bool AddExtras(int orderId, /*ArrayList*/Hashtable selectedExtras) {
			return AddExtras(orderId, selectedExtras, null);
		}
		
		[Obsolete("",true)]
		public bool AddExtras(int orderId, /*ArrayList*/Hashtable selectedExtras, SqlTransaction transaction) {
			if ((selectedExtras==null)||(selectedExtras.Count==0)) { 
				// no extras should be added 
				return true;
			}
			//----------------------------------------
			// TODO: consider moving to the separate function
			StringBuilder sb = new StringBuilder(255);
			int i = 0;
			foreach(object extraId in selectedExtras) {
				if (i>0) sb.Append(",");
				sb.Append(extraId);
				i++;
			}
			//----------------------------------------
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}
			//GetAddExtrasCmd
			SqlCommand cmd = GetAddExtrasCmd();
			cmd.Parameters[OrderID].Value = orderId;

			cmd.Parameters["@extras"].Value = ((sb.ToString()==null)||(sb.ToString()==String.Empty))?DBNull.Value:(object)sb.ToString();
			
			int rowsAffected = 0;
			
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
				}
				rowsAffected = cmd.ExecuteNonQuery();

				return (rowsAffected >0); // temporary
			} catch (Exception ex) {
				return false;
			} finally {
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}
		}
		//UpdateSelectedExtra
		public bool UpdateSelectedExtras(int orderId, /*ArrayList*/Hashtable selectedExtras, SqlTransaction transaction) {
			
			if ((selectedExtras==null)||(selectedExtras.Count==0)) { 
				// no extras should be added 
				return true;
			}
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}
			//GetAddExtrasCmd
			SqlCommand cmdDel = GetDeleteSelectedExtrasCmd();
			SqlCommand cmd = GetUpdateSelectedExtraCmd();
			/*
			 	@ExtraID	int,
				@OrderID int,
				@Price smallmoney,
				@QuantityToApply int,
				@IsPricePerM bit,
				@TotalExtraAmount smallmoney,
				@Attribute1Value varchar(20),
				@Attribute2Value varchar(20),
				@Attribute3Value varchar(20),
				@Attribute4Value varchar(20)
			*/
			bool res = true;
			try {
				if (transaction==null) {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
				} else {
					cmd.Connection = transaction.Connection;
					cmd.Transaction = transaction;
					cmdDel.Transaction = cmd.Transaction;
				}
				//TODO: add inner transaction
				cmdDel.Connection = cmd.Connection;
				

				cmdDel.Parameters[OrderID].Value = orderId;
				int ra = 0;
				ra = cmdDel.ExecuteNonQuery();
				/*res = res && (ra>0);
				if (!res) return false;*/
				foreach (SelectedExtraInfo selectedExtra in selectedExtras.Values) {
					cmd.Parameters[OrderID].Value = orderId;
					cmd.Parameters["@ExtraID"].Value = selectedExtra.ExtraID;
					cmd.Parameters["@Price"].Value = selectedExtra.Price;
					cmd.Parameters["@QuantityToApply"].Value = selectedExtra.QuantityToApply;
					cmd.Parameters["@IsPricePerM"].Value = selectedExtra.IsPricePerM;
					cmd.Parameters["@TotalExtraAmount"].Value = selectedExtra.TotalExtraAmount;
				
					cmd.Parameters["@Attribute1Value"].Value = (selectedExtra.AttributesValues[0].Value==null?DBNull.Value:selectedExtra.AttributesValues[0].Value);
					cmd.Parameters["@Attribute2Value"].Value = (selectedExtra.AttributesValues[1].Value==null?DBNull.Value:selectedExtra.AttributesValues[1].Value);
					cmd.Parameters["@Attribute3Value"].Value = (selectedExtra.AttributesValues[2].Value==null?DBNull.Value:selectedExtra.AttributesValues[2].Value);
					cmd.Parameters["@Attribute4Value"].Value = (selectedExtra.AttributesValues[3].Value==null?DBNull.Value:selectedExtra.AttributesValues[3].Value);
					/*
					cmd.Parameters["@Attribute2Value"].Value = selectedExtra.AttributesValues[1];
					cmd.Parameters["@Attribute3Value"].Value = selectedExtra.AttributesValues[2];
					cmd.Parameters["@Attribute4Value"].Value = selectedExtra.AttributesValues[3];
					*/
					int rowsAffected = 0;
					rowsAffected = cmd.ExecuteNonQuery();
					res = res && (rowsAffected>0);

				}
				

				return (res); 
			} catch (Exception ex) {
				return false;
			} finally {
				cmdDel.Dispose();
				if (transaction==null) {
					cmd.Connection.Close();
				}
			}

			
		}

		#endregion

		#region promotion codes usage

		public bool AddCodesUsage(OrderInfo orderInfo) {
			return AddCodesUsage(orderInfo, null);
		}

		public bool AddCodesUsage(OrderInfo orderInfo, SqlTransaction transaction) {
			PromoCodesDAL pcDal = new PromoCodesDAL(this.components);
			try {
				//TODO: consider having transaction for adding several codes even if 'transaction' param is null
				//This is not urgent as long as currently only a single promocde will be used.
				foreach( string code in orderInfo.OrderPromoCodes.Codes) {
					pcDal.AddCodeUsage(code,orderInfo.SiteID, orderInfo.CustomerID, orderInfo.OrderId, transaction);
				}
				return true;
			} catch (DALException) {
				return false;
			}
		}

		#endregion

		#region Order log
		public bool AddOrderLogEntry(OrderLog.Entry logEntry) {
			return AddOrderLogEntry(logEntry, null);
		}

		public bool AddOrderLogEntry(OrderLog.Entry logEntry, SqlTransaction transaction) {
			if ((transaction!=null)&&((transaction.Connection == null)||(transaction.Connection.State != ConnectionState.Open))) {
				throw new ArgumentException("'transaction.Connection' should be 'open' ");
			}

			// dispose command after using because we don't pool this command in private class member
			using (SqlCommand cmd = GetAddOrderLogEntryCommand()) {
				cmd.Parameters[OrderID].Value = logEntry.OrderId;
				cmd.Parameters["@eventTimeStamp"].Value = logEntry.TimeStamp;
				cmd.Parameters["@Category"].Value = logEntry.Category;
				cmd.Parameters["@Name"].Value = logEntry.Name;
				cmd.Parameters["@Description"].Value = logEntry.Description;
				cmd.Parameters["@EmployeeUID"].Value = (logEntry.EmployeeUID!=null)?(object)logEntry.EmployeeUID:DBNull.Value;

				int rowsAffected = 0;
			
				try {
					//conn = new SqlConnection(AppConfig.dbConnString);
					if (transaction==null) {
						cmd.Connection = new SqlConnection(AppConfig.dbConnString);
						cmd.Connection.Open();
					} else {
						cmd.Connection = transaction.Connection;
						cmd.Transaction = transaction;
					}
					rowsAffected = cmd.ExecuteNonQuery();
					return rowsAffected > 0;
				} catch (Exception ex) {
					return false;
				} finally {
					if (transaction==null) {
						cmd.Connection.Close();
					}
				}
			}

		}
		//GetFullOrderLog
		public string GetFullOrderLog(int orderId) {
			using (SqlCommand cmd = GetFullOrderLogCmd()) {
				cmd.Parameters[OrderID].Value = orderId;
				try {
					cmd.Connection = new SqlConnection(AppConfig.dbConnString);
					cmd.Connection.Open();
					string res = (string)cmd.ExecuteScalar();
					//add root tag to the xml string
					res = (res == null)?String.Empty:res;
					StringBuilder sb = new StringBuilder(res.Length+OrderLog.OrderLogRootTag.Length*2+5);
					sb.Append("<");
					sb.Append(OrderLog.OrderLogRootTag);
					sb.Append(">");
					sb.Append(res);
					sb.Append("</");
					sb.Append(OrderLog.OrderLogRootTag);
					sb.Append(">");
					return sb.ToString();
					//return (string) cmd.ExecuteScalar();
				} finally {
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
