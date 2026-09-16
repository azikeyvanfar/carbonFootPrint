using System;
using AutoMapper;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.QASubjects.Commands.CreateQASubject;
using ContractorBackend.Application.QASubjects.Commands.UpdateQASubject;
using ContractorBackend.Domain.Entities.Shared;

namespace ContractorBackend.Application.Mappings.Shared
{
    public class QASubjectProfile : Profile
    {
        public QASubjectProfile()
        {
            CreateMap<QASubject, QASubjectDto>()
      // DO NOT REMOVE THIS COMMENT:NG01
      .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
      .ForMember(d => d.SubjectName, m => m.MapFrom(s => s.SubjectName))
      .ForMember(d => d.IsActive, m => m.MapFrom(s => s.IsActive))
      //.ForMember(c => c.LastModifiedByUserId, x => x.MapFrom(d => EF.Property<long?>(d, "ModifiedByUserId")))
      //.ForMember(c => c.LastModifiedDateTime, x => x.MapFrom(d => EF.Property<DateTimeOffset?>(d, "ModifiedDateTime")))
      .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();


            // DO NOT REMOVE THIS COMMENT:NG02

            CreateMap<CreateQASubjectCommand, QASubject>()
            // DO NOT REMOVE THIS COMMENT:NG03
            .ForMember(s => s.Id, m => m.MapFrom(_ => Guid.NewGuid()))
            .ForMember(s => s.SubjectName, m => m.MapFrom(s => s.SubjectName))
            .ForMember(s => s.IsActive, m => m.MapFrom(s => s.IsActive))
            .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
            // DO NOT REMOVE THIS COMMENT:NG04

            CreateMap<UpdateQASubjectCommand, QASubject>()
            // DO NOT REMOVE THIS COMMENT:NG05
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.SubjectName, m => m.MapFrom(s => s.SubjectName))
            .ForMember(d => d.IsActive, m => m.MapFrom(s => s.IsActive))
            .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

            // DO NOT REMOVE THIS COMMENT:NG06
        }
    }
}
