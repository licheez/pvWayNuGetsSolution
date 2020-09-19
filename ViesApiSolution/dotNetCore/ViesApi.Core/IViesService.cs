using System.Threading.Tasks;

namespace pvWay.ViesApi.Core
{
    public interface IViesService
    {
        Task<IViesResult> CheckVatAsync(string countryCode, string vatNumber);
    }
}