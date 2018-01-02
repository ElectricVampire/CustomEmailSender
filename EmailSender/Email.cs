using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender
{
    public class Email
    {
        /// <summary>
        /// SMTP address
        /// </summary>
        private string smtpAddress = null;

        /// <summary>
        /// SMTP port
        /// </summary>
        private int port = -1;

        /// <summary>
        /// SSL Security
        /// </summary>
        private bool enableSSL = true;

        /// <summary>
        /// Email account from address
        /// </summary>
        private string fromAddress = null;

        /// <summary>
        /// Bcc address
        /// </summary>
        private string bcc = null;

        /// <summary>
        /// Cc address
        /// </summary>
        private string cc = null;

        /// <summary>
        /// password
        /// </summary>
        private string password = null;

        /// <summary>
        /// Email Configuration Manager instance
        /// </summary>
        private EmailConfig emailConfig = null;

        /// <summary>
        /// Template manager Instance
        /// </summary>
        private TemplateManager templateManager = null;

        /// <summary>
        /// Email Class Constructor
        /// - Reads EmailConfig.xml 
        /// - If parameter values are not passed from constructor they will be set from config file
        /// </summary>
        /// <param name="smtpAddress">Address of SMTP server</param>
        /// <param name="port">Port of SMTP server</param>
        /// <param name="enableSSL">Want to enable SSL?</param>
        /// <param name="fromAddress">From address for e-mail</param>
        /// <param name="pwd">Password for e-mail account</param>
        /// <param name="bcc">Need Bcc?</param>
        /// <param name="cc">Need Cc?</param>
        public Email(string smtpAddress = null, int port = -1, bool enableSSL = true, string fromAddress = null, string pwd = null, string bcc = null, string cc = null)
        {
            emailConfig = ConfigReader.Read("EmailConfig.xml", typeof(EmailConfig)) as EmailConfig;
            if (emailConfig != null)
            {
                this.smtpAddress = string.IsNullOrEmpty(smtpAddress) ? emailConfig.Smtp.SmtpAddress : smtpAddress;
                this.port = port == -1 ? emailConfig.Smtp.Port : port;
                this.enableSSL = enableSSL;
                this.fromAddress = string.IsNullOrEmpty(fromAddress) ? emailConfig.User.From : fromAddress;
                this.cc = string.IsNullOrEmpty(cc) ? emailConfig.User.Cc : cc;
                this.bcc = string.IsNullOrEmpty(bcc) ? emailConfig.User.Bcc : bcc;
                this.password = string.IsNullOrEmpty(password) ? emailConfig.User.Password : password;
                templateManager = new TemplateManager(emailConfig.Misc.TemplateConfigPath);
            }
            else
            {
                throw new ApplicationException("Not able to read EmailConfig.xml");
            }
        }

        /// <summary>
        /// Send an email where body of email can be retrieved from templates.
        /// </summary>
        /// <param name="toAddress">"To" account address</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="templateName">Name of template which need to used for body of email</param>
        /// <param name="templateInfoTable">DataTable with two column where First column should contains wildcards and Second Column name should contain values.
        /// All the wildcard in the template body will be replaced by coresponding value of that wild card to create e-mail body</param>
        /// <param name="attachmentLinkArray">Attachments path with file name</param>
        /// <param name="ishtml">body of e-mail contains html tags?</param>
        public void Send(string toAddress, string subject, string templateName, DataTable templateInfoTable, string[] attachmentLinkArray = null, bool ishtml = true)
        {
            string content = templateManager.GetTemplateData(templateName, templateInfoTable);
            Send(new string[] { toAddress }, subject, content, attachmentLinkArray, ishtml);
        }

        /// <summary>
        /// Send an email where body of email can be retrieved from templates.
        /// </summary>
        /// <param name="toAddress">"To" account addresses</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="templateName">Name of template which need to used for body of email</param>
        /// <param name="templateInfoTable">DataTable with two column where First column should contains wildcards and Second Column name should contain values.
        /// All the wildcard in the template body will be replaced by coresponding value of that wild card to create e-mail body</param>
        /// <param name="attachmentLinkArray">Attachments path with file name</param>
        /// <param name="ishtml">body of e-mail contains html tags?</param>
        public void Send(string [] toAddress, string subject, string templateName, DataTable templateInfoTable, string[] attachmentLinkArray = null, bool ishtml = true)
        {
            string content = templateManager.GetTemplateData(templateName, templateInfoTable);
            Send(toAddress, subject, content, attachmentLinkArray, ishtml);
        }

        /// <summary>
        /// Send an email 
        /// </summary>
        /// <param name="toAddress">"To" account addresses</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="content">e-mail body</param>
        /// <param name="attachmentLinkArray">Attachments path with file name</param>
        /// <param name="ishtml">body of e-mail contains html tags?</param>
        public void Send(string toAddress, string subject, string content, string[] attachmentLinkArray = null, bool ishtml = true)
        {
            Send(new string[] { toAddress }, subject, content, attachmentLinkArray, ishtml);
        }

        /// <summary>
        /// Send an email 
        /// </summary>
        /// <param name="toAddress">"To" account addresses</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="content">e-mail body</param>
        /// <param name="attachmentLinkArray">Attachments path with file name</param>
        /// <param name="ishtml">body of e-mail contains html tags?</param>
        public void Send(string[] toAddress, string subject, string content, string[] attachmentLinkArray = null, bool ishtml = true)
        {
            Task.Factory.StartNew(() =>
            {
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        mail.From = new MailAddress(this.fromAddress);

                        foreach (var item in toAddress)
                        {
                            mail.To.Add(item);
                        }

                        mail.Subject = subject;
                        mail.Body = content;
                        mail.IsBodyHtml = ishtml;

                        if (!string.IsNullOrEmpty(this.bcc)) mail.Bcc.Add(this.bcc);
                        if (!string.IsNullOrEmpty(this.cc)) mail.Bcc.Add(this.cc);

                        if (attachmentLinkArray != null && attachmentLinkArray.Length > 0)
                        {
                            foreach (var attachment in attachmentLinkArray)
                            {
                                if (string.IsNullOrEmpty(attachment))
                                    mail.Attachments.Add(new Attachment(attachment));
                            }
                        }

                        SendMessage(mail);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            });           
        }

        /// <summary>
        /// Final message send call to smtp server
        /// </summary>
        /// <param name="mail">Mailmsg object</param>
        private void SendMessage(MailMessage mail)
        {
            using (SmtpClient smtp = new SmtpClient(this.smtpAddress, this.port))
            {
                try
                {
                    smtp.Credentials = new NetworkCredential(this.fromAddress, this.password);
                    smtp.EnableSsl = this.enableSSL;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
