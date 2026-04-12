using FluentValidation;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Origami.Core.Models
{
    /// <summary>
    /// Simple class for Results with Status and a Message.
    /// </summary>
    public class Result : IChanged, IId, IDateCreated
    {
        protected DateTime _dateCreated = DateTime.UtcNow;
        /// <summary>
        /// Field for Messages property
        /// </summary>
        protected ObservableCollection<ResultMessage> _messages;

        protected int _rowsAffected;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Result() : base()
        {
            Id = Guid.NewGuid();
            _messages = new();
            _messages.CollectionChanged += (sender, e) =>
            {
                Changed?.Invoke(this, new PropertyChangedEventArgs(nameof(Messages)));
            };
        }

        /// <summary>
        /// Constructor supporting an exception as the error message
        /// </summary>
        /// <param name="ex"></param>
        public Result(Exception ex) : this()
        {
            Error = ex.GetMessage();
        }

        /// <summary>
        /// Event for when something changes in the instance
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        /// <summary>
        /// Creates an error message
        /// </summary>
        public virtual string? Error
        {
            set => AddMessage(ResultMessage.MessageTypes.Error, value);
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Creates an info message
        /// </summary>
        public virtual string? Info
        {
            set => AddMessage(ResultMessage.MessageTypes.Info, value);
        }

        /// <summary>
        /// Result Messages
        /// </summary>
        public ObservableCollection<ResultMessage> Messages
        {
            get => _messages;
            set
            {
                this.Set(ref _messages, value, Changed);
            }
        }

        /// <summary>
        /// Status of the Result: true for OK, false for not OK.
        /// </summary>
        public bool Ok
        {
            get
            {
                if (_messages?.Any(x => x.MessageType == ResultMessage.MessageTypes.Error) == true) return false;
                return true;
            }
        }

        /// <summary>
        /// Rows affected
        /// </summary>
        public int RowsAffected
        {
            get => _rowsAffected;
            set => this.Set(ref _rowsAffected, value, Changed);
        }

        /// <summary>
        /// Creates a simple message
        /// </summary>
        public virtual string? Simple
        {
            set => AddMessage(ResultMessage.MessageTypes.Simple, value);
        }

        /// <summary>
        /// Creates a success message
        /// </summary>
        public virtual string? Success
        {
            set => AddMessage(ResultMessage.MessageTypes.Success, value);
        }

        /// <summary>
        /// Creates a warning message
        /// </summary>
        public virtual string? Warning
        {
            set => AddMessage(ResultMessage.MessageTypes.Warning, value);
        }

        /// <summary>
        /// Gets the result's entity, if it exists (or if this instance is Result<>).
        /// </summary>
        /// <returns></returns>
        public virtual object? GetEntity()
        {
            var entity = GetType().GetRuntimeProperty(nameof(Result<IId>.Entity))?.GetValue(this);

            return entity switch
            {
                null => null,
                HubContentPage page => page.Entity,
                HubContentPost post => post.Entity,
                HubContentSpecialMessage specialMessage => specialMessage.Entity,
                HubContentSpecialPage specialPage => specialPage.Entity,
                HubContentVideo video => video.Entity,
                HubContentQuickNote quickNote => quickNote.Entity,
                _ => entity
            };
        }

        /// <summary>
        /// Executes the <paramref name="onFail"/> action in case of success.
        /// </summary>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public virtual Result OnFailure(Action onFail)
        {
            if (Ok == false) onFail();
            return this;
        }

        /// <summary>
        /// Executes the <paramref name="onSuccess"/> action in case of success.
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public virtual Result OnSuccess(Action onSuccess)
        {
            if (Ok == true) onSuccess();
            return this;
        }

        /// <summary>
        /// Pulls the messages <paramref name="from"/> to this instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public virtual Result Pull<T2>(Result<T2> from)
        {
            foreach (var message in from.Messages)
            {
                Messages.Add(message);
            }

            return this;
        }

        /// <summary>
        /// Pushes the messages <paramref name="to"/> from this instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to"></param>
        /// <returns></returns>
        public Result Push(Result to)
        {
            to.RowsAffected += RowsAffected;

            foreach (var message in Messages)
            {
                to.Messages.Add(message);
            }

            return to;
        }

        /// <summary>
        /// Pushes the messages <paramref name="to"/> from this instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to"></param>
        /// <returns></returns>
        public Result<T2> Push<T2>(Result<T2> to)
        {
            to.RowsAffected += RowsAffected;

            foreach (var message in Messages)
            {
                to.Messages.Add(message);
            }

            return to;
        }

        /// <summary>
        /// Creates a message, if it doesn't exist in the collection
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool AddMessage(ResultMessage.MessageTypes messageType, string? message)
        {
            if (message != null && message.Has() == true)
            {
                var found = _messages.FirstOrDefault(x =>
                    x.MessageType == messageType &&
                    x.Message.Like(message) == true);

                if (found == null)
                {
                    _messages.Add(new ResultMessage
                    {
                        Message = message,
                        MessageType = messageType
                    });

                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Simple class for Results with Status and a Message.
    /// </summary>
    public class Result<T> : Result
    {
        /// <summary>
        /// Field for the Entity property
        /// </summary>
        private T? _entity;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Result() : base()
        {

        }

        /// <summary>
        /// Default constructor with a parameter
        /// </summary>
        /// <param name="entity"></param>
        public Result(T entity) : this()
        {
            Entity = entity;
        }

        /// <summary>
        /// Constructor with parameters, especially the <paramref name="errorMessage"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="errorMessage"></param>
        public Result(T entity, string errorMessage) : this()
        {
            Entity = entity;
            Error = errorMessage;
        }

        /// <summary>
        /// Constructor with parameters, especially the <paramref name="validationResult"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="validationResult"></param>
        public Result(T entity, ValidationResult validationResult) : this()
        {
            Entity = entity;
            validationResult.Errors.Each(e => Error = e.ErrorMessage);
        }

        /// <summary>
        /// Constructor with parameters, especially the <paramref name="validationResult"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="validationResult"></param>
        public Result(T entity, IValidator<T> validator) : this()
        {
            Entity = entity;
            validator.Validate(entity).Errors.Each(e => Error = e.ErrorMessage);
        }

        /// <summary>
        /// Event for when something changes in the instance
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> EntityChanged = (sender, e) => { };

        /// <summary>
        /// Entity associated with the Result (if any)
        /// </summary>
        public T? Entity
        {
            get => _entity;
            set => this.Set(ref _entity, value, EntityChanged);
        }

        /// <summary>
        /// Executes the <paramref name="onFail"/> action in case of success.
        /// </summary>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public override Result<T> OnFailure(Action onFail)
        {
            if (Ok == false) onFail();
            return this;
        }

        /// <summary>
        /// Executes the <paramref name="onSuccess"/> action in case of success.
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public override Result<T> OnSuccess(Action onSuccess)
        {
            if (Ok == true) onSuccess();
            return this;
        }

        /// <summary>
        /// Pulls the messages <paramref name="from"/> to this instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public override Result<T> Pull<T2>(Result<T2> from)
        {
            RowsAffected += from.RowsAffected;

            foreach (var message in from.Messages)
            {
                Messages.Add(message);
            }

            return this;
        }
    }
}
