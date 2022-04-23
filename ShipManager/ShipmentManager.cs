/**
 * FILE: ShipmentManager.cs
 * 
 * PROJECT: HiResStudios web application
 * 
 * ABSTRACT: The facade around FedEx and UPS shipment management APIs
 * 
 * LEGAL: (c)2001 Eurosoft International Inc.
 * 
 * Revision history:
 *		17-May-2002	Maxim Lysenko
 *			Initial implementation
 */
using System;
using System.Text;

using HiRes.Common;
using HiRes.ShipmentManager.FedEx;
using HiRes.ShipmentManager.UPS;

using HiRes.ShipmentManager.services;

namespace HiRes.ShipmentManager {
	public enum ShippingServicePrinterType {
		PlainPaper = 01,
		EltronOrion = 02,
		EltronEclipse = 03
	}

	public enum ShippingServiceMediaType {
		ThermalWithDocTab = 03,
		ThermalWithoutDocTab = 04,
		PlainPaperPNG = 05
	}

	public struct ShippingResponseInfo {
		public string TrackingNumber;
		public decimal ChargeAmount;
		public byte[] LabelDataBuffer;
	}

	/// <summary>
	/// ShipManager Exception class
	/// </summary>
	/// <exception cref="System.Exception"> Thrown when ShipManager functions failed </exception>
	public class ShipmentManagerException : Exception {
		public ShipmentManagerException() {
		}
		public ShipmentManagerException(string message) : base(message) {
		}
		public ShipmentManagerException(string message, Exception inner) : base(message, inner) {
		}
	}

	public class ShippingServiceOptionsInfo {
		public bool additionalHandling = false;
		public bool residentalDestination = false;
		public bool saturdayDelivery = false;
		public bool saturdayPickup = false;
	}

	/// <summary>
	/// Implements the facade for UPS andFedEx services
	/// </summary>
	public class ShipManager {
		/// <summary>
		/// Creates the new instance of ShipManager class
		/// </summary>
		public ShipManager() {
		}

		/// <summary>
		/// The function returns the estimated cost of delivery the package(s) to destination address
		/// </summary>
		/// <param name="postalCarrier">The postal carrier to use - UPX or FedEx</param>
		/// <param name="serviceType">The string code of postal carrier's service type</param>
		/// <param name="destination">The destination address. Default sender is HiRes</param>
		/// <param name="packageInfo">The inforamtion about packages in shipment</param>
		/// <param name="orderShippingDate">The date when the shipment have to be delivered (FedEx only)</param>
		/// <param name="declaredValue">The declared value for the package (FedEx only).
		/// FIXME : change shipManager.RatePackage the way it accepts decimal amount
		/// </param>
		/// <returns>The rate of package delivery. For FedEx it may differ from real rate.</returns>
		/// <exception cref="ShipmentManagerException">If function or service fails</exception>
		public decimal RatePackage(PostalCarrier postalCarrier, string serviceType, AddressInfo destination, PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue) {
			// Call RatePackage with the default origin parameter
			return RatePackage(postalCarrier, serviceType, ShipmentManagerConfigHandler.DefaultSenderAddress, destination, packageInfo, orderShippingDate, declaredValue);
		}

		public decimal RatePackage(PostalCarrier postalCarrier, string serviceType, AddressInfo destination, PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue, ShippingServiceOptionsInfo info) {
			// Call RatePackage with the default origin parameter
			return RatePackage(postalCarrier, serviceType, ShipmentManagerConfigHandler.DefaultSenderAddress, destination, packageInfo, orderShippingDate, declaredValue, info);
		}

