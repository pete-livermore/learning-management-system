using Domain.Common;

namespace Domain.Uploads.Entities
{
    public class UploadFile : BaseEntity
    {
        public int Id { get; init; }
        public required string Url { get; init; }
        public required string Mime { get; set; }
        public required string Ext { get; set; }
        public required string ProviderId { get; set; }
        public required Guid OwnerId { get; init; }
    }
}
