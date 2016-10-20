using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class Boundary
    {
        public int NodeNO { set; get; }
        public string MidasConstraint { set; get; }
        public int[] AbaqusConstraint { set; get; }
        public Boundary()
        {
            NodeNO = 0;
            MidasConstraint = "000000";
            AbaqusConstraint=new int[] {0,0,0,0,0,0};
        }
        public Boundary(int no,string mCon)
        {
            NodeNO = no;
            mCon = mCon.TrimStart(' ');
            mCon = mCon.TrimEnd(' ');
            MidasConstraint = mCon;
            AbaqusConstraint = new int[] { 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < 6; i++)
            {
                if (mCon.Substring(i, 1) == "1")
                {
                    AbaqusConstraint[i] = 1;
                }
                else
                {
                    AbaqusConstraint[i] = 0;
                }
            }


        }
        public List<string> GetAbaqusBoundary()
        {
            List<string> listStr = new List<string>();
            listStr.Clear();
            for(int i=0;i<6;i++)
            {
                if(AbaqusConstraint[i]==1)
                {
                    listStr.Add(NodeNO.ToString() + "," + (i + 1).ToString() + "," + (i + 1).ToString());
                }
            }
            return listStr;
        }

    }
}
