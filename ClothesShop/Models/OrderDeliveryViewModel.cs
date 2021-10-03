namespace ClothesShop.Models
{
    public class OrderDeliveryViewModel
    {
        public long EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeMobileNumber { get; set; }
        public int NumberOfOrders { get; set; }
        public double TotalOrderCash { get; set; }
    }
}