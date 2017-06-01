using MyBlog.Repo;
using MyBlog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using MyBlog.Data;

namespace MyBlogAPI
{
    /// <summary>
    /// Summary description for SignUp
    /// </summary>
    [WebService(Namespace = "http://api.a-hamoud.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SignUp : System.Web.Services.WebService
    {
        EFDbContext db = new EFDbContext();
        private IUserRepository p = new EFUserRepository();
        private MembershipProvider mm = new EFMembershipProvider();
        private IDEncryptionRepository repoDEncryption = new EFEncryptionRepository();

        [WebMethod]
        public bool SignUpAPI(string FName, string LName,string email,string password)
        {
            bool status = false;

            string EncryptedPW = repoDEncryption.Encrypt(password);

           bool IsUniqeEmail = p.UniqueEmail(email);
            
            if (!IsUniqeEmail) {

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
        [WebMethod]
        public bool IsUniqeEmail(string email)
        {
            bool status = false;

            status = p.UniqueEmail(email);

            return status;
        }

    }
}
