using Microsoft.AspNetCore.Authorization;

namespace NetCoreIdentityApp.Web.Requirements
{
    public class ViolenceRequirement : IAuthorizationRequirement
    {
        public int ThreshOldAge { get; set; }
    }


    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {

            var hasExchangeExpireDate = context.User.HasClaim(x => x.Type == "birthdate");

            if (!hasExchangeExpireDate)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var birthdateClaim = context.User.FindFirst("birthdate");

            var today = DateTime.Now;
            var birthdate = Convert.ToDateTime(birthdateClaim.Value);
            var age = today.Year - birthdate.Year;

            if (birthdate > today.AddYears(-age)) --age;
            

            if (requirement.ThreshOldAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
