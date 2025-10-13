using System;
using AutoMapper;
using ContractorBackend.Application.Core.Menus.Commands.CreateMenu;
using ContractorBackend.Application.Core.Menus.Commands.UpdateMenu;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;

namespace ContractorBackend.Application.Common.Mappings.Core
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<Menu, MenuDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.DocumentId, m => m.MapFrom(s => s.DocumentId))
                .ForMember(s => s.Options, m => m.MapFrom(s => s.Options))
                .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
                .ForMember(d => d.DocumentName, m => m.MapFrom(s => s.Document != null ? s.Document.GeneratedName : string.Empty))
                .ForMember(d => d.DocumentName, m => m.MapFrom(s => s.Document != null ? s.Document.GeneratedName : string.Empty))
                .ForMember(d => d.DocumentMimeType, m => m.MapFrom(s => s.Document != null ? s.Document.MimeType : string.Empty))
                .ForMember(d => d.DocumentExtension, m => m.MapFrom(s => s.Document != null ? s.Document.Extension : string.Empty))
                .ForMember(d => d.UserPersonnelCode, m => m.MapFrom(s => s.Owner != null ? s.Owner.PersonnelCode.ToString() : string.Empty))
                //.ForMember(d => d.UserAvatar, m => m.MapFrom(s => s.Owner != null ? s.Owner.Employee.ProfileImage.ToString() : null))
                .ForMember(d => d.UserFullName, m => m.MapFrom(s => s.Owner != null ? s.Owner.FirstName + " " + s.Owner.LastName : string.Empty))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateMenuCommand, Menu>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
              .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
               .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.DocumentId, m => m.MapFrom(s => s.DocumentId))
                // .ForMember(s => s.Options, m => m.MapFrom(s => s.Options))
                .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UpdateMenuCommand, Menu>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                // .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForMember(d => d.DocumentId, m => m.MapFrom(s => s.DocumentId))
                //  .ForMember(s => s.Options, m => m.MapFrom(s => s.Options))
                .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
