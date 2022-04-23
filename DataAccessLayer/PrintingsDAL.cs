using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using HiRes.Common;
using System.Text;

using HiRes.SystemFramework.Logging;

namespace HiRes.DAL {
	/// <summary>
	/// PrintingsDAL incapsulates printing types related db operations.
	/// </summary>
	public class PrintingsDAL : System.ComponentModel.Component {
		/// <summary>
		/// Required designer variable.
		/// </summary>

		#region Constants for SQLCommand parameters

		private System.ComponentModel.Container components = null;
		
		private const int MAX_EXTRA_ATTR_NUM = 4;

		private const String SiteId = "@siteId";
		private const String PrintingTypeId = "@printingTypeID";
		private const String PaperSizeID = "@paperSizeID";
		private const String Quantity = "@Quantity";
		private const String QuantityDispLabel = "@QuantityDispLabel";
		private const String PaperTypeID = "@PaperTypeID";
		private const String Price = "@Price";
		private const String IsSpecial = "@IsSpecial";
		private const String WholesaleCode = "@code";
		
		private const String PrintingTypeName = "@PrintingTypeName";
		private const String PrintingTypeDescription = "@PrintingTypeDescription";
		private const String PaperSizeName = "@PaperSizeName";
		private const String PaperSizeDescription = "@PaperSizeDescription";
		private const String PaperTypeName = "@PaperTypeName";
		private const String PaperTypeDescription = "@PaperTypeDescription";
		private const String PartName = "@PartName";
		private const String Description = "@Description";
		private const String PartID = "@PartID";

		private const String ExtraID = "@ExtraID";
		private const String ExtraName = "@ExtraName";
		private const String ExtraDescription = "@ExtraDescription";
		//private const String ExtraPrice = "@ExtraPrice";
		private const String Attribute1Name = "@Attribute1Name";
		private const String Attribute1Type = "@Attribute1Type";
		private const String Attribute1DefValue = "@Attribute1DefValue";
		private const String Attribute2Name = "@Attribute2Name";
		private const String Attribute2Type = "@Attribute2Type";
		private const String Attribute2DefValue = "@Attribute2DefValue";
		private const String Attribute3Name = "@Attribute3Name";
		private const String Attribute3Type = "@Attribute3Type";
		private const String Attribute3DefValue = "@Attribute3DefValue";
		private const String Attribute4Name = "@Attribute4Name";
		private const String Attribute4Type = "@Attribute4Type";
		private const String Attribute4DefValue = "@Attribute4DefValue";

		private const String Filter = "@filterClause";
		private const String Order = "@orderClause";
	
		#endregion
		
		#region Sql commands
		private SqlCommand printingTypesNamesCommand;
		private SqlCommand loadPaperSizesPaperTypesCmd;
		private SqlCommand loadPrintingTypeCommand;
		private SqlCommand loadPaperSizeCommand;
		private SqlCommand loadPaperTypeCommand;
		private SqlCommand loadPrintingTypesCommand;
		private SqlCommand loadPaperSizesCommand;
		private SqlCommand loadPaperTypesCommand;
		private SqlCommand loadExtrasCommand;
		private SqlCommand loadOrderQuantitiesCommand;
		private SqlCommand loadPrintingTypeParts;
		private SqlCommand loadPricesCommand;
		private SqlCommand loadWSPricesCommand;
		private SqlCommand loadPrintingSheduleCommand;
		private SqlCommand getAvailPaperTypesCmd;
		private SqlCommand getExtraPricesCommand;
		private SqlCommand addPrintingPriceCommand;
		private SqlCommand addWholesalePriceCommand;
		private SqlCommand updatePrintingPriceCommand;
		private SqlCommand updateWSPriceCommand;


		private SqlCommand loadAllPaperTypesCommand;
		private SqlCommand loadAllExtrasCommand;
		private SqlCommand addExtraInfoCommand;
		private SqlCommand updateExtraInfoCommand;
		private SqlCommand loadExtraInfoCommand;

		private SqlCommand setAvailableExtrasCommand;
		private SqlCommand deleteAvailableExtrasCommand;

		private SqlCommand addPrintingTypeCommand;
		private SqlCommand addPaperSizeCommand;
		private SqlCommand addPaperTypeCommand;
		private SqlCommand addPaperTypePartCommand;
		private SqlCommand updatePrintingTypeCommand;
		private SqlCommand updatePaperSizeCommand;
		private SqlCommand updatePaperTypeCommand;
		private SqlCommand updatePaperTypePartCommand;
		private SqlCommand allowPaperTypeCommand;
		private SqlCommand deleteAvailablePaperTypesCommand;
		private SqlCommand setAvailablePaperTypesCommand;

		private SqlCommand checkPrintingTypeCommand;

		private SqlCommand removePaperSizeCommand;

		private SqlCommand removeOrderQuantityCommand;
		private SqlCommand loadOrderQuantityInfoCommand;
		private SqlCommand addOrderQuantityInfoCommand;
		private SqlCommand updateOrderQuantityInfoCommand;

		private SqlCommand loadPrintingTypePartInfoCommand;
		private SqlCommand removePrintingTypePartCommand;

		#endregion

