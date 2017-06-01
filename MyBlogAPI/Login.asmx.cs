using MyBlog.Data;
using MyBlog.Repo;
using MyBlog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;

namespace MyBlogAPI
{
    /// <summary>
    /// Summary description for Login
    /// </summary>
    [WebService(Namespace = "http://api.a-hamoud.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Login : System.Web.Services.WebService
    {
        EFDbContext db = new EFDbContext();
      private  IUserRepository p = new EFUserRepository();
      private MembershipProvider mm = new EFMembershipProvider();
      private IDEncryptionRepository repoDEncryption = new EFEncryptionRepository();

        [WebMethod]
        public bool  LoginAPI(string email,string password)
        {
            bool status = false;

            string EncryptedPW = repoDEncryption.Encrypt(password);
            status = mm.ValidateUser(email, EncryptedPW);
            if (status)
            {
               User _User = p.UserIEmum.Where(a => a.Email.Equals(email.TrimEnd())).FirstOrDefault();
                _User.Last_Login = DateTime.Now;
                p.Save(_User);
            }
        // status=   p.UniqueEmail(email);
            
            return status ;
        }
       

    }
}
