namespace CourtRoom.Dtos.PaymentDetailDtos;

public record PaymentDetailDtoWithCourtOrder(
    int Id,
    double CostDeposited,
    string PaymentMode,
    string PaymentRefNo,
    DateTime DateOfDeposit,
    string ReceiptNo,
    DateTime DateOfCatBarLibFund,
    string Remarks,
    CourtOrderDto CourtOrder
);

public record CourtOrderDto(
    int Id,
    string CourtNo, 
    string CaseNo, 
    string CauseListSlNo, 
    DateTime Date, 
    string CourtDirection, 
    double CostImposed,
    string ImposedOn, 
    string CashWhereToBeDeposited);