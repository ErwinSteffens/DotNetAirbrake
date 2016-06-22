namespace DotNetAirbrake.Models
{
    public class AirbrakeBacktraceItem
    {
        public string File { get; set; }
        public string Function { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return $"{this.Function} [{this.File}]: {this.Line}";
        }
    }
}