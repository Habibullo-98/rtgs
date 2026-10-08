using System.ComponentModel.DataAnnotations;

namespace RTGS.Models.Controller;

public class TransferRequest
{
    [Required(ErrorMessage = "Id is required")]
    [MinLength(1, ErrorMessage = "Id cannot be empty")]
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "NbtBic is required")]
    [MinLength(1, ErrorMessage = "NbtBic cannot be empty")]
    public string NbtBic { get; set; }

    [Required(ErrorMessage = "DbtrBic is required")]
    [MinLength(1, ErrorMessage = "DbtrBic cannot be empty")]
    public string DbtrBic { get; set; }
    
    [Required(ErrorMessage = "DbtrInn is required")]
    [MinLength(1, ErrorMessage = "DbtrInn cannot be empty")]
    public string DbtrInn { get; set; }
    
    [Required(ErrorMessage = "DbtrOrgName is required")]
    [MinLength(1, ErrorMessage = "DbtrOrgName cannot be empty")]
    public string DbtrOrgName { get; set; }
    
    [Required(ErrorMessage = "DbtrAgtAcct is required")]
    [MinLength(1, ErrorMessage = "DbtrAgtAcct cannot be empty")]
    public string DbtrAgtAcct { set; get; }
    
    [Required(ErrorMessage = "DbtrAcct is required")]
    [MinLength(1, ErrorMessage = "DbtrAcct cannot be empty")]
    public string DbtrAcct { set; get; }
    
    [Required(ErrorMessage = "CdtrBic is required")]
    [MinLength(1, ErrorMessage = "CdtrBic cannot be empty")]
    public string CdtrBic { get; set; }
    
    [Required(ErrorMessage = "CdtrInn is required")]
    [MinLength(1, ErrorMessage = "CdtrInn cannot be empty")]
    public string CdtrInn { get; set; }
    
    [Required(ErrorMessage = "CdtrOrgName is required")]
    [MinLength(1, ErrorMessage = "CdtrOrgName cannot be empty")]
    public string CdtrOrgName { get; set; }
    
    [Required(ErrorMessage = "CdtrAgtAcct is required")]
    [MinLength(1, ErrorMessage = "CdtrAgtAcct cannot be empty")]
    public string CdtrAgtAcct { set; get; }
    
    [Required(ErrorMessage = "CdtrAcct is required")]
    [MinLength(1, ErrorMessage = "CdtrAcct cannot be empty")]
    public string CdtrAcct { set; get; }
    
    [Required(ErrorMessage = "Comment is required")]
    [MinLength(1, ErrorMessage = "Comment cannot be empty")]
    public string Comment { get; set; }
    
    [Required(ErrorMessage = "SerialNumber is required")]
    [MinLength(1, ErrorMessage = "SerialNumber cannot be empty")]
    public string SerialNumber { get; set; }
    
    [Required(ErrorMessage = "PartnerId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "PartnerId must be greater than 0")]
    public int PartnerId { get; set; }
    
    [Required(ErrorMessage = "Key is required")]
    [MinLength(1, ErrorMessage = "Key cannot be empty")]
    public required string Key { get; set; }
}