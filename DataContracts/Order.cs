using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.DataContracts
{
    public class Order
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string SAN { get; set; }
        public string Certificate { get; set; }
        public Uri link { get; set; }
    }
}
