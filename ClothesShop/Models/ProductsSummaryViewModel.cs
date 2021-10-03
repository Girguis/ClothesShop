namespace ClothesShop.Models
{
    public class ProductsSummaryViewModel
    {
        public string ProductName { get; set; }
        public int TotalIncoming { get; set; }
        public int TotalReturned { get; set; }
        public int TotalSelled { get; set; }
        public int TotalRemaining
        {
            get
            {
                return TotalIncoming - (TotalReturned + TotalSelled);
            }
        }
        public double OriginalPrice { get; set; }
        public double TotalProductPrice
        {
            get
            {
                return TotalRemaining * OriginalPrice;
            }
        }
    }
}