using System;
using System.Collections.Generic;
using System.IO;

using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit;
using MimeKit;

public class MailRepository
{
    private readonly string mailServer, login, password,smtpServer;
    private readonly int port;
    private readonly bool ssl;

    public MailRepository(string smtpServer, string mailServer, int port, bool ssl, string login, string password)
    {
        this.smtpServer = smtpServer;
        this.mailServer = mailServer;
        this.port = port;
        this.ssl = ssl;
        this.login = login;
        this.password = password;
    }

    public IEnumerable<string> GetUnreadMails()
    {
        var messages = new List<string>();

        using (var client = new ImapClient())
        {
            client.Connect(mailServer, port, ssl);

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            client.Authenticate(login, password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
            foreach (var uniqueId in results.UniqueIds)
            {
                var message = inbox.GetMessage(uniqueId);

                messages.Add(message.HtmlBody);

                //Mark message as read
                //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
            }

            client.Disconnect(true);
        }

        return messages;
    }

    public IEnumerable<string> GetAllMails()

    {
        var messages = new List<string>();

        using (var client = new ImapClient())
        {
            client.Connect(mailServer, port, ssl);

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            client.Authenticate(login, password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            var results = inbox.Search(SearchOptions.All, SearchQuery.NotSeen);
            foreach (var uniqueId in results.UniqueIds)
            {
                var message = inbox.GetMessage(uniqueId);

                messages.Add(message.HtmlBody);

                //Mark message as read
                //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
            }

            client.Disconnect(true);
        }

        return messages;
    }

    public void SendMail(String subject,String text){
        var message = new MimeMessage ();
        message.From.Add(new MailboxAddress("username", login));
        message.To.Add(new MailboxAddress("receiver", "receiverremail"));
        message.Subject = subject;

        message.Body = new TextPart ("plain") {Text = text};
        
        using (var client = new SmtpClient())
        {
            client.Connect(smtpServer,587,false);


            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate(login, password);

            client.Send(message);
            client.Disconnect(true);
        }
    }

    public void sendMailwithAttachments(String subject, String text){
        var message = new MimeMessage ();
        message.From.Add (new MailboxAddress ("username", login));
        message.To.Add (new MailboxAddress ("receiver", "receiverremail"));
        message.Subject = subject;

        var body = new TextPart ("plain") {Text = text};

        var attachment = new MimePart("image", "gif"){
            Content = new MimeContent (File.OpenRead ("E:\\Wallpapers"), ContentEncoding.Default),
            ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName ("E:\\Wallpapers")
        };

        var multipart = new Multipart ("mixed");
        multipart.Add (body);
        multipart.Add (attachment);
        // now set the multipart/mixed as the message body
        message.Body = multipart;
        
        using (var client = new SmtpClient())
        {
            client.Connect(smtpServer,587,false);


            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate(login, password);

            client.Send(message);
            client.Disconnect(true);
        }
    }

    public void readMailAttachments(){
        using (var client = new ImapClient ()) {
            
            client.Connect(mailServer, port, ssl);

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            client.Authenticate(login, password);

    client.Inbox.Open (FolderAccess.ReadWrite);
    IList<UniqueId> uids = client.Inbox.Search (SearchQuery.All);

    foreach (UniqueId uid in uids) {
        MimeMessage message = client.Inbox.GetMessage (uid);

        foreach (MimeEntity attachment in message.Attachments) {
            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

            using (var stream = File.Create (fileName)) {
                if (attachment is MessagePart) {
                    var rfc822 = (MessagePart) attachment;

                    rfc822.Message.WriteTo (stream);
                } else {
                    var part = (MimePart) attachment;

                    part.Content.DecodeTo (stream);
                }
            }
        }
    }
}
    }
}