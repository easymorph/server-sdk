using Morph.Server.Sdk.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Morph.Server.Sdk.Model
{
    /// <summary>
    /// Disposable api session
    /// </summary>
    /// 



    public class ApiSessionFactory
    {
        public static AnonymousSession CreateAnonymous()
        {   
            return new AnonymousSession()
            {
                
            };

        }

        public static LegacyApiSession CreateLegacySession(ICanCloseSession canCloseSession, string authToken)
        {
            return new LegacyApiSession(canCloseSession, authToken);
        }
    }


    public abstract class ApiSession : IDisposable
    {
        public const string AuthHeaderName = "X-EasyMorph-Auth";
        public string AuthToken { get; protected set; }

        public bool IsAnonymous { get; }
        protected ApiSession(string authToken)
        {
            IsAnonymous =string.IsNullOrEmpty(authToken);
            AuthToken = authToken;
        }

        public virtual void Dispose()
        {
            //throw new NotImplementedException();
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
        }
    }


    public class AnonymousSession: ApiSession
    {
        public AnonymousSession():base(null)
        {
         
        }
    }




    public class RobustApiSession : ApiSession
    {
        public RobustApiSession(string authToken) : base(authToken)
        {
        }
    }


    public class LegacyApiSession : ApiSession
    {

        public bool IsClosed { get; internal set; }


        ICanCloseSession _client;

        private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Api session constructor
        /// </summary>
        /// <param name="client">reference to client </param>
        public LegacyApiSession(ICanCloseSession client, string authToken): 
            base(authToken?? throw new ArgumentException("AuthToken must be set"))
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            IsClosed = false;


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
                base.Dispose();

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


