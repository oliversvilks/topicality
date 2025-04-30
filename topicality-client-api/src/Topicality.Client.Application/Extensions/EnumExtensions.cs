using System.ComponentModel;

namespace Topicality.Client.Application.Extensions;

/// <summary>
/// Enum Extensions
/// </summary>

public static class EnumExtensions
{
    /// <summary>
    /// Iegūst Enum tipa vērtības lokalizēto nosaukumu.
    /// </summary>
    public static string GetDescription(this Enum enumVal)
    {
        //TODO pārbaudīt vai tiek izmantots šajā koroservisā
        if (enumVal == null)
        {
            return string.Empty;
        }

        Type type = enumVal.GetType();
        var fieldInfo = type.GetField(enumVal.ToString());
        if (fieldInfo != null)
        {
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
        }
        return enumVal.ToString();
    }
}
