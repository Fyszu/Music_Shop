namespace Music_Shop.Data
{
    public class Transaction
    {
        public int Id { get; set; }
        public User Buyer { get; set; }
        public float Price { get; set; }
        public Album Album { get; set; }
        public DateTime DateTime { get; set; }

    }
}
