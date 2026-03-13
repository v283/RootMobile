using System;
using System.ComponentModel;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RootMobile.Models
{
    [Table("plant_comments")]
    public class PlantCommentModel : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

        [Column("plant_pin_id")]
        public long PlantPinId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        private string userName;
        [Column("user_name")]
        public string UserName
        {
            get => userName;
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        private string userImage;
        [Column("user_image")]
        public string UserImage
        {
            get => userImage;
            set
            {
                if (userImage != value)
                {
                    userImage = value;
                    OnPropertyChanged(nameof(UserImage));
                }
            }
        }

        private string commentText;
        [Column("comment_text")]
        public string CommentText
        {
            get => commentText;
            set
            {
                if (commentText != value)
                {
                    commentText = value;
                    OnPropertyChanged(nameof(CommentText));
                }
            }
        }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}