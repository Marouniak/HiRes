using System;
using HiRes.DAL;
using HiRes.Common;

namespace HiRes.BusinessRules {
	/// <summary>
	/// This class encapsulate Customer-related operations
	/// </summary>
	public class Customer {

		public bool CheckLogin(int siteId, string customerId, string passwd) {
			bool res = false;
			
			try {
				using (CustomerDAL CDal = new CustomerDAL()){
					res = CDal.CheckLogin(customerId,siteId,passwd);
				}
			} catch {
				throw;
			}
			return res;
		}

		public CustomerInfo GetInfo(String customerId, int siteId) {
			CustomerInfo customerInfo = null;
			using (CustomerDAL CDal = new CustomerDAL()) {
				customerInfo = CDal.GetInfo(customerId, siteId);
			}
			return customerInfo;
		}

		public bool Register(CustomerInfo customerInfo) {
			bool res;
			using (CustomerDAL CDal = new CustomerDAL()) {
				res = CDal.Insert(customerInfo);
			}
			return res;
		}

		public bool UpdateProfile(CustomerInfo customerInfo) {
			bool res;
			using (CustomerDAL CDal = new CustomerDAL()) {
				res = CDal.Update(customerInfo);
			}
			return res;
		}

		public CustomerInfo[] GetCustomers(FilterExpression filter, OrderExpression order) {
			CustomerInfo[] customers = null;
			using (CustomerDAL CDal = new CustomerDAL()) {
				customers = CDal.GetCustomers(filter, order);
			}
			return customers;
		}

		public CustomerAddressInfo[] GetCustomerAddresses(int siteId, String customerId) {
			CustomerAddressInfo[] addresses = null;
			using (CustomerDAL CDal = new CustomerDAL()) {
				addresses = CDal.GetCustomerAddresses(siteId, customerId);
			}
			return addresses;
		}

		public CustomerAddressInfo GetCustomerAddressInfo(int siteId, String customerId, int customerAddressId) {
			CustomerAddressInfo address = null;
			using (CustomerDAL CDal = new CustomerDAL()) {
				address = CDal.GetCustomerAddressInfo(customerId, siteId, customerAddressId);
			}
			return address;
		}

		public int AddCustomerAddress(int siteId, String customerId, CustomerAddressInfo address) {
			int result = -1;
			using (CustomerDAL CDal = new CustomerDAL()) {
				result = CDal.AddCustomerAddressInfo(customerId, siteId, address);
			}
			return result;
		}

		public bool UpdateCustomerAddress(int siteId, String customerId, CustomerAddressInfo address) {
			bool result = false;
			using (CustomerDAL CDal = new CustomerDAL()) {
				result = CDal.UpdateCustomerAddress(customerId, siteId, address);
			}
			return result;
		}

		public bool RemoveCustomerAddress(int siteId, String customerId, int customerAddressId) {
			bool result = false;
			using (CustomerDAL CDal = new CustomerDAL()) {
				result = CDal.RemoveCustomerAddress(customerId, siteId, customerAddressId);
			}
			return result;
		}

		public int GetPlacedOrdersNum(int siteId, string customerId) {
			throw new NotImplementedException();
		}


	}
}
