using System.Globalization;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Services;

namespace RepairsApi.V2.Authorisation
{
    public static class CurrentUserServiceExtension
    {
        public static HubUserModel GetHubUser(this ICurrentUserService currentUserService)
        {
            var hubUser = new HubUserModel();
            var user = currentUserService.GetUser();

            hubUser.Sub = user.Sub();
            hubUser.Email = user.Email();
            hubUser.Name = user.Name();
            hubUser.Contractors = user.Contractors();

            if (double.TryParse(user.FindFirst(CustomClaimTypes.RaiseLimit)?.Value, out var number))
                hubUser.RaiseLimit = number.ToString(CultureInfo.InvariantCulture);

            if (double.TryParse(user.FindFirst(CustomClaimTypes.VaryLimit)?.Value, out number))
                hubUser.VaryLimit = number.ToString(CultureInfo.InvariantCulture);

            return hubUser;
        }

    }
}
