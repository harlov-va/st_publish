using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Core;
using System.Web.Security;
using System.IO;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace arkAS.Controllers
{
    public class ProfileFormUserController : Controller
    {
        static string PathChapter = @"/uploads/Images/users_avatar/";
        static string FileStandart = @"avatar.png";
        static string GuiDefault = @"83e9d28d-e644-460e-9fc6-44999afb80c2";//hecrus@mail.ru
        public ProfileFormUserController()
        {
            string PathCurrent = AppDomain.CurrentDomain.BaseDirectory + PathChapter;
            if (!Directory.Exists(PathCurrent))
            {
                Directory.CreateDirectory(PathCurrent);
            }
        }
        //
        // GET: /ProfileFormUser/
        public static string GetUrl(string pathToUrl, int level)
        {
            string delimiter = @"\";
            string delimiterLevel = null;
            for (int i = level; i > 0; i--)
            {
                delimiterLevel = "";
                for (int j = i; j > 0; j--)
                {
                    delimiterLevel += delimiter;
                }
                pathToUrl = pathToUrl.Replace(delimiterLevel, "/");
            }
            return pathToUrl;
        }
        public static string GetImageSRC(string userGuid)
        {
            string pathImage = null;
            try
            {
              
                string PathFind = AppDomain.CurrentDomain.BaseDirectory + PathChapter;
                string[] extensions = {".jpg",".png",".jpeg",".gif"};
                List<string> files = new List<string>();

                foreach (string ext in extensions)
                 {
                     files.AddRange(Directory.GetFiles(PathFind, userGuid + ext, SearchOption.AllDirectories));
                     if (files.Count >= 1)
                     {
                         break;
                     }
                 }
                if(files.Count==1)
                {
                    pathImage = PathChapter + Path.GetFileName(files[0]);
                }
                else
                {
                    pathImage = PathChapter + FileStandart;      //GuiDefault+".jpg";
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return pathImage;
        }
        public ActionResult Index()
        {
            string UserName="";
            string GuiUser = GuiDefault;
            string UserAvatar="";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
                GuiUser=((Guid)Membership.GetUser().ProviderUserKey).ToString();
                UserAvatar = GetImageSRC(GuiUser);
            }
            else
            {
                UserName = "NoRegisterUser";
                UserAvatar = PathChapter + FileStandart;  
            }
       

            ViewBag.UserName=UserName;
            ViewBag.Avatar =GetUrl(UserAvatar,1);
            ViewBag.UserGuid = GuiUser;
            return View();
        }
         public ActionResult GetUserData()
        {
            string UserSurname = "";
            string UserName = "";
            string UserPatronymic = "";
            string UserEmail = "";
            string UserSkype = "";
            string UserPhoneOne = "";
            string UserPhoneTwo = "";
            string GuiUser = GuiDefault;
            if (User.Identity.IsAuthenticated)
            {
                GuiUser = ((Guid)Membership.GetUser().ProviderUserKey).ToString();
            }
            ProfileManager pm = new ProfileManager();
            UserSurname = pm.GetProperty("surname", "pfu", GuiUser);
            UserName = pm.GetProperty("name", "pfu", GuiUser);
            UserPatronymic = pm.GetProperty("otch", "pfu", GuiUser);
            UserEmail = pm.GetProperty("email", "pfu", GuiUser);
            UserSkype = pm.GetProperty("skype", "pfu", GuiUser);
            UserPhoneOne = pm.GetProperty("phoneOne", "pfu", GuiUser);
            UserPhoneTwo = pm.GetProperty("phoneTwo", "pfu", GuiUser);
          
            return Json(new
            {
                UserSurname = UserSurname,
                UserName = UserName,
                UserPatronymic = UserPatronymic,
                UserEmail = UserEmail,
                UserSkype =UserSkype,
                UserPhoneOne = UserPhoneOne,
                UserPhoneTwo = UserPhoneTwo
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult saveImage(string UserGuid, HttpPostedFileBase FileImg)
        {
            string error = "Error";
            string imgFile = PathChapter + FileStandart;
            string PathCurrent = AppDomain.CurrentDomain.BaseDirectory + PathChapter;
         
            if (FileImg.ContentLength > 0 && FileImg.ContentLength < 2100000)
            {

                try
                {
                    string extension = "";
                    if (Request.Browser.Browser.ToUpper() == "IE")
                    {
                        string[] files = FileImg.FileName.Split(new char[] { '\\' });
                        extension = files[files.Length - 1];
                    }
                    else
                        extension = Path.GetExtension(FileImg.FileName);
                    imgFile = PathChapter + UserGuid + extension;
                    FileImg.SaveAs(PathCurrent +UserGuid + extension);
                    error = "Success";
                }
                catch (Exception ex)
                {
                    RDL.Debug.LogError(ex);
                }
            } 
            return Json(new
            {
                error = error,
                imgFile = GetUrl(imgFile, 1)
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ContactDataSave(string UserGuid,string UserSurname, string UserName, string UserPatronymic, string UserEmail, string UserSkype, string UserPhoneOne, string UserPhoneTwo)
        {
            string error = "Error";
            try
            {
                ProfileManager pm = new ProfileManager();
                pm.SetProperty("surname", UserSurname, UserGuid);
                pm.SetProperty("name", UserName, UserGuid);
                pm.SetProperty("otch", UserPatronymic, UserGuid);
                pm.SetProperty("email", UserEmail, UserGuid);
                pm.SetProperty("skype", UserSkype, UserGuid);
                pm.SetProperty("phoneOne", UserPhoneOne, UserGuid);
                pm.SetProperty("phoneTwo", UserPhoneTwo, UserGuid);
                error = "Success";
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
           
            return Json(new
            {
                error = error
            }, JsonRequestBehavior.AllowGet);
        }
	}
}