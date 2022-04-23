using System;
using System.EnterpriseServices;

namespace HiRes.DAL {
	/// <remarks>
	///	Instances of the DAO class inherit from ServicedComponent and overrides
	/// the ServicedComponent.Deactivate method to assure SqlConnection's are 
	/// disposed as soon at the end of every transaction, whether it's completed
	/// or aborted.
	/// </remarks>
	[Obsolete("",true)]
	public abstract class DAOSC : ServicedComponent {
		//internal StoredProcedure sproc = null;

		override protected void Deactivate() {

		}
	}

}
