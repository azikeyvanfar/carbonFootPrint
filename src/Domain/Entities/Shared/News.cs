using System;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;

namespace ContractorBackend.Domain.Entities.Shared
{
    public class News : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// خبر ویژه یا خیر
        /// </summary>
        public bool IsSpecial { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public long? RelatedOrgUnitId { get; set; }
        public long PublisherUserId { get; set; }

        public User PublisherUser { get; set; }
        public Guid CategoryId { get; set; }
        public NewsCategory Category { get; set; }
        public DateTime? StartShownDate { get; set; }
        public DateTime? EndShownDate { get; set; }

        /// <summary>
        /// content of news
        /// </summary>
        public string BodyContent { get; set; }

        /// <summary>
        /// first AttachmentId is mainPhoto
        /// comma seperated documentIds from document entity
        /// </summary>
        public string PhotoIds { get; set; }
        /// <summary>
        /// ایدی اتچمنت ها
        /// </summary>
        public string AttachmentIds { get; set; }


    }
}