		#region Constructors
		public PrintingsDAL(System.ComponentModel.IContainer container) {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public PrintingsDAL() {
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
			
			if (! disposing)
				return; // we're being collected, so let the GC take care of this object
			try {
				if (printingTypesNamesCommand != null) 
					printingTypesNamesCommand.Dispose();
				if (loadPaperSizesPaperTypesCmd != null) 
					loadPaperSizesPaperTypesCmd.Dispose();
				if (loadPrintingTypeCommand != null) 
					loadPrintingTypeCommand.Dispose();
				if (loadPrintingTypesCommand!= null) 
					loadPrintingTypesCommand.Dispose();
				if (loadPaperSizesCommand != null) 
					loadPaperSizesCommand.Dispose();
				if (loadPaperTypesCommand != null) 
					loadPaperTypesCommand.Dispose();
				if (loadExtrasCommand != null) 
					loadExtrasCommand.Dispose();
				if (loadOrderQuantitiesCommand != null) 
					loadOrderQuantitiesCommand.Dispose();
				if (loadPrintingTypeParts!= null) 
					loadPrintingTypeParts.Dispose();
				if (loadPricesCommand != null) 
					loadPricesCommand.Dispose();
				if (loadWSPricesCommand != null) 
					loadWSPricesCommand.Dispose();
				if (loadPrintingSheduleCommand != null) 
					loadPrintingSheduleCommand.Dispose();
				if (getAvailPaperTypesCmd != null) 
					getAvailPaperTypesCmd.Dispose();
				if (getExtraPricesCommand != null) 
					getExtraPricesCommand.Dispose();
				if (addPrintingPriceCommand != null) 
					addPrintingPriceCommand.Dispose();
				if (addWholesalePriceCommand != null) 
					addWholesalePriceCommand.Dispose();
				if (updatePrintingPriceCommand != null) 
					updatePrintingPriceCommand.Dispose();
				if (updateWSPriceCommand != null) 
					updateWSPriceCommand.Dispose();

				if (loadPaperSizeCommand != null) 
					loadPaperSizeCommand.Dispose();
				if (loadPaperTypeCommand != null) 
					loadPaperTypeCommand.Dispose();
				if (loadAllPaperTypesCommand != null)
					loadAllPaperTypesCommand.Dispose();

				if (loadAllExtrasCommand != null)
					loadAllExtrasCommand.Dispose();
				if (loadExtraInfoCommand != null)
					loadExtraInfoCommand.Dispose();
				if (addExtraInfoCommand != null)
					addExtraInfoCommand.Dispose();
				if (updateExtraInfoCommand != null)
					updateExtraInfoCommand.Dispose();
				if (setAvailableExtrasCommand != null)
					setAvailableExtrasCommand.Dispose();
				if (deleteAvailableExtrasCommand != null)
					deleteAvailableExtrasCommand.Dispose();

				if (addPrintingTypeCommand != null) 
					addPrintingTypeCommand.Dispose();
				if (addPaperSizeCommand != null) 
					addPaperSizeCommand.Dispose();
				if (addPaperTypeCommand != null) 
					addPaperTypeCommand.Dispose();
				if (addPaperTypePartCommand != null) 
					addPaperTypePartCommand.Dispose();

				if (updatePrintingTypeCommand != null) 
					updatePrintingTypeCommand.Dispose();
				if (updatePaperSizeCommand != null) 
					updatePaperSizeCommand.Dispose();
				if (updatePaperTypeCommand != null) 
					updatePaperTypeCommand.Dispose();
				if (updatePaperTypePartCommand != null) 
					updatePaperTypePartCommand.Dispose();
				if (allowPaperTypeCommand != null) 
					allowPaperTypeCommand.Dispose();
				if (deleteAvailablePaperTypesCommand != null) 
					deleteAvailablePaperTypesCommand.Dispose();
				if (setAvailablePaperTypesCommand != null) 
					setAvailablePaperTypesCommand.Dispose();
			
				if (removePaperSizeCommand!= null) 
					removePaperSizeCommand.Dispose();

				if (checkPrintingTypeCommand != null)
					checkPrintingTypeCommand.Dispose();

				if (removeOrderQuantityCommand!= null) 
					removeOrderQuantityCommand.Dispose();

				if (loadOrderQuantityInfoCommand!= null) 
					loadOrderQuantityInfoCommand.Dispose();
				if (addOrderQuantityInfoCommand!= null) 
					addOrderQuantityInfoCommand.Dispose();
				if (updateOrderQuantityInfoCommand!= null) 
					updateOrderQuantityInfoCommand.Dispose();

				if (loadPrintingTypePartInfoCommand != null)
					loadPrintingTypePartInfoCommand.Dispose();
				if (removePrintingTypePartCommand != null)
					removePrintingTypePartCommand.Dispose();

			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion

		#region SQLCommand getters

		private SqlCommand GetCheckPrintingTypeCommand() {
			if ( checkPrintingTypeCommand == null ) {
				checkPrintingTypeCommand = new SqlCommand("HiResAdmin.CheckPrintingType");
				checkPrintingTypeCommand.CommandType = CommandType.StoredProcedure;
				checkPrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				checkPrintingTypeCommand.Parameters.Add(new SqlParameter("@valid", SqlDbType.Int));
				checkPrintingTypeCommand.Parameters["@valid"].Direction = ParameterDirection.Output;
			}
			return checkPrintingTypeCommand;
		}

		private SqlCommand GetLoadPrintingTypeCommand() {
			if ( loadPrintingTypeCommand == null ) {
				loadPrintingTypeCommand = new SqlCommand("HiResAdmin.getPrintingType");
				loadPrintingTypeCommand.CommandType = CommandType.StoredProcedure;
				loadPrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPrintingTypeCommand;
		}

		private SqlCommand GetLoadPaperSizeCommand() {
			if ( loadPaperSizeCommand == null ) {
				loadPaperSizeCommand = new SqlCommand("HiResAdmin.GetPaperSize");
				loadPaperSizeCommand.CommandType = CommandType.StoredProcedure;
				loadPaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				loadPaperSizeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPaperSizeCommand;
		}


		private SqlCommand GetRemovePaperSizeCommand() {
			if ( removePaperSizeCommand== null ) {
				removePaperSizeCommand = new SqlCommand("HiResAdmin.removePaperSize");
				removePaperSizeCommand.CommandType = CommandType.StoredProcedure;
				removePaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
			}
			return removePaperSizeCommand;
		}

		private SqlCommand GetLoadPaperTypeCommand() {
			if ( loadPaperTypeCommand == null ) {
				loadPaperTypeCommand = new SqlCommand("HiResAdmin.getPaperType");
				loadPaperTypeCommand.CommandType = CommandType.StoredProcedure;
				loadPaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
			}
			return loadPaperTypeCommand;
		}


		private SqlCommand GetLoadAllPaperTypesCommand() {
			if ( loadAllPaperTypesCommand == null ) {
				loadAllPaperTypesCommand = new SqlCommand("HiResAdmin.getAllPaperTypes");
				loadAllPaperTypesCommand.CommandType = CommandType.StoredProcedure;
				loadAllPaperTypesCommand.Parameters.Add(new SqlParameter(Filter, SqlDbType.NVarChar,1000));
				loadAllPaperTypesCommand.Parameters.Add(new SqlParameter(Order, SqlDbType.NVarChar,1000));
			}
			return loadAllPaperTypesCommand;
		}

		private SqlCommand GetLoadAllExtrasCommand() {
			if ( loadAllExtrasCommand == null ) {
				loadAllExtrasCommand = new SqlCommand("HiResAdmin.getAllExtras");
				loadAllExtrasCommand.CommandType = CommandType.StoredProcedure;
				loadAllExtrasCommand.Parameters.Add(new SqlParameter(Filter, SqlDbType.NVarChar,1000));
				loadAllExtrasCommand.Parameters.Add(new SqlParameter(Order, SqlDbType.NVarChar,1000));
			}
			return loadAllExtrasCommand;
		}


		
		private SqlCommand GetLoadExtraInfoCommand() {
			if ( loadExtraInfoCommand == null ) {
				loadExtraInfoCommand = new SqlCommand("HiResAdmin.getExtraInfo");
				loadExtraInfoCommand.CommandType = CommandType.StoredProcedure;
				loadExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraID, SqlDbType.Int));
			}
			return loadExtraInfoCommand;
		}


		private SqlCommand GetAddExtraInfoCommand() {
			if ( addExtraInfoCommand == null ) {
				addExtraInfoCommand = new SqlCommand("HiResAdmin.addExtraInfo");
				addExtraInfoCommand.CommandType = CommandType.StoredProcedure;
				addExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraName, SqlDbType.VarChar, 40));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraDescription, SqlDbType.VarChar, 255));
				//addExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraPrice, SqlDbType.SmallMoney));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute1Name, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute1Type, SqlDbType.TinyInt));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute1DefValue, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute2Name, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute2Type, SqlDbType.TinyInt));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute2DefValue, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute3Name, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute3Type, SqlDbType.TinyInt));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute3DefValue, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute4Name, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute4Type, SqlDbType.TinyInt));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute4DefValue, SqlDbType.Char, 20));
				addExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraID, SqlDbType.Int));
				addExtraInfoCommand.Parameters[ExtraID].Direction = ParameterDirection.Output;
			}
			return addExtraInfoCommand;
		}


		private SqlCommand GetUpdateExtraInfoCommand() {
			if ( updateExtraInfoCommand == null ) {
				updateExtraInfoCommand = new SqlCommand("HiResAdmin.updateExtraInfo");
				updateExtraInfoCommand.CommandType = CommandType.StoredProcedure;
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraName, SqlDbType.VarChar, 40));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraDescription, SqlDbType.VarChar, 255));
				//updateExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraPrice, SqlDbType.SmallMoney));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute1Name, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute1Type, SqlDbType.TinyInt));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute1DefValue, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute2Name, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute2Type, SqlDbType.TinyInt));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute2DefValue, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute3Name, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute3Type, SqlDbType.TinyInt));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute3DefValue, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute4Name, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute4Type, SqlDbType.TinyInt));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(Attribute4DefValue, SqlDbType.Char, 20));
				updateExtraInfoCommand.Parameters.Add(new SqlParameter(ExtraID, SqlDbType.Int));
			}
			return updateExtraInfoCommand;
		}

		
		
		
		/// <summary>
		/// SQLCommand for getting printing types list 
		/// </summary>
		/// <returns></returns>
		private SqlCommand GetLoadPrintingTypesCommand() {
			if ( loadPrintingTypesCommand == null ) {
				loadPrintingTypesCommand = new SqlCommand("HiResAdmin.getPrintingTypes");
				loadPrintingTypesCommand.CommandType = CommandType.StoredProcedure;
				loadPrintingTypesCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return loadPrintingTypesCommand;
		}

		private SqlCommand GetPrintingTypesNamesCommand() {
			if ( printingTypesNamesCommand == null ) {
				printingTypesNamesCommand = new SqlCommand("HiResAdmin.getPrintingTypesList");
				printingTypesNamesCommand.CommandType = CommandType.StoredProcedure;
				printingTypesNamesCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return printingTypesNamesCommand;
		}

		private SqlCommand GetLoadPaperSizesPaperTypesCommand() {
			if ( loadPaperSizesPaperTypesCmd == null ) {
				loadPaperSizesPaperTypesCmd = new SqlCommand("HiResAdmin.loadPaperSizesPaperTypes");
				loadPaperSizesPaperTypesCmd.CommandType = CommandType.StoredProcedure;
				loadPaperSizesPaperTypesCmd.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPaperSizesPaperTypesCmd;
		}
		
		private SqlCommand GetLoadPaperSizesCommand() {
			if ( loadPaperSizesCommand == null ) {
				loadPaperSizesCommand = new SqlCommand("HiResAdmin.loadPaperSizes");
				loadPaperSizesCommand.CommandType = CommandType.StoredProcedure;
				loadPaperSizesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPaperSizesCommand;
		}

		private SqlCommand GetLoadPaperTypesCommand() {
			if ( loadPaperTypesCommand == null ) {
				loadPaperTypesCommand = new SqlCommand("HiResAdmin.loadPaperTypes");
				loadPaperTypesCommand.CommandType = CommandType.StoredProcedure;
				loadPaperTypesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				loadPaperTypesCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
			}
			return loadPaperTypesCommand;
		}

		private SqlCommand GetLoadExtrasCommand() {
			if ( loadExtrasCommand == null ) {
				loadExtrasCommand = new SqlCommand("HiResAdmin.loadExtras");
				loadExtrasCommand.CommandType = CommandType.StoredProcedure;
				loadExtrasCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadExtrasCommand;
		}


		
		private SqlCommand GetLoadOrderQuantitiesCommand() {
			if ( loadOrderQuantitiesCommand == null ) {
				loadOrderQuantitiesCommand = new SqlCommand("HiResAdmin.loadOrderQuantities");
				loadOrderQuantitiesCommand.CommandType = CommandType.StoredProcedure;
				loadOrderQuantitiesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadOrderQuantitiesCommand;
		}
		
		private SqlCommand GetLoadOrderQuantityInfoCommand() {
			if ( loadOrderQuantityInfoCommand == null ) {
				loadOrderQuantityInfoCommand = new SqlCommand("HiResAdmin.GetOrderQuantityInfo");
				loadOrderQuantityInfoCommand.CommandType = CommandType.StoredProcedure;
				loadOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				loadOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
			}
			return loadOrderQuantityInfoCommand;
		}

		private SqlCommand GetAddOrderQuantityCommand() {
			if ( addOrderQuantityInfoCommand == null ) {
				addOrderQuantityInfoCommand = new SqlCommand("HiResAdmin.AddOrderQuantity");
				addOrderQuantityInfoCommand.CommandType = CommandType.StoredProcedure;
				addOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				addOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
				addOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(QuantityDispLabel, SqlDbType.NVarChar, 20));
			}
			return addOrderQuantityInfoCommand;
		}
		
		private SqlCommand GetUpdateOrderQuantityCommand() {
			if ( updateOrderQuantityInfoCommand == null ) {
				updateOrderQuantityInfoCommand = new SqlCommand("HiResAdmin.UpdateOrderQuantity");
				updateOrderQuantityInfoCommand.CommandType = CommandType.StoredProcedure;
				updateOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				updateOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
				updateOrderQuantityInfoCommand.Parameters.Add(new SqlParameter(QuantityDispLabel, SqlDbType.NVarChar, 20));
			}
			return updateOrderQuantityInfoCommand;
		}
		


/*		private SqlCommand GetPrintingTypesCommand() {
			if ( loadPrintingTypesCommand == null ) {
				loadPrintingTypesCommand = new SqlCommand("HiResAdmin.getPrintingTypes");
				loadPrintingTypesCommand.CommandType = CommandType.StoredProcedure;
				loadPrintingTypesCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
			}
			return loadPrintingTypesCommand;
		}*/
		
		private SqlCommand GetLoadPrintingTypeParts() {
			if ( loadPrintingTypeParts == null ) {
				loadPrintingTypeParts = new SqlCommand("HiResAdmin.getPrintingTypeParts");
				loadPrintingTypeParts.CommandType = CommandType.StoredProcedure;
				loadPrintingTypeParts.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPrintingTypeParts;
		}

		private SqlCommand GetLoadPrintingTypePartInfo() {
			if ( loadPrintingTypePartInfoCommand == null ) {
				loadPrintingTypePartInfoCommand = new SqlCommand("HiResAdmin.getPrintingTypePart");
				loadPrintingTypePartInfoCommand.CommandType = CommandType.StoredProcedure;
				loadPrintingTypePartInfoCommand.Parameters.Add(new SqlParameter(PartID, SqlDbType.Int));
				loadPrintingTypePartInfoCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPrintingTypePartInfoCommand;
		}

		private SqlCommand GetRemovePrintingTypePart() {
			if ( removePrintingTypePartCommand == null ) {
				removePrintingTypePartCommand = new SqlCommand("HiResAdmin.removePrintingTypePart");
				removePrintingTypePartCommand.CommandType = CommandType.StoredProcedure;
				removePrintingTypePartCommand.Parameters.Add(new SqlParameter(PartID, SqlDbType.Int));
			}
			return removePrintingTypePartCommand;
		}


		private SqlCommand GetLoadPricesCommand() {
			if ( loadPricesCommand == null ) {
				loadPricesCommand = new SqlCommand("HiResAdmin.getPrices");
				loadPricesCommand.CommandType = CommandType.StoredProcedure;
				loadPricesCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				loadPricesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadPricesCommand;
		}
		
		private SqlCommand GetLoadWholesalePricesCommand() {
			if ( loadWSPricesCommand == null ) {
				loadWSPricesCommand = new SqlCommand("HiResAdmin.getWholesalePrices");
				loadWSPricesCommand.CommandType = CommandType.StoredProcedure;
				loadWSPricesCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				loadWSPricesCommand.Parameters.Add(new SqlParameter(WholesaleCode, SqlDbType.VarChar,64));
				loadWSPricesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return loadWSPricesCommand;
		}
		

		private SqlCommand GetPrintingSheduleCommand() {
			if ( loadPrintingSheduleCommand == null ) {
				loadPrintingSheduleCommand = new SqlCommand("HiResAdmin.getPrintingShedule");
				loadPrintingSheduleCommand.CommandType = CommandType.StoredProcedure;
				loadPrintingSheduleCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				loadPrintingSheduleCommand.Parameters.Add(new SqlParameter("@since", SqlDbType.DateTime));
				loadPrintingSheduleCommand.Parameters.Add(new SqlParameter("@upTo", SqlDbType.DateTime));
			}
			return loadPrintingSheduleCommand;
		}

		private SqlCommand GetAvailPaperTypesCommand() {
			if ( getAvailPaperTypesCmd == null ) {
				getAvailPaperTypesCmd = new SqlCommand("HiResAdmin.getAvailablePaperTypes");
				getAvailPaperTypesCmd.CommandType = CommandType.StoredProcedure;
				getAvailPaperTypesCmd.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return getAvailPaperTypesCmd;
		}


		private SqlCommand GetDeleteAvailPaperTypesCommand() {
			if ( deleteAvailablePaperTypesCommand == null ) {
				deleteAvailablePaperTypesCommand = new SqlCommand("HiResAdmin.DeleteAvailablePaperTypes");
				deleteAvailablePaperTypesCommand.CommandType = CommandType.StoredProcedure;
				deleteAvailablePaperTypesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				deleteAvailablePaperTypesCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
			}
			return deleteAvailablePaperTypesCommand;
		}


		private SqlCommand GetExtraPricesCommand() {
			
			if ( getExtraPricesCommand == null ) {
				getExtraPricesCommand = new SqlCommand("HiResAdmin.GetFullExtrasPriceForPrintingType");
				//getExtraPricesCommand = new SqlCommand("HiResAdmin.getExtras");
				getExtraPricesCommand.CommandType = CommandType.StoredProcedure;
				getExtraPricesCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				getExtraPricesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return getExtraPricesCommand;

		}

		private SqlCommand GetAddPrintingPriceCommand() {
			if (addPrintingPriceCommand == null) {
				addPrintingPriceCommand = new SqlCommand("HiResAdmin.addPrintingPrice");
				addPrintingPriceCommand.CommandType = CommandType.StoredProcedure;
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(Price, SqlDbType.SmallMoney));
				addPrintingPriceCommand.Parameters.Add(new SqlParameter(IsSpecial, SqlDbType.Bit));
			}
			return addPrintingPriceCommand;
		}
		
		private SqlCommand GetAddWholesalePriceCommand() {
			if (addWholesalePriceCommand == null) {
				addWholesalePriceCommand = new SqlCommand("HiResAdmin.addWholesalePrice");
				addWholesalePriceCommand.CommandType = CommandType.StoredProcedure;
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(WholesaleCode, SqlDbType.VarChar,64));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(Price, SqlDbType.SmallMoney));
				addWholesalePriceCommand.Parameters.Add(new SqlParameter(IsSpecial, SqlDbType.Bit));
			}
			return addWholesalePriceCommand;
		}

		private SqlCommand GetUpdatePrintingPriceCommand() {
			if (updatePrintingPriceCommand == null) {
				updatePrintingPriceCommand = new SqlCommand("HiResAdmin.updatePrintingPrice");
				updatePrintingPriceCommand.CommandType = CommandType.StoredProcedure;
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(Price, SqlDbType.SmallMoney));
				updatePrintingPriceCommand.Parameters.Add(new SqlParameter(IsSpecial, SqlDbType.Bit));
			}
			return updatePrintingPriceCommand;
		}

		private SqlCommand GetUpdateWSPriceCommand() {
			if (updateWSPriceCommand == null) {
				updateWSPriceCommand = new SqlCommand("HiResAdmin.updateWholesalePrice");
				updateWSPriceCommand.CommandType = CommandType.StoredProcedure;
				updateWSPriceCommand.Parameters.Add(new SqlParameter(WholesaleCode, SqlDbType.VarChar,64));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(Price, SqlDbType.SmallMoney));
				updateWSPriceCommand.Parameters.Add(new SqlParameter(IsSpecial, SqlDbType.Bit));

			}
			return updateWSPriceCommand;
		}


		
		private SqlCommand GetAddPrintingTypeCommand() {
			if (addPrintingTypeCommand == null) {
				addPrintingTypeCommand = new SqlCommand("HiResAdmin.AddPrintingType");
				addPrintingTypeCommand.CommandType = CommandType.StoredProcedure;
				addPrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeName, SqlDbType.NVarChar,100));
				addPrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeDescription, SqlDbType.NVarChar,255));
				addPrintingTypeCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				addPrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				addPrintingTypeCommand.Parameters[PrintingTypeId].Direction = ParameterDirection.Output;
			}
			return addPrintingTypeCommand;
		}
		
		private SqlCommand GetAddPaperSizeCommand() {
			if (addPaperSizeCommand == null) {
				addPaperSizeCommand = new SqlCommand("HiResAdmin.AddPaperSize");
				addPaperSizeCommand.CommandType = CommandType.StoredProcedure;
				addPaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeName, SqlDbType.NVarChar,30));
				addPaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeDescription, SqlDbType.NVarChar,255));
				addPaperSizeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				addPaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				addPaperSizeCommand.Parameters[PaperSizeID].Direction = ParameterDirection.Output;
			}
			return addPaperSizeCommand;
		}
		
		private SqlCommand GetAddPaperTypeCommand() {
			if (addPaperTypeCommand == null) {
				addPaperTypeCommand = new SqlCommand("HiResAdmin.AddPaperType");
				addPaperTypeCommand.CommandType = CommandType.StoredProcedure;
				addPaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeName, SqlDbType.NVarChar,30));
				addPaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeDescription, SqlDbType.NVarChar,255));
				addPaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
				addPaperTypeCommand.Parameters[PaperTypeID].Direction = ParameterDirection.Output;
			}
			return addPaperTypeCommand;
		}
		
		private SqlCommand GetAddPaperTypePartCommand() {
			if (addPaperTypePartCommand == null) {
				addPaperTypePartCommand = new SqlCommand("HiResAdmin.AddPrintingTypePart");
				addPaperTypePartCommand.CommandType = CommandType.StoredProcedure;
				addPaperTypePartCommand.Parameters.Add(new SqlParameter(PartName, SqlDbType.NVarChar,30));
				addPaperTypePartCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				addPaperTypePartCommand.Parameters.Add(new SqlParameter(Description, SqlDbType.NVarChar,255));
				addPaperTypePartCommand.Parameters.Add(new SqlParameter(PartID, SqlDbType.Int));
				addPaperTypePartCommand.Parameters[PartID].Direction = ParameterDirection.Output;
			}
			return addPaperTypePartCommand;
		}
		

		private SqlCommand GetUpdatePrintingTypeCommand() {
			if (updatePrintingTypeCommand == null) {
				updatePrintingTypeCommand = new SqlCommand("HiResAdmin.UpdatePrintingType");
				updatePrintingTypeCommand.CommandType = CommandType.StoredProcedure;
				updatePrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeName, SqlDbType.NVarChar,100));
				updatePrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeDescription, SqlDbType.NVarChar,255));
				updatePrintingTypeCommand.Parameters.Add(new SqlParameter(SiteId, SqlDbType.Int));
				updatePrintingTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return updatePrintingTypeCommand;
		}
		
		private SqlCommand GetUpdatePaperSizeCommand() {
			if (updatePaperSizeCommand == null) {
				updatePaperSizeCommand = new SqlCommand("HiResAdmin.UpdatePaperSize");
				updatePaperSizeCommand.CommandType = CommandType.StoredProcedure;
				updatePaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeName, SqlDbType.NVarChar,30));
				updatePaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeDescription, SqlDbType.NVarChar,255));
				updatePaperSizeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				updatePaperSizeCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
			}
			return updatePaperSizeCommand;
		}
		
		private SqlCommand GetUpdatePaperTypeCommand() {
			if (updatePaperTypeCommand == null) {
				updatePaperTypeCommand = new SqlCommand("HiResAdmin.UpdatePaperType");
				updatePaperTypeCommand.CommandType = CommandType.StoredProcedure;
				updatePaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeName, SqlDbType.NVarChar,30));
				updatePaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeDescription, SqlDbType.NVarChar,255));
				updatePaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
			}
			return updatePaperTypeCommand;
		}
		
		private SqlCommand GetUpdatePaperTypePartCommand() {
			if (updatePaperTypePartCommand == null) {
				updatePaperTypePartCommand = new SqlCommand("HiResAdmin.UpdatePrintingTypePart");
				updatePaperTypePartCommand.CommandType = CommandType.StoredProcedure;
				updatePaperTypePartCommand.Parameters.Add(new SqlParameter(PartName, SqlDbType.NVarChar,30));
				updatePaperTypePartCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				updatePaperTypePartCommand.Parameters.Add(new SqlParameter(Description, SqlDbType.NVarChar,255));
				updatePaperTypePartCommand.Parameters.Add(new SqlParameter(PartID, SqlDbType.Int));
			}
			return updatePaperTypePartCommand;
		}
		
		private SqlCommand GetAllowPaperTypeCommand() {
			if (allowPaperTypeCommand == null) {
				allowPaperTypeCommand = new SqlCommand("HiResAdmin.AllowPaperType");
				allowPaperTypeCommand.CommandType = CommandType.StoredProcedure;
				allowPaperTypeCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				allowPaperTypeCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				allowPaperTypeCommand.Parameters.Add(new SqlParameter(PaperTypeID, SqlDbType.Int));
			}
			return allowPaperTypeCommand;
		}
		
		private SqlCommand GetSetAvailPaperTypeCommand() {
			if (setAvailablePaperTypesCommand == null) {
				setAvailablePaperTypesCommand = new SqlCommand("HiResAdmin.SetAvailablePaperTypes");
				setAvailablePaperTypesCommand.CommandType = CommandType.StoredProcedure;
				setAvailablePaperTypesCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				setAvailablePaperTypesCommand.Parameters.Add(new SqlParameter(PaperSizeID, SqlDbType.Int));
				setAvailablePaperTypesCommand.Parameters.Add(new SqlParameter("@paramString", SqlDbType.VarChar, 255));
			}
			return setAvailablePaperTypesCommand;
		}
		

		private SqlCommand GetSetAvailExtrasCommand() {
			if (setAvailableExtrasCommand == null) {
				setAvailableExtrasCommand = new SqlCommand("HiResAdmin.SetAvailableExtras");
				setAvailableExtrasCommand.CommandType = CommandType.StoredProcedure;
				setAvailableExtrasCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				setAvailableExtrasCommand.Parameters.Add(new SqlParameter("@paramString", SqlDbType.VarChar, 255));
			}
			return setAvailableExtrasCommand;
		}
		
		private SqlCommand GetDeleteAvailExtrasCommand() {
			if ( deleteAvailableExtrasCommand == null ) {
				deleteAvailableExtrasCommand = new SqlCommand("HiResAdmin.DeleteAvailableExtras");
				deleteAvailableExtrasCommand.CommandType = CommandType.StoredProcedure;
				deleteAvailableExtrasCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
			}
			return deleteAvailableExtrasCommand;
		}

		#endregion
		
		#region Relational Info Loaders by printingTypeID

		/// <summary>
		/// Getting all Paper Sizes, related to giving Printing Type.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <returns></returns>
		public PaperSizeInfo[] GetPaperSizesPaperTypes(int printingTypeID) {
			SqlCommand cmd = GetLoadPaperSizesPaperTypesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlConnection conn = null;
			PaperSizeInfo[] array = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList sizes = new ArrayList();
				ArrayList types = new ArrayList();
				while (reader.Read()) {
					PaperSizeInfo size = null;
					if (sizes.Count != 0) {
						size = (PaperSizeInfo)sizes[sizes.Count-1];
					}
					if ((size != null) && 
						(size.PaperSizeID != (int)reader["PaperSizeID"])) {
						size.PaperTypes = new PaperTypeInfo[types.Count];
						types.CopyTo(size.PaperTypes);
						types.Clear();
						size = null;
					}
					if (size == null) {
						size = new PaperSizeInfo();
						size.PaperSizeID = (int)reader["PaperSizeID"];
						size.PaperSizeName = (String)reader["PaperSizeName"];
						if (reader["PaperSizeDescription"] != DBNull.Value) 
							size.PaperSizeDescription = (String)reader["PaperSizeDescription"];
						size.PrintingTypeID = printingTypeID;
						sizes.Add(size);
					}
					PaperTypeInfo type = new PaperTypeInfo();
					type.PaperTypeID = (int)reader["PaperTypeID"];
					type.PaperTypeName = (String)reader["PaperTypeName"];
					if (reader["PaperTypeDescription"] != DBNull.Value) 
						type.PaperTypeDescription = (String)reader["PaperTypeDescription"];
					types.Add(type);
				}
				if (sizes.Count != 0) {
					PaperSizeInfo size = (PaperSizeInfo)sizes[sizes.Count-1];
					size.PaperTypes = new PaperTypeInfo[types.Count];
					types.CopyTo(size.PaperTypes);
				}
				reader.Close();
				if (sizes.Count != 0) {
					array = new PaperSizeInfo[sizes.Count];
					sizes.CopyTo(array);
				}
			} catch (Exception e) {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return array;
		}


		/// <summary>
		/// Getting all Paper Sizes, related to giving Printing Type.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <returns></returns>
		public PaperSizeInfo[] GetPaperSizes(int printingTypeID) {
			SqlCommand cmd = GetLoadPaperSizesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlConnection conn = null;
			PaperSizeInfo[] array = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					PaperSizeInfo el = new PaperSizeInfo();
					el.PaperSizeID = (int)reader["PaperSizeID"];
					el.PaperSizeName = (String)reader["PaperSizeName"];
					if (reader["PaperSizeDescription"] != DBNull.Value) el.PaperSizeDescription = (String)reader["PaperSizeDescription"];
					el.PrintingTypeID = (int)reader["PrintingTypeID"];
					elements.Add(el);
				}
				reader.Close();
				if (elements.Count != 0) {
					array = new PaperSizeInfo[elements.Count];
					elements.CopyTo(array);
				}
			} catch (Exception e) {
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return array;
		}

		/// <summary>
		/// Getting all Paper Types, related to giving Printing Type.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <returns></returns>
		public PaperTypeInfo[] GetPaperTypes(int printingTypeID, int paperSizeId) {
			SqlCommand cmd = GetLoadPaperTypesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[PaperSizeID].Value = paperSizeId;
			SqlConnection conn = null;
			PaperTypeInfo[] array = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					PaperTypeInfo el = new PaperTypeInfo();
					el.PaperTypeID = (int)reader["PaperTypeID"];
					el.PaperTypeName = (String)reader["PaperTypeName"];
					if (reader["PaperTypeDescription"] != DBNull.Value) el.PaperTypeDescription = (String)reader["PaperTypeDescription"];
					elements.Add(el);
				}
				reader.Close();
				if (elements.Count != 0) {
					array = new PaperTypeInfo[elements.Count];
					elements.CopyTo(array);
				}
			} catch (Exception e) {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return array;
		}

		/// <summary>
		/// Getting all Extras, related to giving Printing Type.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <returns></returns>
		public Hashtable/*ExtraInfo[]*/ GetExtras(int printingTypeID) {
			SqlCommand cmd = GetLoadExtrasCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlConnection conn = null;
			ExtraInfo[] array = null;
			Hashtable ht;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
//				ArrayList elements = new ArrayList();
				ht = new Hashtable();
				while (reader.Read()) {
					ExtraInfo el = new ExtraInfo();
					el.ExtraID = (int)reader["ExtraID"];
					el.ExtraName = (String)reader["ExtraName"];
//					if (reader["ExtraDescription"] != DBNull.Value) el.ExtraDescription = (String)reader["ExtraDescription"];
					el.PrintingTypeID = (int)reader["PrintingTypeID"];
					ht.Add(el.ExtraID,el);
//					elements.Add(el);
				}
				reader.Close();
				/*if (elements.Count != 0) {
					array = new ExtraInfo[elements.Count];
					elements.CopyTo(array);
				}*/
			} catch (Exception e) {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return ht;
			//return array;
		}
		
		/// <summary>
		/// Getting all Order Quantities, related to giving Printing Type.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <returns></returns>
		public OrderQuantityInfo[] GetOrderQuantities(int printingTypeID) {
			SqlCommand cmd = GetLoadOrderQuantitiesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlConnection conn = null;
			OrderQuantityInfo[] array = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					OrderQuantityInfo el = new OrderQuantityInfo();
					el.Quantity = (int)reader["Quantity"];
					el.PrintingTypeID = (int)reader["PrintingTypeID"];
					if (reader["QuantityDispLabel"] != DBNull.Value)
						el.QuantityDispLabel = (String)reader["QuantityDispLabel"];
					elements.Add(el);
				}
				reader.Close();
				if (elements.Count != 0) {
					array = new OrderQuantityInfo[elements.Count];
					elements.CopyTo(array);
				}
			} catch {
				throw;
			}
			finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return array;
		}
		public PrintingTypePart[] GetPrintingTypeParts(int printingTypeID) {
			SqlCommand cmd = this.GetLoadPrintingTypeParts();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlConnection conn = null;
			PrintingTypePart[] array = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				PrintingTypePart part;
				while (reader.Read()) {
					part = new PrintingTypePart();
					part.PartId = (int)reader["PartID"];;
					part.PartName = (String)reader["PartName"];;
					if (reader["Description"] != DBNull.Value) part.Description = (String)reader["Description"];
					part.PrintingTypeId = (int)reader["PrintingTypeID"];
					elements.Add(part);
				}
				reader.Close();
				if (elements.Count != 0) {
					array = new PrintingTypePart[elements.Count];
					elements.CopyTo(array);
				}
			} catch (Exception e) {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return array;
		}


		private SqlCommand GetRemoveOrderQuantityCommand() {
			if ( removeOrderQuantityCommand == null ) {
				removeOrderQuantityCommand = new SqlCommand("HiResAdmin.RemoveOrderQuantity");
				removeOrderQuantityCommand.CommandType = CommandType.StoredProcedure;
				removeOrderQuantityCommand.Parameters.Add(new SqlParameter(PrintingTypeId, SqlDbType.Int));
				removeOrderQuantityCommand.Parameters.Add(new SqlParameter(Quantity, SqlDbType.Int));
			}
			return removeOrderQuantityCommand;
		}
		#endregion

		public bool DeleteAvailPaperTypes(int printingTypeID, int paperSizeID){
			return DeleteAvailPaperTypes(printingTypeID, paperSizeID, null);
			/*SqlCommand cmd = GetDeleteAvailPaperTypesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[PaperSizeID].Value = paperSizeID;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);*/
		}

		public bool DeleteAvailPaperTypes(int printingTypeID, int paperSizeID, IDbTransaction transaction){
			SqlCommand cmd = GetDeleteAvailPaperTypesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[PaperSizeID].Value = paperSizeID;
			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			//AppLog.LogError("del "+rowsAffected.ToString());
			if (rowsAffected > 0) {
				return true;
			} else {
				return false;
			}
		}

		public PaperTypeInfo[] GetAvailPaperTypes(int printingTypeID) {
			SqlCommand cmd = GetAvailPaperTypesCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			PaperTypeInfo[] array = null;
			SqlConnection conn = null;
			ArrayList list = new ArrayList(2);// FIXME: ?????
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while(reader.Read()) {
					PaperTypeInfo item = new PaperTypeInfo();
					item.PaperTypeID = (int)reader["PaperTypeId"];
					item.PaperTypeName = (String)reader["PaperTypeName"];
					if (reader["PaperTypeDescription"] != DBNull.Value) 
						item.PaperTypeDescription = (String)reader["PaperTypeDescription"];
					list.Add(item);
				}
				reader.Close();
			} catch {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			if (list.Count != 0) {
				array = new PaperTypeInfo[list.Count];
				list.CopyTo(array);
			}
			return array;
		}


		public bool CheckPrintingType(int printingTypeID) {
			SqlCommand cmd = GetCheckPrintingTypeCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters["@valid"].Value = 0;

			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				cmd.ExecuteNonQuery();
			} catch(Exception ex) {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			int	valid = (int)cmd.Parameters["@valid"].Value;
			return (valid == 1);
		}
		
		/// <summary>
		/// Getting PrintingTypeInfo, specified by printingTypeID.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <returns></returns>
		/// 
		public PrintingTypeInfo GetPrintingTypeInfo(int printingTypeID) {
			SqlCommand cmd = GetLoadPrintingTypeCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			PrintingTypeInfo pti = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					pti = new PrintingTypeInfo();
					pti.PrintingTypeID = (int)reader["PrintingTypeID"];
					pti.PrintingTypeName = (String)reader["PrintingTypeName"];
					if (reader["PrintingTypeDescription"] != DBNull.Value) pti.PrintingTypeDescription = (String)reader["PrintingTypeDescription"];
					pti.SiteID = (int)reader["SiteID"];
				}
				reader.Close();
			} catch {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			if (pti != null) {
				pti.PaperSizes = GetPaperSizesPaperTypes(pti.PrintingTypeID);
				pti.Extras = GetExtras(pti.PrintingTypeID);
				pti.OrderQuantities = GetOrderQuantities(pti.PrintingTypeID);
			}
			return pti;
		}
		

		public PrintingTypeInfo[] GetPrintingTypes(int siteId) {
			SqlCommand cmd = GetLoadPrintingTypesCommand()/*GetPrintingTypesCommand()*/;
			cmd.Parameters[SiteId].Value = siteId;
			SqlConnection conn = null;
			PrintingTypeInfo pti;
			PrintingTypeInfo[] PTIs = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					pti = new PrintingTypeInfo();
					pti.PrintingTypeID = (int)reader["PrintingTypeID"];
					pti.PrintingTypeName = (String)reader["PrintingTypeName"];
					if (reader["PrintingTypeDescription"] != DBNull.Value) pti.PrintingTypeDescription = (String)reader["PrintingTypeDescription"];
					pti.SiteID = (int)reader["SiteId"];

					pti.PaperSizes = GetPaperSizesPaperTypes(pti.PrintingTypeID);
					pti.Extras = GetExtras(pti.PrintingTypeID);
					pti.OrderQuantities = GetOrderQuantities(pti.PrintingTypeID);
					pti.Parts = this.GetPrintingTypeParts(pti.PrintingTypeID);

					elements.Add(pti);
				}
				reader.Close();
				if (elements.Count != 0) {
					PTIs = new PrintingTypeInfo[elements.Count];
					elements.CopyTo(PTIs);
				}
			} catch (Exception e) {
				throw;
			} finally {
				if (cmd.Connection!=null) { cmd.Connection = null; }
				if (conn != null) { conn.Close(); }
			}
			return PTIs;
		}

		/// <summary>
		/// Return (id, name) array of Printing Types for specified site.
		/// </summary>
		/// <param name="siteId"></param>
		/// <returns></returns>
		public PrintingTypeElement[] GetPrintingTypesNames(int siteId) {
			SqlCommand cmd = GetPrintingTypesNamesCommand();
			cmd.Parameters[SiteId].Value = siteId;
			SqlConnection conn = null;
			PrintingTypeElement[] PTEs = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					PrintingTypeElement el = new PrintingTypeElement();
					el.PrintingTypeID = (int)reader["PrintingTypeID"];
					el.PrintingTypeName = (String)reader["PrintingTypeName"];
					elements.Add(el);
				}
				reader.Close();
				if (elements.Count != 0) {
					PTEs = new PrintingTypeElement[elements.Count];
					elements.CopyTo(PTEs);
				}
			} catch (Exception e) {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return PTEs;
		}

