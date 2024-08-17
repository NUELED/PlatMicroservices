using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;   
            _mapper = mapper;                  
        }


        [HttpGet]       
        public async Task< ActionResult<IEnumerable<CommandReadDto>> >  GetCommandsForPlatform( int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsPlatforms: {platformId}");

            if(! _repository.PlaformExits(platformId)) 
            { 
                return NotFound();  
            }

            var commands = _repository.GetCommandsForPlatform(platformId);

            return Ok( _mapper.Map<IEnumerable<CommandReadDto>>(commands));    
        }


        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandPlatforms: {platformId} / {commandId} ");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }
             
            var commands = _repository.GetCommand(platformId, commandId);
            if(commands == null) 
            {
                return NotFound();  
            }

            return Ok(_mapper.Map<CommandReadDto>(commands));
        }



        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandPlatforms: {platformId} ");

            if (!_repository.PlaformExits(platformId))
            {
                return NotFound();
            }

            var commands = _mapper.Map<Command>(commandDto);

            _repository.CreateCommand(platformId, commands);
            _repository.SaveChanges();  


            var commandReadDto =  _mapper.Map<CommandReadDto>(commands) ;

            return CreatedAtAction (nameof(GetCommandForPlatform), new {platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);   
        }






    }
}
