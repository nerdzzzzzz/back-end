using Google.Cloud.Firestore;

namespace Nerdz.Api.Configurations
{
    public static class FireBaseConfiguration
    {
        public static void ConfigureFireBase(this WebApplicationBuilder builder)
        {
            var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
            var credentialPath = builder.Configuration["Firebase:CredentialPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            FirestoreDb firestore = FirestoreDb.Create(firebaseProjectId);
            builder.Services.AddSingleton(firestore);
        }
    }
}
