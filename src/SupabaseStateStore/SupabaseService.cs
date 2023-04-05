using System.Text;
using Dapr.PluggableComponents.Components;
using Dapr.PluggableComponents.Components.StateStore;

namespace SupabaseStateStore
{
    internal sealed class SupabaseService : IStateStore
    {
        private Supabase.Client _supabaseClient;
        private const string PROJECT_APIKEY_KEYWORD = "projectApiKey";
        private const string PROJECT_URL_KEYWORD = "projectUrl";

        public async Task DeleteAsync(StateStoreDeleteRequest request, CancellationToken cancellationToken = default)
        {
            await _supabaseClient.From<KeyValue>()
                .Where(x => x.Key == request.Key)
                .Delete(cancellationToken: cancellationToken);
        }

        public async Task<StateStoreGetResponse?> GetAsync(StateStoreGetRequest request, CancellationToken cancellationToken = default)
        {
            var kv = await _supabaseClient.From<KeyValue>()
                .Select(x => new object[] { x.Key, x.Value })
                .Where(x => x.Key == request.Key)
                .Single(cancellationToken);

            if (kv != null)
            {
                var valueAsBytes = Encoding.UTF8.GetBytes(kv.Value);
                var response = new StateStoreGetResponse() { Data = valueAsBytes };
                return response;
            }

            return new StateStoreGetResponse();
        }

        public async Task InitAsync(MetadataRequest request, CancellationToken cancellationToken = default)
        {
            _supabaseClient = new Supabase.Client(request.Properties[PROJECT_URL_KEYWORD], request.Properties[PROJECT_APIKEY_KEYWORD]);
            await _supabaseClient.InitializeAsync();
        }

        public async Task SetAsync(StateStoreSetRequest request, CancellationToken cancellationToken = default)
        {
            var newKV = new KeyValue
            {
                Key = request.Key,
                CreatedAt = DateTime.UtcNow,
                Value = Encoding.UTF8.GetString(request.Value.Span)
            };

            await _supabaseClient.From<KeyValue>().Insert(newKV, cancellationToken: cancellationToken);
        }
    }
}