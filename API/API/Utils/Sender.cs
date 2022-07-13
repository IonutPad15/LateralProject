using System.Net.Mail;

namespace API.Utils
{
    public class Sender
    {
        private readonly ILogger<Sender>? logger;
        public Sender(ILogger<Sender> logger)
        {
            this.logger = logger;
        }
        public Sender()
        {
            
        }

        //TODO: methods should start with an uppercase letter
        //metoda de a trimite mail in C#
        public enum ResultCode
        {
            InvalidAdress, ValidAdress
        }
        public string SendEmail(string to)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    string from = "ioan.padurean@liceuludus.ro";
                    string pass = "ihparbqnevrgcalm";//cheie de aplicatie furnizata de gmail
                    if (to.Equals("") || !to.Contains("@"))//verificare adresa de mail introdusa sa fie valida
                    {
                        //TODO: use Enums for Error Codes
                        return "err1";
                    }
                    else
                    {
                        Random random = new Random();
                        int nr1, nr2, nr3, nr4;//4 cifre random
                        nr1 = random.Next(9);
                        nr2 = random.Next(9);
                        nr3 = random.Next(9);
                        nr4 = random.Next(9);
                        string verif = "" + nr1 + nr2 + nr3 + nr4;
                        string body = "<p>V-ati facut cont pe TheForestMan!</p> ";
                        body += "<p> Codul de siguranta:</p>";
                        body += "<p> <h1> " + verif + " </h1> </p>";
                        mail.From = new MailAddress(from);
                        mail.To.Add(to);
                        mail.Subject = "TheForestMan";
                        mail.Body = body;
                        mail.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential(from, pass);
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                        return verif;
                    }

                }
            }
            catch (Exception ex)
            {
                //TODO: use ILogger https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
                logger.LogError(ex.Message);
                return ""; //TODO: string.Empty
            }
        }
    }
}
