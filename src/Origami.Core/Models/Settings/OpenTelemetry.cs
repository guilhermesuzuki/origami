namespace Origami.Core.Models.Settings
{
    public class OpenTelemetry : IEnabled, IEndpoint
    {
        public bool Enabled { get; set; }

        /// <summary>
        /// OpenTelemetry exporter endpoint
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;
    }
}