		/// <summary>
		/// The function returns the estimated cost of delivery the package(s) to destination address
		/// </summary>
		/// <param name="postalCarrier">The postal carrier to use - UPX or FedEx</param>
		/// <param name="serviceType">The string code of postal carrier's service type</param>
		/// <param name="origin">The origin address.</param>
		/// <param name="destination">The destination address.</param>
		/// <param name="packageInfo">The information about packages in shipment</param>
		/// <param name="orderShippingDate">The date when the shipment have to be delivered (FedEx only)</param>
		/// <param name="declaredValue">The declared value for the package (FedEx only).
		/// FIXME : change shipManager.RatePackage the way it accepts decimal amount.
		/// </param>
		/// <returns>The rate of package delivery. For FedEx it may differ from real rate.</returns>
		/// <exception cref="ShipmentManagerException">If function or service fails</exception>
		public decimal RatePackage(PostalCarrier postalCarrier, string serviceType, AddressInfo origin, AddressInfo destination, PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue) {
			decimal result = 0.0m;
			try {
				switch(postalCarrier) {
					case PostalCarrier.FedEx: 
						FedExAPI fedExAPI = new FedExAPI(ShipmentManagerConfigHandler.FedExATOMInfo.Host, ShipmentManagerConfigHandler.FedExATOMInfo.Port);
						try {
							#region FedExRateRequestInfo initialization
							FedExRateRequestInfo fedExRateRequestInfo = new FedExRateRequestInfo();
							// Origin address
							fedExRateRequestInfo.Origin.PostalCode = origin.ZipCode;
							fedExRateRequestInfo.Origin.State = origin.State;
							fedExRateRequestInfo.Origin.CountryCode = origin.Country;
							// Destiation address
							fedExRateRequestInfo.Destination.PostalCode = destination.ZipCode;
							fedExRateRequestInfo.Destination.State = destination.State;
							fedExRateRequestInfo.Destination.CountryCode = destination.Country;
							
							// Package info
							if (!packageInfo.IsCarrierPackaging) {
								// In case of custom packaging
								fedExRateRequestInfo.PackageInfo.PackagingType = FedExPackagingType.CustomPackaging;
								fedExRateRequestInfo.PackageInfo.DimWidth  = (decimal) packageInfo.Width;
								fedExRateRequestInfo.PackageInfo.DimHeight = (decimal) packageInfo.Height;
								fedExRateRequestInfo.PackageInfo.DimLength = (decimal) packageInfo.Length;
								// Default units - inches
								fedExRateRequestInfo.PackageInfo.DimUnits = DimUnits.I;
							} else {
								// CarrierPackageTypeId must be one of the numbers of FedExPackagingType
								fedExRateRequestInfo.PackageInfo.PackagingType = (FedExPackagingType) int.Parse(packageInfo.CarrierPackageTypeId);
							}
							// number of packages
							fedExRateRequestInfo.PackageTotal = packageInfo.BoxesNumber;
							// package weight
							fedExRateRequestInfo.TotalPackageWeight = (decimal)(packageInfo.Weight * packageInfo.BoxesNumber);
							// default units - libras
							fedExRateRequestInfo.WeightUnits = WeightUnits.LBS;
							// planning shipping date
							fedExRateRequestInfo.ShipmentDateTime = orderShippingDate;
							// declared value 
							fedExRateRequestInfo.DeclaredValue = declaredValue;
							// service type
							fedExRateRequestInfo.ServiceType = (FedExShippingService)Enum.Parse(typeof(FedExShippingService), serviceType, true);
							// pickup type
							fedExRateRequestInfo.Flags.DropOffType = ShipmentManagerConfigHandler.FedExPickupType;
							#endregion
							
							FedExRateRequest fedExRateRequest = new FedExRateRequest(fedExAPI);
							FedExRateResponseInfo fedExResponse = fedExRateRequest.SendRequest(ShipmentManagerConfigHandler.FedExAccessInfo, FedExCarierCode.USD, fedExRateRequestInfo);
							result = fedExResponse.NetChargeAmount;
						} finally {
							fedExAPI.Dispose();
						}
						break;
					case PostalCarrier.UPS:
						#region UPS rate request initialization
						RatingServiceSelectionRequest upsRateRequest = new RatingServiceSelectionRequest(ShipmentManagerConfigHandler.UpsAccessInfo);
						// Origin address
						upsRateRequest.ShipFromAddress.PostalCode = origin.ZipCode;
						upsRateRequest.ShipFromAddress.StateProvinceCode = origin.State;
						upsRateRequest.ShipFromAddress.CountryCode = origin.Country;
						// Destination address
						upsRateRequest.ShipToAddress.PostalCode = destination.ZipCode;
						upsRateRequest.ShipToAddress.StateProvinceCode = destination.State;
						upsRateRequest.ShipToAddress.CountryCode = destination.Country;
						// Pickup type
						upsRateRequest.PickupType = ShipmentManagerConfigHandler.UpsPickupType;
						// Packages
						Package upsPackage = new Package();
						if (!packageInfo.IsCarrierPackaging) {
							// In case of custom packaging
							upsPackage.PackagingType = PackageTypeCodes._Unknown;
							upsPackage.Width = (float) packageInfo.Width;
							upsPackage.Height = (float) packageInfo.Height;
							upsPackage.Length = (float) packageInfo.Length;
							// Default units - inches
							upsPackage.UnitOfDimensionMeasurement = UnitOfMeasurement.IN;
						} else {
							// CarrierPackageTypeId must be one of the numbers of PackageTypeCodes
							upsPackage.PackagingType = (PackageTypeCodes)int.Parse(packageInfo.CarrierPackageTypeId);
						}
						// package weight
						upsPackage.Weight = (float) packageInfo.Weight;
						
						// default units - libras
						upsPackage.UnitOfWeightMeasurement = UnitOfMeasurement.LBS;

						for (int i = 1; i <= packageInfo.BoxesNumber; i++) {
							upsRateRequest.Packages.Add(upsPackage);
						}
						#endregion

						result = upsRateRequest.GetRates((UpsServiceCodes)Enum.Parse(typeof(UpsServiceCodes), serviceType, true));
						break;
				}
			} catch (Exception ex) {
				StringBuilder sb = new StringBuilder();
				sb.Append("Shipment manager exception: ").Append("\n")
					.Append(ex.Message).Append("\n")
					.Append("Postal carrier: ").Append(postalCarrier.ToString()).Append("\n")
					.Append("Service type: ").Append(serviceType).Append("\n")
					.Append("Origin: ").Append(origin.Country).Append(" ").Append(origin.ZipCode).Append(" ").Append(origin.State).Append("\n")
					.Append("Destination: ").Append(destination.Country).Append(" ").Append(destination.ZipCode).Append(" ").Append(destination.State).Append("\n")
					.Append("Package info: ").Append("Carrier package type id = ").Append(packageInfo.CarrierPackageTypeId).Append(" Width = ").Append(packageInfo.Width).Append(" Height = ").Append(packageInfo.Height).Append(" Length = ").Append(packageInfo.Length).Append(" Weight = ").Append(packageInfo.Weight).Append("\n");
				throw new ShipmentManagerException(sb.ToString());
			}
			return result;
		}

