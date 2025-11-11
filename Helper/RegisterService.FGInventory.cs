using erpsolution.entities;
using erpsolution.service.Interface;
using erpsolution.service.FGInventoryMobile;
namespace erpsolution.api.Helper
{
    public static partial class RegisterService
    {

        public static IServiceCollection AddRegisterServiceFGInventory(this IServiceCollection services)
        {
           
            //FGInventory Mobile
            services.AddScoped<IAmtAuthService, AmtAuthService>();
            services.AddScoped<IFGInventoryService, FGInventoryService>();

            //Add Helper
         //   services.AddScoped<IExternalDatabaseQueries, ExternalDatabaseQueries>();


            return services;
        }
    }
}
