using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace HiRes.Common {

	public delegate string PromoCodeGeneratorDelegate(PromoCodeInfo promoCodeInfo);

	public class PromoCodeGenerator {
		//PromCodeGeneratorDelegate GenerateCode;
		
		public static string Generate(PromoCodeInfo promoCodeInfo) {
			return Generate(promoCodeInfo, new PromoCodeGeneratorDelegate(HexGenerator));
		}

		public static string Generate(PromoCodeInfo promoCodeInfo, PromoCodeGeneratorDelegate generator) {
			return generator(promoCodeInfo);
			//throw new NotImplementedException();
		}

		public static string SimpleGenerator(PromoCodeInfo promoCodeInfo) {
			string baseString = promoCodeInfo.SiteId+":"+promoCodeInfo.CustomerUID+":"+promoCodeInfo.CreatedTS.ToUniversalTime().ToString();
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] data = UnicodeEncoding.ASCII.GetBytes(baseString);
			byte[] hash = md5.ComputeHash(data);
			return UnicodeEncoding.ASCII.GetString(hash);
			//return EncodeToHexView(hash);
		}

		public static string HexGenerator(PromoCodeInfo promoCodeInfo) {
			string baseString = promoCodeInfo.SiteId+":"+promoCodeInfo.CustomerUID+":"+promoCodeInfo.CreatedTS.ToUniversalTime().ToString();
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] data = UnicodeEncoding.ASCII.GetBytes(baseString);
			byte[] hash = md5.ComputeHash(data);
			return EncodeToHexView(hash);
		}

		public static string EncodeToHexView(string code) {
			return EncodeToHexView(UnicodeEncoding.ASCII.GetBytes(code));
		}

		public static string EncodeToHexView(byte[] code) {
			if (code.Length%4!=0) {
				throw new ArgumentException("Byte array length should be divisible by 4","code");
			}

			StringBuilder res = new StringBuilder(code.Length/4+4);
			StringBuilder numBuilder = new StringBuilder(10);
			for (int i=0, num=0; i<code.Length; i++) {
				int mult = i%4;
				int add = ((int)code[i])<<(mult*8);
				num+=((int)code[i])<<(mult*8);
				if (mult==3) {
					//res.AppendFormat("{0:X}",num);
					numBuilder.Remove(0,numBuilder.Length);
					numBuilder.AppendFormat("{0:X}",num);
					while (numBuilder.Length<8) {
						numBuilder.Insert(0,"0");
					}
					res.Append(numBuilder.ToString());
					num = 0;
				}
				
			}

			return res.ToString();
		}

		public static byte[] DecodeFromHexView(string hexCodeString) {
			if (hexCodeString==null) {
				throw new ArgumentNullException("hexCodeString");
			}
			if (hexCodeString.Equals(String.Empty)) {
				throw new ArgumentException("param shouldn't be an empty string","hexCodeString");
			}
			if (hexCodeString.Length%8!=0) {
				throw new ArgumentException("param length should be divisible by 8","hexCodeString");
			}

			byte[] decodedBytes = new byte[hexCodeString.Length];
			for (int i=0; i<hexCodeString.Length; i+=8) {
				string substr = hexCodeString.Substring(i,8);
				int curNum = Int32.Parse(hexCodeString.Substring(i,8),NumberStyles.HexNumber);
				int indexBase = i/2;
				for (int j=4, num=0; j>0; j--) {
					num = (curNum<<24)>>24;
					decodedBytes[indexBase+(4-j)] = Convert.ToByte(num); 
					curNum = (curNum^num)>>8;
				}
			}
			return decodedBytes;//UnicodeEncoding.ASCII.GetString(decodedBytes);
		}

		public static string DecodeFromHexView(string hexCodeString, out string decodedString) {
			decodedString = UnicodeEncoding.ASCII.GetString(DecodeFromHexView(hexCodeString));
			return decodedString;
		}
	}

}
