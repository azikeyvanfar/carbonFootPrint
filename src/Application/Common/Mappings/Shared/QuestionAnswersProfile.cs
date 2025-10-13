using System;
using AutoMapper;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Application.QuestionAnswers.Commands.CreateQA;
using ContractorBackend.Application.QuestionAnswers.Commands.UpdateQA;
using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Mappings.Shared
{
    public class QuestionAnswersProfile : Profile
    {
        public QuestionAnswersProfile()
        {
            CreateMap<QuestionAnswer, QuestionAnswersDto>()
                .ForMember(d => d.QASubjectName, m => m.MapFrom(s => s.QASubject != null ? s.QASubject.SubjectName : ""))
                .ForMember(d => d.Question, m => m.MapFrom(s => s.Question))
                .ForMember(d => d.Answer, m => m.MapFrom(s => s.Answer))
                .ForMember(d => d.CreateDate, m => m.MapFrom(s => s.CreateDate))
                .ForMember(d => d.QuestionarDisplayName, m => m.MapFrom(s => s.Questioner.FirstName + " " + s.Questioner.LastName))
                .ForMember(d => d.QuestionerId, m => m.MapFrom(s => s.QuestionerId))
                .ForMember(d => d.ResponderId, m => m.MapFrom(s => s.ResponderId))
                .ForMember(d => d.ResponseDate, m => m.MapFrom(s => s.ResponseDate))
                .ForMember(d => d.ResponderDisplayName, m => m.MapFrom(s => s.ResponderId != null ? s.Respond.FirstName + " " + s.Respond.LastName : ""))
                .ForMember(c => c.CreatedDateTime, x => x.MapFrom(d => EF.Property<DateTimeOffset?>(d, "CreatedDateTime")))
                .ForMember(c => c.ModifiedDateTime, x => x.MapFrom(d => EF.Property<DateTimeOffset?>(d, "ModifiedDateTime")))
                //.ForMember(c => c.LastModifiedByUserId, x => x.MapFrom(d => EF.Property<long?>(d, "ModifiedByUserId")))
                //.ForMember(c => c.LastModifiedDateTime, x => x.MapFrom(d => EF.Property<DateTimeOffset?>(d, "ModifiedDateTime")))
                ;

            CreateMap<CreateQuestionAnswersCommand, QuestionAnswer>()
            .ForMember(d => d.IsActive, m => m.MapFrom(s => s.IsActive))
            .ForMember(d => d.IsAdminType, m => m.MapFrom(s => s.IsAdminType))
            .ForMember(d => d.IsPrivate, m => m.MapFrom(s => s.IsPrivate))
            .ForMember(d => d.QaSubjectId, m => m.MapFrom(s => s.QaSubjectId))
            .ForMember(d => d.Question, m => m.MapFrom(s => s.Question))
            .ForMember(d => d.Answer, m => m.MapFrom(s => s.Answer))
            .ForMember(d => d.QuestionerId, m => m.MapFrom(s => s.QuestionerId))
            .ForMember(d => d.ResponderId, m => m.MapFrom(s => s.ResponderId))
            ;

            CreateMap<UpdateQuestionAnswersCommand, QuestionAnswer>()
            .ForMember(d => d.IsActive, m => m.MapFrom(s => s.IsActive))
            .ForMember(d => d.IsPrivate, m => m.MapFrom(s => s.IsPrivate))
            .ForMember(d => d.QaSubjectId, m => m.MapFrom(s => s.QaSubjectId))
            .ForMember(d => d.Question, m => m.MapFrom(s => s.Question))
            .ForMember(d => d.Answer, m => m.MapFrom(s => s.Answer))
            .ForMember(d => d.QuestionerId, m => m.MapFrom(s => s.QuestionerId))
            .ForMember(d => d.ResponderId, m => m.MapFrom(s => s.ResponderId))
             ;
            // DO NOT REMOVE THIS COMMENT:NG04    
        }
    }
}
