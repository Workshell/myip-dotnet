using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyIP
{
    public sealed class MyIPResponse
    {
        internal MyIPResponse(IPAddress ipv4, IPAddress ipv6)
        {
            IPv4Address = ipv4;
            IPv6Address = ipv6;
        }

        #region Properties

        public IPAddress IPv4Address { get; }
        public IPAddress IPv6Address { get; }

        #endregion
    }
}
