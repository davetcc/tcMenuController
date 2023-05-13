using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcMenuControlApi.MenuItems;
using embedCONTROL.Models;

namespace embedCONTROL.Services
{
    public enum DataStoreError
    {
        NameNotUnique,
        FailedToSave,
        InvalidUniqueId
    }

    public class DataStoreException : Exception
    {
        public DataStoreError DataError { get; }

        public DataStoreException(DataStoreError error) : base(error.ToString())
        {
            DataError = error;
        }

        public DataStoreException(string message, DataStoreError error) : base(message)
        {
            DataError = error;
        }

        public DataStoreException(string message, DataStoreError error, Exception innerException) : base(message, innerException)
        {
            DataError = error;
        }
    }

    public class ConnectionDataStore : IDataStore
    {
        private Serilog.ILogger logger = Serilog.Log.Logger.ForContext<ConnectionDataStore>();

        private Dictionary<int, TcMenuConnection> _items = null;
        public readonly object _theLock = new object();
        private readonly IMenuConnectionPersister _persister;

        public event DataStoreChange ChangeNotification;

        public ConnectionDataStore(IMenuConnectionPersister persister)
        {
            _persister = persister;
        }

        public async Task AddOrUpdateItemAsync(TcMenuConnection item)
        {
            var needLoad = false;
            lock (_theLock) needLoad = _items == null;

            if (needLoad)
            {
                await GetItemsAsync(true).ConfigureAwait(true);
            }

            var structureChanged = false;
            lock (_theLock)
            {
                if (item.LocalId == -1)
                {
                    structureChanged = true;
                    item.LocalId = _items.Keys.DefaultIfEmpty(0).Max() + 1;
                }
                _items[item.LocalId] = item;
            }

            await Task.Run(() => {
                try
                {
                    _persister.Update(item);
                }
                catch(Exception ex)
                {
                    logger.Error(ex, $"Unable to add item {item.Name}");
                    throw new DataStoreException(DataStoreError.FailedToSave);
                }
            });
            ChangeNotification?.Invoke(item, structureChanged);
        }


        public async Task<bool> DeleteItemAsync(int localId)
        {
            TcMenuConnection item;
            lock (_theLock)
            {
                if (!_items.ContainsKey(localId)) return false;
                item = _items[localId];
                _items.Remove(localId);
            }

            try
            {
                await Task.Run(() =>
                {
                    _persister.DeleteNamed(localId);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Unable to remove item {localId}");
                return false;
            }

            ChangeNotification?.Invoke(item, true);
            return true;
        }

        public async Task<IEnumerable<TcMenuConnection>> GetItemsAsync(bool forceRefresh = false)
        {
            bool needReload = false;
            lock(_theLock)
            {
                if (forceRefresh || _items == null)
                {
                    _items = new Dictionary<int, TcMenuConnection>();
                    needReload = true;
                }
            }

            if(needReload)
            {
                var list = await Task.Run(() => _persister.LoadAll()).ConfigureAwait(true);

                lock (_theLock)
                {
                    foreach (var it in list)
                    {
                        _items[it.LocalId] = it;
                    }
                }
            }

            lock(_theLock)
            {
                return _items.Values.OrderBy(it => it.LocalId);
            }
        }
    }
}