using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas2Abaqus
{
    public class Section
    {
        public int NO { set; get; }
        public string Type { set; get; }
        public string Name { set; get; }
        public string Offset { set; get; }
        public double[] OffsetData { set; get; }
        //是否考虑剪切变形
        public bool IsShearDeform { set; get; }
        public string Shape { set; get; }
        public string AbaqusShape { set; get; }
        //截面数据类型，1表示直接选数据库的，2表示选完后修改参数的
        public int DataType { set; get; }
        public string [] Data { set; get; }
        public string AbaqusSectionData { set; get; }
        //截面面积，用于Abaqus中的Truss单元
        public double Area { set; get; }

        public Section()
        {

        }
        public Section(int no, string type, string name, string isSD, string shape, int dataType, string[] data)
        {
            NO = no;
            Type = type;
            Name = name;
            Offset = "CC";
            OffsetData = new double[] { 0, 0, 0, 0, 0, 0 };
            if(isSD=="YES")
            {
                IsShearDeform = true;
            }
            else
            {
                IsShearDeform = false;
            }
            Shape = shape;
            DataType = dataType;
            Data = data;
            AbaqusShape = SectionShapeMidas2Abaqus(Shape);
            AbaqusSectionData = SectionDataMidas2Abaqus(Data);

        }
        public Section(int no, string type, string name, bool isSD, string shape, int dataType, string[] data)
        {
            NO = no;
            Type = type;
            Name = name;
            Offset = "CC";
            OffsetData = new double[] { 0, 0, 0, 0, 0, 0 };
            IsShearDeform = isSD;
            Shape = shape;
            DataType = dataType;
            Data = data;
            AbaqusShape = SectionShapeMidas2Abaqus(Shape);
            AbaqusSectionData = SectionDataMidas2Abaqus(Data);

        }
        public Section(int no, string type, string name, string offset, double[] offsetData, string isSD, string shape, int dataType, string[] data)
        {
            NO = no;
            Type = type;
            Name = name;
            Offset = offset;
            OffsetData = offsetData;
            if(isSD=="YES")
            {
                IsShearDeform = true;
            }
            else
            {
                IsShearDeform = false;
            }
            Shape = shape;
            DataType = dataType;
            Data = data;
            AbaqusShape = SectionShapeMidas2Abaqus(Shape);
            AbaqusSectionData = SectionDataMidas2Abaqus(Data);

        }
        public Section(int no, string type, string name, string offset, double[] offsetData, bool isSD, string shape, int dataType, string[] data)
        {
            NO = no;
            Type = type;
            Name = name;
            Offset = offset;
            OffsetData = offsetData;
            IsShearDeform = isSD;
            Shape = shape;
            DataType = dataType;
            Data = data;
            AbaqusShape = SectionShapeMidas2Abaqus(Shape);
            AbaqusSectionData = SectionDataMidas2Abaqus(Data);

        }
        //根据材料类型和名称给出材性
        private void SetSectionPropertyByName(string type, string name)
        {
            if(type=="STEEL")
            {
                switch (name)
                {
                    case "Q235":

                        break;
                    case "Q345":

                        break;
                    default:

                        break;
                }
            }
            else if(type=="CONCRETE")
            {

            }
            else
            {

            }
        }
        //Midas的截面类型转为Abaqus的截面类型
        public string SectionShapeMidas2Abaqus(string shape)
        {
            string abaShpae;
            switch (shape)
            {
                case "L":
                    abaShpae = "L";
                    break;
                case "C":
                    abaShpae = "ARBITRARY";
                    break;
                case "H":
                    abaShpae = "I";
                    break;
                case "T":
                    abaShpae = "T";
                    break;
                case "B":
                    abaShpae = "BOX";
                    break;
                case "P":
                    abaShpae = "PIPE";
                    break;
                case "SB":
                    abaShpae = "RECT";
                    break;
                case "SR":
                    abaShpae = "CIRC";
                    break;
                default:
                    abaShpae = "I";
                    break;
            }
            return abaShpae;
        }
        //Midas截面数据到Abaqus截面数据
        public string SectionDataMidas2Abaqus(string[] dat)
        {
            string abaSec="";
            //数据类型1表示数据库直接选取
            if (DataType == 1)
            {
                switch (Shape)
                {
                    //工字钢
                    case "H":
                        {
                            //Midas数据GB-YB05, HM 294x200x8/12
                            string data1 = dat[1];
                            string[] data2 = data1.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
                            string[] data3 = data2[1].Split(new char[2] { 'x', '/' }, StringSplitOptions.RemoveEmptyEntries);
                            double HH = double.Parse(data3[0]);
                            abaSec += (HH / 2.0).ToString() + ",";//底部到连接中心距离
                            abaSec += data3[0] + ",";//高度
                            abaSec += data3[1] + ",";//下翼缘宽度
                            abaSec += data3[1] + ",";//上翼缘宽度
                            abaSec += data3[3] + ",";//下翼缘厚度
                            abaSec += data3[3] + ",";//上翼缘厚度
                            abaSec += data3[2];//腹板厚度
                            break;
                        }
                    //槽钢
                    case "C":
                        {
                            //Midas数据GB-YB05, C 32b
                            for (int i = 0; i < dat.Count(); i++)
                            {
                                abaSec += dat[i] + ",";
                            }
                            break;
                        }
                    //圆钢管
                    case "P":
                        {
                            //Midas数据
                            abaSec = "P";
                            break;
                        }
                    case "L":
                        abaSec = "L";
                        break;
                    case "T":
                        abaSec = "T";
                        break;
                    case "B":
                        {
                            //Midas数据B  , 1, GB-YB05, B 30x2
                            abaSec = "B";
                            break;
                        }
                    //实心矩形
                    case "SB":
                        abaSec = "RECT";
                        break;
                    //实心圆
                    case "SR":
                        abaSec = "SR";
                        break;
                    default:
                        abaSec = "";
                        break;
                }
            }

            //数据类型2表示数据库修改参数
            else if (DataType == 2)
            {
                switch (Shape)
                {
                    //工字钢
                    case "H":
                        {
                            //Midas数据H  , 2, 0.338, 0.351, 0.013, 0.013, 0, 0, 0.013, 0, 0, 0
                            //H,2,高度，上翼缘宽度，腹板厚度，上翼缘厚度，下翼缘宽度，下翼缘厚度，，，，
                            double[] db = new double[10];
                            for (int i = 0; i < 10; i++)
                            {
                                db[i] = double.Parse(dat[i]);
                            }
                            if(Math.Abs(db[4])<=1e-5)
                            {
                                db[4] = db[1];
                            }
                            if(Math.Abs(db[5])<=1e-5)
                            {
                                db[5] = db[3];
                            }
                            Area = (db[0] - db[3] - db[5]) * db[2] + db[1] * db[3] + db[4] * db[5];

                            abaSec += (db[0] / 2.0).ToString() + ",";//底部到连接中心距离
                            abaSec += (db[0]).ToString() + ",";//高度
                            abaSec += (db[4]).ToString() + ",";//下翼缘宽度
                            abaSec += (db[1]).ToString() + ",";//上翼缘宽度
                            abaSec += (db[5]).ToString() + ",";//下翼缘厚度
                            abaSec += (db[3]).ToString() + ",";//上翼缘厚度
                            abaSec += (db[2]).ToString();//腹板厚度
                            break;
                        }
                    //槽钢
                    case "C":
                        {
                            //Midas数据C  , 2, 0.32, 0.09, 0.01, 0.014, 0.09, 0.014, 0, 0, 0, 0
                            //C,2,高度，上翼缘宽度，腹板厚度，上翼缘厚度，下翼缘宽度，下翼缘厚度
                            double H = double.Parse(dat[0]);
                            double B1 = double.Parse(dat[1]);
                            double tw = double.Parse(dat[2]);
                            double tf1 = double.Parse(dat[3]);
                            double B2 = double.Parse(dat[4]);
                            double tf2 = double.Parse(dat[5]);
                            Area = (H - tf1 - tf2) * tw + B1 * tf1 + B2 * tf2;

                            double x0 = (B2 * tf2 * tf2 / 2 + (H - tf1 - tf2) * tf1 * (H + tf2 - tf1) / 2 + tf1 * B1 * (H - tf1 / 2)) / (B2 * tf2 + B1 * tf1 + (H - tf1 - tf2) * tw);
                            double y0 = (H * tw * tw / 2 + (B2 - tw) * tf2 * (B2 + tw) / 2 + (B1 - tw) * tf1 * (B1 + tw) / 2) / (B2 * tf2 + B1 * tf1 + (H - tf1 - tf2) * tw);
                            double x1 = B1 - x0;
                            double y1 = H - y0 - tf1 / 2;
                            double x2 = -x0 + tw / 2;
                            double y2 = H - y0 - tf1 / 2;
                            double x3 = -x0 + tw / 2;
                            double y3 = -y0 + tf2 / 2;
                            double x4 = B2 - x0;
                            double y4 = -y0 + tf2 / 2;

                            abaSec += "3," + x1 + "," + y1 + "," + x2 + "," + y2 + "," + tf1 + "\r\n";
                            abaSec += x3.ToString() + "," + y3.ToString() + "," + tw.ToString() + "\r\n";
                            abaSec += x4.ToString() + "," + y4.ToString() + "," + tf2.ToString();
                            break;
                        }
                    //圆钢管
                    case "P":
                        {
                            //Midas数据P（圆钢管）  , 2（用户数据）, 0.2（直径）, 0.008（厚度）, 0, 0, 0, 0, 0, 0, 0, 0
                            //Abaqus数据section=PIPE
                            //半径600., 12.
                            double dia = double.Parse(dat[0]);
                            double riad = dia / 2.0;
                            double thick = double.Parse(dat[1]);
                            Area = Math.PI * thick * (dia - thick);
                            abaSec = riad.ToString()+","+thick.ToString();
                            break;
                        }
                    case "L":
                        abaSec = "L";
                        break;
                    case "T":
                        abaSec = "T";
                        break;
                    //箱型
                    case "B":
                        {
                            //Midas数据B  , 2, 0.06, 0.04, 0.002, 0.003, 0, 0, 0, 0, 0, 0
                            //高,宽,竖向板厚,横向板厚
                            //Abaqus数据section=BOX
                            //300., 600., 10., 10., 10., 10.
                            //宽，高，右侧厚度，上侧厚度，左侧厚度，下侧厚度
                            double wid = double.Parse(dat[1]);
                            double hig = double.Parse(dat[0]);
                            double t1 = double.Parse(dat[2]);
                            double t2 = double.Parse(dat[3]);
                            abaSec = wid.ToString() + "," + hig.ToString() + "," + t1 + "," + t2 + "," + t1 + "," + t2;
                            break;
                        }
                    //实心矩形
                    case "SB":
                        {
                            //Midas数据SB , 2, 0.6, 0.3, 0, 0, 0, 0, 0, 0, 0, 0
                            //Abaqus数据section=RECT
                            //300., 600.
                            double HH = double.Parse(dat[0]);
                            double BB = double.Parse(dat[1]);
                            Area = HH * BB;
                            abaSec = BB.ToString() + "," + HH.ToString();
                            break;
                        }
                    //实心圆
                    case "SR":
                        {
                            //Midas数据SR（实心圆） , 2（用户数据）, 0.8（直径）, 0, 0, 0, 0, 0, 0, 0, 0, 0
                            //Abaqus数据section=CIRC
                            //500.
                            double banjing = double.Parse(dat[0]) / 2.0;
                            Area = Math.PI * banjing * banjing;
                            abaSec = banjing.ToString() ;
                            break;
                        }
                    default:
                        abaSec = "";
                        break;
                }
            }
            else
            {
                abaSec = "";
            }
            return abaSec;
        }

    }
}
