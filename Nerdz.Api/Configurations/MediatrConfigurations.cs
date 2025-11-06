using FluentValidation;
using MediatR;
using Nerdz.Application.Common;
using Nerdz.Application.Validators;

namespace Nerdz.Api.Configurations
{
    public static class MediatrConfigurations
    {
        public static void ConfigureMediatr(this WebApplicationBuilder builder)
        {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
            });
        }

        public static void ConfigureFluentValidator(this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidatorBehavior<,>));
        }
    }
}
