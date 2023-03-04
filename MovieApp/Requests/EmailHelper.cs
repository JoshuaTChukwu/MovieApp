using GOSBackend.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using static MovieApp.Contracts.Common.AuxillaryObjs;

namespace MovieApp.Requests
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IEmailConfiguration _config;
        public EmailHelper( IEmailConfiguration emailConfiguration)
        {
            _config = emailConfiguration;
        }
        public bool SendMail(string email, string name, string subject, string body, string attachmentUrl ="", FileType type = FileType.None, string fileName ="")
        {
			try
			{
				var message = new MimeMessage();
                message.From.Add(new MailboxAddress("GOS Software", _config.SmtpUsername));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = subject;
                if(attachmentUrl == "" || type == FileType.None)
                {
                    message.Body = new TextPart("html") { Text = body};
                }
                else
                {
                    var bodyEmail = new TextPart("html")
                    {
                        Text = body
                    };
                    var attachment = GetFileMimePart(fileName,type,attachmentUrl);
                    var multipart = new Multipart("mixed");
                    multipart.Add(bodyEmail);
                    multipart.Add(attachment);
                    message.Body = multipart;
                }
                using (var client = new SmtpClient())
                {
                    client.Connect(_config.SmtpServer, _config.SmtpPort, SecureSocketOptions.StartTls);
                    client.Authenticate(_config.SmtpUsername, _config.SmtpPassword);

                    // Send the message
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
			catch (Exception)
			{

				return false;
			}

        }
        private static MimePart GetFileMimePart(string fileName, FileType fileType, string fileUrl)
        {
            if(fileType == FileType.JPEG)
            {
                var attachment = new MimePart("image", "jpeg")
                {
                    Content = new MimeContent(File.OpenRead(fileUrl)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = fileName,
                };
                return attachment;
            }
            else if(fileType == FileType.PNG)
            {
                var attachment = new MimePart("image", "jpeg")
                {
                    Content = new MimeContent(File.OpenRead(fileUrl)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = fileName,
                };
                return attachment;
            }
            else if(fileType == FileType.Word)
            {
                var attachment = new MimePart("application", "vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    Content = new MimeContent(File.OpenRead(fileUrl)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = fileName,
                };
                return attachment;
            }
            else if(fileType == FileType.Excel)
            {
                var attachment = new MimePart("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet (xlsx)")
                {
                    Content = new MimeContent(File.OpenRead(fileUrl)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = fileName,
                };
                return attachment;
            }
            else
            {
                var attachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(File.OpenRead(fileUrl)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = fileName
                };
                return attachment;
            }
        }
    }
    public interface IEmailHelper
    {
        bool SendMail(string email, string name, string subject, string body, string attachmentUrl = "", FileType type = FileType.None, string fileName = "");
    }
}
