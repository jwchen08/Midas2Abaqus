using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class Thickness
    {
        public int NO { set; get; }
        public string Type { set; get; }
        //面内面外厚度是否相同
        public bool IsSame { set; get; }
        public double ThicknessIn { set; get; }
        public double ThicknessOut { set; get; }
        public bool IsOffset { set; get; }
        public int OffType { set; get; }
        public double OffValue { set; get; }

        public Thickness()
        {

        }
        public Thickness(int no, string type, string same, double Tin, double Tout)
        {
            NO = no;
            Type = type;
            if(same=="YES")
            {
                IsSame = true;
            }
            else
            {
                IsSame = false;
            }
            ThicknessIn = Tin;
            ThicknessOut = Tout;
            if(IsSame)
            {
                ThicknessOut = ThicknessIn;
            }
        }
        public Thickness(int no,string type,bool same,double Tin,double Tout)
        {
            NO = no;
            Type = type;
            IsSame = same;
            ThicknessIn = Tin;
            ThicknessOut = Tout;
            if (IsSame)
            {
                ThicknessOut = ThicknessIn;
            }
        }
        public Thickness(int no, string type, bool same, double Tin, double Tout, string offset, int offType, double offValue)
        {
            NO = no;
            Type = type;
            IsSame = same;
            ThicknessIn = Tin;
            ThicknessOut = Tout;
            if(offset=="YES")
            {
                IsOffset = true;
            }
            else
            {
                IsOffset = false;
            }
            OffType = offType;
            OffValue = offValue;
            if (IsSame)
            {
                ThicknessOut = ThicknessIn;
            }
        }
        public Thickness(int no, string type, bool same, double Tin, double Tout,bool offset,int offType,double offValue)
        {
            NO = no;
            Type = type;
            IsSame = same;
            ThicknessIn = Tin;
            ThicknessOut = Tout;
            IsOffset = offset;
            OffType = offType;
            OffValue = offValue;
            if (IsSame)
            {
                ThicknessOut = ThicknessIn;
            }
        }

    }
}
