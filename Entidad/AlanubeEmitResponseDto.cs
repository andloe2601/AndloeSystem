namespace Andloe.Entidad
{
    public sealed class AlanubeEmitResponseDto
    {
        public string? Id { get; set; }
        public string? TrackId { get; set; }
        public string? Status { get; set; }
        public string? LegalStatus { get; set; }
        public string? Message { get; set; }
        public string? RawJson { get; set; }

        public string GetTrackOrId()
        {
            if (!string.IsNullOrWhiteSpace(TrackId)) return TrackId!;
            if (!string.IsNullOrWhiteSpace(Id)) return Id!;
            return string.Empty;
        }
    }
}