using EventHub.BLL.Configuration;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EventHub.BLL.Services.Implementations
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventService _eventService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly AttachmentStorageOptions _storage;

        public AttachmentService(
            IUnitOfWork unitOfWork,
            IEventService eventService,
            IHostEnvironment hostEnvironment,
            IOptions<AttachmentStorageOptions> storageOptions)
        {
            _unitOfWork = unitOfWork;
            _eventService = eventService;
            _hostEnvironment = hostEnvironment;
            _storage = storageOptions.Value;
        }

        public async Task<EventAttachment> UploadForEventAsync(string eventId, Stream fileStream, string originalFileName, CancellationToken cancellationToken = default)
        {
            var @event = await _eventService.GetEventByIdAsync(eventId);
            if (@event == null)
                throw new KeyNotFoundException("Event not found.");

            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("File name is required.");

            var root = Path.Combine(_hostEnvironment.ContentRootPath, _storage.RelativeRoot.TrimStart('/', '\\'));
            Directory.CreateDirectory(root);

            var safeName = Path.GetFileName(originalFileName);
            var storedName = $"{Guid.NewGuid():N}_{safeName}";
            var physicalPath = Path.Combine(root, storedName);

            await using (var fs = File.Create(physicalPath))
            {
                await fileStream.CopyToAsync(fs, cancellationToken);
            }

            var relativePath = Path.Combine(_storage.RelativeRoot, storedName).Replace('\\', '/');
            var attachment = new EventAttachment
            {
                EventId = eventId,
                FilePath = relativePath
            };

            await _unitOfWork.EventAttachments.AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();
            return attachment;
        }

        public Task<IEnumerable<EventAttachment>> GetByEventAsync(string eventId, CancellationToken cancellationToken = default) =>
            _unitOfWork.EventAttachments.GetByEventAsync(eventId);

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var attachment = await _unitOfWork.EventAttachments.GetByIdAsync(id);
            if (attachment == null)
                throw new KeyNotFoundException("Attachment not found.");

            var fullPath = Path.Combine(_hostEnvironment.ContentRootPath, attachment.FilePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _unitOfWork.EventAttachments.Remove(attachment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
