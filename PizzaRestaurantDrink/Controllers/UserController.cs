using dblayer;
using PizzaRestaurantDrink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzaRestaurantDrink.Controllers
{
    public class UserController : Controller
    {
        Pro_PizzResturentandDrinkDbEntities1 Db = new Pro_PizzResturentandDrinkDbEntities1();
        public ActionResult Login()
        {
            var user = new LoginMV();
            return View(user);
        }


        [HttpPost]
        public ActionResult Login(LoginMV loginMV)
        {

            if (ModelState.IsValid)
            {
                var user = Db.UserTables.Where(u => (u.EmailAddress == loginMV.UserName.Trim()) || u.UserName.Trim() == loginMV.UserName.Trim() && u.Password.Trim() == loginMV.Password.Trim()).FirstOrDefault();
                if (user != null)
                {
                    if (user.UserStatusID == 1)
                    {
                        Session["UserID"] = user.UserID;
                        Session["UserTypeID"] = user.UserTypeID;
                        return RedirectToAction("Dashboard", "User");
                    }
                    else
                    {
                        var accountstatus = Db.UserStatusTables.Find(user.UserStatusID);
                        ModelState.AddModelError(string.Empty, "Account is " + accountstatus.UserStatus + "");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please Enter Correct User Name and Password!");
                }
            }
            Session["UserID"] = string.Empty;
            Session["UserTypeID"] = string.Empty;
            return View(loginMV);
        }
        public ActionResult Logout()
        {
            Session["UserID"] = string.Empty;
            Session["UserTypeID"] = string.Empty;
            return RedirectToAction("Index", "Home");
        }



        public ActionResult ResetPassword(string recoverycode)
        {

            var forgotpassword = new ForgotPasswordMV();
            var userrecovery = Db.UserPasswordRecoveryTables.Where(p => p.RecoveryCode == recoverycode && p.RecoveryCodeExpiryDateTime > DateTime.Now && p.RecoveryStatus == true).FirstOrDefault();
            if (userrecovery == null)
            {
                return RedirectToAction("Login");
            }
            var user = Db.UserTables.Find(userrecovery.UserID);
            forgotpassword.UserID = userrecovery.UserID;
            forgotpassword.UserName = user.UserName;
            forgotpassword.EmailAddress = user.EmailAddress;
            return View(forgotpassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ForgotPasswordMV forgotPasswordMV)
        {
            using (var transaction = Db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = Db.UserTables.Find(forgotPasswordMV.UserID);
                        if (user != null)
                        {
                            if (forgotPasswordMV.NewPassword != forgotPasswordMV.ConfirmPassword)
                            {
                                ModelState.AddModelError("ConfirmPassword", "Not Match!");
                                return View(forgotPasswordMV);
                            }

                            var userrecovery = Db.UserPasswordRecoveryTables.Where(u => u.UserID == forgotPasswordMV.UserID && u.RecoveryStatus == true);
                            foreach (var item in userrecovery)
                            {
                                item.RecoveryStatus = false;
                                item.OldPassword = user.Password;
                                Db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                Db.SaveChanges();
                            }
                            user.Password = forgotPasswordMV.NewPassword;
                            Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            Db.SaveChanges();
                            transaction.Commit();
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Please Try-Again!");
                            return View(forgotPasswordMV);
                        }
                    }
                    ModelState.AddModelError(string.Empty, "Fill Field's Properly!");
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Please Try Again later.");
                }
            }
            return View(forgotPasswordMV);
        }

        public ActionResult Register()
        {
            var user = new Reg_UserMV();
            ViewBag.GenderID = new SelectList(Db.GenderTables.ToList(), "GenderID", "GenderTitle", "0");
            return View(user);
        }
        [HttpPost]
        public ActionResult Register(Reg_UserMV reg_UserMV)
        {
            reg_UserMV.UserTypeID = 4; // Customer User_Type_ID
            reg_UserMV.RegisterationDate = DateTime.Now;
            reg_UserMV.UserStatusID = 1;
            if (ModelState.IsValid)
            {
                bool isexist = false;
                var checkexist = Db.UserTables.Where(u => u.UserName.ToUpper().Trim() == reg_UserMV.UserName.ToUpper().Trim()).FirstOrDefault();
                if (checkexist != null)
                {
                    isexist = true;
                    ModelState.AddModelError("UserName", "Already Exist!");
                }
                checkexist = Db.UserTables.Where(u => u.EmailAddress.ToUpper().Trim() == reg_UserMV.EmailAddress.ToUpper().Trim()).FirstOrDefault();
                if (checkexist != null)
                {
                    isexist = true;
                    ModelState.AddModelError("EmailAddress", "Already Exist!");
                }
                if (isexist == false)
                {
                    var user = new UserTable();
                    user.UserTypeID = reg_UserMV.UserTypeID;
                    user.UserName = reg_UserMV.UserName;
                    user.Password = reg_UserMV.Password;
                    user.FirstName = reg_UserMV.FirstName;
                    user.LastName = reg_UserMV.LastName;
                    user.ContactNo = reg_UserMV.ContactNo;
                    user.GenderID = reg_UserMV.GenderID;
                    user.EmailAddress = reg_UserMV.EmailAddress;
                    user.RegisterationDate = reg_UserMV.RegisterationDate;
                    user.UserStatusID = reg_UserMV.UserStatusID;
                    Db.UserTables.Add(user);
                    Db.SaveChanges();
                    return RedirectToAction("Login", "User");
                }
            }
            ViewBag.GenderID = new SelectList(Db.GenderTables.ToList(), "GenderID", "GenderTitle", reg_UserMV.GenderID);
            return View(reg_UserMV);
        }

        public ActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Index", "Home");
            }

            int userid = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                int.TryParse(Convert.ToString(Session["UserID"]), out userid);
            }

            var dashboard = new DashboardMV(userid);
            return View(dashboard);
        }

        [HttpPost]
        public ActionResult Dashboard(DashboardMV dashboardMV)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }
            int userid = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserID"])))
            {
                int.TryParse(Convert.ToString(Session["UserID"]), out userid);
            }
            var dasboard = new DashboardMV(userid);
            var userdetail = Db.UserDetailTables.Find(userid);
            if (!string.IsNullOrEmpty(dashboardMV.OldPassword))
            {

                if (dasboard.ProfileMV.Password == dashboardMV.OldPassword)
                {
                    if (dashboardMV.NewPassword.Trim() == dashboardMV.ConfirmPassword.Trim())
                    {
                        var user = Db.UserTables.Find(userid);
                        user.Password = dashboardMV.NewPassword;
                        Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        Db.SaveChanges();
                        ModelState.AddModelError("OldPassword", "Password Changed");
                    }
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "Old Password is Incorrect!");
                }
            }
            if (!string.IsNullOrEmpty(dashboardMV.ProfileMV.FirstName) &&
               !string.IsNullOrEmpty(dashboardMV.ProfileMV.LastName) &&
               !string.IsNullOrEmpty(dashboardMV.ProfileMV.EmailAddress) &&
               !string.IsNullOrEmpty(dashboardMV.ProfileMV.ContactNo))
            {
                var user = Db.UserTables.Find(userid);
                user.FirstName = dashboardMV.ProfileMV.FirstName;
                user.LastName = dashboardMV.ProfileMV.LastName;
                user.EmailAddress = dashboardMV.ProfileMV.EmailAddress;
                user.ContactNo = dashboardMV.ProfileMV.ContactNo;
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                Db.SaveChanges();
                if (dashboardMV.ProfileMV.UserPhoto != null)
                {
                    var folder = "~/Content/ProfilePhoto";
                    var photoname = string.Format("{0}.jpg", user.UserID);
                    var response = HelperClass.FileUpload.UploadPhoto(dashboardMV.ProfileMV.UserPhoto, folder, photoname);
                    if (response)
                    {
                        var photo = string.Format("{0}/{1}", folder, photoname);
                       
                        if (userdetail == null)
                        {
                            userdetail = new UserDetailTable();
                            userdetail.UserDetailID = userid;
                            userdetail.UserID = userid;
                            userdetail.CreatedBy_UserID = userid;
                            userdetail.UserDetailProvideDate = DateTime.Now;
                            userdetail.PhotoPath = photo;
                            Db.UserDetailTables.Add(userdetail);
                            Db.SaveChanges();
                        }
                        userdetail.PhotoPath = photo;
                        userdetail.UserDetailProvideDate = DateTime.Now;
                        Db.Entry(userdetail).State = System.Data.Entity.EntityState.Modified;
                        Db.SaveChanges();
                    }
                }
                ModelState.AddModelError(string.Empty, "Updated");
            }

            if (!string.IsNullOrEmpty(dashboardMV.ProfileMV.EducationLevel))
            {
                userdetail.EducationLevel = dashboardMV.ProfileMV.EducationLevel;
                Db.Entry(userdetail).State = System.Data.Entity.EntityState.Modified;
                Db.SaveChanges();
            }
            if (!string.IsNullOrEmpty(dashboardMV.ProfileMV.ExperenceLevel))
            {
                userdetail.ExperenceLevel = dashboardMV.ProfileMV.ExperenceLevel;
                Db.Entry(userdetail).State = System.Data.Entity.EntityState.Modified;
                Db.SaveChanges();
            }
            if (dashboardMV.ProfileMV.EducationLastDegreePhoto != null)
            {
                var folder = "~/Content/OtherFiles";
                var photoname = string.Format("Education_{0}.jpg", userid);
                var response = HelperClass.FileUpload.UploadPhoto(dashboardMV.ProfileMV.EducationLastDegreePhoto, folder, photoname);
                if (response)
                {
                    var photo = string.Format("{0}/{1}", folder, photoname);
                    if (userdetail == null)
                    {
                        userdetail = new UserDetailTable();
                        userdetail.UserDetailID = userid;
                        userdetail.UserID = userid;
                        userdetail.CreatedBy_UserID = userid;
                        userdetail.UserDetailProvideDate = DateTime.Now;
                        userdetail.EducationLastDegreeScanPhotoPath = photo;
                        Db.UserDetailTables.Add(userdetail);
                        Db.SaveChanges();
                    }
                    else
                    {
                        userdetail.EducationLastDegreeScanPhotoPath = photo;
                        userdetail.UserDetailProvideDate = DateTime.Now;
                        Db.Entry(userdetail).State = System.Data.Entity.EntityState.Modified;
                        Db.SaveChanges();
                    }
                }
            }
            if (dashboardMV.ProfileMV.ExperenceLastPhoto != null)
            {
                var folder = "~/Content/OtherFiles";
                var photoname = string.Format("Experience_{0}.jpg", userid);
                var response = HelperClass.FileUpload.UploadPhoto(dashboardMV.ProfileMV.ExperenceLastPhoto, folder, photoname);
                if (response)
                {
                    var photo = string.Format("{0}/{1}", folder, photoname);
                    if (userdetail == null)
                    {
                        userdetail = new UserDetailTable();
                        userdetail.UserDetailID = userid;
                        userdetail.UserID = userid;
                        userdetail.CreatedBy_UserID = userid;
                        userdetail.UserDetailProvideDate = DateTime.Now;
                        userdetail.LastExperenceScanPhotoPath = photo;
                        Db.UserDetailTables.Add(userdetail);
                        Db.SaveChanges();
                    }
                    else
                    {
                        userdetail.LastExperenceScanPhotoPath = photo;
                        userdetail.UserDetailProvideDate = DateTime.Now;
                        Db.Entry(userdetail).State = System.Data.Entity.EntityState.Modified;
                        Db.SaveChanges();
                    }
                }
            }
            return View(dasboard);
        }



    }
}