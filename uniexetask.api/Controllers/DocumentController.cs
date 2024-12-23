﻿using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly string _bucketName = "exeunitask.appspot.com";
        private readonly StorageClient _storageClient;
        private readonly IProjectService _projectService;
        private readonly IDocumentService _documentService;
        private readonly IUserService _userSerivce;

        public DocumentController(StorageClient storageClient, IProjectService projectService, IDocumentService documentService, IUserService userSerivce)
        {
            _storageClient = storageClient;
            _projectService = projectService;
            _documentService = documentService;
            _userSerivce = userSerivce;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetDocuments(int userId)
        {
            var project = await _projectService.GetProjectByUserId(userId);
            if (project == null)
            {
                return NotFound("Project not found for the given student.");
            }

            var documents = await _documentService.GetDocumentsByProjectId(project.ProjectId);
            var storageObjects = _storageClient.ListObjects(_bucketName, $"Project{project.ProjectId}/").ToList();

            var documentResponses = new List<DocumentRespone>();

            foreach (var doc in documents)
            {
                var uploadUser = await _userSerivce.GetUserById(doc.UploadBy);
                var modifyUser = doc.ModifiedBy.HasValue ? await _userSerivce.GetUserById(doc.ModifiedBy.Value) : null;
                var storageObject = storageObjects.FirstOrDefault(obj => obj.Name == doc.Url);

                var documentResponse = new DocumentRespone
                {
                    DocumentId = doc.DocumentId,
                    ProjectId = doc.ProjectId,
                    Name = doc.Name,
                    Type = doc.Type,
                    Url = doc.Url,
                    UploadBy = uploadUser.FullName,
                    ModifiedBy = modifyUser?.FullName,
                    ModifiedDate = doc.ModifiedDate,
                    Size = storageObject != null && storageObject.Size <= long.MaxValue ? (long)storageObject.Size : 0
                };
                documentResponses.Add(documentResponse);
            }

            ApiResponse<IEnumerable<DocumentRespone>> response = new ApiResponse<IEnumerable<DocumentRespone>>
            {
                Data = documentResponses
            };


            return Ok(response);
        }

        [HttpGet("getByProjectId/{projectId}")]
        public async Task<IActionResult> GetDocumentsByProjectId(int projectId)
        {
            var project = await _projectService.GetProjectById(projectId);
            if (project == null)
            {
                return NotFound("Project not found for the given student.");
            }

            var documents = await _documentService.GetDocumentsByProjectId(project.ProjectId);
            var storageObjects = _storageClient.ListObjects(_bucketName, $"Project{project.ProjectId}/").ToList();

            var documentResponses = new List<DocumentRespone>();

            foreach (var doc in documents)
            {
                var uploadUser = await _userSerivce.GetUserById(doc.UploadBy);
                var modifyUser = doc.ModifiedBy.HasValue ? await _userSerivce.GetUserById(doc.ModifiedBy.Value) : null;
                var storageObject = storageObjects.FirstOrDefault(obj => obj.Name == doc.Url);

                var documentResponse = new DocumentRespone
                {
                    DocumentId = doc.DocumentId,
                    ProjectId = doc.ProjectId,
                    Name = doc.Name,
                    Type = doc.Type,
                    Url = doc.Url,
                    UploadBy = uploadUser.FullName,
                    ModifiedBy = modifyUser?.FullName,
                    ModifiedDate = doc.ModifiedDate,
                    Size = storageObject != null && storageObject.Size <= long.MaxValue ? (long)storageObject.Size : 0
                };
                documentResponses.Add(documentResponse);
            }

            ApiResponse<IEnumerable<DocumentRespone>> response = new ApiResponse<IEnumerable<DocumentRespone>>
            {
                Data = documentResponses
            };


            return Ok(response);
        }

        private string MapMimeTypeToDocumentType(string mimeType)
        {
            var mimeTypeToDocumentType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "application/msword", "DOC" },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "DOCX" },
        { "application/vnd.ms-excel", "XLS" },
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "XLSX" },
        { "application/pdf", "PDF" },
        { "text/plain", "TXT" },
        { "image/jpeg", "JPG" },
        { "image/png", "PNG" },
        { "application/zip", "ZIP" },
        { "application/x-rar-compressed", "RAR" }
    };

            return mimeTypeToDocumentType.TryGetValue(mimeType, out var documentType)
                ? documentType
                : throw new InvalidOperationException($"Unsupported MIME type: {mimeType}");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var project = await _projectService.GetProjectByUserId(userId);
            if (project == null)
                return NotFound(new ApiResponse<Document>() { Success = false, ErrorMessage = "Project not found for the given student." });

            var existedDocument = await _documentService.GetDocumentByName($"Project{project.ProjectId}/{file.FileName}");
            if (existedDocument != null)
                return Conflict(new ApiResponse<Document>() { Success = false ,ErrorMessage = "Document with the same name already exists." });
            var document = await _documentService.UploadDocument(new Document
            {
                Name = file.FileName,
                ProjectId = project.ProjectId,
                Type = MapMimeTypeToDocumentType(file.ContentType),
                Url = $"Project{project.ProjectId}/{file.FileName}",
                UploadBy = userId
            });

            var fileName = file.FileName;
            var filePath = document.Url;

            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, filePath, file.ContentType, stream);
            }
            ApiResponse<Document> respone = new ApiResponse<Document>();
            respone.Data = document;

            return Ok(respone);
        }

        [HttpPost("overwrite")]
        public async Task<IActionResult> OverwriteDocument(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var project = await _projectService.GetProjectByUserId(userId);
            if (project == null)
                return NotFound("Project not found for the given student.");
            var modifyUser = await _userSerivce.GetUserById(userId);
            var document = await _documentService.OverWriteDocument(new Document()
            {
                Url = $"Project{project.ProjectId}/{file.FileName}",
                ModifiedBy = modifyUser.UserId,
                ModifiedDate = DateTime.Now,
            });

            var fileName = file.FileName;
            var filePath = $"Project{project.ProjectId}/{file.FileName}";
            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, filePath, file.ContentType, stream);
            }
            ApiResponse<Document> respone = new ApiResponse<Document>();
            respone.Data = document;
            return Ok(respone);
            
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var document = await _documentService.GetDocumentById(documentId);
            if (document == null)
                return NotFound("Document not found.");

            await _storageClient.GetObjectAsync(_bucketName, document.Url);

            var credential = GoogleCredential.FromFile(
                Path.Combine(Directory.GetCurrentDirectory(), "exeunitask-firebase-adminsdk-3jz7t-66373e3f35.json")
            ).UnderlyingCredential as ServiceAccountCredential;

            if (credential == null)
                return StatusCode(500, "Failed to load service account credentials.");

            var signedUrl = UrlSigner.FromCredential(credential).Sign(
                _bucketName,
                document.Url,
                TimeSpan.FromHours(1),
                HttpMethod.Get
            );

            return Ok(new { Url = signedUrl });
        }


        [HttpDelete("documentid")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var documentToDelete = await _documentService.DeleteDocumentById(documentId);
            if (documentToDelete == null)
            {
                return NotFound("Document not found.");
            }

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, documentToDelete.Url);
                return Ok(new { Message = "File deleted successfully" });
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return NotFound("File not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }

        [HttpGet("getallprojectdocumentstorage")]
        public async Task<IActionResult> GetAllProjectDocumentsStorage()
        {
            var projects = await _projectService.GetAllProjects();
            List<ProjectDocumentsStorageRespone> projectDocumentsStorageRespones = new List<ProjectDocumentsStorageRespone>();
            foreach (var project in projects) 
            {
                var storageObjects = _storageClient.ListObjects(_bucketName, $"Project{project.ProjectId}/").ToList();
                long total = 0;
                foreach (var storageObject in storageObjects) 
                {
                    total += storageObject != null && storageObject.Size <= long.MaxValue ? (long)storageObject.Size : 0;
                }
                projectDocumentsStorageRespones.Add(new ProjectDocumentsStorageRespone
                {
                    Project = project,
                    TotalStorage = total,
                });
            }
            ApiResponse<IEnumerable<ProjectDocumentsStorageRespone>> respone = new ApiResponse<IEnumerable<ProjectDocumentsStorageRespone>>();
            respone.Data = projectDocumentsStorageRespones;

            return Ok(respone);
        }
    }
}
