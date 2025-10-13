using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;
using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Common.Interfaces
{
    /// <summary>
    /// سرویس ذخیره فایل در درایو و ذخیره داکیومنت مربوط به ان در دیتابیس
    /// </summary>
    public interface IDocumentService
    {
        //Task<bool> CheckExistNameAsync(Guid? id, string name, CancellationToken cancellationToken);
        //void CreateDocument(string name);
        //void RemoveFolder(string folderName);
        Task<Document> InsertDocument(IFormFile doc, bool isPublicFiles, DocumentFolder folderToSaveEnum, CancellationToken cancellationToken);
        Task<List<Document>> InsertDocuments(List<IFormFile> docs, bool isPublicFiles, DocumentFolder folderToSaveEnum, CancellationToken cancellationToken);
        void DeleteDocument(Guid documentId, DocumentFolder folderToSaveEnum);
        void DeleteDocuments(IEnumerable<Guid> documentIds, DocumentFolder folderToSaveEnum);
        Task<FileRecordDto> GetFileRecord(Guid id, CancellationToken cancellationToken);

    }
}
