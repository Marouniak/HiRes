using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

using HiRes.Common;
using HiRes.BusinessRules;
using HiRes.PaymentFramework;
using HiRes.ShipmentManager;
using HiRes.SystemFramework;
using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessFacade {

	/*public enum OrderPlacingResCode {
		Ok = 0,
		PaymentAuthorizationFailed,
		DbOperationFailed
	}*/
	public enum OrderProcessResCode {
		OK = 0,
		AUTHORIZATION_FAILED = -1,
		ORDER_DIRECTORY_CREATION_FAILED = -2,
		DB_OPERATION_FAILED = -3,
		NON_AUTHENTICATED_CUSTOMER = -4,
		BILLINGCONTACT_NOTSET = -5,
		ORDERING_INCOMPLETE = -6,
		INSUFFICIENT_PAYMENT_AMT = -7
	}
	/// <summary>
	/// Summary description for OrderFacade.
	/// </summary>
	public class OrderFacade {
		public const int DESIGN_TIMEFRAME = 7;
		public const int PRINTING_TIMEFRAME = 2;

		public OrderInfo GetOrderInfo(int orderId) { 
			Order order = new Order();
			return order.GetOrderInfo(orderId);
		}

		public OrderInfo[] GetOrders(FilterExpression filter, OrderExpression orderBy) {
			Order order = new Order();
			return order.GetOrders(filter, orderBy);
		}
		/// <summary>
		/// Adds new order info including info about downpayment and design files
		/// </summary>
		/// <param name="orderInfo"></param>
		/*public bool AddNewOrder(ref OrderInfo orderInfo) {
			orderInfo.SiteID = AppConfig.siteId;
			return (new Order()).AddNewOrder(ref orderInfo);
			//throw new NotImplementedException();
		}*/

		public OrderProcessResCode PlaceNewOrders ( string customerId, ref ArrayList orders, ref PaymentTransactionInfo txn ) {

			if ((txn.TxnType!=PaymentTransactionType.Authrozation)&&(txn.TxnType!=PaymentTransactionType.Sale)) {
				throw new ArgumentException("txn", "Wrong transaction type. Should be 'authorize' or 'sale'");
			}
			// FIXME:additional checking - development time
			/*if (txn.Amount!=GetDownpaimentRequired(orders.ToArray(),customerId)) {
				//RemovePayments(ref orders);
				//SetOrdersState(ref orders, OrderStatus.New_Ordering);
				throw new ArgumentException("insufficient payment amount","paymentAmount");
				//return OrderPlacingResCode.InsufficientAmount;
			}
			*/
			decimal totalDownpaymentRequired = 0.00m;
			
			//NameValueCollection names = new IdNameAdapter(new PrintingsFacade().GetPrintingTypesNames(),"PrintingTypeID","PrintingTypeName").AdaptedCollection;
			
			PaymentInfo payment;
			DateTime now = DateTime.Now;

			foreach (OrderInfo orderInfo in orders) {
				totalDownpaymentRequired += orderInfo.DownpaymentRequired;
				orderInfo.SiteID = AppConfig.siteId;
				//HACK: (added because a job autonaming is required)
				//TODO: move autonaming to the part where orders are created
				//orderInfo.OrderJob.JobName = names[orderInfo.OrderJob.PrintingTypeID.ToString()]+orderInfo.OrderId;
				//orderInfo.OrderJob.JobName = "";
				if (orderInfo.OrderJob.JobName==null) { orderInfo.OrderJob.JobName = ""; }
				/*switch (orderInfo.OrderJob.JobType) {
					case JobType.DesignOnly:
						orderInfo.DesignDue = orderInfo.PrintingDue = orderInfo.PlacedTS.AddDays(DESIGN_TIMEFRAME);
						break;
					case JobType.PrintingOnly:
						orderInfo.PrintingDue = orderInfo.DesignDue = orderInfo.PlacedTS.AddDays(PRINTING_TIMEFRAME);
						break;
					case JobType.DesignAndPrinting:
						orderInfo.DesignDue = orderInfo.PlacedTS.AddDays(DESIGN_TIMEFRAME);
						orderInfo.PrintingDue = orderInfo.DesignDue.AddDays(PRINTING_TIMEFRAME);
						break;
					default: return OrderProcessResCode.ORDERING_INCOMPLETE;
				}
				orderInfo.DesignDue = orderInfo.PlacedTS.AddDays(DESIGN_TIMEFRAME);
				orderInfo.PrintingDue = orderInfo.PlacedTS.AddDays(PRINTING_TIMEFRAME);
				orderInfo.BillTo = txn.BillTo;*/

				if (orderInfo.OrderJob.JobType==JobType.PrintingOnly) {
					if (orderInfo.Design.AllPartsUploaded) {
						orderInfo.Status = OrderStatus.New_DesignIsUploaded;
					} else {
						orderInfo.Status = OrderStatus.New_WaitingUpload;
					}
				} else if (orderInfo.OrderJob.JobType==JobType.DesignAndPrinting) {
					//orderInfo.Status = OrderStatus.New;
					orderInfo.Status = OrderStatus.InDesign;
				}

				payment = new PaymentInfo();
				payment.Amount = orderInfo.DownpaymentRequired;
				//payment.OrderId
				payment.PaymentDate = now;
				payment.PaymentType = OrderPaymentType.Downpayment;
				txn.Payments.Add(payment);
				orderInfo.Payments.Add(payment);
				//payment.TxnReferenceId = pnref;
				//orderInfo.Payments.Add(payment);
			}

			// FIXME:additional checking - development time only
			if (txn.Amount!=totalDownpaymentRequired) {
				
				throw new ArgumentException("insufficient payment amount");
			}
			

			OrderProcessResCode rescode = OrderProcessResCode.OK;
			
			//Pass the transaction through the payment gateway only if it's a card or echeck transaction that can be conducted online.
			if ((txn.PaymentSource.PaymentInstrumentType==PaymentInstrumentType.Card)||(txn.PaymentSource.PaymentInstrumentType==PaymentInstrumentType.ECheck)) {
				
				PaymentTransactionResponse ptr = null;
				if (AppConfig.DemoMode) {
					ptr = DemoModeHelper.GetFakedTransactionResponse();
				} else {
					//FIXME: Consider changing the processing order
					PaymentProcessor paymentProcessor = new PaymentProcessor();

					ptr = paymentProcessor.ProcessTransaction(txn);
				}
				if (!ptr.isApproved) {
					rescode = OrderProcessResCode.AUTHORIZATION_FAILED;
				} else {
					txn.TxnReferenceId = ptr.PNREF;
				}
			} else {
				txn.TxnReferenceId = PaymentTransaction.GenerateTxnId();
			}

			if (rescode==OrderProcessResCode.OK) {
				TransactionState newState;
				TxnFSM.GetNewState(txn.TxnState,txn.TxnType,out newState);
				txn.TxnState = newState;

				rescode = ((new Order()).PlaceNewOrders(ref orders, ref txn)?OrderProcessResCode.OK:OrderProcessResCode.DB_OPERATION_FAILED);

				if (rescode!=OrderProcessResCode.OK) {
					//RemovePayments(ref orders);
					SetOrdersState(ref orders, OrderStatus.New_Ordering);
				}

			}

			return rescode;
		}



		//public OrderProcessResCode ( string customerId, ref ArrayList orders, ref PaymentTransactionInfo txn ) {}

		/*
		public bool AddNewOrders(
			ref ArrayList orders, 
			PaymentTransactionInfo txn ) {

			return AddNewOrders(ref orders,txn.TxnReferenceId,txn.Amount,txn.BillTo);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="pnref">payment service provider (VeriSign) transaction ID</param>
		/// <param name="paymentAmount"></param>
		/// <returns></returns>
		public bool AddNewOrders(ref ArrayList orders, string pnref, decimal paymentAmount,ContactInfo billTo) {
			if (billTo==null) throw new ArgumentNullException("billTo");

			decimal totalDownpaymentRequired = 0.00m;
			
			NameValueCollection names = new IdNameAdapter(new PrintingsFacade().GetPrintingTypesNames(),"PrintingTypeID","PrintingTypeName").AdaptedCollection;
			
			PaymentInfo payment;
			DateTime now = DateTime.Now;

			foreach (OrderInfo orderInfo in orders) {
				totalDownpaymentRequired += orderInfo.DownpaymentRequired;
				orderInfo.SiteID = AppConfig.siteId;
				//HACK: (added because a job autonaming is required)
				orderInfo.OrderJob.JobName = names[orderInfo.OrderJob.PrintingTypeID.ToString()]+orderInfo.OrderId;
				orderInfo.BillTo = billTo;

				if (orderInfo.OrderJob.JobType==JobType.PrintingOnly) {
					if (orderInfo.Design.AllPartsUploaded) {
						orderInfo.Status = OrderStatus.New_DesignIsUploaded;
					} else {
						orderInfo.Status = OrderStatus.New_WaitingUpload;
					}
				} else if (orderInfo.OrderJob.JobType==JobType.DesignAndPrinting) {
					orderInfo.Status = OrderStatus.New;
				}

				payment = new PaymentInfo();
				payment.Amount = orderInfo.DownpaymentRequired;
				//payment.OrderId
				payment.PaymentDate = now;
				payment.TxnReferenceId = pnref;
				orderInfo.Payments.Add(payment);
			}
			
			// FIXME:additional checking - development time
			if (paymentAmount!=totalDownpaymentRequired) {
				RemovePayments(ref orders);
				SetOrdersState(ref orders, OrderStatus.New_Ordering);
				throw new ArgumentException("insufficient payment amount","paymentAmount");
			}
			


			bool res = (new Order()).AddNewOrders(ref orders);

			if (!res) {
				RemovePayments(ref orders);
				SetOrdersState(ref orders, OrderStatus.New_Ordering);

			}
			return res;
		}
*/		
		private void SetOrdersState(ref ArrayList orders, OrderStatus state) {
			foreach (OrderInfo orderInfo in orders) {
				orderInfo.Status = state;
			}
		}

		private void RemovePayments(ref ArrayList orders) {
			foreach (OrderInfo orderInfo in orders) {
				orderInfo.Payments.Clear();
			}
		}

		public bool UpdateDeliveryDetails(OrderInfo orderInfo) {
			return (new Order()).UpdateDeliveryDetails(orderInfo);
		}
		
		[Obsolete()]
		public bool UpdatePDInfo(int orderID, int upNo, int psNo) {
			return (new Order()).UpdatePDInfo(orderID, upNo, psNo);
		}
		public bool UpdatePDInfo(ref OrderInfo orderInfo) {
			//orderInfo.Amounts.RecalculateTotalAmount();
			return (new Order()).UpdatePDInfo(orderInfo);
		}
		
		public bool UpdateDDInfo(OrderInfo orderInfo) {
			return (new Order()).UpdateDDInfo(orderInfo);
		}

		public bool UpdateMailingInfo(OrderInfo orderInfo) {
			return (new Order()).UpdateMailingInfo(orderInfo);
		}
/*
		public bool SendOrderToDepartment(int orderId,OrderStatus currState, string toDepartment) {
			//TODO: add sending email
			return new Order().SendOrderToDepartment(orderId, currState, toDepartment);
			//throw new NotImplementedException();
		}*/
		public bool SendOrderBackToCustomer(int orderId, string senderComment, string sender) {
			return SendOrderBackToCustomer(orderId, senderComment, sender);
			//TODO: add email notification
		}

		public bool SendOrderForCustomerApproval(ref OrderInfo orderInfo, string senderUID) {
			return new Order().SendOrderForCustomerApproval(ref orderInfo, senderUID);
			//TODO: add email notification
		}
		
		public bool SetOrderApproved(ref OrderInfo orderInfo, string customerComments) {
			return new Order().SetOrderApproved(ref orderInfo, customerComments);
		}
		
		public bool SetOrderDeclined(ref OrderInfo orderInfo, string customerComments) {
			return new Order().SetOrderDeclined(ref orderInfo, customerComments);
		}

/*		public bool SendOrderToDepartment(OrderInfo orderInfo, string toDepartment, string senderUID) {
			return new Order().SendOrderToDepartment(orderInfo, toDepartment, senderUID);
			//TODO: add email notification
		}

		public bool SendOrderToDepartment(ref OrderInfo orderInfo, string toDepartment, string senderUID) {
			return new Order().SendOrderToDepartment(ref orderInfo, toDepartment, senderUID);
			//TODO: add email notification
		}
*/		
		/// <summary>
		/// Set order state as being printed ("InPrint")
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <returns></returns>
		public bool SendOrderInPrint(ref OrderInfo orderInfo, string senderUID) {
			return new Order().SendOrderInPrint(ref orderInfo, senderUID);
			
		}

		public bool SetOrderPrinted(ref OrderInfo orderInfo, string senderUID) {
			return new Order().SetOrderPrinted(ref orderInfo, senderUID);
			//TODO: add email notification
		}
		
		public bool SetOrderShippedOrWaitingPickUp(ref OrderInfo orderInfo) {
			/*if (new Order().SetOrderShippedWaitingPickUp(orderInfo)) {
				orderInfo.Status = OrderStatus.Shipped_WaitingPickUp;
				return true;
			} else {
				return false;
			}*/
			return new Order().SetOrderShippedWaitingPickUp(ref orderInfo);
			//TODO: add email notification
		}
		
		public bool SetOrderDeliveredPickedUp(ref OrderInfo orderInfo, string senderUID) {
			return new Order().SetOrderDeliveredPickedUp(ref orderInfo, senderUID);
		}

		public bool CloseOrder(ref OrderInfo orderInfo, string senderUID) {
			return new Order().CloseOrder(ref orderInfo, senderUID);
		}
		
		public bool CancelOrder(ref OrderInfo orderInfo, string emploeeId) {
			return new Order().CancelOrder(ref orderInfo, emploeeId);
		}

		#region Design
		
		public void CreateEmptyDesignContainer(ref OrderInfo orderInfo){
			PrintingTypePart[] parts = (new Printing()).GetPrintingTypeParts(AppConfig.siteId,orderInfo.OrderJob.PrintingTypeID);
			PartDesign partDesign;
			PartDesign[] partsDesign;
			ArrayList pts = new ArrayList();
			foreach (PrintingTypePart part in parts) {
				partDesign = new PartDesign();
				partDesign.OrderId = orderInfo.OrderId;
				partDesign.PartId = part.PartId;
				partDesign.FileName = String.Empty;
				partDesign.IsModified = true;
				pts.Add(partDesign);
			}
			partsDesign = new PartDesign[pts.Count];
			pts.CopyTo(partsDesign);
			orderInfo.Design = new JobDesign(partsDesign);
		}
		public void CreateEmptyDesignContainer(ref OrderInfo orderInfo, PartDesignFileCategory fileCategory){
			PrintingTypePart[] parts = (new Printing()).GetPrintingTypeParts(AppConfig.siteId,orderInfo.OrderJob.PrintingTypeID);
			PartDesign partDesign;
			PartDesign[] partsDesign;
			ArrayList pts = new ArrayList();
			foreach (PrintingTypePart part in parts) {
				partDesign = new PartDesign();
				partDesign.OrderId = orderInfo.OrderId;
				partDesign.PartId = part.PartId;
				partDesign.FileName = String.Empty;
				partDesign.IsModified = true;
				partDesign.FileCategory = fileCategory;
				pts.Add(partDesign);
			}
			partsDesign = new PartDesign[pts.Count];
			pts.CopyTo(partsDesign);
			switch (fileCategory) {
				case PartDesignFileCategory.CompletedDesign:
					orderInfo.Design.Parts = partsDesign;
					break;
				case PartDesignFileCategory.DesignPreview:
					orderInfo.Design.PartPreviews = partsDesign;
					break;
			}
			
		}
		
		public void LoadPartDesign(ref OrderInfo orderInfo) {
			orderInfo.Design.Parts = (new Order()).GetPartsDesign(orderInfo.OrderId);
			if (!orderInfo.Design.IsPartDesignsLoaded) {
				CreateEmptyDesignContainer(ref orderInfo,PartDesignFileCategory.CompletedDesign);
			}
		}

		public void LoadPartDesignPreviews(ref OrderInfo orderInfo) {
			orderInfo.Design.PartPreviews = (new Order()).GetPartsDesignPreviews(orderInfo.OrderId);
			if (!orderInfo.Design.IsPartDesignPreviewsLoaded) {
				CreateEmptyDesignContainer(ref orderInfo, PartDesignFileCategory.DesignPreview);
			}
		}

		public bool SavePartsDesign(ref OrderInfo orderInfo/*PartDesign[] parts*/, string filesDir) {
			return (new Order()).UpdatePartsDesign(ref orderInfo, filesDir);
		}

		public bool SetNewOrderDesignIsUploaded(int orderId) {
			return (new Order().SetNewDesignIsUploaded(orderId));
		}

		public bool SavePartDesignPreviews(ref OrderInfo orderInfo/*PartDesign[] parts*/, string filesDir) {
			return (new Order()).UpdatePartDesignPreviews(ref orderInfo, filesDir);
		}

		public void LoadAuxFiles(ref OrderInfo orderInfo) {
			orderInfo.Design.AuxFiles = new Order().GetAuxFiles(orderInfo.OrderId).GetHashtable();
		}
/*		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderInfo">order info which design part should be updated</param>
		/// <param name="partsForUpdate">ArrayList</param>
		public void SavePartsDesign(ref OrderInfo orderInfo, ref ArrayList partsForUpdate) {
				
		}
*/
		#endregion
		
		#region AmountCalculation

		public OrderInfo.PaymentAmounts CalculatePaymentAmounts(OrderInfo orderinfo) {
			throw new Exception("Not implemented yet");
		}
		//TODO: remove this	
		[Obsolete("Use CalculateBaseJobAmount instead",true)]
		public decimal CalculateJobAmount(OrderInfo.JobInfo job) {
			Printing printing = new Printing();
			//FIXME: implement Printing method that return single price item and use it instead of this 
			PrintingPrice[] priceinfo = printing.GetPrices(AppConfig.siteId, job.PrintingTypeID, job.PaperTypeID, job.PaperSizeID, job.Quantity);
			//TODO: add extra calculation
			return priceinfo[0].Price;
		}
		public void CalculateBaseJobAmount(ref OrderInfo orderinfo) {
			orderinfo.Amounts.CalculatedAmount = (new Order()).CalculateBaseJobAmount(AppConfig.siteId, orderinfo.OrderJob);
			// TODO: consider adding total amount recalculation here
		}
		public decimal CalculateShippingAmount(OrderInfo orderInfo) {
			
			//orderInfo.DeliveryDetails.Carrier = carrier;
			
			// get packaging info for the current job
			(new PackagingFacade()).Determine(ref orderInfo );
			
			//FIXME: no need now to create a ne object
			AddressInfo destination = new AddressInfo();
			
			destination.Country = orderInfo.DeliveryDetails.ShipAddress.Country;
			destination.ZipCode = orderInfo.DeliveryDetails.ShipAddress.ZipCode;
			destination.State = orderInfo.DeliveryDetails.ShipAddress.State;

			DateTime shipmentDate = DateTime.Now.AddDays(1);//FIXME:!!!
			//FIXME: float to decimal
			decimal declaredValue = orderInfo.Amounts.DeclaredAmount; // Package cost

			ShipManager shipManager = new ShipManager();

			decimal result = shipManager.RatePackage(orderInfo.DeliveryDetails.Carrier, orderInfo.DeliveryDetails.ShipMethod, destination, /*packagingInfo*/orderInfo.DeliveryDetails.Packaging, orderInfo.DeliveryDetails.ShipDate, declaredValue);
			return result;
		}

		public void CalculateShippingAmount(ref OrderInfo orderInfo) {
			try {
				orderInfo.Amounts.ShippingAmount = CalculateShippingAmount(orderInfo);
				orderInfo.IsValidDeliveryDetails = true;
				//orderInfo.Amounts.RecalculateTotalAmount();
			} catch (ShipmentManagerException ex) {
				//orderInfo.DeliveryDetails.IsValid = false;
				orderInfo.IsValidDeliveryDetails = false;
				orderInfo.DeliveryDetails.ShipMethod = null;
				throw ex;
			} /*finally {
				orderInfo.Amounts.RecalculateTotalAmount();
			}*/

		}

		/// <summary>
		/// return downpayment customer has to pay for the orders
		/// </summary>
		/// <remarks></remarks>
		/// <param name="orders"></param>
		/// <param name="customerId"></param>
		/// <returns>Downpayment required from Customer for the orders</returns>
		public decimal GetDownpaimentRequired(OrderInfo[] orders, string customerId) {
			// TODO: get the downpayment persent for the customer specified by customerId param
			decimal downpayment = 0.00m;
			foreach (OrderInfo order in orders) {
				//order.Amounts.RecalculateTotalAmount();
				order.DownpaymentRequired = order.Amounts.TotalAmount;
				downpayment += order.Amounts.TotalAmount;
			}
			return downpayment;
		}
		
		/*public decimal CalculateRemainingAmount(params int[] orderIdList) {
			if (orderIdList==null) {
				throw new ArgumentNullException();
			}
			if (orderIdList.Length==0) return 0.00m;

			return new Order().CalculateRemainingAmount(orderIdList);
		}
		public decimal CalculateRemainingAmount(params string[] orderIdList) {
			if (orderIdList==null) {
				throw new ArgumentNullException();
			}
			if (orderIdList.Length==0) return 0.00m;

			int[]intOrderIdList = new int[orderIdList.Length];
			try {
				for(int i=0;i<orderIdList.Length;i++) {
					intOrderIdList[i] = Int32.Parse(orderIdList[i]);
				}
				return CalculateRemainingAmount(intOrderIdList);
			} catch {
				throw new ArgumentException();
			}

		}*/


		public decimal CalculateRemainingAmounts(out ArrayList amounts, params string[] orderIdList) {
			if (orderIdList==null) {
				throw new ArgumentNullException();
			}
			if (orderIdList.Length==0) { 
				amounts = new ArrayList();
				return 0.00m;
			}

			int[]intOrderIdList = new int[orderIdList.Length];
			try {
				for(int i=0;i<orderIdList.Length;i++) {
					intOrderIdList[i] = Int32.Parse(orderIdList[i]);
				}
			} catch {
				throw new ArgumentException();
			}
			return CalculateRemainingAmounts(out amounts,intOrderIdList);

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="amounts"><code>ArrayList</code> of PaymentInfo. each item represent payment that is required to fully pay out the corresponding order</param>
		/// <param name="orderIdList">ids of orders to be calculated</param>
		/// <returns>total remaining amount for the given order</returns>
		public decimal CalculateRemainingAmounts(out ArrayList amounts, params int[] orderIdList) {
			
			if (orderIdList==null) {
				throw new ArgumentNullException();
			}
			amounts = new ArrayList();
			if (orderIdList.Length==0) return 0.00m;
			
			Order order = new Order();
			OrderInfo orderInfo;
			//DateTime nowTS = DateTime.Now;
			decimal totalAmount = 0.00m;

			for (int i=0; i<orderIdList.Length; i++) {
				orderInfo = order.GetOrderInfo(orderIdList[i]);
				decimal amountToPay = orderInfo.Amounts.TotalAmount-orderInfo.TotalAmountPaid;
				if (amountToPay>0.00m) {
					totalAmount+=amountToPay;

					PaymentInfo paymentInfo = new PaymentInfo();
					paymentInfo.Amount = amountToPay;
					paymentInfo.OrderId = orderIdList[i];
					paymentInfo.PaymentType = OrderPaymentType.RemainingAmoount;
					
					amounts.Add(paymentInfo);
				}
			}

			return totalAmount;
			
		}
		#endregion

/*		
		public void GetPackagingInfo(ref OrderInfo orderinfo) {
			orderinfo.DeliveryDetails.Packaging = GetPackagingInfo(orderinfo.DeliveryDetails.Carrier, orderinfo.OrderJob);
		}

		public PackagingInfo GetPackagingInfo(PostalCarrier carrier, OrderInfo.JobInfo jobInfo) {
			return (new Packaging()).Determine(carrier, jobInfo);
		}*/
		public void CalculateTax(ref OrderInfo orderInfo) {
			orderInfo.Amounts.TaxAmount = new Order().CalculateTax(orderInfo,ShipmentManagerConfigHandler.DefaultSenderAddress);
			//orderInfo.Amounts.RecalculateTotalAmount();
		}

		#region Mailing lists
		public ArrayList GetMailingLists(int orderId) {
			return new MailingList().GetMailingLists(orderId);
		}

		public MailingListInfo GetMailListInfo(int mailingListId) {
			return new MailingList().GetInfo(mailingListId);
			
		}
		public MailingListInfo GetMailListInfo(int mailingListId, bool loadFile) {
			return new MailingList().GetInfo(mailingListId,loadFile);
		}

		public bool AddMailList(ref MailingListInfo listInfo, string fileDir) {
			if (!Path.IsPathRooted(fileDir)) {
				throw new ArgumentException("fileDir should contain a rooted path to the directory that contain design file","fileDir");
			}

			if (listInfo==null) {
				throw new ArgumentNullException("listInfo","'listInfo' param not found");
			}

			if (listInfo.IsEmpty) {
				throw new ArgumentOutOfRangeException("listInfo","listInfo name shouldn't be empty");
			}
			byte[] fileData = null;
			FileStream fs = null;
			try {
				string filePath = Path.Combine(fileDir,listInfo.FileName);
				// TODO: add try-catch block here
				fs = new FileStream(filePath,FileMode.Open,FileAccess.Read);
				fileData = new Byte[fs.Length];
				fs.Read(fileData,0,(int)fs.Length);
			} catch(Exception ex) {
				return false;
			} finally {
				if (fs!=null) {
					fs.Close();
				}
			}
			listInfo.MailingListBlob = fileData;
			
			bool res = new MailingList().Add(ref listInfo);
			listInfo.MailingListBlob = null;
			return res;
		}

		public bool UpdateListData(MailingListInfo listInfo) {
			return new MailingList().UpdateListData(listInfo);
		}

		public bool RemoveMailingList(int mailingListId) {
			return new MailingList().Remove(mailingListId);
		}
		#endregion

		#region Shipped packages

		#endregion
	}
}
