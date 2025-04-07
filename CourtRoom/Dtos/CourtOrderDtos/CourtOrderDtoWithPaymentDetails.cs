namespace CourtRoom.Dtos.CourtOrderDtos;

public record CourtOrderDtoWithPaymentDetails(
    int Id,
    string CourtNo,
    string CaseNo,
    string CauseListSlNo,
    DateTime Date,
    string CourtDirection,
    double CostImposed,
    string ImposedOn,
    string CashWhereToBeDeposited,
    PaymentDetailDto? PaymentDetail
);

public record PaymentDetailDto(
    int Id,
    double CostDeposited,
    string PaymentMode,
    string PaymentRefNo,
    DateTime DateOfDeposit,
    string ReceiptNo,
    DateTime DateOfCatBarLibFund,
    string Remarks
);