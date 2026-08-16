using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public class TheCreator : ITheCreator
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IBlogRepository _blogRepository;
        protected readonly IUserFacade _userFacade;

        public TheCreator(IAppFacade appFacade, IUserFacade userFacade, IBlogRepository blogRepository)
        {
            _appFacade = appFacade;
            _userFacade = userFacade;
            _blogRepository = blogRepository;
        }

        public T Create<T>() where T : class, new()
        {
            T entity = new();

            entity.SetAuthor(_userFacade.User);
            entity.SetBlog(_blogRepository.ReadFromCache().Id(_userFacade.BlogId) ?? new());
            entity.SetDateCreated(DateTime.UtcNow);

            if (entity is OrigamiUser user)
            {
                user.GenerateNewPasswordForNewUsers();
            }

            if (entity is OrigamiSpecialMessage specialMessage)
            {
                specialMessage.BlogId = null;
            }

            if (entity is OrigamiSpecialPage specialPage)
            {
                specialPage.BlogId = null;
            }

            return entity;
        }
    }
}
