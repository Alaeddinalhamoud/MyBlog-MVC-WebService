using MyBlog.Data;
using MyBlog.Repo;
using MyBlog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.Services;

namespace MyBlogAPI
{
    /// <summary>
    /// Summary description for Account
    /// </summary>
    [WebService(Namespace = "http://api.a-hamoud.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Account : System.Web.Services.WebService
    {
        EFDbContext db = new EFDbContext();
        private IUserRepository p = new EFUserRepository();
        private MembershipProvider mm = new EFMembershipProvider();
        private IDEncryptionRepository repositoryDEncryption = new EFEncryptionRepository();
        private IEmailSettingRepository repositoryEmailSetting = new EFEmailSettingRepository();


        /// <summary>
        /// Forgot Password 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        [WebMethod]
        public bool ForgotPasswordAPI(string email)
        {
            bool status = false;

            User user = p.UserIEmum.Where(p => p.Email.Equals(email)).FirstOrDefault();

            if (user != null)
            {
                //Send Email with password
                EMailPasswordSender(user.Email, user.Password);

                status = true;

                return status;
            }
            

            return status;
        }

        [WebMethod]
        public bool LoginAPI(string email, string password)
        {
            bool status = false;

            string EncryptedPW = repositoryDEncryption.Encrypt(password);
            status = mm.ValidateUser(email, EncryptedPW);
            if (status)
            {
                User _User = p.UserIEmum.Where(a => a.Email.Equals(email.TrimEnd())).FirstOrDefault();
                _User.Last_Login = DateTime.Now;
                p.Save(_User);
            }
            // status=   p.UniqueEmail(email);

            return status;
        }

        /// <summary>
        /// SignUp 
        /// </summary>
        /// <param name="FName"></param>
        /// <param name="LName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [WebMethod]
        public bool SignUpAPI(string FName, string LName, string email, string password)
        {
            bool status = false;

            string EncryptedPW = repositoryDEncryption.Encrypt(password);

            bool IsUniqeEmail = p.UniqueEmail(email);

            if (!IsUniqeEmail)
            {

                User _user = new User();

                _user.FName = FName;
                _user.LName = LName;
                _user.Email = email;
                _user.Password = EncryptedPW;
                _user.Create_time = DateTime.Now;
                _user.Update_Time = DateTime.Now;
                _user.Last_Login = DateTime.Now;
                _user.RoleId = 2;//TO be normal user Role

                status = p.Save(_user);
                status = true;
            }
            return status;
        }


        /// <summary>
        /// Email Sender 
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="Password"></param>

        public void EMailPasswordSender(string receiver, string Password)
        {
            EmailSetting _emailsetting = repositoryEmailSetting.GetEmailSetting;

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(_emailsetting.SMTP_Server);

            mail.From = new MailAddress(_emailsetting.Sender);
            mail.To.Add(receiver);
            mail.Subject = "Your Password";
            string HashUserPassword = repositoryDEncryption.Decrypt(Password);
            mail.Body = HashUserPassword;

            // SmtpServer.Port = _emailsetting.SMTPServer_Port;
            string HashEmailPassword = repositoryDEncryption.Decrypt(_emailsetting.Password);
            SmtpServer.Credentials = new NetworkCredential(_emailsetting.UserName, HashEmailPassword);
            NetworkCredential Credentials = new NetworkCredential(_emailsetting.Sender, HashEmailPassword);
            SmtpServer.Credentials = Credentials;
            // SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            // SmtpServer.UseDefaultCredentials = true;
            // SmtpServer.EnableSsl = _emailsetting.EnableSSL;

            SmtpServer.Send(mail);
           
        }
    }
}
