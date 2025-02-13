namespace UMS.Contracts.InvoicingApp.Poco;

public class Transaction
{
    public long Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public PaymentChannel PaymentChannel { get; set; } = new();
}