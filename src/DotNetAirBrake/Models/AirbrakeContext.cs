namespace DotNetAirbrake.Models
{
    public class AirbrakeContext
    {
        public string Environment { get; set; }
        public string Component { get; set; }
        public string Action { get; set; }
        public string Os { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public string Hostname { get; set; }
        public string RootDirectory { get; set; }
        public string UserId { get; set; }
        public string UserUsername { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}