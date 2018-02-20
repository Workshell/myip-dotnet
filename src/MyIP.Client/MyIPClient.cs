using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyIP
{
    public interface IMyIPClient
    {
        #region Methods

        MyIPResponse Get();
        Task<MyIPResponse> GetAsync();

        #endregion

        #region Properties

        bool IPv4 { get; set; }
        bool IPv6 { get; set; }

        #endregion
    }

    public sealed class MyIPClient : IMyIPClient
    {
        private static readonly HttpClient _httpClient;

        private readonly string _ipv4Url;
        private readonly string _ipv6Url;

        private int _timeout;
        private bool _ipv4;
        private bool _ipv6;

        static MyIPClient()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyIP/Client 1.0");
        }

        public MyIPClient(string ipv4Url = null, string ipv6Url = null)
        {
            _ipv4Url = string.IsNullOrWhiteSpace(ipv4Url) ? "https://api4.my-ip.io/ip" : ipv4Url;
            _ipv6Url = string.IsNullOrWhiteSpace(ipv6Url) ? "https://api6.my-ip.io/ip" : ipv6Url;

            _timeout = 20;
            _ipv4 = true;
            _ipv6 = Socket.OSSupportsIPv6;
        }

        #region Methods

        public MyIPResponse Get()
        {
            return GetAsync().GetAwaiter().GetResult();
        }

        public async Task<MyIPResponse> GetAsync()
        {
            var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromSeconds(_timeout));

            IPAddress ipv4 = null;
            IPAddress ipv6 = null;

            try
            {
                var tasks = new Queue<Task>();

                if (_ipv4)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var response = await _httpClient.GetAsync(_ipv4Url, cts.Token);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();

                                if (IPAddress.TryParse(content, out var value) && value.AddressFamily == AddressFamily.InterNetwork)
                                    ipv4 = value;

                            }
                        }
                        catch
                        {
                            // Squash
                        }
                    }, cts.Token);

                    tasks.Enqueue(task);
                }

                if (_ipv6 && Socket.OSSupportsIPv6)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var response = await _httpClient.GetAsync(_ipv6Url, cts.Token);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();

                                if (IPAddress.TryParse(content, out var value) && value.AddressFamily == AddressFamily.InterNetworkV6)
                                    ipv6 = value;
                            }
                        }
                        catch
                        {
                            // Squash
                        }
                    }, cts.Token);

                    tasks.Enqueue(task);
                }

                await Task.WhenAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                if (!(ex is TaskCanceledException || ex is OperationCanceledException))
                    throw;
            }

            var result = new MyIPResponse(ipv4, ipv6);

            return result;
        }

        #endregion

        #region Properties

        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value <= 1)
                {
                    _timeout = 1;
                }
                else
                {
                    _timeout = value;
                }
            }
        }

        public bool IPv4
        {
            get => _ipv4;
            set => _ipv4 = value;
        }

        public bool IPv6
        {
            get => _ipv6;
            set => _ipv6 = value;
        }

        #endregion
    }
}
