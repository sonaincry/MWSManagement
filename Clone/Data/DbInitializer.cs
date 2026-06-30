using Indotalent.Applications.ApplicationUsers;
using Indotalent.Applications.Companies;
using Indotalent.AppSettings;
using Indotalent.Data.System;
using Indotalent.DTOs;
using Indotalent.Infrastructures.Countries;
using Indotalent.Infrastructures.Currencies;
using Indotalent.Infrastructures.Images;
using Indotalent.Infrastructures.TimeZones;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Indotalent.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            IServiceProvider services
            )
        {

            var context = services.GetRequiredService<ApplicationDbContext>();
            if (context.Users.Any())
            {
                return;
            }


            var appConfig = services.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var fileImageService = services.GetRequiredService<IFileImageService>();
            var countryService = services.GetRequiredService<ICountryService>();
            var currencyService = services.GetRequiredService<ICurrencyService>();
            var timeZoneService = services.GetRequiredService<ITimeZoneService>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var companyService = services.GetRequiredService<AspNetCompanyService>();
            var applicationUserService = services.GetRequiredService<ApplicationUserService>();
           // var retailTransactionService = services.GetRequiredService<TransactionDetailService>();

            var creator = await applicationUserService.GetAll()
                .Where(x => x.UserName == appConfig.Value.DefaultAdminEmail)
                .FirstOrDefaultAsync();

            await DefaultCompany.GenerateAsync(companyService, currencyService, timeZoneService, countryService, creator);
            await DefaultRole.GenerateAsync(roleManager, appConfig);
            await DefaultUser.GenerateAsync(userManager, appConfig, fileImageService, companyService);

            //await DefaultSystemWarehouse.GenerateAsync(services);

        }

        public static DateTime[] GetRandomDays(int year, int month, int count)
        {
            Random random = new Random();
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime[] dates = new DateTime[Math.Min(count, daysInMonth)];

            for (int i = 0; i < dates.Length; i++)
            {
                dates[i] = DateTime.MinValue;
            }

            for (int i = 0; i < count; i++)
            {
                int day = random.Next(1, daysInMonth + 1);
                DateTime date = new DateTime(year, month, day);

                while (Array.IndexOf(dates, date) != -1)
                {
                    day = random.Next(1, daysInMonth + 1);
                    date = new DateTime(year, month, day);
                }

                dates[i] = date;
            }

            return dates;
        }

        public static string GetRandomString(string[] strings, Random random)
        {
            int randomIndex = random.Next(0, strings.Length);
            return strings[randomIndex];
        }
        public static double GetRandomValue(double[] targetValues, Random random)
        {
            int randomIndex = random.Next(0, targetValues.Length);
            return targetValues[randomIndex];

        }
        public static int GetRandomValue(int[] targetValues, Random random)
        {
            int randomIndex = random.Next(0, targetValues.Length);
            return targetValues[randomIndex];

        }
    }
}
