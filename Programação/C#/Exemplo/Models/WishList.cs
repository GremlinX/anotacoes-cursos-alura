namespace backend_wishlist.Models;

public class WishList
{
    public long Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public DateTime DataCriada { get; set; }
    public WishList()
    {
        DataCriada = DateTime.UtcNow;
    }
}
