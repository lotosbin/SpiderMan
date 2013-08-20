using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using SpiderMan.Models;

namespace SpiderMan.Controllers {
    [Authorize]
    public class AccountController : Controller {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl) {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe)) {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                return RedirectToLocal(returnUrl);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "提供的用户名或密码不正确。");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Task");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model) {
            if (ModelState.IsValid) {
                // 尝试注册用户
                try {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Task");
                } catch (MembershipCreateUserException e) {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        #region 帮助程序
        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Task");
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请输入其他用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "该电子邮件地址的用户名已存在。请输入其他电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
        #endregion
    }
}
