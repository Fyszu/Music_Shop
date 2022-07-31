using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Shop.Data
{
    public class Album : Product
    {
        public ICollection<Artist> Artists { get; set; } = new HashSet<Artist>();
    }
}
