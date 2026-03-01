using System;
using Supabase.Postgrest.Attributes;
using System.ComponentModel;
using Supabase.Postgrest.Models;

namespace RootMobile.Models
{
    [Table("cart_items")]
    public class SuperCartModel: BaseModel, INotifyPropertyChanged
    {
        private Guid _id;
        private Guid _userId;
        private int _productId;
        private string _productTable = string.Empty;
        private float _quantity;
        private DateTime _addedAt;

        private ProductModel product;
        private float pay;

        [PrimaryKey("id")]
        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        [Column("user_id")]
        public Guid UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        [Column("product_id")]
        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }

        [Column("product_table")]
        public string ProductTable
        {
            get => _productTable;
            set
            {
                _productTable = value;
                OnPropertyChanged(nameof(ProductTable));
            }
        }

        [Column("quantity")]
        public float Quantity
        {
            get => _quantity;
            set
            {
                _quantity = (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);
                Pay = _quantity * (Product?.Price ?? 0);
                OnPropertyChanged(nameof(Quantity));
            }
        }

        [Column("added_at")]
        public DateTime AddedAt
        {
            get => _addedAt;
            set
            {
                _addedAt = value;
                OnPropertyChanged(nameof(AddedAt));
            }
        }


        public ProductModel Product
        {
            get => product;
            set
            {
                product = value;
                Pay = Quantity * (product?.Price ?? 0);
                OnPropertyChanged(nameof(Product));
            }
        }

        public float Pay
        {
            get => pay;
            set
            {
                pay = value;
                OnPropertyChanged(nameof(Pay));
            }
        }


        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

