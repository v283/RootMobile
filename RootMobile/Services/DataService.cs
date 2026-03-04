using System.ComponentModel;
using System.Diagnostics;

using RootMobile;
using RootMobile.Models;
using RootMobile.Views;
using RootMobile.Constants;
using CommunityToolkit.Maui.Views;
using Microsoft.IdentityModel.Tokens;
using RootMobile.Views.Templates;
using Supabase;
using Supabase.Gotrue;
using static Supabase.Postgrest.Constants;

namespace RootMobile.Services
{
    public class DataService : IDataService, INotifyPropertyChanged
    {
        // log
        // pasw 1234567
        private readonly Supabase.Client _supabaseClient;
        private readonly Supabase.Client _productsSupabaseClient;

        public static bool fallBackIsAvailable = true;

        private List<SuperCartModel> cart;
        public List<SuperCartModel> Cart
        {
            get
            {
                if (cart.IsNullOrEmpty())
                {
                    return new();
                }
                else { return cart; }
            }
            set
            {
                cart = value;
            }
        }
        

        public Supabase.Client SupabaseClient { get => _supabaseClient; }


        public DataService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            _productsSupabaseClient = new Supabase.Client(AppConfig.PRODUCTS_SUPABASE_URL, AppConfig.PRODUCTS_SUPABASE_KEY);
        }

        public async Task Initialize()
        {
            fallBackIsAvailable = await CheckProductsSupabaseAvailability();

            Cart = await GetCartItemAsync();
        }


        private async Task<bool> CheckProductsSupabaseAvailability()
        {
            try
            {
                // Простий тестовий запит (наприклад, отримання 1 запису)
                await _productsSupabaseClient
                    .From<ProductModel>()
                    .Select("*")
                    .Limit(1)
                    .Get();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка доступу до додаткового Supabase: {ex.Message}");
                return false;
            }
        }

        //auth


        public async Task SignInWithGoogleAsync()
        {
            try
            {
                var response = await _supabaseClient.Auth.SignIn(Supabase.Gotrue.Constants.Provider.Google);

                var authUri = new Uri(response.Uri.ToString());

                Uri redirectUri = new Uri("com.valentineos.avocadomobile://callback");

                var authResult = await WebAuthenticator.AuthenticateAsync(authUri, redirectUri);

                if (authResult != null)
                {

                    if (authResult.Properties.ContainsKey("access_token"))
                    {
                        var accessToken = authResult.Properties["access_token"];
                        var refreshToken = authResult.Properties["refresh_token"];


                        await _supabaseClient.Auth.SetSession(accessToken, refreshToken);

                        await SecureStorage.Default.SetAsync("authToken", accessToken);
                        await SecureStorage.Default.SetAsync("refreshToken", refreshToken);

                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Authentication failed: No access token found.", "OK");
                    }

                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Authentication failed: No auth result.", "OK");
                }
            }
            catch (TaskCanceledException)
            {
                await Shell.Current.DisplayAlert("Error", "Authentication was canceled by the user.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Authentication failed: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Authentication failed: {ex.Message}", "OK");
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var session = await _supabaseClient.Auth.SignIn(email, password);

            if (session != null)
            {
                await SecureStorage.Default.SetAsync("authToken", session.AccessToken);
                await SecureStorage.Default.SetAsync("refreshToken", session.RefreshToken);
                return true;
            }

            return false;
        }



        public async Task<bool> RestoreSessionAsync()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("authToken");
                var refreshToken = await SecureStorage.Default.GetAsync("refreshToken");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
                {
                    var session = await _supabaseClient.Auth.SetSession(token, refreshToken);

                    return session != null;

                }
            }
            catch (Exception ex)
            {

            }
            SecureStorage.RemoveAll();

