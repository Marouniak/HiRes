using System;
using System.Security.Principal;

namespace HiRes.Common.Security {
	public enum UserRoles {Customer = 0, ODEmployee, DDEmployee, PDEmployee, Admin};


	
	/// <summary>
	/// custom implementation of IPrincipal interface
	/// </summary>
	/*	public class HiResPrincipal : GenericPrincipal {
	//		private UserRoles _role;

			public HiResPrincipal( IIdentity identity, string[] roles, UserRoles defaultRole ):base(identity,roles) {
				if (roles.Length<1) {
					throw new ArgumentException("'roles' array should contain at least one item","roles");
				}
				_role = defaultRole;
			}

			public UserRoles Role {
				get { return _role; }
			}
		
			public bool IsInRole( UserRoles role ) {
				return this._role.Equals(role);
			}
			public override bool IsInRole( string role ) {
				return this._role.Equals(UserRoles.Parse(typeof(UserRoles),role,true));
			}
		}*/
}
