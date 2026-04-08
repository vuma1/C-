namespace QuanLyQuanCafe.Models
{
    public class OrderDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; private set; } // Tổng tiền = số lượng * đơn giá

        // Constructor
        public OrderDetail(string productName, int quantity, double price)
        {
            ProductName = productName;
            Quantity = quantity;
            Price = price;
            Total = quantity * price; // Tự động tính tổng
        }

        // Hàm cập nhật số lượng và tính lại tổng tiền
        public void UpdateQuantity(int newQuantity)
        {
            Quantity = newQuantity;
            Total = Quantity * Price; // Cập nhật lại tổng tiền
        }
    }
}
