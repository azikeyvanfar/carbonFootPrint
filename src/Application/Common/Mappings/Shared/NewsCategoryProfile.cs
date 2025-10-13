using System;
using System.Linq;
using AutoMapper;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Share;
using ContractorBackend.Application.Shared.NewsCategories.Commands.CreateNewsCategory;
using ContractorBackend.Application.Shared.NewsCategories.Commands.UpdateNewsCategory;
using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Mappings.Shared
{
    public class NewsCategoryProfile : Profile
    {
        public NewsCategoryProfile()
        {
            CreateMap<CreateNewsCategoryCommand, NewsCategory>();

            CreateMap<UpdateNewsCategoryCommand, NewsCategory>();

            CreateMap<NewsCategory, NewsCategoryDto>()
                .ForMember(c => c.OrgUnits, x => x.MapFrom(d => d.NewsCategoryOrgUnits
                .Select(x => new SelectModel
                {
                    Value = x.OrgUnitId.ToString(),
                    Text = "111111"//x.OrgUnit.des_busun
                }
                )))
                .ForMember(c => c.LastModifiedByUserId, x => x.MapFrom(d => EF.Property<long?>(d, "ModifiedByUserId")))
                .ForMember(c => c.LastModifiedDateTime, x => x.MapFrom(d => EF.Property<DateTimeOffset?>(d, "ModifiedDateTime")))
                ;
        }
    }
}
