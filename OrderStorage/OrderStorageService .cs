using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Data.Tables.Models;
using LcaService.DataContracts;

namespace LcaService.OrderStorage
{
    public class OrderStorageService : IOrderStorageService
    {
        private const string TableName = "Item";
        private readonly IConfiguration _configuration;
        private TableClient _tableClient;

        public OrderStorageService(IConfiguration configuration)
        {
            _configuration = configuration;

            var storageAccount = _configuration["StorageConnectionString"];
            //_tableClient = new TableClient(storageAccount, TableName, new DefaultAzureCredential());
            _tableClient = new TableClient(storageAccount, TableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task<Order> RetrieveAsync(string id)
        {
            return TableEntityExtension.CreateOrder(_tableClient.Query<TableEntity>(entity => entity.RowKey == id).FirstOrDefault());

        }

        public async Task AddEntity(Order entity)
        {
            await _tableClient.AddEntityAsync(TableEntityExtension.CreateOrderTableEntity(entity));
        }

        public async Task UpdateEntity(Order entity)
        {
            await _tableClient.UpsertEntityAsync(TableEntityExtension.CreateOrderTableEntity(entity),TableUpdateMode.Replace);
        }


        public async Task DeleteAsync(Order entity)
        {
            await _tableClient.DeleteEntityAsync("Order", entity.Id);
        }
    }
}
