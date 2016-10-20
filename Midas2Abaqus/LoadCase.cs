using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    //荷载工况
    public class LoadCase
    {
        public string LoadCaseName { set; get; }//荷载工况名称
        public string LoadCaseType { set; get; }//荷载工况类型
        public double LoadCaseFactor { set; get; }//组合值系数
        public LoadCase()
        {

        }
        public LoadCase(string loadName,string loadType)
        {
            LoadCaseName = loadName;
            LoadCaseType = loadType;
        }
        public LoadCase(string loadName, string loadType,double factor)
        {
            LoadCaseName = loadName;
            LoadCaseType = loadType;
            LoadCaseFactor = factor;
        }
    }
}
