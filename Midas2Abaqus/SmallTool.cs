using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    class SmallTool
    {
        SmallTool()
        {

        }

        //三角形面积
        public double GetTriangleArea(double x1,double y1,double z1,double x2,double y2,double z2,double x3,double y3,double z3)
        {
            return 0.0;
        }

        //三角形投影面积
        public double GetTrianglePlaneArea(double x1,double y1,double x2,double y2,double x3,double y3)
        {
            return 0.0;
        }

        //统计全部板单元面积
        public double GetPlateTotalArea()
        {
            return 0.0;
        }

        //统计全部板单元投影面积
        public double GetPlateTotalPlaneArea()
        {
            return 0.0;
        }
    }
}
