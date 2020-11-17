using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.ComponentModel;

namespace LeanCode.Standard.Mail
{
    public class EmailProvider
    {
        private object emailLock = new object();
        SmtpClient client;

        public EmailProvider()
        {
            client = new SmtpClient();
            client.Host = "smtp.office365.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("techsystems@LeanCodecapitalgroup.com", "Astonvillafc1!");
        }

        public void SendEmail(MailMessage message)
        {
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            client.SendAsync(message, message);
        }
        public void SendEmail(string subject, string body, List<string> _attachments, List<string> emailList)
        {
            try
            {
                MailAddress from = new MailAddress("techsystems@LeanCodecapitalgroup.com", String.Empty, System.Text.Encoding.UTF8);
                if (emailList == null || emailList.Count == 0)
                    return;
                else
                {
                    MailAddress to = new MailAddress(emailList[0]);
                    MailMessage message = new MailMessage(from, to);
                    for (int i = 1; i < emailList.Count; i++)
                    {
                        message.To.Add(emailList[i]);
                    }

                    if (_attachments.Count != 0)
                    {
                        foreach (var filename in _attachments)
                        {
                            message.Attachments.Add(new Attachment(filename));
                        }
                    }
                    message.Body = body;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = subject;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                    client.SendAsync(message, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to send an email\n " + ex.ToString());
            }
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            MailMessage msg = (MailMessage)e.UserState;
            lock (emailLock)
            {
                if (e.Cancelled || e.Error != null)
                {
                    if (e.Cancelled)
                        Console.WriteLine("send cancelled");
                    if (e.Error != null)
                        Console.WriteLine(e.Error.Message);
                    Thread.Sleep(60 * 1000);
                    SendEmail(msg);
                }
                else
                {
                    Console.WriteLine("email sent");
                }


                if (msg != null)
                    msg.Dispose();
            }
        }
    }
}
