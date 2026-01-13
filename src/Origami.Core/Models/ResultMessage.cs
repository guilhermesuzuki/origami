using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Origami.Core.Models
{
    public class ResultMessage : IChanged
    {
        /// <summary>
        /// Message Types
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MessageTypes : byte
        {
            Simple = 0,
            Success,
            Error,
            Warning,
            Info,
        }

        private MessageTypes _messageType;
        private string _message = string.Empty;

        /// <summary>
        /// Event for when something in the instance changes
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Message Type
        /// </summary>
        public MessageTypes MessageType
        {
            get => _messageType;
            set => this.Set(ref _messageType, value, Changed);
        }

        /// <summary>
        /// Result Message
        /// </summary>
        public string Message
        {
            get => _message;
            set => this.Set(ref _message, value, Changed);
        }
    }
}
