using System.Threading.Tasks;

namespace pvWay.IpApi.Core
{
    public interface ILocalizer
    {
        Task<ILocalizerResult> LocalizeAsync(string ip);
    }
}