using System;
using System.Collections;
using System.Data;
using System.IO;

using HiRes.Common;
using HiRes.DAL;
using HiRes.ShipmentManager;
using HiRes.SystemFramework.Logging;

namespace HiRes.BusinessRules {
	/// <summary>
	/// Summary description for Order.
	/// </summary>
	public class Order {
		
		public const decimal TAX_DC = 5.75m;
		public const decimal TAX_MD = 5m;
		public OrderInfo GetOrderInfo(int orderId) { 
			OrderInfo orderInfo = null;
			using (OrdersDAL ODal = new OrdersDAL()) {
				orderInfo = ODal.GetOrderInfo(orderId);
			}
			return orderInfo;
		}
		
		public OrderInfo[] GetOrders(FilterExpression filter, OrderExpression orderBy) {
			OrderInfo[] orders = null;
			using (OrdersDAL ODal = new OrdersDAL()) {
				orders = ODal.GetOrders(filter, orderBy);
			}
			return orders;
		}

/*		/// <summary>
		/// Adds new order
		/// </summary>
		/// <remarks>There is no SiteID parameter because SiteID is contained by OrderInfo class</remarks>
		/// <param name="orderInfo"></param>
		public bool AddNewOrder(ref OrderInfo orderInfo) {
			if (orderInfo.OrderJob.JobType==JobType.PrintingOnly) {
				if (orderInfo.Design.AllPartsUploaded) {
					orderInfo.Status = OrderStatus.New_DesignIsUploaded;
				} else {
					orderInfo.Status = OrderStatus.New_WaitingUpload;
				}
			} else if (orderInfo.OrderJob.JobType==JobType.DesignAndPrinting) {
				orderInfo.Status = OrderStatus.New;
			}

			using (OrdersDAL odal = new OrdersDAL()) {
				bool res = odal.AddNewOrder(ref orderInfo);
				// if order were not stored to the db then set the order status back to 'New_Ordering'
				if (!res) {
					orderInfo.Status = OrderStatus.New_Ordering;
				}
				return res;
			}

		}*/

		public bool PlaceNewOrders(ref ArrayList orders, ref PaymentTransactionInfo downpaymentTxn) {
			using (OrdersDAL odal = new OrdersDAL()) {
				
				bool res = true;
				//IDbTransaction transaction = DbTransactionFactory.BeginTransaction();
				IDbTransaction transaction = null;
				try {
					transaction = DbTransactionFactory.BeginTransaction();
					for(int i=0;i<orders.Count; i++) {
						OrderInfo orderInfo = (OrderInfo)orders[i];
						if (!odal.AddNewOrder(ref orderInfo,transaction)) {
							transaction.Rollback();
							return false;
						}
						/*#region move to stored proc
						if (!odal.UpdateMailingInfoTmp(orderInfo, transaction)) {
							transaction.Rollback();
							return false;
						}
						if (!odal.UpdateOrderAmountsTmp(orderInfo, transaction)) {
							transaction.Rollback();
							return false;
						}
						#endregion*/
					}
					using (PaymentsDAL pdal = new PaymentsDAL()) {
						//TODO: add authorize transaction goes here
						res = pdal.AddBaseTransaction(downpaymentTxn,transaction);
					}
					if (res) {
						transaction.Commit();
						return true;
					} else {
						transaction.Rollback();
						return false;
					}
				} catch {
					if (transaction!=null) {
						transaction.Rollback();
					}
					return false;
				}
			}
		}
		
/*		public bool AddNewOrders(ref ArrayList orders) {

			bool res;
			using (OrdersDAL odal = new OrdersDAL()) {
				res = odal.AddNewOrders(ref orders);
				// if order were not stored to the db then set the order status back to 'New_Ordering'
			}
			return res;
		}
*/
		/*public bool AddNewOrders(ref ArrayList orders, PaymentTransactionInfo txn) {
			throw new NotImplementedException();
			using (OrdersDAL odal = new OrdersDAL()) {

				using (PaymentsDAL pdal = new PaymentsDAL()) {
				}
			}
		}*/

