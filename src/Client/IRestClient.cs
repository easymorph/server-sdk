﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Morph.Server.Sdk.Model.InternalModels;

namespace Morph.Server.Sdk.Client
{
    internal interface IRestClient : IDisposable
    {
        HttpClient HttpClient { get; set; }
        Task<ApiResult<TResult>> GetAsync<TResult>(string url, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);
        Task<ApiResult<TResult>> HeadAsync<TResult>(string url, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);
        Task<ApiResult<TResult>> PostAsync<TModel, TResult>(string url, TModel model, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);
        Task<ApiResult<TResult>> PutAsync<TModel, TResult>(string url, TModel model, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);
        Task<ApiResult<TResult>> DeleteAsync<TResult>(string url, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);

        Task<ApiResult<TResult>> PutFileStreamAsync<TResult>(string url, SendFileStreamData sendFileStreamData, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);
        Task<ApiResult<TResult>> PostFileStreamAsync<TResult>(string url, SendFileStreamData sendFileStreamData, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);

        Task<ApiResult<FetchFileStreamData>> RetrieveFileGetAsync(string url, NameValueCollection urlParameters, HeadersCollection headersCollection, CancellationToken cancellationToken);

    }



}