using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class PressureLoad
    {
        //*PRESSURE    ; Pressure Loads
        //; ELEM_LIST, CMD, ETYP, LTYP, DIR, VX, VY, VZ, bPROJ, PU, P1, P2, P3, P4, GROUP  ; ETYP=PLATE, LTYP=FACE
        //; ELEM_LIST, CMD, ETYP, LTYP, iEDGE, DIR, VX, VY, VZ, PU, P1, P2, GROUP          ; ETYP=PLATE, LTYP=EDGE
        //; ELEM_LIST, CMD, ETYP, iEDGE, DIR, VX, VY, VZ, PU, P1, P2, GROUP                ; ETYP=PLANE
        //; ELEM_LIST, CMD, ETYP, iFACE, DIR, VX, VY, VZ, bPROJ, PU, P1, P2, P3, P4, GROUP ; ETYP=SOLID
        //; [PLATE] : plate, plane stress, wall, [PLANE] : axisymmetric, plane strain
        //   836, PRES , PLATE, FACE, GY, 0, 0, 0, NO, -0.37, 0, 0, 0, 0, 
        public int ElementNO { set; get; }
        public string CMD { set; get; }
        public string ElementType { set; get; }
        public string LoadType { set; get; }
        public string Direction { set; get; }
        public string AbaqusDirection { set; get; }
        public double VX { set; get; }
        public double VY { set; get; }
        public double VZ { set; get; }
        public bool isProjection { set; get; }//指定是否将压力荷载进行投影
        public double PU { set; get; }
        public double P1 { set; get; }
        public double P2 { set; get; }
        public double P3 { set; get; }
        public double P4 { set; get; }
        public string LoadCaseType { set; get; }//所属荷载工况类型

        public PressureLoad()
        {

        }

        public PressureLoad(int elemNo,string cmd,string eType,string lType,string dir,double pu)
        {
            ElementNO = elemNo;
            CMD = cmd;
            ElementType = eType;
            LoadType = lType;
            Direction = dir;
            PU = pu;
            AbaqusDirection = MidasDir2AbaqusDir(Direction);
        }
        public PressureLoad(int no, string cmd, string eType, string lType, string dir, double pu,string loadCase)
        {
            ElementNO = no;
            CMD = cmd;
            ElementType = eType;
            LoadType = lType;
            Direction = dir;
            PU = pu;
            LoadCaseType = loadCase;
            AbaqusDirection = MidasDir2AbaqusDir(Direction);
        }

        public string MidasDir2AbaqusDir(string midasDir)
        {
            switch (midasDir)
            {
                case "GX":
                    return "1";
                case "GY":
                    return "2";
                case "GZ":
                    return "3";
                default:
                    return "3";
            }
        }

    }
}
