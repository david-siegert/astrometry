using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrometrie
{
    class CoordinatesModel
    {
        public double Ra { get; set; }
        public double Dec { get; set; }
        public double Radius { get; set; }


        public string HumanReadableRa()
        {
            int deg = (int)(Ra/15);

            int min = (int)((Ra/15 - deg) * 60);

            double sec = (((Ra/15 - deg) * 60) - min) * 60;

            sec = Math.Round(sec, 3);

            string raString = String.Format("{0}h {1}m {2}s", deg, min, sec);

            return raString;
        }

        public string HumanReadableDec()
        {
            string plus = "+";

            if (Dec < 0)
            {
                Dec = -Dec;
                plus = "-";
            }

            int deg = (int)Dec;

            int min = (int)((Dec - deg) * 60);

            double sec = (((Dec - deg) * 60) - min) * 60;

            sec = Math.Round(sec, 3);

            string raString = String.Format("{0}{1}° {2}' {3}\"", plus, deg, min, sec);

            return raString;
        }

        public string HumanReadableRadius()
        {
            return Math.Round(Radius, 3).ToString() + " deg";
        }
    }
}
