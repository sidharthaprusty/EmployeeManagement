using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;

namespace EmployeeManagement.BusinessLogic
{
    public class EmployeeBusinessLogic
    {
        string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        public IEnumerable<ApplicationUser> Users
        {
            get
            {
                List<ApplicationUser> Users = new List<ApplicationUser>();
                using (SqlConnection conObj = new SqlConnection(conStr))
                {
                    SqlCommand cmdObj = new SqlCommand("selectUsers",conObj);
                    conObj.Open();

                    SqlDataReader readerObj = cmdObj.ExecuteReader();

                    while (readerObj.Read())
                    {
                        ApplicationUser User = new ApplicationUser();
                        User.Id = readerObj["Id"].ToString();
                        User.Email = readerObj["Email"].ToString();
                        User.EmailConfirmed = (bool)readerObj["EmailConfirmed"];
                        User.PhoneNumber = readerObj["PhoneNumber"].ToString();
                        User.FirstName = readerObj["FirstName"].ToString();
                        User.LastName = readerObj["LastName"].ToString();
                        User.Gender = readerObj["Gender"].ToString();
                        User.DOB = (DateTime)readerObj["DOB"];
                        User.State = readerObj["State"].ToString();
                        User.Country = readerObj["Country"].ToString();

                        Users.Add(User);
                    }
                }
                return Users;   
            }
            
        }

        
    }
}