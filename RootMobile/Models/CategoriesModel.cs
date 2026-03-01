using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RootMobile.Models
{
    [Table("categories_ssub")]
    public class CategoriesModel : BaseModel, INotifyPropertyChanged
    {
        private string subJson = "";
        private bool isSelected;

        [PrimaryKey("id")]
        public long Id { get; set; }   // bigint -> long

        [Column("name")]
        public string Name { get; set; }

        [Column("image")]
        public string Image { get; set; }

        [Column("subjson")]
        public string SubJson
        {
            get => subJson;
            set
            {
                subJson = value ?? "";

                // subjson = ["A","B","C"]  -> List<string> -> List<SubCategoriesModel>
                SubCategory = ParseSubCategories(subJson);

                OnPropertyChanged(); // SubJson
                OnPropertyChanged(nameof(SubCategory));
            }
        }

        // не колонка БД, а зручна структура для UI
        public List<SubCategoriesModel> SubCategory { get; set; } = new();

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private static List<SubCategoriesModel> ParseSubCategories(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<SubCategoriesModel>();

            try
            {
                var names = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

                var result = new List<SubCategoriesModel>(names.Count);
                for (int i = 0; i < names.Count; i++)
                {
                    result.Add(new SubCategoriesModel
                    {
                        // Id тут НЕ з БД, а просто для UI-ідентифікації
                        Id = i + 1,
                        Name = names[i],
                        IsSelected = false
                    });
                }
                return result;
            }
            catch
            {
                // якщо раптом прилетить не масив, або битий JSON
                return new List<SubCategoriesModel>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}