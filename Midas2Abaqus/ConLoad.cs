using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    //节点集中荷载
    public class ConLoad
    {
        //******Midas中
        //*CONLOAD    ; Nodal Loads
        //; NODE_LIST, FX, FY, FZ, MX, MY, MZ, GROUP
        //   7, 0, 0, -60, 0, 0, 0, 
        //******Abaqus中
        //*CLOAD
        //<节点编号或节点集合>,<自由度编号>,<荷载值>
        //NSet-1,2,-50000
        //2为自由度编号，表示沿全局坐标轴Y向
        public int NodeNO { set; get; }
        public double FX { set; get; }
        public double FY { set; get; }
        public double FZ { set; get; }
        public double MX { set; get; }
        public double MY { set; get; }
        public double MZ { set; get; }
        public string LoadCaseType { set; get; }//所属荷载工况类型
        public ConLoad()
        {

        }

        public ConLoad(int nodeNo,double fx,double fy,double fz,double mx,double my,double mz)
        {
            NodeNO = nodeNo;
            FX = fx;
            FY = fy;
            FZ = fz;
            MX = mx;
            MY = my;
            MZ = mz;
        }

        public ConLoad(int no, double fx, double fy, double fz, double mx, double my, double mz,string loadCase)
        {
            NodeNO = no;
            FX = fx;
            FY = fy;
            FZ = fz;
            MX = mx;
            MY = my;
            MZ = mz;
            LoadCaseType = loadCase;
        }
    }
}
