namespace WebMoney.ModelTransfer;

public class PrepareNewDepositResult
{
    public bool Success => Errors.Count == 0;
    public List<(string Field, string Message)> Errors { get; } = new();
    public string CardNumber { get; set; } = string.Empty;  
}