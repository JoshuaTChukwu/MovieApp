using GOSBackend.Handlers;
using GOSBackend.IdentityServices;
using GOSBackend.Requests;

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
        }
    }
}
