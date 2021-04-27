using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RepairsApi.V2.Authorisation
{
    public static class ClaimsPrincipalExtension
    {
        public static string Name(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
        }
        public static string Email(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.Email).Value;
        }
        public static List<string> Groups(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindAll(CustomClaimTypes.Contractor).Select(c => c.Value).ToList();
        }

        public static string VaryLimit(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(CustomClaimTypes.VaryLimit).Value;
        }

        public static string RaiseLimit(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(CustomClaimTypes.RaiseLimit).Value;
        }

        public static string Sub(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.PrimarySid).Value;
        }

    }
}
