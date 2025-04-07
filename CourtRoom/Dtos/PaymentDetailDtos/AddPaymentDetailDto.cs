namespace CourtRoom.Dtos.PaymentDetailDtos;

public record AddPaymentDetailDto(
        double CostDeposited,
        string PaymentMode,
        string PaymentRefNo,
        DateTime DateOfDeposit,
        string ReceiptNo,
        DateTime DateOfCatBarLibFund,
        string Remarks,
        int CourtOrderId
    );