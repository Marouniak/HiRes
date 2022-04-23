/*
 * FILE:		OrderInfo.cs
 * 
 * PROJECT:		HiRes PrintingsSite
 * 
 * ABSTRACT:	"Order" entity related classes, structures and enums
 * 
 * LEGAL:		Copyright (c) HiRes Studios, 2002-2003
 * 
 * Revision history:
 * 
 * 28-Apr-2002 Gleb Novodran
 * Initial implementation
 * 
 * 
 */
using System;
using System.Collections;

namespace HiRes.Common {
	
	public enum PostalCarrier {
		//PickUpFromHiRes = 0,
		UPS = 1,
		FedEx = 2
	}
	public enum ProofType : short {
		None = -1,
		Web = 0,
		PDF = 1,
		Laser = 2
	}

	public enum OrderFields {
		NONE = 0,
		OrderID,
		SiteID,
		//AmountCalculated,
		//AmountShipping,
		//AmountTax,
		//AmountDiscount,
		//AmountTotal,
		//RequiredDownpayment,
		PlacedTS,
		DesignDue,
		PrintingDue,
		//LastModifiedTS,
		Status,
		IsCancelled,
		//ShipTrackingID,
		CustomerID,
		//PrintingTypeID,
		//PaperTypeID,
		//PaperSizeID,
		//Quantity,
		//OrderLog,
		//OrderJobType
	}

	public enum ScanType : short {
		None = 0,
		Flat_Bed = 1,
		Drum = 2
	}
	
	public enum PostageType : short {
		None = 0,
		StandardAutomated = 1,
		StandardNonAutomated = 2,
		FirstClassAutomated = 3,
		FirstClassNonAutomated = 4
	}

	/// <summary>
	/// OrderInfo
	/// </summary>
	public class OrderInfo : PersistentBusinessEntity {

		public enum OrderingFault {
			None,
			ExtrasSelection,
			ArtworkOptions,
			ShippingOptions
		}
		
		public class PaymentAmounts {
			/// <summary>
			/// a base amount that is calculated basing upon pricelist info
			/// </summary>
			private decimal _calculatedAmount;
			/// <summary>
			/// amount for extras (die cutting etc.)
			/// </summary>
			private decimal _extrasAmount;
			/// <summary>
			/// discount amount
			/// </summary>
			private decimal _discountAmount;
			private decimal _shippingAmount;
			private decimal _taxAmount;
			
			private decimal _designHours;
			private decimal _designCost;
			private bool _isDesignCostFixed = false;
			//private decimal _designAmount;

			private decimal _proofAmount;
			private decimal _imagesAmount;
			private decimal _scanAmount;

			private decimal _postageCostPerPiece;
			private int _postageQnt; // postage quantity
			private decimal _mailingPrcCostPerM; // Processing cost per 1000
			private int _mailingPrcQnt; // Processing quantity
			private decimal _diskPrepcost;// Disk preparation cost
			private decimal _mailingListsTotalCost;

			/// <summary>
			/// totalAmount = CalculatedAmout + ExtrasAmount + TaxAmount + ShippingAmount + DesignAmount - DiscountAmount
			/// </summary>
			private decimal _totalAmount;
/*			
			public void RecalculateTotalAmount() {
				_totalAmount = _calculatedAmount+_extrasAmount+_taxAmount+DesignAmount+_imagesAmount+_scanAmount+_proofAmount+_shippingAmount-_discountAmount;
			}
*/
			//private decimal _downpaymentRequired;

			public decimal CalculatedAmount {
				get { return _calculatedAmount; }
				set { 
					_calculatedAmount = value;
					//RecalculateTotalAmount();
				}
			}

			public decimal ExtrasAmount {
				get { return _extrasAmount; }
				set { 
					_extrasAmount = value;
					//RecalculateTotalAmount();
				}
			}
			/// <summary>
			/// Return amount that is charged for printing and extras.
			/// It also is used as abase amount for calculating taxes.
			/// </summary>
			public decimal BasePrintingAmount {
				get {
					return _calculatedAmount+_extrasAmount;
				}
			}

			public decimal DiscountAmount {
				get { return _discountAmount; }
				set { 
					_discountAmount = value;
					//RecalculateTotalAmount();
				}
			}

			public decimal ShippingAmount {
				get { return _shippingAmount; }
				set { 
					_shippingAmount = value;
					//RecalculateTotalAmount();
				}
			}

			public decimal TaxAmount {
				get { return _taxAmount; }
				set { 
					_taxAmount = value;
					//RecalculateTotalAmount();
				}
			}

