using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class EmailRepository : IEmailRepository
    {
        protected readonly ISettingsRepository _settingsRepository;
        protected readonly Text _text;

        public EmailRepository(
            ISettingsRepository settingsRepository,
            Text text) : base()
        {
            _settingsRepository = settingsRepository;
            _text = text;
        }

        public Result ConnectWithDefaultSettings()
        {
            var settings = _settingsRepository.GetSettings();
            using var smtp = new SmtpClient();

            try
            {
                var socketOptions = settings.EnableSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None;
                smtp.Connect(settings.SmtpServer, settings.SmtpServerPort, socketOptions);
                smtp.Authenticate(settings.SmtpUserName, settings.SmtpPassword);
            }
            catch (Exception ex)
            {
                return new(ex);
            }
            finally
            {
                if (smtp.IsConnected) smtp.Disconnect(true);
            }

            return new();
        }

        public Result ConnectWithTheseSettings(OrigamiSettings settings)
        {
            using var smtp = new SmtpClient();

            try
            {
                var socketOptions = settings.EnableSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None;
                smtp.Connect(settings.SmtpServer, settings.SmtpServerPort, socketOptions);
                smtp.Authenticate(settings.SmtpUserName, settings.SmtpPassword);
            }
            catch (Exception ex)
            {
                return new(ex);
            }
            finally
            {
                if (smtp.IsConnected) smtp.Disconnect(true);
            }

            return new();
        }

        public Result ConnectWithTheseSettings(bool ssl, string smtpServer, int smtpPort, string username, string password)
        {
            using var smtp = new SmtpClient();

            try
            {
                var socketOptions = ssl ? SecureSocketOptions.Auto : SecureSocketOptions.None;
                smtp.Connect(smtpServer, smtpPort, socketOptions);
                smtp.Authenticate(username, password);
            }
            catch (Exception ex)
            {
                return new(ex);
            }
            finally
            {
                if (smtp.IsConnected) smtp.Disconnect(true);
            }

            return new();
        }

        public Result Send(string subject, BodyBuilder body, IEnumerable<string> to)
        {
            return this.Send(subject, body, to, [], []);
        }

        public Result Send(string subject, BodyBuilder body, IEnumerable<string> to, IEnumerable<string> cc)
        {
            return this.Send(subject, body, to, cc, []);
        }

        public Result Send(string subject, BodyBuilder body, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            var settings = _settingsRepository.GetSettings();
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(settings.SmtpUserName),
                Subject = subject,
                Body = body.ToMessageBody(),
            };

            foreach (var recipient in to)
            {
                email.To.Add(MailboxAddress.Parse(recipient));
            }

            foreach (var recipient in cc)
            {
                email.Cc.Add(MailboxAddress.Parse(recipient));
            }

            foreach (var recipient in bcc)
            {
                email.Bcc.Add(MailboxAddress.Parse(recipient));
            }

            using var smtp = new SmtpClient();

            try
            {
                var socketOptions = settings.EnableSsl
                    ? SecureSocketOptions.Auto
                    : SecureSocketOptions.None;

                smtp.Connect(settings.SmtpServer, settings.SmtpServerPort, socketOptions);
                smtp.Authenticate(settings.SmtpUserName, settings.SmtpPassword);
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                return new(ex);
            }
            finally
            {
                if (smtp.IsConnected) smtp.Disconnect(true);
            }

            return new();
        }

        public Result SendVerificationCode(string toEmail, string code)
        {
            var settings = _settingsRepository.GetSettings();
            var body = new BodyBuilder()
            {
                HtmlBody =
                    $"<p>{_text.Original("This code is for the subscription process in the website")}</p>" +
                    $"<p>{_text.Original("Do NOT share this code with anybody")}: <b>{code}</b>.</p>",
            };
            return this.Send(_text.Original("Here's your verification code for {0}", settings.Name), body, [toEmail]);
        }
    }
}
