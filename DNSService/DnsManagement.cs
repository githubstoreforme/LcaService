using Microsoft.Azure.Management.Dns.Fluent;
using Microsoft.Azure.Management.Dns.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Rest;
using System;
using System.Threading.Tasks;

namespace LcaService.DNSService
{
    public static class DnsManagement
    {        

        public static async Task Update(string challenge)
        {
            string tenantId = Environment.GetEnvironmentVariable("TenantId", EnvironmentVariableTarget.Process) ??
                "cfd26b50-fb8f-44cf-87b2-d5df3d15d884";
            var rmastp = new AzureServiceTokenProvider();
            string armAuthToken = await rmastp.GetAccessTokenAsync("https://management.azure.com/", tenantId);


            var astp = new AzureServiceTokenProvider();
            string graphToken = await astp.GetAccessTokenAsync("https://graph.windows.net/", tenantId);


            var credentials = new AzureCredentials(
                        new TokenCredentials(armAuthToken),
                        new TokenCredentials(graphToken),
                        tenantId,
                        AzureEnvironment.AzureGlobalCloud);


            var client = RestClient
                                .Configure()
                                .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                                .WithCredentials(credentials)
                                .Build();

            using (var dnsClient = new DnsManagementClient(client))
            {
                dnsClient.SubscriptionId = "44650d0a-f134-44e3-94f1-550c71b27d99";
                var name = "_acme-challenge";
                await dnsClient.RecordSets.CreateOrUpdateAsync(
                    "letsencryptsample",
                    "startsampleapptest.online",
                    name,
                    RecordType.TXT,
                    new RecordSetInner(
                        name: name,
                        tTL: 1,
                        txtRecords: new[] { new TxtRecord(new[] { challenge }) }));
            }
        }
      
    }
}
