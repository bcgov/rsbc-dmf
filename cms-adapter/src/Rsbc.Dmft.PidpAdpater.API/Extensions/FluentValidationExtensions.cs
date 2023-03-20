using FluentValidation;
using System.Security.Claims;

namespace MedicalPortal.API.Extensions;
public static class FluentValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, string?> MatchesUserClaim<T>(this IRuleBuilder<T, string?> ruleBuilder, ClaimsPrincipal? user, string claimType)
    {
        return ruleBuilder.Custom((property, context) =>
        {
            if (user == null)
            {
                context.AddFailure("No User found");
                return;
            }

            if (property != user.FindFirstValue(claimType))
            {
                context.AddFailure($"Must match the \"{claimType}\" Claim on the current User");
            }
        });
    }
}