		public bool UpdateDeliveryDetails(OrderInfo orderInfo) {
			using (OrdersDAL odal = new OrdersDAL()) {
				/// Update delivery details and amounts because changing of delivery details might affects the shipping amount
				return odal.UpdateOrderInfo(orderInfo,OrderInfoParts.DeliveryDetails|OrderInfoParts.Amounts|OrderInfoParts.OrderStatus,null);
			}
		}

		
		#region order lifecycle
		//fixme:get state from OrderFSM
		public bool SetNewDesignIsUploaded(int orderId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_Order_NewDesignUploaded,"");
				if (odal.UpdateOrderStatus(orderId,OrderStatus.New_DesignIsUploaded,logEntry)) {
					return true;
				} else 
					return false;
			}
		}

		//fixme:get state from OrderFSM
		public bool SendOrderInPrint(ref OrderInfo orderInfo, string senderUID) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_PD_SentInPrint,OrderLog.DESC_PD_SentInPrint);
				logEntry.EmployeeUID = senderUID;
				if (odal.UpdateOrderStatus(orderInfo.OrderId,OrderStatus.InPrint,logEntry)) {
					orderInfo.Status = OrderStatus.InPrint;
					return true;
				} else 
					return false;
			}
		}
		//fixme:get state from OrderFSM
		public bool SetOrderPrinted(ref OrderInfo orderInfo, string senderUID) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_PD_SetPrinted,OrderLog.DESC_PD_SetPrinted);
				logEntry.EmployeeUID = senderUID;
				if (odal.UpdateOrderStatus(orderInfo.OrderId,OrderStatus.Printed,logEntry)) {
					orderInfo.Status = OrderStatus.Printed;
					return true;
				} else 
					return false;
			}
		}

		/// <summary>
		/// TODO: add order logging 
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <returns></returns>
		//fixme:get state from OrderFSM
		public bool SetOrderShippedWaitingPickUp(ref OrderInfo orderInfo) {
			using (OrdersDAL odal = new OrdersDAL()) {
				bool res = false;
				if (orderInfo.DeliveryDetails.PickUpOrder) {
					res = odal.UpdateOrderStatus(orderInfo.OrderId,OrderStatus.Shipped_WaitingPickUp);
				} else {
					res = odal.UpdateOrderInfo(orderInfo,OrderInfoParts.OrderStatus&OrderInfoParts.DeliveryDetails,String.Empty);
				}
				if (res) {
					orderInfo.Status = OrderStatus.Shipped_WaitingPickUp;
				}
				return res;
			}
		}
		
		//fixme:get state from OrderFSM
		public bool SetOrderDeliveredPickedUp(ref OrderInfo orderInfo, string senderUID) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_OD_SetDelivered,OrderLog.DESC_OD_SetDelivered);
				logEntry.EmployeeUID = senderUID;
				if (odal.UpdateOrderStatus(orderInfo.OrderId,OrderStatus.Delivered_PickedUp,logEntry)) {
					orderInfo.Status = OrderStatus.Delivered_PickedUp;
					return true;
				} else { return false; }
			}
		}
		
		//fixme:get state from OrderFSM
		public bool CloseOrder(ref OrderInfo orderInfo, string emploeeId) {
			// TODO: consider checking if order is fully paid.
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_Order_Closed,OrderLog.DESC_OD_Closed);
				logEntry.EmployeeUID = emploeeId;
				if (odal.UpdateOrderStatus(orderInfo.OrderId,OrderStatus.Closed, logEntry)) {
					orderInfo.Status = OrderStatus.Closed;
					return true;
				} else { return false; }
			}
		}
		//fixme:get state from OrderFSM
		public bool CancelOrder(ref OrderInfo orderInfo, string emploeeId) {
			using (IDbTransaction dbTxn = DbTransactionFactory.BeginTransaction()) {
				using (OrdersDAL odal = new OrdersDAL()) {
					OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_Order_Cancelled,OrderLog.DESC_OD_Cancelled);
					logEntry.EmployeeUID = emploeeId;
					if (!odal.UpdateOrderStatusTxn(orderInfo.OrderId,OrderStatus.Cancelled,dbTxn,logEntry)) {
						dbTxn.Rollback();
						return false;
					} else {
						using (PaymentsDAL pdal = new PaymentsDAL()) {
							try {
								orderInfo.Payments = new PaymentTransaction().GetOrderPayments(orderInfo.OrderId);
							} catch (DALException ex) {
								AppLog.LogError("",ex);
								dbTxn.Rollback();
								return false;
							}
							for(int i=0; i<orderInfo.Payments.Count;i++) {
								PaymentInfo pi = (PaymentInfo)orderInfo.Payments[i];
								if (!pdal.CancelPayment(pi.PaymentId,dbTxn)) {
									dbTxn.Rollback();
									return false;
								}
							}
							
						}
						orderInfo.Status = OrderStatus.Cancelled;
						dbTxn.Commit();
						return true;
					}
				}
			}
		}
