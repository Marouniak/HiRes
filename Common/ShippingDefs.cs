using System;
using System.Collections;

using HiRes.Common;

namespace HiRes.Common.ShippingDefs {

	public class ShippingServiceInfo {
		//private static ShippingServiceInfo _empty;
		private string id;
		private string name;
		private string dispLabel;

		public string Id {
			get { return id;}
			set { id = value;}
		}
		public string Name {
			get { return name;}
			set { name = value;}
		}
		public string DispLabel {
			get { return dispLabel;}
			set { dispLabel = value;}
		}
		/*public static ShippingServiceInfo EmptyService {
			get {
				if (_empty == null) {
					_empty = new 
				}
				return _empty;
			}
		}*/
	}
	
	public class PostalCarrierInfo {
		private string id;
		private string name;
		private string dispLabel;
		private ArrayList _availableServices;

		public string Id {
			get { return id;}
			set { id = value;}
		}
		public string Name {
			get { return name;}
			set { name = value;}
		}
		public string DispLabel {
			get { return dispLabel;}
			set { dispLabel = value;}
		}
		public ArrayList AvailableServices {
			get { return _availableServices;}
			//set { _availableServices = value;}
		}

		public int AddService(ShippingServiceInfo service) {
			return _availableServices.Add(service);
		}

		public void ClearServices() {
			_availableServices.Clear();
		}
		#region constructors
		public PostalCarrierInfo() {
			_availableServices = new ArrayList();
		}
		#endregion
	}
	
	public struct ShippingServicePrice {
		private string carrierId;
		private string serviceId;
		private string serviceDispLabel;
		private decimal price;
		public bool IsAvailable;

		public string CarrierId {
			get { return carrierId;}
			set { carrierId = value;}
		}
		public string ServiceId {
			get { return serviceId;}
			set { serviceId = value;}
		}
		public string ServiceDispLabel {
			get { return serviceDispLabel;}
			set { serviceDispLabel = value;}
		}
		
		public decimal Price {
			get { return price;}
			set { price = value;}
		}

		public ShippingServicePrice(string carrierId, string serviceId, decimal price) {
			this.carrierId = carrierId;
			this.serviceId = serviceId;
			this.serviceDispLabel = string.Empty;
			this.price = price;
			this.IsAvailable = true;
		}
	}


}
