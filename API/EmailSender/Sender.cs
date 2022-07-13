using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace EmailSender
{
    public class Sender
    {
        //metoda de a trimite mail in C#
        public string sendEmail(string to)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    string from = "ioan.padurean@liceuludus.ro";
                    string pass = "ihparbqnevrgcalm";//cheie de aplicatie furnizata de gmail
                    if (to.Equals("") || !to.Contains("@"))//verificare adresa de mail introdusa sa fie valida
                    {

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
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