		public decimal RatePackage(PostalCarrier postalCarrier, string serviceType, AddressInfo origin, AddressInfo destination, PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue, ShippingServiceOptionsInfo info) {
			decimal result = 0.0m;
			try {
				switch(postalCarrier) {
					case PostalCarrier.FedEx: 
						FedExAPI fedExAPI = new FedExAPI(ShipmentManagerConfigHandler.FedExATOMInfo.Host, ShipmentManagerConfigHandler.FedExATOMInfo.Port);
						try {
							#region FedExRateRequestInfo initialization
							FedExRateRequestInfo fedExRateRequestInfo = new FedExRateRequestInfo();
							// Origin address
							fedExRateRequestInfo.Origin.PostalCode = origin.ZipCode;
							fedExRateRequestInfo.Origin.State = origin.State;
							fedExRateRequestInfo.Origin.CountryCode = origin.Country;
							// Destiation address
							fedExRateRequestInfo.Destination.PostalCode = destination.ZipCode;
							fedExRateRequestInfo.Destination.State = destination.State;
							fedExRateRequestInfo.Destination.CountryCode = destination.Country;
							
							// Package info
							if (!packageInfo.IsCarrierPackaging) {
								// In case of custom packaging
								fedExRateRequestInfo.PackageInfo.PackagingType = FedExPackagingType.CustomPackaging;
								fedExRateRequestInfo.PackageInfo.DimWidth  = (decimal) packageInfo.Width;
								fedExRateRequestInfo.PackageInfo.DimHeight = (decimal) packageInfo.Height;
								fedExRateRequestInfo.PackageInfo.DimLength = (decimal) packageInfo.Length;
								// Default units - inches
								fedExRateRequestInfo.PackageInfo.DimUnits = DimUnits.I;
							} else {
								// CarrierPackageTypeId must be one of the numbers of FedExPackagingType
								fedExRateRequestInfo.PackageInfo.PackagingType = (FedExPackagingType) int.Parse(packageInfo.CarrierPackageTypeId);
							}
							// number of packages
							fedExRateRequestInfo.PackageTotal = packageInfo.BoxesNumber;
							// package weight
							fedExRateRequestInfo.TotalPackageWeight = (decimal)(packageInfo.Weight * packageInfo.BoxesNumber);
							// default units - libras
							fedExRateRequestInfo.WeightUnits = WeightUnits.LBS;
							// planning shipping date
							fedExRateRequestInfo.ShipmentDateTime = orderShippingDate;
							// declared value 
							fedExRateRequestInfo.DeclaredValue = declaredValue;
							// service type
							fedExRateRequestInfo.ServiceType = (FedExShippingService)Enum.Parse(typeof(FedExShippingService), serviceType, true);
							// pickup type
							fedExRateRequestInfo.Flags.DropOffType = ShipmentManagerConfigHandler.FedExPickupType;
							
							// process some special service flags from info
							fedExRateRequestInfo.Flags.ResidentialDelivery = info.residentalDestination;
							fedExRateRequestInfo.Flags.SaturdayDelivery = info.saturdayDelivery;
							fedExRateRequestInfo.Flags.SaturdayPickup = info.saturdayPickup;
							#endregion
							
							FedExRateRequest fedExRateRequest = new FedExRateRequest(fedExAPI);
							FedExRateResponseInfo fedExResponse = fedExRateRequest.SendRequest(ShipmentManagerConfigHandler.FedExAccessInfo, FedExCarierCode.USD, fedExRateRequestInfo);
							result = fedExResponse.NetChargeAmount;
						} finally {
							fedExAPI.Dispose();
						}
						break;
					case PostalCarrier.UPS:
						#region UPS rate request initialization
						RatingServiceSelectionRequest upsRateRequest = new RatingServiceSelectionRequest(ShipmentManagerConfigHandler.UpsAccessInfo);

						// Saturday delivery/pickup
						upsRateRequest.SaturdayDelivery = info.saturdayDelivery;
						upsRateRequest.SaturdayPickup = info.saturdayPickup;

						// Origin address
						upsRateRequest.ShipFromAddress.PostalCode = origin.ZipCode;
						upsRateRequest.ShipFromAddress.StateProvinceCode = origin.State;
						upsRateRequest.ShipFromAddress.CountryCode = origin.Country;
						// Destination address
						upsRateRequest.ShipToAddress.PostalCode = destination.ZipCode;
						upsRateRequest.ShipToAddress.StateProvinceCode = destination.State;
						upsRateRequest.ShipToAddress.CountryCode = destination.Country;
						upsRateRequest.ShipToAddress.ResidentialAddress = info.residentalDestination; 
						// Pickup type
						upsRateRequest.PickupType = ShipmentManagerConfigHandler.UpsPickupType;
						// Packages
						Package upsPackage = new Package();
						if (!packageInfo.IsCarrierPackaging) {
							// In case of custom packaging
							upsPackage.PackagingType = PackageTypeCodes._Unknown;
							upsPackage.Width = (float) packageInfo.Width;
							upsPackage.Height = (float) packageInfo.Height;
							upsPackage.Length = (float) packageInfo.Length;
							// Default units - inches
							upsPackage.UnitOfDimensionMeasurement = UnitOfMeasurement.IN;
						} else {
							// CarrierPackageTypeId must be one of the numbers of PackageTypeCodes
							upsPackage.PackagingType = (PackageTypeCodes)int.Parse(packageInfo.CarrierPackageTypeId);
						}
						// package weight
						upsPackage.Weight = (float) packageInfo.Weight;
						
						// default units - libras
						upsPackage.UnitOfWeightMeasurement = UnitOfMeasurement.LBS;

						// Insured value
						upsPackage.InsuredValue = (float)declaredValue;
						// Additional handling 
						upsPackage.AdditionalHandling = info.additionalHandling;

						for (int i = 1; i <= packageInfo.BoxesNumber; i++) {
							upsRateRequest.Packages.Add(upsPackage);
						}
						#endregion

						result = upsRateRequest.GetRates((UpsServiceCodes)Enum.Parse(typeof(UpsServiceCodes), serviceType, true));
						break;
				}
			} catch (Exception ex) {
				StringBuilder sb = new StringBuilder();
				sb.Append("Shipment manager exception: ").Append("\n")
					.Append(ex.Message).Append("\n")
					.Append("Postal carrier: ").Append(postalCarrier.ToString()).Append("\n")
					.Append("Service type: ").Append(serviceType).Append("\n")
					.Append("Origin: ").Append(origin.Country).Append(" ").Append(origin.ZipCode).Append(" ").Append(origin.State).Append("\n")
					.Append("Destination: ").Append(destination.Country).Append(" ").Append(destination.ZipCode).Append(" ").Append(destination.State).Append("\n")
					.Append("Package info: ").Append("Carrier package type id = ").Append(packageInfo.CarrierPackageTypeId).Append(" Width = ").Append(packageInfo.Width).Append(" Height = ").Append(packageInfo.Height).Append(" Length = ").Append(packageInfo.Length).Append(" Weight = ").Append(packageInfo.Weight).Append("\n");
				throw new ShipmentManagerException(sb.ToString());
			}
			return result;
		}

