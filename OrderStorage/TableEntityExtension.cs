using Azure.Data.Tables;
using LcaService.DataContracts;
using System;

namespace LcaService.OrderStorage
{
    public static class TableEntityExtension 
    {
        public static TableEntity CreateOrderTableEntity(Order order)
        {
            return new TableEntity("Order", order.Id)
            {
                { "SAN",order.SAN },
                { "Status",order.Status },
                { "Certificate",order.Certificate },
                { "link",order.link?.AbsoluteUri }

            };
        }

        public static Order CreateOrder(TableEntity tableEntity)
        {
            return tableEntity == null ? new Order(): new Order { Id = tableEntity.RowKey, SAN = tableEntity["SAN"] as string, Status = tableEntity["Status"] as string, Certificate = tableEntity["Certificate"] as string, link = new Uri(tableEntity["link"] as string)  };
        }


        public static TableEntity CreateCsrTableEntity(CertificateSigningRequest csr, string id)
        {
            return new TableEntity("csr", id)
            {
                { "CommonName",csr.CommonName },
                { "CountryName",csr.CountryName },
                { "Locality",csr.Locality },
                { "Organization",csr.Organization },
                { "OrganizationUnit",csr.OrganizationUnit },
                { "State",csr.State },
            };
        }

        public static CertificateSigningRequest CreateCsr(TableEntity tableEntity)
        {
            return tableEntity == null ? new CertificateSigningRequest() : new CertificateSigningRequest { CommonName = tableEntity["CommonName"] as string, Locality = tableEntity["Locality"] as string, Organization = tableEntity["Organization"] as string , OrganizationUnit = tableEntity["OrganizationUnit"] as string , State = tableEntity["State"] as string };
        }


    }
}
