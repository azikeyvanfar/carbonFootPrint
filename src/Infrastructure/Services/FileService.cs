using System.IO;
using ContractorBackend.Application.Common.Interfaces;

namespace ContractorBackend.Infrastructure.Services
{
    public class FileService : IFileService
    {
        public string CorrectFileNameExtension(string fileName)
        {
            string imageExtention = Path.GetExtension(fileName);

            while (!string.IsNullOrEmpty(imageExtention))
            {
                var extention = imageExtention;
                fileName = fileName.Replace(imageExtention, "");
                imageExtention = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(imageExtention))
                {
                    fileName += extention;
                    break;
                }
            }

            return fileName;
        }
    }
}
