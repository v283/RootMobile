using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{

    [Table("mark_table")]
    public partial class MarkModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        private string name;

        [Column("user_name")]
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

        private DateTime? writenAt;

        [Column("writen_at")]
        public DateTime? WritenAt
        {
            get => writenAt;
            set
            {
                if (writenAt != value)
                {
                    writenAt = value;
                    OnPropertyChanged(nameof(WritenAt));
                }
            }
        }

        private string text;
        [Column("text")]
        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        private string images;
        [Column("images")]
        public string Images
        {
            get => images;
            set
            {
                if (images != value)
                {
                    images = value;
                    OnPropertyChanged(nameof(Images));
                }
            }
        }

        private int mark;

        [Column("mark")]
        public int Mark
        {
            get => mark;
            set
            {
                if (mark != value)
                {
                    mark = value;
                    OnPropertyChanged(nameof(Mark));
                }

            }
        }
        //product_id

        private int productId;
        [Column("product_id")]
        public int ProductId
        {
            get => productId;
            set
            {
                if (productId != value)
                {
                    productId = value;
                    OnPropertyChanged(nameof(ProductId));
                }
            }
        }

        private string shop;
        [Column("shop")]
        public string Shop
        {
            get => shop;
            set
            {
                if (shop != value)
                {
                    shop = value;
                    OnPropertyChanged(nameof(Shop));
                }
            }
        }
        //location

        private string location;
        [Column("location")]
        public string Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }

}

