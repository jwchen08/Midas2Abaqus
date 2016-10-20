using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class ElasticLink
    {
        //***Midas数据格式
        //*ELASTICLINK
        //; iNO, iNODE1, iNODE2, LINK, ANGLE, SDx, SDy, SDz, SRx, SRy, SRz, bSHEAR, DRy, DRz, GROUP                       ; GEN
        //; iNO, iNODE1, iNODE2, LINK, ANGLE, bSHEAR, DRy, DRz, GROUP                                                     ; RIGID
        //; iNO, iNODE1, iNODE2, LINK, ANGLE, SDx, bSHEAR, DRy, DRz, GROUP                                                ; TENS,COMP
        //; iNO, iNODE1, iNODE2, LINK, ANGLE, (UN)SYM, NUM, DIST1, FORCE1 ... DIST10, FORCE10, DIR, bSHEAR, DRENDI, GROUP ; MULTI LINEAR
        //     1,  3331,  2341, GEN  ,     0, 1e+010, 1e+007, 1e+007, 0, 0, 0, NO, 0.5, 0.5, 
        //     2,  3330,  2302, GEN  ,     0, 1e+010, 1e+007, 1e+007, 0, 0, 0, NO, 0.5, 0.5, 
        //     3,  3329,  2263, GEN  ,     0, 1e+010, 1e+007, 1e+007, 0, 0, 0, NO, 0.5, 0.5, 
        //说明，两节点连线方向为SDx

        //Abaqus数据格式
        //*Spring, elset=Springs/Dashpots-1-spring
        //1, 1  两节点自由度方向
        //1e+06  刚度值
        //*Element, type=Spring2, elset=Springs/Dashpots-1-spring
        //304, 127, 1  弹性连接编号，节点1，节点2
        //305, 128, 2
        //306, 129, 3
        //说明，对每个存在弹性自由度的方向各添加一遍
        public int LinkNO { set; get; }
        public int NodeNo1 { set; get; }
        public int NodeNo2 { set; get; }
        public string LinkType { set; get; }
        public double Angle { set; get; }
        public double SDx { set; get; }
        public double SDy { set; get; }
        public double SDz { set; get; }
        public double SRx { set; get; }
        public double SRy { set; get; }
        public double SRz { set; get; }
        public ElasticLink()
        {
            LinkNO = 0;
            NodeNo1=0;
            NodeNo2 = 0;
            SDx = 0.0;
            SDy = 0.0;
            SDz = 0.0;
            SRx = 0.0;
            SRy = 0.0;
            SRz = 0.0;
        }
        public ElasticLink(int no, int n1, int n2, double sdx, double sdy, double sdz)
        {
            LinkNO = no;
            NodeNo1 = n1;
            NodeNo2 = n2;
            SDx = sdx;
            SDy = sdy;
            SDz = sdz;
        }
        public ElasticLink(int no, int n1, int n2,string type, double sdx, double sdy, double sdz, double srx, double sry, double srz)
        {
            LinkNO = no;
            NodeNo1 = n1;
            NodeNo2 = n2;
            LinkType = type;
            SDx = sdx;
            SDy = sdy;
            SDz = sdz;
            SRx = srx;
            SRy = sry;
            SRz = srz;
        }
        public string ElasticLink2Abaqus(string rel)
        {
            int s1 =int.Parse( rel.Substring(3, 1));
            int s2 =int.Parse( rel.Substring(4, 1));
            int s3 =int.Parse( rel.Substring(5, 1));
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
