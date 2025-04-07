namespace CourtRoom.Dtos.PaymentDetailDtos;

public record UpdatePaymentDetailDto(
    double CostDeposited,
    string PaymentMode,
    string PaymentRefNo,
    DateTime DateOfDeposit,
    string ReceiptNo,
    DateTime DateOfCatBarLibFund,
    string Remarks
);