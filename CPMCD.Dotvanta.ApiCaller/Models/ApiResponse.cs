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
        public long ElapsedMilliseconds { get; set; }
        public bool IsTimeout { get; set; }
        public bool IsCancelled { get; set; }
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

        /// <summary>Request start se response end tak ka time - diagnostics/perf-tracking ke liye.</summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>True jab request server timeout ki wajah se fail hui (config ka TimeoutMinutes).</summary>
        public bool IsTimeout { get; set; }

        /// <summary>True jab caller ne khud CancellationToken cancel kiya (timeout se alag).</summary>
        public bool IsCancelled { get; set; }
    }
}
