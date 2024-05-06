using Serilog;

namespace ViennaDotNet.ObjectStore.Server
{
    public class Server
    {
        private readonly DataStore dataStore;

        public Server(DataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public string? store(byte[] data)
        {
            try
            {
                string id = dataStore.store(data);
                Log.Information($"Stored new object {id}");
                return id;
            }
            catch (DataStore.DataStoreException ex)
            {
                Log.Error("Could not store object", ex);
                return null;
            }
        }

        public byte[]? load(string id)
        {
            Log.Information($"Request for object {id}");
            try
            {
                byte[]? data = dataStore.load(id);
                if (data is null)
                    Log.Information($"Requested object {id} does not exist");

                return data;
            }
            catch (DataStore.DataStoreException ex)
            {
                Log.Error($"Could not load object {id}: {ex}");
                return null;
            }
        }

        public bool delete(string id)
        {
            Log.Information($"Request to delete object {id}");
            dataStore.delete(id);
            return true;
        }
    }
}
