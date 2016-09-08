namespace Meceqs.AzureEventHubs.FileFake
{
    public class FileFakeEventHubProcessorOptions
    {
        public string Directory { get; set; }

        public bool ClearEventHubOnStart { get; set; }

        public string EventHubName { get; set; }
    }
}