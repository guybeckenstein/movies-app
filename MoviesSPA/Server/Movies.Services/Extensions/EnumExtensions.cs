using Newtonsoft.Json;

namespace Movies.Services.Extensions;
public static class EnumExtensions
{
    public static string ToEnumString<T>(T value)
    {
        return JsonConvert.SerializeObject(value).Replace("\"", "");
    }

    public static T ToEnum<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>($"\"{value}\"");
    }
}
