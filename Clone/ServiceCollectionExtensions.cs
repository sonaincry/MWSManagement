using Indotalent.Applications.AdjustmentMinuss;
using Indotalent.Applications.AdjustmentPluss;
using Indotalent.Applications.ApplicationUsers;
using Indotalent.Applications.AX;
using Indotalent.Applications.Companies;
using Indotalent.Applications.Customers;
using Indotalent.Applications.LogAnalytics;
using Indotalent.Applications.LogErrors;
using Indotalent.Applications.LogSessions;
using Indotalent.Applications.Products;
using Indotalent.Applications.TaxVatNums;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Currencies;
using Indotalent.Infrastructures.Docs;
using Indotalent.Infrastructures.Emails;
using Indotalent.Infrastructures.Images;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Infrastructures.TimeZones;
using Indotalent.Models.Entities;
using Indotalent.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MWSManagement.Applications.Locations;
using MWSManagement.Applications.Lookups;
using MWSManagement.Applications.TableSyncs;

namespace Indotalent
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IEmailSender, SMTPEmailService>();
            services.AddScoped<IFileImageService, FileImageService>();
            services.AddScoped<IFileDocumentService, FileDocumentService>();
            services.AddScoped<ITimeZoneService, TimeZoneService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IAuditColumnTransformer, AuditColumnTransformer>();
            services.AddScoped<AspNetCompanyService>();
            services.AddScoped<ApplicationUserService>();
            services.AddScoped<LogErrorService>();
            services.AddScoped<LogSessionService>();
            services.AddScoped<LogAnalyticService>();
            services.AddScoped<AdjustmentMinusService>();
            services.AddScoped<TransactionDetailService>();
            services.AddScoped<SalesReportService>();
            services.AddScoped<LookupService>();
            services.AddScoped<ProductService>();
            services.AddScoped<CustomerService>();
<<<<<<< HEAD
            services.AddScoped<TaxVatNumTableService>();
            services.AddScoped<IProcedureRepository, ProcedureRepository>();
            services.AddScoped(typeof(AxCrudService<>));
=======
            services.AddScoped<LocationService>();
            services.AddScoped<TableSyncService>();
>>>>>>> 1ce99524544ee11bc1422957011437417685a5c7
            return services;
        }
    }
}
