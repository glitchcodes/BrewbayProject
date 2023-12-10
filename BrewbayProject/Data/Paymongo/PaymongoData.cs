namespace BrewbayProject.Data.Paymongo;

public class PaymongoData
{
    public Attributes attributes { get; set; }
}

public class Attributes
{
    public bool send_email_receipt { get; set; }
    public bool show_description { get; set; }
    public bool show_line_items { get; set; }
    public string description { get; set; }
    public string success_url { get; set; }
    public string cancel_url { get; set; }
    
    public List<String> payment_method_types { get; set; }
    public List<LineItem> line_items { get; set; }
}

public class LineItem
{
    public string name { get; set; }
    public string description { get; set; }
    public int amount { get; set; }
    public int quantity { get; set; }
    public string currency { get; set; }
    public List<String> images { get; set; }
}