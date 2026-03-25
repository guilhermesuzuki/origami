using FluentValidation;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Validators;

public class OrigamiSettingsValidator : AbstractValidator<OrigamiSettings>
{
    public OrigamiSettingsValidator(Text text, IWebRootPath webRootPath) : base()
    {
        RuleFor(x => x.Name).Name(text);
        RuleFor(x => x.Description).Description(text);
        RuleFor(x => x.HeaderImage).HeaderImage(text, webRootPath);
        RuleFor(x => x.RssFeed1).RssFeed(text);
        RuleFor(x => x.RssFeed2).RssFeed(text);
        RuleFor(x => x.RssFeed3).RssFeed(text);
        RuleFor(x => x.RssFeed4).RssFeed(text);
        RuleFor(x => x.RssFeed5).RssFeed(text);
        RuleFor(x => x.OpenTelemetry.Endpoint).Website(text, field: "Open telemetry endpoint");
        RuleFor(x => x.SmtpServer).Domain(text, field: "SMTP server");
    }
}