			public decimal TotalAmount {
				get {
					//return _totalAmount;
					return _calculatedAmount+_extrasAmount+_taxAmount+DesignAmount+_imagesAmount+_scanAmount+_proofAmount+TotalMailingAmount+_shippingAmount-_discountAmount;
				}
				//set { _totalAmount = value; }
			}
			public decimal SubTotalAmount {
				get { return (TotalAmount-TaxAmount); }
			}

			public decimal DesignHours {
				get { return _designHours; }
				set { _designHours = value; }
			}
			public decimal DesignCost {
				get { return _designCost; }
				set { _designCost = value; }
			}
			public bool IsDesignCostFixed {
				get { return _isDesignCostFixed; }
				set { _isDesignCostFixed = value; }
			}

			/*public decimal DesignAmount {
				get { return _designAmount; }
				set { 
					_designAmount = value;
					RecalculateTotalAmount();
				}
			}*/
			
			public decimal DesignAmount {
				get {
					if (_isDesignCostFixed) { return _designCost; }
					else {
						return _designCost*_designHours;
					}
				}
			}

			public decimal ImagesAmount {
				get { return _imagesAmount; }
				set { 
					_imagesAmount = value;
				}
			}
			public decimal ScanAmount {
				get { return _scanAmount; }
				set { 
					_scanAmount = value;
				}
			}

			public decimal ProofAmount {
				get { return _proofAmount; }
				set { 
					_proofAmount = value;
				}
			}

			public decimal PostageCostPerPiece {
				get { return _postageCostPerPiece; }
				set { _postageCostPerPiece = value;	}
			}
			public int PostageQuantity {
				get { return _postageQnt; }
				set { 
					if (value<0) {throw new ArgumentException("value should be positive");}
					_postageQnt = value;
				}
			}

			public decimal PostageTotalAmount {
				get { return _postageCostPerPiece*_postageQnt; }
			}

			public decimal MailingPrcCostPerM {
				get { return _mailingPrcCostPerM; }
				set { _mailingPrcCostPerM = value;	}
			}
			public int MailingPrcQuantity {
				get { return _mailingPrcQnt; }
				set { 
					if (value<0) {throw new ArgumentException("value should be positive");}
					_mailingPrcQnt = value;
				}
			}

			public decimal MailingPrcTotalAmount {
				get { return (_mailingPrcCostPerM*_mailingPrcQnt)/1000; }
			}

			public decimal DiskPrepCost {
				get { return _diskPrepcost; }
				set { _diskPrepcost = value;	}
			}
			
			/// <summary>
			/// Total cost of mailing lists
			/// </summary>
			public decimal MailingListsTotalCost {
				get { return _mailingListsTotalCost; }
				set { _mailingListsTotalCost = value;	}
			}
			/*protected void SetMailingListsTotalCost(decimal val) {
				_mailingListsTotalCost = val;
			}*/

			public decimal TotalMailingAmount {
				get { return PostageTotalAmount+MailingPrcTotalAmount+_diskPrepcost+MailingListsTotalCost; }
			}
			/// <summary>
			/// This amount to use to transfer as a 'Declared Value for Carriage' to postal carriers
			/// </summary>
			public decimal DeclaredAmount {
				get { return CalculatedAmount + ExtrasAmount; }
			}

			/// <summary>
			/// Set tax amount by specifing the tax persentage
			/// </summary>
			/// <param name="percentage"></param>
			public void SetTaxPersentage(decimal percentage) {
				_taxAmount = (BasePrintingAmount/100)*percentage;
			}

		}


		public class JobInfo {
			private string _jobName;
			// "Printing only" or "Design and printing"
			private JobType _jobType;
			// printing type
			private int _printingTypeID;
			//private string _printingTypeName;
			// paper type ID
			private int _paperTypeID;
			// paper size ID
			private int _paperSizeID;
			private string _customPaperSize;
			// quanity
			private int _quantity;
			private bool _isCustomQuantity;
			// selected extras
			private Hashtable/*ArrayList*/ _extras;
			private bool _hasExtras;

			private ProofType _proofType;
			private ScanType _scanType;
			private short _imageSourceId;

			private int _proofNumber;
			private int _scanNumber;
			private int _imagesNumber;

			private PostageType _postageType;

			public String JobName {
				get { return _jobName; }
				set { _jobName = value; }
			}

			public JobType JobType {
				get { return _jobType; }
				set { _jobType = value; }
			}

			public bool IsDesignOrdered {
				get {
					return (this.JobType == JobType.DesignAndPrinting)||(this.JobType == JobType.DesignOnly);
				}
			}
			public bool IsPrintingOrdered {
				get {
					return (this.JobType == JobType.DesignAndPrinting)||(this.JobType == JobType.PrintingOnly);
				}
			}

