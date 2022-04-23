using System;
using HiRes.DAL;
using HiRes.Common;


namespace HiRes.BusinessRules {
	/// <summary>
	/// This class encapsulate Sample request-related operations
	/// </summary>
	public class SampleRequest {

		public SampleRequestInfo GetInfo(int sampleRequestID, int siteId){
			SampleRequestInfo srInfo = null;
			using (SamplesDAL srDal = new SamplesDAL())	{
				srInfo = srDal.GetInfo(sampleRequestID, siteId);
			}
			return srInfo;
		}


		public bool Register(ref SampleRequestInfo sr, int siteId){
			bool res;
			using (SamplesDAL srDal = new SamplesDAL()) {
				res = srDal.Insert(ref sr, siteId);
			}
			return res;
		}


		public bool Update(SampleRequestInfo sr, int siteId) {
			bool res;
			using (SamplesDAL srDal = new SamplesDAL())	{
				res = srDal.Update(sr, siteId);
			}
			return res;
		}


		public bool Remove(int sampleRequestID, int siteId) {
			bool res = false;
			using (SamplesDAL srDal = new SamplesDAL())	{
				res = srDal.Delete(sampleRequestID, siteId);
			}
			return res;
		}

		public SampleRequestInfo[] GetSampleRequests(FilterExpression filter, OrderExpression order) {
			SampleRequestInfo[] samples = null;
			using (SamplesDAL srDal = new SamplesDAL()) {
				samples = srDal.GetSampleRequests(filter, order);
			}
			return samples;
		}



	
	}
}
