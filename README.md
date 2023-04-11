# Dapr pluggable state store component for Supabase

A prototype Dapr pluggable state store component that uses Supabase Tables.

## Prerequisites

1. [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
3. [Supabase account](https://supabase.io/)
4. Supported OS: macOS, Linux (Windows with WSL should work as well)

## Supabase Setup

1. Using the Supabase dashboard, create a new Supabase project.
2. Create a new database table with the following specifications:
    - Name: `dapr_state_store`
    - Primary Key: `id`, (int8, not null)
    - Columns:
        - `created_at` (timestamptz, not null)
        - `key` (text, not null)
        - `value` (text, null)

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

You'll need the Supabase project URL and public API key to configure the Dapr component in the next section. This information is found in the Supabase portal under the `Settings > API` tab.

## Update the Dapr Pluggable Supabase Component file

1. Clone this repo locally and navigate to the `resources` folder.
2. Rename the `resources\pluggableSupabase.yml.template` file to `resources\pluggableSupabase.yml`.

   > The `pluggableSupabase.yml` file is added to .gitignore so it won't be accidentally committed to source control for this demo app. For production use, the yaml files **should** be checked into source control and [secret store references](https://docs.dapr.io/operations/components/component-secrets/) should be used, instead of plain text values.

3. Open the `pluggableSupabase.yml` file and update the values for `projectUrl` and `projectApiKey`.
4. Save the file and copy it to the users Dapr components folder: `~/.dapr/components`.

   > When the Dapr CLI is run, the all the component files in this folder will loaded so the pluggable Supabase component should be available.

## Run the DaprPluggableSupabase project

1. Open a terminal and navigate to the `src/DaprPluggableSupabase` folder.
2. Build the project:

    ```bash
    dotnet build
    ```

3. Run the project:

    ```bash
    dotnet run
    ```

## Run the Dapr process and test the state store

1. Open a new terminal and use the Dapr CLI to run the Dapr process

    ```bash
    dapr run --app-id myapp --dapr-http-port 3500
    ```

2. Set a new state using a REST client:

    ```bash
    curl --request POST --url http://localhost:3500/v1.0/state/pluggable-supabase --header 'content-type: application/json' --data '[{"key": "key1","value": "This is stored in Supabase!"}]'
    ```

3. Retrieve the new state using a REST client:

    ```bash
    curl --request GET --url http://localhost:3500/v1.0/state/pluggable-supabase/key1
    ```

    Or have a look in the Supabase dashboard to see the new state record.

## Resources

- [Dapr Pluggable Components Overview](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-overview/)
- [Dapr Pluggable Components .NET SDK](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-sdks/pluggable-components-dotnet/)
- [Supabase C# SDK](https://supabase.com/docs/reference/csharp/installing)