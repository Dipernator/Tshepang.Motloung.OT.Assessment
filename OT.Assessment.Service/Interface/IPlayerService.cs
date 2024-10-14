using OT.Assessment.Models.Response;

namespace OT.Assessment.Service.Interface
{
    public interface IPlayerService
    {
        Task<List<TopSpenderResponse>> GetTopSpenders(int count);

        Task<List<PlayerWadgerResponse>> GetPlayerCasinoWagers(Guid playerId, int pageSize = 10, int page = 1);
    }
}
