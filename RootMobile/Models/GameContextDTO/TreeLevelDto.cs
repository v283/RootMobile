using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{
    [Table("trees")]
    public class TreeLevelModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

        private int lvl;

        [Column("lvl")]
        public int Lvl
        {
            get => lvl;
            set
            {
                if (lvl != value)
                {
                    lvl = value;
                    OnPropertyChanged(nameof(Lvl));
                }
            }
        }

        private int amountForLevelUp;

        [Column("amount_for_level_up")]
        public int AmountForLevelUp
        {
            get => amountForLevelUp;
            set
            {
                if (amountForLevelUp != value)
                {
                    amountForLevelUp = value;
                    OnPropertyChanged(nameof(AmountForLevelUp));
                }
            }
        }

        private float offsetX;

        [Column("offset_x")]
        public float OffsetX
        {
            get => offsetX;
            set
            {
                if (offsetX != value)
                {
                    offsetX = value;
                    OnPropertyChanged(nameof(OffsetX));
                }
            }
        }

        private float offsetY;

        [Column("offset_y")]
        public float OffsetY
        {
            get => offsetY;
            set
            {
                if (offsetY != value)
                {
                    offsetY = value;
                    OnPropertyChanged(nameof(OffsetY));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}