using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using ContractorBackend.WebApiClient.Filters;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ContractorBackend.WebApiClient.Areas.Core
{
    [Area("Core")]
    [Route("api/cli/[area]/[controller]/[action]")]
    public class SeedController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationDbContext _dbContext;
        private readonly IBackgroundJobClient _backgroundJob;
        private readonly ISeedService _seedService;


        public SeedController(IApplicationDbContext dbContext, IWebHostEnvironment env, IBackgroundJobClient backgroundJob, ISeedService seedService)
        {
            _dbContext = dbContext;
            _env = env;
            _backgroundJob = backgroundJob;
            _seedService = seedService;
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


        [HttpPost]
        [DisplayName("SeedLookups")]
        [ErrorCode("130-11")]
        [AllowAnonymous]
        public async Task<bool> SeedLookups()
        {
            if (!_env.IsDevelopment())
            {
                return false;
            }
            await _seedService.SeedLookups();
            return true;
        }

        [HttpPost]
        [DisplayName("SeedUnits")]
        [ErrorCode("130-12")]
        [AllowAnonymous]
        public async Task<bool> SeedUnits()
        {
            if (!_env.IsDevelopment())
            {
                return false;
            }
            //await _seedService.SeedUnits();
            return true;
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








        //[HttpPost]
        //[DisplayName("Delete")]
        //[ErrorCode("130-10")]
        //[AllowAnonymous]
        //public async Task<string> Delete(List<string> list)
        //{
        //    if (!_env.IsDevelopment())
        //    {

        //        return "o";
        //    }

        //    var dell = _dbContext.RiskDefinitions   ;
        //    //var dell = _dbContext.RiskDefinitions.FirstOrDefault(x => x.Id == new Guid("804B523F-BEB6-4899-9C4C-B6E2097123F2"));

        //    try
        //    {
        //        _dbContext.RiskDefinitions.RemoveRange(dell);

        //        _dbContext.SaveChanges();



        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }


        //    return "done";
        //}




    }
}
