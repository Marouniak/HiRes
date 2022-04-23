using System;
using System.Collections;

namespace HiRes.Common {

	public sealed class OrderPromoCodes {
		
		private ArrayList _usedPromoCodes;

		public OrderPromoCodes() {
			_usedPromoCodes = new ArrayList();
		}

		/// <summary>
		/// return collection of the promotion codes used
		/// </summary>
		public ICollection Codes {
			get { return (ICollection)_usedPromoCodes; }
		}

		public void AddCode(string code) {
			_usedPromoCodes.Add(code);
		}

		public bool Contains(string code) {
			return _usedPromoCodes.Contains(code);
		}
		
		public string GetFirst() {
			return _usedPromoCodes.Count>0?(string)_usedPromoCodes[0]:null;
		}

		public void RemoveFirst() {
			if (_usedPromoCodes.Count<=0) { return; }

			_usedPromoCodes.RemoveAt(0);
		}
	}
}
