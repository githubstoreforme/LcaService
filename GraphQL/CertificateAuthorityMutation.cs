using GraphQL;
using GraphQL.Types;
using LcaService.DataContracts;
using LcaService.GraphQL.CSR;
using LcaService.OrderStorage;
using LcaService.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LcaService.GraphQL
{
    public class CertificateAuthorityMutation : ObjectGraphType
    {
        private readonly IQueueProvider _queueProvider;
        private readonly IOrderStorageService _orderStorageService;

        public CertificateAuthorityMutation(IOrderStorageService orderStorageService)
        {
            _queueProvider = new QueueProvider("incoming");
            _orderStorageService = orderStorageService;

            Field<OrderType>(
                "createCertificateOrder",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CSRInputType>> { Name = "CSR" }
                ),
                resolve: context =>
                {
                    var item = context.GetArgument<CertificateSigningRequest>("CSR");
                    var order = new Order { Id = Guid.NewGuid().ToString(), SAN = item.SAN, Status = "Pending" };
                    _orderStorageService.AddEntity(order).GetAwaiter().GetResult();
                    _queueProvider.SendMessage(JsonSerializer.Serialize(order)).GetAwaiter().GetResult();

                    return order;
                });
          
        }
    }
}
