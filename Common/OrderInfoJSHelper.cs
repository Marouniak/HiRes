using System;
using System.Globalization;

namespace HiRes.Common {
	/// <summary>
	/// Summary description for OrderInfoJSHelper.
	/// </summary>
	public class OrderInfoJSHelper {

		public static string GenerateAmountsDefScript() {
				string defScript = @"
				<script language='javascript'>
				function Amounts() {
					this.calculatedAmount = 0.00;

					this.designHours = 0.00;
					this.designCost  = 0.00;
					this.isDesignCostFixed = false;

					//this.designAmount = 0.00;
					this.discountAmount = 0.00;
					this.extrasAmount = 0.00;
					this.shippingAmount = 0.00;
					this.taxAmount = 0.00;

					this.imagesAmount = 0.00;
					this.scanAmount = 0.00;
					this.proofAmount = 0.00;

					this.postageCostPerPiece = 0.00;
					this.postageQnt = 0.00; // postage quantity
					this.mailingPrcCostPerM = 0.00; // Processing cost per 1000
					this.mailingPrcQnt = 0.00; // Processing quantity
					this.diskPrepcost = 0.00;// Disk preparation cost
					this.mailingListsTotalCost = 0.00;

					this.totalAmount = 0.00;
					this.subTotalAmount = 0.00;

					this.recalculateTotal = _recalculateTotalAmount
					this.getTotalMailingAmount = _getTotalMailingAmount
					this.getTotalDesignAmount = _getTotalDesignAmount

					function _getTotalMailingAmount() {
						return this.postageCostPerPiece*this.postageQnt+(this.mailingPrcCostPerM*this.mailingPrcQnt)/1000+this.diskPrepcost+this.mailingListsTotalCost;
					}
					function _getTotalDesignAmount() {
						if(this.isDesignCostFixed) {
							return this.designCost;
						} else {return this.designCost*this.designHours;}
					}
					
					function _recalculateTotalAmount() {
						mailingAmount = this.getTotalMailingAmount();
						designAmount = this.getTotalDesignAmount();
						this.totalAmount = this.calculatedAmount+this.extrasAmount+this.taxAmount+designAmount+this.imagesAmount+this.scanAmount+this.proofAmount+mailingAmount+this.shippingAmount-this.discountAmount;
						this.subTotalAmount = this.calculatedAmount+this.extrasAmount+designAmount+mailingAmount+this.shippingAmount-this.discountAmount;
					}
				}
				</script>";
			return defScript;
		}
		
		public static string GenerateOrderAmountsScript(string varId, OrderInfo.PaymentAmounts amounts) {
			string script = @"
				<script language='javascript'>
				var "+varId+@" = new Amounts();
				
				"+varId+@".calculatedAmount = " +amounts.CalculatedAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".designHours =  " +amounts.DesignHours.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".designCost =  " +amounts.DesignCost.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".isDesignCostFixed =  " +amounts.IsDesignCostFixed.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture)+@"

				"+varId+@".discountAmount =  " +amounts.DiscountAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".extrasAmount =  " +amounts.ExtrasAmount.ToString(CultureInfo.InvariantCulture)+@"

				"+varId+@".imagesAmount =  " +amounts.ImagesAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".scanAmount =  " +amounts.ScanAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".proofAmount =  " +amounts.ProofAmount.ToString(CultureInfo.InvariantCulture)+@"

				"+varId+@".postageCostPerPiece =  " +amounts.PostageCostPerPiece.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".postageQnt =  " +amounts.PostageQuantity.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".mailingPrcCostPerM =  " +amounts.MailingPrcCostPerM.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".mailingPrcQnt =  " +amounts.MailingPrcQuantity.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".diskPrepcost =  " +amounts.DiskPrepCost.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".mailingListsTotalCost =  " +amounts.MailingListsTotalCost.ToString(CultureInfo.InvariantCulture)+@"

				"+varId+@".shippingAmount =  " +amounts.ShippingAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".taxAmount =  " +amounts.TaxAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".totalAmount =  " +amounts.TotalAmount.ToString(CultureInfo.InvariantCulture)+@"
				"+varId+@".subTotalAmount =  " +amounts.SubTotalAmount.ToString(CultureInfo.InvariantCulture)+@"				
				</script>";
			return script;
		}
	}
}
