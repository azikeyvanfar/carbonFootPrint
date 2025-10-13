using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Validation
{
    //  چک کردن پسوند فایل های آپلود شده
    public class ListFileValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                throw new ValidationException("فایل انتخاب نشده است");

            //var ContentTypeService = (IFileContentTypeService)validationContext.GetService(typeof(IFileContentTypeService));
            //var ContentTypes = ContentTypeService?.GetContentTypeNames() as IReadOnlyCollection<string>;
            //if (ContentTypes is null || ContentTypes.Count()==0)
            var ContentTypes = new[]
            {
                    "image/jpg",
                    "image/png",
                    "image/gif",
                    "image/jpeg",
                    "image/tif",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "application/x-zip-compressed",
                    "application/octet-stream",
                    "video/mp4",
                    "audio/mpeg",
                    "video/mpeg",
                    "application/pdf",
                    "text/csv"


                };
            //var ExtensionService = validationContext.GetService(typeof(IFileExtensionService)) as IFileExtensionService;
            //var Extensions = ExtensionService?.GetFileExtensionsAsync() as IReadOnlyCollection<string>;
            //if (Extensions is null || Extensions.Count() == 0)
            var Extensions = new[]
            {
                    ".jpg",
                    ".jpeg",
                    ".gif",
                    ".tif",
                    ".png",
                    ".docx",
                    ".xlsx",
                    ".rar",
                    ".zip",
                    ".mp4",
                    ".mpeg",
                    ".mp3",
                    ".pdf",
                    ".csv"
                };

            var files = value as IEnumerable<IFormFile>;

            foreach (IFormFile file in files)
            {
                var fileContentType = file.ContentType;
                var fileFileExtension = Path.GetExtension(file.FileName);
                var fileLenght = file.Length;

                if (file.Length > 15728640) throw new ValidationException("حجم فایل مورد نظر بیش از حد مجاز می باشد");


                if (!string.IsNullOrEmpty(fileFileExtension)) fileFileExtension = fileFileExtension.ToLower();

                if (!ContentTypes.Contains(fileContentType) || !Extensions.Contains(fileFileExtension))
                    throw new ValidationException("فایل مورد نظر معتبر نیست");
            }

            return ValidationResult.Success;
        }
    }
}