			public bool IsMailingOrdered {
				get {
					return this.PostageType!=PostageType.None;
				}
			}
		
			public int PrintingTypeID {
				get { return _printingTypeID; }
				set { _printingTypeID = value; }
			}
			public int PaperSizeID {
				get { return _paperSizeID; }
				set { _paperSizeID = value; }
			}
			public string CustomPaperSize {
				get { return _customPaperSize; }
				set { _customPaperSize = value; }
			}
			public bool IsCustomPaperSize {
				get {
					//return (this._paperTypeID==PersistentBusinessEntity.ID_EMPTY)
					return (_customPaperSize!=null)&&(_customPaperSize.Length>0);
				}
			}

			public int PaperTypeID {
				get { return _paperTypeID; }
				set { _paperTypeID = value; }
			}
			public int Quantity {
				get { return _quantity; }
				set { _quantity = value; }
			}
			public bool IsCustomQuantity {
				get { return _isCustomQuantity; }
				set { _isCustomQuantity = value; }
			}

			public bool IsCustomJob {
				get { return IsCustomQuantity||IsCustomPaperSize;}
			}

			public Hashtable Extras {
				get { return _extras; }
				set { 
					if (value!=null) {
						if (value.Count>0) {
							HasExtras = true;
						} else {
							HasExtras = false;
						}
					} else { HasExtras = false; }

					_extras = value;
				}
			}

			public JobInfo() {
				this.JobType = JobType.NONE;
				_isCustomQuantity = false;
				this._extras = new Hashtable();/*ArrayList();*/
				_postageType = PostageType.None;
				_proofType = ProofType.Web;
				_scanType = ScanType.None;
			}

			public bool HasExtras {
				get {
					return _hasExtras;
					//return this.Extras.Count > 0;
				}
				set {
					_hasExtras = value;
				}
			}

			public ProofType ProofType {
				get { return _proofType; }
				set { _proofType = value; }
			}
			
			public ScanType ScanType {
				get { return _scanType; }
				set { _scanType = value; }
			}

			public short ImageSourceId {
				get { return _imageSourceId; }
				set { _imageSourceId = value; }
			}
			
			public int ProofNumber {
				get { return _proofNumber; }
				set { _proofNumber = value; }
			}

			public int ScanNumber {
				get { return _scanNumber; }
				set { _scanNumber = value; }
			}
			public int ImagesNumber {
				get { return _imagesNumber; }
				set { _imagesNumber = value; }
			}

			public PostageType PostageType {
				get { return _postageType; }
				set { _postageType = value; }
			}
		}


		#region private members
		
		private int _orderId;
		//private string _jobName;
		private String _customerId;
		private int _siteID;
		
		private DeliveryDetailsInfo _deliveryDetails;
		private ContactInfo _billTo;

		//private String _shipTrackingId;

		private OrderStatus _status;
		private bool _isCancelled;
		
		private JobInfo _job;
		private JobDesign _design;

		private decimal _requiredDownpayment;

		private PaymentAmounts _amounts;

		private DateTime _placedTS;
		private DateTime _lastModifiedTS;
		
		private DateTime _designDue;
		private DateTime _printingDue;
		private DateTime _mailingDue;
		// the collection contains payments made by customer
		private ArrayList _payments;
		
		private decimal _totalAmountPaid;

		/*private decimal _designHours;
		private decimal _designCost;
		private bool _isDesignCostFixed = false;*/
		private string _designer;

		//private string _orderLog;
		private OrderLog _orderLog;

		private int _UPNo;
		private int _PSNo;
		// promo codes
		private OrderPromoCodes _orderPromoCodes;
		private ArrayList _shippedPackages;
		#endregion

		#region Constructors

		public OrderInfo():this(true) {
		}

		public OrderInfo(bool force) : base() {
			_orderId = ID_EMPTY;
			this.CreatedTS = this.LastModifiedTS = DateTime.Now;
			Status = OrderStatus.New_Ordering_IncompleteInfo;//TODO: consider removing
			if (force) {
				ForceParts(null,null);
			}
		}

		public OrderInfo(JobInfo job):this(false) {
			ForceParts(job,null);
		}

		public OrderInfo(PaymentAmounts paymentAmounts):this(false) {
			ForceParts(null,paymentAmounts);
		}
		public OrderInfo(JobInfo job, PaymentAmounts paymentAmounts):this(false) {
			ForceParts(job,paymentAmounts);
		}
		#endregion

