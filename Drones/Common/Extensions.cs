using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones.Common
{
    public static class MathFunctions
    {
        public static int CalculateDistance(int startR,int startC, int endR, int endC)
        {
            var absR = Math.Abs(startR - endR);
            var absC = Math.Abs(startC - endC);
            var flyPath = absR * absR + absC * absC;
            return (int)Math.Ceiling(Math.Sqrt(flyPath));
        }
    }
}
