using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace RepairsApi.Tests.Helpers
{
    public static class ControllerExtensions
    {
        public static void SetUser(this ControllerBase controller, ClaimsPrincipal user)
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(h => h.User).Returns(user);
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = httpContext.Object;

            controller.ControllerContext = controllerContext;
        }
    }
}
