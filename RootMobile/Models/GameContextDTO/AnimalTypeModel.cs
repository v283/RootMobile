using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{
    [Table("animal_types")]
    public class AnimalTypeModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

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

        private int attack;

        [Column("attack")]
        public int Attack
        {
            get => attack;
            set
            {
                if (attack != value)
                {
                    attack = value;
                    OnPropertyChanged(nameof(Attack));
                }
            }
        }

        private double attackSpeed;

        [Column("attack_speed")]
        public double AttackSpeed
        {
            get => attackSpeed;
            set
            {
                if (attackSpeed != value)
                {
                    attackSpeed = value;
                    OnPropertyChanged(nameof(AttackSpeed));
                }
            }
        }

        private int hp;

        [Column("hp")]
        public int Hp
        {
            get => hp;
            set
            {
                if (hp != value)
                {
                    hp = value;
                    OnPropertyChanged(nameof(Hp));
                }
            }
        }

        private int defense;

        [Column("defense")]
        public int Defense
        {
            get => defense;
            set
            {
                if (defense != value)
                {
                    defense = value;
                    OnPropertyChanged(nameof(Defense));
                }
            }
        }

        private int speed;

        [Column("speed")]
        public int Speed
        {
            get => speed;
            set
            {
                if (speed != value)
                {
                    speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }

        private double critChance;

        [Column("crit_chance")]
        public double CritChance
        {
            get => critChance;
            set
            {
                if (critChance != value)
                {
                    critChance = value;
                    OnPropertyChanged(nameof(CritChance));
                }
            }
        }

        private double critDamage;

        [Column("crit_damage")]
        public double CritDamage
        {
            get => critDamage;
            set
            {
                if (critDamage != value)
                {
                    critDamage = value;
                    OnPropertyChanged(nameof(CritDamage));
                }
            }
        }

        private string rarity;

        [Column("rarity")]
        public string Rarity
        {
            get => rarity;
            set
            {
                if (rarity != value)
                {
                    rarity = value;
                    OnPropertyChanged(nameof(Rarity));
                }
            }
        }

        private DateTime createdAt;

        [Column("created_at")]
        public DateTime CreatedAt
        {
            get => createdAt;
            set
            {
                if (createdAt != value)
                {
                    createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}