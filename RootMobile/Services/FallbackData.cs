using System;
using static Supabase.Postgrest.Constants;
using Supabase.Interfaces;
using RootMobile.Models;

namespace RootMobile.Services
{
  public class FallbackData
  {

    public static async Task<List<ProductModel>> GetProductsAsync(
        Supabase.Client supabaseClient,
        int limit,
        int page,
        string category = "",
        string sub = "",
        string subsub = "",
        int sortOrder = 0,
        string findField = "")
    {
      var offset = (page - 1) * limit;

      var query = supabaseClient.From<ProductModel>()
          .Where(x => x.Category.Contains(category))
          .Where(x => x.Subcategory.Contains(sub))
          .Where(x => x.Subsubcategory.Contains(subsub));

      if (!string.IsNullOrWhiteSpace(findField))
      {
        query = query.Filter("name", Operator.ILike, $"%{findField}%");
      }

      query = sortOrder switch
      {
        0 => query.Order(x => x.Top, Ordering.Descending),
        1 => query.Order(x => x.Price, Ordering.Ascending),
        2 => query.Order(x => x.Price, Ordering.Descending),
        3 => query.Order(x => x.Name, Ordering.Ascending),
        4 => query.Order(x => x.Name, Ordering.Descending),
        _ => query.Order(x => x.Id, Ordering.Ascending)
      };

      var response = await query.Range(offset, offset + limit - 1).Get();
      return response.Models;
    }

  }
}