		/// <summary>
		/// Returns the package tracking info
		/// </summary>
		/// <param name="postalCarrier">Postal carrier code UPS or FedEx</param>
		/// <param name="shipmentDateTime">Shipment datetime</param>
		/// <param name="trackingNumber">The tracking number assigned to shipment</param>
		/// <returns>The string with state of shipment or throws ShipmentManagerException if fails</returns>
		public String TrackShipment(PostalCarrier postalCarrier, DateTime shipmentDate, string trackingNumber) {
			string result = String.Empty;
			try {
				switch(postalCarrier) {
					case PostalCarrier.FedEx: 
						FedExAPI fedExAPI = new FedExAPI(ShipmentManagerConfigHandler.FedExATOMInfo.Host, ShipmentManagerConfigHandler.FedExATOMInfo.Port);
						try {
							FedExTrackRequest fedExTrackRequest = new FedExTrackRequest(fedExAPI);
							FedExTrackResponseInfo fedExResponse = fedExTrackRequest.SendRequest(ShipmentManagerConfigHandler.FedExAccessInfo, trackingNumber, shipmentDate);
							result = fedExResponse.ToString();
						} finally {
							fedExAPI.Dispose();
						}
						break;
					case PostalCarrier.UPS:
						TrackRequest trackRequest = new TrackRequest(ShipmentManagerConfigHandler.UpsAccessInfo);
						TrackingInfo t_info = trackRequest.TrackPackage(trackingNumber);
						result = t_info.ToXmlString();
						break;
						/*
						TrackRequest trackRequest = new TrackRequest(ShipmentManagerConfigHandler.UpsAccessInfo);
						PackageShipment[] upsResponse = trackRequest.TrackShipment(trackingNumber);
						StringBuilder output = new StringBuilder();
						for (int i=0; i<upsResponse.Length; i++) {
							output.Append(upsResponse[i].ToString());
							output.Append("\n");
						}
						result = output.ToString();
						break;
						*/
						// this was the code for tracking individual packages from shipment
						// uncomment, if u whenever will use such kind of tracking
						/*
						TrackRequest trackRequest = new TrackRequest(ShipmentManagerConfigHandler.UpsAccessInfo);
						PackageShipment upsResponse = trackRequest.TrackPackage(trackingNumber);
						result = upsResponse.ToString();
						break;
						*/
				}
			} catch (Exception ex) {
				StringBuilder sb = new StringBuilder();
				sb.Append("Shipment manager exception: ").Append("\n")
					.Append(ex.Message).Append("\n")
					.Append("Postal carrier: ").Append(postalCarrier.ToString()).Append("\n")
					.Append("Tracking number: ").Append(trackingNumber).Append("\n");
				throw new ShipmentManagerException(sb.ToString());
			}
			return result;
		}