		private void ForceParts(JobInfo job, PaymentAmounts paymentAmounts) {
			if (job!=null) {
				this._job = job;
			} else {
				this._job = new JobInfo();
			}
			if (paymentAmounts!=null) {
				this._amounts = paymentAmounts;
			} else {
				this._amounts = new PaymentAmounts();
			}
			this._design = new JobDesign();
			this._deliveryDetails = new DeliveryDetailsInfo();
			this._payments = new ArrayList();
			this._orderPromoCodes = new OrderPromoCodes();
			_shippedPackages = new ArrayList();
		}


		#region properties
		
		public override bool IsNew {
			get { return (_orderId==ID_EMPTY); }
		}

		public int OrderId {
			get { return _orderId; }
			set { 
				_orderId = value;
				if (this.Payments!=null) {
					foreach (PaymentInfo paymentInfo in this.Payments) {
						paymentInfo.OrderId = value;
					}
				}
				
			}
		}

		public String CustomerID {
			get { return _customerId; }
			set { _customerId = value; }
		}

		public String Designer {
			get { return _designer; }
			set { _designer = value; }
		}
/*		public decimal DesignHours {
			get { return _designHours; }
			set { _designHours = value; }
		}
		public decimal DesignCost {
			get { return _designCost; }
			set { _designCost = value; }
		}
		public bool IsDesignCostFixed {
			get { return _isDesignCostFixed; }
			set { _isDesignCostFixed = value; }
		}
*/
		public DeliveryDetailsInfo DeliveryDetails {
			get { return _deliveryDetails; }
			set {
				if (value!=null) {
					_deliveryDetails = value;
				} else {
					throw new ArgumentNullException();
				}
			}
		}

		public ContactInfo  BillTo {
			get { return _billTo; }
			set {
				if (value!=null) {
					_billTo = value;
				} else {
					throw new ArgumentNullException();
				}
				//_billTo = value;
			}
		}

		public bool IsValidDeliveryDetails {
			get { return this.DeliveryDetails.IsValid; }
			set { 
/*				if (value==false) {
					
				}*/
				this.DeliveryDetails.IsValid = value;
			}
		}
		
		//TODO: consider removing
		public String ShipTrackingID {
			get { return this.DeliveryDetails.ShipTrackingID; }
			set { this.DeliveryDetails.ShipTrackingID = value; }
		}

		public OrderInfo.JobInfo OrderJob {
			get { return _job; }
			set { _job = value; }
		}
		
		public OrderStatus Status {
			get { return _status; }
			set { _status = value; }
		}

		public bool IsCancelled {
			get { return _isCancelled; }
			set { _isCancelled = true; }
		}

		public JobDesign Design {
			get { return _design; }
			set { _design = value; }
		}

		public decimal DownpaymentRequired {
			get { return _requiredDownpayment; }
			set { _requiredDownpayment = value; }
		}

		public PaymentAmounts Amounts {
			get { return _amounts; }
			set { _amounts = value; }
		}

		public ArrayList Payments {
			get { return _payments; }
			set { _payments = value; }
		}

		public decimal TotalAmountPaid {
			get {
				return _totalAmountPaid;
			}
			set {
				_totalAmountPaid = value;
			}
		}

		public DateTime PlacedTS {
			get { return CreatedTS;/*._placedTS;*/ }
			set { CreatedTS = value; }
		}

		public DateTime LastModifiedTS {
			get { return _lastModifiedTS; }
			set { _lastModifiedTS = value; }
		}
		
		public DateTime DesignDue {
			get { return _designDue; }
			set { _designDue = value; }
		}
		public DateTime PrintingDue {
			get { return _printingDue; }
			set { _printingDue = value; }
		}

		public DateTime MailingDue {
			get { return _mailingDue; }
			set { _mailingDue = value; }
		}
/*
		public String OrderLog {
			get { return _orderLog; }
			set { _orderLog = value; }
		}
*/
		public int SiteID {
			get { return _siteID; }
			set { _siteID = value; }
		}


		public OrderLog Log {
			get { return _orderLog; }
			set { _orderLog = value; }
		}
		public bool IsLogLoaded {
			get { return _orderLog==null; }
		}

		public int UPNo {
			get { return _UPNo; }
			set { _UPNo = value; }
		}
		public int PSNo {
			get { return _PSNo; }
			set { _PSNo = value; }
		}
		public OrderPromoCodes OrderPromoCodes {
			get { return _orderPromoCodes; }
			set { _orderPromoCodes = value; }
		}
		
		public ArrayList ShippedPackages {
			get { return _shippedPackages; }
			set { _shippedPackages = value; }
		}
		
		#endregion

		#region BL-related properties

