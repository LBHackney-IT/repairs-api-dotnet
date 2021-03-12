using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Services;

namespace RepairsApi.V2.Authorisation
{
    public static class CurrentUserServiceExtension
    {
        public static HubUserModel GetHubUser(this ICurrentUserService currentUserService)
        {
            var hubUser = new HubUserModel();
            var _user = currentUserService.GetUser();

            hubUser.Sub = _user.Sub();
            hubUser.Email = _user.Email();
            hubUser.Name = _user.Name();

            double number;
            if (double.TryParse(_user.FindFirst(CustomClaimTypes.RAISELIMIT)?.Value, out number))
                hubUser.RaiseLimit = number.ToString();

            if (double.TryParse(_user.FindFirst(CustomClaimTypes.VARYLIMIT)?.Value, out number))
                hubUser.VaryLimit = number.ToString();

            return hubUser;
        }

    }
}
