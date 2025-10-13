using System;
using System.Collections.Generic;
using ContractorBackend.Application.Common.Dtos;

namespace ContractorBackend.Application.Dtos.Share
{
    public class NewsDto : ShadowPropertyDto
    {
        public NewsDto()
        {
            PhotosVM = new();
        }
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsSpecial { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public DateTime CreatedDate { get; set; }
        public long? RelatedOrgUnitId { get; set; }
        public string? RelatedOrgUnitName { get; set; }
        public long PublisherUserId { get; set; }
        public string PublisherUserAvatar { get; set; }
        public string PublisherUserDisplayName { get; set; }
        public string PublisherUserPersonnelCode { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime? StartShownDate { get; set; }
        public DateTime? EndShownDate { get; set; }
        public string BodyContent { get; set; }
        public bool IsNotifications { get; set; }

        public string PhotosId { get; set; }
        public string AttachmentIds { get; set; }

        public List<FileVM> PhotosVM { get; set; } = new();

        /// <summary>
        /// ایدی های پیوست
        /// </summary>
        public List<FileVM> AttachmentsVM { get; set; } = new();


    }
}
