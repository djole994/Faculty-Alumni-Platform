using AlumniApi.Options;
using AlumniApi.Services.Email;

namespace AlumniApi.Helpers.Email
{
    public static class EmailingExtensions
    {
        public static IServiceCollection AddEmailing(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<EmailOptions>().Bind(config.GetSection(EmailOptions.SectionName));
            services.AddOptions<SmtpOptions>().Bind(config.GetSection(SmtpOptions.SectionName));

            services.AddTransient<IEmailSender, SmtpEmailSender>();
            services.AddScoped<IEmailOutboxQueue, EmailOutboxQueue>();
            services.AddHostedService<EmailOutboxWorker>();

            return services;
        }
    }

}
