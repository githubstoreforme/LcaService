using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LcaService.GraphQL.CSR
{
    public class CSRInputType : InputObjectGraphType
    {
        public CSRInputType()
        {
            Name = "CSRInput";
            Field<NonNullGraphType<StringGraphType>>("san");
            Field<NonNullGraphType<StringGraphType>>("countryname");
            Field<NonNullGraphType<StringGraphType>>("commonname");

        }
    }
}
