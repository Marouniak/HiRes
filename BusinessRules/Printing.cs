/*
 * FILE:		Printing.cs
 * PROJECT:		HiRes PrintingsSite
 * ABSTRACT:	Business Rules Layer - Printings- and prices- related operations
 * LEGAL:		Copyright (c) Eurosoft International Inc., 2001
 * 
 * Revision history:
 * 
 * 25-Apr-2002 Gleb Novodran
 * Initial implementation
 * 
 */

using System;
using System.Collections;
using HiRes.Common;
using HiRes.DAL;
using System.Data;
using HiRes.SystemFramework.Logging;


namespace HiRes.BusinessRules {
	
	/// <summary>
	/// Bussiness rules for Printings-related operations
	/// </summary>
	public class Printing {
		
		public PrintingTypeInfo GetPrintingTypeInfo(int printingTypeId) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPrintingTypeInfo(printingTypeId);
			}			
		}
		
		public PaperSizeInfo GetPaperSizeInfo(int printingTypeId, int paperSizeId) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPaperSizeInfo(printingTypeId, paperSizeId);
			}			
		}
		

		public PaperTypeInfo GetPaperTypeInfo(int paperTypeId) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPaperTypeInfo(paperTypeId);
			}			
		}
		
		public PrintingTypeInfo[] GetPrintingTypes(int siteId) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPrintingTypes(siteId);
			}			
		}

		public PrintingTypeElement[] GetPrintingTypesNames(int siteId) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPrintingTypesNames(siteId);
			}			
		}

		public PaperSizeInfo[] GetPaperSizes(int siteId, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPaperSizesPaperTypes(/*siteId,*/printingTypeID);
			}	
		}

		public PaperSizeInfo[] GetPrintingPaperSizes(int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPaperSizes(printingTypeID);
			}	
		}


		public PaperTypeInfo[] GetPaperTypes(int siteId, int printingTypeID, int paperSizeId) {
			//throw new Exception("Method not implemented.");
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPaperTypes(/*siteId,*/printingTypeID, paperSizeId);
			}	
		}
		
		public Hashtable/*ExtraInfo[]*/ GetExtras(int siteId, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetExtras(/*siteId,*/printingTypeID);
			}	
		}

		public OrderQuantityInfo[] GetOrderQuantities(int siteId, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetOrderQuantities(/*siteId,*/printingTypeID);
			}	
		}
		
		public PrintingTypePart[] GetPrintingTypeParts(int siteId, int printingTypeID) {
			PrintingTypePart[] parts ;

			using (PrintingsDAL PDal= new PrintingsDAL()) {
				parts =  PDal.GetPrintingTypeParts(printingTypeID);
			}
			return parts;
		}
		
		[Obsolete("")]
		public Hashtable/*ExtraInfo[]*/ GetExtraPrices (int siteId, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetExtraPrices(siteId, printingTypeID);
			}	
			
		}

		public Hashtable GetFullExtraPrices (int siteId, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetFullExtraPrices(siteId, printingTypeID);
			}	
			
		}

		
		public PrintingPrice[] GetPrices(int siteId, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPrintingPrices(siteId, printingTypeID, null, null);
			}	

		}

		public PrintingPrice[] GetPrices(int siteId, int printingTypeID, int paperTypeID, int paperSizeID, int quantity) {

			return GetPrices(siteId, printingTypeID, paperTypeID, paperSizeID, quantity, null);
		}

		public PrintingPrice[] GetPrices(int siteId, int printingTypeID, int paperTypeID, int paperSizeID, int quantity, OrderExpression orderBy) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				//FIXME: filtering and sorting parameters need to be set according to parameters acquired by this method
				FilterExpression fexp = null;
				if ((paperTypeID!=PrintingPrice.ALL_ITEMS)||(paperSizeID!=PrintingPrice.ALL_ITEMS)||(quantity!=PrintingPrice.ALL_ITEMS)) {
					fexp = new FilterExpression(typeof(PriceListFields));
					if (paperTypeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.PaperSizeID,paperSizeID);
						//fexp[(int)PriceListFields.PaperSizeID] = paperSizeID;
					}
					if (paperSizeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.PaperTypeID,paperTypeID);
						//fexp[(int)PriceListFields.PaperTypeID] = paperTypeID;
					}
					if (quantity!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.Quantity,quantity);
						//fexp[(int)PriceListFields.Quantity] = quantity;
					}
				}
				return printingsDAL.GetPrintingPrices(siteId, printingTypeID, fexp, orderBy);
			}	
		}


		#region wholesale prices getters
		[Obsolete("",true)]
		public PrintingPrice[] GetWholesalePrices(int siteId, string code, int printingTypeID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetWholesalePrices(siteId, code, printingTypeID, null, null);
				/*
				if (code!=null) {
					return printingsDAL.GetWholesalePrices(siteId, code, printingTypeID, null, null);
				} else { //"common" code
					return printingsDAL.GetWholesalePrices(siteId, printingTypeID, null, null);					
				}
				 */
			}	

		}

		public PrintingPrice[] GetWholesalePrices(int siteId, string code, int printingTypeID, int paperTypeID, int paperSizeID, int quantity) {

			return GetWholesalePrices(siteId, code, printingTypeID, paperTypeID, paperSizeID, quantity, null);
		}

		public PrintingPrice[] GetWholesalePrices(int siteId, string code, int printingTypeID, int paperTypeID, int paperSizeID, int quantity, OrderExpression orderBy) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				//FIXME: filtering and sorting parameters need to be set according to parameters acquired by this method
				FilterExpression fexp = null;
				if ((paperTypeID!=PrintingPrice.ALL_ITEMS)||(paperSizeID!=PrintingPrice.ALL_ITEMS)||(quantity!=PrintingPrice.ALL_ITEMS)) {
					fexp = new FilterExpression(typeof(PriceListFields));
					if (paperTypeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.PaperSizeID,paperSizeID);
						//fexp[(int)PriceListFields.PaperSizeID] = paperSizeID;
					}
					if (paperSizeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.PaperTypeID,paperTypeID);
						//fexp[(int)PriceListFields.PaperTypeID] = paperTypeID;
					}
					if (quantity!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.Quantity,quantity);
						//fexp[(int)PriceListFields.Quantity] = quantity;
					}
				}

				return printingsDAL.GetWholesalePrices(siteId, code, printingTypeID, fexp, orderBy);
			}	
		}
		

		public PrintingPrice[] GetCommonWholesalePrices(int siteId, int printingTypeID, int paperTypeID, int paperSizeID, int quantity) {

			return GetCommonWholesalePrices(siteId, printingTypeID, paperTypeID, paperSizeID, quantity, null);
		}
		public PrintingPrice[] GetCommonWholesalePrices(int siteId, int printingTypeID, int paperTypeID, int paperSizeID, int quantity, OrderExpression orderBy) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				//FIXME: filtering and sorting parameters need to be set according to parameters acquired by this method
				FilterExpression fexp = null;
				if ((paperTypeID!=PrintingPrice.ALL_ITEMS)||(paperSizeID!=PrintingPrice.ALL_ITEMS)||(quantity!=PrintingPrice.ALL_ITEMS)) {
					fexp = new FilterExpression(typeof(PriceListFields));
					if (paperTypeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.PaperSizeID,paperSizeID);
						//fexp[(int)PriceListFields.PaperSizeID] = paperSizeID;
					}
					if (paperSizeID!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.PaperTypeID,paperTypeID);
						//fexp[(int)PriceListFields.PaperTypeID] = paperTypeID;
					}
					if (quantity!=PrintingPrice.ALL_ITEMS) {
						fexp.Add(PriceListFields.Quantity,quantity);
						//fexp[(int)PriceListFields.Quantity] = quantity;
					}
				}

				return printingsDAL.GetCommonWholesalePrices(siteId, printingTypeID, fexp, orderBy);
			}	
		}
		#endregion

		public DateTime[][] GetPrintingShedule(int siteID, DateTime since, DateTime upTo) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPrintingShedule(siteID, since, upTo);
			}	
		}
		
