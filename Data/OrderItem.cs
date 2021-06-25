namespace ciklonalozi.Data
{
    public class OrderItem
    {
        public OrderItem(string title)
        {
            Title = title;
        }

        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string Title { get; set; }

        public Order? Order { get; set; }
    }
}