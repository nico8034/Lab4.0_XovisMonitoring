using Newtonsoft.Json;

namespace Domain.Entities.JSON;

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class LiveDataContent
    {
        public List<Element> element { get; set; }
    }

    public class Element
    {
        [JsonProperty("element-id")]
        public int elementid { get; set; }

        [JsonProperty("element-name")]
        public string elementname { get; set; }

        [JsonProperty("sensor-type")]
        public string sensortype { get; set; }

        [JsonProperty("data-type")]
        public string datatype { get; set; }

        [JsonProperty("live-data")]
        public LiveData livedata { get; set; }
    }

    public class LiveData
    {
        public DateTime time { get; set; }
        public List<Value> value { get; set; }
    }

    public class Root
    {
        [JsonProperty("sensor-time")]
        public SensorTime sensortime { get; set; }
        public Status status { get; set; }
        public LiveDataContent content { get; set; }
    }

    public class SensorTime
    {
        public string timezone { get; set; }
        public DateTime time { get; set; }
    }

    public class Status
    {
        public string code { get; set; }
    }

    public class Value
    {
        public int value { get; set; }
        public string label { get; set; }
    }
