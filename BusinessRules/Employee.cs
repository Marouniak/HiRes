using System;
using System.Collections;
using HiRes.Common;
using HiRes.DAL;

namespace HiRes.BusinessRules {
	/// <summary>
	/// This class encapsulate employee-related operations
	/// </summary>
	public class Employee {
		public EmployeeInfo GetInfo(string employeeId) {
			EmployeeInfo employeeInfo = null;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				employeeInfo = EDal.GetInfo(employeeId);
			}
			return employeeInfo;
		}

		public bool CheckLogin(string employeeId, string passwd) { 
			bool res;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				res = EDal.CheckLogin(employeeId, passwd);
			}
			return res;
		}

		public bool AddEmployee (EmployeeInfo info, string passwd) {
			bool res;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				res = EDal.Add(info, passwd);
			}
			return res;
		}

		public bool UpdateInfo (EmployeeInfo info) {
			bool res;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				res = EDal.UpdateInfo(info);
			}
			return res;
		}

		public bool UpdatePassword(string employeeId, string passwd) {
			bool res;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				res = EDal.UpdatePassword(employeeId, passwd);
			}
			return res;
		}
		public bool RemoveEmployee (string employeeId) {
			bool res = false;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				res = EDal.Remove(employeeId);
			}
			return res;
		}

		public ArrayList GetEmployees(FilterExpression filter, OrderExpression order) {
			ArrayList employees = null;
			using (EmployeeDAL EDal = new EmployeeDAL()) {
				employees = EDal.GetEmployees(filter,order);
			}
			return employees;
		}
	}
}
