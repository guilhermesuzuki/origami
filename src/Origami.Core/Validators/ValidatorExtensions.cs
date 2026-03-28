using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Origami.Core.Validators
{
    internal static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, Guid> AuthorId<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("Author is required"));
        }

        public static IRuleBuilderOptions<T, Guid?> BlogId<T>(this IRuleBuilder<T, Guid?> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotNull()
                .NotEmpty()
                .WithMessage(text.Original("Blog is required"));
        }

        public static IRuleBuilderOptions<T, List<OrigamiContentCategory>> CategoriesMustBeUnique<T>(this IRuleBuilder<T, List<OrigamiContentCategory>> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(categories =>
                {
                    if (categories.DistinctBy(x => x.CategoryId).Count() != categories.Count)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(text.Original("Categories must be unique"));
        }

        public static IRuleBuilderOptions<T, Guid> Category<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(categoryId =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return db.Set<OrigamiCategory>().AsNoTracking().Id(categoryId) != null;
                })
                .WithMessage(text.Original("Category must exist"));
        }

        public static IRuleBuilderOptions<T, Guid> Content<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(contentId =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return db.Set<OrigamiContent>().AsNoTracking().Id(contentId) != null;
                })
                .WithMessage(text.Original("Content must be valid"));
        }

        public static IRuleBuilderOptions<T, string> ContentType<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .Must(type =>
                {
                    if (type.Has() == false) return false;

                    var types = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => typeof(OrigamiContent).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

                    return types.Any(t => t.Name == type);
                }).WithMessage("Content type is required")
                ;
        }

        public static IRuleBuilderOptions<T, string?> Description<T>(this IRuleBuilder<T, string?> ruleBuilder, Text text)
        {
            return ruleBuilder
                .MaximumLength(1024)
                .WithMessage(x => text.Original("Description cannot exceed 1024 characters"));
        }

        public static IRuleBuilderOptions<T, string?> DisplayName<T>(this IRuleBuilder<T, string?> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotNull().WithMessage(text.Original("Display name is required"))
                .NotEmpty().WithMessage(text.Original("Display name is required"))
                .MaximumLength(200).WithMessage(text.Original("Display name cannot exceed 200 characters"));
        }
        public static IRuleBuilderOptions<T, T> DisplayNameMustBeDifferentThanUsername<T>(this IRuleBuilder<T, T> ruleBuilder, Text text)
            where T : IUsername, IDisplayName
        {
            return ruleBuilder
                .Must(x => x.DisplayName.Like(x.Username) == false)
                .WithMessage("For security reasons, the username and display name must be different from each other");
        }

        public static IRuleBuilderOptions<T, string> Domain<T>(this IRuleBuilder<T, string> ruleBuilder, Text text, string field = "website")
        {
            return ruleBuilder
                .Must(domain => Uri.CheckHostName(domain) == UriHostNameType.Dns)
                .WithMessage(text.Original("{0}: Invalid domain format.", field));
        }

        public static IRuleBuilderOptions<T, string> HeaderImage<T>(this IRuleBuilder<T, string> ruleBuilder, Text text, IWebRootPath webRootPath)
        {
            return ruleBuilder
                .Must(header =>
                {
                    if (header.StartsWith("data:image/") == true)
                    {
                        var base64Data = header.Contains(",") ? header[(header.IndexOf(",") + 1)..] : header;
                        try
                        {
                            // Try decoding from Base64
                            var imageBytes = Convert.FromBase64String(base64Data);

                            // Validate with ImageSharp (will throw if not a valid image)
                            using var image = Image.Load<Rgba32>(imageBytes);

                            // Ok
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    if (header.Has() == true)
                    {
                        var path = header.TrimStart('/');
                        var fullPath = Path.Combine(webRootPath.WebRootPath, path);
                        return File.Exists(fullPath);
                    }
                    return true;
                })
                .WithMessage(text.Original("Header image must be valid"));
        }

        public static IRuleBuilderOptions<T, string> Html<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .Must(content =>
                {
                    if (content.Has() == false) return true;
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(content);
                        return doc.ParseErrors == null || !doc.ParseErrors.Any();
                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessage(text.Original("Content must be a valid HTML"));
        }

        public static IRuleBuilderOptions<T, string> HtmlInjection<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
            where T : IContent
        {
            return ruleBuilder
                .Must(content =>
                {
                    //avoiding cross-site script attacks
                    if (content.Contains("<script", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return false;
                    }
                    //avoiding cross-site script attacks
                    if (content.Contains("<link", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return false;
                    }
                    //avoiding cross-site script attacks
                    if (content.Contains("<iframe", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(text.Original("Content must be a valid HTML"));
        }

        public static IRuleBuilderOptions<T, Guid> Id<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("Id is required"));
        }

        public static IRuleBuilderOptions<T, string> IFrame<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .Must(iframe => iframe.Has() ? iframe.IsIFrame() : true)
                .WithMessage(text.Original("Iframe must be valid"));
        }

        public static IRuleBuilderOptions<T, string> Language<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .Must(language =>
                {
                    if (language.Has() == true)
                    {
                        try
                        {
                            var cinfo = new CultureInfo(language);
                            return true;
                        }
                        catch (CultureNotFoundException)
                        {
                            return false;
                        }
                    }
                    return false;
                })
                .WithMessage(text.Original("Language must be valid"));
        }

        public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("Name is required"))
                .MaximumLength(255)
                .WithMessage(text.Original("Name cannot exceed 255 characters"));
        }

        public static IRuleBuilderOptions<T, string> NanoId<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("NanoId is required"))
                .MaximumLength(6)
                .WithMessage(text.Original("NanoId cannot exceed 6 characters"));
        }

        public static IRuleBuilderOptions<T, string> Note<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("Note is required"))
                .MaximumLength(255)
                .WithMessage(text.Original("Note cannot exceed 255 characters"));
        }

        public static IRuleBuilderOptions<T, T> ParentId<T>(this IRuleBuilder<T, T> ruleBuilder, Text text) where T : IId, IParentIdNull
        {
            return ruleBuilder
                .Must(x => x.ParentId == null || x.Id != x.ParentId)
                .WithMessage(text.Original("An entity cannot be its own parent"));
        }

        public static IRuleBuilderOptions<T, byte> Rating<T>(this IRuleBuilder<T, byte> ruleBuilder, Text text)
        {
            return ruleBuilder
                .LessThanOrEqualTo((byte)5)
                .WithMessage(text.Original("Rating must be less than or equal to 5"));
        }

        public static IRuleBuilderOptions<T, string> RssFeed<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .Must(url =>
                {
                    if (url.Has() == true)
                    {
                        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    }
                    return true;
                })
                .WithMessage(text.Original("RSS Feed must be a valid website address"));
        }

        public static IRuleBuilderOptions<T, string> ShortName<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("Name is required"))
                .MaximumLength(50)
                .WithMessage(text.Original("Name cannot exceed 50 characters"));
        }

        public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .MaximumLength(255)
                .WithMessage(text.Original("Slug cannot exceed 255 characters"));
        }

        public static IRuleBuilderOptions<T, string> Tag<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .MaximumLength(128)
                .WithMessage(text.Original("Tag cannot exceed 128 characters"));
        }

        public static IRuleBuilderOptions<T, List<OrigamiContentTag>> TagsMustBeUnique<T>(this IRuleBuilder<T, List<OrigamiContentTag>> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(tags =>
                {
                    if (tags.DistinctBy(x => x.Tag).Count() != tags.Count)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(text.Original("Tags must be unique"));
        }
        public static IRuleBuilderOptions<T, string> Title<T>(this IRuleBuilder<T, string> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(text.Original("Title is required"))
                .MaximumLength(255)
                .WithMessage(text.Original("Title cannot exceed 255 characters"));
        }

        public static IRuleBuilderOptions<T, string?> Username<T>(this IRuleBuilder<T, string?> ruleBuilder, Text text)
        {
            return ruleBuilder
                .NotNull().WithMessage(text.Original("Username is required"))
                .NotEmpty().WithMessage(text.Original("Username is required"))
                .MaximumLength(200).WithMessage(text.Original("Username cannot exceed 200 characters"));
        }

        public static IRuleBuilderOptions<T, string> Website<T>(this IRuleBuilder<T, string> ruleBuilder, Text text, string field = "website")
        {
            return ruleBuilder
                .Must(url =>
                {
                    if (url.Has() == true)
                    {
                        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    }
                    return true;
                })
                .WithMessage(text.Original("{0}: URL must be a valid website address", field));
        }

        public static IRuleBuilderOptions<T, T> TopLevelPageWhenFrontPage<T>(this IRuleBuilder<T, T> ruleBuilder, Text text)
        {
            return ruleBuilder
                .Must(entity =>
                {
                    if (entity is OrigamiPage page)
                    {
                        if (page.IsFrontPage == false)
                        {
                            return true;
                        }
                        return page.ParentId == null;
                    }

                    return false;
                }).WithMessage(text.Original("To promote to front-page, that page must be top-level"));
        }
    }
}
