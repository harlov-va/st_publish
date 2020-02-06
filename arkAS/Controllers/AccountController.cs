using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using arkAS.Models;

using System.Web.Script.Serialization;
using System.Web.Security;
using arkAS.BLL.Core;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;

namespace arkAS.Controllers
{

    //[Authorize]
    public class AccountController : Controller
    {
       static string param = "";
        public ActionResult Index()
        {
            return View();
        }


        //
        // POST: /Account/LogOff

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //if (Request.Params["demoType"]!=null)
            {
                ViewBag.isGuest = 1;
                param = Request.Params[0];

                var users = Membership.FindUsersByEmail("core-guest@mail.ru");
                foreach (MembershipUser user in users)
                {
                    if (user != null)
                    {
                        var my = user.GetPassword();
                        ViewBag.passwordGuest = my;
                       var r= Roles.GetAllRoles();
                       string[] members = Roles.GetRolesForUser();
                    }
                }
            }
                
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {  
            var users = Membership.FindUsersByEmail(model.Email);
           // var param = "";
           // if (Request.Params!=null) param = Request.Params[0];
            foreach (MembershipUser user in users)
            {
                if (user != null)
                {
                    try
                    {
                        var my = user.GetPassword();
                        if (user.GetPassword() == model.Password)
                        {
                            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                            if (!String.IsNullOrEmpty(returnUrl))
                            {
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                if (Roles.IsUserInRole(user.UserName, "admin"))
                                {
                                    return Redirect("/Admin");
                                }
                                else
                                {
                                    if (Roles.IsUserInRole(user.UserName, "guest"))
                                    {
                                        return Redirect("/Demo/" + param);
                                    }
                                    else
                                    {
                                        return Redirect("/");
                                    }
                                    //return Redirect("/");
                                }
                                

                            }


                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }


            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");
            return View(model);
        }

        //
        // GET: /Account/Register

        [Authorize]
        public ActionResult ResetPassword()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotViewModel model)
        {

            if (model.Email != null)
            {
                var user = Membership.FindUsersByEmail(model.Email).Cast<MembershipUser>().FirstOrDefault();
                bool isUserFind = false;
                if (user != null)
                {
                    var CoreManager = new CoreManager();
                    model.Password = user.GetPassword(); 
                    model.UserName = user.UserName;
                    string body = RenderRazorViewToString("Mail", model);

                    try
                    {
                        CoreManager.SendEmail(user.Email, "Восстановление пароля", body);
                        ViewBag.res = "Пароль выслан на вашу почту";
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Произошла ошибка при отправке на почту");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким E-mail не найдено");
                }
            }    
            return View();
        }
    
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        public ActionResult Manage(ManageViewModel model)
        {
            var users = Membership.FindUsersByName(HttpContext.User.Identity.Name);
            foreach (MembershipUser user in users)
            {

                if (user != null)
                {
                    if (model.Email != null)
                    {
                        user.Email = model.Email;
                        Membership.UpdateUser(user);
                    }

                    try
                    {
                        model.Email = user.Email;
                        model.LastActivityDate = user.LastActivityDate;
                        model.UserName = user.UserName;
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Произошла ошибка");
                    }
                }
            }
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var users = Membership.FindUsersByName(HttpContext.User.Identity.Name);
            foreach (MembershipUser user in users)
            {

                if (user != null)
                {

                    try
                    {
                        if (user.GetPassword() == model.oldPassword)
                        {
                            if (model.Password == model.ConfirmPassword)
                            {
                                user.ChangePassword(model.oldPassword, model.Password);
                                //FormsAuthentication.SetAuthCookie(model.Password, false);
                                ViewData["MessageSuccess"] = "Пароль успешно изменен";
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Старый пароль введен неверно");
                            return View(model);
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Произошла ошибка при смене пароля");
                    }
                }
            }
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            ModelState.AddModelError("", "Старый пароль указан неверно.");
            return View(model);
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return null; // GAG! Регистрация нам не нужна View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Попытка зарегистрировать пользователя
                try
                {
                    if (model.Password == model.ConfirmPassword)
                    {
                        Membership.CreateUser(model.Email, model.Password, email: model.Email);

                        //WebSecurity.Login(model.UserName, model.Password);
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //--------------------- Profile ------------------------------------------------------
        public ActionResult Profile()
        {
            return View();
        }

        public ActionResult Profile_get()
        {
            var mng = new ProfileManager();
            string fio =mng.GetProperty("fio","");
            string email = mng.GetProperty("email", "");
            string skype = mng.GetProperty("skype", "");
            string phone = mng.GetProperty("phone", "");
            string note = mng.GetProperty("note", "");
            return Json(new
            {
                fio = fio,
                email = email,
                skype=skype,
                phone=phone,
                note=note
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Profile_save()
        {
            var res = false;
            var msg = "";
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                var mng = new ProfileManager();
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var fio = AjaxModel.GetValueFromSaveField("fio", fields);
                var email = AjaxModel.GetValueFromSaveField("email", fields);
                var skype = AjaxModel.GetValueFromSaveField("skype", fields);
                var phone = AjaxModel.GetValueFromSaveField("phone", fields);
                var note = AjaxModel.GetValueFromSaveField("note", fields);

                mng.SetProperty("fio", fio);
                mng.SetProperty("email", email);
                mng.SetProperty("skype", skype);
                mng.SetProperty("phone", phone);
                mng.SetProperty("note", note);
                msg = "Изменения в профиле успешно подтверждены";
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                msg = ex.Message;
            }
            return Json(new
            {
                result = res,
                msg = msg
            }, JsonRequestBehavior.AllowGet);
        }
        //--------------------- END Profile ------------------------------------------------------

        #region Вспомогательные методы
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }



        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Полный список кодов состояния см. по адресу http://go.microsoft.com/fwlink/?LinkID=177550
            //.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Введите другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль. Введите допустимое значение пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                default:
                    return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
            }
        }
        #endregion
    }
}