using TrainingCenter.Api.Infrastructure.Data;
using TrainingCenter.Api.Infrastructure.Mail;
using TrainingCenter.Api.Services.Leads;
using TrainingCenter.Api.Services.Pricing;

namespace TrainingCenter.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPricing(this IServiceCollection services)
    {
        // Все правила попадают в PriceCalculator как IEnumerable<IDiscountRule>;
        // порядок регистрации не важен — калькулятор сам выбирает максимальную скидку.
        services.AddSingleton<IDiscountRule, HotGroupDiscountRule>();
        services.AddSingleton<IDiscountRule, GraduateDiscountRule>();
        services.AddSingleton<IPriceCalculator, PriceCalculator>();
        services.AddScoped<ICartService, CartService>();

        services.AddSingleton<ICourseRepository, InMemoryCourseRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        return services;
    }

    public static IServiceCollection AddLeads(this IServiceCollection services, IConfiguration configuration)
    {
        // Ошибка в адресах обнаружится при старте приложения, а не на первой заявке.
        services.AddOptions<LeadRoutingOptions>()
            .Bind(configuration.GetSection(LeadRoutingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.Configure<CorporateMailOptions>(configuration.GetSection(CorporateMailOptions.SectionName));

        services.AddSingleton<InMemoryMailbox>();
        services.AddScoped<IMailService, CorporateMailService>();
        services.AddScoped<IBackupMailService, BackupMailService>();
        services.AddScoped<ILeadService, LeadService>();

        return services;
    }
}
