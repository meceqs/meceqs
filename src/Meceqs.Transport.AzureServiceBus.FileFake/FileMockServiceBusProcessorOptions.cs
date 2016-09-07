namespace Meceqs.Transport.AzureServiceBus.FileMock
{
    public class FileMockServiceBusProcessorOptions
    {
        public bool ClearOnStart { get; set; } = false;

        public string Directory { get; set; }

        public string EntityPath { get; set; }

        public string ArchiveFolderName { get; set; } = "archive";
    }
}