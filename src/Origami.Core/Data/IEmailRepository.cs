using MimeKit;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IEmailRepository
    {
        Result ConnectWithDefaultSettings();
        Result ConnectWithTheseSettings(OrigamiSettings settings);
        Result ConnectWithTheseSettings(bool ssl, string smtpServer, int smtpPort, string username, string password);
        Result Send(string subject, BodyBuilder body, IEnumerable<string> to);
        Result Send(string subject, BodyBuilder body, IEnumerable<string> to, IEnumerable<string> cc);
        Result Send(string subject, BodyBuilder body, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc);
        Result SendVerificationCode(string toEmail, string code);
    }
}
