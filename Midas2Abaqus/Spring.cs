using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class Spring
    {
        //***Midas数据格式
        //*SPRING    ; Point Spring Supports
        //; NODE_LIST, Type, SDx, SDy, SDz, SRx, SRy, SRz, GROUP, FROMTYPE, EFFAREA, Kx, Ky, Kz                                                  ; LINEAR
        //; NODE_LIST, Type, Direction, Vx, Vy, Vz, Stiffness, GROUP, FROMTYPE, EFFAREA                                                          ; COMP, TENS
        //; NODE_LIST, Type, Multi-Linear Type, Direction, Vx, Vy, Vz, ax, ay, bx, by, cx, cy, dx, dy, ex, ey, fx, fy, GROUP, FROMTYPE, EFFAREA  ; MULTI
        //19to35by2 , LINEAR, 10000, 10000, 1e+007, 1e-009, 1e-009, 1e-009, NO, 0, 0, 0, 0, 0, 0, 正常支座, 0, 0, 0, 0, 0
        //36 , LINEAR, 10000, 10000, 1e+007, 1e-009, 1e-009, 1e-009, NO, 0, 0, 0, 0, 0, 0, 正常支座, 0, 0, 0, 0, 0
        //说明，SDx，SDy，SDz若没有局部坐标系则按整体坐标系规定
        
        //Abaqus数据格式
        //*Spring, elset=Springs/Dashpots-2-spring
        //2  自由度方向
        //1e+06  刚度值
        //*Element, type=Spring1, elset=Springs/Dashpots-2-spring
        //829, 3  单元组编号，节点号
        //830, 5
        //831, 20
        //说明，对每个存在弹性自由度的方向各添加一遍
        public int NodeNO { set; get; }
        public string Type { set; get; }
        public double SDx { set; get; }
        public double SDy { set; get; }
        public double SDz { set; get; }
        public double SRx { set; get; }
        public double SRy { set; get; }
        public double SRz { set; get; }
        public Spring()
        {
            NodeNO = 0;
            Type = "LINEAR";
            SDx = 0.0;
            SDy = 0.0;
            SDz = 0.0;
            SRx = 0.0;
            SRy = 0.0;
            SRz = 0.0;
        }
        public Spring(int no, string type, double sdx, double sdy, double sdz)
        {
            NodeNO = no;
            Type = type;
            SDx = sdx;
            SDy = sdy;
            SDz = sdz;
        }
        public Spring(int no, string type, double sdx, double sdy, double sdz, double srx, double sry, double srz)
        {
            NodeNO = no;
            Type = type;
            SDx = sdx;
            SDy = sdy;
            SDz = sdz;
            SRx = srx;
            SRy = sry;
            SRz = srz;
        }
        public string Spring2Abaqus(string rel)
        {
            int s1 = int.Parse(rel.Substring(3, 1));
            int s2 = int.Parse(rel.Substring(4, 1));
            int s3 = int.Parse(rel.Substring(5, 1));
            string abaRel = "";
            if (s1 == 0)
            {
                if (s2 == 0)
                {
                    if (s3 == 0)
                    {
                        abaRel = "";
                    }
                    else
                    {
                        abaRel = "M2";
                    }
                }
                else
                {
                    if (s3 == 0)
                    {
                        abaRel = "M1";
                    }
                    else
                    {
                        abaRel = "M1-M2";
                    }
                }
            }
            else
            {
                if (s2 == 0)
                {
                    if (s3 == 0)
                    {
                        abaRel = "T";
                    }
                    else
                    {
                        abaRel = "M2-T";
                    }
                }
                else
                {
                    if (s3 == 0)
                    {
                        abaRel = "M1-T";
                    }
                    else
                    {
                        abaRel = "ALLM";
                    }
                }
            }

            return abaRel;
        }

    }
}