            return false;
        }

        public async Task<bool> SignUpAsync(string email, string password, string name)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignUp(email, password);
                var emailCkeck = await Shell.Current.CurrentPage.ShowPopupAsync(new EmailCkeckPopup());

                if ((response != null) && (emailCkeck is bool result && result))
                {
                    if (Guid.TryParse(response.User.Id, out var userId))
                    {
                        var newUser = new UserDataModel
                        {
                            UserId = userId,
                            Email = email,
                            Phone = "",
                            Name = name,
                            Birth = new(),
                            Image = "svg_user.png"
                        };

                        await _supabaseClient.From<UserDataModel>().Insert(newUser);
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Помилка", "Такий email уже використовується", "Ок");
            }

            // треба буде іще удаляти токен авторизації якщо видалено акаунт користувача і обробляи ошибку якщо він є а акаунта в супі немає

            return false;
        }

        public async Task SignOutAsync()
        {

            await _supabaseClient.Auth.SignOut();
            SecureStorage.RemoveAll();
        }

        public async Task DeleteAccount()
        {
            if (Guid.TryParse(_supabaseClient.Auth.CurrentUser.Id, out var userId))
            {
                var newUser = new DeteleUserModel
                {
                    UserId = userId,
                    Email = _supabaseClient.Auth.CurrentUser.Email,
                    Phone = _supabaseClient.Auth.CurrentUser.Phone,
                };

                await _supabaseClient.From<DeteleUserModel>().Insert(newUser);
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var options = new ResetPasswordForEmailOptions(email)
                {
                    RedirectTo = "com.valentineos.avocadomobile://callback"
                };

                await _supabaseClient.Auth.ResetPasswordForEmail(options);

                await Shell.Current.DisplayAlert("Успіх", "Лист для скидання пароля надіслано.", "Ок");
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Помилка", $"Не вдалося надіслати лист: {ex.Message}", "Ок");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                // Оновлюємо пароль через токен
                await _supabaseClient.Auth.Update(
                    new Supabase.Gotrue.UserAttributes
                    {
                        Password = newPassword
                    }
                );

                await Shell.Current.DisplayAlert("Успіх", "Пароль змінено!", "Ок");
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Помилка", $"Не вдалося змінити пароль: {ex.Message}", "Ок");
                return false;
            }
        }



        //products
        public async Task<ProductModel> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _supabaseClient
                    .From<ProductModel>()
                    .Select("*")
                    .Where(x => x.Id == id)
                    .Get();

                var product = response.Models.FirstOrDefault();

                // Додаємо індикацію, чи продукт у кошику
                if (product != null && Cart.Any(c => c.ProductId == product.Id))
                {
                    product.CartIdent = "heart_done.png";
                }

                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetProductByIdAsync] Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ProductModel>> GetProductsAsync(int limit, int page, string category = "", string sub = "",  int sortOrder = 0, string findField = "")
        {
            List<ProductModel> rezult =new();
            if (fallBackIsAvailable)
            {
                try
                {
                    rezult = await FallbackData.GetProductsAsync(_productsSupabaseClient, limit, page, category, sub, sortOrder, findField);
                }
                catch (Exception ex)
                {
                    fallBackIsAvailable = false;
                    rezult = await FallbackData.GetProductsAsync(_supabaseClient, limit, page, category, sub, sortOrder, findField);
                }
            }
            else { rezult = await FallbackData.GetProductsAsync(_supabaseClient, limit, page, category, sub, sortOrder, findField); }


            foreach (var item in Cart)
            {
                for (int i = 0; i < rezult.Count; i++)
                {
                    if (item.ProductId == rezult[i].Id)
                    {
                        rezult[i].CartIdent = "heart_done.png";
                    }
                }

            }

            return rezult;
        }

        public async Task<List<CategoriesModel>> GetCategories()
        {
            var response = await _supabaseClient.From<CategoriesModel>().Select("*").Order("id", Supabase.Postgrest.Constants.Ordering.Ascending).Get();
            List<CategoriesModel> rezult = response.Models;

            return rezult;
        }


        //cart
        public async Task<bool> AddCartItemAsync(Guid userId, int productId, string productTable, int quantity)
        {
            try
            {
                SuperCartModel superCartItem = new();
                CartItemsModel cartItem = new();
                superCartItem.UserId = cartItem.UserId = userId;
                superCartItem.ProductId = cartItem.ProductId = productId;
                superCartItem.ProductTable = cartItem.ProductTable = productTable;
                superCartItem.Quantity = cartItem.Quantity = quantity;
                superCartItem.AddedAt = cartItem.AddedAt = DateTime.UtcNow;

                var response = await _supabaseClient.From<CartItemsModel>().Insert(cartItem);

                if (response.Models != null && response.Models.Count > 0)
                {
                    Cart.Add(superCartItem);
                    Console.WriteLine("Cart item added successfully!");
                    return true;
                }

                Console.WriteLine("Failed to add cart item.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding cart item: {ex.Message}");
                return false;
            }
        }

        public async Task RemoveCartItemAsync(Guid userId, int productId)
        {
            try
            {
                Cart.Remove(Cart.FirstOrDefault(ci => ci.ProductId == productId));
            }
            catch (Exception ex) { }

            await _supabaseClient.From<CartItemsModel>().Where(x => x.UserId == userId).Where(x => x.ProductId == productId).Delete();
        }

        public async Task<List<SuperCartModel>> GetCartItemAsync()
        {
            //get products id, count by user id
            List<SuperCartModel> rezult = new();

            if (Guid.TryParse(SupabaseClient.Auth.CurrentUser.Id, out var userId))
            {
                var response = await _supabaseClient.From<SuperCartModel>()
                    .Select("*").Where(x => x.UserId == userId)
                    .Get();
                return response.Models;
            }

            return rezult;
        }

        public async Task<List<ProductModel>> GetCart(List<int> productIds)
        {
            // get info about product by it's id
            var response = await _supabaseClient
                .From<ProductModel>()
                .Filter(x => x.Id, Operator.In, productIds)
                .Select("*").Get();

            List<ProductModel> rezult = response.Models;
            for (int i = 0; i < rezult.Count; i++)
            {
                rezult[i].CartIdent = "heart_done.png";
            }

            return rezult;
        }

        public async Task UpdateCartItemAsync(Guid userId, int productId, float quantity)
        {
            await _supabaseClient.From<CartItemsModel>()
                .Where(x => x.UserId == userId)
                .Where(x => x.ProductId == productId)
                .Set(x => x.Quantity, quantity).Update();
        }




        //user
        public async Task<UserDataModel> GetUserData()
        {
            var rezult = new UserDataModel();

            if (Guid.TryParse(_supabaseClient.Auth.CurrentUser?.Id, out var userId))
            {
                var existingUser = await _supabaseClient
                    .From<UserDataModel>()
                    .Select("*").Where(x => x.UserId == userId).Get();

                if (existingUser.Models.Count == 0) // Insert only if user does not exist
                {
                    var newUser = new UserDataModel
                    {
                        UserId = userId,
                        Email = _supabaseClient.Auth.CurrentUser.Email,
                        Phone = "",
                        Name = "User",
                        Birth = new(),
                        Image = "svg_user.png"
                    };

                    await _supabaseClient.From<UserDataModel>().Insert(newUser);
                    return newUser; // Return the new user directly
                }

                return existingUser.Models.FirstOrDefault() ?? rezult;
            }

            return rezult;
        }

        public async Task UpdateUserDataAsync(string name, string email, string phone, DateTime? birth, string image = "svg_user.png")
        {
            try
            {
                if (Guid.TryParse(SupabaseClient.Auth.CurrentUser.Id, out var userId))
                {

                    var response = await _supabaseClient.From<UserDataModel>().
                        Where(x => x.UserId == userId)
                        .Set(x => x.Email, email).Set(x => x.Phone, phone).Set(x => x.Name, name).Set(x => x.Birth, birth).Set(x => x.Image, image)
                        .Update(); ;

                    if (SupabaseClient.Auth.CurrentUser.Email != email)
                    {
                        // change email in auth shema
                    }
                    if (SupabaseClient.Auth.CurrentUser.Phone != phone)
                    {
                        // change phone in auth shema
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user data: {ex.Message}");

            }
        }




        //comments and marks
        public async Task<bool> AddMarkAsync(ProductModel product, string commentText, string location, int mark, string shop)
        {
            try
            {
                var userData = await GetUserData();
                if (userData == null || userData.UserId == Guid.Empty)
                {
                    return false;
                }

                var currentTime = DateTime.UtcNow;
                var roundedTime = new DateTime(
                    currentTime.Year,
                    currentTime.Month,
                    currentTime.Day
                );

                MarkModel newMark = new()
                {
                    UserId = userData.UserId,
                    Name = userData.Name,
                    WritenAt = roundedTime,
                    Text = commentText,
                    Images = " ",
                    Mark = mark,
                    ProductId = product.Id,
                    Shop = shop,
                    Location = location
                };

                var response = await _supabaseClient.From<MarkModel>().Insert(newMark);

                if (response != null && response.Models.Count > 0) { return true; }
                else { return false; }
            }
            catch (Exception ex) { return false; }
        }
        public async Task<List<MarkModel>> GetMarksAsync(ProductModel product)
        {
            var response = await _supabaseClient.From<MarkModel>().Where(x => x.ProductId == product.Id).Get();
            return response.Models;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
