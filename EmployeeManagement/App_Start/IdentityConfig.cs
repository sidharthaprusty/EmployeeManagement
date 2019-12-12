using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using EmployeeManagement.Models;
using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EmployeeManagement
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            var client = new SendGridClient(ConfigurationManager.AppSettings["mailClient"]);
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            var To = new EmailAddress(message.Destination, "Example User");
            myMessage.From = new EmailAddress("siddharth.prusty@gmail.com", "Welcome Aboard!!!");
            myMessage.Subject = message.Subject;
            myMessage.PlainTextContent = message.Body;
            myMessage.HtmlContent = message.Body;
            var msg = MailHelper.CreateSingleEmail(myMessage.From, To, myMessage.Subject, myMessage.PlainTextContent, myMessage.HtmlContent);
            
            // Send the email.
            if (client != null)
            {
                await client.SendEmailAsync(msg);
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                await Task.FromResult(0);
            }

        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
            var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
            var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

            try
            {
                TwilioClient.Init(accountSid, authToken);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                | SecurityProtocolType.Tls11
                                                | SecurityProtocolType.Tls12
                                                | SecurityProtocolType.Ssl3;
                MessageResource result = MessageResource.Create(
                new PhoneNumber(message.Destination),
                from: new PhoneNumber(fromNumber),
                body: message.Body
                );
                //Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
                Trace.TraceInformation(result.Status.ToString());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            //Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(2);
            manager.MaxFailedAccessAttemptsBeforeLockout = 3;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromMinutes(2)
                    };

            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // Identity Role
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole,string> roleStore): base(roleStore) { }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var applicationRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));

            return applicationRoleManager;
        }
    }
}
