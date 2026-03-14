using System;
using Mscc.GenerativeAI;
using System.Data.Common;
using RootMobile.Models;
using Microsoft.IdentityModel.Tokens;
using RootMobile.Services;
using System;

namespace RootMobile.Services
{
    //public class GeminiAIServiceVan
    //{
    //    // ВАЖЛИВО: Використовуйте 1.5 Flash для швидкості та аналізу фото
    //    private static readonly string ApiKey = "AIzaSyA3yOce0NeFxQ9TORu7Lt__WaODB1KXrd8";
    //    private readonly Microsoft.Maui.Storage.IFileSystem _fileSystem;

    //    public GeminiAIServiceVan()
    //    {
    //        // Gemini 1.5 Flash найкраще підходить для Vision задач
    //    }

    //    public async Task<string> IdentifyPlantAsync(string localPath)
    //    {
    //        try
    //        {
    //            // 1. Читаємо файл зображення
    //            byte[] imageBytes = await File.ReadAllBytesAsync(localPath);
    //            string base64Image = Convert.ToBase64String(imageBytes);

    //            // 2. Створюємо системну інструкцію
    //            var systemInstruction = "Ти професійний ботанік. Твоє завдання — ідентифікувати рослину на фото. " +
    //                                    "Якщо на зображенні НЕ рослина, відповідай лише одним словом: NOT_PLANT. " +
    //                                    "Якщо це рослина, напиши лише її назву українською мовою (наприклад: Фікус Бенджаміна).";

    //            IGenerativeAI genAi = new GoogleAI(ApiKey);
    //            var model = genAi.GenerativeModel(Mscc.GenerativeAI.Model.Gemini15Flash, systemInstruction: new Content(systemInstruction));

    //            // 3. Формуємо запит із текстом та зображенням (Multimodal)
    //            var request = new GenerateContentRequest("Що це за рослина?");
    //            request.Contents[0].Parts.Add(new Part
    //            {
    //                InlineData = new InlineData
    //                {
    //                    MimeType = "image/jpeg",
    //                    Data = base64Image
    //                }
    //            });

    //            var response = await model.GenerateContent(request);

    //            return response.Text?.Trim() ?? "NOT_PLANT";
    //        }
    //        catch (Exception ex)
    //        {
    //            return $"ERROR: {ex.Message}";
    //        }
    //    }
    //}
}

