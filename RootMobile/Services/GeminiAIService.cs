using System;
using Mscc.GenerativeAI;
using System.Data.Common;
using RootMobile.Models;
using Microsoft.IdentityModel.Tokens;
using RootMobile.Services;

namespace RootMobile.Services
{
    public class GeminiAIService
    {
        private readonly DataService _dataService;
        public GeminiAIService(DataService dataService)
        {
            _dataService = dataService;
        }

        private static readonly string ApiKey = "AIzaSyA3yOce0NeFxQ9TORu7Lt__WaODB1KXrd8";

//         public async Task<List<ProductModel>> RunClassificationAsync(List<string> products)
//         {
//             List<ProductModel> rezult = new List<ProductModel>();
//
//             foreach (var item in products)
//             {
//                 rezult.Add(new ProductModel() { Name = item });
//             }
//             string categoriesJson;
//
//             categoriesJson = await GetSbSSbCategoriesJson();
//
//             if (string.IsNullOrEmpty(categoriesJson)) { throw new InvalidOperationException("Categories JSON is empty or null."); }
//
//             var systemInstruction = new Content("""
//             Ти експерт із класифікації інгредієнтів за наданими критеріями.
//
//             Результат для кожного інгредієнту повинен бути у форматі:
//             "category;subcategory;subsubcategory|"
//             (має бути рівно три символи ";" та жодна з частин не може бути порожньою).
//             Кількість прокласифікованих інгредієнтів у відповіді повинна строго відповідати кількості введених інгредієнтів.
//             Якщо запит відправили повторно, отже ти зробив помилку: не правильна кількість інградієнтів прокласифікована, або немає такої кетегорії.
//             Використовуй тільки ті слова, які містяться у наданому JSON. УВАЖНО ПЕРЕВІР результат перед відправкою.
//             Структура JSON виглядає так:
//             {
//             "categories": {
//             "category": {
//             "subcategory": [
//             "subsubcategory1"
//             "subsubcategory2",
//             "subsubcategory3"
//             ]
//             }
//             }
//             }
//
//             Ти не знаєш інших слів для класифікації, крім тих, що є в JSON, і не можеш їх змінювати.
//             Якщо для якогось інгредієнту немає відповідної категорії, субкатегорії або субсубкатегорії, поверни значення " ; ; |".
//             У відповіді використовуй тільки слова з JSON, не додаючи інших.
//
//             json:
//             """
//                 + categoriesJson);
//
//
//             IGenerativeAI genAi = new GoogleAI(ApiKey);
//             var model = genAi.GenerativeModel(Mscc.GenerativeAI.Model.Gemini20FlashExperimental, systemInstruction: systemInstruction);
//
//
//
//
//
//             try
//             {
//                 string prompt = string.Join("\n", products);
//
//                 var request = new GenerateContentRequest(prompt);
//                 var response = await model.GenerateContent(request);
//
//                 Console.WriteLine($"Запит:\n{prompt}");
//                 Console.WriteLine($"Відповідь:\n{response.Text}\n");
//
//                 FillProductsListWithCategories(rezult, response.Text);
//
//             }
//             catch (Exception ex)
//             {
//
//             }
//
//
//             return rezult;
//         }
//
//         public async Task<List<string>> RunDefineProductsAsync(string prompt)
//         {
//             var systemInstruction = new Content("""
//             Ти експерт із класифікації кулінарних інгредієнтів. Твоя задача – розділити наданий список інгредієнтів на окремі складові, ідентифікуючи кожен продукт, не враховуючи кількості та додаткові слова (наприклад, "для змащування").  Результат повинен бути у форматі: "продукт1;продукт2;продукт3;...;продуктN". Використовуй лише назви продуктів, без додаткових слів, кількостей та одиниць виміру. Порядок продуктів має відповідати вхідному списку.
//             """
//                 );
//
//
//             IGenerativeAI genAi = new GoogleAI(ApiKey);
//             var model = genAi.GenerativeModel(Mscc.GenerativeAI.Model.Gemini20FlashExperimental, systemInstruction: systemInstruction);
//
//
//             try
//             {
//
//                 var request = new GenerateContentRequest(prompt);
//                 var response = await model.GenerateContent(request);
//
//                 return (response.Text).Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
//             }
//             catch (Exception ex)
//             {
//
//             }
//             return new();
//
//         }
//
//
//
//
//         public static List<ProductModel> FillProductsListWithCategories(List<ProductModel> products, string qeminiOutput)
//         {
//             qeminiOutput = qeminiOutput.Replace("\n", "").Trim();
//             if (qeminiOutput[qeminiOutput.Length - 1] == '|')
//             {
//                 qeminiOutput = qeminiOutput.Remove(qeminiOutput.Length - 1);
//             }
//             var outputs = qeminiOutput.Split('|', StringSplitOptions.RemoveEmptyEntries);
//
//
//             for (int i = 0; i < products.Count; i++)
//             {
//                 string[] temp = outputs[i].Split(';');
//                 if (temp.Length < 3)
//                 {
//                     throw new FormatException("output is not in the expected format: 'категорія;субкатегорія;субсубкатегорія'");
//                 }
//
//                 products[i].Category = temp[0].Trim();
//                 products[i].Subcategory = temp[1].Trim();
//                 products[i].Subsubcategory = temp[2].Trim();
//             }
//
//             return products;
//         }
//
//
//
//
//
//
//         public async Task<List<string>> GetProductFromGPT(string prompt)
//         {
//             var systemInstruction = new Content("""
//                         Твоє завдання – з наданого тексту вилучити лише **продуктів харчування**. Кожен продукт у переліку має бути представлений у такому форматі: "Назва продукту;".
//
//             Важливо:
//             1.  До результату повинні бути включені **тільки ті елементи списку, які є продуктами харчування**.
//             2.  Ігноруй будь-який інший текст, що не є частиною списку продуктів харчування (наприклад, вступні речення, пояснення, загальну суму тощо).
//             3.  В кінці отриманого продукту обовєʼязково має бути ';'
//             4.  Не номеруй продукти, лише назва продукту
//             """
//                 );
//
//
//             IGenerativeAI genAi = new GoogleAI(ApiKey);
//             var model = genAi.GenerativeModel(Mscc.GenerativeAI.Model.Gemini20FlashExperimental, systemInstruction: systemInstruction);
//
//
//             try
//             {
//
//                 var request = new GenerateContentRequest(prompt);
//                 var response = await model.GenerateContent(request);
//                 
//
//                 return (response.Text.Replace("\n","")).Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
//             }
//             catch (Exception ex)
//             {
//
//             }
//             return new();
//
//         }
    }
}

