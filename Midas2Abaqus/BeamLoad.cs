using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    //均布梁荷载
    public class BeamLoad
    {
        //******Midas中
        //*BEAMLOAD    ; Element Beam Loads
        //; ELEM_LIST, CMD, TYPE, DIR, bPROJ, [ECCEN], [VALUE], GROUP
        //; ELEM_LIST, CMD, TYPE, TYPE, DIR, VX, VY, VZ, bPROJ, [ECCEN], [VALUE], GROUP
        //; [VALUE]       : D1, P1, D2, P2, D3, P3, D4, P4
        //; [ECCEN]       : bECCEN, ECCDIR, I-END, J-END, bJ-END
        //; [ADDITIONAL]  : bADDITIONAL, ADDITIONAL_I-END, ADDITIONAL_J-END, bADDITIONAL_J-END
        //   9, BEAM   , UNILOAD, GZ, NO , NO, aDir[1], , , , 0, -20, 1, -20, 0, 0, 0, 0, , NO, 0, 0, NO
        //*****Abaqus中
        //*DLOAD
        //<单元编号或单元集合>,<荷载类型的代码>,<荷载值>
        //ELSET-2,PY,-30000
        public int ElementNO { set; get; }
        public string Type { set; get; }
        public string MidasDirection { set; get; }
        public string AbaqusDirection { set; get; }
        public double D1 { set; get; }
        public double D2 { set; get; }
        public double D3 { set; get; }
        public double D4 { set; get; }
        public double P1 { set; get; }
        public double P2 { set; get; }
        public double P3 { set; get; }
        public double P4 { set; get; }
        public string LoadCaseType { set; get; }//所属荷载工况类型

        public BeamLoad()
        {
            ElementNO = 0;
            MidasDirection = "GZ";
            AbaqusDirection = "PZ";
            P1 = 0.0;
        }

        public BeamLoad(int elemNo,string type,string midasDir,double d1,double d2,double d3,double d4,
            double p1,double p2,double p3,double p4)
        {
            ElementNO = elemNo;
            Type = type;
            MidasDirection = midasDir;
            AbaqusDirection = MidasDir2AbaqusDir(midasDir);
            D1 = d1;
            D2 = d2;
            D3 = d3;
            D4 = d4;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }
        public BeamLoad(int no, string type, string midasDir, double d1, double d2, double d3, double d4,
    double p1, double p2, double p3, double p4,string loadCase)
        {
            ElementNO = no;
            Type = type;
            MidasDirection = midasDir;
            AbaqusDirection = MidasDir2AbaqusDir(midasDir);
            D1 = d1;
            D2 = d2;
            D3 = d3;
            D4 = d4;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            LoadCaseType = loadCase;
        }

        public BeamLoad(int no,string midasDir,double p1)
        {
            ElementNO = no;
            MidasDirection = midasDir;
            AbaqusDirection = MidasDir2AbaqusDir(midasDir);
            P1 = p1;
        }
        public BeamLoad(int no, string midasDir, double p1,string loadCase)
        {
            ElementNO = no;
            MidasDirection = midasDir;
            AbaqusDirection = MidasDir2AbaqusDir(midasDir);
            P1 = p1;
            LoadCaseType = loadCase;
        }

        public string MidasDir2AbaqusDir(string midasDir)
        {
            switch (midasDir)
            {
                case "GX":
                    return "PX";
                case "GY":
                    return "PY";
                case "GZ":
                    return "PZ";
                default:
                    return "PZ";
            }
        }
    }
}
