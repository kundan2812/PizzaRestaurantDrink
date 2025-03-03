﻿using dblayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PizzaRestaurantDrink.Models
{
    public class DashboardMV
    {
        Pro_PizzResturentandDrinkDbEntities1 db = new Pro_PizzResturentandDrinkDbEntities1();
        public DashboardMV()
        {
            ProfileMV = new User_ProfileMV();
        }
        public DashboardMV(int? id)
        {
            ProfileMV = new User_ProfileMV();
            var user = db.UserTables.Find(id);
            ProfileMV.UserID = user.UserID;
            ProfileMV.UserType = user.UserTypeTable.UserType;
            ProfileMV.UserTypeID = user.UserTypeID;
            ProfileMV.UserName = user.UserName;
            ProfileMV.Password = user.Password;
            ProfileMV.FirstName = user.FirstName;
            ProfileMV.LastName = user.LastName;
            ProfileMV.FullName = string.Format("{0} {1}", user.FirstName, user.LastName);
            ProfileMV.ContactNo = user.ContactNo;
            ProfileMV.GenderTitle = user.GenderTable.GenderTitle;
            ProfileMV.EmailAddress = user.EmailAddress;
            ProfileMV.RegisterationDate = user.RegisterationDate;
            var userpersonaladdress = db.UserAddressTables.Where(u => u.UserID == user.UserID).FirstOrDefault();

            ProfileMV.FullAddress = userpersonaladdress != null ? userpersonaladdress.FullAddress : string.Empty;
            ProfileMV.UserStatus = user.UserStatusTable.UserStatus;
            ProfileMV.UserStatusID = user.UserStatusID;
            ProfileMV.UserStatusChangeDate = user.UserStatusChangeDate;
            if (user.UserDetailTable != null)
            {
                ProfileMV.UserDetailProvideDate = user.UserDetailTable.UserDetailProvideDate;
                ProfileMV.PhotoPath = user.UserDetailTable.PhotoPath;
                ProfileMV.CNIC = user.UserDetailTable.CNIC;
                ProfileMV.EducationLevel = user.UserDetailTable.EducationLevel;
                ProfileMV.ExperenceLevel = user.UserDetailTable.ExperenceLevel;
                ProfileMV.EducationLastDegreePhotoPath = user.UserDetailTable.EducationLastDegreeScanPhotoPath;
                ProfileMV.ExperenceLastPhotoPath = user.UserDetailTable.LastExperenceScanPhotoPath;
            }
            GetUserAddress();
        }
        public virtual User_ProfileMV ProfileMV { get; set; }
        public virtual List<UserAddressMV>UserAddress { get; set; }

        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public void GetUserAddress()
        {
            UserAddress = new List<UserAddressMV>();
            foreach (var address in db.UserAddressTables.Where(u => u.VisibleStatusID == 1).ToList())
            {
                UserAddress.Add(new UserAddressMV()
                {
                    UserAddressID = address.UserAddressID,
                    AddressType = address.AddressTypeTable.AddressType,
                    FullAddress = address.FullAddress,
                    VisibleStatus = address.VisibleStatusTable.VisibleStatus,
                    UserName = address.UserTable.UserName
                });
            }
        }
    }
}