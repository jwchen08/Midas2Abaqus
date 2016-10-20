using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class ElementSet
    {
        public List<int> SetNOList { set; get; }
        public string SetType { set; get; }
        public string SetName { set; get; }
        public string SetMaterial { set; get; }
        public string SetSection { set; get; }
        public string SetSectionParameter { set; get; }

        public double[] SetSectionN1 { set; get; }
        public ElementSet()
        {
            SetNOList = new List<int>();
            SetSectionN1 = new double[3] { 0.0, 0.0, 0.0 };
        }

        public ElementSet(int No,string setType,string setName,string setMaterial,string setSection,
            string setSectionPara, double[] setN1)
        {
            SetNOList = new List<int>();
            SetNOList.Add(No);
            SetType = setType;
            SetName = setName;
            SetMaterial = setMaterial;
            SetSection = setSection;
            SetSectionParameter = setSectionPara;
            SetSectionN1 = setN1;
        }
    }
}
