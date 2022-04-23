using System;

namespace HiRes.DAL {
	/// <summary>
	/// Summary description for DALException.
	/// </summary>
	public class DALException : Exception {
		public DALException(string msg) : base(msg) {}
		public DALException(string msg, Exception ex) : base(msg,ex) {}
	}
}
