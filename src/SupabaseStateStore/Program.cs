using Dapr.PluggableComponents;
using SupabaseStateStore;

var builder = WebApplication.CreateBuilder(args);
var app = DaprPluggableComponentsApplication.Create();

app.RegisterService(
    "pluggable-supabase",
    serviceBuilder =>
    {
        serviceBuilder.RegisterStateStore<SupabaseService>();
    });

app.Run();
