namespace Application
{
    using Application.Common.Behaviours;
    using Application.Common.Configuration;
    using Application.UseCases.Uploads.Commands;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));

            services.AddTransient<IValidator<CreateFileCommand>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UploadOptions>>();
                return new CreateFileCommandValidator(options);
            });
        }
    }
}
