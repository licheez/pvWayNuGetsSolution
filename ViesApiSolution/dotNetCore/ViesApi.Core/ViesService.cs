using pvWay.MethodResultWrapper.Core;
using System;
using System.Threading.Tasks;
using ViesWebService;

namespace pvWay.ViesApi.Core
{
    public class ViesService : IViesService
    {
        private readonly ILoggerService _ls;

        public ViesService(ILoggerService ls)
        {
            _ls = ls;
        }

        public async Task<IMethodResult<IViesResult>> CheckVatAsync(string countryCode, string vatNumber)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                var err = new MethodResult<IViesResult>("country code should not be null or empty",
                    SeverityEnum.Error);
                _ls.Log(err);
                return err;
            }
            if (string.IsNullOrEmpty(vatNumber))
            {
                var err = new MethodResult<IViesResult>("vat number should not be null or empty",
                    SeverityEnum.Error);
                _ls.Log(err);
                return err;
            }
            vatNumber = vatNumber
                .Replace(" ", "")
                .Replace(".", "")
                .Replace("-", "");

            var request = new checkVatRequest(countryCode, vatNumber);
            try
            {
                var vs = new checkVatPortTypeClient();
                var vr = await vs.checkVatAsync(request);
                var res = new ViesResult(vr);
                return new MethodResult<IViesResult>(res);
            }
            catch (Exception e)
            {
                _ls.Log(e);
                return new MethodResult<IViesResult>(e);
            }
        }
    }
}
