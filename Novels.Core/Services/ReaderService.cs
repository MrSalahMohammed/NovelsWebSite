using AutoMapper;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Repositories;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.Core.Services
{
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IMapper _mapper;

        public ReaderService(IReaderRepository readerRepository, IMapper mapper)
        {
            _readerRepository = readerRepository;
            _mapper = mapper;
        }

        public async Task<bool> DeleteReaderAsync(int readerId)
        {
            var reader = await _readerRepository.GetReaderByID(readerId);
            if (reader is null)
                return false;

            bool Succeeded = await _readerRepository.DeleteReader(reader);
            await _readerRepository.SaveChangesAsync();
            return Succeeded;
        }

        public async Task<List<ReadingProgressDto>> GetReadingHistoryAsync(int readerId)
        {
            var History = await _readerRepository.GetReadingHistoryAsync(readerId);
            return _mapper.Map<List<ReadingProgressDto>>(History);
        }

        public async Task<bool> UpdateReaderDataAsync(int readerId, UpdateReaderRequest request)
        {
            var reader = await _readerRepository.GetReaderByID(readerId);
            if (reader is null)
                return false;

            if (request is null)
                return false;

            var user = new ApplicationUser
            {
                FName = request.FName,
                LName = request.FName,
                RecoveryEmail = request.FName,
                PhoneNumber = request.FName,
                Email = request.Email,
            };

            await _readerRepository.UpdateReaderDataAsync(user);
            await _readerRepository.SaveChangesAsync();
            return true;
        }
    }
}
