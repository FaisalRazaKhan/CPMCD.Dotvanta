using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CPMCD.Dotvanta.ApiCaller.Models;

namespace CPMCD.Dotvanta.ApiCaller.Interfaces
{
    /// <summary>
    /// Universal API caller contract. Endpoint (relative path, BaseUrl ke saath jud jayega,
    /// ya full absolute URL bhi de sakte ho) pass karo aur strongly-typed response wapas milega -
    /// GET/POST/PUT/PATCH/DELETE sab covered. Har method me optional CancellationToken aur
    /// per-call extra headers bhi diye ja sakte hain.
    /// </summary>
    public interface IApiCaller
    {
        Task<ApiResponse<TResult>> GetAsync<TResult>(string endpoint, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        Task<ApiResponse<TResult>> PostAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        Task<ApiResponse<TResult>> PutAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        Task<ApiResponse<TResult>> PatchAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        Task<ApiResponse<TResult>> DeleteAsync<TResult>(string endpoint, string token = null,
            Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ek hi generic entry point - method + endpoint + (optional) body do,
        /// package khud request build/serialize/deserialize kar ke result de dega.
        /// Internally connection pooling + retry ke saath fast aur platform-universal.
        /// </summary>
        Task<ApiResponse<TResult>> CallAsync<TRequest, TResult>(HttpMethodType method, string endpoint,
            TRequest data = default, string token = null, Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default);
    }

    public enum HttpMethodType
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }
}
