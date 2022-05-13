using Certes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LcaService.LetsEncrypt
{
    public interface IAcmeProvider
    {
        Task<string> CreateAccount(string mailId);
        Task<Uri> CreateOrder(IList<string> sans);
        Task<string> Download(Uri location, CsrInfo csrInfo);
        Task UpdateAccount(string mailId);
    }
}