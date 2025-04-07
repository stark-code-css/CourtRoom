using System.ComponentModel.DataAnnotations;

namespace CourtRoom.Models;

public class CourtOrder
{
    public int Id { get; set; }

    public string CourtNo { get; set; } = string.Empty;

    public string CaseNo { get; set; } = string.Empty;

    public string CauseListSlNo { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string CourtDirection { get; set; } = string.Empty;

    public double CostImposed { get; set; }

    public string ImposedOn { get; set; } = string.Empty;

    public string CashWhereToBeDeposited { get; set; } = string.Empty;
    public PaymentDetail? PaymentDetail { get; set; } = null!;
}