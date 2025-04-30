namespace CourtRoom.Dtos.AuthDtos;

public record ChangePasswordDto(int Id, string Email, string OldPassword, string NewPassword);