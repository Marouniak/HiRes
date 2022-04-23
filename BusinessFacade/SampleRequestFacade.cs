using System;
using HiRes.Common;
using HiRes.BusinessRules;

namespace HiRes.BusinessFacade {
	/// <summary>
	/// </summary>
	public class SampleRequestFacade {

		public SampleRequestInfo GetInfoByID(int sampleRequestID) {
			SampleRequest sr = new SampleRequest();
			return sr.GetInfo(sampleRequestID , AppConfig.siteId);
		}


		public bool Register(ref SampleRequestInfo srInfo){
			SampleRequest sr = new SampleRequest();
			return sr.Register(ref srInfo,AppConfig.siteId);
		}


		public bool Update(SampleRequestInfo srInfo) {
			SampleRequest sr = new SampleRequest();
			return sr.Update(srInfo, AppConfig.siteId);
		}


		public bool Remove(int sampleRequestID) {
			SampleRequest sr = new SampleRequest();
			return sr.Remove(sampleRequestID, AppConfig.siteId); 
		}

		public SampleRequestInfo[] GetSampleRequests(FilterExpression filter, OrderExpression order) {
			SampleRequest sr = new SampleRequest();
			return sr.GetSampleRequests(filter, order);
		}


	}
}
