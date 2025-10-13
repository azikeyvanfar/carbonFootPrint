using AutoMapper;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.Shared.Newss.Commands.CreateNews;
using ContractorBackend.Application.Shared.Newss.Commands.UpdateNews;
using ContractorBackend.Domain.Entities.Shared;

namespace ContractorBackend.Application.Mappings.Shared
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<News, NewsDto>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.IsActive, m => m.MapFrom(s => s.IsActive))
            .ForMember(d => d.IsSpecial, m => m.MapFrom(s => s.IsSpecial))
            .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
            .ForMember(d => d.Subtitle, m => m.MapFrom(s => s.Subtitle))
            .ForMember(d => d.PublisherUserDisplayName, m => m.MapFrom(s => s.PublisherUser != null ? s.PublisherUser.FirstName + " " + s.PublisherUser.LastName : ""))
            .ForMember(d => d.PublisherUserPersonnelCode, m => m.MapFrom(s => s.PublisherUser != null ? s.PublisherUser.PersonnelCode : ""))
            .ForMember(d => d.PublisherUserId, m => m.MapFrom(s => s.PublisherUserId))
            .ForMember(d => d.RelatedOrgUnitId, m => m.MapFrom(s => s.RelatedOrgUnitId))
            .ForMember(d => d.CategoryId, m => m.MapFrom(s => s.CategoryId))
            .ForMember(d => d.CategoryName, m => m.MapFrom(s => s.Category != null ? s.Category.Name : ""))
            .ForMember(d => d.StartShownDate, m => m.MapFrom(s => s.StartShownDate))
            .ForMember(d => d.EndShownDate, m => m.MapFrom(s => s.EndShownDate))
            .ForMember(d => d.PhotosId, m => m.MapFrom(s => s.PhotoIds))
            .ForMember(d => d.AttachmentIds, m => m.MapFrom(s => s.AttachmentIds))
            .ForMember(d => d.BodyContent, m => m.MapFrom(s => s.BodyContent))
            .ForMember(d => d.IsNotifications, m => m.MapFrom(s => s.Category != null ? s.Category.IsNotifications : false))

            //.ForMember(c => c.LastModifiedByUserId, x => x.MapFrom(d => EF.Property<long?>(d, "ModifiedByUserId")))
            //.ForMember(c => c.LastModifiedDateTime, x => x.MapFrom(d => EF.Property<DateTimeOffset?>(d, "ModifiedDateTime")))
            .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateNewsCommand, News>()
                ;



            CreateMap<UpdateNewsCommand, News>()
                .ForMember(d => d.PhotoIds, m => m.Ignore())
                .ForMember(d => d.AttachmentIds, m => m.Ignore())
                ;

        }
    }
}
