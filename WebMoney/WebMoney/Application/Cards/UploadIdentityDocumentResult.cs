namespace WebMoney.Services;

public class UploadIdentityDocumentResult
{
    public bool Success => Errors.Count == 0;
    public List<string> Errors { get; set; } = [];
}
