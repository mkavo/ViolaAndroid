using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace ViolaLibrary
{
    public class CallSlip
    {
        /// <summary>
        /// Call slipens id. Id:t i databasen
        /// </summary>
        
        public int CallSlipId { get; set; }

        /// <summary>
        /// ID:t för den källa som beställningen hämtas ifrån. 
        /// </summary>
        
        public string SourceId { get; set; }

        /// <summary>
        /// Beställningens ID i det system som den hämtas ifrån.
        /// </summary>
       
        public string RequestNumber { get; set; }

        /// <summary>
        /// Beställarens name
        /// </summary>
       
        public string PatronName { get; set; }

        /// <summary>
        /// Beställarens Email
        /// </summary>
        
        public string PatronEmail { get; set; }

        /// <summary>
        /// Barcode
        /// </summary>
       
        public string PatronBarcode { get; set; }

        /// <summary>
        /// Barcode
        /// </summary>
       
        public string ItemBarcode { get; set; }

        /// <summary>
        ///  Lokal placering? Ex. "Mag".
        /// </summary>
      
        public string Location { get; set; }

        /// <summary>
        /// Hyllsignum
        /// </summary>
        
        public string CallNumber { get; set; }

        /// <summary>
        /// Author
        /// </summary>
        
        public string Author { get; set; }

        /// <summary>
        /// Title
        /// </summary>
       
        public string Title { get; set; }

        /// <summary>
        /// ISBN number
        /// </summary>
       
        public string ISBN { get; set; }

        /// <summary>
        /// Book enumeration
        /// </summary>
       
        public string ItemEnum { get; set; }

        /// <summary>
        /// Book chron
        /// </summary>
       
        public string ItemChronology { get; set; }

        /// <summary>
        /// Book year
        /// </summary>
       
        public string ItemYear { get; set; }

        /// <summary>
        /// CS Note
        /// </summary>
       
        public string CSNote { get; set; }

        /// <summary>
        /// Status description
        /// </summary>
       
        public string StatusDescription { get; set; }

        /// <summary>
        /// Date requested
        /// </summary>
       
        public string DateRequested { get; set; }

        /// <summary>
        /// MFHD Note
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MFHD")]
        [DisplayName("MFHDNote")]
        public string MFHDNote { get; set; }

        /// <summary>
        /// F440 A -Serietitel - biuppslagsform
        /// </summary>
        [DisplayName("Serietitel")]
        public string F440_A { get; set; }

        /// <summary>
        /// F440 V -Serietitel - biuppslagsform
        /// </summary>
        [DisplayName("Serietitel")]
        public string F440_V { get; set; }

        /// <summary>
        /// F 260 C -Utgivning, distribution etc.
        /// </summary>
        [DisplayName("Utgivning, distribution")]
        public string F260_C { get; set; }

        /// <summary>
        /// Direct delivery
        /// </summary>
        [DisplayName("Leveranssätt")]
        public string DeliveryType { get; set; }

        /// <summary>
        /// Pick up location. Biblioteket där boken hämtas (eller beställande biblioteks sigel om det är ett fjärrlån från Libris)
        /// </summary>
        [DisplayName("Hämtställe")]
        public string PickupLocation { get; set; }

        /// <summary>
        /// Researcher address
        /// </summary>
        [DisplayName("Forskarens adress")]
        public string ResearcherAddress { get; set; }

        /// <summary>
        /// Id för det callNumber i databasen som denna callslip matchats mot
        /// </summary>
        [DisplayName("CallNumberId")]
        public int CallNumberId { get; set; }

        /// <summary>
        /// Koden(Sigel) för den filial som fått fjärrlånebeställningen. Används inte på magasinbeställningar
        /// </summary>
        [DisplayName("Mottagande filials sigel")]
        public string BranchSigel { get; set; }

        /// <summary>
        /// LibrisId för posten
        /// </summary>
        [DisplayName("LibrisId (libris bib-id)")]
        public int LibrisId { get; set; }

        /// <summary>
        /// Från tabellen CallSlipStatuses
        /// </summary>
        [ResultColumn]
        [DisplayName("Status")]
        public String Status { get; set; }

        /// <summary>
        /// Från tabellen CallSlipStatuses
        /// </summary>
        [ResultColumn]
        [DisplayName("Handläggare")]
        public String UserId { get; set; }

        /// <summary>
        /// Från tabellen CallSlipStatuses
        /// </summary>
        [ResultColumn]
        [DisplayName("Datum för senaste statusändring")]
        public DateTime Date { get; set; }



    }
    public class CallSLipWithLocation : CallSlip
    {
        [DisplayName("Plockområde")]
        public String AreaName { get; set; }

        [DisplayName("AreaId")]
        public int AreaId { get; set; }

        [DisplayName("AreaDefaultSortOrder")]
        public String AreaDefaultSortOrder { get; set; }

        [DisplayName("CallNumberDefaultSortOrder")]
        public String CallNumberDefaultSortOrder { get; set; }
    }
}