		public bool DeliveryDetailsCanBeChanged {
			get {
				if (  /*(this.Status==OrderStatus.New)
					||*/(this.Status == OrderStatus.New_DesignIsUploaded)
					||(this.Status == OrderStatus.New_WaitingUpload) 
					||(this.Status == OrderStatus.New_Ordering)
					||(this.Status == OrderStatus.New_Ordering_IncompleteInfo)
				) {
					return true;
				} else { return false; }

			}
		}

		/*public bool IsCustomOrder {
			get {
				return OrderJob.IsCustomJob;
			}
		}*/
/*		public void ClearShippingMethodInfo() {
			this.DeliveryDetails.ShipMethod = null;
			this.Amounts.ShippingAmount = 0.00m;
			this.Amounts.RecalculateTotalAmount();
		}*/
		#endregion
		
		/// <summary>
		/// This method is used to get the info regarding the exact step of the ordering process during which this process was terminated.
		/// Frontend functionality use this method to determine to which step the customer should be redirected to if he wants to complete the order.
		/// </summary>
		/// <param name="faultedAt">Code that indicates the exact step of the ordering process during which this process was terminated.</param>
		/// <returns>
		/// <code>true</code> if order is completed
		/// <code>false</code> if the ordering process was terminated
		/// </returns>
		public bool IsOrderIncomplete(out OrderInfo.OrderingFault faultedAt) {
			if (!this.IsNew) {
				faultedAt = OrderInfo.OrderingFault.None;
				return false;
			}

			if (this.OrderJob.JobType == JobType.NONE) {
				faultedAt = OrderInfo.OrderingFault.ArtworkOptions;
				return true;
			}

			if (!this.DeliveryDetails.IsValid) {
				faultedAt = OrderInfo.OrderingFault.ShippingOptions;
				return true;
			}
			faultedAt = OrderInfo.OrderingFault.None;
			return false;
		}

		/// <summary>
		/// </summary>
		/// <remarks>Usually is used in aspx pages in grids, repeaters etc.</remarks>
		/// <returns></returns>
		public bool IsOrderIncomplete() {
			OrderInfo.OrderingFault faultedAt;
			return IsOrderIncomplete(out faultedAt);
		}

		/*public void RecalculateDesignAmount() {
			this.Amounts
		}*/

		public void ReCalculateExtrasAmounts(bool isFullRecalc) {
			decimal totalExtraAmount = 0.00m;
			foreach (SelectedExtraInfo selectedExtra in this.OrderJob.Extras.Values) {
				
				/*decimal addAmount = selectedExtra.Price*(this.OrderJob.Quantity/1000);
				decimal addAmountM = selectedExtra.Price*(this.OrderJob.Quantity/1000m);*/
				//(selectedExtra.IsPricePerM?selectedExtra.Price*(this.OrderJob.Quantity/1000m):selectedExtra.Price);
				if (isFullRecalc) { selectedExtra.RecalculateTotalExtraAmount(); }
				totalExtraAmount += selectedExtra.TotalExtraAmount;
			}
			this.Amounts.ExtrasAmount = totalExtraAmount;
			//this.Amounts.RecalculateTotalAmount();
		}
		// TODO: change it to meet new extras requirements - add quantity for each extra and change recalculation
		/*public void ReCalculateExtrasAmounts(Hashtable newPrices) {
			if (newPrices==null) { throw new ArgumentNullException("newPrices"); }
			decimal totalExtraAmount = 0.00m;
			foreach(object extraIdKey in this.OrderJob.Extras) {
				totalExtraAmount += ((ExtraInfo)newPrices[extraIdKey]).PriceInfo.Price;
			}
			this.Amounts.ExtrasAmount = totalExtraAmount;
			this.Amounts.RecalculateTotalAmount();
		}
		*/
		public bool UsePromoCode(PromoCodeInfo codeInfo) {
			if (this.OrderPromoCodes.Codes.Count>0) {
				/// Currently usage of multiple codes is disabled
				/// add checking if code is cooperative after enabling it.
				return false;
				//if (!codeInfo.IsCooperative) return false;
			}
			//uncomment the code below after enabling multiple code usage;
			/*
			if (this.OrderPromoCodes.Contains(codeInfo.Code)) {
				return false;
			}
			*/

			decimal discountAmount = 0.00m;
			//decimal newAmount = 0.00m;
			if (codeInfo.Discount.AmountType == DiscountAmountType.Money) {
				discountAmount = codeInfo.Discount.Amount;
//				newAmount = this.Amounts.CalculatedAmount - codeInfo.Discount.Amount;
			} else if (codeInfo.Discount.AmountType == DiscountAmountType.Persentages) {
				discountAmount = decimal.Round(this.Amounts.CalculatedAmount*codeInfo.Discount.Amount/100,2);
//				newAmount = this.Amounts.CalculatedAmount - this.Amounts.CalculatedAmount*codeInfo.Discount.Amount/100;
			}
						
			if ((this.Amounts.CalculatedAmount - discountAmount)<0) return false;
			
			this.Amounts.DiscountAmount = discountAmount;

			this.OrderPromoCodes.AddCode(codeInfo.Code);
			
			//this.Amounts.RecalculateTotalAmount();

			return true;
		}
	}

