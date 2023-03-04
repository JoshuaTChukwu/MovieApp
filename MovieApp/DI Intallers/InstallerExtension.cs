using Microsoft.AspNetCore.Hosting;

namespace GOSBackend.DI_Intallers
{
    public static class InstallerExtension
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(x =>
               typeof(IInstallers).IsAssignableFrom(x) && !x.IsAbstract).ToList();

            var instanceOfInstallers = installers.Select(Activator.CreateInstance).Cast<IInstallers>().ToList();

            instanceOfInstallers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
