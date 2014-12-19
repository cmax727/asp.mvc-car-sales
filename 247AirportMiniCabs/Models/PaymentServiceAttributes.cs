namespace AlinTuriCab.Models
{
    public class PaymentServiceAttributes
    {

        public string customerEmail { get; set; }
        public string customerMobile { get; set; }

        public string ddExpiryDateMonth { get; set; }
        public string ddExpiryDateYear { get; set; }
        public string ddStartDateMonth { get; set; }
        public string ddStartDateYear { get; set; }

        public string tbCardName { get; set; }
        public string tbCardNumber { get; set; }
        public string tbIssueNumber { get; set; }
        public string tbCV2 { get; set; }
        public string ddCountries { get; set; }

        public string tbAddress1 { get; set; }
        public string tbAddress2 { get; set; }
        public string tbAddress3 { get; set; }
        public string tbAddress4 { get; set; }
        public string tbCity { get; set; }

        public string tbState { get; set; }
        public string tbPostCode { get; set; }
        public string hfAmount { get; set; }
        public string hfCurrencyISOCode { get; set; }
        public string hfOrderID { get; set; }
        public string hfOrderDescription { get; set; }
        public string UserAgent { get; set; }
        public string UserHostIPAddress { get; set; }

    }
}