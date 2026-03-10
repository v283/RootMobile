using System;
using System.ComponentModel;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RootMobile.Models
{
    [Table("map_points")]
    public class PlantPinDataModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id", false)] // false означає, що ми не генеруємо Id самі, а чекаємо від бази
        public long Id { get; set; } // Змінено з Guid на long

        private DateTime created;
        [Column("created_at")]
        public DateTime Created
        {
            get => created;
            set
            {
                if (created != value)
                {
                    created = value;
                    OnPropertyChanged(nameof(Created));
                }
            }
        }

        private Guid userId;
        [Column("user_id")]
        public Guid UserId
        {
            get => userId;
            set
            {
                if (userId != value)
                {
                    userId = value;
                    OnPropertyChanged(nameof(UserId));
                }
            }
        }

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

        private string description;
        [Column("description")]
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private double latitude;
        [Column("latitude")]
        public double Latitude
        {
            get => latitude;
            set
            {
                if (Math.Abs(latitude - value) > 0.0000001)
                {
                    latitude = value;
                    OnPropertyChanged(nameof(Latitude));
                }
            }
        }

        private double longitude;
        [Column("longitude")]
        public double Longitude
        {
            get => longitude;
            set
            {
                if (Math.Abs(longitude - value) > 0.0000001)
                {
                    longitude = value;
                    OnPropertyChanged(nameof(Longitude));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}