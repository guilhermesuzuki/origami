using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Origami.Core
{
    public class Text
    {
        public const string OperationCompletedSuccessfully = "Yay! Everything went smoothly";
        public const string SomethingWentWrongPleaseTryAgain = "Ah heck, something went wrong, try again";
        public const string YouDontHavePermissionForThisFeature = "You don't have permission for this feature";
        public const string YouMadeTooManyCommentsIn5Minutes = "Calm down, you made too many comments in 5 minutes";

        private readonly IStringLocalizer<Text> _localizer;

        public Text(IStringLocalizer<Text> localizer) => _localizer = localizer;

        /// <summary>
        /// Text Styles
        /// </summary>
        public enum Styles : byte
        {
            /// <summary>
            /// Returns the original text added to the Resources' file
            /// </summary>
            Original = 0,

            /// <summary>
            /// Returns the text in lowercase formaat from the Resources' file
            /// </summary>
            Lowercase = 1,

            /// <summary>
            /// Returns the text in uppercase formaat from the Resources' file
            /// </summary>
            Uppercase = 2,
        }

        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Get(string key, Styles style = Styles.Lowercase, params object[] arguments)
        {
            var localizedArguments = new object[arguments.Length];

            if (arguments.Length > 0)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    var argument = arguments[i];
                    var localizedArgument = _localizer[argument.ToString() ?? "Unknown key"];
                    localizedArguments[i] = localizedArgument;
                }
            }

            LocalizedString localizedString = arguments?.Length > 0 ? _localizer[key, localizedArguments] : _localizer[key];

            var result = string.Empty;

            switch (style)
            {
                case Styles.Lowercase:
                    result = localizedString.Value.ToLower();
                    break;
                case Styles.Uppercase:
                    result = localizedString.Value.ToUpper();
                    break;
                default:
                    result = localizedString.Value;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Lower(string key)
        {
            return Get(key, Styles.Lowercase);
        }

        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Lower(string key, params object[] args)
        {
            return Get(key, Styles.Lowercase, args);
        }

        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Original(string key)
        {
            return Get(key, Styles.Original);
        }

        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Original(string key, params object[] args)
        {
            return Get(key, Styles.Original, args);
        }

        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Upper(string key, params object[] args)
        {
            return Get(key, Styles.Uppercase, args);
        }
        /// <summary>
        /// Get a string resource with a given key.
        /// </summary>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(_localizer))]
        public string Upper(string key)
        {
            return Get(key, Styles.Uppercase);
        }
    }
}
