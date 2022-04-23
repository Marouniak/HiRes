using System;
using System.Collections;
using HiRes.Common;
using HiRes.Common.Security;
using HiRes.BusinessRules;

namespace HiRes.BusinessFacade {
	/// <summary>
	/// Summary description for EmployeeFacade.
	/// </summary>
	public class EmployeeFacade {

		public EmployeeInfo GetInfo(string employeeId) {
			Employee employee = new Employee();
			return employee.GetInfo(employeeId);
		}

		public bool CheckLogin(string employeeId, string passwd) { 
			Employee employee = new Employee();
			return employee.CheckLogin(employeeId, passwd);
		}

		public bool AddEmployee (EmployeeInfo info, string passwd) {
			Employee employee = new Employee();
			return employee.AddEmployee(info, passwd);
		}

		public bool UpdateInfo (EmployeeInfo info) {
			Employee employee = new Employee();
			return employee.UpdateInfo(info);
		}
		public bool UpdatePassword(string employeeId, string passwd) {
			Employee employee = new Employee();
			return employee.UpdatePassword(employeeId,passwd);
		}
		public bool RemoveEmployee (string employeeId) {
			Employee employee = new Employee();
			return employee.RemoveEmployee(employeeId);
		}

		public ArrayList GetEmployees(FilterExpression filter, OrderExpression order) {
			Employee employee = new Employee();
			return employee.GetEmployees(filter, order);
		}

		public ArrayList GetOrderedDesginers() {
			FilterExpression filterDisigners = new FilterExpression(typeof(EmployeeFields));
			filterDisigners.Add(EmployeeFields.Roles,UserRoles.DDEmployee.ToString());
			OrderExpression orderDesigners = new OrderExpression(typeof(EmployeeFields));
			orderDesigners[(int)EmployeeFields.UID] = Order_By_Expression.ASC;
			return new Employee().GetEmployees(filterDisigners,orderDesigners);
		}
		
	}
}
