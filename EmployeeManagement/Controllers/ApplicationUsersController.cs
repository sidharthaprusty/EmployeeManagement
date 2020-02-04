using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeManagement.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

namespace EmployeeManagement.Controllers
{
    //[CustomAuthorize(Roles = "Admin")]
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        // GET: ApplicationUsers
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            //string sqlproc = "";
            //sqlproc = "selectUsers";
            //using (SqlCommand command = new SqlCommand(sqlproc, con))
            //{
            //    command.CommandType = CommandType.StoredProcedure;
            //    //command.Parameters.Add("name", SqlDbType.VarChar, 50).Value = "Sid";
            //    List<ApplicationUser> data = new List<ApplicationUser>();
            //    con.Open();
            //    var result = command.ExecuteScalar();
            //    SqlDataReader dr = command.ExecuteReader();
            //    DataTable dt = new DataTable();
            //    dt.Load(dr);
            //    return View(dt);
            //}
        }

        // GET: ApplicationUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Gender,DOB,State,Country,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Gender,DOB,State,Country,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public async Task<ActionResult> ResendConfirmation(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);

            var user = await UserManager.FindByNameAsync(applicationUser.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string x = await SendEmailConfirmationTokenAsync(user.Id, "Confirmation Link");
                       
                    if(x != null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View("Error");
                }
            }

            return HttpNotFound();
        }
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        #region jqGrid
        public JsonResult SelectUsers(string sord, int page, int rows, string searchString)
        {
            //using(ApplicationDbContext db =new ApplicationDbContext())
            //{
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var results = db.Users.Select(a =>
                          new
                          {
                              a.FirstName,
                              a.LastName,
                              a.Email,
                              a.Gender,
                              a.DOB,
                              a.State,
                              a.Country,

                          });
            int totalRecords = results.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (sord.ToUpper() == "DESC")
            {
                results = results.OrderByDescending(s => s.FirstName);
                results = results.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                results = results.OrderBy(s => s.FirstName);
                results = results.Skip(pageIndex * pageSize).Take(pageSize);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                results = results.Where(m => m.FirstName.Contains(searchString));
            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = results
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
            //}

        }

        [HttpPost]
        public JsonResult CreateEmployee([Bind(Exclude = "Id")]  ApplicationUser applicationUser)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Users.Add(applicationUser);
                        db.SaveChanges();
                        return Json("Saved Successfully", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();

                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errormessage = "Error occured: " + ex.Message;
                return Json(errormessage, JsonRequestBehavior.AllowGet);
            }

        }

        //public string DeleteEmp(string Id)
        //{
        //    ApplicationUser applicationUser = db.Users.Find(Id);
        //    db.Users.Remove(applicationUser);
        //    db.SaveChanges();
        //    return "Deleted successfully";
        //}

        #endregion jqGrid

        public void DummyMethod()
        {
            // It does nothing!!!
            // Nothing!!!!
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
