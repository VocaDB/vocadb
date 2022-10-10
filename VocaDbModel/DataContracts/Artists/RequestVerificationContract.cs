namespace VocaDb.Model.DataContracts.Artists;

public sealed record RequestVerificationContract
{
	public string Message { get; init; }
	public string LinkToProof { get; init; }
	public bool PrivateMessage { get; init; }

	public RequestVerificationContract()
	{
		Message = string.Empty;
		LinkToProof = string.Empty;
	}
}
