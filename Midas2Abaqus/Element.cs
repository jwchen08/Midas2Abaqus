using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    //单元类
    public class Element
    {
        public int NO { set; get; }
        public string Type { set; get; }
        public int SubType { set; get; }//类型的二次分类
        public string AbaqusType { set; get; }
        public int Material { set; get; }
        public string MaterialString { set; get; }
        public int Section { set; get; }
        public string AbaqusSection { set; get; }
        public int N1 { set; get; }
        public int N2 { set; get; }
        public int N3 { set; get; }
        public int N4 { set; get; }
        public double Angle { set; get; }
        
        public Element()
        {

        }

        //梁单元、桁架单元构造函数
        public Element(int no,string type,int mat,int sec,int n1,int n2,double ang)
        {
            NO = no;
            Type = type;
            Material = mat;
            Section = sec;
            N1 = n1;
            N2 = n2;
            Angle = ang;
            AbaqusType = ElementTypeMidas2Abaqus(Type);
        }

        //板单元构造函数
        public Element(int no, string type, int mat, int sec, int n1, int n2,int n3,int n4,int subType)
        {
            NO = no;
            Type = type;
            Material = mat;
            Section = sec;
            N1 = n1;
            N2 = n2;
            N3 = n3;
            N4 = n4;
            SubType = subType;
            AbaqusType = ElementTypeMidas2Abaqus(Type);
        }
        public string ElementTypeMidas2Abaqus(string type)
        {
            string abaType;
            switch(type)
            {
                case "BEAM":
                    abaType = "Beam Section";
                    break;
                case "TRUSS":
                    abaType = "Solid Section";
                    break;
                case "PLATE":
                    abaType = "PLATE";
                    break;
                case "TENSTR":
                    abaType = "TENSTR";
                    break;
                default :
                    abaType = "Beam Section";
                    break;
            }
            return abaType;
        }
        public string ElementSectionMidas2Abaqus(string section)
        {
            string abaSection;
            switch (section)
            {
                case "L":
                    abaSection = "L";
                    break;
                case "C":
                    abaSection = "C";
                    break;
                case "H":
                    abaSection = "I";
                    break;
                case "T":
                    abaSection = "T";
                    break;
                case "B":
                    abaSection = "B";
                    break;
                case "P":
                    abaSection = "P";
                    break;
                case "SB":
                    abaSection = "RECT";
                    break;
                case "SR":
                    abaSection = "SR";
                    break;
                default:
                    abaSection = "I";
                    break;
            }
            return abaSection;
        }

        public Element CloneElement()
        {
            return new Element(NO, Type, Material, Section, N1, N2,Angle);
        }
      

    }
}
