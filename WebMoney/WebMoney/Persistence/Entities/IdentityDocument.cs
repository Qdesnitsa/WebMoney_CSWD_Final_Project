namespace WebMoney.Persistence.Entities;

public class IdentityDocument
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string DocumentIDNumber { get; set; }
    public byte[] DocumentDataPhoto { get; set; }
    public DateOnly ExpirationDate { get; set; }
}