/*
		public ExtraPrice[] GetExtraPrices(int siteID, int printingTypeID) {
			SqlCommand cmd = GetExtraPricesCommand();
			cmd.Parameters[SiteId].Value = siteID;
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;

			SqlConnection conn = null;
			ExtraPrice[] extraPrices = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList prices = new ArrayList();
				ExtraPrice eprice;
				while (reader.Read()) {
					eprice.ExtraID = (int)reader["ExtraID"];
					eprice.Price = (int)reader["ExtraPrice"];
					eprice.IsSpecial = (bool)reader["IsSpecial"];
					prices.Add(eprice);
				}
				reader.Close();
				if (prices.Count != 0) {
					extraPrices = new ExtraPrice[prices.Count];
					prices.CopyTo(extraPrices);
				}
			} catch (Exception e) {
				// TODO: Consider logging
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return extraPrices;
		}
*/
		[Obsolete("")]
		public Hashtable/*ExtraInfo[]*/ GetExtraPrices(int siteID, int printingTypeID) {
			SqlCommand cmd = GetExtraPricesCommand();
			cmd.Parameters[SiteId].Value = siteID;
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;

			SqlConnection conn = null;
//			ExtraInfo[] extraPrices = null;
			Hashtable extraPrices;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				extraPrices = new Hashtable();
//				ArrayList prices = new ArrayList();
				ExtraInfo einfo;
				while (reader.Read()) {
					einfo = new ExtraInfo();
					einfo.ExtraID = (int)reader["ExtraID"];
//					einfo.ExtraDescription = (String)reader["ExtraDescription"];
					einfo.ExtraName = (string)reader["ExtraName"];;
					einfo.PrintingTypeID = printingTypeID;
					einfo.PriceInfo.SiteID = (int) reader["SiteID"];
					einfo.PriceInfo.Price = (decimal)reader["ExtraPrice"];
					einfo.PriceInfo.IsSpecial = (bool)reader["IsSpecial"];
//					prices.Add(einfo);
					extraPrices.Add(einfo.ExtraID,einfo);
				}
				reader.Close();
/*				if (prices.Count != 0) {
					extraPrices = new ExtraInfo[prices.Count];
					prices.CopyTo(extraPrices);
				}*/
			} catch (Exception e) {
				// TODO: Consider logging
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return extraPrices;
		}

		//TODO: finish
		public Hashtable GetFullExtraPrices (int siteID, int printingTypeID) {
			//throw new NotImplementedException();
			SqlCommand cmd = GetExtraPricesCommand();
			cmd.Parameters[SiteId].Value = siteID;
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;

			SqlConnection conn = null;
			Hashtable extraPrices;
			SqlDataReader reader = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				reader = cmd.ExecuteReader();
				extraPrices = new Hashtable();
				//				ArrayList prices = new ArrayList();
				FullExtraPrice einfo;
				
				while (reader.Read()) {
					einfo = new FullExtraPrice();
					
					einfo.ExtraInfo.ExtraID = (int)reader["ExtraID"];
					//					einfo.ExtraDescription = (String)reader["ExtraDescription"];
					einfo.ExtraInfo.ExtraName = (string)reader["ExtraName"];
					//einfo.ExtraInfo.PrintingTypeID = printingTypeID;
					//einfo.ExtraInfo.Attributes

					einfo.Price = (decimal)reader["ExtraPrice"];
					einfo.IsPricePerM = (bool)reader["IsPricePerM"];
					einfo.IsSpecial = (bool)reader["IsSpecial"];
					try {
						/*ExtraAttribute attr;
						for (int i=1;i<=MAX_EXTRA_ATTR_NUM; i++) {
							ExtraAttributeType attrType = (ExtraAttributeType) reader["Attribute"+i+"Type"];
							attr = new ExtraAttribute(attrType);
							attr.Name = (string)reader["Attribute"+i+"Name"];
							attr.ParseStringValue((string)reader["Attribute"+i+"DefValue"]);
						}*/
					} catch (Exception ex) {
						throw ex;
					}
					//					prices.Add(einfo);
					extraPrices.Add(einfo.ExtraInfo.ExtraID,einfo);
				}
				//reader.Close();
				/*				if (prices.Count != 0) {
									extraPrices = new ExtraInfo[prices.Count];
									prices.CopyTo(extraPrices);
								}*/
			} catch (Exception e) {
				// TODO: Consider logging
				throw;
			} finally {
				if ((reader!=null)&&(!reader.IsClosed)) {
					reader.Close();
				}
				reader.Close();
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return extraPrices;
		}
		/// <summary>
		/// Used for getting prices for specified 
		/// Printing Type depending on given filtering expression.
		/// 
		/// FIXME: should be changed to perform sorting and filtering operations in the body of the appropriate stored procedure
		/// </summary>
		/// <param name="siteID">mandatory parameter</param>
		/// <param name="printingTypeID">mandatory parameter</param>
		/// <returns>Returns filtered prices.</returns>
		public PrintingPrice[] GetPrintingPrices(int siteID, int printingTypeID, 
										FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadPricesCommand();
			cmd.Connection = new SqlConnection(AppConfig.dbConnString);
			cmd.Parameters[SiteId].Value = siteID;
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlDataAdapter sda = new SqlDataAdapter(cmd);
			DataTable table = new DataTable();
			sda.Fill(table);
			cmd.Connection = null;
	
			String sFilter = filter == null ? "" : filter.ToString();
			String sOrderBy = orderBy == null ? "" : orderBy.ToString();
			DataRow[] rows = table.Select(sFilter, sOrderBy);
			PrintingPrice[] prices = new PrintingPrice[rows.Length];
			for (int i=0; i<rows.Length; i++) {
				prices[i].PrintingTypeID = (int)rows[i]["PrintingTypeID"];
				prices[i].PaperTypeID = (int)rows[i]["PaperTypeID"];
				prices[i].PaperTypeName = (String)rows[i]["PaperTypeName"];
				prices[i].PaperSizeID = (int)rows[i]["PaperSizeID"];
				prices[i].PaperSizeName = (String)rows[i]["PaperSizeName"];
				prices[i].Quantity = (int)rows[i]["Quantity"];
				prices[i].Price = /*Decimal.ToDouble(*/(Decimal)rows[i]["Price"]/*)*/;
				prices[i].IsSpecial = (bool)rows[i]["IsSpecial"];
			}
			return prices;
		}

		/// <summary>
		/// FIXME: should be changed to perform sorting and filtering operations in the body of the appropriate stored procedure
		/// </summary>
		/// <param name="siteID">mandatory parameter</param>
		/// <param name="code"></param>
		/// <param name="printingTypeID">mandatory parameter</param>
		/// <returns>Returns filtered prices.</returns>
		public PrintingPrice[] GetWholesalePrices(int siteID, string code, int printingTypeID, 
			FilterExpression filter, OrderExpression orderBy) {
			SqlCommand cmd = GetLoadWholesalePricesCommand();
			cmd.Connection = new SqlConnection(AppConfig.dbConnString);
			cmd.Parameters[SiteId].Value = siteID;
			cmd.Parameters[WholesaleCode].Value = code;
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			SqlDataAdapter sda = new SqlDataAdapter(cmd);
			DataTable table = new DataTable();
			sda.Fill(table);
			cmd.Connection = null;
	
			String sFilter = filter == null ? "" : filter.ToString();
			String sOrderBy = orderBy == null ? "" : orderBy.ToString();
			DataRow[] rows = table.Select(sFilter, sOrderBy);
			PrintingPrice[] prices = new PrintingPrice[rows.Length];
			for (int i=0; i<rows.Length; i++) {
				prices[i].PrintingTypeID = (int)rows[i]["PrintingTypeID"];
				prices[i].PaperTypeID = (int)rows[i]["PaperTypeID"];
				prices[i].PaperTypeName = (String)rows[i]["PaperTypeName"];
				prices[i].PaperSizeID = (int)rows[i]["PaperSizeID"];
				prices[i].PaperSizeName = (String)rows[i]["PaperSizeName"];
				prices[i].Quantity = (int)rows[i]["Quantity"];
				prices[i].Price = /*Decimal.ToDouble(*/(Decimal)rows[i]["Price"]/*)*/;
				prices[i].IsSpecial = (bool)rows[i]["IsSpecial"];
			}
			return prices;
		}
		public PrintingPrice[] GetCommonWholesalePrices(int siteID, int printingTypeID, 
			FilterExpression filter, OrderExpression orderBy) {
			return GetWholesalePrices(siteID, PromoCodeInfo.COMMON_CODE, printingTypeID, filter, orderBy);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="siteID"></param>
		/// <param name="since"></param>
		/// <param name="upTo"></param>
		/// <returns></returns>
		public DateTime[][] GetPrintingShedule(int siteID, DateTime since, DateTime upTo) {
			SqlCommand cmd = GetPrintingSheduleCommand();
			cmd.Parameters[SiteId].Value = siteID;
			cmd.Parameters["@since"].Value = since;
			cmd.Parameters["@upTo"].Value = upTo;
			SqlConnection conn = null;
			DateTime[][] shedule = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					DateTime[] item = new DateTime[3];
					if (reader["PlaceOrder"] != DBNull.Value) 
						item[0] = (DateTime)reader["PlaceOrder"];
					if (reader["ApproveUploadArtwork"] != DBNull.Value) 
						item[1] = (DateTime)reader["ApproveUploadArtwork"];
					if (reader["PickupShip"] != DBNull.Value) 
						item[2] = (DateTime)reader["PickupShip"];
					elements.Add(item);
				}
				reader.Close();
				if (elements.Count != 0) {
					shedule = new DateTime[elements.Count][];
					elements.CopyTo(shedule);
				}
			} catch (Exception e) {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return shedule;
		}

		/// <summary>
		/// Add new printing price 
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="pPrice"></param>
		/// <returns>true, if pPrice was succesfully added, and false otherwise.</returns>
		public bool  AddPrintingPrice(int siteId, PrintingPrice pPrice) {
			SqlCommand cmd = GetAddPrintingPriceCommand();
			cmd.Parameters[PrintingTypeId].Value = pPrice.PrintingTypeID;
			cmd.Parameters[Quantity].Value = pPrice.Quantity;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[PaperSizeID].Value = pPrice.PaperSizeID;
			cmd.Parameters[PaperTypeID].Value = pPrice.PaperTypeID;
			cmd.Parameters[Price].Value = pPrice.Price;
			cmd.Parameters[IsSpecial].Value = pPrice.IsSpecial;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Add new wholesale price 
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="pPrice"></param>
		/// <param name="code"></param>
		/// <returns>true, if price was succesfully added, and false otherwise.</returns>
		public bool  AddWholesalePrice(int siteId, string code, PrintingPrice pPrice) {
			if ((code==null)||(code.Equals(String.Empty))) {
				throw new ArgumentException("code");
			}
			SqlCommand cmd = GetAddWholesalePriceCommand();
			cmd.Parameters[WholesaleCode].Value = code;
			cmd.Parameters[PrintingTypeId].Value = pPrice.PrintingTypeID;
			
			cmd.Parameters[Quantity].Value = pPrice.Quantity;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[PaperSizeID].Value = pPrice.PaperSizeID;
			cmd.Parameters[PaperTypeID].Value = pPrice.PaperTypeID;
			cmd.Parameters[Price].Value = pPrice.Price;
			cmd.Parameters[IsSpecial].Value = pPrice.IsSpecial;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		/// <summary>
		/// Update printing price 
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="pPrice"></param>
		/// <returns>true, if price was succesfully updated or added, and false otherwise.</returns>
		public bool  UpdatePrintingPrice(int siteId, PrintingPrice pPrice) {
			SqlCommand cmd = GetUpdatePrintingPriceCommand();
			cmd.Parameters[PrintingTypeId].Value = pPrice.PrintingTypeID;
			cmd.Parameters[Quantity].Value = pPrice.Quantity;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[PaperSizeID].Value = pPrice.PaperSizeID;
			cmd.Parameters[PaperTypeID].Value = pPrice.PaperTypeID;
			cmd.Parameters[Price].Value = pPrice.Price;
			cmd.Parameters[IsSpecial].Value = pPrice.IsSpecial;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		/// <summary>
		/// Update wholesale price 
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="code"></param>
		/// <param name="pPrice"></param>
		/// <returns>true, if price was succesfully updated or added, and false otherwise.</returns>
		public bool  UpdateWholesalePrice(int siteId, string code, PrintingPrice pPrice) {
			SqlCommand cmd = GetUpdateWSPriceCommand();

			cmd.Parameters[WholesaleCode].Value = code;
			cmd.Parameters[PrintingTypeId].Value = pPrice.PrintingTypeID;
			cmd.Parameters[Quantity].Value = pPrice.Quantity;
			cmd.Parameters[SiteId].Value = siteId;
			cmd.Parameters[PaperSizeID].Value = pPrice.PaperSizeID;
			cmd.Parameters[PaperTypeID].Value = pPrice.PaperTypeID;
			cmd.Parameters[Price].Value = pPrice.Price;
			cmd.Parameters[IsSpecial].Value = pPrice.IsSpecial;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}
		
		public bool  UpdateCommonWholesalePrice(int siteId, PrintingPrice pPrice) {
			return UpdateWholesalePrice(siteId,PromoCodeInfo.COMMON_CODE,pPrice);
		}


		/// <summary>
		/// Getting PaperSizeInfo, specified by paperSizeID, printingTypeID.
		/// </summary>
		/// <param name="printingTypeID">Printing Type ID</param>
		/// <param name="paperSizeID">Paper Type ID</param>
		/// <returns></returns>
		public PaperSizeInfo GetPaperSizeInfo(int printingTypeID, int paperSizeID) {
			SqlCommand cmd = GetLoadPaperSizeCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[PaperSizeID].Value = paperSizeID;
			PaperSizeInfo psi = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					psi = new PaperSizeInfo();
					psi.PaperSizeID = (int)reader["PaperSizeID"];
					psi.PrintingTypeID = (int)reader["PrintingTypeID"];
					psi.PaperSizeName = (String)reader["PaperSizeName"];
					if (reader["PaperSizeDescription"] != DBNull.Value) psi.PaperSizeDescription = (String)reader["PaperSizeDescription"];
				}
				reader.Close();
			} catch {
				throw;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			if (psi != null)
				psi.PaperTypes = GetPaperTypes(printingTypeID, paperSizeID);
			return psi;
		}


		/// <summary>
		/// Getting PapeTypeInfo, specified by paperTypeID.
		/// </summary>
		/// <param name="paperTypeID">Paper Type ID</param>
		/// <returns></returns>
		public PaperTypeInfo GetPaperTypeInfo(int paperTypeID) {
			SqlCommand cmd = GetLoadPaperTypeCommand();
			cmd.Parameters[PaperTypeID].Value = paperTypeID;
			PaperTypeInfo pti = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					pti = new PaperTypeInfo();
					pti.PaperTypeID = (int)reader["PaperTypeID"];
					pti.PaperTypeName = (String)reader["PaperTypeName"];
					if (reader["PaperTypeDescription"] != DBNull.Value) pti.PaperTypeDescription = (String)reader["PaperTypeDescription"];
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return pti;
		}


		public PaperTypeInfo[] GetAllPaperTypes() {
			return GetAllPaperTypes(null, null);
		}
			
			/// <summary>
		/// Getting All existing paper types.
		/// </summary>
		/// <returns></returns>
		public PaperTypeInfo[] GetAllPaperTypes(FilterExpression filter, OrderExpression order) {
			SqlCommand cmd = GetLoadAllPaperTypesCommand();
			cmd.Parameters[Filter].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters[Order].Value = order == null ? "" : order.ToString();
			SqlConnection conn = null;
			PaperTypeInfo pti = null;
			PaperTypeInfo[] pTypeInfos = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					pti = new PaperTypeInfo();
					pti.PaperTypeID = (int)reader["PaperTypeID"];
					pti.PaperTypeName = (String)reader["PaperTypeName"];
					if (reader["PaperTypeDescription"] != DBNull.Value) pti.PaperTypeDescription = (String)reader["PaperTypeDescription"];

					elements.Add(pti);
				}
				reader.Close();
				if (elements.Count != 0) {
					pTypeInfos = new PaperTypeInfo[elements.Count];
					elements.CopyTo(pTypeInfos);
				}
			} catch {
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return pTypeInfos;
		}



		#region Quantities
		public bool RemoveOrderQuantity(int printingTypeID, int quantity) {
			SqlCommand cmd = GetRemoveOrderQuantityCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[Quantity].Value = quantity;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				throw;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		
		public OrderQuantityInfo GetOrderQuantityInfo(int printingTypeID, int quantity) {
			SqlCommand cmd = GetLoadOrderQuantityInfoCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[Quantity].Value = quantity;
			OrderQuantityInfo quant  = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					quant = new OrderQuantityInfo();
					quant.PrintingTypeID = (int)reader["PrintingTypeID"];
					quant.Quantity = (int)reader["Quantity"];
					if (reader["QuantityDispLabel"] != DBNull.Value) quant.QuantityDispLabel = (String)reader["QuantityDispLabel"];
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return quant;
		}


		public bool AddOrderQuantity(OrderQuantityInfo quantityInfo) {
			SqlCommand cmd = GetAddOrderQuantityCommand();
			cmd.Parameters[PrintingTypeId].Value = quantityInfo.PrintingTypeID;
			cmd.Parameters[Quantity].Value = quantityInfo.Quantity;
			cmd.Parameters[QuantityDispLabel].Value = (quantityInfo.QuantityDispLabel == null) ? DBNull.Value : (Object)quantityInfo.QuantityDispLabel;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public bool UpdateOrderQuantity(OrderQuantityInfo quantityInfo) {
			SqlCommand cmd = GetUpdateOrderQuantityCommand();
			cmd.Parameters[PrintingTypeId].Value = quantityInfo.PrintingTypeID;
			cmd.Parameters[Quantity].Value = quantityInfo.Quantity;
			cmd.Parameters[QuantityDispLabel].Value = (quantityInfo.QuantityDispLabel == null) ? DBNull.Value : (Object)quantityInfo.QuantityDispLabel;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		#endregion


		#region extras
		/// <summary>
		/// Getting All existing extras.
		/// </summary>
		/// <returns></returns>
		public FullExtraInfo[] GetAllExtras(FilterExpression filter, OrderExpression order) {
			SqlCommand cmd = GetLoadAllExtrasCommand();
			cmd.Parameters[Filter].Value = filter == null ? "" : filter.ToString();
			cmd.Parameters[Order].Value = order == null ? "" : order.ToString();
			SqlConnection conn = null;
			FullExtraInfo ei = null;
			FullExtraInfo[] extras = null;
			ExtraAttribute eAttr;

			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				ArrayList elements = new ArrayList();
				while (reader.Read()) {
					ei = new FullExtraInfo();
					ei.ExtraID = (int)reader["ExtraID"];
					ei.ExtraName = (String)reader["ExtraName"];
					if (reader["ExtraDescription"] != DBNull.Value) ei.ExtraDescription = (String)reader["ExtraDescription"];

					if (reader["ExtraName"] != DBNull.Value) ei.ExtraName = (String)reader["ExtraName"];
					if (reader["ExtraDescription"] != DBNull.Value) ei.ExtraDescription = (String)reader["ExtraDescription"];

					if (reader["Attribute1Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute1Type"]);
						if (reader["Attribute1Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute1Name"];
						if (reader["Attribute1DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute1DefValue"]);
					}
					ei.Attributes[0] = eAttr;

					if (reader["Attribute2Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute2Type"]);
						if (reader["Attribute2Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute2Name"];
						if (reader["Attribute2DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute2DefValue"]);
					}
					ei.Attributes[1] = eAttr;

					if (reader["Attribute3Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute3Type"]);
						if (reader["Attribute3Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute3Name"];
						if (reader["Attribute3DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute3DefValue"]);
					}
					ei.Attributes[2] = eAttr;

					if (reader["Attribute4Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute4Type"]);
						if (reader["Attribute4Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute4Name"];
						if (reader["Attribute4DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute4DefValue"]);
					}
					ei.Attributes[3] = eAttr;

					elements.Add(ei);
				}
				reader.Close();
				if (elements.Count != 0) {
					extras = new FullExtraInfo[elements.Count];
					elements.CopyTo(extras);
				}
			} catch (Exception ex){
				AppLog.LogError("Error getting extras",ex);
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return extras;
		}

		
		public bool AddExtraInfo(FullExtraInfo ei, out int extraID) {
			SqlCommand cmd = GetAddExtraInfoCommand();
			cmd.Parameters[ExtraID].Value = ei.ExtraID;
			cmd.Parameters[ExtraName].Value = ei.ExtraName;
			cmd.Parameters[ExtraDescription].Value = (ei.ExtraDescription == null) ? DBNull.Value : (Object)ei.ExtraDescription;

			cmd.Parameters[Attribute1Name].Value = (ei.Attributes[0].Name == null) ? DBNull.Value : (Object)ei.Attributes[0].Name;
			cmd.Parameters[Attribute1DefValue].Value = (ei.Attributes[0].Value == null) ? DBNull.Value : (Object)ei.Attributes[0].Value;
			cmd.Parameters[Attribute1Type].Value = ei.Attributes[0].Type;
		
			cmd.Parameters[Attribute2Name].Value = (ei.Attributes[1].Name == null) ? DBNull.Value : (Object)ei.Attributes[1].Name;
			cmd.Parameters[Attribute2DefValue].Value = (ei.Attributes[1].Value == null) ? DBNull.Value : (Object)ei.Attributes[1].Value;
			cmd.Parameters[Attribute2Type].Value = ei.Attributes[1].Type;
		
			cmd.Parameters[Attribute3Name].Value = (ei.Attributes[2].Name == null) ? DBNull.Value : (Object)ei.Attributes[2].Name;
			cmd.Parameters[Attribute3DefValue].Value = (ei.Attributes[2].Value == null) ? DBNull.Value : (Object)ei.Attributes[2].Value;
			cmd.Parameters[Attribute3Type].Value = ei.Attributes[2].Type;
		
			cmd.Parameters[Attribute4Name].Value = (ei.Attributes[3].Name == null) ? DBNull.Value : (Object)ei.Attributes[2].Name;
			cmd.Parameters[Attribute4DefValue].Value = (ei.Attributes[3].Value == null) ? DBNull.Value : (Object)ei.Attributes[2].Value;
			cmd.Parameters[Attribute4Type].Value = ei.Attributes[3].Type;
		
			int rowsAffected = 0;
			extraID = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				extraID = (int)cmd.Parameters[ExtraID].Value;
			} catch (Exception ex)	{
				AppLog.LogError("Error while adding extra info",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public bool UpdateExtraInfo(FullExtraInfo ei) {
			SqlCommand cmd = GetUpdateExtraInfoCommand();
			cmd.Parameters[ExtraID].Value = ei.ExtraID;
			cmd.Parameters[ExtraName].Value = ei.ExtraName;
			cmd.Parameters[ExtraDescription].Value = (ei.ExtraDescription == null) ? DBNull.Value : (Object)ei.ExtraDescription;

			cmd.Parameters[Attribute1Type].Value = (int)ei.Attributes[0].Type;
			cmd.Parameters[Attribute1Name].Value = (ei.Attributes[0].Name == null) ? DBNull.Value : (Object)ei.Attributes[0].Name;
			cmd.Parameters[Attribute1DefValue].Value = (ei.Attributes[0].Value == null) ? DBNull.Value : (Object)ei.Attributes[0].Value;
		
			cmd.Parameters[Attribute2Type].Value = (int)ei.Attributes[1].Type;
			cmd.Parameters[Attribute2Name].Value = (ei.Attributes[1].Name == null) ? DBNull.Value : (Object)ei.Attributes[1].Name;
			cmd.Parameters[Attribute2DefValue].Value = (ei.Attributes[1].Value == null) ? DBNull.Value : (Object)ei.Attributes[1].Value;
		
			cmd.Parameters[Attribute3Type].Value = (int)ei.Attributes[2].Type;
			cmd.Parameters[Attribute3Name].Value = (ei.Attributes[2].Name == null) ? DBNull.Value : (Object)ei.Attributes[2].Name;
			cmd.Parameters[Attribute3DefValue].Value = (ei.Attributes[2].Value == null) ? DBNull.Value : (Object)ei.Attributes[2].Value;
		
			cmd.Parameters[Attribute4Type].Value = (int)ei.Attributes[3].Type;
			cmd.Parameters[Attribute4Name].Value = (ei.Attributes[3].Name == null) ? DBNull.Value : (Object)ei.Attributes[3].Name;
			cmd.Parameters[Attribute4DefValue].Value = (ei.Attributes[3].Value == null) ? DBNull.Value : (Object)ei.Attributes[3].Value;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch (Exception ex)	{
				AppLog.LogError("Error while updating extra info",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public FullExtraInfo GetExtraInfo(int extraID) {
			SqlCommand cmd = GetLoadExtraInfoCommand();
			cmd.Parameters[ExtraID].Value = extraID;

			FullExtraInfo ei = null;
			ExtraAttribute eAttr = new ExtraAttribute();
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					ei = new FullExtraInfo();
					ei.ExtraName = (String)reader["ExtraName"];
					if (reader["ExtraDescription"] != DBNull.Value) ei.ExtraDescription = (String)reader["ExtraDescription"];

					if (reader["ExtraName"] != DBNull.Value) ei.ExtraName = (String)reader["ExtraName"];
					if (reader["ExtraDescription"] != DBNull.Value) ei.ExtraDescription = (String)reader["ExtraDescription"];

					if (reader["Attribute1Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute1Type"]);
						if (reader["Attribute1Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute1Name"];
						if (reader["Attribute1DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute1DefValue"]);
					}
					ei.Attributes[0] = eAttr;

					if (reader["Attribute2Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute2Type"]);
						if (reader["Attribute2Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute2Name"];
						if (reader["Attribute2DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute2DefValue"]);
					}
					ei.Attributes[1] = eAttr;

					if (reader["Attribute3Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute3Type"]);
						if (reader["Attribute3Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute3Name"];
						if (reader["Attribute3DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute3DefValue"]);
					}
					ei.Attributes[2] = eAttr;

					if (reader["Attribute4Type"] == DBNull.Value) 
						eAttr = new ExtraAttribute(ExtraAttributeType.None);
					else {
						eAttr = new ExtraAttribute((ExtraAttributeType)reader["Attribute4Type"]);
						if (reader["Attribute4Name"] != DBNull.Value) eAttr.Name = (String)reader["Attribute4Name"];
						if (reader["Attribute4DefValue"] != DBNull.Value) 
							eAttr.ParseStringValue((string)reader["Attribute4DefValue"]);
					}
					ei.Attributes[3] = eAttr;

				}
				reader.Close();
			} catch (Exception ex){
				AppLog.LogError("error while GetExtraInfo",ex);
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return ei;
		}


		public bool DeleteAvailableExtras(int printingTypeID, IDbTransaction transaction){
			SqlCommand cmd = GetDeleteAvailExtrasCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			if (rowsAffected > 0) {
				return true;
			} else {
				return false;
			}
		}


		public bool SetAvailableExtras (int printingTypeId, Hashtable extras, IDbTransaction transaction) {
			if (extras==null) {
				//no extras selected
				return true;
			}
			if (extras.Count==0) {
				//no extras selected
				return true;
			}
			SqlCommand cmd = GetSetAvailExtrasCommand();
			
			StringBuilder sb = new StringBuilder(255);
			int i = 0;
			IDictionaryEnumerator eiEnumerator = extras.GetEnumerator();
			while(eiEnumerator.MoveNext()) {
				if (i>0) sb.Append(",");
				sb.Append(eiEnumerator.Key);
				i++;
			}

			#region params
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters["@paramString"].Value = ((sb.ToString()==null)||(sb.ToString()==String.Empty))?DBNull.Value:(object)sb.ToString();
			#endregion 

			IDbConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = (SqlConnection)(transaction.Connection);
					cmd.Transaction = (SqlTransaction)transaction;
				}
				cmd.Connection = (SqlConnection)conn;
				int rowsAffected = cmd.ExecuteNonQuery(); 
				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}
			} catch (Exception ex) {
				AppLog.LogError("Error in SetAvailableExtras", ex);
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
		}

		
		
		#endregion


		#region PrintingTypeParts
		public PrintingTypePart GetPrintingTypePart(int printingTypeID, int partID) {
			SqlCommand cmd = GetLoadPrintingTypePartInfo();
			cmd.Parameters[PrintingTypeId].Value = printingTypeID;
			cmd.Parameters[PartID].Value = partID;
			PrintingTypePart part  = null;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
				if(reader.Read()) {
					part = new PrintingTypePart();
					part.PrintingTypeId = (int)reader["PrintingTypeID"];
					part.PartId = (int)reader["PartID"];
					if (reader["PartName"] != DBNull.Value) part.PartName = (String)reader["PartName"];
					if (reader["Description"] != DBNull.Value) part.Description = (String)reader["Description"];
				}
				reader.Close();
			} catch {
				return null;
			} finally {
				cmd.Connection = null;
				if (conn != null) conn.Close();
			}
			return part;
		}


		public bool RemovePrintingTypePart(int partID) {
			SqlCommand cmd = GetRemovePrintingTypePart();
			cmd.Parameters[PartID].Value = partID;
			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		#endregion


		#region Add, Update, Delete printings functions
		public bool AddPrintingType(ref PrintingTypeInfo pTypeInfo) {
			/*SqlCommand cmd = GetAddPrintingTypeCommand();
			cmd.Parameters[PrintingTypeName].Value = (pTypeInfo.PrintingTypeName == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeName;
			cmd.Parameters[PrintingTypeDescription].Value = (pTypeInfo.PrintingTypeDescription == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeDescription;
			cmd.Parameters[SiteId].Value = pTypeInfo.SiteID;
			cmd.Parameters[PrintingTypeId].Value = pTypeInfo.PrintingTypeID;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				pTypeInfo.PrintingTypeID = (int)cmd.Parameters[PrintingTypeId].Value;
			} catch (Exception ex)	{
				AppLog.LogError("Error while adding printing type",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
			*/
			return AddPrintingType(ref pTypeInfo, null);
		}

		public bool AddPrintingType(ref PrintingTypeInfo pTypeInfo, IDbTransaction transaction) {
			SqlCommand cmd = GetAddPrintingTypeCommand();
			cmd.Parameters[PrintingTypeName].Value = (pTypeInfo.PrintingTypeName == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeName;
			cmd.Parameters[PrintingTypeDescription].Value = (pTypeInfo.PrintingTypeDescription == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeDescription;
			cmd.Parameters[SiteId].Value = pTypeInfo.SiteID;
			cmd.Parameters[PrintingTypeId].Value = pTypeInfo.PrintingTypeID;

			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			pTypeInfo.PrintingTypeID = (int)cmd.Parameters[PrintingTypeId].Value;
			if (rowsAffected > 0) {
				return SetAvailableExtras(pTypeInfo.PrintingTypeID, pTypeInfo.Extras, transaction);
			} else {
				return false;
			}
		}

		
		public bool AddPaperSize(int printingTypeId, ref PaperSizeInfo pSizeInfo) {
			return AddPaperSize(printingTypeId, ref pSizeInfo, null);
			/*SqlCommand cmd = GetAddPaperSizeCommand();
			cmd.Parameters[PaperSizeName].Value = (pSizeInfo.PaperSizeName == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeName;
			cmd.Parameters[PaperSizeDescription].Value = (pSizeInfo.PaperSizeDescription == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeDescription;
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[PaperSizeID].Value = pSizeInfo.PaperSizeID;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				pSizeInfo.PaperSizeID = (int)cmd.Parameters[PaperSizeID].Value;
			} catch	(Exception ex){
				AppLog.LogError("Error while adding paper size",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
			*/
		}

		public bool AddPaperSize(int printingTypeId, ref PaperSizeInfo pSizeInfo, IDbTransaction transaction) {
			SqlCommand cmd = GetAddPaperSizeCommand();
			cmd.Parameters[PaperSizeName].Value = (pSizeInfo.PaperSizeName == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeName;
			cmd.Parameters[PaperSizeDescription].Value = (pSizeInfo.PaperSizeDescription == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeDescription;
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[PaperSizeID].Value = pSizeInfo.PaperSizeID;

			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			pSizeInfo.PaperSizeID = (int)cmd.Parameters[PaperSizeID].Value;
			if (rowsAffected > 0) {
				return SetAvailablePaperTypes(printingTypeId,pSizeInfo.PaperSizeID, pSizeInfo.PaperTypes, transaction);
			} else {
				return false;
			}
		}


		public bool RemovePaperSize(int paperSizeID, int printingTypeID) {
			return RemovePaperSize(paperSizeID, printingTypeID, null);	
		}

		public bool RemovePaperSize(int paperSizeID, int printingTypeID, IDbTransaction transaction) {
			SqlCommand cmd = GetRemovePaperSizeCommand();
			cmd.Parameters[PaperSizeID].Value = paperSizeID;

			DeleteAvailPaperTypes(printingTypeID, paperSizeID, transaction);
			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			if (rowsAffected > 0) {
				return true;
			} else {
				return false;
			}
		}
		
		
		public bool AddPaperType(ref PaperTypeInfo pTypeInfo) {
			SqlCommand cmd = GetAddPaperTypeCommand();
			cmd.Parameters[PaperTypeName].Value = (pTypeInfo.PaperTypeName == null) ? DBNull.Value : (Object)pTypeInfo.PaperTypeName;
			cmd.Parameters[PaperTypeDescription].Value = (pTypeInfo.PaperTypeDescription == null) ? DBNull.Value : (Object)pTypeInfo.PaperTypeDescription;
			cmd.Parameters[PaperTypeID].Value = pTypeInfo.PaperTypeID;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				pTypeInfo.PaperTypeID = (int)cmd.Parameters[PaperTypeID].Value;
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public bool AddPaperTypePart(int printingTypeId, ref PrintingTypePart pTypePart) {
			SqlCommand cmd = GetAddPaperTypePartCommand();
			cmd.Parameters[PartName].Value = (pTypePart.PartName == null) ? DBNull.Value : (Object)pTypePart.PartName;
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[Description].Value = (pTypePart.Description == null) ? DBNull.Value : (Object)pTypePart.Description;
			cmd.Parameters[PartID].Value = pTypePart.PartId;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
				pTypePart.PartId = (int)cmd.Parameters[PartID].Value;
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		
		public bool UpdatePrintingType(PrintingTypeInfo pTypeInfo) {
			/*SqlCommand cmd = GetUpdatePrintingTypeCommand();
			cmd.Parameters[PrintingTypeName].Value = (pTypeInfo.PrintingTypeName == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeName;
			cmd.Parameters[PrintingTypeDescription].Value = (pTypeInfo.PrintingTypeDescription == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeDescription;
			cmd.Parameters[SiteId].Value = pTypeInfo.SiteID;
			cmd.Parameters[PrintingTypeId].Value = pTypeInfo.PrintingTypeID;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	(Exception ex){
				AppLog.LogError("Error while updating printing type",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
			*/
			return UpdatePrintingType(pTypeInfo, null);
		}

		public bool UpdatePrintingType(PrintingTypeInfo pTypeInfo, IDbTransaction transaction) {
			SqlCommand cmd = GetUpdatePrintingTypeCommand();
			cmd.Parameters[PrintingTypeName].Value = (pTypeInfo.PrintingTypeName == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeName;
			cmd.Parameters[PrintingTypeDescription].Value = (pTypeInfo.PrintingTypeDescription == null) ? DBNull.Value : (Object)pTypeInfo.PrintingTypeDescription;
			cmd.Parameters[SiteId].Value = pTypeInfo.SiteID;
			cmd.Parameters[PrintingTypeId].Value = pTypeInfo.PrintingTypeID;
			
			DeleteAvailableExtras(pTypeInfo.PrintingTypeID, transaction);
			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			if (rowsAffected > 0) {
				return SetAvailableExtras(pTypeInfo.PrintingTypeID, pTypeInfo.Extras, transaction);
			} else {
				return false;
			}
		}

		
		public bool UpdatePaperSize(int printingTypeId, PaperSizeInfo pSizeInfo) {
			/*SqlCommand cmd = GetUpdatePaperSizeCommand();
			cmd.Parameters[PaperSizeName].Value = (pSizeInfo.PaperSizeName == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeName;
			cmd.Parameters[PaperSizeDescription].Value = (pSizeInfo.PaperSizeDescription == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeDescription;
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[PaperSizeID].Value = pSizeInfo.PaperSizeID;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
			*/
			return UpdatePaperSize(printingTypeId, pSizeInfo, null);
		}

		public bool UpdatePaperSize(int printingTypeId, PaperSizeInfo pSizeInfo, IDbTransaction transaction) {
			SqlCommand cmd = GetUpdatePaperSizeCommand();
			cmd.Parameters[PaperSizeName].Value = (pSizeInfo.PaperSizeName == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeName;
			cmd.Parameters[PaperSizeDescription].Value = (pSizeInfo.PaperSizeDescription == null) ? DBNull.Value : (Object)pSizeInfo.PaperSizeDescription;
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[PaperSizeID].Value = pSizeInfo.PaperSizeID;

			int rowsAffected = DBHelper.ExecuteSafeNonQueryTransactional(cmd,(SqlTransaction)transaction);
			if (rowsAffected > 0) {
				//
				DeleteAvailPaperTypes(printingTypeId,pSizeInfo.PaperSizeID);
				return SetAvailablePaperTypes(printingTypeId,pSizeInfo.PaperSizeID, pSizeInfo.PaperTypes, transaction);
			} else {
				return false;
			}
		}


		
		public bool UpdatePaperType(PaperTypeInfo pTypeInfo) {
			SqlCommand cmd = GetUpdatePaperTypeCommand();
			cmd.Parameters[PaperTypeName].Value = (pTypeInfo.PaperTypeName == null) ? DBNull.Value : (Object)pTypeInfo.PaperTypeName;
			cmd.Parameters[PaperTypeDescription].Value = (pTypeInfo.PaperTypeDescription == null) ? DBNull.Value : (Object)pTypeInfo.PaperTypeDescription;
			cmd.Parameters[PaperTypeID].Value = pTypeInfo.PaperTypeID;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	(Exception ex){
				AppLog.LogError("Error while updating paper type",ex);
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}


		public bool UpdatePaperTypePart(int printingTypeId, PrintingTypePart pTypePart) {
			SqlCommand cmd = GetUpdatePaperTypePartCommand();
			cmd.Parameters[PartName].Value = (pTypePart.PartName == null) ? DBNull.Value : (Object)pTypePart.PartName;
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[Description].Value = (pTypePart.Description == null) ? DBNull.Value : (Object)pTypePart.Description;
			cmd.Parameters[PartID].Value = pTypePart.PartId;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		
		public bool AllowPaperType(int printingTypeId, int paperSizeId, int paperTypeId) {
			SqlCommand cmd = GetAllowPaperTypeCommand();
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[PaperSizeID].Value = paperSizeId;
			cmd.Parameters[PaperTypeID].Value = paperTypeId;

			int rowsAffected = 0;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(AppConfig.dbConnString);
				cmd.Connection = conn;
				conn.Open();
				rowsAffected = cmd.ExecuteNonQuery();
			} catch	{
				return false;
			} finally {
				if (conn != null) conn.Close();
			}
			return (rowsAffected > 0);
		}

		
		public bool SetAvailablePaperTypes (int printingTypeId, int paperSizeId, PaperTypeInfo[] paperTypes, IDbTransaction transaction) {
			if (paperTypes==null) {
				//no paper type selected
				return true;
			}
			if (paperTypes.Length==0) {
				//no paper type selected
				return true;
			}
			SqlCommand cmd = GetSetAvailPaperTypeCommand();
			
			StringBuilder sb = new StringBuilder(255);
			int i = 0;
			foreach(PaperTypeInfo ptInfo in paperTypes) {
				if (i>0) sb.Append(",");
				sb.Append(ptInfo.PaperTypeID);
				i++;
			}

			#region params
			cmd.Parameters[PrintingTypeId].Value = printingTypeId;
			cmd.Parameters[PaperSizeID].Value = paperSizeId;
			cmd.Parameters["@paramString"].Value = ((sb.ToString()==null)||(sb.ToString()==String.Empty))?DBNull.Value:(object)sb.ToString();
			#endregion 

			IDbConnection conn = null;

			try {
				if (transaction==null) {
					conn = new SqlConnection(AppConfig.dbConnString);
					conn.Open();
				} else {
					conn = (SqlConnection)(transaction.Connection);
					cmd.Transaction = (SqlTransaction)transaction;
				}
				cmd.Connection = (SqlConnection)conn;
				int rowsAffected = cmd.ExecuteNonQuery(); 
				if (rowsAffected > 0) {
					return true;
				} else {
					return false;
				}
			} catch (Exception ex) {
				AppLog.LogError("Error in SetAvailablePaperTypes", ex);
				return false;
			} finally {
				cmd.Connection = null;
				if (transaction==null) {
					if (conn != null) conn.Close();
				}
			}
		}

		#endregion
		
		
		
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion
	}

}
