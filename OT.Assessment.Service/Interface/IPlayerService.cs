using OT.Assessment.Models;
using OT.Assessment.Models.Response;

namespace OT.Assessment.Service.Interface
{
    public interface IPlayerService
    {
        Task<List<TopSpenderResponse>> GetTopSpenders(int count);

        Task<PlayerWadgerResponse> GetPlayerCasinoWagers(Guid playerId, int pageSize = 10, int page = 1);

        Task<bool> SaveCasinoWagerAsync(CasinoWager casinoWager);

        Task<bool> IsDuplicate(Guid wagerId);
    }
}
