using Microsoft.Extensions.Logging;
using OT.Assessment.Database;
using OT.Assessment.Models.Response;
using OT.Assessment.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _dataBaseContext;

        public PlayerService(ApplicationDbContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public async Task<List<PlayerWadgerResponse>> GetPlayerCasinoWagers(Guid playerId, int pageSize = 10, int page = 1)
        {
            try
            {
                // Get players casino wagers
                var playerWagers = _dataBaseContext.CasinoWager
                     .Where(m => m.AccountId == playerId)
                     .ToList();
                
                var response = playerWagers.
                    Select(item => new PlayerWadgerResponse
                    {
                        Amount = item.Amount,
                        CreatedDate = item.CreatedDateTime,
                        Game = item.GameName,
                        Provider = item.Provider,
                        WagerId = item.WagerId
                    }).ToList();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TopSpenderResponse>> GetTopSpenders(int count)
        {
            try
            {
                var players = _dataBaseContext.CasinoWager
                    .GroupBy(p => p.AccountId);

                var response = players.Select(t => new TopSpenderResponse
                    {
                        AccountId = t.Key,
                        Username = t.First().Username,
                        TotalAmountSpend = t.Sum(a => a.Amount)
                    })
                    .OrderByDescending(ts => ts.TotalAmountSpend)
                    .Take(count)
                    .ToList();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
