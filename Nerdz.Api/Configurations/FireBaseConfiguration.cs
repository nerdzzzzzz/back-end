using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nerdz.Domain.Authorization;

namespace Nerdz.Api.Configurations
{
    public static class FireBaseConfiguration
    {
        public static void ConfigureFireBase(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;

            var projectId = config["Firebase:ProjectId"];
            var credentialPath = config["Firebase:CredentialPath"];

            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(credentialPath))
            {
                throw new InvalidOperationException("As configurações 'Firebase:ProjectId' e 'Firebase:CredentialPath' são obrigatórias.");
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = projectId,
            });

            builder.Services.AddSingleton(provider => FirestoreDb.Create(projectId));
            builder.Services.AddSingleton(provider => FirebaseAuth.DefaultInstance);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://securetoken.google.com/{projectId}";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"https://securetoken.google.com/{projectId}",
                        ValidateAudience = true,
                        ValidAudience = projectId,
                        ValidateLifetime = true,
                        RoleClaimType = AppClaimTypes.Role
                    };
                });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPolicies.AdminOnly, policy =>
                    policy.RequireRole(AppRoles.Admin));

                options.AddPolicy(AppPolicies.PremiumUser, policy =>
                    policy.RequireClaim(AppClaimTypes.Premium, "true"));
            });
        }
    }
}
