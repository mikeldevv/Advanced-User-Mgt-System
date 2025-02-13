namespace UMS.Contracts.InvoicingApp.Poco;

public class InvoiceLineItem
{
    public long Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}