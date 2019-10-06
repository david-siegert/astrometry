using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrometrie
{
    class ResponseJson
    {
        public string Status { get; set; }
        public string Session { get; set; } //sessionkey
        public string SubId { get; set; }

        public int?[][] job_calibrations { get; set; }

        public int?[] jobs { get; set; }
    }

    class RequestJson
    {
        public string session { get; set; }

    }

}

