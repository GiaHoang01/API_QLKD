namespace API_KeoDua.Models
{
    public class ResponseModel
    {
        public int status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
