using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Global
/// </summary>
namespace AlinTuriCab
{
    public class Global : System.Web.HttpApplication
    {
        private static ThePaymentGateway.Common.ISOCountryList m_iclISOCountryData;
        private static string m_szSiteSecureBaseURL;
        private static string m_szPaymentProcessorFullDomain;
        private static string m_szMerchantID;
        private static string m_szPassword;

        public Global()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public void Application_Start()
        {
            string szPaymentProcessorDomain;
            int nPaymentProcessorPort;

            // Code that runs on application startup

            // get the MerchantID and Password from the config file
            m_szMerchantID = ConfigurationManager.AppSettings["MerchantID"];
            m_szPassword = ConfigurationManager.AppSettings["Password"];

            // get the SiteSecureBaseURL from the config file
            m_szSiteSecureBaseURL = ConfigurationManager.AppSettings["SiteSecureBaseURL"];

            // work out the PaymentProcessorFullDomain
            szPaymentProcessorDomain = ConfigurationManager.AppSettings["PaymentProcessorDomain"];
            nPaymentProcessorPort = System.Convert.ToInt32(ConfigurationManager.AppSettings["PaymentProcessorPort"]);

            if (nPaymentProcessorPort == 443)
            {
	            m_szPaymentProcessorFullDomain = szPaymentProcessorDomain + "/";
            }
            else
            {
                m_szPaymentProcessorFullDomain = szPaymentProcessorDomain + ":" + nPaymentProcessorPort.ToString() + "/";
            }

            // the ISO country data
            m_iclISOCountryData = new ThePaymentGateway.Common.ISOCountryList();
			m_iclISOCountryData.Add(826,"United Kingdom","GBR",3);
			m_iclISOCountryData.Add(840,"United States","USA",2);
			m_iclISOCountryData.Add(36,"Australia","AUS",1);
			m_iclISOCountryData.Add(124,"Canada","CAN",1);
			m_iclISOCountryData.Add(250,"France","FRA",1);
			m_iclISOCountryData.Add(276,"Germany","DEU",1);
			m_iclISOCountryData.Add(4,"Afghanistan","AFG",0);
			m_iclISOCountryData.Add(248,"Åland Islands","ALA",0);	
			m_iclISOCountryData.Add(8,"Albania","ALB",0);
			m_iclISOCountryData.Add(12,"Algeria","DZA",0);
			m_iclISOCountryData.Add(16,"American Samoa","ASM",0);
			m_iclISOCountryData.Add(20,"Andorra","AND",0);
			m_iclISOCountryData.Add(24,"Angola","AGO",0);
			m_iclISOCountryData.Add(660,"Anguilla","AIA",0);
			m_iclISOCountryData.Add(10,"Antarctica","ATA",0);
			m_iclISOCountryData.Add(28,"Antigua and Barbuda","ATG",0);
			m_iclISOCountryData.Add(32,"Argentina","ARG",0);
			m_iclISOCountryData.Add(51,"Armenia","ARM",0);
			m_iclISOCountryData.Add(533,"Aruba","ABW",0);
			m_iclISOCountryData.Add(40,"Austria","AUT",0);
			m_iclISOCountryData.Add(31,"Azerbaijan","AZE",0);
			m_iclISOCountryData.Add(44,"Bahamas","BHS",0);
			m_iclISOCountryData.Add(48,"Bahrain","BHR",0);
			m_iclISOCountryData.Add(50,"Bangladesh","BGD",0);
			m_iclISOCountryData.Add(52,"Barbados","BRB",0);
			m_iclISOCountryData.Add(112,"Belarus","BLR",0);
			m_iclISOCountryData.Add(56,"Belgium","BEL",0);
			m_iclISOCountryData.Add(84,"Belize","BLZ",0);
			m_iclISOCountryData.Add(204,"Benin","BEN",0);
			m_iclISOCountryData.Add(60,"Bermuda","BMU",0);
			m_iclISOCountryData.Add(64,"Bhutan","BTN",0);
			m_iclISOCountryData.Add(68,"Bolivia","BOL",0);
			m_iclISOCountryData.Add(70,"Bosnia and Herzegovina","BIH",0);
			m_iclISOCountryData.Add(72,"Botswana","BWA",0);
			m_iclISOCountryData.Add(74,"Bouvet Island","BVT",0);
			m_iclISOCountryData.Add(76,"Brazil Federative","BRA",0);
			m_iclISOCountryData.Add(86,"British Indian Ocean Territory","IOT",0);
			m_iclISOCountryData.Add(96,"Brunei","BRN",0);
			m_iclISOCountryData.Add(100,"Bulgaria","BGR",0);
			m_iclISOCountryData.Add(854,"Burkina Faso","BFA",0);
			m_iclISOCountryData.Add(108,"Burundi","BDI",0);
			m_iclISOCountryData.Add(116,"Cambodia","KHM",0);
			m_iclISOCountryData.Add(120,"Cameroon","CMR",0);
			m_iclISOCountryData.Add(132,"Cape Verde","CPV",0);
			m_iclISOCountryData.Add(136,"Cayman Islands","CYM",0);
			m_iclISOCountryData.Add(140,"Central African Republic","CAF",0);
			m_iclISOCountryData.Add(148,"Chad","TCD",0);
			m_iclISOCountryData.Add(152,"Chile","CHL",0);
			m_iclISOCountryData.Add(156,"China","CHN",0);
			m_iclISOCountryData.Add(162,"Christmas Island","CXR",0);
			m_iclISOCountryData.Add(166,"Cocos (Keeling) Islands","CCK",0);
			m_iclISOCountryData.Add(170,"Colombia","COL",0);
			m_iclISOCountryData.Add(174,"Comoros","COM",0);
			m_iclISOCountryData.Add(180,"Congo","COD",0);
			m_iclISOCountryData.Add(178,"Congo","COG",0);
			m_iclISOCountryData.Add(184,"Cook Islands","COK",0);
			m_iclISOCountryData.Add(188,"Costa Rica","CRI",0);
			m_iclISOCountryData.Add(384,"Côte d'Ivoire","CIV",0);
			m_iclISOCountryData.Add(191,"Croatia","HRV",0);
			m_iclISOCountryData.Add(192,"Cuba","CUB",0);
			m_iclISOCountryData.Add(196,"Cyprus","CYP",0);
			m_iclISOCountryData.Add(203,"Czech Republic","CZE",0);
			m_iclISOCountryData.Add(208,"Denmark","DNK",0);
			m_iclISOCountryData.Add(262,"Djibouti","DJI",0);
			m_iclISOCountryData.Add(212,"Dominica","DMA",0);
			m_iclISOCountryData.Add(214,"Dominican Republic","DOM",0);
			m_iclISOCountryData.Add(626,"East Timor","TMP",0);
			m_iclISOCountryData.Add(218,"Ecuador","ECU",0);
			m_iclISOCountryData.Add(818,"Egypt","EGY",0);
			m_iclISOCountryData.Add(222,"El Salvador","SLV",0);
			m_iclISOCountryData.Add(226,"Equatorial Guinea","GNQ",0);
			m_iclISOCountryData.Add(232,"Eritrea","ERI",0);
			m_iclISOCountryData.Add(233,"Estonia","EST",0);
			m_iclISOCountryData.Add(231,"Ethiopia","ETH",0);
			m_iclISOCountryData.Add(238,"Falkland Islands (Malvinas)","FLK",0);
			m_iclISOCountryData.Add(234,"Faroe Islands","FRO",0);
			m_iclISOCountryData.Add(242,"Fiji","FJI",0);
			m_iclISOCountryData.Add(246,"Finland","FIN",0);
			m_iclISOCountryData.Add(254,"French Guiana","GUF",0);
			m_iclISOCountryData.Add(258,"French Polynesia","PYF",0);
			m_iclISOCountryData.Add(260,"French Southern Territories","ATF",0);
			m_iclISOCountryData.Add(266,"Gabon","GAB",0);
			m_iclISOCountryData.Add(270,"Gambia","GMB",0);
			m_iclISOCountryData.Add(268,"Georgia","GEO",0);
			m_iclISOCountryData.Add(288,"Ghana","GHA",0);
			m_iclISOCountryData.Add(292,"Gibraltar","GIB",0);
			m_iclISOCountryData.Add(300,"Greece","GRC",0);
			m_iclISOCountryData.Add(304,"Greenland","GRL",0);
			m_iclISOCountryData.Add(308,"Grenada","GRD",0);
			m_iclISOCountryData.Add(312,"Guadaloupe","GLP",0);
			m_iclISOCountryData.Add(316,"Guam","GUM",0);
			m_iclISOCountryData.Add(320,"Guatemala","GTM",0);
			m_iclISOCountryData.Add(831,"Guernsey","GGY",0);
			m_iclISOCountryData.Add(324,"Guinea","GIN",0);
			m_iclISOCountryData.Add(624,"Guinea-Bissau","GNB",0);
			m_iclISOCountryData.Add(328,"Guyana","GUY",0);
			m_iclISOCountryData.Add(332,"Haiti","HTI",0);
			m_iclISOCountryData.Add(334,"Heard Island and McDonald Islands","HMD",0);
			m_iclISOCountryData.Add(340,"Honduras","HND",0);
			m_iclISOCountryData.Add(344,"Hong Kong","HKG",0);
			m_iclISOCountryData.Add(348,"Hungary","HUN",0);
			m_iclISOCountryData.Add(352,"Iceland","ISL",0);
			m_iclISOCountryData.Add(356,"India","IND",0);
			m_iclISOCountryData.Add(360,"Indonesia","IDN",0);
			m_iclISOCountryData.Add(364,"Iran","IRN",0);
			m_iclISOCountryData.Add(368,"Iraq","IRQ",0);
			m_iclISOCountryData.Add(372,"Ireland","IRL",0);
			m_iclISOCountryData.Add(833,"Isle of Man","IMN",0);
			m_iclISOCountryData.Add(376,"Israel","ISR",0);
			m_iclISOCountryData.Add(380,"Italy","ITA",0);
			m_iclISOCountryData.Add(388,"Jamaica","JAM",0);
			m_iclISOCountryData.Add(392,"Japan","JPN",0);
			m_iclISOCountryData.Add(832,"Jersey","JEY",0);
			m_iclISOCountryData.Add(400,"Jordan","JOR",0);
			m_iclISOCountryData.Add(398,"Kazakhstan","KAZ",0);
			m_iclISOCountryData.Add(404,"Kenya","KEN",0);
			m_iclISOCountryData.Add(296,"Kiribati","KIR",0);
			m_iclISOCountryData.Add(410,"Korea","KOR",0);
			m_iclISOCountryData.Add(408,"Korea","PRK",0);
			m_iclISOCountryData.Add(414,"Kuwait","KWT",0);
			m_iclISOCountryData.Add(417,"Kyrgyzstan","KGZ",0);
			m_iclISOCountryData.Add(418,"Lao","LAO",0);
			m_iclISOCountryData.Add(428,"Latvia","LVA",0);
			m_iclISOCountryData.Add(422,"Lebanon","LBN",0);
			m_iclISOCountryData.Add(426,"Lesotho","LSO",0);
			m_iclISOCountryData.Add(430,"Liberia","LBR",0);
			m_iclISOCountryData.Add(434,"Libyan Arab Jamahiriya","LBY",0);
			m_iclISOCountryData.Add(438,"Liechtenstein","LIE",0);
			m_iclISOCountryData.Add(440,"Lithuania","LTU",0);
			m_iclISOCountryData.Add(442,"Luxembourg","LUX",0);
			m_iclISOCountryData.Add(446,"Macau","MAC",0);
			m_iclISOCountryData.Add(807,"Macedonia","MKD",0);
			m_iclISOCountryData.Add(450,"Madagascar","MDG",0);
			m_iclISOCountryData.Add(454,"Malawi","MWI",0);
			m_iclISOCountryData.Add(458,"Malaysia","MYS",0);
			m_iclISOCountryData.Add(462,"Maldives","MDV",0);
			m_iclISOCountryData.Add(466,"Mali","MLI",0);
			m_iclISOCountryData.Add(470,"Malta","MLT",0);
			m_iclISOCountryData.Add(584,"Marshall Islands","MHL",0);
			m_iclISOCountryData.Add(474,"Martinique","MTQ",0);
			m_iclISOCountryData.Add(478,"Mauritania Islamic","MRT",0);
			m_iclISOCountryData.Add(480,"Mauritius","MUS",0);
			m_iclISOCountryData.Add(175,"Mayotte","MYT",0);
			m_iclISOCountryData.Add(484,"Mexico","MEX",0);
			m_iclISOCountryData.Add(583,"Micronesia","FSM",0);
			m_iclISOCountryData.Add(498,"Moldova","MDA",0);
			m_iclISOCountryData.Add(492,"Monaco","MCO",0);
			m_iclISOCountryData.Add(496,"Mongolia","MNG",0);
			m_iclISOCountryData.Add(499,"Montenegro","MNE",0);	
			m_iclISOCountryData.Add(500,"Montserrat","MSR",0);
			m_iclISOCountryData.Add(504,"Morocco","MAR",0);
			m_iclISOCountryData.Add(508,"Mozambique","MOZ",0);
			m_iclISOCountryData.Add(104,"Myanmar","MMR",0);
			m_iclISOCountryData.Add(516,"Namibia","NAM",0);
			m_iclISOCountryData.Add(520,"Nauru","NRU",0);
			m_iclISOCountryData.Add(524,"Nepal","NPL",0);
			m_iclISOCountryData.Add(528,"Netherlands","NLD",0);
			m_iclISOCountryData.Add(530,"Netherlands Antilles","ANT",0);
			m_iclISOCountryData.Add(540,"New Caledonia","NCL",0);
			m_iclISOCountryData.Add(554,"New Zealand","NZL",0);
			m_iclISOCountryData.Add(558,"Nicaragua","NIC",0);
			m_iclISOCountryData.Add(562,"Niger","NER",0);
			m_iclISOCountryData.Add(566,"Nigeria","NGA",0);
			m_iclISOCountryData.Add(570,"Niue","NIU",0);
			m_iclISOCountryData.Add(574,"Norfolk Island","NFK",0);
			m_iclISOCountryData.Add(580,"Northern Mariana Islands","MNP",0);
			m_iclISOCountryData.Add(578,"Norway","NOR",0);
			m_iclISOCountryData.Add(512,"Oman","OMN",0);
			m_iclISOCountryData.Add(586,"Pakistan","PAK",0);
			m_iclISOCountryData.Add(585,"Palau","PLW",0);
			m_iclISOCountryData.Add(275,"Palestine","PSE",0);	
			m_iclISOCountryData.Add(591,"Panama","PAN",0);
			m_iclISOCountryData.Add(598,"Papua New Guinea","PNG",0);
			m_iclISOCountryData.Add(600,"Paraguay","PRY",0);
			m_iclISOCountryData.Add(604,"Peru","PER",0);
			m_iclISOCountryData.Add(608,"Philippines","PHL",0);
			m_iclISOCountryData.Add(612,"Pitcairn","PCN",0);
			m_iclISOCountryData.Add(616,"Poland","POL",0);
			m_iclISOCountryData.Add(620,"Portugal","PRT",0);
			m_iclISOCountryData.Add(630,"Puerto Rico","PRI",0);
			m_iclISOCountryData.Add(634,"Qatar","QAT",0);
			m_iclISOCountryData.Add(638,"Réunion","REU",0);
			m_iclISOCountryData.Add(642,"Romania","ROM",0);
			m_iclISOCountryData.Add(643,"Russian Federation","RUS",0);
			m_iclISOCountryData.Add(646,"Rwanda","RWA",0);
			m_iclISOCountryData.Add(652,"Saint Barthélemy","BLM",0);
			m_iclISOCountryData.Add(654,"Saint Helena","SHN",0);
			m_iclISOCountryData.Add(659,"Saint Kitts and Nevis","KNA",0);
			m_iclISOCountryData.Add(662,"Saint Lucia","LCA",0);
			m_iclISOCountryData.Add(663,"Saint Martin (French part)","MAF",0);
			m_iclISOCountryData.Add(666,"Saint Pierre and Miquelon","SPM",0);
			m_iclISOCountryData.Add(670,"Saint Vincent and the Grenadines","VCT",0);
			m_iclISOCountryData.Add(882,"Samoa","WSM",0);
			m_iclISOCountryData.Add(674,"San Marino","SMR",0);
			m_iclISOCountryData.Add(678,"São Tomé and Príncipe Democratic","STP",0);
			m_iclISOCountryData.Add(682,"Saudi Arabia","SAU",0);
			m_iclISOCountryData.Add(686,"Senegal","SEN",0);
			m_iclISOCountryData.Add(688,"Serbia","SRB",0);
			m_iclISOCountryData.Add(690,"Seychelles","SYC",0);
			m_iclISOCountryData.Add(694,"Sierra Leone","SLE",0);
			m_iclISOCountryData.Add(702,"Singapore","SGP",0);
			m_iclISOCountryData.Add(703,"Slovakia","SVK",0);
			m_iclISOCountryData.Add(705,"Slovenia","SVN",0);
			m_iclISOCountryData.Add(90,"Solomon Islands","SLB",0);
			m_iclISOCountryData.Add(706,"Somalia","SOM",0);
			m_iclISOCountryData.Add(710,"South Africa","ZAF",0);
			m_iclISOCountryData.Add(239,"South Georgia and the South Sandwich Islands","SGS",0);
			m_iclISOCountryData.Add(724,"Spain","ESP",0);
			m_iclISOCountryData.Add(144,"Sri Lanka","LKA",0);
			m_iclISOCountryData.Add(736,"Sudan","SDN",0);
			m_iclISOCountryData.Add(740,"Suriname","SUR",0);
			m_iclISOCountryData.Add(744,"Svalbard and Jan Mayen","SJM",0);
			m_iclISOCountryData.Add(748,"Swaziland","SWZ",0);
			m_iclISOCountryData.Add(752,"Sweden","SWE",0);
			m_iclISOCountryData.Add(756,"Switzerland","CHE",0);
			m_iclISOCountryData.Add(760,"Syrian Arab Republic","SYR",0);
			m_iclISOCountryData.Add(158,"Taiwan,","TWN",0);
			m_iclISOCountryData.Add(762,"Tajikistan","TJK",0);
			m_iclISOCountryData.Add(834,"Tanzania","TZA",0);
			m_iclISOCountryData.Add(764,"Thailand","THA",0);
			m_iclISOCountryData.Add(768,"Togo","TGO",0);
			m_iclISOCountryData.Add(772,"Tokelau","TKL",0);
			m_iclISOCountryData.Add(776,"Tonga","TON",0);
			m_iclISOCountryData.Add(780,"Trinidad and Tobago","TTO",0);
			m_iclISOCountryData.Add(788,"Tunisia","TUN",0);
			m_iclISOCountryData.Add(792,"Turkey","TUR",0);
			m_iclISOCountryData.Add(795,"Turkmenistan","TKM",0);
			m_iclISOCountryData.Add(796,"Turks and Caicos Islands","TCA",0);
			m_iclISOCountryData.Add(798,"Tuvalu","TUV",0);
			m_iclISOCountryData.Add(800,"Uganda","UGA",0);
			m_iclISOCountryData.Add(804,"Ukraine","UKR",0);
			m_iclISOCountryData.Add(784,"United Arab Emirates","ARE",0);
			m_iclISOCountryData.Add(581,"United States Minor Outlying Islands","UMI",0);
			m_iclISOCountryData.Add(858,"Uruguay Eastern","URY",0);
			m_iclISOCountryData.Add(860,"Uzbekistan","UZB",0);
			m_iclISOCountryData.Add(548,"Vanuatu","VUT",0);
			m_iclISOCountryData.Add(336,"Vatican City State","VAT",0);
			m_iclISOCountryData.Add(862,"Venezuela","VEN",0);
			m_iclISOCountryData.Add(704,"Vietnam","VNM",0);
			m_iclISOCountryData.Add(92,"Virgin Islands, British","VGB",0);
			m_iclISOCountryData.Add(850,"Virgin Islands, U.S.","VIR",0);
			m_iclISOCountryData.Add(876,"Wallis and Futuna","WLF",0);
			m_iclISOCountryData.Add(732,"Western Sahara","ESH",0);
			m_iclISOCountryData.Add(887,"Yemen","YEM",0);
			m_iclISOCountryData.Add(894,"Zambia","ZMB",0);
			m_iclISOCountryData.Add(716,"Zimbabwe","ZWE",0);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        public static ThePaymentGateway.Common.ISOCountryList ISOCountryData { get { return(m_iclISOCountryData);} }
        public static string SiteSecureBaseURL { get { return (m_szSiteSecureBaseURL); } }
        public static string PaymentProcessorFullDomain { get { return (m_szPaymentProcessorFullDomain); } }
        public static string MerchantID { get { return (m_szMerchantID); } }
        public static string Password { get { return (m_szPassword); } }
    }
}