		/// <summary>
		/// The function returns the estimated cost of delivery the package(s) to destination address
		/// </summary>
		/// <param name="postalCarrier">The postal carrier to use - UPX or FedEx</param>
		/// <param name="serviceType">The string code of postal carrier's service type</param>
		/// <param name="origin">The origin address.</param>
		/// <param name="destination">The destination address.</param>
		/// <param name="packageInfo">The information about packages in shipment</param>
		/// <param name="orderShippingDate">The date when the shipment have to be delivered (FedEx only)</param>
		/// <param name="declaredValue">The declared value for the package (FedEx only).
		/// FIXME : change shipManager.RatePackage the way it accepts decimal amount.
		/// </param>
		/// <returns>The rate of package delivery. For FedEx it may differ from real rate.</returns>
		/// <exception cref="ShipmentManagerException">If function or service fails</exception>
		public ShippingResponseInfo ShipPackage(PostalCarrier postalCarrier, string serviceType, ContactInfo origin, ContactInfo destination, PackagingInfo packageInfo, DateTime orderShippingDate, decimal declaredValue) {
			ShippingResponseInfo result = new ShippingResponseInfo();
			try {
				switch(postalCarrier) {
					case PostalCarrier.FedEx: 
						FedExAPI fedExAPI = new FedExAPI(ShipmentManagerConfigHandler.FedExATOMInfo.Host, ShipmentManagerConfigHandler.FedExATOMInfo.Port);
						try {
							#region FedExShipRequestInfo initialization
							FedExShipRequestInfo fedExShipRequestInfo = new FedExShipRequestInfo();
							// Origin address
							fedExShipRequestInfo.Origin.CompanyName = origin.CompanyName;
							fedExShipRequestInfo.Origin.ContactName = origin.FirstName + " " + origin.LastName;						
							fedExShipRequestInfo.Origin.PhoneNumber = origin.ContactPhone;						

							fedExShipRequestInfo.Origin.AddressLine1 = origin.Address.Address1;
							fedExShipRequestInfo.Origin.AddressLine2 = origin.Address.Address2;
							fedExShipRequestInfo.Origin.City = origin.Address.City;
							fedExShipRequestInfo.Origin.State = origin.Address.State;
							fedExShipRequestInfo.Origin.PostalCode = origin.Address.ZipCode;
							fedExShipRequestInfo.Origin.CountryCode = origin.Address.Country;
							
							// Destination address
							fedExShipRequestInfo.Destination.CompanyName = destination.CompanyName;
							fedExShipRequestInfo.Destination.ContactName = destination.FullName;						
							fedExShipRequestInfo.Destination.PhoneNumber = destination.ContactPhone;						

							fedExShipRequestInfo.Destination.AddressLine1 = destination.Address.Address1;
							fedExShipRequestInfo.Destination.AddressLine2 = destination.Address.Address2;
							fedExShipRequestInfo.Destination.City = destination.Address.City;
							fedExShipRequestInfo.Destination.State = destination.Address.State;
							fedExShipRequestInfo.Destination.PostalCode = destination.Address.ZipCode;
							fedExShipRequestInfo.Destination.CountryCode = destination.Address.Country;
							
							// Label type
							switch (ShipmentManagerConfigHandler.MediaType) {
								case ShippingServiceMediaType.PlainPaperPNG:
									fedExShipRequestInfo.LabelInfo.MediaType = FedExLabelMediaType.PlainPaperPNG;
									break;
								case ShippingServiceMediaType.ThermalWithDocTab:
									fedExShipRequestInfo.LabelInfo.MediaType = FedExLabelMediaType.ThermalWithDocTab;
									break;
								case ShippingServiceMediaType.ThermalWithoutDocTab:
									fedExShipRequestInfo.LabelInfo.MediaType = FedExLabelMediaType.ThermalWithoutDocTab;
									break;

							}

							switch (ShipmentManagerConfigHandler.PrinterType) {
								case ShippingServicePrinterType.PlainPaper:
									fedExShipRequestInfo.LabelInfo.PrinterType = FedExLabelPrinterType.PlainPaper;
									break;
								case ShippingServicePrinterType.EltronOrion:
									fedExShipRequestInfo.LabelInfo.PrinterType = FedExLabelPrinterType.EltronOrion;
									break;
								case ShippingServicePrinterType.EltronEclipse:
									fedExShipRequestInfo.LabelInfo.PrinterType = FedExLabelPrinterType.EltronEclipse;
									break;
							}

							// Package info
							if (!packageInfo.IsCarrierPackaging) {
								// In case of custom packaging
								fedExShipRequestInfo.PackageInfo.PackagingType = FedExPackagingType.CustomPackaging;
								fedExShipRequestInfo.PackageInfo.DimWidth  = (decimal) packageInfo.Width;
								fedExShipRequestInfo.PackageInfo.DimHeight = (decimal) packageInfo.Height;
								fedExShipRequestInfo.PackageInfo.DimLength = (decimal) packageInfo.Length;
								// Default units - inches
								fedExShipRequestInfo.PackageInfo.DimUnits = DimUnits.I;
							} else {
								// CarrierPackageTypeId must be one of the numbers of FedExPackagingType
								fedExShipRequestInfo.PackageInfo.PackagingType = (FedExPackagingType) int.Parse(packageInfo.CarrierPackageTypeId);
							}
							// number of packages
							fedExShipRequestInfo.PackageTotal = packageInfo.BoxesNumber;
							// package weight
							fedExShipRequestInfo.TotalPackageWeight = (decimal)(packageInfo.Weight * packageInfo.BoxesNumber);
							// default units - libras
							fedExShipRequestInfo.WeightUnits = WeightUnits.LBS;
							// planning shipping date
							fedExShipRequestInfo.ShipmentDateTime = orderShippingDate;
							// declared value 
							fedExShipRequestInfo.DeclaredValue = declaredValue;
							// service type
							fedExShipRequestInfo.ServiceType = (FedExShippingService)Enum.Parse(typeof(FedExShippingService), serviceType, true);
							// pickup type
							fedExShipRequestInfo.Flags.DropOffType = ShipmentManagerConfigHandler.FedExPickupType;

							#endregion
							
							FedExShipRequest fedExShipRequest = new FedExShipRequest(fedExAPI);
							FedExShipResponseInfo fedExResponse = fedExShipRequest.SendRequest(ShipmentManagerConfigHandler.FedExAccessInfo, FedExCarierCode.FDXE, fedExShipRequestInfo);

							result.ChargeAmount = fedExResponse.NetChargeAmount;
							result.TrackingNumber = fedExResponse.TrackingNumber;
							result.LabelDataBuffer = fedExResponse.LabelDataBuffer;
						} finally {
							fedExAPI.Dispose();
						}
						break;
					case PostalCarrier.UPS:
						throw new ShipmentManagerException("Isn't implemented for the UPS service.");
						break;
				}
			} catch (Exception ex) {
				StringBuilder sb = new StringBuilder();
				sb.Append("Shipment manager exception: ").Append("\n")
					.Append(ex.Message).Append("\n")
					.Append("Postal carrier: ").Append(postalCarrier.ToString()).Append("\n")
					.Append("Service type: ").Append(serviceType).Append("\n")
					.Append("Origin: ").Append(origin.Address.Country).Append(" ").Append(origin.Address.ZipCode).Append(" ").Append(origin.Address.State).Append("\n")
					.Append("Destination: ").Append(destination.Address.Country).Append(" ").Append(destination.Address.ZipCode).Append(" ").Append(destination.Address.State).Append("\n")
					.Append("Package info: ").Append("Carrier package type id = ").Append(packageInfo.CarrierPackageTypeId).Append(" Width = ").Append(packageInfo.Width).Append(" Height = ").Append(packageInfo.Height).Append(" Length = ").Append(packageInfo.Length).Append(" Weight = ").Append(packageInfo.Weight).Append("\n");
				throw new ShipmentManagerException(sb.ToString());
			}
			return result;
		}
	}
}
