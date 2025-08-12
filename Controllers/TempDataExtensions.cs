using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using X.PagedList;

namespace Application.Controllers;

public class SerializablePagedList<T>
{
    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItemCount { get; set; }
}

public static class Extensions
{
    public static string ToJson<T>(this IPagedList<T> pagedList)
    {
        var dto = new SerializablePagedList<T>
        {
            Items = pagedList.ToList(),
            PageNumber = pagedList.PageNumber,
            PageSize = pagedList.PageSize,
            TotalItemCount = pagedList.TotalItemCount
        };

        return JsonSerializer.Serialize(dto);
    }

    public static IPagedList<T> FromJson<T>(string json)
    {
        var dto = JsonSerializer.Deserialize<SerializablePagedList<T>>(json);

        return new StaticPagedList<T>(
            dto.Items,
            dto.PageNumber,
            dto.PageSize,
            dto.TotalItemCount
        );
    }
}

