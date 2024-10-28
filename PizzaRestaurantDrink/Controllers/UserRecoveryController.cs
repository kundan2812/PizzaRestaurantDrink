using dblayer;
using PizzaRestaurantDrink.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzaRestaurantDrink.Controllers
{
    public class UserRecoveryController : Controller
    {
        // GET: PasswordRecovery
        private Pro_PizzResturentandDrinkDbEntities1 DB = new Pro_PizzResturentandDrinkDbEntities1();
        // GET: PasswordRecovery
        public ActionResult AccountRecovery()
        {

            var accountRecoveryMV = new AccountRecoveryMV();
            return View(accountRecoveryMV);
        }

        [HttpPost]
        public ActionResult AccountRecovery(AccountRecoveryMV accountRecoveryMV)
        {
            using (var transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = DB.UserTables.Where(u => u.UserName.Trim() == accountRecoveryMV.UserName || u.EmailAddress == accountRecoveryMV.UserName.Trim()).FirstOrDefault();
                        if (user != null)
                        {
                            string code = DateTime.Now.ToString("yyyyMMddHHmmssmm") + accountRecoveryMV.UserName;
                            var accountrecoverydetails = new UserPasswordRecoveryTable();
                            accountrecoverydetails.UserID = user.UserID;
                            accountrecoverydetails.OldPassword = user.Password;
                            accountrecoverydetails.RecoveryCode = code;
                            accountrecoverydetails.RecoveryCodeExpiryDateTime = DateTime.Now.AddDays(1);
                            accountrecoverydetails.RecoveryStatus = true;
                            DB.UserPasswordRecoveryTables.Add(accountrecoverydetails);
                            DB.SaveChanges();

                            var callbackUrl = Url.Action("ResetPassword", "User", new { recoverycode = code }, protocol: Request.Url.Scheme);
                            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailTemplates/ForgotPasswordConfirmation.html")))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{ConfirmationLink}", callbackUrl);
                            body = body.Replace("{UserName}", user.EmailAddress);
                            bool IsSendEmail = HelperClass.EmailCls.Send(user.EmailAddress, "Reset Password", body, true);
                            if (IsSendEmail)
                            {
                                transaction.Commit();
                                ModelState.AddModelError(string.Empty, "Recovery Link Sent on your email address(" + user.EmailAddress + ")");
                            }
                            else
                            {
                                transaction.Rollback();
                                ModelState.AddModelError(string.Empty, "Some Issue is Occure! Please Try Again later.");
                            }
                        }
                        else
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("UserName", "Not Found!");
                        }
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "Some Issue is Occure! Please Try Again later.");
                }
            }
            return View(accountRecoveryMV);
        }
    }
}