using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContractorBackend.Application.Services
{
    public class FileExtensions : IFileExtensions
    {
        private readonly ILogger<FileExtensions> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRepository<FileExtension> _fERepository;
        public FileExtensions(ILogger<FileExtensions> logger, IConfiguration configuration, IRepository<FileExtension> fERepository)
        {
            _logger = logger;
            _configuration = configuration;
            _fERepository = fERepository;
        }
        public async Task<bool> ValidTypeFileAsync(ICollection<IFormFile> PhotoesId, DocumentFolder entity)
        {
            if (PhotoesId is not null)
            {
                foreach (IFormFile att in PhotoesId)
                {
                    if (!await CheckAllowedType(att, entity))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public async Task<bool> ValidTypeFileAsync(IFormFile file, DocumentFolder entity)
        {
            if (file is not null)
            {
                if (!await CheckAllowedType(file, entity))
                {
                    return false;
                }
            }
            return true;
        }


        public Task<bool> ValidMaxCountAsync(ICollection<IFormFile> files, DocumentFolder entity)
        {
            if (files == null || !files.Any())
            {
                return Task.FromResult(true);
            }
            var entityStr = entity.ToString();

            var fileExt = _fERepository.GetAllAsNoTracking().Where(_ => _.EntityName == entityStr).FirstOrDefault();

            if (fileExt == null)
            {
                return Task.FromResult(true);
            }
            if (fileExt.MaxAllowedCount <= 0)
            {
                return Task.FromResult(true);
            }

            var isValid = files.Count <= fileExt.MaxAllowedCount;

            if (isValid == false)
            {
                _logger.LogError($"{nameof(ValidMaxCountAsync)}: failed to upload because of Max Upload files limits");
            }

            return Task.FromResult(isValid);
        }


        private async Task<bool> CheckAllowedType(IFormFile postedImage, DocumentFolder entity)
        {
            if (postedImage is not null)
            {
                var entityStr = entity.ToString();

                bool flg = false;
                var fileExt = _fERepository.GetAllAsNoTracking().Where(_ => _.EntityName == entityStr).FirstOrDefault();
                var listMime = fileExt.FileTypes.Split("||");
                if (listMime.Length > 0)
                {
                    var conditionFlg = false;
                    bool ftype = false;
                    foreach (var type in listMime)
                    {
                        if (!conditionFlg)
                        {
                            conditionFlg = conditionFlg || string.Equals(postedImage.ContentType, type, StringComparison.OrdinalIgnoreCase);
                        }

                        if (!ftype)
                        {
                            ftype = CheckFileType.IsImageVideoMP3ZipFile(postedImage, type, out CheckFileType.FileType fileType);
                        }
                        flg = flg || ftype;
                    }
                    flg = flg && conditionFlg;
                }
                if (fileExt.MinSizeByte < postedImage.Length && postedImage.Length < fileExt.MaxSizeByte)
                {
                    flg = flg && true;
                }
                else
                {
                    flg = false;
                }
                _logger.LogError($"{nameof(CheckAllowedType)}: failed upload because type is not ok");
                return flg;
            }
            return true;
        }
    }
}