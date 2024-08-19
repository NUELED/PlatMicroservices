using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _repo;
        public GrpcPlatformService(IMapper mapper, IPlatformRepo repo)
        {           
            _mapper = mapper;   
            _repo = repo;
        }


        public override async Task<PlatformResponse> GetAllPlatforms (GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();  
            var platforms = _repo.GetAllPlatforms();

            foreach (var platform in platforms) 
            { 
               response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }

           return await  Task.FromResult(response);
        }









    }
}

