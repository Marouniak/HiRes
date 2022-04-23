using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Caching;

using HiRes.BusinessFacade;
using HiRes.Common;

using HiRes.ShipmentManager.FedEx;
using HiRes.ShipmentManager.UPS;

namespace HiRes.Web.Common.Components {
	/// <summary>
	/// Serves as a facade for pages that hides the caching related application logic
	///     <remarks> 
	///         Pages should use this class to get the info cached from the db 
	///         such as Printing Type Names, Paper Sizes etc.
	///         The class hides depnedency, cache expiration and other logic.
	///         Special Considerations:
	///         The OnApplicationStart function in this class must be called
	///         from the Application_OnStart event in Global.asax. This is
	///         currently used to determine the Cache object to cache data within.
	///         <example>
	///             The global.asax file should be similar to the following code:
	///             <code>
	///                     void Application_OnStart() {
	///                         CacheManager.OnApplicationStart(Context);
	///                     }
	///             </code>
	///         </example>
	///         TODO: consider write-locking
	///     </remarks>
	///     
	///     TODO: Make Printings Site use this class
	/// </summary>
	
	public class CacheManager {

		public static Cache Cache;
		public static HttpContext Context;

		public const String KEY_CACHED_PRINTINGTYPESNAMES = "PrintingTypesNames:";
		public const String KEY_CACHED_PRINTINGTYPES = "PrintingTypes:";
		public const string KEY_CACHED_ALLPAPERTYPES = "AllPaperTypes:";
		public const String KEY_CACHED_EXTRAPRICES = "ExtraPrices:";
		public const String KEY_CACHED_SHIPPING_METHODS = "ShippingMethods:";

		public const string KEY_DEF_PRINTINGTYPE = "DefaultPrintingTypeId:";

		public const String KEY_CACHED_COUNTRIES = "Countries:";
		public const String KEY_CACHED_STATES = "States:";

		//		private String _cacheKey;

		#region private

		//private static int _defaultPrintingTypeId;
		/*private static int _defaultPaperSizeId;
		private static int _defaultPaperTypeId;*/

		private static void EnsurePrintingTypesLoaded() {
			if (Cache[KEY_CACHED_PRINTINGTYPES]==null) {
				Cache[KEY_CACHED_PRINTINGTYPES] = (new IdObjectAdapter((new PrintingsFacade()).GetPrintingTypes(),"PrintingTypeID")).AdaptedCollection;
			}
		}
		
		private static void EnsureExtraPricesAreLoaded(int printingTypeId) {
			string cacheId = KEY_CACHED_EXTRAPRICES + printingTypeId;
			if (Cache[cacheId]==null) {
				//Cache[cacheId] = new PrintingsFacade().GetExtraPrices(printingTypeId);
				Cache[cacheId] = new PrintingsFacade().GetFullExtraPrices(printingTypeId);
			}
		}
		#endregion

		#region Printings

		public static PrintingTypeInfo DefaultPrintingType {
			get {
				if (Cache[KEY_DEF_PRINTINGTYPE]==null) {
					IEnumerator e = PrintingTypes.Values.GetEnumerator();
					e.Reset();
					e.MoveNext();
					Cache[KEY_DEF_PRINTINGTYPE] = (PrintingTypeInfo)e.Current;
				}
				
				return (PrintingTypeInfo)Cache[KEY_DEF_PRINTINGTYPE];
			}
		}
		/// <summary>
		/// Return NameValueCollection that contain printing type id/names mapping
		/// </summary>
		public static NameValueCollection PrintingTypesNames {
			get {
				if (Cache[KEY_CACHED_PRINTINGTYPESNAMES]==null) {
					Cache[KEY_CACHED_PRINTINGTYPESNAMES] = (new IdNameAdapter((new PrintingsFacade()).GetPrintingTypesNames(),"PrintingTypeID","PrintingTypeName")).AdaptedCollection;
				}
				return (NameValueCollection)Cache[KEY_CACHED_PRINTINGTYPESNAMES];
			}
		}
		
		public static Hashtable PrintingTypes {
			get {
				/*if (Cache[KEY_CACHED_PRINTINGTYPES]==null) {
					Cache[KEY_CACHED_PRINTINGTYPES] = (new IdObjectAdapter((new PrintingsFacade()).GetPrintingTypes(),"PrintingTypeID")).AdaptedCollection;
				}*/
				EnsurePrintingTypesLoaded();
				
				return (Hashtable)Cache[KEY_CACHED_PRINTINGTYPES];
			}
			
		}
		/// <summary>
		/// Return Hashtable that contain Paper Sizes  id mapping
		/// TODO: consider cashing sizes seprately instead of retrieving it the from printingtypes object
		/// </summary>
		/// <param name="printingTypeId"></param>
		/// <returns></returns>
		public static Hashtable PaperSizes(int printingTypeId) {
			//EnsurePrintingTypesLoaded();
			PrintingTypeInfo pti = (PrintingTypeInfo)PrintingTypes[printingTypeId/*.ToString()*/];
			if (pti!=null) {
				return (new IdObjectAdapter(pti.PaperSizes,"PaperSizeID")).AdaptedCollection;
			} else { return null; }
		} 

		/// <summary>
		/// Return Hashtable that contain Paper Types id mapping
		/// TODO: consider cashing types seprately instead of retrieving it the from printingtypes object
		/// </summary>
		public static Hashtable PaperTypes(int printingTypeId, int PaperSizeId) {
			
			//EnsurePrintingTypesLoaded();
			PrintingTypeInfo pti = (PrintingTypeInfo)PrintingTypes[printingTypeId/*.ToString()*/];
			if (pti!=null) {
				ArrayList prta = new ArrayList(((PaperSizeInfo)(new IdObjectAdapter(pti.PaperSizes,"PaperSizeID")).AdaptedCollection[PaperSizeId]).PaperTypes);
					
				//					prta.ToArray(typeof(PaperTypeInfo));
				return (
					(Hashtable)(new IdObjectAdapter(
					prta.ToArray(typeof(PaperTypeInfo)),"PaperTypeID"
					)).AdaptedCollection
					);
			} else { return null; }
		}

