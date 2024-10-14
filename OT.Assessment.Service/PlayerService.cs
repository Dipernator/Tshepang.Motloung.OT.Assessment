using OT.Assessment.Database;
using OT.Assessment.Models;
using OT.Assessment.Models.Response;
using OT.Assessment.Service.Interface;
using System.Data.Entity;

namespace OT.Assessment.Service
{
    public class PlayerService : IPlayerService, IDisposable
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

        /// <summary>
        /// Get top spenders
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Save casino wager
        /// </summary>
        /// <param name="casinoWager"></param>
        /// <returns></returns>
        public async Task<bool> SaveCasinoWagerAsync(CasinoWager casinoWager)
        {
            try
            {
                _dataBaseContext.CasinoWager.AddAsync(casinoWager);
                if (_dataBaseContext.SaveChanges() > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Check for Duplicate using wagerId
        /// </summary>
        /// <param name="wagerId"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicate(Guid wagerId)
        {
            return _dataBaseContext.CasinoWager
                .Any(w => w.WagerId == wagerId);
        }

        public void Dispose()
        {
            _dataBaseContext?.Dispose();
        }
    }
}
