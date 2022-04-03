namespace Music_Shop.Data
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public Artist Owner { get; set; }
        public float Price { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public Album()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}
