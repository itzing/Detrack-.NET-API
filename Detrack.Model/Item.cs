namespace Detrack.Model
{
	public class Item
	{
		public Item(string sku, string description, int qty)
		{
			Sku = sku;
			Desc = description;
			Qty = qty;
		}

		public Item() { }

		public string Sku { get; set; }
		public string Desc { get; set; }
		public decimal? Qty { get; set; }
		public int Reject { get; set; }
		public string Reason { get; set; }
	}
}
