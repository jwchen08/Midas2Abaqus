using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    //节点类
    public class Node
    {
        public int NO { set; get; }
        public double X { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }
        public Node()
        {
            NO = 0;
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }
        public Node(int no,double x,double y,double z)
        {
            NO = no;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
