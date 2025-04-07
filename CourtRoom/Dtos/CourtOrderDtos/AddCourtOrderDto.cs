namespace CourtRoom.Dtos.CourtOrderDtos;

public record AddCourtOrderDto(
    string CourtNo, 
    string CaseNo, 
    string CauseListSlNo, 
    DateTime Date, 
    string CourtDirection, 
    double CostImposed,
    string ImposedOn, 
    string CashWhereToBeDeposited);