using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using embedCONTROL.Models;

namespace embedCONTROL.Services
{
    public delegate void DataStoreChange(TcMenuConnection itemChanged, bool listReordered);

    public interface IDataStore
    {
        event DataStoreChange ChangeNotification;

        Task AddOrUpdateItemAsync(TcMenuConnection item);
        Task<bool> DeleteItemAsync(int localId);
        Task<IEnumerable<TcMenuConnection>> GetItemsAsync(bool forceRefresh = false);
    }
}
