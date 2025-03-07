﻿using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Morph.Server.Sdk.Client;

namespace Morph.Server.Sdk.Model
{
    public class ClientConfiguration : IClientConfiguration
    {

        public TimeSpan OperationTimeout { get; set; } = MorphServerApiClientGlobalConfig.OperationTimeout;
        public TimeSpan FileTransferTimeout { get; set; } = MorphServerApiClientGlobalConfig.FileTransferTimeout;
        public TimeSpan HttpClientTimeout { get; set; } = MorphServerApiClientGlobalConfig.HttpClientTimeout;
        public TimeSpan SessionOpenTimeout { get; set; } = MorphServerApiClientGlobalConfig.SessionOpenTimeout;

        public string ClientId { get; set; } = MorphServerApiClientGlobalConfig.ClientId;
        public string ClientType { get; set; } = MorphServerApiClientGlobalConfig.ClientType;

        public bool AutoDisposeClientOnSessionClose { get; set; } = MorphServerApiClientGlobalConfig.AutoDisposeClientOnSessionClose;

        public ILegacyApiSessionRefresher LegacySessionRefresher { get; set; } = AppWideLegacyApiSessionRefresher.Instance;

        public Uri ApiUri { get; set; }
        public HttpSecurityState HttpSecurityState { get; set; } = HttpSecurityState.NotEvaluated;
        internal string SDKVersionString { get; set; } = MorphServerApiClientGlobalConfig.SDKVersionString;

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { get; set; }
        = MorphServerApiClientGlobalConfig.ServerCertificateCustomValidationCallback;
    }

}