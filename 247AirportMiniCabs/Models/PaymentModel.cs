using System;
using System.Text;
using ThePaymentGateway.Common;
using ThePaymentGateway.PaymentSystem;

namespace AlinTuriCab.Models
{
    public partial class PaymentModel
    {
        public enum FORM_MODE
        {
            PAYMENT_FORM,
            THREE_D_SECURE,
            RESULTS
        }

        private FORM_MODE m_fmFormMode = FORM_MODE.PAYMENT_FORM;
        protected internal string m_szPaREQ;
        protected internal string m_szTermURL;
        protected internal string m_szACSURL;
        protected internal string m_szCrossReference;


        public FORM_MODE Form_Modes { get; set; }
        public bool pnMessagePanelVisible { get; set; }
        public string lbMessageLableText { get; set; }
        public bool pnTransactionResultPanelVisible { get; set; }
        public bool pnTransactionVisible { get; set; }
        //public bool pnMessagePanelVisible { get; set; }
        public string lbGatewayResponseText { get; set; }
        public bool pnDuplicateTransactionVisible { get; set; }
        public string lbPreviewTransactionMessageText { get; set; }

        public string pnMessagePanelCssClass { get; set; }
        public string pnTransactionResultsPanelCssClass { get; set; }
        public string message { get; set; }

