using System.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RootMobile.Models
{

    [Table("products_all_old")]

    public class ProductModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        private string name;
        [Column("name")]
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string image;
        [Column("image")]
        public string Image
        {
            get => image;
            set
            {
                if (image != value)
                {
                    image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        private string size;
        [Column("size")]
        public string Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                    OnPropertyChanged(nameof(Size));
                }
            }
        }

        private string category;
        [Column("category")]
        public string Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        private string subcategory;
        [Column("subcategory")]
        public string Subcategory
        {
            get => subcategory;
            set
            {
                if (subcategory != value)
                {
                    subcategory = value;
                    OnPropertyChanged(nameof(Subcategory));
                }
            }
        }

        private string subsubcategory;
        [Column("subsubcategory")]
        public string Subsubcategory
        {
            get => subsubcategory;
            set
            {
                if (subsubcategory != value)
                {
                    subsubcategory = value;
                    OnPropertyChanged(nameof(Subsubcategory));
                }
            }
        }

        private string barcode;
        [Column("barcode")]
        public string Barcode
        {
            get => barcode;
            set
            {
                if (barcode != value)
                {
                    barcode = value;
                    OnPropertyChanged(nameof(Barcode));
                }
            }
        }

        private string about;
        [Column("about")]
        public string About
        {
            get => about;
            set
            {
                if (about != value)
                {
                    about = value;
                    OnPropertyChanged(nameof(About));
                }
            }
        }

        private string shops;
        [Column("shops")]
        public string Shops
        {
            get => shops;
            set
            {
                if (shops != value)
                {
                    shops = value;
                    OnPropertyChanged(nameof(Shops));
                }
            }
        }

        private string ratingTable;
        [Column("ratingtable")]
        public string RatingTable
        {
            get => ratingTable;
            set
            {
                if (ratingTable != value)
                {
                    ratingTable = value;
                    OnPropertyChanged(nameof(RatingTable));
                }
            }
        }

        private string country;
        [Column("county")]
        public string Country
        {
            get => country;
            set
            {
                if (country != value)
                {
                    country = value;
                    OnPropertyChanged(nameof(Country));
                }
            }
        }

        private string tradeMark;
        [Column("trademark")]
        public string TradeMark
        {
            get => tradeMark;
            set
            {
                if (tradeMark != value)
                {
                    tradeMark = value;
                    OnPropertyChanged(nameof(TradeMark));
                }
            }
        }

        private float price = 0;
        [Column("price")]
        public float Price
        {
            get => price;
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        private DateTime? lastUpdate;
        [Column("last_update")]
        public DateTime? LastUpdate
        {
            get => lastUpdate;
            set
            {
                if (lastUpdate != value)
                {
                    lastUpdate = value;
                    OnPropertyChanged(nameof(LastUpdate));
                }
            }
        }

        [PrimaryKey("top")]
        public int Top { get; set; }
        private string cartIdent;
        public string CartIdent
        {
            get => string.IsNullOrEmpty(cartIdent) ? "heart.png" : cartIdent;
            set
            {
                    cartIdent = value;
                    OnPropertyChanged(nameof(CartIdent));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