/*
		public bool SetOrderShipped(OrderInfo orderInfo) {
			if (orderInfo.DeliveryDetails.PickUpOrder) {
				return false;
			} else {
				using (OrdersDAL odal = new OrdersDAL()) {
					return odal.UpdateOrderInfo(orderInfo,OrderInfoParts.OrderStatus&OrderInfoParts.DeliveryDetails,String.Empty);
				}
			}
		}
		
		public bool SetOrderWaitingPickUp(OrderInfo orderInfo) {
			if (!orderInfo.DeliveryDetails.PickUpOrder) {
				using (OrdersDAL odal = new OrdersDAL()) {
					return odal.UpdateOrderStatus(orderInfo.OrderId,OrderStatus.Shipped_WaitingPickUp);
				}
			} else {
				return false;
			}
		}
*/
		//fixme:get state from OrderFSM
		public bool SendOrderBackToCustomer(int orderId, string senderComment, string senderUID) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_OD_SendBackToCustomer,senderComment);
				logEntry.EmployeeUID = senderUID;
				if (odal.UpdateOrderStatus(orderId,OrderStatus.New_WaitingUpload,logEntry)) {
					return true;
				} else 
					return false;
			}
		}

		//fixme:get state from OrderFSM
		public bool SendOrderForCustomerApproval(ref OrderInfo orderInfo, string senderUID) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.Design,OrderLog.EVT_DD_SentForApproval,OrderLog.DESC_SendForApproval);
				logEntry.EmployeeUID = senderUID;
				
				OrderStatus newState;
				bool canBeSent = OrderFSM.GetNewState(orderInfo,OrderAction.SendForProof,out newState);
				
				if (!canBeSent) { return false; }

				if (odal.UpdateOrderStatus(orderInfo.OrderId,newState,logEntry)) {
					orderInfo.Status = newState;
					return true;
				} else 
					return false;
			}
		}

		public bool SetOrderApproved(ref OrderInfo orderInfo, string customerComments) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.Design,OrderLog.EVT_DD_Approved,OrderLog.DESC_Customer_Approved);
				logEntry.Description += "\n";
				logEntry.Description += customerComments;

				OrderStatus newState;
				bool canBeSent = OrderFSM.GetNewState(orderInfo,OrderAction.Approve,out newState);
				
				if (!canBeSent) { return false; }

				if (odal.UpdateOrderStatus(orderInfo.OrderId,newState,logEntry)) {
					orderInfo.Status = newState;
					return true;
				} else 
					return false;
			}
		}
		public bool SetOrderDeclined(ref OrderInfo orderInfo, string customerComments) {
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.Design,OrderLog.EVT_DD_Declined,OrderLog.DESC_Customer_Declined);
				logEntry.Description += "\n";
				logEntry.Description += customerComments;

				OrderStatus newState;
				bool canBeSent = OrderFSM.GetNewState(orderInfo,OrderAction.Decline,out newState);
				
				if (!canBeSent) { return false; }
				
				if (odal.UpdateOrderStatus(orderInfo.OrderId,newState,logEntry)) {
					orderInfo.Status = newState;
					return true;
				} else 
					return false;
			}
		}		