/*		public void PrintinCreateEmptyDesignParts(ref OrderInfo orderInfo) {

		}*/

		public bool AddPrintingPrice(int siteID, PrintingPrice pPrice) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.AddPrintingPrice(siteID, pPrice);
			}
			return res;
		}

		public bool UpdatePrintingPrice(int siteID, PrintingPrice pPrice) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.UpdatePrintingPrice(siteID, pPrice);
			}
			return res;
		}

		
		public PaperTypeInfo[] GetAllPaperTypes() {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetAllPaperTypes();
			}	
		}
		

		public PaperTypeInfo[] GetAllPaperTypes(FilterExpression filter, OrderExpression order) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetAllPaperTypes(filter, order);
			}	
		}

		
		#region PrintingTypeParts
		public PrintingTypePart GetPrintingTypePartInfo(int printingTypeID, int partID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetPrintingTypePart(printingTypeID, partID);
			}	
		}

		public bool RemovePrintingTypePart(int partID) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.RemovePrintingTypePart(partID);
			}	
		}

		#endregion

		
		#region quantities
		
		public bool RemoveOrderQuantity(int printingTypeID, int quantity) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.RemoveOrderQuantity(printingTypeID, quantity);
			}	
		}
		
		public OrderQuantityInfo GetOrderQuantityInfo(int printingTypeID, int quantity) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetOrderQuantityInfo(printingTypeID, quantity);
			}	
		}


		public bool AddOrderQuantity(OrderQuantityInfo quantityInfo) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.AddOrderQuantity(quantityInfo);
			}	
		}
		

		public bool UpdateOrderQuantity(OrderQuantityInfo quantityInfo) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.UpdateOrderQuantity(quantityInfo);
			}	
		}
		
		
		#endregion


		#region add, update wholesale prices

		public bool AddWholesalePrice(int siteID, string code, PrintingPrice pPrice) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.AddWholesalePrice(siteID, code, pPrice);
			}
			return res;
		}

		public bool UpdateWholesalePrice(int siteID, string code, PrintingPrice pPrice) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.UpdateWholesalePrice(siteID, code, pPrice);
			}
			return res;
		}

		public bool UpdateCommonWholesalePrice(int siteID, PrintingPrice pPrice) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.UpdateCommonWholesalePrice(siteID, pPrice);
			}
			return res;
		}

		#endregion

		#region extras

		public FullExtraInfo[] GetAllExtras(FilterExpression filter, OrderExpression order) {
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				return printingsDAL.GetAllExtras(filter, order);
			}	
		}

		public FullExtraInfo GetExtraInfo(int extraID) {
			using (PrintingsDAL printingDAL = new PrintingsDAL()) {
				return printingDAL.GetExtraInfo(extraID);
			}
		}


		
		public bool AddExtraInfo(FullExtraInfo ei, out int extraID) {
			using (PrintingsDAL printingDAL = new PrintingsDAL()) {
				return printingDAL.AddExtraInfo(ei,out extraID);
			}
		}


		public bool UpdateExtraInfo(FullExtraInfo ei) {
			using (PrintingsDAL printingDAL = new PrintingsDAL()) {
				return printingDAL.UpdateExtraInfo(ei);
			}
		}

		#endregion
	
	
		#region add, update, remove printings

		public bool CheckPrintingType(int printingTypeID) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.CheckPrintingType(printingTypeID);
			}
			return res;
		}

		public bool AddPrintingType(PrintingTypeInfo pTypeInfo) {
			/*bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.AddPrintingType(ref pTypeInfo);
			}
			return res;
			*/
			bool res = false;
			using (IDbTransaction dbTrans = DbTransactionFactory.BeginTransaction()) {
				using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
					res = printingsDAL.AddPrintingType(ref pTypeInfo, dbTrans);
				}

				if (!res) {
					dbTrans.Rollback();
					AppLog.LogError("Attention! Printing type info was not stored to the db");
					return false;
				} else {
					dbTrans.Commit();
					return true;
				}
			}
		
		}


		public bool UpdatePrintingType(PrintingTypeInfo pTypeInfo) {
			/*bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.UpdatePrintingType(pTypeInfo);
			}
			return res;
			*/
			bool res = false;
			using (IDbTransaction dbTrans = DbTransactionFactory.BeginTransaction()) {
				using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
					res = printingsDAL.UpdatePrintingType(pTypeInfo, dbTrans);
				}
				if (!res) {
					dbTrans.Rollback();
					AppLog.LogError("Attention! Printing type info was not stored to the db");
					return false;
				} else {
					dbTrans.Commit();
					return true;
				}
			}
		}


		public bool AddPaperSize(int printingTypeId, PaperSizeInfo pSizeInfo) {
			bool res = false;
			using (IDbTransaction dbTrans = DbTransactionFactory.BeginTransaction()) {
				using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
					res = printingsDAL.AddPaperSize(printingTypeId, ref pSizeInfo, dbTrans);
				}

				if (!res) {
					dbTrans.Rollback();
					AppLog.LogError("Attention! Paper size info was not stored to the db");
					return false;
				} else {
					dbTrans.Commit();
					return true;
				}
			}
		}


		public bool UpdatePaperSize(int printingTypeId, PaperSizeInfo pSizeInfo) {
			bool res = false;
			using (IDbTransaction dbTrans = DbTransactionFactory.BeginTransaction()) {
				using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
					res = printingsDAL.UpdatePaperSize(printingTypeId,pSizeInfo, dbTrans);
				}

				if (!res) {
					dbTrans.Rollback();
					AppLog.LogError("Attention! Paper size info was not stored to the db");
					return false;
				} else {
					dbTrans.Commit();
					return true;
				}
			}
		}


		public bool RemovePaperSize(int paperSizeID, int printingTypeId) {
			bool res = false;
			using (IDbTransaction dbTrans = DbTransactionFactory.BeginTransaction()) {
				using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
					res = printingsDAL.RemovePaperSize(paperSizeID, printingTypeId, dbTrans);
				}
				if (!res) {
					dbTrans.Rollback();
					AppLog.LogError("Attention! Paper size info was not deleted from db");
					return false;
				} else {
					dbTrans.Commit();
					return true;
				}
			}
		}



		public bool AllowPaperType(int printingTypeId, int paperSizeId, int paperTypeId) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.AllowPaperType(printingTypeId, paperSizeId, paperTypeId);
			}
			return res;
		}


		public bool AddPaperType(PaperTypeInfo pTypeInfo) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.AddPaperType(ref pTypeInfo);
			}
			return res;
		}


		public bool UpdatePaperType(PaperTypeInfo pTypeInfo) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.UpdatePaperType(pTypeInfo);
			}
			return res;
		}


		public bool AddPaperTypePart(int printingTypeId, PrintingTypePart pTypePart) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.AddPaperTypePart(printingTypeId, ref pTypePart);
			}
			return res;
		}


		public bool UpdatePaperTypePart(int printingTypeId, PrintingTypePart pTypePart) {
			bool res;
			using (PrintingsDAL printingsDAL = new PrintingsDAL()) {
				res = printingsDAL.UpdatePaperTypePart(printingTypeId, pTypePart);
			}
			return res;
		}

		#endregion
	}
}
