using System.Threading.Tasks;
using CPMCD.Dotvanta.ApiCaller.Models;

namespace CPMCD.Dotvanta.ApiCaller.Interfaces
{
    /// <summary>
    /// Universal API caller contract. Endpoint (relative path, BaseUrl ke saath jud jayega)
    /// pass karo aur strongly-typed response wapas milega - GET/POST/PUT/DELETE sab covered.
    /// </summary>
    public interface IApiCaller
    {
        Task<ApiResponse<TResult>> GetAsync<TResult>(string endpoint, string token = null);

        Task<ApiResponse<TResult>> PostAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null);

        Task<ApiResponse<TResult>> PutAsync<TRequest, TResult>(string endpoint, TRequest data, string token = null);

        Task<ApiResponse<TResult>> DeleteAsync<TResult>(string endpoint, string token = null);

        /// <summary>
        /// Ek hi generic entry point - method + endpoint + (optional) body do,
        /// package khud request build/serialize/deserialize kar ke result de dega.
        /// </summary>
        Task<ApiResponse<TResult>> CallAsync<TRequest, TResult>(HttpMethodType method, string endpoint, TRequest data = default, string token = null);
    }

    public enum HttpMethodType
    {
        Get,
        Post,
        Put,
        Delete
    }
}
