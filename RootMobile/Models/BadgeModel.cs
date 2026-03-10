using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{
    [Table("badges")]
    public class BadgeModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

        private string title;

        [Column("title")]
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
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

        private string type;

        [Column("type")]
        public string Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        private string conditionType;

        [Column("condition_type")]
        public string ConditionType
        {
            get => conditionType;
            set
            {
                if (conditionType != value)
                {
                    conditionType = value;
                    OnPropertyChanged(nameof(ConditionType));
                }
            }
        }

        private int? conditionValue;

        [Column("condition_value")]
        public int? ConditionValue
        {
            get => conditionValue;
            set
            {
                if (conditionValue != value)
                {
                    conditionValue = value;
                    OnPropertyChanged(nameof(ConditionValue));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}