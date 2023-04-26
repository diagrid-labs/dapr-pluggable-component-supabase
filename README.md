# Dapr pluggable Supabase state store component

A demo [Dapr](https://docs.dapr.io/) pluggable component that uses Supabase as the state store.

This demo consists of a C#/.NET 7 application that implements a Dapr [pluggable component](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-overview/) capable of using [Supabase Tables](https://supabase.com/docs/guides/database/tables) for the state store. To test the pluggable component, the [state management HTTP API](https://docs.dapr.io/reference/api/state_api/) is used.

![Calling the State Store API ](images/dapr-supabase-pluggable-v2.png)

Please read the [blog post](https://www.diagrid.io/blog/dapr-supabase-component) that accompanies this repo for more information.

## Prerequisites

1. [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
3. [Supabase account](https://supabase.io/)
4. Supported OS: macOS, Linux, [WSL](https://learn.microsoft.com/en-us/windows/wsl/install) on Windows

## Supabase Setup

1. Using the Supabase dashboard, create a new project.
2. Create a new database table with the following specifications:
    - Name: `dapr_state_store`
    - Primary Key: `id`, (int8, not null)
    - Columns:
        - `created_at` (timestamptz, not null)
        - `key` (text, not null)
        - `value` (text, null)
    - RLS (Row Level Security) is disabled

        > Note that for production use, [RLS](https://supabase.com/docs/learn/auth-deep-dive/auth-row-level-security) should be enabled, and access policies should be added to the table.

    *Table definition:*

    ```sql
    create table
        public.dapr_state_store (
            id bigint generated by default as identity not null,
            created_at timestamp with time zone not null default now(),
            key text not null,
            value text null,
            constraint dapr_state_store_pkey primary key (id),
            constraint dapr_state_store_id_key unique (id)
        ) tablespace pg_default;
    ```

You'll need the Supabase project URL and public API key to configure the Dapr component file in the next section. This information is found in the Supabase portal under the `Settings > API` tab.

## Update the Dapr pluggable Supabase component file

Dapr uses a modular design where functionality is delivered as a [component](https://docs.dapr.io/concepts/components-concept/). A component file contains the specification of a component, including the name, the component type, and related metadata that is specific to connecting with the underlying resource. The component file for the Supabase state store looks like this:

```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pluggable-supabase
spec:
  type: state.supabase
  version: v1
  metadata:
  - name: projectUrl
    value: ""
  - name: projectApiKey
    value: ""
```

The value of the `spec.type` field, `state.supabase`, consists of two parts: the component type (`state`), and the socket name (`supabase`). The socket name needs to match with the socket name argument provided in the `RegisterService` method in the `Program.cs` class of section 2. A template of this component file is available in the repository, follow these steps to update the file, so it can be used locally.

1. Navigate to the `resources` folder in this repository.
2. Rename the `resources/pluggableSupabase.yml.template` file to `resources/pluggableSupabase.yml`.

   > The `pluggableSupabase.yml` file is added to .gitignore so it won't be accidentally committed to source control for this demo app. For production use, the yaml files **should** be checked into source control and [secret store references](https://docs.dapr.io/operations/components/component-secrets/) should be used, instead of plain text values.

3. Open the `pluggableSupabase.yml` file and update the values for `projectUrl` and `projectApiKey`.
4. Save the file and copy it to the user's Dapr components folder: `~/.dapr/components`.

   > When the Dapr CLI is run, all the component files in this folder will be loaded, so the pluggable Supabase component should be available.

## Run the DaprPluggableSupabase service

Regular Dapr components are part of the Dapr runtime and don't require additional processes to run. Pluggable components however, are not part of the Dapr runtime and need to be started separately, which is done in this section.

The `DaprPluggableSupabase` project in this repo implements the Dapr state store interface and uses the [Supabase C# library](https://github.com/supabase-community/supabase-csharp) to access a Supabase table.

1. Open a terminal and navigate to the `src/DaprPluggableSupabase` folder.
2. Build the project:

    ```bash
    dotnet build
    ```

3. Run the project:

    ```bash
    dotnet run
    ```

## Run the Dapr process and test the Supabase state store

1. Open a new terminal and use the Dapr CLI to run the Dapr process

    ```bash
    dapr run --app-id myapp --dapr-http-port 3500
    ```

    *Expected output:*

    The output should contain an INFO message that the pluggable-supabase component is loaded:

    ```bash
    INFO[0000] component loaded. name: pluggable-supabase, type: state.supabase/v1 
    ```

    The log should end with:

    ```bash
    ℹ️  Dapr sidecar is up and running.
    ✅  You're up and running! Dapr logs will appear here.
    ```

2. Set a new state by making a POST request:

    ```bash
    curl --request POST --url http://localhost:3500/v1.0/state/pluggable-supabase --header 'content-type: application/json' --data '[{"key": "key1","value": "This is stored in Supabase!"}]'
    ```

    *Expected output:*

    ```http
    HTTP 204 No Content
    ```

3. Retrieve the new state using a GET request:

    ```bash
    curl --request GET --url http://localhost:3500/v1.0/state/pluggable-supabase/key1
    ```

    *Expected output:*

    ```http
    HTTP 200 OK

    "This is stored in Supabase!"
    ```

    Or have a look in the Supabase dashboard to see the new state record.

🎉 Congratulations! You've successfully used the Dapr pluggable Supabase state store component. 🎉

## Resources

- [Dapr Pluggable Components Overview](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-overview/)
- [Dapr Pluggable Components .NET SDK](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-sdks/pluggable-components-dotnet/)
- [Supabase C# SDK](https://supabase.com/docs/reference/csharp/installing)
- [Postgrest-csharp library](https://github.com/supabase-community/postgrest-csharp)