	/// <summary>
	/// OrderStatus enumerates states that order pass through during its lifecycle
	/// </summary>
	public enum OrderStatus {
		New_Ordering_IncompleteInfo = -2, //new incomplete info for payment and placing
		New_Ordering = -1,// new; has complete info for payment and persisting to the db, waiting payment 
		//New = 0,
		New_WaitingUpload = 1,
		New_DesignIsUploaded = 2,
		InDesign = 3,
		WaitingForProof = 4,
		Approved = 5,
		InPrint = 7,
		Printed = 8,
		Shipped_WaitingPickUp = 9,
		//		WaitingForPickup = 10,
		Delivered_PickedUp = 11,
		Closed = 12,
		Cancelled = 13,
	}	
/*	public enum OrderStatus {
		New_Ordering_IncompleteInfo = -2, //new incomplete info for payment and placing
		New_Ordering = -1,// new; has complete info for payment and persisting to the db, waiting payment 
		New = 0,
		New_WaitingUpload = 1,
		New_DesignIsUploaded = 2,
		InDesign = 3,
		Design_WaitingForProof = 4,
		Design_Approved = 5,
		PrePress = 6,
		InPrint = 7,
		Printed = 8,
		Shipped_WaitingPickUp = 9,
//		WaitingForPickup = 10,
		Delivered_PickedUp = 11,
		Closed = 12,
		Cancelled = 13,

		//states added 14 Apr 2003
		Printing_PendingUpload = 14,
		Printing_WaitingForProof = 15,
		Printing_Approved = 16
	}
*/
	/*public enum Department {
		Ordering = 0,
		Design,
		Printing,
		Shipping
	}*/
	[Obsolete()]
	public sealed class Departments {
		public const string Ordering = "Ordering";
		public const string Design = "Design";
		public const string Production = "Production";
		public const string OrderingShipping = "Ordering shipping";
	}
	[Obsolete()]
	public sealed class DepartmentStates {
/*		const string Ordering = "Ordering";
		const string Design = "Design";
		const string Printing = "Printing";*/
		//static private Hashtable _deptMemebership
		static private Hashtable _container;
		static private ArrayList _activeStates;
		static DepartmentStates() {
			ArrayList activeStatesMutex = new ArrayList();
			_container = new Hashtable(5);
			ArrayList states = new ArrayList();
			//states.Add(OrderStatus.New);
			states.Add(OrderStatus.New_Ordering);
			states.Add(OrderStatus.New_DesignIsUploaded);
			states.Add(OrderStatus.New_WaitingUpload);
			_container[Departments.Ordering] = ArrayList.ReadOnly(states);

			activeStatesMutex.AddRange(states);

			states = new ArrayList();
			states.Add(OrderStatus.InDesign);
			states.Add(OrderStatus.WaitingForProof);
			states.Add(OrderStatus.Approved);
			_container[Departments.Design] = ArrayList.ReadOnly(states);
			activeStatesMutex.AddRange(states);

			states = new ArrayList();
			//states.Add(OrderStatus.PrePress);
			states.Add(OrderStatus.New_WaitingUpload);
			states.Add(OrderStatus.WaitingForProof);
			states.Add(OrderStatus.Approved);
			states.Add(OrderStatus.InPrint);
			states.Add(OrderStatus.Printed);// TODO: consider removing from this department
			_container[Departments.Production] = ArrayList.ReadOnly(states);
			activeStatesMutex.AddRange(states);

			states = new ArrayList();
			states.Add(OrderStatus.Printed);
/*			states.Add(OrderStatus.Shipping);
			states.Add(OrderStatus.WaitingForPickup);
			states.Add(OrderStatus.Shipped);*/

			states.Add(OrderStatus.Shipped_WaitingPickUp);
			states.Add(OrderStatus.Delivered_PickedUp);

			_container[Departments.OrderingShipping] = ArrayList.ReadOnly(states);
			activeStatesMutex.AddRange(states);
			_activeStates = ArrayList.ReadOnly(activeStatesMutex);

			
		} 


