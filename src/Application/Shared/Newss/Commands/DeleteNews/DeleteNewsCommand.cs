using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Resources;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ContractorBackend.Application.Shared.Newss.Commands.DeleteNews
{
    public class DeleteNewsCommand : IRequest
    {
        public Guid NewsId { get; set; }
        public DeleteNewsCommand(Guid id)
        {
            NewsId = id;
        }
    }

    public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand>
    {
        private readonly IRepository<News> _repository;
        private readonly IRepository<Document> _docRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly IDocumentService _documentService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;


        public DeleteNewsCommandHandler(IRepository<News> repository,
            IRepository<Document> docRepository,
            IHttpContextAccessor accessor,
            IConfiguration config,
            UserManager<User> userManager,
            IApplicationDbContext context,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IDocumentService documentService)
        {
            _repository = repository;
            _docRepository = docRepository;
            _accessor = accessor;
            _configuration = config;
            _userManager = userManager;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
            _documentService = documentService;
        }
        public async Task<Unit> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
        {
            var entity = _repository.GetById(request.NewsId);

            if (entity == null)
            {
                throw new CustomException("خبر یافت نشد");
            }

            #region Validate user has access to News Category
            var userId = _accessor.HttpContext.GetUserId();

            var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);


            var userRoles = _context.Set<UserRole>()
                .Where(x => x.UserId == user.Id)
                .Select(x => x.Role)
                .ToList();
            // if user is NOT SuperAdmin then check for OrgUnit Match
            if (!userRoles.Any(x => x.IsAdministrator))
            {
                //var currentUserOrgCode = user.Employee.BusinessUnitIsSuiteId;
                //var currentUserOrgId = _context.BusinessUnits.FirstOrDefault(x => x.IsSuiteId == currentUserOrgCode).Id;
                //var saveCategory = _context.NewsCategories.Include(x => x.NewsCategoryOrgUnits).FirstOrDefault(x => x.Id == entity.CategoryId);

                //if (saveCategory is not null && saveCategory.IsNotifications)
                //{
                //    var hasBusinessUnit = saveCategory.NewsCategoryOrgUnits.Any(l => l.OrgUnitId == currentUserOrgId);
                //    if (!hasBusinessUnit)
                //    {
                //        throw new CustomException(_sharedLocalizer["DeleteNewsCommandBusinessUnitValidation"]);
                //    }
                //}
            }
            #endregion

            if (entity is null)
                throw new NullReferenceException();

            if (entity.PublisherUserId == _accessor.HttpContext.GetUserId() || userRoles.Any(x => x.IsAdministrator))
            {
                _repository.Delete(entity);
                if (!string.IsNullOrWhiteSpace(entity.PhotoIds))
                {
                    var arr = entity.PhotoIds.Split(",");

                    if (!string.IsNullOrWhiteSpace(entity.PhotoIds))
                    {
                        var entityIds = entity.PhotoIds.Split(",").Select(x => Guid.Parse(x)).ToList();
                        _documentService.DeleteDocuments(entityIds, DocumentFolder.NewsPhotos);
                    }
                    if (!string.IsNullOrWhiteSpace(entity.AttachmentIds))
                    {
                        var entityIds = entity.AttachmentIds.Split(",").Select(x => Guid.Parse(x)).ToList();
                        _documentService.DeleteDocuments(entityIds, DocumentFolder.NewsAttachments);
                    }
                }
            }
            else
                throw new CustomException("کاربر جاری این پیام را ایجاد نکرده است");

            return Unit.Value;
        }
    }
}
