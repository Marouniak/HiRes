namespace HiRes.BusinessFacade {
	
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using HiRes.BusinessRules;
	using HiRes.Common;
	using HiRes.SystemFramework;
	/// <summary>
	/// This class incapsulates all the operations related to Printing Types and Prices
	/// </summary>
	/// <remarks>In the future, when several ordering sites will work with the same backend
	/// will get site ID from web.config and pass it as a param to BussinessRules layer class method 
	/// </remarks>
	public class PrintingsFacade {
		
		public PrintingTypeInfo GetPrintingTypeInfo(int printingTypeID) {
			return (new BusinessRules.Printing()).GetPrintingTypeInfo(printingTypeID/*AppConfig.siteId*/);
		}

		public PaperSizeInfo GetPaperSizeInfo(int printingTypeID, int paperSizeID) {
			return (new BusinessRules.Printing()).GetPaperSizeInfo(printingTypeID, paperSizeID);
		}

		public PaperTypeInfo GetPaperTypeInfo(int paperTypeID) {
			return (new BusinessRules.Printing()).GetPaperTypeInfo(paperTypeID);
		}

		public PrintingTypeInfo[] GetPrintingTypes() {
			return (new BusinessRules.Printing()).GetPrintingTypes(AppConfig.siteId);
		}

		/// <summary>
		/// </summary>
		/// <returns>Collection of Printing Types name</returns>
		public PrintingTypeElement[] GetPrintingTypesNames() {
			return (new BusinessRules.Printing()).GetPrintingTypesNames(AppConfig.siteId);
		}

		public PaperSizeInfo[] GetPaperSizes(int printingTypeID) {
			return (new BusinessRules.Printing()).GetPaperSizes(AppConfig.siteId,printingTypeID);
		}


		public PaperSizeInfo[] GetPrintingPaperSizes(int printingTypeID) {
			return (new BusinessRules.Printing()).GetPrintingPaperSizes(printingTypeID);
		}

		public PaperTypeInfo[] GetPaperTypes(int printingTypeID, int paperSizeId) {
			//throw new Exception("Method not implemented.");
			return (new BusinessRules.Printing()).GetPaperTypes(AppConfig.siteId,printingTypeID, paperSizeId);
		}
		
		public Hashtable/*ExtraInfo[]*/ GetExtras(int printingTypeID) {
			return (new BusinessRules.Printing()).GetExtras(AppConfig.siteId,printingTypeID);
		}

		public OrderQuantityInfo[] GetOrderQuantities(int printingTypeID) {
			return (new BusinessRules.Printing()).GetOrderQuantities(AppConfig.siteId,printingTypeID);
		}

		public PrintingTypePart[] GetPrintingTypeParts(int printingTypeID) {
			return (new BusinessRules.Printing()).GetPrintingTypeParts(AppConfig.siteId,printingTypeID);
		}
		
		[Obsolete("")]
		public Hashtable/*ExtraInfo[]*/  GetExtraPrices (int printingTypeID) {
			return (new BusinessRules.Printing()).GetExtraPrices(AppConfig.siteId,printingTypeID);
		}

		public Hashtable GetFullExtraPrices (int printingTypeID) {
			return (new BusinessRules.Printing()).GetFullExtraPrices(AppConfig.siteId,printingTypeID);
		}

		public PrintingPrice[] GetPrices(int printingTypeID) {
			//throw new Exception("Not implemented yet");
			return (new BusinessRules.Printing()).GetPrices(AppConfig.siteId,printingTypeID);//,PrintingPrice.ALL_ITEMS,PrintingPrice.ALL_ITEMS,PrintingPrice.ALL_ITEMS);
		}

		/// <summary>
		/// return printings prices ordered the way convinient for rendering price controls
		/// </summary>
		/// <param name="printingTypeID"></param>
		/// <returns></returns>
		public PrintingPrice[] GetOrderedPrices(int printingTypeID) {

			OrderExpression orderBy = new OrderExpression(typeof(PriceListFields));
			orderBy[(int)PriceListFields.Quantity] = Order_By_Expression.ASC;
			orderBy[(int)PriceListFields.PaperSizeID] = Order_By_Expression.ASC;
			orderBy[(int)PriceListFields.PaperTypeID] = Order_By_Expression.ASC;
			
			return GetOrderedPrices(printingTypeID,orderBy);
		}

		public PrintingPrice[] GetOrderedPrices(int printingTypeID, OrderExpression orderBy) {
			return (new BusinessRules.Printing()).GetPrices(AppConfig.siteId,printingTypeID,PrintingPrice.ALL_ITEMS,PrintingPrice.ALL_ITEMS,PrintingPrice.ALL_ITEMS, orderBy);
		}

		public PrintingPrice GetPrice(int printingTypeID, int paperTypeID, int paperSizeID, int quantity) {
			PrintingPrice[] priceinfo = (new BusinessRules.Printing()).GetPrices(AppConfig.siteId, printingTypeID, paperTypeID, paperSizeID, quantity);

			//FIXME: implement and use here the Printing class method that return single price
			return priceinfo[0];
		}

		public bool GetPrice(int printingTypeID, int paperTypeID, int paperSizeID, int quantity, out PrintingPrice price) {
			PrintingPrice[] priceinfo = (new BusinessRules.Printing()).GetPrices(AppConfig.siteId, printingTypeID, paperTypeID, paperSizeID, quantity);

			//FIXME: implement and use here the Printing class method that return single price
			if (priceinfo.Length == 0) {
				price = new PrintingPrice();
				return false;
			}
			else {
				price = priceinfo[0];
				return true;
			}
		}

		public PrintingPrice GetPrice(int printingTypeID, int paperTypeID, int paperSizeID, int quantity, bool wholesale, string code) {
			PrintingPrice[] priceinfo;
			if (!wholesale) {
				priceinfo = (new BusinessRules.Printing()).GetPrices(AppConfig.siteId, printingTypeID, paperTypeID, paperSizeID, quantity);
			} else {
				if ((code==null)||(code==PromoCodeInfo.COMMON_CODE)) {
					priceinfo = (new BusinessRules.Printing()).GetCommonWholesalePrices(AppConfig.siteId, printingTypeID, paperTypeID, paperSizeID, quantity);
				} else {
					priceinfo = (new BusinessRules.Printing()).GetWholesalePrices(AppConfig.siteId, code, printingTypeID, paperTypeID, paperSizeID, quantity);
				}
			}
			//FIXME: implement and use here the Printing class method that return single price
			return priceinfo[0];
		}

		#region wholesale prices
		/// <summary>
		/// return the price list that is associated with the given wholesale code
		/// </summary>
		/// <param name="code"></param>
		/// <param name="printingTypeID"></param>
		/// <returns></returns>
		public PrintingPrice[] GetOrderedWholesalePrices(string code, int printingTypeID) {
			return GetOrderedWholesalePrices(AppConfig.siteId, code, printingTypeID);
		}

		/// <summary>
		/// return a "common" wholesale pricelist (the price list that is not associated with any wholesale code)
		/// </summary>
		/// <param name="printingTypeID"></param>
		/// <returns></returns>
		public PrintingPrice[] GetOrderedCommonWholesalePrices(int printingTypeID) {
			return GetOrderedCommonWholesalePrices(AppConfig.siteId, printingTypeID, null);
		}
		
		public PrintingPrice[] GetOrderedCommonWholesalePrices(int siteId, int printingTypeID) {
			
			OrderExpression orderBy = new OrderExpression(typeof(PriceListFields));
			orderBy[(int)PriceListFields.Quantity] = Order_By_Expression.ASC;
			orderBy[(int)PriceListFields.PaperSizeID] = Order_By_Expression.ASC;
			orderBy[(int)PriceListFields.PaperTypeID] = Order_By_Expression.ASC;
			
			return GetOrderedCommonWholesalePrices(siteId, printingTypeID, orderBy);
		}

		public PrintingPrice[] GetOrderedCommonWholesalePrices(int siteId, int printingTypeID,  OrderExpression orderBy) {
			return (new BusinessRules.Printing()).GetCommonWholesalePrices(siteId, printingTypeID, PrintingPrice.ALL_ITEMS, PrintingPrice.ALL_ITEMS, PrintingPrice.ALL_ITEMS, orderBy);
		}
		/// <summary>
		/// return wholesale prices ordered the way convinient for rendering price controls
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="code"></param>
		/// <param name="printingTypeID"></param>
		/// <returns></returns>
		public PrintingPrice[] GetOrderedWholesalePrices(int siteId, string code, int printingTypeID) {
			OrderExpression orderBy = new OrderExpression(typeof(PriceListFields));

			orderBy[(int)PriceListFields.Quantity] = Order_By_Expression.ASC;
			orderBy[(int)PriceListFields.PaperSizeID] = Order_By_Expression.ASC;
			orderBy[(int)PriceListFields.PaperTypeID] = Order_By_Expression.ASC;

			return GetOrderedWholesalePrices(siteId, code, printingTypeID,orderBy);
		}

		public PrintingPrice[] GetOrderedWholesalePrices(int siteId, string code, int printingTypeID,  OrderExpression orderBy) {
			return (new BusinessRules.Printing()).GetWholesalePrices(siteId, code, printingTypeID, PrintingPrice.ALL_ITEMS, PrintingPrice.ALL_ITEMS, PrintingPrice.ALL_ITEMS, orderBy);
		}

		#endregion

		public DateTime[][] GetPrintingShedule(DateTime since, DateTime upTo) {
			return (new BusinessRules.Printing()).GetPrintingShedule(AppConfig.siteId, since, upTo);
		}

/*		/// <summary>
		/// RFU: 
		/// </summary>
		/// <param name="siteID"></param>
		/// <returns></returns>
		public ICollection GetPrices(int siteID) {
			throw new Exception("Not implemented yet");
		}*/

		public bool AddPrintingPrice (PrintingPrice pPrice) {
			return (new BusinessRules.Printing()).AddPrintingPrice(AppConfig.siteId, pPrice);
		}

		public bool UpdatePrintingPrice (PrintingPrice pPrice) {
			return (new BusinessRules.Printing()).UpdatePrintingPrice(AppConfig.siteId, pPrice);
		}

		public bool UpdateWholesalePrintingPrice (string code, PrintingPrice pPrice) {
			return (new BusinessRules.Printing()).UpdateWholesalePrice(AppConfig.siteId, code, pPrice);
		}

		public bool UpdateCommonWholesalePrintingPrice (PrintingPrice pPrice) {
			return (new BusinessRules.Printing()).UpdateCommonWholesalePrice(AppConfig.siteId, pPrice);
		}


		public PaperTypeInfo[] GetAllPaperTypes() {
			return (new BusinessRules.Printing()).GetAllPaperTypes();
		}

		public PaperTypeInfo[] GetAllPaperTypes(FilterExpression filter, OrderExpression order) {
			return (new BusinessRules.Printing()).GetAllPaperTypes(filter, order);
		}

		
		#region PrintingTypeParts
		public PrintingTypePart GetPrintingTypePartInfo(int printingTypeID, int partID) {
			return (new BusinessRules.Printing()).GetPrintingTypePartInfo(printingTypeID, partID);
		}

		public bool RemovePrintingTypePart(int partID) {
			return (new BusinessRules.Printing()).RemovePrintingTypePart(partID);
		}
		#endregion


		#region quantities
		public bool RemoveOrderQuantity(int printingTypeID, int quantity) {
			return (new BusinessRules.Printing()).RemoveOrderQuantity(printingTypeID, quantity);
		}


		public OrderQuantityInfo GetOrderQuantityInfo(int printingTypeID, int quantity) {
			return (new BusinessRules.Printing()).GetOrderQuantityInfo(printingTypeID, quantity);
		}

		public bool AddOrderQuantity(OrderQuantityInfo quantityInfo) {
			return (new BusinessRules.Printing()).AddOrderQuantity(quantityInfo);
		}

		public bool UpdateOrderQuantity(OrderQuantityInfo quantityInfo) {
			return (new BusinessRules.Printing()).UpdateOrderQuantity(quantityInfo);
		}
		
		#endregion


		#region extras
		public FullExtraInfo[] GetAllExtras(FilterExpression filter, OrderExpression order) {
			return (new BusinessRules.Printing()).GetAllExtras(filter, order);
		}

		public FullExtraInfo[] GetAllExtras() {
			return GetAllExtras(null, null);
		}

		public FullExtraInfo GetExtraInfo(int extraID) {
			return (new BusinessRules.Printing()).GetExtraInfo(extraID);
		}

		public bool AddExtraInfo(FullExtraInfo ei,out int extraID) {
			return (new BusinessRules.Printing()).AddExtraInfo(ei,out extraID);
		}

		public bool UpdateExtraInfo(FullExtraInfo ei) {
			return (new BusinessRules.Printing()).UpdateExtraInfo(ei);
		}

		#endregion


		#region add, update printings

		public bool CheckPrintingType(int printingTypeID) {
			return (new BusinessRules.Printing()).CheckPrintingType(printingTypeID);
		}


		public bool AddPrintingType(PrintingTypeInfo pTypeInfo) {
			return (new BusinessRules.Printing()).AddPrintingType(pTypeInfo);
		}


		public bool UpdatePrintingType(PrintingTypeInfo pTypeInfo) {
			return (new BusinessRules.Printing()).UpdatePrintingType(pTypeInfo);
		}


		public bool AddPaperSize(int printingTypeId, PaperSizeInfo pSizeInfo) {
			return (new BusinessRules.Printing()).AddPaperSize(printingTypeId,pSizeInfo);
		}


		public bool UpdatePaperSize(int printingTypeId, PaperSizeInfo pSizeInfo) {
			return (new BusinessRules.Printing()).UpdatePaperSize(printingTypeId,pSizeInfo);
		}

		public bool RemovePaperSize(int paperSizeID, int printingTypeId) {
			return (new BusinessRules.Printing()).RemovePaperSize(paperSizeID, printingTypeId);
		}


		public bool AllowPaperType(int printingTypeId, int paperSizeId, int paperTypeId) {
			return (new BusinessRules.Printing()).AllowPaperType(printingTypeId,paperSizeId,paperTypeId);
		}


		public bool AddPaperType(PaperTypeInfo pTypeInfo) {
			return (new BusinessRules.Printing()).AddPaperType(pTypeInfo);
		}


		public bool UpdatePaperType(PaperTypeInfo pTypeInfo) {
			return (new BusinessRules.Printing()).UpdatePaperType(pTypeInfo);
		}


		public bool AddPaperTypePart(int printingTypeId, PrintingTypePart pTypePart) {
			return (new BusinessRules.Printing()).AddPaperTypePart(printingTypeId, pTypePart);
		}


		public bool UpdatePaperTypePart(int printingTypeId, PrintingTypePart pTypePart) {
			return (new BusinessRules.Printing()).UpdatePaperTypePart(printingTypeId, pTypePart);
		}

		#endregion

	
	}
	/*	public class PriceList {
			private Hashtable _price;

			public PriceList() {
				_price = new Hashtable();
			}
			public void Load() {
				PrintingsFacade pf = new PrintingsFacade();
				foreach (PrintingTypeElement ptElem in pf.GetPrintingTypesNames()) {
					_price.Add(new Int32(ptElem.PrintingTypeID),new Hashtable());
					PrintingTypeInfo ptInfo = pf.GetPrintingTypeInfo(ptElem.PrintingTypeID);
					foreach ( ptInfo
				}

			}

		}*/


}


//}
