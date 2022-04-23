using System;
using System.Collections;

using HiRes.Common;
using HiRes.DAL;

namespace HiRes.BusinessRules {
	/// <summary>
	/// </summary>
	public class PromoCode {
		/// <summary>
		/// this routine is used to get promo code info.
		/// </summary>
		/// <param name="code">promotion code to get info about</param>
		/// <param name="codeInfo">promotion code info that corresponds to given <code>code</code>.</param>
		/// <returns><code>true</code> if there is such code in db. <code>false</code> if no</returns>
		public PromoCodeInfo GetCodeInfo(string code/*, out PromoCodeInfo codeInfo*/) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				return pcDAL.GetCodeInfo(code);
			}
		}
		
		public bool AddCode(PromoCodeInfo codeInfo) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				return pcDAL.Add(codeInfo);
			}
		}

		public void DeleteCode(string code) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				pcDAL.Delete(code);
			}
		}

		public void SetCodeState(string code, PromoCodeInfo.PromoCodeState state) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				pcDAL.SetCodeState(code,state);
			}
		}

		public void SuspendCode(string code) {
			SetCodeState(code,PromoCodeInfo.PromoCodeState.Suspended);
		}

		public int GetTimesUsedByCustomer(string code, int siteId, string CustomerId) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				return pcDAL.GetTimesUsedByCustomer(code,siteId, CustomerId);
			}
		}

		public int GetTotalTimesUsed(string code) {
			throw new NotImplementedException();
		}

		public void AddCodeUsage(string code, int siteId, string CustomerId, int orderId) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				pcDAL.AddCodeUsage(code,siteId, CustomerId, orderId);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Return filtered and sorted array of promotion codes.
		/// Used to be a base method for other promotion codes getters
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="orderBy"></param>
		/// <returns>Return filtered and sorted array of promotion codes.</returns>
		public PromoCodeInfo[] GetPromoCodes(FilterExpression filter, OrderExpression orderBy) {
			using (PromoCodesDAL pcDAL = new PromoCodesDAL()) {
				return pcDAL.GetPromoCodes(filter,orderBy);
			}
		}

		/// <summary>
		/// Return promotion codes issued for all customers of the given site.
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="orderBy"></param>
		/// <returns>Return promotion codes issued for all customers of the given site.</returns>
		public PromoCodeInfo[] GetUniversalPromoCodes(int siteId, OrderExpression orderBy) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return promotion codes issued for all customers of the given site those can be used (not used yet or used less than maximum number of times) for the given customer.
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="customerId"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		public PromoCodeInfo[] GetUniversalPromoCodes(int siteId, string customerId, OrderExpression orderBy) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return active promotion codes issued for all customers of the given site.
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		public PromoCodeInfo[] GetActiveUniversalPromoCodes(int siteId, OrderExpression orderBy) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return "active" promotion codes issued for all customers of the given site those can be used (not used yet or used less than maximum number of times) for the given customer.
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="customerId"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		public PromoCodeInfo[] GetActiveUniversalPromoCodes(int siteId, string customerId, OrderExpression orderBy) {
			throw new NotImplementedException();
		}
		/// <summary>
		/// Return all codes issued for the given customer regardless their state
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="customerId"></param>
		/// <param name="orderBy">sorting order</param>
		/// <returns>Return all codes issued for the given customer regardless their state</returns>
		public PromoCodeInfo[] GetCustomerPromoCodes(int siteId, string customerId, OrderExpression orderBy) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return "active" codes issued for the given customer.
		/// </summary>
		/// <param name="siteId"></param>
		/// <param name="customerId"></param>
		/// <param name="orderBy"></param>
		/// <returns>Return "active" codes issued for the given customer.</returns>
		public PromoCodeInfo[] GetActiveCustomerPromoCodes(int siteId, string customerId, OrderExpression orderBy) {
			throw new NotImplementedException();
		}
		
		public ArrayList GetCustomerRecipients(PromoCodeInfo codeInfo) {
			ArrayList customersList;
			if ((codeInfo.CustomerUID!=null)&&(!codeInfo.CustomerUID.Equals(String.Empty))) {
				customersList = new ArrayList();
				customersList.Add(new Customer().GetInfo(codeInfo.CustomerUID,codeInfo.SiteId));
				//return customersList;
			} else {
				if (codeInfo.UsageConditions.FirstTimeCustomerOnly) {
					customersList = new ArrayList();
					//return customersList; //return empty list - we're not going to send mail for "new customers promotion"
				}
				
				FilterExpression filter = new FilterExpression(typeof(CustomerFields));
				filter[CustomerFields.SiteId] = codeInfo.SiteId;
				customersList = new ArrayList(new Customer().GetCustomers(filter,null));
			}
			return customersList;
		}
		//-------------------------------------------------------------------------
	}
}
