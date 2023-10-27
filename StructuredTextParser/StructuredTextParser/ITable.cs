using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuredTextParser
{
    internal interface ISqlTable
    {
        public string Server { get; set; }

        public bool TrustedConnection { get; set; }

        public string IntegratedSecurity { get; set; }

        public string InitialCatalog { get; set; }

        public string? Table { get; set; }

        public string[]? DataNames { get; set; }

        public string[]? TableDataNames { get; set; }

        public string[]? DataTypes { get; set; }

        public string[]? IsDataNull { get; set; }
    }
}
