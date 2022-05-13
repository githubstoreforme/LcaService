using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.GraphQL
{
    public class CASchema : Schema
    {

        public CASchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetService(typeof(CAQueries)) as CAQueries;
            Mutation = serviceProvider.GetService(typeof(CertificateAuthorityMutation)) as CertificateAuthorityMutation;
        }
    }
}
