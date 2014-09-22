using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace anhnv.csharp.utils.SocialMedia
{
    /// <summary>
    /// Xử lý email: Kiểm tra email, gửi mail
    /// </summary>
    public class MailTask
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidEmails(List<string> emails)
        {
            return emails.All(IsValidEmail);
        }

        /// <summary>
        /// Gửi mail
        /// </summary>
        /// <param name="eMail">Email</param>
        /// <param name="mailSettings">Các thiết lập gửi mail</param>
        /// <returns></returns>
        public static bool SendMailTo(EMail eMail, MailSetting mailSettings)
        {
            // Validate email
            if (!IsValidEmails(eMail.ToEmails)) return false;

            // Chuyển về kí tự phân cách email mặc định
            var emailTos = string.Join(",", eMail.ToEmails);

            // Mail Message
            var mail = new MailMessage(mailSettings.UserName, emailTos)
            {
                Subject = eMail.Subject,
                Body = eMail.Body,
                Priority = eMail.Priority,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(mailSettings.UserName, mailSettings.SmtpFromName)
            };
            if (!string.IsNullOrEmpty(mailSettings.SmtpBcc))
            {
                mail.Bcc.Add(mailSettings.SmtpBcc);
            }
            // Attachment
            if (eMail.Attachments != null && eMail.Attachments.Any())
            {
                foreach (var attachment in eMail.Attachments)
                {
                    mail.Attachments.Add(new Attachment(new MemoryStream(attachment.Content), attachment.FileName));
                }
            }
            // Header
            if (eMail.Headers != null && eMail.Headers.Count > 0)
            {
                foreach (var key in eMail.Headers.Keys)
                {
                    mail.Headers.Set(key, eMail.Headers[key].ToString());
                }
            }
            // Smpt Client
            var smtp = new SmtpClient(mailSettings.Host, mailSettings.Port)
            {
                EnableSsl = mailSettings.EnableSsl,
                DeliveryMethod = mailSettings.DeliveryMethod,
                UseDefaultCredentials = mailSettings.DefaultCredentials,
                Credentials = new NetworkCredential(mailSettings.UserName, mailSettings.Password)
            };

            // Sending email
            smtp.Send(mail);
            mail.Dispose();
            return true;
        }
    }

    public class EMail
    {
        private List<string> _emails;

        public EMail()
        {
            Priority = MailPriority.High;
            //SplitChar = ',';
            _emails = new List<string>();
        }

        public EMail(List<string> toEmails, string subject, string body)
            : this()
        {
            Body = body;
            Subject = subject;
            ToEmails = toEmails;
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        /// <summary>
        /// Danh sách email sẽ gửi
        /// </summary>
        public List<string> ToEmails
        {
            get { return _emails; }
            set
            {
                if (!MailTask.IsValidEmails(value))
                {
                    throw new ArgumentException("Danh sách email không đúng định dạng.");
                }
                _emails = value;
            }
        }

        /// <summary>
        /// Defaul: MailPriority.Low;
        /// </summary>
        public MailPriority Priority { get; set; }

        /// <summary>
        /// File đính kèm gửi theo mail
        /// </summary>
        public List<EAttachment> Attachments { get; set; }

        public Dictionary<string, object> Headers { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MailSetting
    {
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// UserName == From
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// DeliveryMethod
        /// </summary>
        public SmtpDeliveryMethod DeliveryMethod { get; set; }

        /// <summary>
        /// DefaultCredentials
        /// </summary>
        public bool DefaultCredentials { get; set; }

        /// <summary>
        /// EnableSsl
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// SmtpFromName
        /// </summary>
        public string SmtpFromName { get; set; }

        /// <summary>
        /// SmtpBcc
        /// </summary>
        public string SmtpBcc { get; set; }
    }

    [Serializable]
    public class EAttachment
    {
        private byte[] _content;
        public byte[] Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string PhysicalFilePath { get; set; }

        public EAttachment(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not exist: " + file.FullName);
            }
            FileName = file.Name;
            PhysicalFilePath = file.FullName;
            _content = File.ReadAllBytes(file.FullName);

        }
    }
}