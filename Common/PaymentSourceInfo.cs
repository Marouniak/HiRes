using System;

namespace HiRes.Common {

	public enum PaymentCard {
		NoCard = -1,
		Visa = 4,
		MasterCard = 5,
		AmericanExpress = 6,
		DinersClub = 7,
		Discover = 8,
		JCB = 9
	}

	/// <summary>
	/// RFU: When other payment instruments except payment cards will be added
	/// </summary>
	public enum PaymentInstrument : short {
		None = 0,
		Visa = 4,
		MasterCard = 5,
		AmericanExpress = 6,
		DinersClub = 7,
		Discover = 8,
		JCB = 9,
		PayPal = 100,
		TeleCheck = 101,
		Cash = 102,
		MoneyOrder = 103,
		Check = 104
	}

	public enum PaymentInstrumentType : short {
		None = 0,
		Card = 1,
		Cash,
		Check,
		ECheck,
		MoneyOrder
	}

	public struct PaymentSourceInfo {

		public PaymentInstrument PaymentInstrument;

		public string AccountNumber;

		public string PayeeName;
		public string CompanyName;
		public string SecurityCode;

		private DateTime _validTo; 
		private string _validToMonthYear;
		public DateTime ValidTo {
			get { return _validTo; }
			set {
				_validTo = value;
				_validToMonthYear = _validTo.Month.ToString("00")+_validTo.Year.ToString().Substring(2,2);
			}
		}

		/*public PaymentSourceInfo(string accountNumber) {
			this.PaymentInstrument = PaymentInstrument.None;
			this.AccountNumber = accountNumber;
		}*/
		public string ValidToMonthYear {
			get {
				return _validToMonthYear;
			}
		}

		public PaymentInstrumentType PaymentInstrumentType {
			get {
				switch (PaymentInstrument) {
					case PaymentInstrument.AmericanExpress:
						return PaymentInstrumentType.Card;
					case PaymentInstrument.DinersClub:
						return PaymentInstrumentType.Card;
					case PaymentInstrument.Discover:
						return PaymentInstrumentType.Card;
					case PaymentInstrument.JCB:
						return PaymentInstrumentType.Card;
					case PaymentInstrument.MasterCard:
						return PaymentInstrumentType.Card;
					case PaymentInstrument.Visa:
						return PaymentInstrumentType.Card;
					case PaymentInstrument.Cash:
						return PaymentInstrumentType.Cash;
					case PaymentInstrument.TeleCheck:
						return PaymentInstrumentType.ECheck;
					case PaymentInstrument.MoneyOrder:
						return PaymentInstrumentType.MoneyOrder;
					case PaymentInstrument.Check:
						return PaymentInstrumentType.Check;
					default :
						return PaymentInstrumentType.None;
				}
			}
		}
	}
}