/*
		public bool SendOrderToDepartment(OrderInfo orderInfo, string toDepartment, string senderUID) {
			OrderStatus toState;
			bool res = OrderFSM.GetNewState(orderInfo, toDepartment, out toState);
			if (!res) {
				return false;
			}

			OrderLog.Entry logEntry = OrderLog.CreateEntry(orderInfo.OrderId,OrderLog.EventCategory.LifeCycleGeneral,OrderLog.EVT_Order_SendToDept,OrderLog.CreateSendToDescription(toDepartment));
			logEntry.EmployeeUID = senderUID;
			using (OrdersDAL odal = new OrdersDAL()){
				return odal.UpdateOrderStatus(orderInfo.OrderId,toState,logEntry);
			}
		}

		public bool SendOrderToDepartment(ref OrderInfo orderInfo, string toDepartment, string senderUID) {
			OrderStatus toState;
			if (SendOrderToDepartment(orderInfo, toDepartment, senderUID)) {
				bool res = OrderFSM.GetNewState(orderInfo, toDepartment, out toState);
				orderInfo.Status = toState;
				return true;
			} else
				return false;
		}
*/
		#endregion
/*
		public bool SendOrderToDepartment(int orderId, OrderStatus currState, string toDepartment) {
			OrderStatus toState = currState;
			switch (currState) {
				case OrderStatus.New:
					if (toDepartment.Equals(Departments.Design)) {
						toState = OrderStatus.InDesign;
					} else { return false; }
					break;
				case OrderStatus.New_DesignIsUploaded:
					if (toDepartment.Equals(Departments.Production)) {
						toState = OrderStatus.PrePress;
					} else { return false; }
					break;
				case OrderStatus.Design_Approved:
					if (toDepartment.Equals(Departments.Production)) {
						toState = OrderStatus.PrePress;
					} else { return false; }
					break;
				case OrderStatus.PrePress:
					if (toDepartment.Equals(Departments.Ordering)) {
						toState = OrderStatus.New_WaitingUpload;
					} else { return false; }
					break;
				default: 
					//TODO: consider throwing exception because if we are here then something went wrong
					return false;
					break;
			}

			
			using (OrdersDAL odal = new OrdersDAL()){
				return odal.UpdateOrderStatus(orderId,toState);
			}
		}
*/
		public PartDesign[] GetPartsDesign(int orderId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.LoadPartsDesigns(orderId,PartDesignFileCategory.CompletedDesign);
			}
		}
		public PartDesign[] GetPartsDesignPreviews(int orderId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.LoadPartsDesigns(orderId,PartDesignFileCategory.DesignPreview);
			}
		}
		public byte[]/*MemoryStream*/ GetCompletedPartDesignFile(int orderId, int partId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.GetPartDesignFile(orderId,partId,PartDesignFileCategory.CompletedDesign);
			}
		}

		public byte[] GetPreviewPartDesignFile(int orderId, int partId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.GetPartDesignFile(orderId,partId,PartDesignFileCategory.DesignPreview);
			}
		}
		public AuxFile GetAuxFile(int auxFileId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.GetAuxFile(auxFileId);
			}
			//throw new NotImplementedException();
		}
		/// <summary>
		/// TODO: IMPORTANT: Consider transactional operation after business rules will be made
		/// a strong name assembly.
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <returns></returns>
		public bool UpdatePartsDesign(ref OrderInfo orderInfo, string filesDir) {
			bool res = false;
			using (OrdersDAL odal = new OrdersDAL()) {
//				string filesDir = DirectoryManager.GetDesignDirAbsolutePath(orderInfo.CustomerID,orderInfo.OrderId);
/*				res  = odal.UpdatePartsDesign(orderInfo.Design.Parts,fileDir);

				if (res) {
					if (orderInfo.Design.AllPartsUploaded) {
						orderInfo.Status = OrderStatus.New_DesignIsUploaded;
						odal.UpdateOrderStatus(orderInfo.OrderId, orderInfo.Status);
					}
				}
*/
				OrderStatus oldState = orderInfo.Status;
				/*
				if (orderInfo.Design.AllPartsUploaded) {
					//HACK: it works as long as it's only used for uploading design by customer
					orderInfo.Status = OrderStatus.New_DesignIsUploaded;
				}
				*/
				OrderInfoParts pu = OrderInfoParts.DesignParts/*|OrderInfoParts.OrderStatus*/;
				res = odal.UpdateOrderInfo(orderInfo,pu,filesDir);
/*				if (!res) {
					orderInfo.Status = oldState;
				}*/
			}			

			return res;
		}
		
		public bool UpdatePartDesignPreviews(ref OrderInfo orderInfo, string filesDir) {
			bool res = false;
			using (OrdersDAL odal = new OrdersDAL()) {
				//				string fileDir = DirectoryManager.GetCurrentUserUploadDestinationAbsolutePath();

				OrderInfoParts pu = OrderInfoParts.DesignPreview|OrderInfoParts.OrderStatus;
				res = odal.UpdateOrderInfo(orderInfo,pu,filesDir);
			}			

			return res;
		}
		[Obsolete()]
		public bool UpdatePDInfo(int orderID, int upNo, int psNo) {
			bool res = false;
			using (OrdersDAL odal = new OrdersDAL()) {
				res = odal.UpdatePDInfo(orderID, upNo, psNo);
			}			
			return res;
		}

		public bool UpdatePDInfo(OrderInfo orderInfo) {
			bool res = false;
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderInfoParts pu = OrderInfoParts.PDInfo|OrderInfoParts.Amounts|OrderInfoParts.PDInfo|/*OrderInfoParts.MainInfo|*/OrderInfoParts.OrderStatus|OrderInfoParts.SelectedExtras;
				res = odal.UpdateOrderInfo(orderInfo,pu,null);
			}			
			return res;
		}

		public bool UpdateDDInfo(OrderInfo orderInfo) {
			bool res = false;
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderInfoParts pu = OrderInfoParts.DDInfo|OrderInfoParts.Amounts|OrderInfoParts.DDInfo|/*OrderInfoParts.MainInfo|*/OrderInfoParts.OrderStatus/*|OrderInfoParts.SelectedExtras*/;
				res = odal.UpdateOrderInfo(orderInfo,pu,null);
			}			
			return res;
		}

		public bool UpdateMailingInfo(OrderInfo orderInfo) {
			bool res = false;
			using (OrdersDAL odal = new OrdersDAL()) {
				OrderInfoParts pu = OrderInfoParts.Amounts|OrderInfoParts.MailingInfo|/*OrderInfoParts.MainInfo|*/OrderInfoParts.OrderStatus;
				res = odal.UpdateOrderInfo(orderInfo,pu,null);
			}			
			return res;
		}

		#region stuff to remove
		/*
		public PartDesign[] CreateEmptyDesignParts(int printingTypeID) {
			PrintingTypePart[] parts ;
			PartDesign[] ptsDes;

			using (PrintingsDAL PDal= new PrintingsDAL()) {
				parts =  PDal.GetPrintingTypeParts(printingTypeID);
			}

			ArrayList pts = new ArrayList();
			PartDesign partDesign;

			foreach (PrintingTypePart part in parts) {
				partDesign = new PartDesign();
				partDesign.OrderId = OrderInfo.ID_EMPTY;
				partDesign.PartId = part.PartId;
				partDesign.FileUrl = String.Empty;
				pts.Add(partDesign);
				//partDesign.Part = part;
			}
			ptsDes = new PartDesign[pts.Count];
			pts.CopyTo(ptsDes);
			return ptsDes;
		}*/

