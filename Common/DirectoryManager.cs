using System;
using System.Web;

namespace HiRes.Common {
	/// <summary>
	/// DirectoryManager encapsulates all the operations related to customer folder management.
	/// </summary>
	public class DirectoryManager {
		
		public const string DesignSubDirName = "design";
		public const string AuxFilesSubDirName = "auxfiles";

		
		#region employees-related
		public static string GetEmployeeUploadDestinationVirtualPath(string employeeID) {
			// FIXME: consider how to improve the security
			return AppConfig.UploadDirectory+"/"+employeeID.GetHashCode();
		}
		public static string GetEmployeeUploadDestinationAbsolutePath(string employeeID) {
			return HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath+"/"+GetEmployeeUploadDestinationVirtualPath(employeeID));
		}
/*
		public static string GetCurrentUserUploadDestinationVP() {
			return AppConfig.UploadDirectory+"/"+HttpContext.Current.User.Identity.Name.GetHashCode();
		}

		public static string GetCurrentUserUploadDestinationAbsolutePath() {
			return HttpContext.Current.Server.MapPath(GetCurrentUserUploadDestinationVP());
		}
*/
		public static void CreateEmployeeDirectories(string employeeID) {
			System.IO.Directory.CreateDirectory(
				HttpContext.Current.Server.MapPath(GetEmployeeUploadDestinationVirtualPath(employeeID))
				);
		}

		public static bool IsEmployeeDirExist(string employeeID) {
			return System.IO.Directory.Exists(
				HttpContext.Current.Server.MapPath(GetEmployeeUploadDestinationVirtualPath(employeeID))
				);
		}

		public static void EnsureEmployeeDirCreated(string employeeID) {
			
			if (!IsEmployeeDirExist(employeeID)) {
				CreateEmployeeDirectories(employeeID);
			}
		}


		#endregion
		/// <summary>
		/// Get virtual path to customer directory
		/// </summary>
		/// <param name="context"></param>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public static string GetCustomerUploadDestinationVirtualPath(string customerID) {
			// FIXME: consider how to improve the security
			return AppConfig.UploadDirectory+"/"+customerID.GetHashCode();
		}
		public static string GetCustomerUploadDestinationAbsolutePath(string customerID) {
			return HttpContext.Current.Server.MapPath(GetCustomerUploadDestinationVirtualPath(customerID));
		}

		public static void CreateCustomerDirectories(HttpContext context, String customerID) {
			System.IO.Directory.CreateDirectory(
					context.Server.MapPath(GetCustomerUploadDestinationVirtualPath(customerID))
			);
		}

		public static bool IsCustomerDirExist(HttpContext context, String customerID) {
			return System.IO.Directory.Exists(
				context.Server.MapPath(GetCustomerUploadDestinationVirtualPath(customerID))
			);
		}

		public static void EnsureCustomerDirCreated(HttpContext context, String customerID) {
			
			if (!IsCustomerDirExist(context, customerID)) {
				CreateCustomerDirectories(context, customerID);
			}
		}

		#region order dirs

		public static string GetOrderDirVirtualPath(String customerID, int orderId) {
			return GetCustomerUploadDestinationVirtualPath(customerID)+"/"+orderId;
		}
		public static string GetOrderDirAbsolutePath(String customerID, int orderId) {
			return HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath +"/" +GetOrderDirVirtualPath(customerID,orderId));
		}
		
		public static string GetOrderDirAbsolutePath1(String customerID, int orderId) {
			string vpath = GetOrderDirVirtualPath(customerID,orderId);
			string absPath = HttpContext.Current.Server.MapPath(vpath);
			return HttpContext.Current.Server.MapPath(GetOrderDirVirtualPath(customerID,orderId));
		}

		public static bool IsOrderDirExist(HttpContext context, String customerID, int orderId) {
			return System.IO.Directory.Exists(
				GetOrderDirAbsolutePath(customerID,orderId)
				);
		}

		public static void CreateOrderDir(HttpContext context, String customerID, int orderId) {
			System.IO.Directory.CreateDirectory(
				GetOrderDirAbsolutePath(customerID,orderId)
				);
		}

		public static void EnsureOrderDirCreated(HttpContext context, String customerID, int orderId) {
			
			if (!IsOrderDirExist(context, customerID, orderId)) {
				CreateOrderDir(context, customerID, orderId);
			}
//			EnsureDesignDirCreated(context, customerID, orderId);
		}



		#endregion


		#region proof design

		public static String GetDesignVirtualPath(String customerID, int orderId) {
			return GetOrderDirVirtualPath(customerID,orderId)+"/"+DesignSubDirName;
		}
		/// <summary>
		/// return an absolute path to the directory intended to
		/// </summary>
		/// <param name="customerID"></param>
		/// <param name="orderId"></param>
		/// <returns></returns>
		/// <remarks>
		/// The path it returns depends on context, in other words it returns a different path for different
		/// web-apps that use this routine
		/// </remarks>
		public static string GetDesignDirAbsolutePath(String customerID, int orderId) {
			return HttpContext.Current.Server.MapPath(GetDesignVirtualPath(customerID, orderId));
		}
		public static bool IsDesignDirExist(HttpContext context, String customerID, int orderId) {
			return System.IO.Directory.Exists(
				context.Server.MapPath(GetDesignVirtualPath(customerID,orderId))
				);
		}

		public static void CreateDesignDir(HttpContext context, String customerID, int orderId) {
			System.IO.Directory.CreateDirectory(
				context.Server.MapPath(GetDesignVirtualPath(customerID,orderId))
				);
		}

		public static void EnsureDesignDirCreated(HttpContext context, String customerID, int orderId) {
			
			if (!IsDesignDirExist(context, customerID, orderId)) {
				CreateDesignDir(context, customerID, orderId);
			}
		}

		#endregion

		#region AuxFiles

		public static String GetAuxFilesVirtualPath(String customerID, int orderId) {
			HttpContext context = HttpContext.Current;
			return GetDesignVirtualPath(customerID,orderId)+"/"+AuxFilesSubDirName;
		}

		public static bool IsAuxFileDirExist(string customerID, int orderId) {
			HttpContext context = HttpContext.Current;
			return System.IO.Directory.Exists(
				context.Server.MapPath(GetAuxFilesVirtualPath(customerID,orderId))
				);
		}

		public static void CreateAuxFilesDir(String customerID, int orderId) {
			HttpContext context = HttpContext.Current;
			System.IO.Directory.CreateDirectory(
				context.Server.MapPath(GetAuxFilesVirtualPath(customerID,orderId))
				);
		}

		public static void EnsureAuxFilesDirCreated(string customerID, int orderId) {
			HttpContext context = HttpContext.Current;
			if (!IsAuxFileDirExist(customerID,orderId)) {
				CreateAuxFilesDir(customerID,orderId);
			}
		}

		#endregion
	}
}
