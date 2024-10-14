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


        /// <summary>
        /// Get player casino wagers
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<PlayerWadgerResponse> GetPlayerCasinoWagers(Guid playerId, int pageSize = 10, int page = 1)
        {
            try
            {
                // Get players casino wagers paginated list
                var playerWagers = _dataBaseContext.CasinoWager
                   .Where(m => m.AccountId == playerId)
                   .OrderByDescending(m => m.CreatedDateTime)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

                // Count players casino wagers
                var totalWagers = _dataBaseContext.CasinoWager
                    .Count(m => m.AccountId == playerId);

                var totalPages = (int)Math.Ceiling((double)totalWagers / pageSize);

                var response = new PlayerWadgerResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = totalWagers,
                    TotalPages = totalPages,
                    PlayerWadgers = playerWagers.Select(pw => new PlayerWadger
                    {
                        Amount = pw.Amount,
                        CreatedDate = pw.CreatedDateTime,
                        Game = pw.GameName,
                        Provider = pw.Provider,
                        WagerId = pw.WagerId
                    }).ToList()
                };

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
