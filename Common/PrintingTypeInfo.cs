using System;
using System.Collections;
using System.Collections.Specialized;

namespace HiRes.Common {

	/// <summary>
	/// PrintingTypeInfo contains printing type info.
	/// TODO: implement the class
	/// </summary>
	public class PrintingTypeInfo {
		
		/* printing type info table fields */
		/*
		private int _printingTypeID;
		private String _printingTypeName;
		private String _printingTypeDescription;
		private int _siteID;
		*/
		private int _printingTypeID;
		public int PrintingTypeID {
			get { return _printingTypeID; }
			set { _printingTypeID = value; }
		}


		private string _printingTypeName;
		public String PrintingTypeName {
			get { return _printingTypeName; }
			set { _printingTypeName = value; }
		}


		public String PrintingTypeDescription;
		public int SiteID;

		/* relation infos */
		private PaperSizeInfo[] _paperSizes;
		//private PaperTypeInfo[] _paperTypes;
		private /*ExtraInfo[]*/Hashtable _extras;
		private OrderQuantityInfo[] _orderQuantities;
		private PrintingTypePart[] _parts;
		public PrintingTypeInfo() {
		}

		public PaperSizeInfo[] PaperSizes {
			get { return _paperSizes; }
			set { _paperSizes = value; }
		}

		public /*ExtraInfo[]*/ Hashtable Extras {
			get { return _extras; }
			set { _extras = value; }
		}

		public OrderQuantityInfo[] OrderQuantities {
			get { return _orderQuantities; }
			set { _orderQuantities = value; }
		}
		public PrintingTypePart[] Parts {
			get { return _parts; }
			set { _parts = value; }
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class PaperSizeInfo {
		/*
		public int _paperSizeID;
		public String _paperSizeName;
		public String _paperSizeDescription;
		public int _printingTypeID;
		*/
		private int _paperSizeID;
		private String _paperSizeName;
		public String PaperSizeDescription;
		public int PrintingTypeID;
		public PaperTypeInfo[] PaperTypes;

		public int PaperSizeID {
			get { return _paperSizeID; }
			set { _paperSizeID=value; }
		}
		public String PaperSizeName {
			get { return _paperSizeName; }
			set { _paperSizeName=value; }
		}
	}


	public enum PaperTypeFields {
		NONE = 0,
		PaperTypeID,
		PaperTypeName,
		PaperTypeDescription
	}



	/// <summary>
	/// Paper type Info 
	/// </summary>
	public class PaperTypeInfo {
		//public const ID_EMPTY = -1;
		private int _paperTypeID;
//		public int PaperTypeID;
		private String _paperTypeName;

		public String PaperTypeDescription;

		public int PaperTypeID {
			get { return _paperTypeID; }
			set { _paperTypeID = value; }
		}

		public String PaperTypeName {
			get { return _paperTypeName; }
			set { _paperTypeName = value; }
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public class OrderQuantityInfo {
		/*
		public int _quantity;
		public int _printingTypeID;
		public String _quantityDispLabel;
		*/
		private int _quantity;

		public int PrintingTypeID;

		private string _quantityDispLabel;
		
		public int Quantity {
			get { return _quantity; }
			set { _quantity = value; }
		}

		public string QuantityDispLabel {
			get { return _quantityDispLabel; }
			set { _quantityDispLabel = value; }
		}
	}

	/// <summary>
	/// PrintingTypeElement represents Printing Types and used in UI components.
	/// </summary>
	public struct PrintingTypeElement {
		public int PrintingTypeID;
		public String PrintingTypeName;
	}

	public enum PriceListFields {
		NONE = 0,
		PrintingTypeID,
		PaperTypeID,
		PaperTypeName,
		PaperSizeID,
		PaperSizeName,
		Quantity,
		Price,
		IsSpecial
	}

	public struct OrderingItemInfo {
		public int PrintingTypeID;
		public int PaperTypeID;
		public int PaperSizeID;
		public int Quantity;
		
		public OrderingItemInfo (int PrintingTypeID, int PaperTypeID, int PaperSizeID, int Quantity) {
			this.PrintingTypeID = PrintingTypeID;
			this.PaperTypeID = PaperTypeID;
			this.PaperSizeID = PaperSizeID;
			this.Quantity = Quantity;
		}
	}
	/// <summary>
	/// PrintingPrice represent one row of expanded Price Information.
	/// </summary>
	public struct PrintingPrice {

		public const int ALL_ITEMS = -1;

		public int PrintingTypeID;
		public int PaperTypeID;
		public String PaperTypeName;
		public int PaperSizeID;
		public String PaperSizeName;
		public int Quantity;
		public decimal Price;
		public bool IsSpecial;

		public override String ToString() {
			return "<br>" + 
				PrintingTypeID + " | " +
				PaperTypeID + " | " +
				PaperTypeName + " | " +
				PaperSizeID + " | " +
				PaperSizeName + " | " +
				Quantity + " | " +
				Price + " | " +
				"</br>";
		}
	}

	/*
	[Flags]
	public enum PriceFields {
		NONE = 0,
		PrintingTypeID = 0x0001,
		PaperTypeID = 0x0002,
		PaperTypeName = 0x0004,
		PaperSizeID = 0x0008,
		PaperSizeName = 0x0010,
		Quantity = 0x0020,
		Price = 0x0040
	}*/
	//----------- printing part
	public sealed class PrintingTypePart {
		public int PartId;
		private String _partName;
		public String Description;
		public int PrintingTypeId;
		
		public String PartName {
			get { return _partName; }
			set { _partName=value; }
		}
	}

}
