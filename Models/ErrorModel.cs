using Newtonsoft.Json;

namespace CustomerAccountDeletionRequest.Models
{
    public class ErrorModel
    {
        [JsonRequired]
        public int StatusCode { get; set; }
        [JsonRequired]
        public string ErrorMessage { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
