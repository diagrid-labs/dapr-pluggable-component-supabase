using Dapr.PluggableComponents.Components;
using Dapr.PluggableComponents.Components.StateStore;

namespace DaprPluggableSupabase
{
    internal sealed class SupabaseBucketStateStore : IStateStore
    {
        private Supabase.Client _supabaseClient;
        private const string PROJECT_APIKEY_KEYWORD = "projectApiKey";
        private const string PROJECT_URL_KEYWORD = "projectUrl";
        private const string BUCKET_NAME = "dapr_state_store";

        public async Task DeleteAsync(StateStoreDeleteRequest request, CancellationToken cancellationToken = default)
        {
            await _supabaseClient.From<KeyValue>()
                .Where(x => x.Key == request.Key)
                .Delete(cancellationToken: cancellationToken);
        }

        public async Task<StateStoreGetResponse?> GetAsync(StateStoreGetRequest request, CancellationToken cancellationToken = default)
        {
            byte[] file = await GetFile(request.Key);

            var response = new StateStoreGetResponse() { Data = file };
            return response;
        }

        public async Task InitAsync(MetadataRequest request, CancellationToken cancellationToken = default)
        {
            _supabaseClient = new Supabase.Client(request.Properties[PROJECT_URL_KEYWORD], request.Properties[PROJECT_APIKEY_KEYWORD]);
            await _supabaseClient.InitializeAsync();
        }

        public async Task SetAsync(StateStoreSetRequest request, CancellationToken cancellationToken = default)
        {
            byte[] existingFile = await GetFile(request.Key);

            if (existingFile.Length > 0)
            {
                await _supabaseClient.Storage.From(BUCKET_NAME).Update(request.Value.Span.ToArray(), request.Key);
                return;
            }
            else
            {
                await _supabaseClient.Storage.From(BUCKET_NAME).Upload(request.Value.Span.ToArray(), request.Key);
            }
        }
        private async Task<byte[]> GetFile(string key)
        {
            return await _supabaseClient.Storage.From(BUCKET_NAME).Download(key);
        }
    }
}