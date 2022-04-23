using System;

using HiRes.Common;
using HiRes.BusinessRules;

namespace HiRes.BusinessFacade {
	/// <summary>
	/// </summary>
	public class CustomerFacade	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="customerId">Customer ID</param>
		/// <param name="passwd">Password</param>
		/// <returns>true, </returns>
		public bool CheckCustomerLogin(String customerId, String passwd) {
			Customer cust = new Customer();
			return cust.CheckLogin(AppConfig.siteId,customerId,passwd);
		}

		public CustomerInfo GetCustomerInfoByID(String customerId) {
			Customer cust = new Customer();
			return cust.GetInfo(customerId, AppConfig.siteId);
		}

		public CustomerInfo GetCustomerInfoByID(String customerId, int SiteId) {
			Customer cust = new Customer();
			return cust.GetInfo(customerId, SiteId);
		}

		public bool Register(CustomerInfo ci) {
			Customer cust = new Customer();
			return cust.Register(ci);
		}

		public bool UpdateProfile(CustomerInfo ci) {
			Customer cust = new Customer();
			return cust.UpdateProfile(ci);
		}

		public CustomerInfo[] GetCustomers(int siteId, FilterExpression filter, OrderExpression order) {
			filter.Add(CustomerFields.SiteId,siteId);
			return GetCustomers(filter, order);
		}

		public CustomerInfo[] GetCustomers(FilterExpression filter, OrderExpression order) {
			Customer cust = new Customer();
			return cust.GetCustomers(filter, order);
		}

		public CustomerAddressInfo[] GetCustomerAddresses(String customerId) {
			Customer cust = new Customer();
			return cust.GetCustomerAddresses(AppConfig.siteId, customerId);
		}

		public CustomerAddressInfo GetCustomerAddress(String customerId, int customerAddressId) {
			Customer cust = new Customer();
			return cust.GetCustomerAddressInfo(AppConfig.siteId, customerId, customerAddressId);
		}

		public int AddCustomerAddress(String customerId, CustomerAddressInfo address) {
			Customer cust = new Customer();
			return cust.AddCustomerAddress (AppConfig.siteId, customerId, address);
		}

		public bool UpdateCustomerAddress(String customerId, CustomerAddressInfo address) {
			Customer cust = new Customer();
			return cust.UpdateCustomerAddress(AppConfig.siteId, customerId, address);
		}

		public bool RemoveCustomerAddress(String customerId, int customerAddressId) {
			Customer cust = new Customer();
			return cust.RemoveCustomerAddress(AppConfig.siteId, customerId, customerAddressId);
		}
	}
}
