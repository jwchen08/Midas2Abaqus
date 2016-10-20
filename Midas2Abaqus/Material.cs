using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class Material
    {
        public int NO { set; get; }
        public string Type { set; get; }
        public string Name { set; get; }
        public double Density { set; get; }
        public double YoungsModulu { set; get; }
        public double PoissonRatio { set; get; }
        public double[] YieldStress { set; get; }
        public double[] PlasticStrain { set; get; }
        public Material()
        {

        }
        public Material(int no,string type,string name)
        {
            NO = no;
            Type = type;
            Name = name;
            SetMaterialPropertyByName(type, name);
        }
        public Material(int no,string type,string name,double den,double young,double poisson)
        {
            NO = no;
            Type = type;
            Name = name;
            Density = den;
            YoungsModulu = young;
            PoissonRatio = poisson;
        }
        public Material(int no,string type,string name,double den,double young,double poisson,double[] yield,double[] plastic)
        {
            NO = no;
            Type = type;
            Name = name;
            Density = den;
            YoungsModulu = young;
            PoissonRatio = poisson;
            YieldStress = yield;
            PlasticStrain = plastic;
        }
        //根据材料类型和名称给出材性
        private void SetMaterialPropertyByName(string type,string name)
        {
            if(type=="STEEL")
            {
                switch (name)
                {
                    case "Q235":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    case "Q345":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    case "Q390":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    case "Q420":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    case "Q460":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    case "Q690":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    case "Q960":
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                    default:
                        Density = 7850;
                        YoungsModulu = 206000000000;
                        PoissonRatio = 0.3;
                        break;
                }
            }
            else if(type=="CONC")
            {
                switch (name)
                {
                    case "C15":
                        Density = 2500;
                        YoungsModulu = 22000000000;
                        PoissonRatio = 0.2;
                        break;
                    case "C20":
                        Density = 2500;
                        YoungsModulu = 25500000000;
                        PoissonRatio = 0.2;

                        break;
                    case "C25":
                        Density = 2500;
                        YoungsModulu = 28000000000;
                        PoissonRatio = 0.2;
                        break;
                    case "C30":
                        Density = 2500;
                        YoungsModulu = 30000000000;
                        PoissonRatio = 0.2;
                        break;
                    case "C35":
                        Density = 2500;
                        YoungsModulu = 31500000000;
                        PoissonRatio = 0.2;
                        break;
                    case "C40":
                        Density = 2500;
                        YoungsModulu = 32500000000;
                        PoissonRatio = 0.2;
                        break;
                    case "C45":
                        Density = 2500;
                        YoungsModulu = 33500000000;
                        PoissonRatio = 0.2;
                        break;
                    case "C50":
                        Density = 2500;
                        YoungsModulu = 34500000000;
                        PoissonRatio = 0.2;
                        break;
                    default:
                        Density = 2500;
                        YoungsModulu = 30000000000;
                        PoissonRatio = 0.2;
                        break;
                }
            }
            else if(type=="SRC")
            {

            }
            else if(type=="USER")
            {
                Density = 7850;
                YoungsModulu = 206000000000;
                PoissonRatio = 0.3;
            }
            else
            {
                Density = 7850;
                YoungsModulu = 206000000000;
                PoissonRatio = 0.3;
            }

        }

    }
}
