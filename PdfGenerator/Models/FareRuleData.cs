using Swashbuckle.Examples.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdfGenerator.Models
{
    [Samplify]
    public class FareRuleData:IOutputData
    {
        [Sample("This is sample Heading")]
        public string Heading { get; set; }
        [Sample("This is sample Body Content")]
        public string Body { get; set; }
        [Sample("Midhun")]
        public string Name { get; set; }
    }
}
