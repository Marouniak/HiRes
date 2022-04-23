using System;

namespace HiRes.Common {
	public struct IntIDName {
		private int id;
		private string name;
		public int Id {
			get { return id;}
			set { id = value;}
		}
		public string Name {
			get { return name;}
			set { name = value;}
		}
	}

	public class IdName {
		private string id;
		private string name;
		public string Id {
			get { return id;}
			set { id = value;}
		}
		public string Name {
			get { return name;}
			set { name = value;}
		}
	}
}
