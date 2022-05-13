using Certes;
using LcaService.DNSService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.LetsEncrypt
{
    public class AcmeProvider : IAcmeProvider
    {
        private readonly IAcmeContext _acmeContext;
        public AcmeProvider(IAcmeContext acmeContext)
        {
            _acmeContext = acmeContext;
            CreateAccount("rajasekhar.reddy@siemens-healthineers.com").GetAwaiter();
        }
        public async Task<string> CreateAccount(string mailId)
        {
            await _acmeContext.NewAccount(mailId, true);
            return _acmeContext.AccountKey.ToPem();
        }
        public async Task UpdateAccount(string mailId)
        {
            var account = await _acmeContext.Account();
            await account.Update(new[] { mailId }, true);
        }
        public async Task<Uri> CreateOrder(IList<string> sans)
        {
            var order = await _acmeContext.NewOrder(sans);
            foreach (var authorization in await order.Authorizations())
            {
                var challenge = await authorization.Dns();
                var dnsTxtRecordText = _acmeContext.AccountKey.DnsTxt(challenge.Token);
                
                await AddChallengeToDNS(dnsTxtRecordText);
                await challenge.Validate();
            }

            return order.Location;
        }
        public async Task<string> Download(Uri location, CsrInfo csrInfo)
        {
            var order = _acmeContext.Order(location);

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
            var certificate = await order.Generate(csrInfo, privateKey);

            return certificate.Certificate.ToPem();
        }
        private async Task AddChallengeToDNS(string challenge)
        {
            await DnsManagement.Update(challenge);
        }
    }
}
