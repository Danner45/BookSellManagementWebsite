using System.Web.Mvc;

namespace Project_64130005.Areas.Admin_64130005
{
    public class Admin_64130005AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_64130005";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_64130005_default",
                "Admin_64130005/{controller}/{action}/{id}",
                new { action = "Index", Controller = "Home", id = UrlParameter.Optional }
            );
        }
    }
}