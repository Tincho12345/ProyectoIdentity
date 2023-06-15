using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace ProyectoIdentity.Services
{
    public class MailJetEmailSender : IEmailSender
    {
        public OpcionesMailJet _opcionesMailJet;
        private readonly IConfiguration _config;

        public MailJetEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _opcionesMailJet = _config.GetSection("MailJet").Get<OpcionesMailJet>();
            MailjetClient client = new MailjetClient(_opcionesMailJet.ApiKey, _opcionesMailJet.SecretKey)
            {
                Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", "carlos1martin2espindola@gmail.com"},
        {"Name", "Confirmar Email/Recupero Contraseña"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }, {
          "Name",
          "Carlos Martin"
         }
        }
       }
      }, {
       "Subject",
       "Greetings from Mailjet."
      }, {
       "TextPart",
       subject
      }, {
       "HTMLPart",
       htmlMessage
      }
     }
             });
            await client.PostAsync(request);
        }
    }
}
    