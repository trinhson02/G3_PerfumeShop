using G3_PerfumeShop.Models;

namespace G3_PerfumeShop.Service
{
    public interface ISessionService
    {
        void SetUserSession(User user);
        void ClearUserSession();
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetUserSession(User user)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            httpContext.Session.SetString("Username", user.Username);
            httpContext.Session.SetString("yourName", user.LastName);
            httpContext.Session.SetInt32("UserId", user.Id);
            httpContext.Session.SetInt32("StatusId", user.StatusId);
            httpContext.Session.SetInt32("RoleId", user.RoleId);
        }

        public void ClearUserSession()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Session.Clear();
        }
    }

}
