using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwitterWebApp1.Data;

namespace TwitterWebApp1.Controllers
{
    public class BaseController : Controller 
    {
        protected readonly ILogger<BaseController> logger;
        protected readonly AppDBContext context;
        protected readonly UserManager<AppUser> userManager;
        protected readonly SignInManager<AppUser> signInManager;

        public BaseController(ILogger<BaseController> logger, AppDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
    }

}