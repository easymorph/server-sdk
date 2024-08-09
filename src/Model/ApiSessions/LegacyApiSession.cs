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



        /// <summary>
        /// Optional client that allows the session on session object dispose
        /// </summary>
        ICanCloseSession _optionalClient;
        private string _spaceName;

        private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public string SpaceName => _spaceName;

        /// <summary>
        /// Api session constructor
        /// </summary>
        /// <param name="client">reference to client </param>
        internal LegacyApiSession(Guid  localIdentifier, string spaceName, string authToken) :
            base(localIdentifier, authToken ?? throw new ArgumentException("AuthToken must be set"))
        {
            
            IsClosed = false;

            if (string.IsNullOrEmpty(spaceName))
            {
                _spaceName = _defaultSpaceName;
            }
            else
            {
                _spaceName = spaceName.ToLowerInvariant().Trim();
            };

        }

        /// <summary>
        /// Sets optional client that allows session close
        /// </summary>
        /// <param name="optionalClient"></param>
        public virtual void SetClient(ICanCloseSession optionalClient)
        {
            _optionalClient = optionalClient;
        }


        /// <summary>
        ///     Import authentication data from other token
        /// </summary>
        /// <param name="freshSession">Session to import from</param>
        /// <exception cref="ArgumentNullException"><see cref="freshSession"/> is null</exception>
        public void FillFrom(ApiSession freshSession)
        {
            if (freshSession == null) throw new ArgumentNullException(nameof(freshSession));

            AuthToken = freshSession.AuthToken;
            LocalIdentifier = freshSession.LocalIdentifier;
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
            if (IsClosed || _optionalClient == null || IsAnonymous)
            {
                return;
            }
            try
            {

                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    linkedCts.CancelAfter(TimeSpan.FromSeconds(5));
                    await _optionalClient.CloseSessionAsync(this, linkedCts.Token);

                    // don't dispose client implicitly, just remove link to client
                    if (_optionalClient.Config.AutoDisposeClientOnSessionClose)
                    {
                        _optionalClient.Dispose();
                    }
                    _optionalClient = null;
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
                        if (!IsClosed && _optionalClient != null)
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


