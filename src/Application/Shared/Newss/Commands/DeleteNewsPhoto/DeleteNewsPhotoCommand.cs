using System;
using System.IO;
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

namespace ContractorBackend.Application.Shared.Newss.Commands.DeleteNewsPhoto
{
    public class DeleteNewsPhotoCommand : IRequest
    {
        public Guid NewsId { get; set; }
        public Guid DocumentId { get; set; }
        public DeleteNewsPhotoCommand(Guid docId, Guid newsId)
        {
            NewsId = newsId;
            DocumentId = docId;
        }
    }

    public class DeleteNewsPhotoCommandHandler : IRequestHandler<DeleteNewsPhotoCommand>
    {
        private readonly IRepository<News> _repository;
        private readonly IRepository<Document> _docRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IApplicationDbContext _context;


        public DeleteNewsPhotoCommandHandler(
            IRepository<News> repository,
            IConfiguration configuration,
            IHttpContextAccessor accessor,
            IRepository<Document> docRepo,
            UserManager<User> userManager,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IApplicationDbContext context
            )
        {
            _repository = repository;
            _docRepository = docRepo;
            _accessor = accessor;
            _configuration = configuration;
            _userManager = userManager;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
        }

        public async Task<Unit> Handle(DeleteNewsPhotoCommand request, CancellationToken cancellationToken)
        {
            var news = _context.News.FirstOrDefault(x => x.Id == request.NewsId);

            if (news is null)
            {
                throw new CustomException("خبر پیدا نشد.");
            }

            #region Validate user has access to News Category
            var userId = _accessor.HttpContext.GetUserId();

            var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            //var currentUserOrgCode = user.Employee.BusinessUnitIsSuiteId;
            //var currentUserOrgId = _context.BusinessUnits.FirstOrDefault(x => x.IsSuiteId == currentUserOrgCode).Id;

            //var saveCategory = _context.NewsCategories.Include(x => x.NewsCategoryOrgUnits).FirstOrDefault(x => x.Id == news.CategoryId);

            //if (saveCategory is not null && saveCategory.IsNotifications)
            //{
            //    var hasBusinessUnit = saveCategory.NewsCategoryOrgUnits.Any(l => l.OrgUnitId == currentUserOrgId);
            //    if (!hasBusinessUnit)
            //    {
            //        throw new CustomException(_sharedLocalizer["DeleteNewsCommandBusinessUnitValidation"]);
            //    }

            //}
            #endregion

            var docEntity = _docRepository.GetById(request.DocumentId);
            var newsEntity = _repository.GetById(request.NewsId);
            if (docEntity is null || newsEntity is null)
            {
                throw new NullReferenceException();
            }

            if (newsEntity.PublisherUserId == _accessor.HttpContext.GetUserId())
            {
                var fullPath = Path.Combine(_configuration["localStaticStoragePath"],
                              "News", docEntity.GeneratedName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                _docRepository.Delete(docEntity);
                var attachIds = newsEntity.PhotoIds;
                if (attachIds.Contains($"{docEntity.Id},"))
                {
                    attachIds = attachIds.Replace($"{docEntity.Id},", "");
                }
                else if (attachIds.Contains($"{docEntity.Id}"))
                {
                    attachIds = attachIds.Replace($"{docEntity.Id}", "");
                }
                newsEntity.PhotoIds = attachIds;
                _repository.Update(newsEntity);
            }
            return Unit.Value;
        }

        Task IRequestHandler<DeleteNewsPhotoCommand>.Handle(DeleteNewsPhotoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
