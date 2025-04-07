using System.ComponentModel.DataAnnotations;

namespace CourtRoom.Models;

public class PaymentDetail
{
    public int Id { get; set; }
    public double CostDeposited { get; set; }
    public string PaymentMode { get; set; } = string.Empty;
    public string PaymentRefNo { get; set; } = string.Empty;
    public DateTime DateOfDeposit { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime DateOfCatBarLibFund { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public int CourtOrderId { get; set; }
    public CourtOrder CourtOrder { get; set; } = null!;
}