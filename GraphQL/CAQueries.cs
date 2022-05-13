using GraphQL;
using GraphQL.Types;
using LcaService.GraphQL.CSR;
using LcaService.OrderStorage;
using System;

namespace LcaService.GraphQL
{
    public class CAQueries : ObjectGraphType
    {
        private readonly IOrderStorageService _orderStorageService;
        public CAQueries(IOrderStorageService orderStorageService)
        {
            _orderStorageService = orderStorageService;

            Field<OrderType>(
            name: "certificateOrderByOrderId",
             arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                ),
            resolve: context =>
            {
                var id = context.GetArgument<String>("id");
                return _orderStorageService.RetrieveAsync(id).GetAwaiter().GetResult();
            }
        );
        }
    }
}
