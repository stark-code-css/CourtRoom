namespace CourtRoom.Dtos.CourtOrderDtos;

public record UpdateCourtOrderDto(
    string CourtNo, 
    string CaseNo, 
    string CauseListSlNo, 
    DateTime Date, 
    string CourtDirection, 
    double CostImposed,
    string ImposedOn, 
    string CashWhereToBeDeposited);