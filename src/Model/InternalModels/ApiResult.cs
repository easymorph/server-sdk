using System;
using System.Net.Http.Headers;

namespace Morph.Server.Sdk.Model.InternalModels
{
    /// <summary>
    /// Represents api result of DTO Model or Error (Exception)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        public virtual T Data { get; protected set; } = default(T);
        public virtual Exception Error { get; protected set; } = default(Exception);
        public virtual bool IsSucceed { get { return Error == null; } }

        public virtual HttpResponseHeaders ResponseHeaders { get; protected set; } =  default(HttpResponseHeaders);
        public static ApiResult<T> Fail(Exception exception, HttpResponseHeaders httpResponseHeaders)
        {
            return new ApiResult<T>()
            {
                Data = default(T),
                Error = exception,
                ResponseHeaders = httpResponseHeaders
            };

        }

        public static ApiResult<T> Ok(T data, HttpResponseHeaders httpResponseHeaders)
        {
            return new ApiResult<T>()
            {
                Data = data,
                Error = null,
                ResponseHeaders = httpResponseHeaders
            };
        }

        public virtual void ThrowIfFailed()
        {
            if (!IsSucceed && Error != null)
            {
                throw Error;
            }
        }
    }


}


