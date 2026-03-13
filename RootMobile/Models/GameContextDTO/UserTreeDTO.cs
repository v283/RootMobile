using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;

namespace RootMobile.Models
{
    [Table("user_trees")]
    public class UserTreeModel : BaseModel, INotifyPropertyChanged
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

        private float currentGrowProgress;

        [Column("current_grow_progress")]
        public float CurrentGrowProgress
        {
            get => currentGrowProgress;
            set
            {
                if (currentGrowProgress != value)
                {
                    currentGrowProgress = value;
                    OnPropertyChanged(nameof(CurrentGrowProgress));
                }
            }
        }

        private long treeId;

        [Column("tree_id")]
        public long TreeId
        {
            get => treeId;
            set
            {
                if (treeId != value)
                {
                    treeId = value;
                    OnPropertyChanged(nameof(TreeId));
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