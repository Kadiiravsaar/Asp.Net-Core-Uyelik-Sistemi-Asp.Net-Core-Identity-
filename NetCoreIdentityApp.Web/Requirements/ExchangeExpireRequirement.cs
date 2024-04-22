using Microsoft.AspNetCore.Authorization;

namespace NetCoreIdentityApp.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
    }


    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            var hasExchangeExpireDate = context.User.HasClaim(x => x.Type == "ExchangeExpireDate");

            if (!hasExchangeExpireDate)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate");

            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value) ) 
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);

            return Task.CompletedTask;



        }
    }
}
