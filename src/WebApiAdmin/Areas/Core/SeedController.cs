using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using ContractorBackend.WebApiAdmin.Filters;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ContractorBackend.WebApiAdmin.Areas.Core
{
    [Area("Core")]
    [Route("api/[area]/[controller]/[action]")]
    public class SeedController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationDbContext _dbContext;
        private readonly IBackgroundJobClient _backgroundJob;


        public SeedController(IApplicationDbContext dbContext, IWebHostEnvironment env, IBackgroundJobClient backgroundJob)
        {
            _dbContext = dbContext;
            _env = env;
            _backgroundJob = backgroundJob;
        }


        [HttpPost]
        [DisplayName("Encrypt")]
        [ErrorCode("130-10")]
        [AllowAnonymous]
        public Task<List<string>> Encrypt(List<string> list)
        {
            if (!_env.IsDevelopment())
            {

                return Task.FromResult(new List<string>());
            }

            var output = new List<string>();
            foreach (var item in list)
            {
                var originalText = item;
                var strEn = AESService.Encrypt(originalText);
                var strDe = AESService.Decrypt(strEn);

                output.Add($"Original Text: {originalText}  |||  Encrypted Text: {strEn} ");
            }

            return Task.FromResult(output);
        }


        [HttpPost]
        [DisplayName("Decrypt")]
        [ErrorCode("130-10")]
        [AllowAnonymous]
        public Task<List<string>> Decrypt(List<string> list)
        {
            if (!_env.IsDevelopment())
            {

                return Task.FromResult(new List<string>());
            }

            var output = new List<string>();
            foreach (var item in list)
            {
                var encryptedText = item;
                var strDe = AESService.Decrypt(encryptedText);

                output.Add($"Original Text: {encryptedText}  |||  Decrypted Text: {strDe} ");
            }

            return Task.FromResult(output);
        }


        //[HttpGet]
        //[DisplayName("RegisterUsers")]
        //[ErrorCode("130-10")]
        //[AllowAnonymous]
        //public OkApiResult<bool> SyncUsers()
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        return new OkApiResult<bool>(true);
        //    }

        //    _backgroundJob.Schedule<IHangFireSyncUserService>((x) => x.SyncUsers(), TimeSpan.FromSeconds(2));

        //    return new OkApiResult<bool>(true);
        //}

        //[HttpGet]
        //[DisplayName("UpdateUsers")]
        //[ErrorCode("130-10")]
        //[AllowAnonymous]
        //public async Task<OkApiResult<bool>> UpdateUsers()
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        return new OkApiResult<bool>(true);
        //    }

        //    _backgroundJob.Schedule<IHangFireSyncUserService>((x) => x.SyncUpdateUsers(), TimeSpan.FromSeconds(2));

        //    return new OkApiResult<bool>(true);
        //}


        //[HttpGet]
        //[DisplayName("DeactivateUsers")]
        //[ErrorCode("130-10")]
        //[AllowAnonymous]
        //public async Task<OkApiResult<bool>> DeactivateUsers()
        //{
        //    if (!_env.IsDevelopment())
        //    {
        //        return new OkApiResult<bool>(true);
        //    }

        //    _backgroundJob.Schedule<IHangFireSyncUserService>((x) => x.SyncDeactiveUsers(), TimeSpan.FromSeconds(2));

        //    return new OkApiResult<bool>(true);
        //}

        //[HttpGet]
        //[DisplayName("SeedUserMarriageData")]
        //[AllowAnonymous]
        //public async Task<bool> SeedUserMarriageData()
        //{
        //    //if (!_env.IsDevelopment())
        //    //{

        //    //    return Task.FromResult(new List<string>());
        //    //}

        //    var command = new SeedUserMarriageDataCommand();
        //    await Mediator.Send(command);

        //    return true;
        //}


    }
}