		public static ArrayList GetStates(string deptName) {
			return (ArrayList)_container[deptName];
		}
		/// <summary>
		/// HACK: for active orders list - all states besides "Closed" and "Cancelled"
		/// </summary>
		/// <param name="deptName"></param>
		/// <returns></returns>
		public static ArrayList GetAllActiveStates() {
			return _activeStates;
		}

		/*public static NameValueCollection GetStates(string deptName) {
			//new IdNameAdapter((ArrayList)_container[deptName],
		}*/

		public static bool IsStateInDepartment(OrderStatus state,string deptName) {
			ArrayList states = GetStates(deptName);
			if (states.BinarySearch(state)<0)
				return false;
			else 
				return true;
		}
		/*public static bool GetDepartment(OrderStatus state) {
			
		}*/
	}

	public enum JobType {
		NONE = -1,
		PrintingOnly = 0,
		DesignAndPrinting = 1,
		DesignOnly = 2
	}

	public sealed class DeliveryDetailsInfo {
		
		private AddressInfo _shipAddress;
		private String _shipMethod;
		private DateTime _shipDate;
		private DateTime _shippedDate;
		private String _specialInstructions;
		private bool _pickUpOrder = true;		
		private bool _isDelivered = false;
		private bool _isValid = false;

//		private String _deliveryServiceId;
		private PostalCarrier _carrier;

		private PackagingInfo _packaging;

		private string _shipTrackingID;

		public DeliveryDetailsInfo() {
			_packaging = new PackagingInfo();
			_shipAddress = new AddressInfo();
		}
		#region properties
		
		public AddressInfo ShipAddress {
			get { return _shipAddress; }
			set { _shipAddress = value; }
		}
		/// <summary>
		/// Ship Service method ID. Service method is e.g. UPS Ground
		/// </summary>
		
		public String ShipMethod {
			get { return _shipMethod; }
			set { _shipMethod = value; }
		}
		
		// HACK: 
/*		public void SetShipMethod(int val) {
				if (val<0) throw new ArgumentException("value should be non-negative");

				_shipMethod = ((val>0)&&(val<10))?"0"+val.ToString():val.ToString();
		}
*/
		public DateTime ShipDate {
			get { return _shipDate; }
			set { _shipDate = value; }
		}
		
		public DateTime ShippedDate {
			get { return _shippedDate; }
			set { _shippedDate = value; }
		}
		
		public String SpecialInstructions {
			get { return _specialInstructions; }
			set { _specialInstructions = value; }
		}
		
		public bool PickUpOrder {
			get { return _pickUpOrder; }
			set { 
				_pickUpOrder = value;
				if (_pickUpOrder) {
					ClearDeliveryDetails();
				}
				_isValid = true;
			}
		}
		private void ClearDeliveryDetails() {
			//TODO:consider clearing packaging info
			//_carrier = PostalCarrier.PickUpFromHiRes;
			_shipMethod = null;
			_specialInstructions = null;
		}

		public bool IsValid {
			get { return _isValid; }
			set { 
				_isValid = value;
			}
		}
		public bool IsDelivered {
			get { return _isDelivered; }
			set { _isDelivered = value; }
		}
/*		/// <summary>
		/// Service method ID. Service method is e.g. UPS Ground
		/// </summary>
		public String DeliveryServiceId {
			get { return _deliveryServiceId; }
			set { _deliveryServiceId = value; }
		}
*/		
		public PostalCarrier Carrier {
			get { return _carrier; }
			set { _carrier = value; }
		}		

		public PackagingInfo Packaging {
			get { return _packaging; }
			set { _packaging = value; }
		}
		
		public string ShipTrackingID {
			get { return _shipTrackingID; }
			set { _shipTrackingID = value; }
		}

		#endregion
	}
	
	public enum PartDesignFileCategory {
		CompletedDesign = 0,
		DesignPreview = 1
	}

	public sealed class PartDesign {

		public int OrderId;// TODO: consider removing this
		private int _partId;
		private String _fileName;
		private String _specialInstructions;
		private PartDesignFileCategory _fileCategory = PartDesignFileCategory.CompletedDesign;
		private bool _isFilePersisted = false;
		private bool _isModified = false;

		public int PartId { 
			get { return  _partId; }
			set { _partId = value; }
		}

		public PartDesignFileCategory FileCategory  { 
			get { return  _fileCategory ; }
			set { _fileCategory  = value; }
		}

		public String FileName { 
			get { return  _fileName; }
			set { _fileName = value; }
		}

		public String SpecialInstructions {
			get { return _specialInstructions; }
			set { _specialInstructions = value; }
		}

