using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{
    [Table("users_data")]
    public class UserDataModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

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

        private string lastName;

        private DateTime? birth;

        [Column("birth")]
        public DateTime? Birth
        {
            get => birth;
            set
            {
                if (birth != value)
                {
                    birth = value;
                    OnPropertyChanged(nameof(Birth));
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
        
        private string email;

        [Column("email")]
        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private string phone;

        [Column("phone")]
        public string Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        private string status;

        [Column("status")]
        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private int? treesPlanted;

        [Column("trees_planted")]
        public int? TreesPlanted
        {
            get => treesPlanted;
            set
            {
                if (treesPlanted != value)
                {
                    treesPlanted = value;
                    OnPropertyChanged(nameof(TreesPlanted));
                }
                else
                {
                    treesPlanted = 0;
                    OnPropertyChanged(nameof(TreesPlanted));
                }
            }
        }
        
        private int? treesFound;

        [Column("trees_found")]
        public int? TreesFound
        {
            get => treesFound;
            set
            {
                if (treesFound != value)
                {
                    treesFound = value;
                    OnPropertyChanged(nameof(TreesFound));
                }
                else
                {
                    treesFound = 0;
                    OnPropertyChanged(nameof(TreesFound));
                }
            }
        }

        private int? topRating;

        [Column("top_rating")]
        public int? TopRating
        {
            get => topRating;
            set
            {
                if (topRating != value)
                {
                    topRating = value;
                    OnPropertyChanged(nameof(TopRating));
                }
                else
                {
                    topRating = 999999;
                    OnPropertyChanged(nameof(TopRating));
                }
            }
        }

        private int? badgesCount;

        [Column("badges_count")]
        public int? BadgesCount
        {
            get => badgesCount;
            set
            {
                if (badgesCount != value)
                {
                    badgesCount = value;
                    OnPropertyChanged(nameof(BadgesCount));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}