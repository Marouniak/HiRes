using System;

namespace HiRes.Common {

	public sealed class OrderLog {
		public const string OrderLogRootTag = "ENTRIES";
		public enum EventCategory {
			LifeCycleGeneral,
			Design
		}

		public sealed class Entry {
			public int OrderId;
			public DateTime TimeStamp;
			public string Category;
			public string Name;
			public string Description;
			public string EmployeeUID;
		}

		// Order lifecycle event names
		public const string EVT_Order_Placed = "Placed";
		public const string EVT_Order_NewDesignUploaded = "NewDesignUploaded";
		public const string EVT_Order_SendToDept = "SentToDepartment";
		public const string EVT_Order_Cancelled = "Cancelled";
		public const string EVT_Order_Closed = "Closed";
		
		public const string EVT_PD_SentInPrint = "SentInPrint";
		public const string EVT_PD_SetPrinted = "SetPrinted";

		public const string EVT_OD_SetDelivered = "SetDelivered";
		public const string EVT_OD_SendBackToCustomer = "SendBackCustomer";
		
		// design event names
		public const string EVT_DD_SentForApproval = "SentForApproval";
		public const string EVT_DD_Approved = "Approved";
		public const string EVT_DD_Declined = "Declined";
	
		/*		public const string EVT_Design_SentToCustomer = "SentToCustomer";
				public const string EVT_Design_Approved = "Approved";
				public const string EVT_Design_Declined = "Declined";
		*/
		#region event descriptions
		public const string DESC_SendForApproval = "Order was sent for customer approval.";
		public const string DESC_Customer_Approved = "Order design was approved by customer.";
		public const string DESC_Customer_Declined = "Order design was declined by customer.";
		
		public const string DESC_PD_SentInPrint = "Order was sent in print.";
		public const string DESC_PD_SetPrinted = "Order state was changed to 'Printed'.";

		public const string DESC_OD_SetDelivered = "Order state was changed to 'Delivered/Picked Up'.";
		public const string DESC_OD_Closed = "Order was closed.";
		public const string DESC_OD_Cancelled = "Order cancelled.";

		#endregion

		private string orderLogXml;
		private string designLogXml;

		public string OrderLogXml {
			get { return orderLogXml; }
			set { orderLogXml = value; }
		}
		public string DesignLogXml {
			get { return designLogXml; }
			set { designLogXml = value; }
		}

		public static Entry CreateEntry(int orderId, EventCategory catgry, string name, string description) {
			Entry entry = new Entry();
			entry.OrderId = orderId;
			entry.TimeStamp = DateTime.Now;
			entry.Category = catgry.ToString();
			entry.Name = name;
			entry.Description = description;
			return entry;
		}
		public static string CreateSendToDescription(string department) {
			return "Order was sent to "+department;
		}
		
		public static string CreateCustomerApprovedDescription(string customerDescription) {
			return DESC_Customer_Approved+"\n"+customerDescription;
		}
		public static string CreateCustomerDeclinedDescription(string customerDescription) {
			return DESC_Customer_Declined+"\n"+customerDescription;
		}
		//public static string CreateSendForCustomerApprovalDescription(
	}
}
