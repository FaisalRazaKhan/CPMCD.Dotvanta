using System;

namespace CPMCD.Dotvanta.ApiCaller.Models
{
    /// <summary>
    /// Non-generic response wrapper (jab data ka type fixed na ho).
    /// </summary>
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string ErrorDetails { get; set; }
    }

    /// <summary>
    /// Generic response wrapper - strongly typed Data ke saath.
    /// </summary>
    /// <typeparam name="T">Expected result type</typeparam>
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string ErrorDetails { get; set; }
    }
}
