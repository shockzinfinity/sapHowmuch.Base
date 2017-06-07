#pragma warning disable 1591

using System.ComponentModel;

namespace sapHowmuch.Base.Constants
{
	public static class SboTable
	{
		[Description("1")]
		public const string ChartOfAccounts = "OACT";

		[Description("2")]
		public const string BusinessPartner = "OCRD";

		[Description("3")]
		public const string Banks = "ODSC";

		[Description("4")]
		public const string Item = "OITM";

		[Description("5")]
		public const string VatGroup = "OVTG";

		[Description("6")]
		public const string PriceList = "OPLN";

		[Description("7")]
		public const string SpecialPrices = "OSPP";

		[Description("8")]
		public const string ItemProperties = "OITG";

		[Description("10")]
		public const string BusinessPartnerGroups = "OCRG";

		[Description("12")]
		public const string Users = "OUSR";

		[Description("13")]
		public const string Invoice = "OINV";

		[Description("14")]
		public const string CreditNotes = "ORIN";

		[Description("15")]
		public const string DeliveryNote = "ODLN";

		[Description("16")]
		public const string Returns = "ORDN";

		[Description("17")]
		public const string Order = "ORDR";

		[Description("18")]
		public const string PurchaseInvoices = "OPCH";

		[Description("19")]
		public const string PurchaseCreditNotes = "ORPC";

		[Description("20")]
		public const string PurchaseDeliveryNotes = "OPDN";

		[Description("21")]
		public const string PurchaseReturns = "ORPD";

		[Description("22")]
		public const string PurchaseOrders = "OPOR";

		[Description("23")]
		public const string Quotations = "OQUT";

		[Description("40")]
		public const string PaymentTerm = "OCTG";

		[Description("49")]
		public const string ShippingType = "OSHP";

		[Description("52")]
		public const string ItemGroup = "OITB";

		[Description("59")]
		public const string InventoryGenEntry = "OIGN";

		[Description("60")]
		public const string InventoryGenExit = "OIGE";

		[Description("112")]
		public const string Drafts = "ODRF";

		[Description("128")]
		public const string SalesTaxCodes = "OSTC";

		[Description("163")]
		public const string CorrectionPurchaseInvoice = "OCPI";

		[Description("164")]
		public const string CorrectionPurchaseInvoiceReversal = "OCPV";

		[Description("165")]
		public const string CorrectionInvoice = "OCSI";

		[Description("166")]
		public const string CorrectionInvoiceReversal = "OCSV";

		[Description("203")]
		public const string DownPayments = "ODPI";

		[Description("204")]
		public const string PurchaseDownPayments = "ODPO";

		[Description("540000006")]
		public const string PurchaseQuotations = "OPQT";

		[Description("1470000113")]
		public const string PurchaseRequest = "OPRQ";

		[Description("")]
		public const string DeliveryNotePackage = "DLN7"; // Document Packages 중 일부
	}
}

// TODO: 테이블 검색 및 추가 필요
//oIncomingPayments = 24,
//oJournalVouchers = 28,
//oJournalEntries = 30,
//oStockTakings = 31,
//oContacts = 33,
//oCreditCards = 36,
//oCurrencyCodes = 37,
//oBankPages = 42,
//oManufacturers = 43,
//oVendorPayments = 46,
//oLandedCostsCodes = 48,
//oLengthMeasures = 50,
//oWeightMeasures = 51,
//oSalesPersons = 53,
//oCustomsGroups = 56,
//oChecksforPayment = 57,
//oWarehouses = 64,
//oCommissionGroups = 65,
//oProductTrees = 66,
//oStockTransfer = 67,
//oWorkOrders = 68,
//oCreditPaymentMethods = 70,
//oCreditCardPayments = 71,
//oAlternateCatNum = 73,
//oBudget = 77,
//oBudgetDistribution = 78,
//oMessages = 81,
//oBudgetScenarios = 91,
//oUserDefaultGroups = 93,
//oSalesOpportunities = 97,
//oSalesStages = 101,
//oActivityTypes = 103,
//oActivityLocations = 104,
//oDeductionTaxHierarchies = 116,
//oDeductionTaxGroups = 117,
//oAdditionalExpenses = 125,
//oSalesTaxAuthorities = 126,
//oSalesTaxAuthoritiesTypes = 127,
//oQueryCategories = 134,
//oFactoringIndicators = 138,
//oPaymentsDrafts = 140,
//oAccountSegmentations = 142,
//oAccountSegmentationCategories = 143,
//oWarehouseLocations = 144,
//oForms1099 = 145,
//oInventoryCycles = 146,
//oWizardPaymentMethods = 147,
//oBPPriorities = 150,
//oDunningLetters = 151,
//oUserFields = 152,
//oUserTables = 153,
//oPickLists = 156,
//oPaymentRunExport = 158,
//oUserQueries = 160,
//oMaterialRevaluation = 162,
//oContractTemplates = 170,
//oEmployeesInfo = 171,
//oCustomerEquipmentCards = 176,
//oWithholdingTaxCodes = 178,
//oBillOfExchangeTransactions = 182,
//oKnowledgeBaseSolutions = 189,
//oServiceContracts = 190,
//oServiceCalls = 191,
//oUserKeys = 193,
//oQueue = 194,
//oSalesForecast = 198,
//oTerritories = 200,
//oIndustries = 201,
//oProductionOrders = 202,
//oPackagesTypes = 205,
//oUserObjectsMD = 206,
//oTeams = 211,
//oRelationships = 212,
//oUserPermissionTree = 214,
//oActivityStatus = 217,
//oChooseFromList = 218,
//oFormattedSearches = 219,
//oAttachments2 = 221,
//oUserLanguages = 223,
//oMultiLanguageTranslations = 224,
//oDynamicSystemStrings = 229,
//oHouseBankAccounts = 231,
//oBusinessPlaces = 247,
//oLocalEra = 250,
//oNotaFiscalCFOP = 258,
//oNotaFiscalCST = 259,
//oNotaFiscalUsage = 260,
//oClosingDateProcedure = 261,
//oBPFiscalRegistryID = 278,
//oSalesTaxInvoice = 280,
//oPurchaseTaxInvoice = 281,
//BoRecordset = 300,
//BoRecordsetEx = 301,
//BoBridge = 305,
//oStockTransferDraft = 1179,
//oInventoryTransferRequest = 1250000001,