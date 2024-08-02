using Morph.Server.Sdk.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model
{
    public class LegacyApiSession : ApiSession
    {

        protected readonly string _defaultSpaceName = "default";
        public bool IsClosed { get; internal set; }


        ICanCloseSession _client;
        private string _spaceName;

        private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public string SpaceName => _spaceName;

        /// <summary>
        /// Api session constructor
        /// </summary>
        /// <param name="client">reference to client </param>
        internal LegacyApiSession(ICanCloseSession client,string spaceName,  string authToken): 
            base(authToken?? throw new ArgumentException("AuthToken must be set"))
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            IsClosed = false;

            if (string.IsNullOrEmpty(spaceName))
            {
                _spaceName = _defaultSpaceName;
            }
            else
            {
                _spaceName = this._spaceName.ToLowerInvariant().Trim();
            };

    }





        public async Task CloseSessionAsync(CancellationToken cancellationToken)
        {

            await _lock.WaitAsync(cancellationToken);
            try
            {
                await _InternalCloseSessionAsync(cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task _InternalCloseSessionAsync(CancellationToken cancellationToken)
        {
            // don't close if session is already closed or anon.            
            if (IsClosed || _client == null || IsAnonymous)
            {
                return;
            }
            try
            {

                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    linkedCts.CancelAfter(TimeSpan.FromSeconds(5));
                    await _client.CloseSessionAsync(this, linkedCts.Token);

                    // don't dispose client implicitly, just remove link to client
                    if (_client.Config.AutoDisposeClientOnSessionClose)
                    {
                        _client.Dispose();
                    }
                    _client = null;

                    IsClosed = true;
                }
            }
            catch (Exception)
            {
                // 
            }
        }

        public override void Dispose()
        {
            try
            {   

                if (_lock != null)
                {
                    _lock.Wait(5000);
                    try
                    {
                        if (!IsClosed && _client != null)
                        {
                            Task.Run(async () =>
                            {
                                try
                                {
                                    await _InternalCloseSessionAsync(CancellationToken.None);
                                }
                                catch (Exception ex)
                                {

                                }
                            }).Wait(TimeSpan.FromSeconds(5));


                        }

                    }
                    finally
                    {
                        _lock.Release();
                        _lock.Dispose();
                        _lock = null;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }


}


