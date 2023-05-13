using System;
using System.Collections.Generic;
using System.Text;
using tcMenuControlApi.RemoteCore;

namespace embedCONTROL.Models
{
    public interface IConnectionConfiguration
    {
        string Name { get; }

        IRemoteController Build();

        bool Pair(PairingUpdateEventHandler handler);
    }
}
