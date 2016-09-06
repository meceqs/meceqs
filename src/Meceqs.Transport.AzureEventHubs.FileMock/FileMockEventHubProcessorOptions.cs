namespace Meceqs.Transport.AzureEventHubs.FileMock
{
    public class FileMockEventHubProcessorOptions
    {
        public string Directory { get; set; }

        public bool ClearEventHubOnStart { get; set; }

        public string EventHubName { get; set; }
    }
}