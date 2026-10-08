namespace RTGS.Models.ISO20022;

[System.Xml.Serialization.XmlRoot("DataPDU", Namespace = "urn:cma:stp:xsd:stp.1.0", IsNullable = false)]
public class DataPdu
{
    [System.Xml.Serialization.XmlElement(Namespace = "urn:cma:stp:xsd:stp.1.0")]
    public required Body Body { get; set; }
}

public class Body
{
    [System.Xml.Serialization.XmlElement(Namespace = "urn:iso:std:iso:20022:tech:xsd:head.001.001.01")]
    public required AppHeader.AppHeader AppHdr { get; set; }

    [System.Xml.Serialization.XmlElement("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.08")]
    public CustomerCreditTransfer.CustomerCreditTransfer? CustomerCreditTransfer { get; set; }

    [System.Xml.Serialization.XmlElement("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.009.001.08")]
    public InstitutionCreditTransfer.InstitutionCreditTransfer? InstitutionCreditTransfer { get; set; }

    [System.Xml.Serialization.XmlElement("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.054.001.08")]
    public DebitCreditNotification.DebitCreditNotification? DebitCreditNotification { get; set; }
    
    [System.Xml.Serialization.XmlElement("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.025.001.05")]
    public Receipt.Receipt? Receipt { get; set; }
    
    [System.Xml.Serialization.XmlElement("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.053.001.08")]
    public Statement.Statement? Statement { get; set; }
    
    [System.Xml.Serialization.XmlElement("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.052.001.08")]
    public Account.Account? Account { get; set; }
}