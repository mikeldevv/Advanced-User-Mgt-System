namespace UMS.Contracts.InvoicingApp.Poco;

public class Invoice
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Transaction Transaction { get; set; } = new();
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public List<InvoiceLineItem> LineItems { get; set; } = new();
    public InvoiceStatus Status { get; set; }
}