using AngleSharp;
using CloneExtensions;
using Origami.Core.Models;
using Origami.Core.Models.Settings;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Origami.Core
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Represents a collection of file extensions commonly associated with image formats.
        /// </summary>
        /// <remarks>The collection is case-insensitive, allowing comparisons to be performed without
        /// regard to letter casing. Supported extensions include: .jpg, .jpeg, .png, .gif, .bmp, .tiff, .webp, and
        /// .tga.</remarks>
        private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".tga"
        };

        /// <summary>
        /// Hex Digits
        /// </summary>
        private static char[] _hexDigits = {
            '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        private static Regex iframeRegex = new(@"^<iframe\b[^>]*>(.*?)</iframe>$", RegexOptions.Singleline);

        public static IList<OrigamiSetting> Add(this IList<OrigamiSetting> settings, SocialNetwork socialNetwork)
        {
            var prefix = "socialnetwork";

            settings.Add(new() { Name = $"{prefix}-facebook-enabled", Value = socialNetwork.Facebook.Enabled.ToString(), });
            settings.Add(new() { Name = $"{prefix}-facebook-appid", Value = socialNetwork.Facebook.AppId.ToString(), });
            settings.Add(new() { Name = $"{prefix}-facebook-appsecret", Value = socialNetwork.Facebook.AppSecret.ToString(), });

            settings.Add(new() { Name = $"{prefix}-google-enabled", Value = socialNetwork.Google.Enabled.ToString(), });
            settings.Add(new() { Name = $"{prefix}-google-apikey", Value = socialNetwork.Google.ApiKey.ToString(), });
            settings.Add(new() { Name = $"{prefix}-google-clientid", Value = socialNetwork.Google.ClientId.ToString(), });
            settings.Add(new() { Name = $"{prefix}-google-clientsecret", Value = socialNetwork.Google.ClientSecret.ToString(), });

            settings.Add(new() { Name = $"{prefix}-github-enabled", Value = socialNetwork.GitHub.Enabled.ToString(), });
            settings.Add(new() { Name = $"{prefix}-github-appname", Value = socialNetwork.GitHub.AppName.ToString(), });
            settings.Add(new() { Name = $"{prefix}-github-clientid", Value = socialNetwork.GitHub.ClientId.ToString(), });
            settings.Add(new() { Name = $"{prefix}-github-clientsecret", Value = socialNetwork.GitHub.ClientSecret.ToString(), });

            settings.Add(new() { Name = $"{prefix}-microsoft-enabled", Value = socialNetwork.Microsoft.Enabled.ToString(), });
            settings.Add(new() { Name = $"{prefix}-microsoft-tenantid", Value = socialNetwork.Microsoft.TenantId.ToString(), });
            settings.Add(new() { Name = $"{prefix}-microsoft-clientid", Value = socialNetwork.Microsoft.ClientId.ToString(), });
            settings.Add(new() { Name = $"{prefix}-microsoft-clientsecret", Value = socialNetwork.Microsoft.ClientSecret.ToString(), });

            return settings;
        }

        public static IList<OrigamiSetting> Add(this IList<OrigamiSetting> settings, OpenTelemetry socialNetwork)
        {
            var prefix = "opentelemetry";

            settings.Add(new() { Name = $"{prefix}-enabled", Value = socialNetwork.Enabled.ToString(), });
            settings.Add(new() { Name = $"{prefix}-endpoint", Value = socialNetwork.Endpoint });

            return settings;
        }

        /// <summary>
        /// Age in years
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static int Age(this DateTime birthday)
        {
            var zeroTime = new DateTime(1, 1, 1);
            var span = DateTime.Now - birthday;
            // because we start at year 1 for the Gregorian 
            // calendar, we must subtract a year here.
            return (zeroTime + span).Year - 1;
        }

        /// <summary>
        /// Converts the <paramref name="base64image"/> into a byte array
        /// </summary>
        /// <param name="base64image"></param>
        /// <returns></returns>
        public static byte[] Base64ImageToBytes(this string base64image)
        {
            if (base64image.Has() == true)
            {
                var split = base64image.Split(',');
                if (split.Length > 1)
                {
                    var s = split[1].SolvePotentialIssuesWithBase64Image();
                    return Convert.FromBase64String(s);
                }
            }
            return [];
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="blog"></param>
        /// <returns></returns>
        public static IEnumerable<T> Blog<T>(this IEnumerable<T> entities, Guid blog)
            where T : IBlogId
        {
            return entities.Where(x => x.BlogId == blog);
        }

        public static Result<T2> Call<T1, T2>(this IEnumerable<T1> entities, Func<T1, bool, Result<T2>> function, bool checkPermission)
        {
            var result = new Result<T2>();

            //bugfix: this is necessary
            entities = entities.ToList();

            foreach (var entity in entities)
            {
                function.Invoke(entity, checkPermission).Push(result);
                if (result.Ok == false) return result;
            }

            return result;
        }

        /// <summary>
        /// Clones the <paramref name="entity"/>, returning a brand new instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T Clone<T>(this T? entity)
            where T : class, new()
        {
            //TODO: workaround for OrigamiBackupRestore, find a better way to do this
            if (entity is OrigamiBackupRestore restore)
            {
                return restore.GetClone() as T ?? new T();
            }
            return entity != null ? entity.GetClone() : new();
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static Guid? Decrypt(this string cipherText, string key, string iv)
        {
            try
            {
                using var aes = Aes.Create();

                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                var json = sr.ReadToEnd();

                return JsonSerializer.Deserialize<Guid>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public static T Deleted<T>(this T entity, T delete)
        {
            var del1 = entity as IDeleted;
            var del2 = delete as IDeleted;

            if (del1 != null) del1.IsDeleted = del2!.IsDeleted;

            return entity;
        }

        /// <summary>
        /// generic method using a class instance to perform deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T? Deserialize<T>(this string xml)
            where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(xml) == false)
            {
                //string reader
                using (var stringReader = new StringReader(xml))
                {
                    var serial = new XmlSerializer(typeof(T));
                    return serial.Deserialize(stringReader) as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T1}"/> and returns the original
        /// sequence.
        /// </summary>
        /// <remarks>This method forces immediate execution of the sequence by converting it to a list
        /// before applying the action.</remarks>
        /// <typeparam name="T1">The type of the elements in the sequence.</typeparam>
        /// <param name="entities">The sequence of elements on which the action is performed. Cannot be null.</param>
        /// <param name="action">The action to perform on each element of the sequence. Cannot be null.</param>
        /// <returns>The original sequence of elements after the action has been applied to each element.</returns>
        public static IEnumerable<T1> Each<T1>(this IEnumerable<T1> entities, Action<T1> action)
        {
            //bugfix: this is necessary
            entities = entities.ToList();

            foreach (var entity in entities)
            {
                action.Invoke(entity);
            }

            return entities;
        }

        /// <summary>
        /// Executes a specified method on each element of the collection and returns the original collection.
        /// </summary>
        /// <remarks>The method is executed on each element of the collection, and the original collection
        /// is returned. The collection is enumerated once and converted to a list to ensure stability during
        /// iteration.</remarks>
        /// <typeparam name="T1">The type of elements in the collection.</typeparam>
        /// <typeparam name="T2">The return type of the method to be executed on each element.</typeparam>
        /// <param name="entities">The collection of elements on which the method will be executed. Cannot be null.</param>
        /// <param name="method">A function to execute on each element of the collection. Cannot be null.</param>
        /// <returns>The original collection of elements after the method has been executed on each element.</returns>
        public static IEnumerable<T1> Each<T1, T2>(this IEnumerable<T1> entities, Func<T1, T2> method)
        {
            //bugfix: this is necessary
            entities = entities.ToList();

            foreach (var entity in entities)
            {
                method.Invoke(entity);
            }

            return entities;
        }

        /// <summary>
        /// Is the string email an email?
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool Email(this string? email)
        {
            if (email.Has() == true)
            {
                try
                {
                    MailAddress mailAddress = new MailAddress(email!);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Encrypt(this string plainText, string key, string iv)
        {
            using var aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(plainText);
            sw.Flush();
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Extracts 5 errors and put it in a single error message
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static string Error(this IEnumerable<ResultMessage> messages)
        {
            var errors = messages
                .Where(x => x.MessageType == ResultMessage.MessageTypes.Error)
                .Take(5);

            if (errors.Count() > 0)
            {
                var errorBuilder = new StringBuilder();

                foreach (var error in errors)
                {
                    errorBuilder.AppendFormat(" • {0}", error.Message);
                }

                return errorBuilder.ToString()
                    .TrimStart()
                    .TrimStart('•')
                    .TrimStart();
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts the <paramref name="entity"/> back into an XML in string form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string From<T>(this string? additionalInfo, Action<T> action)
            where T : AdditionalInfo
        {
            //converts the additional info into entity
            var entity = additionalInfo.To<T>();

            //calls the action to modify the entity
            action.Invoke(entity);

            // Create an XmlSerializer instance for the class type
            var serializer = new XmlSerializer(typeof(T));

            // Create a StringWriter to hold the XML output
            var stringWriter = new StringWriter();

            // Serialize the object to XML
            serializer.Serialize(stringWriter, entity);

            // Get the serialized XML as a string
            string xml = stringWriter.ToString();

            //returns the XML in string form
            return xml;
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string FullName<T>(this T entity)
            where T : IFirstName, ILastName
        {
            List<string> names = [];

            if (entity.FirstName.Has() == true) names.Add(entity.FirstName);
            if (entity.LastName.Has() == true) names.Add(entity.LastName);

            if (names.Count == 2)
            {
                if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith("en") == true)
                {
                    return $"{names[1]}, {names[0]}";
                }

                return $"{names[0]} {names[1]}";
            }

            return names.Count == 1 ? names[0] : string.Empty;
        }

        public static DataOperationContext<T> GetContext<T>(this T entity)
        {
            return new(entity is OrigamiUser user ? user : OrigamiUser.AnonymousUser, entity);
        }

        public static DataOperationContext<T> GetContext<T>(this T entity, OrigamiUser user)
        {
            return new(user, entity);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static DateTime GetDate<T>(this T entity)
            where T : IDateCreated, IDateModified
        {
            return entity.DateModified != null ? entity.DateModified.Value : entity.DateCreated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetHexString(this byte[] bytes)
        {
            int j = bytes.Length;

            char[] chars = new char[j * 2];

            for (int i = 0; i < j; i++)
            {
                int b = bytes[i];
                chars[i * 2] = _hexDigits[b >> 4];
                chars[i * 2 + 1] = _hexDigits[b & 0xF];
            }

            return new string(chars);
        }

        public static string GetHyperlink(this OrigamiBlog blog, OrigamiTag tag, INanoId? entity = null)
        {
            return $"/blogs/{blog.Slug}/tags/{tag.Slug}/{entity?.NanoId}";
        }

        public static string GetHyperlink(this OrigamiBlog blog, OrigamiCategory category, INanoId? entity = null)
        {
            return $"/blogs/{blog.Slug}/categories/{category.Slug}/{entity?.NanoId}";
        }

        /// <summary>
        /// Extracts the exception message, traversing diving into the inner exceptions.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetMessage(this Exception? exception)
        {
            return exception.M()[2..].TrimStart();
        }

        /// <summary>
        /// Gets the plural from a <paramref name="type"/>'s name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPlural(this Type type)
        {
            var name = type.Name;
            if (name.StartsWith("Origami") == true)
            {
                name = name[7..];
            }
            if (name.EndsWith("y") == true)
            {
                name = name.TrimEnd('y') + "ies";
                return name;
            }
            switch (name)
            {
                case "Settings": return "Settings";
                default: return $"{name}s";
            }
        }

        /// <summary>
        /// Retrieves a social profile by its identifier and ensures it is not blocked.
        /// </summary>
        /// <param name="entities">The collection of <see cref="OrigamiSocialProfile"/> objects to search.</param>
        /// <param name="id">The unique identifier of the social profile to retrieve.</param>
        /// <returns>The <see cref="OrigamiSocialProfile"/> with the specified identifier if it exists and is not blocked.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the social profile with the specified <paramref name="id"/> does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown if the social profile with the specified <paramref name="id"/> is blocked.</exception>
        public static OrigamiSocialProfile GetProfileThrowIfBlocked(this IEnumerable<OrigamiSocialProfile> entities, Guid id)
        {
            var profile = entities.Id(id);
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile), "Social profile cannot be null");
            }
            if (profile.IsBlocked)
            {
                throw new ArgumentException("Social profile is blocked", nameof(profile));
            }
            return profile;
        }

        /// <summary>
        /// Based on a given <paramref name="timePeriod"/>, returns the appropriate DateTime range.
        /// </summary>
        /// <param name="timePeriod"></param>
        /// <returns></returns>
        public static (DateTime Start, DateTime End) GetRange(this TimePeriod timePeriod)
        {
            if (timePeriod == TimePeriod.Last24Hours) return (DateTime.UtcNow.AddHours(-24), DateTime.MaxValue);
            if (timePeriod == TimePeriod.Last7Days) return (DateTime.UtcNow.AddDays(-7), DateTime.MaxValue);
            if (timePeriod == TimePeriod.Last30Days) return (DateTime.UtcNow.AddDays(-30), DateTime.MaxValue);
            if (timePeriod == TimePeriod.Last90Days) return (DateTime.UtcNow.AddDays(-90), DateTime.MaxValue);
            if (timePeriod == TimePeriod.Last180Days) return (DateTime.UtcNow.AddDays(-180), DateTime.MaxValue);
            if (timePeriod == TimePeriod.Last365Days) return (DateTime.UtcNow.AddDays(-365), DateTime.MaxValue);

            if (timePeriod == TimePeriod.CurrentMonth)
            {
                var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0);
                var end = start.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                return (start, end);
            }

            if (timePeriod == TimePeriod.CurrentYear)
            {
                var start = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0);
                var end = start.AddYears(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                return (start, end);
            }

            return (new DateTime(1753, 1, 1), DateTime.MaxValue);
        }

        public static string GetSlug(this string text)
        {
            if (text.Has() == false) return string.Empty;

            // Normalize text to remove diacritics (e.g., accents)
            text = text.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            text = sb.ToString().Normalize(NormalizationForm.FormC);

            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Replace invalid characters with hyphens
            text = Regex.Replace(text, @"[^a-z0-9\s-]", string.Empty);

            // Replace multiple spaces or hyphens with a single hyphen
            text = Regex.Replace(text, @"[\s-]+", "-").Trim('-');

            return text;
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetText(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue) return string.Empty;
            return dateTime.ToString();
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetText(this DateTime? dateTime)
        {
            if (dateTime != null && dateTime.HasValue)
            {
                if (dateTime.Value == DateTime.MinValue) return string.Empty;
                return dateTime.Value.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Extension for string.IsNullOrWhitespace.
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static bool Has([NotNullWhen(true)] this string? @string)
        {
            return string.IsNullOrWhiteSpace(@string) == false;
        }

        public static bool Has<T>(this T? entity)
            where T : class, INew
        {
            if (entity == null) return false;
            if (entity.New) return false;
            return true;
        }

        public static string HexString(this byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", string.Empty).TrimStart('0');
        }

        /// <summary>
        /// Retrieves the first entity from the collection that matches the specified identifier.
        /// </summary>
        /// <typeparam name="T">The type of the entities in the collection, which must implement <see cref="IId"/>.</typeparam>
        /// <param name="entities">The collection of entities to search.</param>
        /// <param name="id">The unique identifier to match. If <see langword="null"/>, the method returns <see langword="null"/>.</param>
        /// <returns>The first entity in the collection with a matching identifier, or <see langword="null"/> if no match is
        /// found or if <paramref name="id"/> is <see langword="null"/>.</returns>
        public static T? Id<T>(this IEnumerable<T> entities, Guid? id)
            where T : class, IId, new()
        {
            if (id == null) return null;
            return entities.FirstOrDefault(x => x.Id == id.Value);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Implements<T>(this Type type)
        {
            if (type.FullName == typeof(T).FullName) return true;
            if (type.GetInterfaces().Any(x => x.FullName == typeof(T).FullName)) return true;
            return false;
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsDeleted<T>(this T? entity)
        {
            if (entity == null) return false;
            if (entity is IDeleted == false) return false;
            if (entity is IDeleted del && del.IsDeleted) return true;
            return false;
        }

        public static bool IsIFrame(this string? html)
        {
            if (html.Has() == true)
            {
                if (iframeRegex.IsMatch(html) == false) return false;

                var context = BrowsingContext.New(Configuration.Default);
                var doc = context.OpenAsync(req => req.Content(html)).Result;

                var iframe = doc.QuerySelector("iframe");
                if (iframe == null) return false;
                if (iframe.Attributes["src"] == null) return false;

                var src = iframe.Attributes["src"]!.Value;
                if (Uri.TryCreate(src, UriKind.Absolute, out var uri) == false || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public static bool IsImage(this string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension) && ImageExtensions.Contains(extension);
        }

        /// <summary>
        /// Analyzes the password returning a result
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Result IsPasswordStrong(this string password)
        {
            var result = new Result();

            if (password.Has() == false)
            {
                result.ErrorMessage = "Password is empty";
            }
            else
            {
                if (password.Length < 5) result.ErrorMessage = "Password too short";
                if (Regex.IsMatch(password, "[0-9]+") == false) result.ErrorMessage = "Number was not found in password";
                if (Regex.IsMatch(password, "[a-zA-Z]+") == false) result.ErrorMessage = "Character was not found in password";
                if (Regex.IsMatch(password, @"[!@#$%^&*()_\-+=\[\]{}|\\:;\""<>,.?/~`]") == false) result.ErrorMessage = "Special character was not found in password";
            }

            return result.Ok ? new() { SuccessMessage = "Password is strong" } : result;
        }

        /// <summary>
        /// Generates a cache key for the specified type.
        /// </summary>
        /// <param name="type">The type for which to generate the cache key. Cannot be <see langword="null"/>.</param>
        /// <returns>A string representing the cache key, formatted as "entities-{type.FullName}".</returns>
        public static string KeyForCaching(this Type type)
        {
            return $"entities-{type.FullName}";
        }

        /// <summary>
        /// Generates a unique cache key for storing or retrieving comment counts associated with the specified entity.
        /// </summary>
        /// <param name="parent">The entity for which the cache key is generated. Must implement <see cref="IId"/>.</param>
        /// <returns>A string representing the cache key, formatted to include the entity's type and ID.</returns>
        public static string KeyForCachingComments(this IId parent)
        {
            return $"entities-comments-count-{parent.GetType().FullName}[{parent.Id}]";
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string KeyForCachingViews(this IId parent)
        {
            return $"entities-views-count-{parent.GetType().FullName}[{parent.Id}]";
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Language(this string? value)
        {
            if (value.Has() == true)
            {
                var language1 = OrigamiConstants.ContentLanguages().FirstOrDefault(x => x.Language == value).Language;
                if (language1.Has() == true) return language1;

                var split = value.Split('-')[0];
                var language2 = OrigamiConstants.ContentLanguages().FirstOrDefault(x => x.Language == split).Language;
                if (language2.Has() == true) return language2;
            }
            return OrigamiConstants.ContentLanguages().First().Name;
        }

        /// <summary>
        /// string.Equals(StringComparison.CurrentCultureIgnoreCase)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool Like(this string? a, string? b, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return string.Equals(a, b, comparison);
        }

        /// <summary>
        /// Name or FirstName and LastName
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <returns></returns>
        public static string Name(this OrigamiSocialProfile? socialProfile)
        {
            if (socialProfile == null) return string.Empty;
            if (socialProfile.Name.Has() == true) return $"{socialProfile.Name}";
            return $"{socialProfile.FirstName} {socialProfile.LastName}";
        }

        /// <summary>
        /// Retrieves the first entity from the collection that matches the specified Nano ID.
        /// </summary>
        /// <typeparam name="T">The type of the entities in the collection. Must implement <see cref="IId"/> and <see cref="INanoId"/>.</typeparam>
        /// <param name="entities">The collection of entities to search.</param>
        /// <param name="nanoId">The Nano ID to match. If <see langword="null"/>, the method returns <see langword="null"/>.</param>
        /// <returns>The first entity in the collection with a matching Nano ID, or <see langword="null"/> if no match is found
        /// or if <paramref name="nanoId"/> is <see langword="null"/>.</returns>
        public static T? NanoId<T>(this IEnumerable<T> entities, string? nanoId)
            where T : class, INanoId
        {
            if (nanoId == null) return null;
            return entities.FirstOrDefault(x => x.NanoId == nanoId);
        }

        /// <summary>
        /// Returns no image, if necessary
        /// </summary>
        /// <param name="source"></param>
        /// <param name="noImage"></param>
        /// <returns></returns>
        public static string NoHeader(this string? source, string noImage = OrigamiConstants.NoHeader)
        {
            return source.Has() ? source : noImage;
        }

        /// <summary>
        /// Filters the <paramref name="entities"/>, retrieving only non-deleted <paramref name="entities"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static IEnumerable<T> NonDeleted<T>(this IEnumerable<T> entities)
        {
            if (typeof(T).Implements<IDeleted>() == true)
            {
                return entities.Cast<IDeleted>().Where(entity => entity.IsDeleted == false).Cast<T>();
            }
            return entities;
        }

        /// <summary>
        /// Are all results ok?
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static bool Ok(this IEnumerable<Result> results)
        {
            return results.All(x => x.Ok);
        }

        /// <summary>
        /// Profile picture (or no-icon.png)
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <returns></returns>
        public static string ProfilePicture(this OrigamiSocialProfile? socialProfile)
        {
            const string noIcon = OrigamiConstants.NoUser;
            if (socialProfile == null) return noIcon;
            if (socialProfile.ProfilePictureUrl.Has() == true) return socialProfile.ProfilePictureUrl;
            if (socialProfile.ProfilePicture.Has() == true) return socialProfile.ProfilePicture;
            return noIcon;
        }



        /// <summary>
        /// Copies all the information <paramref name="entity"/> <paramref name="from"/>
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="entity"></param>
        /// <param name="from"></param>
        public static T1 Pull<T1, T2>(this T1 entity, T2? from)
            where T1 : class, T2
            where T2 : class, new()
        {
            if (from != null) from.Push(entity);
            return entity;
        }

        /// <summary>
        /// Copies all the information <paramref name="from"/> <paramref name="to"/>
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Push<T1, T2>(this T1 from, T2? to)
            where T1 : class, new()
        {
            if (from != null && to != null)
            {
                var clone = from.Clone();

                foreach (var field in from.GetType().GetRuntimeFields())
                {
                    if (field.IsInitOnly) continue;

                    try
                    {
                        var value = field.GetValue(clone);
                        if (value == null || value.GetType().IsPrimitive)
                        {
                            field.SetValue(to, value);
                            continue;
                        }

                        field.SetValue(to, value);
                    }
                    catch { }
                }

                foreach (var property in from.GetType().GetRuntimeProperties())
                {
                    if (property.CanRead == false) continue;
                    if (property.CanWrite == false) continue;

                    try
                    {
                        var value = property.GetValue(clone);
                        if (value == null || value.GetType().IsPrimitive)
                        {
                            property.SetValue(to, value);
                            continue;
                        }

                        property.SetValue(to, value);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Adds a query string to <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string QueryString(this string? url, string key, string value)
        {
            if (url.Has() == true)
            {
                if (url.Contains($"?{key}=", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    var split1 = url.Split('?', StringSplitOptions.RemoveEmptyEntries);
                    var split2 = split1[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
                    var queryString = string.Join('&', split2.Skip(1));
                    return $"{split1[0]}?{key}={Uri.EscapeDataString(value)}&{queryString}";
                }

                if (url.Contains($"&{key}=", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    var split1 = url.Split('?', StringSplitOptions.RemoveEmptyEntries);
                    var split2 = split1[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
                    var list = new List<string>();

                    foreach (var keyValue in split2)
                    {
                        if (keyValue.StartsWith(key) == true) continue;
                        list.Add(keyValue);
                    }

                    list.Add($"{key}={Uri.EscapeDataString(value)}");

                    var queryString = string.Join('&', split2);
                    return $"{split1[0]}?{queryString}";
                }

                url += url.Contains('?') ? "&" : "?";
                url += $"{key}={Uri.EscapeDataString(value)}";
                return url;
            }

            return string.Empty;
        }

        /// <summary>
        /// Adds a query string to <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string QueryString(this string? url, string key)
        {
            if (url.Has() == true)
            {
                var regex = new Regex($"[?&]{Regex.Escape(key)}=([^&]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var match = regex.Match(url);
                if (match.Success == true)
                {
                    return Uri.UnescapeDataString(match.Groups[1].Value);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the reference's value to the parameter and invokes the eventHandler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <param name="eventHandler"></param>
        public static void Set<T>(this object entity, ref T reference, T value, EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            //does nothing if the reference and the value is null
            if (reference == null && value == null) return;

            var args = new PropertyChangedEventArgs(string.Empty);
            var stackTrace = new StackTrace();

            //gets the property name that is calling this function
            var propertyName = stackTrace.GetFrame(1)?.GetMethod()?.Name;
            if (propertyName.Has() == true && propertyName!.StartsWith("set_") == true)
            {
                propertyName = propertyName[4..];
                args = new PropertyChangedEventArgs(propertyName);
            }

            if (value == null)
            {
                var type = typeof(T);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    reference = value;
                    eventHandler?.Invoke(entity, args);
                }
                else
                {
                    try
                    {
                        reference = type.Name.Like("string")
                            ? (T)(object)string.Empty
                            : value;

                        eventHandler?.Invoke(entity, args);
                    }
                    catch (MissingMethodException)
                    {
                        reference = value;
                        eventHandler?.Invoke(entity, args);
                    }
                }

                return;
            }

            //value is not null
            if (reference == null || reference.Equals(value) == false)
            {
                reference = value;
                eventHandler?.Invoke(entity, args);
            }

            //if value is an observable collection, attaches itself to the collection changed event
            if (value is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is IChanged and needs to be hooked up
            if (value is IChanged changed)
            {
                changed.Changed += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is ICommentChanged and needs to be hooked up
            if (value is ICommentChanged commentChanged)
            {
                commentChanged.CommentChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is IContentChanged and needs to be hooked up
            if (value is IContentChanged contentChanged)
            {
                contentChanged.ContentChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is IRatingChanged and needs to be hooked up
            if (value is IRatingChanged ratingChanged)
            {
                ratingChanged.RatingChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is ISettingChanged and needs to be hooked up
            if (value is ISettingChanged settingChanged)
            {
                settingChanged.SettingChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is IViewChanged and needs to be hooked up
            if (value is IViewChanged viewChanged)
            {
                viewChanged.ViewChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }

            //value is IReactionChanged and needs to be hooked up
            if (value is IReactionChanged reactionChanged)
            {
                reactionChanged.ReactionChanged += (sender, e) => eventHandler?.Invoke(entity, args);
            }
        }

        public static T? SetAuthor<T>(this T? entity, OrigamiUser? author)
        {
            if (author == null) return entity;
            if (entity is IAuthorId fKAuthor && fKAuthor.AuthorId == Guid.Empty)
            {
                fKAuthor.AuthorId = author.Id;
            }
            return entity;
        }

        public static T? SetBlog<T>(this T? entity, OrigamiBlog? blog)
        {
            if (blog == null) return entity;
            if (entity is IBlogId fkBlog && fkBlog.BlogId == Guid.Empty)
            {
                fkBlog.BlogId = blog.Id;
            }
            return entity;
        }

        public static T? SetDateCreated<T>(this T? entity, DateTime dateTime)
        {
            if (entity is IDateCreated dateCreated)
            {
                dateCreated.DateCreated = dateTime;
            }
            return entity;
        }

        public static T? SetDateModified<T>(this T? entity, DateTime dateTime)
        {
            if (entity is IDateModified dateModified)
            {
                dateModified.DateModified = dateTime;
            }
            return entity;
        }

        /// <summary>
        /// Sets the Id, when <paramref name="entity"/> is <see cref="IId"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T? SetId<T>(this T? entity)
        {
            if (entity is IId id && id.Id == Guid.Empty)
            {
                id.Id = Guid.NewGuid();
            }
            return entity;
        }

        /// <summary>
        /// Generates a SHA256 string from <paramref name="rawData"/>
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string SHA256Hash(this string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string SizeInBytes(this long size)
        {
            long tb = (long)1024 * 1024 * 1024 * 1024;
            long gb = (long)1024 * 1024 * 1024;
            long mb = (long)1024 * 1024;
            long kb = 1024;

            if (size / tb >= 1) return $"{Math.Round((double)size / tb, 2)} TB";
            if (size / gb >= 1) return $"{Math.Round((double)size / gb, 2)} GB";
            if (size / mb >= 1) return $"{Math.Round((double)size / mb, 2)} MB";
            if (size / kb >= 1) return $"{Math.Round((double)size / kb, 2)} KB";

            return $"{size} B";
        }

        /// <summary>
        /// Retrieves the first entity from the collection that matches the specified slug.
        /// </summary>
        /// <typeparam name="T">The type of the entities in the collection. Must implement the <see cref="ISlug"/> interface.</typeparam>
        /// <param name="entities">The collection of entities to search. Cannot be <see langword="null"/>.</param>
        /// <param name="slug">The slug to match against the entities. If <see langword="null"/>, the method returns <see
        /// langword="null"/>.</param>
        /// <returns>The first entity in the collection with a matching slug, or <see langword="null"/> if no match is found or
        /// if <paramref name="slug"/> is <see langword="null"/>.</returns>
        public static T? Slug<T>(this IEnumerable<T> entities, string? slug)
            where T : class, ISlug
        {
            if (slug == null) return null;
            return entities.FirstOrDefault(x => x.Slug == slug);
        }

        public static string SolvePotentialIssuesWithBase64Image(this string base64Image)
        {
            base64Image = base64Image.Replace('-', '+').Replace('_', '/');
            int mod4 = base64Image.Length % 4;
            if (mod4 > 0)
            {
                base64Image += new string('=', 4 - mod4);
            }
            return base64Image;
        }

        /// <summary>
        /// Parses AdditionalInfo Xml string to the typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ainfo"></param>
        /// <returns></returns>
        public static T To<T>(this string? ainfo) where T : AdditionalInfo
        {
            if (string.IsNullOrWhiteSpace(ainfo) == false)
            {
                var t = typeof(T);
                var serializer = new XmlSerializer(t);
                var treader = new StringReader(ainfo);
                var xreader = XmlReader.Create(treader);

                try
                {
                    if (serializer.CanDeserialize(xreader) == true)
                    {
                        var @return = serializer.Deserialize(xreader);
                        if (@return != null) return (T)@return;
                    }
                }
                finally
                {
                    treader.Close();
                    xreader.Close();
                    xreader.Dispose();
                    treader.Dispose();
                }
            }

            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Converts a file path to a byte array for handler processing
        /// </summary>
        /// <param name="filePath">the file path to process</param>
        /// <returns>a new binary array</returns>
        public static byte[] ToBytes(this string filePath)
        {
            byte[] buffer = [];

            try
            {
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var reader = new BinaryReader(stream);
                var bytes = new FileInfo(filePath).Length;
                buffer = reader.ReadBytes((int)bytes);
                stream.Close();
                stream.Dispose();
                reader.Close();
            }
            catch
            {
                //TODO:
                //Utils.Log("File Provider FileToByArray", ex);
            }

            return buffer;
        }
        /// <summary>
        /// Unescapes the string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Unescape(this string? value)
        {
            return Uri.UnescapeDataString(value ?? string.Empty);
        }
        public static T Version<T>(this T entity, T version)
        {
            var version1 = entity as IVersion;
            var version2 = version as IVersion;

            if (version1 != null) version1.Version = version2!.Version;

            return entity;
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string YesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string YesNo(this bool? value)
        {
            if (value != null) return value.GetValueOrDefault().YesNo();
            return "Empty (Null)";
        }

        /// <summary>
        /// Extracts the message.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static string M(this Exception? exception, int depth = 0)
        {
            if (depth >= 8) return string.Empty;
            if (exception == null) return string.Empty;
            return string.Concat(" • ", exception.Message, exception.InnerException.M(depth + 1));
        }
    }
}
