using System;

using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;

namespace Emailimap
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var mailRepository = new MailRepository("smtp.gmail.com","imap.gmail.com",993,true,"youremailhere","youremailpassword");
            var allmails = mailRepository.GetAllMails();
            
             foreach(var email in allmails){
                 Console.WriteLine(email);
             }

            mailRepository.SendMail("Testing smtp","Hello world");

            mailRepository.sendMailwithAttachments("Testing smtp","Hello world");

            mailRepository.readMailAttachments();


            
            
        }
    }
}
