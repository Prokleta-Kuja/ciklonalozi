namespace ciklonalozi.Data
{
    public class Contact
    {
        public Contact(string name, string? phone = null)
        {
            Name = name;
            Phone = phone;
        }

        public int ContactId { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
    }
}