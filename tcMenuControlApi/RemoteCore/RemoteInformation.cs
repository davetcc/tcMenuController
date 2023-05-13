using System;
using System.Collections.Generic;
using System.Text;
using tcMenuControlApi.Commands;

namespace tcMenuControlApi.RemoteCore
{
    /// <summary>
    /// Holds remote connectivity information about the remote connection
    /// </summary>
    public class RemoteInformation
    {
        public static RemoteInformation EMPTY_REMOTE_INFO = new RemoteInformation("Unknown", 0, ApiPlatform.ARDUINO, Guid.NewGuid().ToString());

        public string Name { get; }
        public ApiPlatform Platform { get; }
        public int Major { get; }
        public int Minor { get; }
        public string Uuid { get; }

        public RemoteInformation(string name, int version, ApiPlatform platform, string uuid)
        {
            Major = version / 100;
            Minor = version % 100;
            Name = name;
            Platform = platform;
            Uuid = uuid;
        }

        public override string ToString()
        {
            return $"{Name} V{Major}.{Minor} {Platform}";
        }
    }
}