        public PaymentModel ProcessDirectCardPayment(PaymentServiceAttributes attributes)
        {
            PaymentModel _model = new PaymentModel();


            string szMessage = null;
            bool boTransactionSuccessful = false;
            StringBuilder sbString;
            int nCount = 0;
            int nTemp;
            CardDetailsTransaction cdtCardDetailsTransaction;
            RequestGatewayEntryPointList lrgepRequestGatewayEntryPoints;
            GatewayOutput goGatewayOutput;
            TransactionOutputMessage tomTransactionOutputMessage;
            TransactionControl tcTransactionControl;
            CardDetails cdCardDetails;
            CreditCardDate ccdExpiryDate = null;
            CreditCardDate ccdStartDate = null;
            CustomerDetails cdCustomerDetails = null;
            NullableInt nCountryCode = null;
            NullableInt nExpiryDateMonth = null;
            NullableInt nExpiryDateYear = null;
            NullableInt nStartDateMonth = null;
            NullableInt nStartDateYear = null;
            string szPreviousTransactionMessage = null;
            bool boDuplicateTransaction = false;

            lrgepRequestGatewayEntryPoints = new RequestGatewayEntryPointList();
            // you need to put the correct gateway entry point urls in here
            // contact support to get the correct urls

            // The actual values to use for the entry points can be established in a number of ways
            // 1) By periodically issuing a call to GetGatewayEntryPoints
            // 2) By storing the values for the entry points returned with each transaction
            // 3) Speculatively firing transactions at https://gw1.xxx followed by gw2, gw3, gw4....
            // The lower the metric (2nd parameter) means that entry point will be attempted first,
            // EXCEPT if it is -1 - in this case that entry point will be skipped
            // NOTE: You do NOT have to add the entry points in any particular order - the list is sorted
            // by metric value before the transaction sumbitting process begins
            // The 3rd parameter is a retry attempt, so it is possible to try that entry point that number of times
            // before failing over onto the next entry point in the list
            lrgepRequestGatewayEntryPoints.Add(new RequestGatewayEntryPoint("https://gw1." + Global.PaymentProcessorFullDomain, 100, 2));
            lrgepRequestGatewayEntryPoints.Add(new RequestGatewayEntryPoint("https://gw2." + Global.PaymentProcessorFullDomain, 200, 2));
            lrgepRequestGatewayEntryPoints.Add(new RequestGatewayEntryPoint("https://gw3." + Global.PaymentProcessorFullDomain, 300, 2));

            tcTransactionControl = new TransactionControl(new NullableBool(true),
                new NullableBool(true),
                new NullableBool(true),
                new NullableBool(true),
                new NullableInt(60),
                null,
                null,
                null,
                null,
                null,
                null);

            if (!String.IsNullOrEmpty(attributes.ddExpiryDateMonth))
            {
                nExpiryDateMonth = new NullableInt(System.Convert.ToInt32(attributes.ddExpiryDateMonth));
            }
            if (!String.IsNullOrEmpty(attributes.ddExpiryDateYear))
            {
                nExpiryDateYear = new NullableInt(System.Convert.ToInt32(attributes.ddExpiryDateYear));
            }
            ccdExpiryDate = new CreditCardDate(nExpiryDateMonth, nExpiryDateYear);

            if (!String.IsNullOrEmpty(attributes.ddStartDateMonth))
            {
                nStartDateMonth = new NullableInt(System.Convert.ToInt32(attributes.ddStartDateMonth));
            }
            if (!String.IsNullOrEmpty(attributes.ddStartDateYear))
            {
                nStartDateYear = new NullableInt(System.Convert.ToInt32(attributes.ddStartDateYear));
            }
            ccdStartDate = new CreditCardDate(nStartDateMonth, nStartDateYear);

            cdCardDetails = new CardDetails(attributes.tbCardName, attributes.tbCardNumber, ccdExpiryDate, ccdStartDate, attributes.tbIssueNumber, attributes.tbCV2);

            if (!String.IsNullOrEmpty(attributes.ddCountries))
            {
                nTemp = System.Convert.ToInt32(attributes.ddCountries);
                if (nTemp != -1)
                {
                    nCountryCode = new NullableInt(nTemp);
                }
            }
            cdCustomerDetails = new CustomerDetails(new AddressDetails(attributes.tbAddress1, attributes.tbAddress2, attributes.tbAddress3, attributes.tbAddress4, attributes.tbCity, attributes.tbState, attributes.tbPostCode, nCountryCode),
                attributes.customerEmail, attributes.customerMobile, attributes.UserHostIPAddress);

            cdtCardDetailsTransaction = new CardDetailsTransaction(lrgepRequestGatewayEntryPoints,
                new MerchantDetails(Global.MerchantID, Global.Password),
                new TransactionDetails(TRANSACTION_TYPE.SALE, new NullableInt(System.Convert.ToInt32(attributes.hfAmount)), new NullableInt(System.Convert.ToInt32(attributes.hfCurrencyISOCode)), attributes.hfOrderID, attributes.hfOrderDescription, tcTransactionControl, new ThreeDSecureBrowserDetails(new NullableInt(0), "*/*", attributes.UserAgent)),
                cdCardDetails,
                cdCustomerDetails,
                null);

            // send the SOAP request
            if (!cdtCardDetailsTransaction.ProcessTransaction(out goGatewayOutput, out tomTransactionOutputMessage))
            {
                szMessage = "Couldn't communicate with payment gateway";
                boTransactionSuccessful = false;
            }
            else
            {
                switch (goGatewayOutput.StatusCode)
                {
                    case 0:
                        // status code of 0 - means transaction successful 
                        boTransactionSuccessful = true;
                        m_fmFormMode = FORM_MODE.RESULTS;
                        szMessage = "transaction successful: " + goGatewayOutput.Message;
                        break;
                    case 3:
                        // status code of 3 - means 3D Secure authentication required 
                        m_fmFormMode = FORM_MODE.THREE_D_SECURE;
                        m_szTermURL = Global.SiteSecureBaseURL + "ThreeDSecureLandingPage.aspx";
                        m_szPaREQ = tomTransactionOutputMessage.ThreeDSecureOutputData.PaREQ;
                        m_szACSURL = tomTransactionOutputMessage.ThreeDSecureOutputData.ACSURL;
                        m_szCrossReference = tomTransactionOutputMessage.CrossReference;
                        break;
                    case 5:
                        // status code of 5 - means transaction declined 
                        boTransactionSuccessful = false;
                        m_fmFormMode = FORM_MODE.RESULTS;
                        szMessage = "transaction declined: " + goGatewayOutput.Message;
                        break;
                    case 20:
                        // status code of 20 - means duplicate transaction 
                        m_fmFormMode = FORM_MODE.RESULTS;
                        szMessage = "duplicate transaction: " + goGatewayOutput.Message;
                        if (goGatewayOutput.PreviousTransactionResult.StatusCode.Value == 0)
                        {
                            boTransactionSuccessful = true;
                        }
                        else
                        {
                            boTransactionSuccessful = false;
                        }
                        szPreviousTransactionMessage = goGatewayOutput.PreviousTransactionResult.Message;
                        boDuplicateTransaction = true;
                        break;
                    case 30:
                        // status code of 30 - means an error occurred 
                        boTransactionSuccessful = false;
                        m_fmFormMode = FORM_MODE.PAYMENT_FORM;

                        sbString = new StringBuilder();

                        // get any additional messages
                        if (goGatewayOutput.ErrorMessages.Count > 0)
                        {
                            sbString.Append("<br /><ul>");

                            for (nCount = 0; nCount < goGatewayOutput.ErrorMessages.Count; nCount++)
                            {
                                sbString.AppendFormat("<li>{0}</li>", goGatewayOutput.ErrorMessages[nCount]);
                            }
                            sbString.Append("</ul>");
                        }

                        szMessage = goGatewayOutput.Message + sbString.ToString();
                        break;
                    default:
                        // unhandled status code  
                        boTransactionSuccessful = false;
                        m_fmFormMode = FORM_MODE.PAYMENT_FORM;
                        szMessage = goGatewayOutput.Message;
                        break;
                }
            }

            if (m_fmFormMode == FORM_MODE.PAYMENT_FORM)
            {
                Form_Modes = FORM_MODE.PAYMENT_FORM;
                pnMessagePanelCssClass = "ErrorMessage";
                //pnMessagePanel.Visible = true;
                //lbMessageLabel.Text = szMessage;
                //pnTransactionResultsPanel.Visible = false;

                pnMessagePanelVisible = true;
                lbMessageLableText = szMessage;
                pnTransactionResultPanelVisible = false;

            }
            else
            {
                //pnTransactionResultsPanel.Visible = true;
                //pnMessagePanel.Visible = false;

                pnTransactionResultPanelVisible = true;
                pnMessagePanelVisible = false;

                if (!boTransactionSuccessful)
                {
                    pnTransactionResultsPanelCssClass = "ErrorMessage";

                }
                else
                {
                    pnTransactionResultsPanelCssClass = "SuccessMessage";
                }

                //lbGatewayResponse.Text = szMessage;
                lbGatewayResponseText = szMessage;

                // sort out the duplicate transaction reporting
                if (boDuplicateTransaction)
                {
                    //pnDuplicateTransactionPanel.Visible = true;
                    //lbPreviousTransactionMessage.Text = szPreviousTransactionMessage;
                    pnDuplicateTransactionVisible = true;
                    lbPreviewTransactionMessageText = szPreviousTransactionMessage;
                }

                // the process another link
                //hlProcessAnother.NavigateUrl = Global.SiteSecureBaseURL + "PaymentForm.aspx";

            }
            _model.message = szMessage;
            return _model;
        }