		public bool IsEmpty {
			get {
				if ((this.FileName==null)||(this.FileName==String.Empty)) {
					return true;
				} else {
					return false;
				}
			}
		}
		public bool IsFilePersisted {
			get { return _isFilePersisted ; }
			set { _isFilePersisted = value; }
		}
		public bool IsModified {
			get { return _isModified ; }
			set { _isModified = value; }		
		}
	}
	public sealed class JobDesign {

		public enum DesignFileType {
			Scetch = 0,
			Description,
			Artwork
		}
		
		private string _customerDescription;

		private PartDesign[] _parts;
		private PartDesign[] _partPreviews;
		/// <summary>
		/// Contain collection of auxiliary files uploaded by a customer like logos, design scetches etc.
		/// </summary>
		private Hashtable _auxfiles;

		public JobDesign() {
			//_parts = new PartDesign[];
			_auxfiles = new Hashtable();
		}

		public JobDesign(PartDesign[] parts) : this(){
			_parts = parts;
		}

		public JobDesign(PartDesign[] parts, AuxFile[] auxFiles) {
			if ((parts==null)||(auxFiles==null)) {
				throw new ArgumentNullException();
			}
			_parts = parts;
			_auxfiles = new Hashtable(parts.Length+1);
			foreach(AuxFile file in auxFiles) {
				AddAuxFile(file);
			}
		}

		public string CustomerDescription {
			get { return _customerDescription; }
			set { _customerDescription = value; }
		}

		public PartDesign[] Parts {
			get { return _parts; }
			set { _parts = value; }
		}
		public PartDesign[] PartPreviews {
			get { return _partPreviews; }
			set { _partPreviews = value; }
		}

		/// <summary>
		/// Return true if there is a design files for all the parts
		/// </summary>
		public bool AllPartsUploaded {
			get { 
				if (Parts==null) return false;
				foreach (PartDesign pd in Parts) {
					/*if ((pd.FileName==null)||(pd.FileName==String.Empty)) {
						return false;
					}*/
					if (pd.IsEmpty) {
						return false;
					}
				}
				return true; 
			}
		}

		public bool AllPartPreviewsUploaded {
			get {
				if (PartPreviews==null) return false;
				foreach (PartDesign pd in PartPreviews) {
					/*if ((pd.FileName==null)||(pd.FileName==String.Empty)) {
						return false;
					}*/
					if (pd.IsEmpty) {
						return false;
					}
				}
				return true; 
			}
		}
		public bool IsPartDesignsLoaded {
			get {
				return ((this._parts!=null)&&(this._parts.Length>0));
			}
		}

		public bool IsPartDesignPreviewsLoaded {
			get {
				return ((this._partPreviews!=null)&&(this._partPreviews.Length>0));
			}
		}

		public Hashtable AuxFiles {
			get { return _auxfiles; }
			set { _auxfiles = value; }
		}
/*		public bool HasAuxFiles {
			get {
				//FIXME: incorrect result
				return (_auxfiles!=null)&&(_auxfiles.Count>0);
			}
		}*/
		public void AddAuxFile(AuxFile file) {
			if (!_auxfiles.ContainsKey(file.PartId)) {
				_auxfiles.Add(file.PartId,new ArrayList());
			}
			((ArrayList)_auxfiles[file.PartId]).Add(/*file.AuxFileId,*/file);
		}

		/*public ArrayList GetAuxFiles(int partId) {
			ArrayList files = (ArrayList)_auxfiles[partId];
			if (files==null)
		}*/

		//public void AddAuxFile(AuxFile file, ) {
	/*	public void RemoveAuxFile(AuxFile file) {
			if (_auxfiles.ContainsKey(file.PartId)) {
				((Hashtable)_auxfiles[file.PartId]).Remove(file.AuxFileId);
			}
		}*/
		public void RemoveAuxFileAt(int partId, int pos) {
			if (_auxfiles.ContainsKey(partId)) {
				((ArrayList)_auxfiles[partId]).RemoveAt(pos);
			}
		}
		
/*		public void RemoveAuxFile(int auxFileId) {
			foreach(Hashtable p in _auxfiles) {
				if (p.ContainsKey(auxFileId)) {
					p.Remove(auxFileId);
				}
			}
		}*/
		public PartDesign GetPart(int partId) {
			for(int i=0;i<_parts.Length;i++) {
				if (_parts[i].PartId == partId) {
					return _parts[i];
				}
			}
			return null;
		}
		/*public void RemovePartDesignFile(int partId) {
			for(int i=0;i<_parts.Length;i++) {
				if (_parts[i].PartId = partId) {
					_parts[i].
				}
			}
		}*/
	}





}
