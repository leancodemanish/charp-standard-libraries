namespace LeanCode.Standard.Logging
{
    public class LogDebugInformation
    {
        public LogDebugInformation(string category, object parameter, bool replace = true)
        {
            Category = category;
            Parameter = parameter;
            Replace = replace;
        }

        public string Category { get; }
        public object Parameter { get; }
        public bool Replace { get; }
    }
}
