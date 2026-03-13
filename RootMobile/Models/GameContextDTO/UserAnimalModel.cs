using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{
    [Table("user_animals")]
    public class UserAnimalModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

        private long animalTypeId;

        [Column("animal_type_id")]
        public long AnimalTypeId
        {
            get => animalTypeId;
            set
            {
                if (animalTypeId != value)
                {
                    animalTypeId = value;
                    OnPropertyChanged(nameof(AnimalTypeId));
                }
            }
        }

        private int level;

        [Column("level")]
        public int Level
        {
            get => level;
            set
            {
                if (level != value)
                {
                    level = value;
                    OnPropertyChanged(nameof(Level));
                }
            }
        }

        private int exp;

        [Column("exp")]
        public int Exp
        {
            get => exp;
            set
            {
                if (exp != value)
                {
                    exp = value;
                    OnPropertyChanged(nameof(Exp));
                }
            }
        }

        private int currentHp;

        [Column("current_hp")]
        public int CurrentHp
        {
            get => currentHp;
            set
            {
                if (currentHp != value)
                {
                    currentHp = value;
                    OnPropertyChanged(nameof(CurrentHp));
                }
            }
        }

        private string nickname;

        [Column("nickname")]
        public string Nickname
        {
            get => nickname;
            set
            {
                if (nickname != value)
                {
                    nickname = value;
                    OnPropertyChanged(nameof(Nickname));
                }
            }
        }

        private bool isActive;

        [Column("is_active")]
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        private Guid owner;

        [Column("owner")]
        public Guid Owner
        {
            get => owner;
            set
            {
                if (owner != value)
                {
                    owner = value;
                    OnPropertyChanged(nameof(Owner));
                }
            }
        }

        [Reference(typeof(AnimalTypeModel))]
        public AnimalTypeModel AnimalType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}