using System;
using System.Collections;
using System.Collections.Specialized;

namespace HiRes.Common {
	public enum OrderAction {
		SendForProof,
		Approve,
		Decline,
		Cancel,
		Close
	}

	/// <summary>
	/// Summary description for OrderFSM.
	/// </summary>
	public class OrderFSM {
		private static OrderStatus[] _activeStates = {OrderStatus.Delivered_PickedUp,OrderStatus.Approved, OrderStatus.WaitingForProof,OrderStatus.InDesign,OrderStatus.InPrint,OrderStatus.New_DesignIsUploaded,OrderStatus.New_WaitingUpload,OrderStatus.Printed,OrderStatus.Shipped_WaitingPickUp};
		private static OrderStatus[] _inactiveStates = {OrderStatus.Cancelled, OrderStatus.Closed};
		
		private static IList _activeStatesList;
		private static IList _inactiveStatesList;

		public static IList ActiveStatesList {
			get { return _activeStatesList; }
		}

		public static IList InactiveStatesList {
			get { return _inactiveStatesList; }
		}
		static OrderFSM() {
			IList list = (IList)_activeStates;
			_activeStatesList = (IList)ArrayList.ReadOnly(_activeStates);
			_inactiveStatesList = (IList)ArrayList.ReadOnly(_inactiveStates);
		}

		public static bool GetNewState(OrderInfo orderInfo, OrderAction action,  out OrderStatus toState) {
			toState = orderInfo.Status;
			switch (action) {
				case OrderAction.Approve:
					if (orderInfo.Status==OrderStatus.WaitingForProof) {
						toState = OrderStatus.Approved;
						return true;
					} else { return false; }
				case OrderAction.Decline:
					if (orderInfo.Status==OrderStatus.WaitingForProof) {
						if (orderInfo.OrderJob.IsDesignOrdered) {
							toState = OrderStatus.InDesign;
						} else {
							toState = OrderStatus.New_DesignIsUploaded;
						}
						return true;
					} else { return false; }
				case OrderAction.SendForProof: 
					toState = OrderStatus.WaitingForProof;
					return true;
				default:
					throw new NotImplementedException();
			}
		}
		/// <summary>
		/// wrapper method around GetNewState for callers those only need to know whether transition is available to the given department
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="toDepartment"></param>
		/// <returns></returns>
		[Obsolete("",true)]
		public static bool IsTransitionAvailable(OrderInfo orderInfo, string toDepartment) {
			OrderStatus toState;
			return GetNewState(orderInfo, toDepartment, out toState);
		}
		//public static bool Get
		[Obsolete("",true)]
		public static bool GetNewState(OrderInfo orderInfo, string toDepartment, out OrderStatus toState) {
			/*OrderStatus toState = orderInfo.Status;
			newState = toState;*/
			toState = orderInfo.Status;
			/*switch (orderInfo.Status) {
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
				case OrderStatus.InDesign:
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

			}*/
			return true;
		}
		
		/*public static ArrayList GetTransitionStatesAvailable (OrderStatus state) {
			ArrayList availList = new ArrayList();
		}*/
		/*public static bool GetNewState(OrderInfo orderInfo, string toDepartment, out OrderStatus toState) {
		}*/
	}

	public class OrderStateHelper {

		/// <summary>
		/// Returns short label for the given state.
		/// The short label it is used in dropdownbox, lists etc.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public static string GetStateShortLabel(OrderStatus state) {
			switch(state) {
				case OrderStatus.Delivered_PickedUp: return "Delivered/Picked Up"; break;
				case OrderStatus.Approved: return "Approved"; break;
				case OrderStatus.WaitingForProof: return "Pending Proof"; break;
				case OrderStatus.InDesign: return "In Design"; break;
				case OrderStatus.InPrint: return "In Print"; break;
				case OrderStatus.New_DesignIsUploaded: return "Design Uploaded"; break;
				case OrderStatus.New_WaitingUpload: return "Pending upload"; break;
				/*case OrderStatus.Printing_Approved: return "Printing is approved"; break;
				case OrderStatus.Printing_PendingUpload: return "Printing - pending upload"; break;
				case OrderStatus.Printing_WaitingForProof: return "Printing - pending proof"; break;*/
				case OrderStatus.Shipped_WaitingPickUp: return "Waiting for pickup"; break;
				default: return state.ToString(); break;
			}
		}

		public static Hashtable GetIdShortLabelCollection(IList states) {
			if (states==null) { throw new ArgumentNullException(); }
			Hashtable res = new Hashtable();
			for (int i=0;i<states.Count;i++) {
				OrderStatus curState = (OrderStatus)states[i];
				res.Add(curState.ToString("d"),GetStateShortLabel(curState));
			}
			return res;
		}
	}
}