        /* private void PerformThreeDSecureAuthentication(string szPaRES, string szCrossReference)
         {
            string szMessage = null;
             bool boTransactionSuccessful = false;
             StringBuilder sbString;
             int nCount = 0;
             ThreeDSecureAuthentication tdsaThreeDSecureAuthentication;
             RequestGatewayEntryPointList lrgepRequestGatewayEntryPoints;
             GatewayOutput goGatewayOutput;
             TransactionOutputMessage tomTransactionOutputMessage;
             string szPreviousTransactionMessage = null;
             bool boDuplicateTransaction = false;

             lrgepRequestGatewayEntryPoints = new RequestGatewayEntryPointList();
             // you need to put the correct gateway entry point urls in here
             // contact support to get the correct urls

             // The actual values to use for the entry points can be established in a number of ways
             // 1) By periodically issuing a call to GetGatewayEntryPoints
             // 2) By storing the values for the entry points returned with each transaction
             // 3) Speculatively firing transactions at https://gw1.xxx followed by gw2, gw3, gw4....
             // The lower the metric (2nd parameter) means that entry point will be attempted first,
             // EXCEPT if it is -1 - in this case that entry point will be skipped
             // NOTE: You do NOT have to add the entry points in any particular order - the list is sorted
             // by metric value before the transaction sumbitting process begins
             // The 3rd parameter is a retry attempt, so it is possible to try that entry point that number of times
             // before failing over onto the next entry point in the list
             lrgepRequestGatewayEntryPoints.Add(new RequestGatewayEntryPoint("https://gw1." + Global.PaymentProcessorFullDomain, 100, 2));
             lrgepRequestGatewayEntryPoints.Add(new RequestGatewayEntryPoint("https://gw2." + Global.PaymentProcessorFullDomain, 200, 2));
             lrgepRequestGatewayEntryPoints.Add(new RequestGatewayEntryPoint("https://gw3." + Global.PaymentProcessorFullDomain, 300, 2));

             tdsaThreeDSecureAuthentication = new ThreeDSecureAuthentication(
                 lrgepRequestGatewayEntryPoints,
                 new MerchantDetails(Global.MerchantID, Global.Password),
                 new ThreeDSecureInputData(szCrossReference, szPaRES),
                 null);

             // send the SOAP request
             if (!tdsaThreeDSecureAuthentication.ProcessTransaction(out goGatewayOutput, out tomTransactionOutputMessage))
             {
                 szMessage = "Couldn't communicate with payment gateway";
                 boTransactionSuccessful = true;
             }
             else
             {
                 switch (goGatewayOutput.StatusCode)
                 {
                     case 0:
                         // status code of 0 - means transaction successful 
                         boTransactionSuccessful = true;
                         m_fmFormMode = FORM_MODE.RESULTS;
                         szMessage = goGatewayOutput.Message;
                         break;
                     case 5:
                         // status code of 5 - means transaction declined 
                         boTransactionSuccessful = false;
                         m_fmFormMode = FORM_MODE.RESULTS;
                         szMessage = goGatewayOutput.Message;
                         break;
                     case 20:
                         // status code of 20 - means duplicate transaction 
                         m_fmFormMode = FORM_MODE.RESULTS;
                         szMessage = goGatewayOutput.Message;
                         if (goGatewayOutput.PreviousTransactionResult.StatusCode.Value == 0)
                         {
                             boTransactionSuccessful = true;
                         }
                         else
                         {
                             boTransactionSuccessful = false;
                         }
                         szPreviousTransactionMessage = goGatewayOutput.PreviousTransactionResult.Message;
                         boDuplicateTransaction = true;
                         break;
                     case 30:
                         // status code of 30 - means an error occurred 
                         boTransactionSuccessful = false;
                         m_fmFormMode = FORM_MODE.RESULTS;

                         sbString = new StringBuilder();

                         // get any additional messages
                         if (goGatewayOutput.ErrorMessages.Count > 0)
                         {
                             sbString.Append("<br /><ul>");

                             for (nCount = 0; nCount < goGatewayOutput.ErrorMessages.Count; nCount++)
                             {
                                 sbString.AppendFormat("<li>{0}</li>", goGatewayOutput.ErrorMessages[nCount]);
                             }
                             sbString.Append("</ul>");
                         }

                         szMessage = goGatewayOutput.Message + sbString.ToString();
                         break;
                     default:
                         // unhandled status code  
                         boTransactionSuccessful = false;
                         m_fmFormMode = FORM_MODE.RESULTS;
                         szMessage = goGatewayOutput.Message;
                         break;
                 }
             }

             //pnTransactionResultsPanel.Visible = true;
             //pnMessagePanel.Visible = false;

             if (!boTransactionSuccessful)
             {
                 //pnTransactionResultsPanel.CssClass = "ErrorMessage";
             }
             else
             {
                 //pnTransactionResultsPanel.CssClass = "SuccessMessage";
             }

             //lbGatewayResponse.Text = szMessage;

             // sort out the duplicate transaction reporting
             if (boDuplicateTransaction)
             {
                 //pnDuplicateTransactionPanel.Visible = true;
                 //lbPreviousTransactionMessage.Text = szPreviousTransactionMessage;
             }

             // the process another link
             //hlProcessAnother.NavigateUrl = Global.SiteSecureBaseURL + "PaymentForm.aspx";
         }*/
    }
}