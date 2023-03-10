using MovieApp.Handlers;
using MovieApp.IdentityServices;
using MovieApp.Repository.Implements;
using MovieApp.Repository.Interface;
using MovieApp.Requests;

namespace GOSBackend.DI_Intallers
{
    public class ClassIntallers : IInstallers
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailHelper, EmailHelper>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILoginServices, LoginServicecs>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAPIHelper, APIHelper>();
            services.AddScoped<IUsersInterface, UsersInterface>();
        }
    }
}