		public static Hashtable AllPaperTypes(int printingTypeId) {
			string key = EnsureAllPaperTypesCached(printingTypeId);
			return (Hashtable)Cache[key];
		}

		private static string EnsureAllPaperTypesCached(int printingTypeId) {
			string key = KEY_CACHED_PRINTINGTYPES+printingTypeId+":"+KEY_CACHED_ALLPAPERTYPES;

			if (Cache[key]==null) {
				Hashtable _allPaperTypes = new Hashtable();
				PrintingTypeInfo pti = (PrintingTypeInfo)PrintingTypes[printingTypeId];

				foreach(PaperSizeInfo psz in pti.PaperSizes) {
					foreach(PaperTypeInfo paperTypeInfo in psz.PaperTypes) {
						if (_allPaperTypes[paperTypeInfo.PaperTypeID]==null) {
							_allPaperTypes[paperTypeInfo.PaperTypeID] = paperTypeInfo;
						}
					}
				}
				Cache[key] = _allPaperTypes;

				/*if (HttpContext.Current.Cache[key]!=null) {
					;
				}*/
			}
			return key;
		}
		/// <summary>
		/// Return Hashtable that contain Extras id mapping
		/// TODO: consider cashing extras seprately instead of retrieving it the from printingtypes object
		/// </summary>
		public static Hashtable Extras(int printingTypeId) {
			
			//EnsurePrintingTypesLoaded();
			PrintingTypeInfo pti = (PrintingTypeInfo)PrintingTypes[printingTypeId];
			if (pti!=null) {
				return pti.Extras;
				//				return (new IdObjectAdapter(pti.Extras,"ExtraID")).AdaptedCollection;
			} else { return null; }				
					
		}

		public static Hashtable ExtrasPrices(int printingTypeId) {
			EnsureExtraPricesAreLoaded(printingTypeId);
			return (Hashtable)Cache[KEY_CACHED_EXTRAPRICES + printingTypeId];
		}

		public static Hashtable PrintingParts(int printingTypeId) {
			PrintingTypeInfo pti = (PrintingTypeInfo)PrintingTypes[printingTypeId];
			if (pti!=null) {
				return (new IdObjectAdapter(pti.Parts,"PartId")).AdaptedCollection;
			} else { return null; }				
		}

		#endregion

		#region Shipping
		
		public static Hashtable GetShippingMethods(PostalCarrier carrier) {
			EnsureShippingServiceInfoLoaded();
			/*			Hashtable carriers = (Hashtable)Cache[KEY_CACHED_SHIPPING_METHODS];
						Hashtable methods = (Hashtable)(carriers[carrier]);*/
			return (Hashtable)((Hashtable)Cache[KEY_CACHED_SHIPPING_METHODS])[carrier];
			
		}
		
		// TODO: consider loading shipping methods from db
		private static void EnsureShippingServiceInfoLoaded() {
			if (Cache[KEY_CACHED_SHIPPING_METHODS]==null) {
				Hashtable carriers = new Hashtable();
				foreach( PostalCarrier carrier in Enum.GetValues(typeof(PostalCarrier))) {
					carriers.Add(carrier, ConvertMethodEnums(carrier));
				}
				Cache[KEY_CACHED_SHIPPING_METHODS] = carriers;
			}
		}

		private static Hashtable ConvertMethodEnums(PostalCarrier carrier) {
			IdNameEnumAdapter adapter;
			switch (carrier) {
				case PostalCarrier.UPS: {
					adapter = new IdNameEnumAdapter(typeof(UpsServiceCodes));
					// HACK: hack to support "0x"-like shipmethod codes
					adapter.AddLeadZero = true;
					return adapter.AdaptedHashtable;
				}
				case PostalCarrier.FedEx: {
					adapter = new IdNameEnumAdapter(typeof(FedExShippingService));
					// HACK: hack to support "0x"-like shipmethod codes
					adapter.AddLeadZero = true;
					return adapter.AdaptedHashtable;
				}
			}
			return null;
		}

		#endregion

		#region Country and Sates codes
		public static ArrayList Countries {
			get { 
				if(Cache[KEY_CACHED_COUNTRIES]==null) {
					Cache[KEY_CACHED_COUNTRIES] = 
						(new CountriesAndStateCodes(Context.Server.MapPath("Countries.xml"))).GetCodeNames();
				}
				ArrayList res = (ArrayList)Cache[KEY_CACHED_COUNTRIES];
				return res;
			}
		}
		public static ArrayList States {
			get { 
				if(Cache[KEY_CACHED_STATES]==null) {
					Cache[KEY_CACHED_STATES] = (new CountriesAndStateCodes(Context.Server.MapPath("States.xml"))).GetCodeNames();
				}
				ArrayList res = (ArrayList)Cache[KEY_CACHED_STATES];
				return res;
			}
		}		
		#endregion
		#region public methods
		/// <summary>
		/// This method should be invoked by
		/// </summary>
		/// <param name="context">HTTPContext to get Cache from</param>
		public static void OnApplicationStart (HttpContext context) {
			Cache = context.Cache;
			Context = context;
		}

		/// <summary>
		/// Invalidates cache 
		/// </summary>
		private static void InvalidateCache() {
			throw new Exception("Not implenented yet");
		}
		#endregion
		

	}
}
