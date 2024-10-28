using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using dblayer;

namespace PizzaRestaurantDrink.Models
{
    public class CRU_UserTypeMV
    {
        public CRU_UserTypeMV()
        {
            List_UserTypes = new List<UserTypeMV>();
            foreach (var usertype in new Pro_PizzResturentandDrinkDbEntities1().UserTypeTables.ToList())
            {
                List_UserTypes.Add(new UserTypeMV()
                {
                    UserTypeID = usertype.UserTypeID,
                    UserType = usertype.UserType
                });
            }
        }
        public CRU_UserTypeMV(int? id)
        {
            List_UserTypes = new List<UserTypeMV>();
            foreach (var usertype in new Pro_PizzResturentandDrinkDbEntities1().UserTypeTables.ToList())
            {
                List_UserTypes.Add(new UserTypeMV()
                {
                    UserTypeID = usertype.UserTypeID,
                    UserType = usertype.UserType
                });
            }

            var editusertype = new Pro_PizzResturentandDrinkDbEntities1().UserTypeTables.Where(u => u.UserTypeID == id).FirstOrDefault();
            if (editusertype != null)
            {
                UserTypeID = editusertype.UserTypeID;
                UserType = editusertype.UserType;
            }
            else
            {
                UserTypeID = 0;
                UserType = string.Empty;
            }
        }
        public int UserTypeID { get; set; }
        [Display(Name = "User Type")]
        [Required(ErrorMessage = "Required*")]
        public string UserType { get; set; }
        public List<UserTypeMV> List_UserTypes { get; set; }
    }
}