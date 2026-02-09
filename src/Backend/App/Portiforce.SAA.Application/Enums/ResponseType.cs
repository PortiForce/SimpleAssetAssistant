namespace Portiforce.SAA.Application.Enums;

public enum ResponseType : byte
{
	Info = 0,
	Message = 1,
	SuccessInfo = 2,
	WarningMessage = 3,
	ValidationError = 4,
	ApplicationError = 5
}