/*		public void CreateEmptyDesignParts(ref OrderInfo orderInfo) {
			PrintingTypePart[] parts ;
			using (PrintingsDAL PDal= new PrintingsDAL()) {
				parts =  PDal.GetPrintingTypeParts(orderInfo.OrderJob.PrintingTypeID);
			}
			if (orderInfo.Design == null) {
				orderInfo.Design = new JobDesign();
			}
			//orderInfo.Design.Parts = new PartDesign[parts.Length];
			ArrayList pts = new ArrayList();
			PartDesign partDesign;
			foreach (PrintingTypePart part in parts) {
				partDesign = new PartDesign();
				partDesign.OrderId = orderInfo.OrderId;
				partDesign.PartId = part.PartId;
				pts.Add(partDesign);
				//partDesign.Part = part;
			}
			orderInfo.Design.Parts = new PartDesign[pts.Count];
			pts.CopyTo(orderInfo.Design.Parts);
		}
*/
/*		public PartDesign[] GetEmptyDesignParts(int PrintingTypeID) {
			PrintingTypePart[] parts ;
			PartDesign[] emptyDesignParts;
			using (PrintingsDAL PDal= new PrintingsDAL()) {
				parts =  PDal.GetPrintingTypeParts(orderInfo.OrderJob.PrintingTypeID);
			}
			if (orderInfo.Design == null) {
				orderInfo.Design = new JobDesign();
			}
			//orderInfo.Design.Parts = new PartDesign[parts.Length];
			ArrayList pts = new ArrayList();
			PartDesign partDesign;
			foreach (PrintingTypePart part in parts) {
				partDesign = new PartDesign();
				partDesign.OrderId = orderInfo.OrderId;
				partDesign.PartId = part.PartId;
				pts.Add(partDesign);
				//partDesign.Part = part;
			}
			emptyDesignParts = new PartDesign[pts.Count];
			pts.CopyTo(emptyDesignParts);
			return emptyDesignParts;
		}
*/		
		//---------------------------------------------------
		/*
		public void LoadDeliveryDetails(OrderInfo orderInfo){
			orderInfo.DeliveryDetails = GetDeliveryDetails(orderInfo.OrderId);
			//throw new Exception("Not implemented yet");
		}

		public DeliveryDetailsInfo GetDeliveryDetails(int orderId) {
			throw new Exception("Not implemented yet");
		}
		*/
		//---------------------------------------------------
		#endregion

		/*public decimal CalculateRemainingAmount(params int[] orderIdList) {
			throw new NotImplementedException();
		}*/

		//public bool PlaceTransaction
		public decimal CalculateBaseJobAmount(int siteId, OrderInfo.JobInfo job) {
			
			if (job.IsDesignOrdered&&!job.IsPrintingOrdered) {
				return 0.00m;
			}
			if (job.IsCustomJob) {
				return 0.00m;
			}
			Printing printing = new Printing();
			//FIXME: implement Printing method that return single price item and use it instead of this 
			PrintingPrice[] priceinfo = printing.GetPrices(siteId, job.PrintingTypeID, job.PaperTypeID, job.PaperSizeID, job.Quantity);
			
			return priceinfo[0].Price;
		}

		public decimal CalculateShippingAmount(DeliveryDetailsInfo deliveryInfo) {
			throw new Exception("Not implemented yet");
		}
		
		#region AuxFiles

		public AuxFilesContainer GetAuxFiles(int orderId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.GetAuxFiles(orderId);
			}
		}

