# Dapr pluggable state store component for Supabase

A prototype Dapr pluggable state store component that uses Supabase Tables.

## Prerequisites

1. [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
3. [Supabase account](https://supabase.io/)

## Supabase Setup

1. Create a new Supabase project.
2. Create a new database table with the following specifications:
    - Name: `dapr_state_store`
    - Primary Key: `id`, (int8, not null)
    - Columns:
        - `created_at` (timestamptz, not null)
        - `key` (text, not null)
        - `value` (text, null)

You'll need the Supabase project URL and public API key to configure the Dapr component in the next section. This information is found in the Supabase portal under the `Settings > API` tab.

## Run the SupabaseStateStore project

1. Open a terminal, clone this repo locally and navigate to the `src/SupabaseStateStore` folder.
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

2. Set a new state via curl:

    ```bash
    ```

3. Retrieve the new state via curl:

    ```bash
    ```

## Resources

- [Dapr Pluggable Components Overview](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-overview/)
- [Dapr Pluggable Components .NET SDK](https://docs.dapr.io/developing-applications/develop-components/pluggable-components/pluggable-components-sdks/pluggable-components-dotnet/)
- 