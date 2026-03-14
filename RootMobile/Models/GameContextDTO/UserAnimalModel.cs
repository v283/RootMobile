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
        public long id { get; set; }

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

        private int current_hp;

        [Column("current_hp")]
        public int CurrentHp
        {
            get => current_hp;
            set
            {
                if (current_hp != value)
                {
                    current_hp = value;
                    OnPropertyChanged(nameof(CurrentHp));
                }
            }
        }

        private bool is_active;

        [Column("is_active")]
        public bool IsActive
        {
            get => is_active;
            set
            {
                if (is_active != value)
                {
                    is_active = value;
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