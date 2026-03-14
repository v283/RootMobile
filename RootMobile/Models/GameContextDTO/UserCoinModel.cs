using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

[Table("user_moneys")]
public class UserCoinModel : BaseModel, INotifyPropertyChanged
{
    // Це основне поле для ID користувача (UUID в базі)
    [PrimaryKey("id")]
    public string Id { get; set; }

    private int _coins;
    [Column("coins")]
    public int Coins
    {
        get => _coins;
        set { _coins = value; OnPropertyChanged(nameof(Coins)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}