using System;
using ContractorBackend.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ContractorBackend.Application.Common.Extensions
{
    public class FileExtensions
    {
        private readonly IConfiguration _configuration;
        public FileExtensions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetMimeType(string fileName)
        {
            string contentType;
            new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return contentType ?? "application/octet-stream";
        }
        public const int ImageMinimumBytes = 2097152;//2 megabyte
        public const int audioMinimumBytes = 20971520;//20 megabyte
        public const int videoMinimumBytes = 20971520;//20 megabyte
        public const int doMinimumBytes = 1000000;//2 megabyte


        public const int suggestionMaximumBytes = 1024 * 10;//10 megabyte
        public static bool IsSaveFile(IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types

            //-------------------------------------------
            if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                if (postedFile.Length > ImageMinimumBytes)
                {
                    throw new CustomException("حجم تصویر از 2 مگا بایت بیشتر است");
                }
                else return true;
            }
            else if (string.Equals(postedFile.ContentType, "audio/x-wav", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "audio/x-mp3", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "audio/mp4", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "application/ogg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "audio/x-mp2	", StringComparison.OrdinalIgnoreCase))
            {
                if (postedFile.Length > audioMinimumBytes)
                {
                    throw new CustomException("حجم از 20 مگا بایت بیشتر است");
                }
                else return true;
            }
            else if (string.Equals(postedFile.ContentType, "video/x-mpeg1", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "video/x-mpeg2", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "video/mp4", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "video/quicktime", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "video/quicktime", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "video/x-msvideo", StringComparison.OrdinalIgnoreCase))
            {
                if (postedFile.Length > audioMinimumBytes)
                {
                    throw new CustomException("حجم از 20 مگا بایت بیشتر است");
                }
                else return true;
            }
            else if (string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(postedFile.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(postedFile.ContentType, "application/msword", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(postedFile.ContentType, "application/vnd.openxmlformats", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(postedFile.ContentType, "video/quicktime", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(postedFile.ContentType, "video/x-msvideo", StringComparison.OrdinalIgnoreCase))
            {
                if (postedFile.Length > audioMinimumBytes)
                {
                    throw new CustomException("حجم از 1 مگا بایت بیشتر است");
                }
                else return true;
            }
            return false;


        }

        public static bool IsImage(IFormFile postedFile)
        {
            if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            { return true; }
            return false;
        }
        public static bool IsImage(string mimeType)
        {
            if (string.Equals(mimeType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "image/png", StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(mimeType, "image/png", StringComparison.OrdinalIgnoreCase))
            { return true; }
            return false;
        }

        //public static bool IsEducationAllowedTypes(IFormFile postedFile)
        //{
        //    if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
        //        string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
        //        //string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
        //        // string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
        //        //string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
        //        string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
        //        string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        //    { return true; }
        //    return false;

        //}



        public static bool IsSuggestionAllowedType(IFormFile postedFile)
        {

            if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                if (postedFile.Length > suggestionMaximumBytes)
                {
                    throw new CustomException("حجم از 10 مگا بایت بیشتر است");
                }
                else return true;
            }

            return false;
        }


        //public static bool IsEligibilityAllowedType(IFormFile postedFile)
        //{
        //    if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
        //       //string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
        //       // string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
        //       //string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        //    { return true; }
        //    return false;
        //}

        //public static bool IsHonorAllowedType(IFormFile postedFile)
        //{
        //    if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
        //       //string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
        //       // string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
        //       //string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        //    { return true; }
        //    return false;
        //}

        //public static bool IsExWorkHistoryAllowedType(IFormFile postedFile)
        //{
        //    if (string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
        //       //string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
        //       // string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
        //       //string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        //    { return true; }
        //    return false;
        //}

        public static bool IsUserPersonalMessageAllowedType(IFormFile postedFile)
        {
            if (
                string.Equals(postedFile.ContentType, "application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase)
               )
            {

                return true;
            }
            return false;
        }

        //public static bool IsUserGroupMessageAllowedType(IFormFile postedImage)
        //{
        //    if (postedImage is null)
        //    {
        //        return false;
        //    }

        //    if (string.Equals(postedImage.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedImage.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedImage.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public static bool IsRadioOrWeeklyNewsAllowedType(IFormFile postedFile)
        //{
        //    if (string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "audio/mpeg", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "audio/mp3", StringComparison.OrdinalIgnoreCase) ||
        //       string.Equals(postedFile.ContentType, "audio/m4a", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        internal static bool IsGroupMessageAllowedType(IFormFile postedImage)
        {
            if (postedImage is null)
            {
                return false;
            }

            if (string.Equals(postedImage.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedImage.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(postedImage.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static bool IsPayButtonStateExceptAllowedType(IFormFile postedFile)
        {
            if (
                string.Equals(postedFile.ContentType, "application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(postedFile.ContentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase)
               )
            {

                return true;
            }
            return false;
        }

    }
}
