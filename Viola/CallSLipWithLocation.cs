using System;
using System.Runtime.Serialization;
using Viola;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

public class CallSLipWithLocation
{
    public int CallSlipId { get; set; }
    public string RequestNumber { get; set; }
    public string PatronName { get; set; }
    public string ItemBarcode { get; set; }
    public string CallNumber { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string ItemEnum { get; set; }
    public string ItemChronology { get; set; }
    public string ItemYear { get; set; }
    public string CSNote { get; set; }
    public string SeriesStatement { get; set; }
    public string PublicationYear { get; set; }
    public CallSlipDeliveryTypes DeliveryType { get; set; }
    public String Status { get; set; }
    public String AreaName { get; set; }
    public string PatronEmail { get; set; }
    public string PatronBarcode { get; set; }
    public string SourceId { get; set; }
    public string ItemId { get; set; }
    public string Location { get; set; }
    public string ISBN { get; set; }
    public string Pages { get; set; }
    public string StatusDescription { get; set; }
    public string DateRequested { get; set; }
    public string MFHDNote { get; set; }
    public string SequentialDesignation { get; set; }
    public string PickupLocation { get; set; }
    public string ResearcherAddress { get; set; }
    public int CallNumberId { get; set; }
    public string BranchSigel { get; set; }
    public int LibrisId { get; set; }
    public String UserId { get; set; }
    public DateTime Date { get; set; }
    public string PrintType { get; set; }
    public string Location_Id { get; set; }
    public int AreaId { get; set; }
    public String AreaDefaultSortOrder { get; set; }
    public String CallNumberDefaultSortOrder { get; set; }
}