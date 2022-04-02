namespace Music_Shop.Data
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Album> Albums { get; set; }

        public Artist()
        {
            Albums = new HashSet<Album>();
        }
    }
}
