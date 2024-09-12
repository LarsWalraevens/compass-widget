using System.Globalization;
using System.Text.RegularExpressions;

namespace Vvoids.Api.Services;

public static class Routes
{
    public const string DeleteUserBook = "v1/delete/user/book";
    public const string InsertUserBook = "v1/insert/user/book";
    public const string SelectUserBooks = "v1/select/user/books";
    public const string SelectGoogleBooks = "v1/select/google/books";
    public const string SelectBook = "v1/select/book";
    public const string ToggleUserBookComplete = "v1/toggle/user/book/complete";
    public const string UpdateUserBookRating = "v1/update/user/book/rating";
    public const string UpdateUserBookCharacters = "v1/update/user/book/characters";
    public const string UpdateUserBookNotes = "v1/update/user/book/notes";
    public const string DeleteUserBookSession = "v1/delete/user/book/session";
    public const string InsertUserBookSession = "v1/insert/user/book/session";
    public const string Login = "v1/login";
    public const string UpdateUserBookSession = "v1/update/user/book/session";
    public const string SelectUserBookSessions = "v1/select/user/book/sessions";
}

public static class Triggers
{

}

public static class Extensionss
{
    public static DateTime ExtractDateTimeFromFileName(this string fileName)
    {
        // Regex to extract potential date and time sections
        string pattern = @"(\d{2,4}-\d{2}-\d{2,4})_(\d{2}-\d{2}-\d{2})";
        Match match = Regex.Match(fileName, pattern);

        if (match.Success)
        {
            string[] dateFormats = { "dd-MM-yyyy HH:mm:ss", "MM-dd-yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss" };

            if (DateTime.TryParseExact($"{match.Groups[1].Value} {match.Groups[2].Value.Replace("-", ":")}", dateFormats, null, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
        }

        return DateTime.MinValue;
    }
}

/// <summary>
/// These tags should exist in your configuration like <see langword="local.settings.json"/>.<br/>
/// The <see langword="Z_"/> prefix only exists in purpose of keeping them grouped.
/// </summary>
public static class Tags
{
    public const string API_KEY = "Z_API_KEY";
    public const string API_DEVELOPMENT = "Z_API_DEVELOPMENT";
    public const string SQL_PASSWORD = "Z_SQL_PASSWORD";
    public const string AZURE_MAIL_KEY = "Z_AZURE_MAIL_KEY";
    public const string API_LOCALUTCADDON = "Z_API_LOCALUTCADDON";
    public const string GOOGLE_BOOKS_KEY = "Z_GOOGLE_BOOKS_KEY";
}