using OT.Assessment.Models;

namespace OT.Assessment.Service.Interface
{
    public interface IRabbitMQService
    {
        Task SendCasinoWagerAsync(CasinoWager casinoWager);

        Task ReadCasinoWagerAsync();
    }
}
