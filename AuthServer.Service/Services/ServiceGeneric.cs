using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services   
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;

        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork; 
            _genericRepository = genericRepository; 
        }

        public async Task<Response<TDto>> AddAsync(TDto dto) 
        {
            //TDto dan dönüştürülmüş TEntity olacak
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync() 
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id) 
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if (product==null)
            {
                return Response<TDto>.Fail("Id not found",404,true); 
            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity==null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }
            _genericRepository.Remove(isExistEntity);
            //state'ini deleted olarak işaretledik
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);  
        }

        public async Task<Response<NoDataDto>> Update(TDto dto,int id)  
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }
            //    _genericRepository.Update(isExistEntity);
            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(dto);

            _genericRepository.Update(isExistEntity);

            await _unitOfWork.CommitAsync();
            //204 durum kodu => no content => response body'sinde data olmayacak.
            return Response<NoDataDto>.Success(204); 
        }

        public async Task<Response<IEnumerable<TEntity>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            //where(x=>x.id>8)
            var list = _genericRepository.Where(predicate); 

            //list.Skip(4).Take(5); memory de track ediliyor. memory islemleri  
            return Response<IEnumerable<TEntity>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TEntity>>(await list.ToListAsync()), 200); 
        }
      
        
    }
}