//		public bool
		#region
/*		public Hashtable GetAuxFilesByParts(int orderId) {
			AuxFile[] files;
			Hashtable res;
			using (OrdersDAL odal = new OrdersDAL()) {
				files = odal.GetAuxFiles(orderId);
			}
			
			return SortOutAuxFilesByParts(files);
			//return res;
		}
		private Hashtable SortOutAuxFilesByParts(AuxFile[] files) {
			Hashtable res;
			if (files==null) { return null; }
				
			res = new Hashtable();
			foreach (AuxFile file in files) {
				if (!res.ContainsKey(file.PartId)) {
					res.Add(file.PartId, new ArrayList());
				}
				((ArrayList)res[file.PartId]).Add(file);
			}
			return res;

		}*/
		#endregion

		#endregion

		#region order log
		//public bool AddLogEntry(
		public string GetFullOrderLog(int orderId) {
			using (OrdersDAL odal = new OrdersDAL()) {
				return odal.GetFullOrderLog(orderId);
			}
		}

		#endregion

		public decimal CalculateTax(OrderInfo orderInfo,AddressInfo sender) {
			decimal taxPercentage = CalculateTaxPersentage(orderInfo,sender);
			if (taxPercentage>0) {
				return decimal.Round((orderInfo.Amounts.BasePrintingAmount*taxPercentage)/100,2);
			} else { return 0.00m; }
		}

		public decimal CalculateTaxPersentage(OrderInfo orderInfo,AddressInfo sender) {
			if (orderInfo.DeliveryDetails.PickUpOrder) {
				if (sender.State.ToUpper().Equals("DC")) {
					return TAX_DC;
				}
				if (sender.State.ToUpper().Equals("MD")) {
					return TAX_MD;
				}
			} else {
				if (orderInfo.DeliveryDetails.ShipAddress.State.ToUpper().Equals("MD")) {
					return TAX_MD;
				} else
					if (orderInfo.DeliveryDetails.ShipAddress.State.ToUpper().Equals("DC")) {
						return TAX_DC;

					}
			}
			return 0m;
		}


	}
}
