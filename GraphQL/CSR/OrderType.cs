using GraphQL.Types;
using LcaService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.GraphQL.CSR
{
    public class OrderType : ObjectGraphType<Order>
    {
        public OrderType()
        {
            Field(m => m.Id, true);
            Field(m => m.SAN, true);
            Field(m => m.Status,true);
            Field(m => m.Certificate, true);
        }
    }
}
