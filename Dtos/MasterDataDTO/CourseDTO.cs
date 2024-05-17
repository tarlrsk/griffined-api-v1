using Newtonsoft.Json;

namespace griffined_api.Dtos.MasterDataDTO
{
    public class CreateCourseDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class CourseDTO : CreateCourseDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public new string Name { get { return base.Name; } }
    }
}