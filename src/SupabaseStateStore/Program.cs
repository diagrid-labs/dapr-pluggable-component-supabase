using Dapr.PluggableComponents;
using SupabaseStateStore;

var builder = WebApplication.CreateBuilder(args);
var app = DaprPluggableComponentsApplication.Create();

app.RegisterService(
    "supabase",
    serviceBuilder =>
    {
        serviceBuilder.RegisterStateStore<SupabaseService>();
    });

app.Run();
