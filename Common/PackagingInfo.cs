using System;
using System.Runtime.Serialization;

namespace HiRes.Common {
	/// <summary>
	/// 
	/// </summary>
	[Serializable()]
	public class PackagingInfo : ISerializable, ICloneable {
		public const int ALL_ITEMS = -1;

		public int PrintingTypeId;
		public int PaperSizeId; 
		public int PaperTypeId;
		public PostalCarrier Carrier;
		public int Quantity;
		public decimal Width;
		public decimal Height;
		public decimal Length;
		public decimal Weight;
		public int BoxesNumber;

		private String _carrierPackageTypeId;
		
		#region ISerializable
		public PackagingInfo(SerializationInfo info, StreamingContext context) {
			PrintingTypeId = (int)info.GetValue("PrintingTypeId",typeof(int));
			PaperSizeId = (int)info.GetValue("PaperSizeId",typeof(int));
			PaperTypeId = (int)info.GetValue("PaperTypeId",typeof(int));
			Carrier = (PostalCarrier)info.GetValue("Carrier",typeof(PostalCarrier));
			Quantity = (int)info.GetValue("Quantity",typeof(int));
			Width = (decimal)info.GetValue("Width",typeof(decimal));
			Height = (decimal)info.GetValue("Height",typeof(decimal));
			Length = (decimal)info.GetValue("Length",typeof(decimal));
			Weight = (decimal)info.GetValue("Weight",typeof(decimal));
			BoxesNumber = (int)info.GetValue("BoxesNumber",typeof(int));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("PrintingTypeId",PrintingTypeId);
			info.AddValue("PaperSizeId",PaperSizeId);
			info.AddValue("PaperTypeId",PaperTypeId);
			info.AddValue("Carrier",Carrier);
			info.AddValue("Quantity",Quantity);
			info.AddValue("Width",Width);
			info.AddValue("Height",Height);
			info.AddValue("Length",Length);
			info.AddValue("Weight",Weight);
			info.AddValue("BoxesNumber",BoxesNumber);
		}
		#endregion

		public PackagingInfo() {}

		public String CarrierPackageTypeId {
			get { return _carrierPackageTypeId; }
			set { _carrierPackageTypeId = value; }
		}
		public bool IsCarrierPackaging {
			get {
				return (_carrierPackageTypeId!=null);
			}
		}
		
		public override string ToString() {
			return "L:"+Length.ToString()+":W:"+Width.ToString()+":H:"+Height.ToString()+":Weight:"+Weight.ToString()+":PackageTypeId:"+CarrierPackageTypeId;
		}

		public static string GetKey(PackagingInfo pInfo) {
			//return pInfo.Carrier.ToString()+":"+pInfo.PrintingTypeId+":"+pInfo.PaperSizeId+":"+pInfo.PaperTypeId+":"+pInfo.Quantity;
			return GetKey(pInfo.Carrier,pInfo.PrintingTypeId,pInfo.PaperSizeId,pInfo.PaperTypeId,pInfo.Quantity);
		}
		public string GetKey() {
			//return pInfo.Carrier.ToString()+":"+pInfo.PrintingTypeId+":"+pInfo.PaperSizeId+":"+pInfo.PaperTypeId+":"+pInfo.Quantity;
			return PackagingInfo.GetKey(this);
		}

		public static string GetKey(PostalCarrier Carrier, int PrintingTypeId,int PaperSizeId,int PaperTypeId,int Quantity) {
			return Carrier.ToString()+":"+PrintingTypeId+":"+PaperSizeId+":"+PaperTypeId+":"+Quantity;
		}

		public static string GetKey(PostalCarrier Carrier, OrderingItemInfo oi) {
			return GetKey(Carrier,oi.PrintingTypeID,oi.PaperSizeID,oi.PaperTypeID,oi.Quantity);
		}

		public object Clone() {
			PackagingInfo clony = new PackagingInfo();

			clony.PrintingTypeId = PrintingTypeId;
			clony.PaperSizeId = PaperSizeId;
			clony.PaperTypeId = PaperTypeId;
			clony.Carrier = Carrier;
			clony.Quantity = Quantity ;
			clony.Width = Width;
			clony.Height = Height;
			clony.Length = Length;
			clony.Weight = Weight;
			clony.BoxesNumber = BoxesNumber;
			
			return clony;
		}

	}

	public enum PackagingsFields {
		NONE = 0,
		PrintingTypeID,
		PaperSizeID,
		PaperTypeID,
		CarrierID,
		Quantity,
		Height,
		Width,
		Length,
		Weight,
		CarrierPackagingTypeID,
		BoxesNumber
	}

}
