﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using IntelliAbbXamarinTemplate.Constants;
using IntelliAbbXamarinTemplate.Models;

namespace IntelliAbbXamarinTemplate.Services
{
    public class ApiService : IApiService
    {
        readonly HttpClient _client;
        readonly INetworkService _networkService;
        readonly IEssentialsService _essentialsService;


        public ApiService(INetworkService networkService, IEssentialsService essentialsService)
        {
            _networkService = networkService;
            _essentialsService = essentialsService;
            _client = InitializeClient();
        }

        HttpClient InitializeClient()
        {
#if DEBUG
            //var tracer = new HttpHandlerBuilder(new ModernHttpClient.NativeMessageHandler()).Build();
            //var client = new HttpClient(tracer);
            var client = new HttpClient(new ModernHttpClient.NativeMessageHandler());
#else
            var client = new HttpClient(new ModernHttpClient.NativeMessageHandler());
#endif
            client.DefaultRequestHeaders.Add("User-Agent", "Space City Weather app (sci.writer@gmail.com)");
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept", "application/geo+json");
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("Sec-GPC", "1");

            return client;
        }

        public Task<T> Get<T>(Uri uri) where T : HttpResponseMessage
        {
            return GetAndRetryInner<T>(uri);
        }

        public Task<string> GetString(Uri uri)
        {
            return _client.GetStringAsync(uri);
        }

        public async Task<string> GetString(string apiPath)
        {
            return await _client.GetStringAsync(apiPath);
        }

        public async Task<Stream> GetStream(string apiPath)
        {
            return await _client.GetStreamAsync(apiPath);
        }

        public async Task<byte[]> Get(string apiPath)
        {
            return await _client.GetByteArrayAsync(apiPath);
        }

        public async Task<byte[]> Get(Uri uri)
        {
            return await _client.GetByteArrayAsync(uri);
        }

        public Task<T> Post<T>(Uri uri, HttpContent content) where T : HttpResponseMessage
        {
            return PostAndRetryInner<T>(uri, content);
        }

        public Task<T> Post<T>(string apiPath, HttpContent content) where T : HttpResponseMessage
        {
            return PostAndRetryInner<T>(new Uri(apiPath, UriKind.Absolute), content);
        }

        #region Internal
        internal Task<T> GetAndRetryInner<T>(Uri uri, int retryCount = AppConstants.DefaultGetRetryCount, Func<Exception, int, Task> onRetry = null) where T : HttpResponseMessage
        {
            return _networkService.Retry<T>(new Func<Task<T>>(() => ProcessRequest<T>(uri, isGet: true)), retryCount, onRetry);
        }

        internal Task<byte[]> GetAndRetryInner(string apiPath, int retryCount = AppConstants.DefaultGetRetryCount, Func<Exception, int, Task> onRetry = null)
        {
            return _networkService.Retry<byte[]>(new Func<Task<byte[]>>(() => FetchGetBytes(apiPath)), retryCount, onRetry);
        }

        internal Task<T> PostAndRetryInner<T>(Uri uri, HttpContent content, string contentType = AppConstants.ContentTypeJson, int retryCount = 1, Func<Exception, int, Task> onRetry = null) where T : HttpResponseMessage
        {
            return _networkService.Retry<T>(new Func<Task<T>>(() => ProcessRequest<T>(uri, content)), retryCount, onRetry);
        }

        internal async Task<T> ProcessRequest<T>(Uri uri, HttpContent content = null, bool isGet = false) where T : HttpResponseMessage
        {
            if (!_essentialsService.IsNetworkAvailable())
                throw new OfflineException();

            return isGet ? await FetchGetResponse<T>(uri) : await FetchPostResponse<T>(uri, content);
        }

        async Task<T> FetchGetResponse<T>(Uri uri) where T : HttpResponseMessage
        {
            return (T)await _client.GetAsync(uri);
        }

        async Task<T> FetchPostResponse<T>(Uri uri, HttpContent content) where T : HttpResponseMessage
        {
            return (T)await _client.PostAsync(uri, content);
        }

        internal Task<byte[]> FetchGetBytes(string apiPath)
        {
            return _client.GetByteArrayAsync(apiPath);
        }

        internal Task<byte[]> FetchGetBytes(Uri uri)
        {
            return _client.GetByteArrayAsync(uri);
        }
        #endregion
    }
}