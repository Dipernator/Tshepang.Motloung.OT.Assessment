using Azure;
using Microsoft.AspNetCore.Mvc;
using OT.Assessment.Models;
using OT.Assessment.Service;
using OT.Assessment.Service.Interface;

namespace OT.Assessment.App.Controllers
{
  
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IPlayerService _playerService;

        public PlayerController(IRabbitMQService rabbitMQService, IPlayerService playerService)
        {
            _rabbitMQService = rabbitMQService;
            _playerService = playerService;
        }

        //POST api/player/casinowager
        [HttpPost]
        [Route("casinowager")]
        public async Task<IActionResult> Casinowager([FromBody] CasinoWager casinoWagerRequest) 
        {
            var result = _rabbitMQService.SendCasinoWagerAsync(casinoWagerRequest);
            return Ok(result);
        }

        //GET api/player/{playerId}/wagers
        [HttpGet]
        [Route("{playerId}/wagers")]
        public async Task<IActionResult> GetPlayerWagers(Guid playerId)
        {
            var playerWagers = await _playerService.GetPlayerCasinoWagers(playerId);

            if (playerWagers == null || !playerWagers.Any())
            {
                return NotFound($"Wagers for {playerId} is not found");
            }

            return Ok(playerWagers);
        }

        //GET api/player/topSpenders?count=10        
        [HttpGet]
        [Route("api/player/topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count = 10)
        {
            var topSpenders = await _playerService.GetTopSpenders(count);

            if (topSpenders == null || !topSpenders.Any())
            {
                return NotFound($"");
            }

            return Ok(topSpenders);
        }
    }
}
