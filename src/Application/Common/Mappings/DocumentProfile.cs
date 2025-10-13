using System;
using AutoMapper;
using ContractorBackend.Application.Documents.Commands.CreateDocument;
using ContractorBackend.Application.Documents.Commands.UpdateDocument;
using ContractorBackend.Application.Dtos;
using ContractorBackend.Domain.Entities.Core;
namespace ContractorBackend.Application.Mapping
{

    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<AttachmentDto, Document>()
            .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
            .ForMember(d => d.Alt, m => m.MapFrom(s => s.Alt))
            .ForMember(d => d.RootId, m => m.MapFrom(s => s.RootId))
            .ForAllOtherMembers(x => x.Ignore());


            CreateMap<Document, DocumentDto>()
            // DO NOT REMOVE THIS COMMENT:NG01
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.ParentId, m => m.MapFrom(s => s.ParentId))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.AliasName, m => m.MapFrom(s => s.AliasName))
            .ForMember(d => d.RootId, m => m.MapFrom(s => s.RootId))
            //.ForMember(d => d.RealName, m => m.MapFrom(s => s.RealName))
            .ForMember(d => d.Extension, m => m.MapFrom(s => s.Extension))
            .ForMember(d => d.MimeType, m => m.MapFrom(s => s.MimeType))
            .ForMember(d => d.Type, m => m.MapFrom(s => s.Type))
            .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
            .ForMember(d => d.Alt, m => m.MapFrom(s => s.Alt))
            .ForMember(d => d.OwnerId, m => m.MapFrom(s => s.OwnerId))
            .ForMember(d => d.UserPersonnelCode, m => m.MapFrom(s => s.Owner != null ? s.Owner.PersonnelCode : string.Empty))
            //.ForMember(d => d.UserAvatar, m => m.MapFrom(s => s.Owner != null && s.Owner.UserDetails != null ? s.Owner.UserDetails.ProfileImage : null))
            .ForMember(d => d.UserFullName, m => m.MapFrom(s => s.Owner != null ? s.Owner.FirstName + " " + s.Owner.LastName : string.Empty))
            .ForAllOtherMembers(x => x.Ignore());

            // DO NOT REMOVE THIS COMMENT:NG02

            CreateMap<CreateDocumentCommand, Document>()
            // DO NOT REMOVE THIS COMMENT:NG03
            .ForMember(s => s.Id, m => m.MapFrom(_ => Guid.NewGuid()))
            .ForMember(s => s.ParentId, m => m.MapFrom(s => s.ParentId))
            .ForMember(s => s.RootId, m => m.MapFrom(s => s.RootId))
            .ForMember(s => s.AliasName, m => m.MapFrom(s => s.AliasName))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.Type, m => m.MapFrom(s => s.Type))
            .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
            .ForMember(d => d.Alt, m => m.MapFrom(s => s.Alt))
            .ForAllOtherMembers(opt => opt.Ignore());
            // DO NOT REMOVE THIS COMMENT:NG04

            CreateMap<UpdateDocumentCommand, Document>()
            // DO NOT REMOVE THIS COMMENT:NG05
            .ForMember(s => s.RootId, m => m.MapFrom(s => s.RootId))
            .ForMember(s => s.AliasName, m => m.MapFrom(s => s.AliasName))
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
            .ForMember(d => d.Alt, m => m.MapFrom(s => s.Alt))
            .ForAllOtherMembers(x => x.Ignore());

            // DO NOT REMOVE THIS COMMENT:NG06

        }
    }
}
