using System.Text;
using Dapr.PluggableComponents.Components;
using Dapr.PluggableComponents.Components.StateStore;

namespace DaprPluggableSupabase
{
    internal sealed class SupabaseStateStore : IStateStore
    {
#nullable disable
        private Supabase.Client _supabaseClient;
#nullable enable

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
            KeyValue? kv = await GetKV(request.Key, cancellationToken);

            if (kv != null && kv.Value != null)
            {
                var valueAsBytes = Encoding.UTF8.GetBytes(kv.Value);
                var response = new StateStoreGetResponse() { Data = valueAsBytes };
                return response;
            }

            return new StateStoreGetResponse();
        }


        public async Task InitAsync(MetadataRequest request, CancellationToken cancellationToken = default)
        {
            if (!request.Properties.TryGetValue(PROJECT_URL_KEYWORD, out string? projectUrl))
            {
                throw new InvalidOperationException($"Missing required property \"{PROJECT_URL_KEYWORD}\" in component file.");
            }

            if (!request.Properties.TryGetValue(PROJECT_APIKEY_KEYWORD, out string? projectApiKey))
            {
                throw new InvalidOperationException($"Missing required property \"{PROJECT_APIKEY_KEYWORD}\" in component file.");
            }

            _supabaseClient = new Supabase.Client(projectUrl, projectApiKey);
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

            KeyValue? existingKV = await GetKV(request.Key, cancellationToken);

            if (existingKV != null)
            {
                newKV.Id = existingKV.Id;
                await _supabaseClient.From<KeyValue>().Update(newKV, cancellationToken: cancellationToken);
                return;
            }
            else
            {
                await _supabaseClient.From<KeyValue>().Insert(newKV, cancellationToken: cancellationToken);
            }
        }

        private async Task<KeyValue?> GetKV(string key, CancellationToken cancellationToken)
        {
            return await _supabaseClient.From<KeyValue>()
#nullable disable
                            .Select(x => new object[] { x.Id, x.Key, x.Value })
#nullable enable
                            .Where(x => x.Key == key)
                            .Single(cancellationToken);
        }
    }
}