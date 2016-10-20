using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class Release
    {
        public int ElementNO { set; get; }
        public string Release1 { set; get; }
        public string Release2 { set; get; }
        public string AbaqusRelease1 { set; get; }
        public string AbaqusRelease2 { set; get; }
        public Release()
        {
            ElementNO = 0;
            Release1 = "000000";
            Release2 = "000000";
            AbaqusRelease1 = "";
            AbaqusRelease2 = "";
        }
        public Release(int no, string r1, string r2)
        {
            ElementNO = no;
            Release1 = r1.TrimStart(' ');
            Release2 = r2.TrimEnd(' ');
            AbaqusRelease1 = Release2Abaqus(Release1);
            AbaqusRelease2 = Release2Abaqus(Release2);
        }
        public string Release2Abaqus(string rel)
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
