using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Documents.Queries.GetDocumentById;
using ContractorBackend.Application.Documents.Queries.GetDocumentFile;
using ContractorBackend.Application.Documents.Queries.GetDocumentImage;
using ContractorBackend.Application.Documents.Queries.GetDocumentThumbnail;
using ContractorBackend.Application.Dtos;
using ContractorBackend.WebApiClient.Controllers;
using ContractorBackend.WebApiClient.Filters;
using ContractorBackend.WebApiClient.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContractorBackend.WebApiClient.Areas.Core.Controllers
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]

    public class DocumentsController : ApiControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public DocumentsController(IHttpContextAccessor accessor)
        {
            _contextAccessor = accessor;
        }

        /// <summary>
        /// نمایش فایل و پوشه بر اساس شناسه  
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [DisplayName("نمایش فایل و پوشه بر اساس شناسه")]
        [ErrorCode("109-01")]
        public async Task<OkApiResult<DocumentDto>> GetById([FromQuery] Guid Id)
        {
            return new OkApiResult<DocumentDto>(
                await Mediator.Send(new GetDocumentByIdQuery(Id)));
        }

        /// <summary>
        /// public دریافت فایل 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("public دریافت فایل")]
        [AllowAnonymous]
        public async Task<FileBase64Dto> GetFile(Guid Id)
        {
            return await Mediator.Send(new GetDocumentFileQuery(Id, false));
        }

        /// <summary>
        /// public دریافت تصویر 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("public دریافت تصویر")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetImage(Guid Id)
        {
            // var xyz = HttpContext.Request.Cookies;
            var result = await Mediator.Send(new GetDocumentImageQuery(Id, false));

            if (result.FileContents is null)
                throw new NullReferenceException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }

        /// <summary>
        /// private دریافت تصویر 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("private دریافت تصویر")]
        [ErrorCode("109-02")]
        public async Task<FileStreamResult> GetPImage(Guid Id)
        {
            var result = await Mediator.Send(new GetDocumentImageQuery(Id, true));

            if (result.FileContents is null)
                throw new NullReferenceException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }

        /// <summary>
        /// دریافت thumbnail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisplayName("دریافت thumbnail")]
        [ErrorCode("109-03")]
        public async Task<FileStreamResult> GetThumbnail(Guid Id)
        {
            var result = await Mediator.Send(new GetDocumentThumbnailQuery(Id));

            if (result.FileContents is null)
                throw new NullReferenceException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }

    }
}