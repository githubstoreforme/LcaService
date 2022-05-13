using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.DataContracts
{
    public class CertificateSigningRequest
    {
        public string SAN { get; set; }
        public string CountryName { get; set; }
        public string State { get; set; }
        public string Locality { get; set; }
        public string Organization { get; set; }
        public string OrganizationUnit { get; set; }
        public string CommonName { get; set; }

    }
}
