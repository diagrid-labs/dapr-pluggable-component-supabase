using Dapr.PluggableComponents;
using DaprPluggableSupabase;

var builder = WebApplication.CreateBuilder(args);
var app = DaprPluggableComponentsApplication.Create();

app.RegisterService(
    "supabase",
    serviceBuilder =>
    {
        serviceBuilder.RegisterStateStore<SupabaseBucketStateStore>();
    });

app.Run();
