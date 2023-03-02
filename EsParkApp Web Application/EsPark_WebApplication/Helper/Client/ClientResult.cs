using Newtonsoft.Json;

namespace EsPark_WebApplication.Helper.Client
{
    public class ClientResult : ClientResult<object>, IClientResult
    {

    }

    /// <summary>
    /// Client Result
    /// </summary>
    public class ClientResult<T> : IClientResult
    {
        /// <summary>
        /// Basarili
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Kod
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Mesaj Tipi
        /// </summary>
        public ClientResultMessageType MessageType { get; set; }

        /// <summary>
        /// Mesaj
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Mesaj
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InternalMessage { get; set; }

        /// <summary>
        /// Veri
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        /// <summary>
        /// Validasyon Mesajlari
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientResultValidationMessage> Validations { get; set; }
    }

    /// <summary>
    /// Client Result
    /// </summary>
    public interface IClientResult
    {
    }
}
