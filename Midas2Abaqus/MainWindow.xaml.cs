using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Midas2Abaqus
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //Midas的mgt文件字符串列表
        List<string> MidasStringList = new List<string>();
        //Abaqus的inp文件字符串列表
        List<string> AbaqusStringList = new List<string>();

        List<Node> NodeList = new List<Node>();
        List<Element> ElementList = new List<Element>();
        List<Element> AbaqusElementList = new List<Element>();//划分网格后的单元
        List<Material> MaterialList = new List<Material>();
        List<Section> SectionList = new List<Section>();
        List<Thickness> ThicknessList = new List<Thickness>();
        List<ElementSet> ElementSetList = new List<ElementSet>();
        List<ConLoad> ConLoadList = new List<ConLoad>();
        List<BeamLoad> BeamLoadList = new List<BeamLoad>();
        List<PressureLoad> PressureLoadList = new List<PressureLoad>();
        List<LoadCase> LoadCaseList = new List<LoadCase>();
        List<Boundary> BoundaryList = new List<Boundary>();
        List<Release> ReleaseList = new List<Release>();

        string filePath = "";
        const double min = 1e-6;
        const double angleMin = 1.0;
        //单位系统
        string Force = "";
        string Length = "";
        string Heat = "";
        string Temper = "";
        //划分网格数据
        const int StartNodeIndex = 100000;
        const int StartElementIndex = 100000;
        int MeshDensity = 2;
        //public static double unitTransfer=1.0;
        public MainWindow()
        {
            InitializeComponent();
            MeshDensity = 2;
        }

        //打开mgt文件
        private void OpenMgtButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Filter = "Midas Document(*.mgt)|*.mgt";

            if (openFileDialog1.ShowDialog() ==System.Windows.Forms.DialogResult.OK)
            {
                filePath = System.IO.Path.GetDirectoryName(openFileDialog1.FileName)+@"\";
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.FileName,Encoding.GetEncoding("GBK")))
                    {
                        SourceFileTextBlock.Text = openFileDialog1.FileName;
                        OutputFileTextBlock.Text = filePath+"M2A-Static.inp";

                        string temp = sr.ReadLine();
                        while(temp!=null)
                        {
                            MidasStringList.Add(temp);
                            temp = sr.ReadLine();
                        }
                    }
                    //获取Midas版本和单位信息
                    if (MidasStringList.Count > 0)
                    {
                        //提取Midas版本
                        bool isVersion = false;
                        List<string> versionStringList = new List<string>();
                        for (int i = 0; i < MidasStringList.Count; i++)
                        {
                            string s = MidasStringList[i];
                            if (isVersion && !string.IsNullOrEmpty(s))
                            {
                                versionStringList.Add(s);
                            }
                            if (s.Contains("*VERSION"))
                            {
                                isVersion = true;
                            }
                            if (isVersion && MidasStringList[i + 1].Contains("*"))
                            {
                                break;
                            }
                        }
                        string versionString = "Midas版本:\n";
                        foreach (string versionStr in versionStringList)
                        {
                            versionString += versionStr + "\n";
                        }
                        VersionTextBlock.Text = versionString;

                        //提取Midas单位
                        bool isUnit = false;
                        for (int i = 0; i < MidasStringList.Count; i++)
                        {
                            string s = MidasStringList[i];
                            if (isUnit && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                            {
                                string[] us = s.Split(',');
                                us[0] = us[0].TrimStart();
                                us[0] = us[0].TrimEnd();
                                Force = us[0];
                                us[1] = us[1].TrimStart();
                                us[1] = us[1].TrimEnd();
                                Length = us[1];
                                us[2] = us[2].TrimStart();
                                us[2] = us[2].TrimEnd();
                                Heat = us[2];
                                us[3] = us[3].TrimStart();
                                us[3] = us[3].TrimEnd();
                                Temper = us[3];
                            }
                            if (s.Contains("*UNIT"))
                            {
                                isUnit = true;
                            }
                            if (isUnit && MidasStringList[i + 1].Contains("*"))
                            {
                                break;
                            }
                        }
                        string unitString = "Midas单位系统:\n";
                        unitString += "  力  :  " + Force + "\n";
                        unitString += "  长度:  " + Length + "\n";
                        unitString += "  热量:  " + Heat + "\n";
                        unitString += "  温度:  " + Temper + "\n";
                        unitString += "\nAbaqus单位系统:\n   N, m, kg, s, J\n";
                        UnitTextBlock.Text = unitString;

                        //提取Midas荷载工况信息
                        bool isLoadCase = false;
                        for (int i = 0; i < MidasStringList.Count; i++)
                        {
                            string s = MidasStringList[i];
                            if (isLoadCase && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                            {
                                string[] ss = s.Split(',');
                                for (int j = 0; j < ss.Count(); j++)
                                {
                                    ss[j] = ss[j].Trim();
                                }
                                LoadCaseList.Add(new LoadCase(ss[0], ss[1]));
                            }
                            if (s.Contains("*STLDCASE"))
                            {
                                isLoadCase = true;
                            }
                            if (isLoadCase && MidasStringList[i + 1].Contains("*"))
                            {
                                break;
                            }
                        }
                        string loadcaseString = "Midas包含的荷载工况:\n";
                        for (int iCase = 0; iCase < LoadCaseList.Count;iCase++ )
                        {
                            loadcaseString += "荷载工况" + (iCase + 1).ToString() + ": " + LoadCaseList[iCase].LoadCaseName
                                + ",工况类型: " + LoadCaseList[iCase].LoadCaseType + "\n";
                        }
                        LoadCaseBlock.Text = loadcaseString;

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取文件失败！");
                }

            }
        }

        //开始格式转换
        private void StartTransferButton_Click(object sender, RoutedEventArgs e)
        {
            if (MidasStringList.Count > 0)
            {
                //提取Midas节点数据
                bool isNode = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if (isNode && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                    {
                        string[] ns = s.Split(',');
                        NodeList.Add(new Node(int.Parse(ns[0]), double.Parse(ns[1]), double.Parse(ns[2]), double.Parse(ns[3])));
                    }
                    if (s.Contains("*NODE"))
                    {
                        isNode = true;
                    }
                    if (isNode && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas单元数据
                bool isElement = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if (isElement && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                    {
                        string[] es = s.Split(',');
                        for (int j = 0; j < es.Count(); j++)
                        {
                            es[j] = es[j].Trim();
                        }
                        //*ELEMENT    ; Elements
                        //; iEL, TYPE, iMAT, iPRO, iN1, iN2, ANGLE, iSUB, EXVAL, iOPT(EXVAL2) ; Frame  Element
                        //; iEL, TYPE, iMAT, iPRO, iN1, iN2, ANGLE, iSUB, EXVAL, EXVAL2, bLMT ; Comp/Tens Truss
                        //; iEL, TYPE, iMAT, iPRO, iN1, iN2, iN3, iN4, iSUB, iWID             ; Planar Element
                        //; iEL, TYPE, iMAT, iPRO, iN1, iN2, iN3, iN4, iN5, iN6, iN7, iN8     ; Solid  Element
                        //; iEL, TYPE, iMAT, iPRO, iN1, iN2, REF, RPX, RPY, RPZ, iSUB, EXVAL  ; Frame(Ref. Point)
                        if(es[1]=="BEAM"||es[1]=="TRUSS")
                        {
                            ElementList.Add(new Element(int.Parse(es[0]), es[1], int.Parse(es[2]), int.Parse(es[3]), int.Parse(es[4]), int.Parse(es[5]), double.Parse(es[6])));
                        }
                        else if(es[1]=="TENSTR"||es[1]=="COMPTR")
                        {
                            ElementList.Add(new Element(int.Parse(es[0]), es[1], int.Parse(es[2]), int.Parse(es[3]), int.Parse(es[4]), int.Parse(es[5]), double.Parse(es[6])));
                        }
                        else if(es[1]=="PLATE")
                        {
                            ElementList.Add(new Element(int.Parse(es[0]), es[1], int.Parse(es[2]), int.Parse(es[3]), int.Parse(es[4]), int.Parse(es[5]), int.Parse(es[6]), int.Parse(es[7]), int.Parse(es[8])));
                        }
                        else
                        {

                        }
                    }
                    if (s.Contains("*ELEMENT"))
                    {
                        isElement = true;
                    }
                    if (isElement && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas材料数据
                bool isMaterial = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if (isMaterial && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                    {
                        string[] ms = s.Split(',');
                        ms[1] = ms[1].Trim();
                        ms[2] = ms[2].Trim();
                        MaterialList.Add(new Material(int.Parse(ms[0]),ms[1], ms[2]));
                    }
                    if (s.Contains("*MATERIAL"))
                    {
                        isMaterial = true;
                    }
                    if (isMaterial && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas截面数据
                bool isSection = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if (isSection && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                    {
                        string[] ss = s.Split(',');
                        for (int j = 0; j < ss.Count(); j++)
                        {
                            ss[j] = ss[j].Trim();
                        }
                        string[] strData=new string[ss.Count()-13];
                        for (int k = 0; k < strData.Count(); k++)
                        {
                            strData[k] = ss[k + 13];
                        }
                        SectionList.Add(new Section(int.Parse(ss[0]), ss[1], ss[2], ss[10], ss[11], int.Parse(ss[12]), strData));
                    }
                    if (s.Contains("*SECTION"))
                    {
                        isSection = true;
                    }
                    if (isSection && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas厚度数据
                bool isThickness = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if (isThickness && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                    {
                        string[] ss = s.Split(',');
                        for (int j = 0; j < ss.Count(); j++)
                        {
                            ss[j] = ss[j].Trim();
                        }
                        ThicknessList.Add(new Thickness(int.Parse(ss[0]), ss[1], ss[2], double.Parse(ss[3]), double.Parse(ss[4])));
                    }
                    if (s.Contains("*THICKNESS"))
                    {
                        isThickness = true;
                    }
                    if (isThickness && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas荷载工况信息
                bool isLoadCase = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if (isLoadCase && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                    {
                        string[] ss = s.Split(',');
                        for (int j = 0; j < ss.Count(); j++)
                        {
                            ss[j] = ss[j].Trim();
                        }
                        LoadCaseList.Add(new LoadCase(ss[0], ss[1]));
                    }
                    if (s.Contains("*STLDCASE"))
                    {
                        isLoadCase = true;
                    }
                    if (isLoadCase && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas节点约束信息
                bool isBoundary = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s = MidasStringList[i];
                    if(isBoundary&&!string.IsNullOrEmpty(s)&&!s.Contains(";"))
                    {
                        string[] bs = s.Split(',');
                        string[] nodeStr = bs[0].Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
                        bs[1] = bs[1].TrimStart(' ');
                        bs[1] = bs[1].TrimEnd(' ');
                        for (int j = 0; j < nodeStr.Count(); j++)
                        {
                            if(nodeStr[j].Contains("to")&&nodeStr[j].Contains("by"))
                            {
                                string[] ns = nodeStr[j].Split(new string[] { "to", "by" }, StringSplitOptions.RemoveEmptyEntries);
                                int startNO = int.Parse(ns[0]);
                                int endNO = int.Parse(ns[1]);
                                int step = int.Parse(ns[2]);
                                for (int k = startNO; k <= endNO; k+=step)
                                {
                                    BoundaryList.Add(new Boundary(k, bs[1]));
                                }
                            }
                            else if(nodeStr[j].Contains("to"))
                            {
                                string[] ns=nodeStr[j].Split(new char[2]{'t','o'},StringSplitOptions.RemoveEmptyEntries);
                                int startNO=int.Parse(ns[0]);
                                int endNO=int.Parse(ns[1]);
                                for(int k=startNO;k<=endNO;k++)
                                {
                                    BoundaryList.Add(new Boundary(k,bs[1]));
                                }
                            }
                            else
                            {
                                BoundaryList.Add(new Boundary(int.Parse(nodeStr[j]), bs[1]));
                            }
                        }

                    }
                    if(s.Contains("*CONSTRAINT"))
                    {
                        isBoundary = true;
                    }
                    if(isBoundary&&MidasStringList[i+1].Contains("*"))
                    {
                        break;
                    }
                }

                //提取Midas释放梁端约束
                bool isRelease = false;
                for (int i = 0; i < MidasStringList.Count; i++)
                {
                    string s1 = MidasStringList[i];
                    if (isRelease && !string.IsNullOrEmpty(s1) && !s1.Contains(";"))
                    {
                        if (s1.Contains("YES") || s1.Contains("NO"))
                        {
                            string s2 = MidasStringList[i + 1];
                            string[] rs1 = s1.Split(',');
                            rs1[0] = rs1[0].TrimStart(' ');
                            rs1[0] = rs1[0].TrimEnd(' ');
                            rs1[2] = rs1[2].TrimStart(' ');
                            rs1[2] = rs1[2].TrimEnd(' ');
                            string[] rs2 = s2.Split(',');
                            rs2[0] = rs2[0].TrimStart(' ');
                            rs2[0] = rs2[0].TrimEnd(' ');
                            ReleaseList.Add(new Release(int.Parse(rs1[0]), rs1[2], rs2[0]));
                        }
                    }
                    if (s1.Contains("*FRAME-RLS"))
                    {
                        isRelease = true;
                    }
                    if (isRelease && MidasStringList[i + 1].Contains("*"))
                    {
                        break;
                    }
                }

                //按不同荷载工况提取荷载值
                for(int icase=0;icase<LoadCaseList.Count;icase++)
                {
                    for(int ii=0;ii<MidasStringList.Count;ii++)
                    {
                        string temp = MidasStringList[ii];
                        if(temp.Contains("USE-STLD")&&temp.Contains(LoadCaseList[icase].LoadCaseName))
                        {
                            //提取Midas节点集中荷载数据
                            bool isConLoad = false;
                            for (int i = ii+1; i < MidasStringList.Count; i++)
                            {
                                string s = MidasStringList[i];
                                if (isConLoad && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                                {
                                    string[] cs = s.Split(',');
                                    ConLoadList.Add(new ConLoad(int.Parse(cs[0]), double.Parse(cs[1]), double.Parse(cs[2]), double.Parse(cs[3]),
                                        double.Parse(cs[4]), double.Parse(cs[5]), double.Parse(cs[6]),LoadCaseList[icase].LoadCaseType));
                                }
                                if (s.Contains("*CONLOAD"))
                                {
                                    isConLoad = true;
                                }
                                if (isConLoad && MidasStringList[i + 1].Contains("*"))
                                {
                                    break;
                                }
                            }

                            //提取Midas梁单元均布荷载数据
                            bool isBeamLoad = false;
                            for (int i = ii+1; i < MidasStringList.Count; i++)
                            {
                                string s = MidasStringList[i];
                                if (isBeamLoad && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                                {
                                    string[] ds = s.Split(',');
                                    BeamLoadList.Add(new BeamLoad(int.Parse(ds[0]), ds[3], double.Parse(ds[11]), LoadCaseList[icase].LoadCaseType));
                                }
                                if (s.Contains("*BEAMLOAD"))
                                {
                                    isBeamLoad = true;
                                }
                                if (isBeamLoad && MidasStringList[i + 1].Contains("*"))
                                {
                                    break;
                                }
                            }

                            //提取Midas板单元压力荷载
                            bool isPressure = false;
                            for (int i = ii+1; i < MidasStringList.Count; i++)
                            {
                                string s = MidasStringList[i];
                                if (isPressure && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                                {
                                    string[] ps = s.Split(',');
                                    for (int j = 0; j < ps.Count(); j++)
                                    {
                                        ps[j] = ps[j].Trim();
                                    }
                                    PressureLoadList.Add(new PressureLoad(int.Parse(ps[0]), ps[1], ps[2], ps[3], ps[4], double.Parse(ps[9]), LoadCaseList[icase].LoadCaseType));
                                }
                                if (s.Contains("*PRESSURE"))
                                {
                                    isPressure = true;
                                }
                                if (isPressure && MidasStringList[i + 1].Contains("*"))
                                {
                                    break;
                                }
                            }

                        }
                    }
                }
                    

            }

            //写入Abaqus输入文件
            WriteAbaqusInpFile();

        }


        //写入Abaqus信息数据
        public void WriteAbaqusInfoData()
        {
            AbaqusStringList.Add("*HEADING");
            AbaqusStringList.Add("**");
        }

        //细分网格，增加节点和单元
        public void AddMeshData()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                Element elem = ElementList[i];
                Node node1 = NodeList.Find(nd => nd.NO == elem.N1);
                Node node2 = NodeList.Find(nd => nd.NO == elem.N2);
                if (elem.Type == "BEAM")
                {
                    //增加节点数据
                    NodeList.Add(new Node(
                        StartNodeIndex + elem.NO,
                        node1.X + (node2.X - node1.X) /2,
                        node1.Y + (node2.Y - node1.Y) /2,
                        node1.Z + (node2.Z - node1.Z) /2
                        ));

                    //for (int j = 1; j < MeshDensity; j++)
                    //{
                    //    NodeList.Add(new Node(
                    //        StartNodeIndex + i * (MeshDensity - 1) + j,
                    //        node1.X + (node2.X - node1.X) * j / MeshDensity,
                    //        node1.Y + (node2.Y - node1.Y) * j / MeshDensity,
                    //        node1.Z + (node2.Z - node1.Z) * j / MeshDensity
                    //        ));
                    //}

                    //替换单元数据
                    //for (int k = 1; k <= MeshDensity; k++)
                    //{
                    //    Element newElem = elem.CloneElement();//如果对类的对象直接用等号，只是引用
                    //    newElem.NO = StartElementIndex + i * MeshDensity + k;
                    //    if (k == 1)
                    //    {
                    //        newElem.N1 = elem.N1;
                    //        newElem.N2 = StartNodeIndex + i * (MeshDensity - 1) + k;
                    //    }
                    //    else if (k == MeshDensity)
                    //    {
                    //        newElem.N1 = StartNodeIndex + i * (MeshDensity - 1) + k - 1;
                    //        newElem.N2 = elem.N2;
                    //    }
                    //    else
                    //    {
                    //        newElem.N1 = StartNodeIndex + i * (MeshDensity - 1) + k - 1;
                    //        newElem.N2 = StartNodeIndex + i * (MeshDensity - 1) + k;
                    //    }
                    //    AbaqusElementList.Add(newElem);
                    //}
                }
            }
        }

        //写入Abaqus节点数据
        public void WriteAbaqusNodeData(List<Node> nList)
        {
            AbaqusStringList.Add("*NODE");
            int count = nList.Count;
            for(int i=0;i<count;i++)
            {
                string s = nList[i].NO.ToString() + "," + nList[i].X + "," + nList[i].Y + "," + nList[i].Z;
                AbaqusStringList.Add(s);
            }
            AbaqusStringList.Add("**");
        }

        //写入Abaqus单元数据
        public void WriteAbaqusElementData(List<Element> eList)
        {
            //Beam单元B31，Truss单元T3D2，
            int count = eList.Count;
            for (int i = 0; i < count; i++)
            {
                if (eList[i].Type == "BEAM")
                {
                    //B31单元
                    //AbaqusStringList.Add("*ELEMENT,TYPE=B31");
                    //string s = eList[i].NO.ToString() + "," + eList[i].N1 + "," + eList[i].N2;
                    int middleNO = StartNodeIndex + eList[i].NO;
                    //B32单元
                    AbaqusStringList.Add("*ELEMENT,TYPE=B32");
                    string s = eList[i].NO.ToString() + "," + eList[i].N1 + "," + middleNO +","+ eList[i].N2;
                    AbaqusStringList.Add(s);
                }
                else if (eList[i].Type == "TRUSS")
                {
                    AbaqusStringList.Add("*ELEMENT,TYPE=T3D2");
                    string s = eList[i].NO.ToString() + "," + eList[i].N1 + "," + eList[i].N2;
                    AbaqusStringList.Add(s);
                }
                //只受拉单元
                else if (eList[i].Type == "TENSTR")
                {
                    AbaqusStringList.Add("*ELEMENT,TYPE=T3D2");
                    string s = eList[i].NO.ToString() + "," + eList[i].N1 + "," + eList[i].N2;
                    AbaqusStringList.Add(s);
                }
                //只受压单元
                else if (eList[i].Type == "COMPTR")
                {
                    AbaqusStringList.Add("*ELEMENT,TYPE=T3D2");
                    string s = eList[i].NO.ToString() + "," + eList[i].N1 + "," + eList[i].N2;
                    AbaqusStringList.Add(s);
                }
                else if (eList[i].Type == "PLATE")
                {

                }
                else
                {

                }

            }
            AbaqusStringList.Add("**");
        }

        //写入Abaqus节点集合
        public void WriteAbaqusNodeSet(List<Node> nList)
        {
            AbaqusStringList.Add("*NSET,NSET=NODESET_1,GENERATE");
            string s = "1," + nList.Count + ",1";
            AbaqusStringList.Add(s);
            AbaqusStringList.Add("**");
        }

        //写入Abaqus单元集合
        public void WriteAbaqusElementSet(List<Element> elemList)
        {
            //分析Abaqus集合数据，为简化起见，每个单元分为一个组
            for (int i = 0; i < elemList.Count; i++)
            {
                Element elem = elemList[i];
                Node node1 = NodeList.Find(nd => nd.NO == elem.N1);
                Node node2 = NodeList.Find(nd => nd.NO == elem.N2);
                double ang = elem.Angle;
                //当把方向指定为(0,0,0)时，Abaqus会尝试指定默认方向(0,0,-1)
                double[] n1 = { 0, 0, 0 };
                
                //a=d,b=e,c!=f
                if (Math.Abs(node1.X - node2.X) < min && Math.Abs(node1.Y - node2.Y) < min && Math.Abs(node1.Z - node2.Z) > min)
                {
                    if (node2.Z > node1.Z)
                    {
                        n1 = new double[] { -Math.Sin(Math.PI * ang / 180), Math.Cos(Math.PI * ang / 180), 0.0 };
                    }
                    else
                    {
                        n1 = new double[] { Math.Sin(Math.PI * ang / 180), -Math.Cos(Math.PI * ang / 180), 0.0 };
                    }
                }
                //a!=d或者b!=e
                else if (Math.Abs(node1.X - node2.X) > min || Math.Abs(node1.Y - node2.Y) > min)
                {
                    double[] T = { node2.X - node1.X, node2.Y - node1.Y, node2.Z - node1.Z };
                    double modal = Math.Pow(T[0] * T[0] + T[1] * T[1] + T[2] * T[2], 0.5);
                    double[] a = { T[0] / modal, T[1] / modal, T[2] / modal };
                    SimpleMatrix A1 = new SimpleMatrix(new double[,] { 
                    {a[0]*a[0],a[0]*a[1],a[0]*a[2] },
                    {a[1]*a[0],a[1]*a[1],a[1]*a[2] },
                    {a[2]*a[0],a[2]*a[1],a[2]*a[2] }
                    });
                    SimpleMatrix A2 = new SimpleMatrix(new double[,]{
                    {0,-a[2],a[1]},
                    {a[2],0,-a[0]},
                    {-a[1],a[0],0}});
                    SimpleMatrix I = new SimpleMatrix(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
                    SimpleMatrix M = A1 + (Math.Cos(Math.PI * ang / 180)) * (I - A1) - (Math.Sin(Math.PI * ang / 180)) * A2;
                    SimpleMatrix N0 = new SimpleMatrix(new double[,] { { -a[1] }, { a[0] }, { 0 } });
                    SimpleMatrix N1 = M * N0;
                    n1 = new double[] { N1[0, 0], N1[1, 0], N1[2, 0] };
                    //n1 = new double[] { -N1[0, 0], -N1[1, 0], -N1[2, 0] };
                }
                //其他情况
                else
                {
                    n1 = new double[] { 0, 0, 0 };
                }


                if(elem.Type=="BEAM")
                {
                    Material mat = MaterialList.Find(mt => mt.NO == elem.Material);
                    Section sec = SectionList.Find(sc => sc.NO == elem.Section);
                    ElementSet set = new ElementSet
                    {
                        SetType = elem.Type,
                        SetName = "ElementSet-" + i,
                        SetMaterial = mat.Name,
                        SetSection = sec.AbaqusShape,
                        SetSectionN1 = n1
                    };
                    set.SetSectionParameter = sec.AbaqusSectionData;
                    set.SetNOList.Add(elem.NO);
                    ElementSetList.Add(set);
                }
                else if(elem.Type=="TRUSS")
                {
                    Material mat = MaterialList.Find(mt => mt.NO == elem.Material);
                    Section sec = SectionList.Find(sc => sc.NO == elem.Section);
                    ElementSet set = new ElementSet
                    {
                        SetType = elem.Type,
                        SetName = "ElementSet-" + i,
                        SetMaterial = mat.Name,
                        SetSection = sec.AbaqusShape,
                        SetSectionN1 = n1
                    };
                    set.SetSectionParameter = sec.Area.ToString()+",";
                    set.SetNOList.Add(elem.NO);
                    ElementSetList.Add(set);
                }
                else if(elem.Type=="TENSTR"||elem.Type=="COMPTR")
                {
                    Material mat = MaterialList.Find(mt => mt.NO == elem.Material);
                    Section sec = SectionList.Find(sc => sc.NO == elem.Section);
                    ElementSet set = new ElementSet
                    {
                        SetType = elem.Type,
                        SetName = "ElementSet-" + i,
                        SetMaterial = mat.Name,
                        SetSection = sec.AbaqusShape,
                        SetSectionN1 = n1
                    };
                    set.SetSectionParameter = sec.Area.ToString() + ",";
                    set.SetNOList.Add(elem.NO);
                    ElementSetList.Add(set);
                }
                else if(elem.Type=="PLATE")
                {

                }
                else
                {
                    
                }
                
            }

            //写入Abaqus单元集合数据，每行最多16个
            for (int i = 0; i < ElementSetList.Count; i++)
            {
                //如果需要导入CAE，则需要在句尾加上INTERNAL
                AbaqusStringList.Add("*ELSET,ELSET=" + ElementSetList[i].SetName);
                int count = ElementSetList[i].SetNOList.Count;
                int a = count / 16;
                int b = count % 16;
                if (a > 0)
                {
                    for (int j = 0; j < a; j++)
                    {
                        string s1 = "";
                        for (int k = 0; k < 16; k++)
                        {
                            s1 += ElementSetList[i].SetNOList[j * 16 + k] + ",";
                        }
                        AbaqusStringList.Add(s1);
                    }
                }
                if (b > 0)
                {
                    string s2 = "";
                    for (int j = 0; j < b; j++)
                    {
                        s2 += ElementSetList[i].SetNOList[a * 16 + j] + ",";
                    }
                    AbaqusStringList.Add(s2);
                }
            }
            AbaqusStringList.Add("**");
        }

        //写入Abaqus截面属性
        public void WriteAbaqusBeamSection()
        {
            for (int i = 0; i < ElementSetList.Count;i++ )
            {
                if (ElementSetList[i].SetType == "BEAM")
                {
                    AbaqusStringList.Add("*Beam Section,ELSET=" + ElementSetList[i].SetName + ",MATERIAL=" + ElementSetList[i].SetMaterial + ",SECTION=" + ElementSetList[i].SetSection);
                    AbaqusStringList.Add(ElementSetList[i].SetSectionParameter);
                    string n1 = ElementSetList[i].SetSectionN1[0].ToString() + "," + ElementSetList[i].SetSectionN1[1].ToString() + "," + ElementSetList[i].SetSectionN1[2].ToString();
                    AbaqusStringList.Add(n1);
                    
                    //梁截面横向剪切刚度
                    //AbaqusStringList.Add("*TRANSVERSE SHEAR STIFFNESS");
                    //AbaqusStringList.Add("1.0E10,1.0E10,");
                }
                else if(ElementSetList[i].SetType=="TRUSS")
                {
                    AbaqusStringList.Add("*Solid Section,ELSET=" + ElementSetList[i].SetName + ",MATERIAL=" + ElementSetList[i].SetMaterial);
                    AbaqusStringList.Add(ElementSetList[i].SetSectionParameter);
                }
                else if(ElementSetList[i].SetType=="TENSTR"||ElementSetList[i].SetType=="COMPTR")
                {
                    AbaqusStringList.Add("*Solid Section,ELSET=" + ElementSetList[i].SetName + ",MATERIAL=" + ElementSetList[i].SetMaterial);
                    AbaqusStringList.Add(ElementSetList[i].SetSectionParameter);
                }
                else if(ElementSetList[i].SetType=="PLATE")
                {

                }
                else
                {
                    
                }
            }
            AbaqusStringList.Add("**");
        }

        //写入Abaqus壳单元厚度
        public void WriteAbaqusShellThickness()
        {

        }

        //将荷载转换为质量
        public void TransferLoadToMass()
        {
            int count = 1000000;
            //节点集中荷载
            int count1 = ConLoadList.Count;
            if (count1 > 0)
            {
                for (int i = 0; i < count1; i++)
                {
                    double load = Math.Abs(ConLoadList[i].FX) + Math.Abs(ConLoadList[i].FY) + Math.Abs(ConLoadList[i].FZ);
                    load = load / 9.806;
                    if(ConLoadList[i].LoadCaseType=="D")
                    {
                        AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        AbaqusStringList.Add(count.ToString() + "," + ConLoadList[i].NodeNO.ToString());
                        AbaqusStringList.Add("*Mass,elset=set-" + count);
                        AbaqusStringList.Add(load.ToString() + ",");
                        count++;
                    }
                    else if(ConLoadList[i].LoadCaseType=="L")
                    {
                        load = load * 0.5;
                        AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        AbaqusStringList.Add(count.ToString() + "," + ConLoadList[i].NodeNO.ToString());
                        AbaqusStringList.Add("*Mass,elset=set-" + count);
                        AbaqusStringList.Add(load.ToString() + ",");
                        count++;
                    }
                    //else if (ConLoadList[i].LoadCaseType == "S")
                    //{
                    //    load = load * 0.5;
                    //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                    //    AbaqusStringList.Add(count.ToString() + "," + ConLoadList[i].NodeNO.ToString());
                    //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                    //    AbaqusStringList.Add(load.ToString() + ",");
                    //    count++;
                    //}

                }
            }

            //单元均布荷载
            int count2 = BeamLoadList.Count;
            if(count2>0)
            {
                for(int i=0;i<count2;i++)
                {
                    Element elem = ElementList.Find(el => el.NO == BeamLoadList[i].ElementNO);
                    Node n1 = NodeList.Find(nd => nd.NO == elem.N1);
                    Node n2 = NodeList.Find(nd => nd.NO == elem.N2);
                    double beamLength = Math.Pow((n2.X - n1.X) * (n2.X - n1.X) + (n2.Y - n1.Y) * (n2.Y - n1.Y) + (n2.Z - n1.Z) * (n2.Z - n1.Z), 0.5);
                    double load=BeamLoadList[i].P1*beamLength;
                    load = Math.Abs(load);
                    load = load / 9.806;
                    load=load*0.5;//分到两个节点

                    if(BeamLoadList[i].LoadCaseType=="D")
                    {
                        AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                        AbaqusStringList.Add("*Mass,elset=set-" + count);
                        AbaqusStringList.Add(load.ToString() + ",");
                        count++;
                        AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                        AbaqusStringList.Add("*Mass,elset=set-" + count);
                        AbaqusStringList.Add(load.ToString() + ",");
                        count++;
                    }
                    else if(BeamLoadList[i].LoadCaseType=="L")
                    {
                        load = load * 0.5;
                        AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                        AbaqusStringList.Add("*Mass,elset=set-" + count);
                        AbaqusStringList.Add(load.ToString() + ",");
                        count++;
                        AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                        AbaqusStringList.Add("*Mass,elset=set-" + count);
                        AbaqusStringList.Add(load.ToString() + ",");
                        count++;
                    }
                    //else if (BeamLoadList[i].LoadCaseType == "S")
                    //{
                    //    load = load * 0.5;
                    //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                    //    AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                    //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                    //    AbaqusStringList.Add(load.ToString() + ",");
                    //    count++;
                    //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                    //    AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                    //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                    //    AbaqusStringList.Add(load.ToString() + ",");
                    //    count++;
                    //}
                }
            }

            //板单元荷载
            int count3 = PressureLoadList.Count;
            if(count3>0)
            {
                for (int i = 0; i < count3; i++)
                {
                    Element elem = ElementList.Find(el => el.NO == PressureLoadList[i].ElementNO);
                    Node n1 = NodeList.Find(nd => nd.NO == elem.N1);
                    Node n2 = NodeList.Find(nd => nd.NO == elem.N2);
                    Node n3 = NodeList.Find(nd => nd.NO == elem.N3);
                    //三节点板
                    if (elem.N4 == 0)
                    {
                        double[] aa = { n2.X - n1.X, n2.Y - n1.Y, 0.0 };
                        double[] bb = { n3.X - n1.X, n3.Y - n1.Y, 0.0 };
                        double cosa = (aa[0] * bb[0] + aa[1] * bb[1] + aa[2] * bb[2]) / (Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5));
                        double sina = Math.Pow(1 - cosa * cosa, 0.5);
                        double area = 0.5 * Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5) * sina;
                        double load = PressureLoadList[i].PU * area / 3.0;
                        load = Math.Abs(load);
                        load = load / 9.806;

                        if(PressureLoadList[i].LoadCaseType=="D")
                        {
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n3.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                        }
                        else if(PressureLoadList[i].LoadCaseType=="L")
                        {
                            load = load * 0.5;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n3.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                        }
                        //else if (PressureLoadList[i].LoadCaseType == "S")
                        //{
                        //    load = load * 0.5;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n3.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //}

                    }
                    //四节点板
                    else
                    {
                        Node n4 = NodeList.Find(nd => nd.NO == elem.N4);
                        double[] aa = { n2.X - n1.X, n2.Y - n1.Y, 0.0 };
                        double[] bb = { n3.X - n1.X, n3.Y - n1.Y, 0.0 };
                        double cosa1 = (aa[0] * bb[0] + aa[1] * bb[1] + aa[2] * bb[2]) / (Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5));
                        double sina1 = Math.Pow(1 - cosa1 * cosa1, 0.5);
                        double area1 = 0.5 * Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5) * sina1;

                        double[] cc = { n3.X - n1.X, n3.Y - n1.Y, 0.0 };
                        double[] dd = { n4.X - n1.X, n4.Y - n1.Y, 0.0 };
                        double cosa2 = (cc[0] * dd[0] + cc[1] * dd[1] + cc[2] * dd[2]) / (Math.Pow(cc[0] * cc[0] + cc[1] * cc[1] + cc[2] * cc[2], 0.5) * Math.Pow(dd[0] * dd[0] + dd[1] * dd[1] + dd[2] * dd[2], 0.5));
                        double sina2 = Math.Pow(1 - cosa2 * cosa2, 0.5);
                        double area2 = 0.5 * Math.Pow(cc[0] * cc[0] + cc[1] * cc[1] + cc[2] * cc[2], 0.5) * Math.Pow(dd[0] * dd[0] + dd[1] * dd[1] + dd[2] * dd[2], 0.5) * sina2;

                        double load = PressureLoadList[i].PU * (area1 + area2) / 4.0;
                        load = Math.Abs(load);
                        load = load / 9.806;

                        if (PressureLoadList[i].LoadCaseType == "D")
                        {
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n3.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n4.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                        }
                        else if (PressureLoadList[i].LoadCaseType == "L")
                        {
                            load = load * 0.5;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n3.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                            AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                            AbaqusStringList.Add(count.ToString() + "," + n4.NO.ToString());
                            AbaqusStringList.Add("*Mass,elset=set-" + count);
                            AbaqusStringList.Add(load.ToString() + ",");
                            count++;
                        }
                        //else if (PressureLoadList[i].LoadCaseType == "S")
                        //{
                        //    load = load * 0.5;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n1.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n2.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n3.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //    AbaqusStringList.Add("*Element,type=MASS,elset=set-" + count);
                        //    AbaqusStringList.Add(count.ToString() + "," + n4.NO.ToString());
                        //    AbaqusStringList.Add("*Mass,elset=set-" + count);
                        //    AbaqusStringList.Add(load.ToString() + ",");
                        //    count++;
                        //}

                    }
                }

            }
        }

        //写入Abaqus材料参数
        public void WriteAbaqusMaterial()
        {
            foreach (Material mt in MaterialList)
            {

                AbaqusStringList.Add("*MATERIAL,NAME="+mt.Name);
                AbaqusStringList.Add("*DENSITY");
                AbaqusStringList.Add(mt.Density.ToString());
                AbaqusStringList.Add("*ELASTIC");
                AbaqusStringList.Add(mt.YoungsModulu.ToString()+","+mt.PoissonRatio.ToString());
            }
            AbaqusStringList.Add("**");
        }

        //写入Abaqus梁端释放
        public void WriteAbaqusRelease()
        {
            if (ReleaseList.Count > 0)
            {
                AbaqusStringList.Add("*RELEASE");
                for (int i = 0; i < ReleaseList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ReleaseList[i].AbaqusRelease1))
                    {
                        string s1 = ReleaseList[i].ElementNO + ",S1," + ReleaseList[i].AbaqusRelease1;
                        AbaqusStringList.Add(s1);
                    }
                    if (!string.IsNullOrEmpty(ReleaseList[i].AbaqusRelease2))
                    {
                        string s2 = ReleaseList[i].ElementNO + ",S2," + ReleaseList[i].AbaqusRelease2;
                        AbaqusStringList.Add(s2);
                    }
                }
                AbaqusStringList.Add("**");
            }
        }

        //写入Abaqus边界条件
        public void WriteAbaqusBoundary()
        {
            if (BoundaryList.Count > 0)
            {
                AbaqusStringList.Add("*BOUNDARY");
                for (int i = 0; i < BoundaryList.Count; i++)
                {
                    List<string> ls = BoundaryList[i].GetAbaqusBoundary();
                    for (int j = 0; j < ls.Count; j++)
                    {
                        AbaqusStringList.Add(ls[j]);
                    }
                }
                AbaqusStringList.Add("**");
            }
        }

        //写入Abaqus静力分析步
        public void WriteAbaqusStep(List<ConLoad> cLoadList, List<BeamLoad> dLoadList)
        {
            AbaqusStringList.Add("*STEP,NAME=STEP-Static");
            AbaqusStringList.Add("**");
            AbaqusStringList.Add("*STATIC");
            AbaqusStringList.Add("0.01,1,0.000001,0.1");

            WriteAbaqusLoadData(cLoadList,dLoadList);
            WriteAbaqusFieldOutput();
            WriteAbaqusHistoryOutput();

            AbaqusStringList.Add("*END STEP");
            AbaqusStringList.Add("**");
        }

        //写入Abaqus模态分析步
        public void WriteAbaqusModalStep()
        {
            AbaqusStringList.Add("*Step, name=Step-Modal, perturbation");
            AbaqusStringList.Add("*Frequency, eigensolver=Lanczos, acoustic coupling=on, normalization=displacement");
            //计算前20阶模态
            AbaqusStringList.Add("20, , , , ,");
            AbaqusStringList.Add("**OUTPUT REQUESTS");
            AbaqusStringList.Add("*Restart, write, frequency=0");
            AbaqusStringList.Add("** FIELD OUTPUT: F-Output-1");
            AbaqusStringList.Add("*Output, field, variable=PRESELECT");
            AbaqusStringList.Add("*End Step");
            AbaqusStringList.Add("**");
        }

        //写入Abaqus稳定分析步
        public void WriteAbaqusBuckleStep(List<ConLoad> cLoadList, List<BeamLoad> dLoadList)
        {
            AbaqusStringList.Add("**Step:Buckle");
            AbaqusStringList.Add("*STEP,NAME=STEP-Buckle, perturbation");
            AbaqusStringList.Add("*Buckle");
            AbaqusStringList.Add("5, , 10, 10000");
            WriteAbaqusLoadData(cLoadList, dLoadList);
            AbaqusStringList.Add("** OUTPUT REQUESTS");
            AbaqusStringList.Add("*Restart, write, frequency=0");
            AbaqusStringList.Add("** FIELD OUTPUT: F-Output-1");
            AbaqusStringList.Add("*Output, field, variable=PRESELECT");
            AbaqusStringList.Add("*END STEP");
            AbaqusStringList.Add("**");
        }

        //写入Abaqus荷载数据
        public void WriteAbaqusLoadData(List<ConLoad> cLoadList,List<BeamLoad> dLoadList)
        {
            //*DLOAD为梁均布荷载，*CLOAD为节点集中荷载
            //重力荷载
            AbaqusStringList.Add("*DLOAD");
            AbaqusStringList.Add(",GRAV,9.806, 0., 0., -1.");
            AbaqusStringList.Add("**");

            //梁上均布线荷载
            int count = dLoadList.Count;
            if(count>0)
            {
                AbaqusStringList.Add("*DLOAD");
                for(int i=0;i<count;i++)
                {
                    AbaqusStringList.Add(dLoadList[i].ElementNO.ToString()+","+dLoadList[i].AbaqusDirection+","
                        +dLoadList[i].P1);
                }
            }
            AbaqusStringList.Add("**");

            //节点集中荷载
            int count2 = cLoadList.Count;
            if (count2 > 0)
            {
                AbaqusStringList.Add("*CLOAD");
                for (int i = 0; i < count2; i++)
                {
                    if(Math.Abs( cLoadList[i].FX)>min)
                    {
                        AbaqusStringList.Add(cLoadList[i].NodeNO.ToString() + ",1," + cLoadList[i].FX);
                    }
                    if (Math.Abs(cLoadList[i].FY) > min)
                    {
                        AbaqusStringList.Add(cLoadList[i].NodeNO.ToString() + ",2," + cLoadList[i].FY);
                    }
                    if (Math.Abs(cLoadList[i].FZ) > min)
                    {
                        AbaqusStringList.Add(cLoadList[i].NodeNO.ToString() + ",3," + cLoadList[i].FZ);
                    }
                    if (Math.Abs(cLoadList[i].MX) > min)
                    {
                        AbaqusStringList.Add(cLoadList[i].NodeNO.ToString() + ",4," + cLoadList[i].MX);
                    }
                    if (Math.Abs(cLoadList[i].MY) > min)
                    {
                        AbaqusStringList.Add(cLoadList[i].NodeNO.ToString() + ",5," + cLoadList[i].MY);
                    }
                    if (Math.Abs(cLoadList[i].MZ) > min)
                    {
                        AbaqusStringList.Add(cLoadList[i].NodeNO.ToString() + ",6," + cLoadList[i].MZ);
                    }
                }
            }

            //Midas的压力荷载转换为节点荷载
            //一般压力荷载都是竖直的，所以可以通过将节点坐标Z轴设为一致得到投影面积，哈哈！
            int count3 = PressureLoadList.Count;
            if(count3>0)
            {
                AbaqusStringList.Add("*CLOAD");
                for(int i=0;i<count3;i++)
                {
                    Element elem = ElementList.Find(el => el.NO == PressureLoadList[i].ElementNO);
                    Node n1 = NodeList.Find(nd => nd.NO == elem.N1);
                    Node n2 = NodeList.Find(nd => nd.NO == elem.N2);
                    Node n3 = NodeList.Find(nd => nd.NO == elem.N3);
                    //三节点板
                    if (elem.N4 == 0)
                    {
                        double[] aa = { n2.X - n1.X, n2.Y - n1.Y, 0.0};
                        double[] bb = { n3.X - n1.X, n3.Y - n1.Y, 0.0 };
                        double cosa = (aa[0] * bb[0] + aa[1] * bb[1] + aa[2] * bb[2]) / (Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5));
                        double sina = Math.Pow(1 - cosa * cosa, 0.5);
                        double area = 0.5 * Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5) * sina;
                        double load = PressureLoadList[i].PU * area / 3.0;

                        AbaqusStringList.Add(elem.N1.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());
                        AbaqusStringList.Add(elem.N2.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());
                        AbaqusStringList.Add(elem.N3.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());

                    }
                    //四节点板
                    else
                    {
                        Node n4 = NodeList.Find(nd => nd.NO == elem.N4);
                        double[] aa = { n2.X - n1.X, n2.Y - n1.Y, 0.0 };
                        double[] bb = { n3.X - n1.X, n3.Y - n1.Y, 0.0 };
                        double cosa1 = (aa[0] * bb[0] + aa[1] * bb[1] + aa[2] * bb[2]) / (Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5));
                        double sina1 = Math.Pow(1 - cosa1 * cosa1, 0.5);
                        double area1 = 0.5 * Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5) * sina1;
                        
                        double[] cc = { n3.X - n1.X, n3.Y - n1.Y, 0.0 };
                        double[] dd = { n4.X - n1.X, n4.Y - n1.Y, 0.0 };
                        double cosa2 = (cc[0] * dd[0] + cc[1] * dd[1] + cc[2] * dd[2]) / (Math.Pow(cc[0] * cc[0] + cc[1] * cc[1] + cc[2] * cc[2], 0.5) * Math.Pow(dd[0] * dd[0] + dd[1] * dd[1] + dd[2] * dd[2], 0.5));
                        double sina2 = Math.Pow(1 - cosa2 * cosa2, 0.5);
                        double area2 = 0.5 * Math.Pow(cc[0] * cc[0] + cc[1] * cc[1] + cc[2] * cc[2], 0.5) * Math.Pow(dd[0] * dd[0] + dd[1] * dd[1] + dd[2] * dd[2], 0.5) * sina2;

                        double load = PressureLoadList[i].PU * (area1+area2) / 4.0;

                        AbaqusStringList.Add(elem.N1.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());
                        AbaqusStringList.Add(elem.N2.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());
                        AbaqusStringList.Add(elem.N3.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());
                        AbaqusStringList.Add(elem.N4.ToString() + "," + PressureLoadList[i].AbaqusDirection + "," + load.ToString());
                    }
                }
            }


            AbaqusStringList.Add("**");

        }

        //写入Abaqus场变量输出
        public void WriteAbaqusFieldOutput()
        {
            AbaqusStringList.Add("*OUTPUT,FIELD");
            AbaqusStringList.Add("*ELEMENT OUTPUT,DIRECTIONS=YES");
            AbaqusStringList.Add("E,S,SE,SF");
            AbaqusStringList.Add("*NODE OUTPUT");
            AbaqusStringList.Add("RF,U");
            AbaqusStringList.Add("**");
        }

        //写入Abaqus历史变量输出
        public void WriteAbaqusHistoryOutput()
        {
            AbaqusStringList.Add("*OUTPUT,HISTORY,VARIABLE=PRESELECT");
            AbaqusStringList.Add("**");
        }


        //将Abaqus数据写入inp文件
        public void WriteAbaqusInpFile()
        {
            //静力分析步
            AbaqusStringList.Clear();
            WriteAbaqusInfoData();
            AddMeshData();//此处增加节点后，后面不需要再次增加
            WriteAbaqusNodeData(NodeList);
            WriteAbaqusElementData(ElementList);
            WriteAbaqusNodeSet(NodeList);
            WriteAbaqusElementSet(ElementList);
            WriteAbaqusBeamSection();
            WriteAbaqusShellThickness();
            WriteAbaqusMaterial();
            WriteAbaqusRelease();
            WriteAbaqusBoundary();
            WriteAbaqusStep(ConLoadList, BeamLoadList);

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath+"M2A-Static.inp"))
                {
                    foreach (string s in AbaqusStringList)
                    {
                        sw.WriteLine(s);
                    }
                }

                //振型模态分析步
                AbaqusStringList.Clear();
                WriteAbaqusInfoData();
                WriteAbaqusNodeData(NodeList);
                WriteAbaqusElementData(ElementList);
                WriteAbaqusNodeSet(NodeList);
                WriteAbaqusElementSet(ElementList);
                WriteAbaqusBeamSection();
                WriteAbaqusShellThickness();
                //TransferLoadToMass();
                WriteAbaqusMaterial();
                WriteAbaqusRelease();
                WriteAbaqusBoundary();
                //WriteAbaqusStep(ConLoadList, BeamLoadList);
                WriteAbaqusModalStep();
                using (StreamWriter sw = new StreamWriter(filePath + "M2A-Modal.inp"))
                {
                    foreach (string s in AbaqusStringList)
                    {
                        sw.WriteLine(s);
                    }
                }

                //屈曲模态分析步
                AbaqusStringList.Clear();
                WriteAbaqusInfoData();
                WriteAbaqusNodeData(NodeList);
                WriteAbaqusElementData(ElementList);
                WriteAbaqusNodeSet(NodeList);
                WriteAbaqusElementSet(ElementList);
                WriteAbaqusBeamSection();
                WriteAbaqusShellThickness();
                WriteAbaqusMaterial();
                WriteAbaqusRelease();
                WriteAbaqusBoundary();
                WriteAbaqusBuckleStep(ConLoadList, BeamLoadList);
                using (StreamWriter sw = new StreamWriter(filePath + "M2A-Buckle.inp"))
                {
                    foreach (string s in AbaqusStringList)
                    {
                        sw.WriteLine(s);
                    }
                }

                MessageBox.Show("转换完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入文件失败！");
            }

            //写入批处理文件
            using (StreamWriter sw = new StreamWriter(filePath + "CallAbaqus-Static.bat"))
            {
                sw.WriteLine("call abaqus interactive job=M2A-Static");
                sw.WriteLine("cmd/k");
            }
            using (StreamWriter sw = new StreamWriter(filePath + "CallAbaqus-Modal.bat"))
            {
                sw.WriteLine("call abaqus interactive job=M2A-Modal");
                sw.WriteLine("cmd/k");
            }
            using (StreamWriter sw = new StreamWriter(filePath + "CallAbaqus-Buckle.bat"))
            {
                sw.WriteLine("call abaqus interactive job=M2A-Buckle");
                sw.WriteLine("cmd/k");
            }
        }



        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            //提取Midas单元数据
            bool isElement = false;
            for (int i = 0; i < MidasStringList.Count; i++)
            {
                string s = MidasStringList[i];
                if (isElement && !string.IsNullOrEmpty(s) && !s.Contains(";"))
                {
                    string[] es = s.Split(',');
                    for (int j = 0; j < es.Count(); j++)
                    {
                        es[j] = es[j].Trim();
                    }
                    //*ELEMENT    ; Elements
                    //; iEL, TYPE, iMAT, iPRO, iN1, iN2, ANGLE, iSUB, EXVAL, iOPT(EXVAL2) ; Frame  Element
                    //; iEL, TYPE, iMAT, iPRO, iN1, iN2, ANGLE, iSUB, EXVAL, EXVAL2, bLMT ; Comp/Tens Truss
                    //; iEL, TYPE, iMAT, iPRO, iN1, iN2, iN3, iN4, iSUB, iWID             ; Planar Element
                    //; iEL, TYPE, iMAT, iPRO, iN1, iN2, iN3, iN4, iN5, iN6, iN7, iN8     ; Solid  Element
                    //; iEL, TYPE, iMAT, iPRO, iN1, iN2, REF, RPX, RPY, RPZ, iSUB, EXVAL  ; Frame(Ref. Point)
                    if (es[1] == "BEAM" || es[1] == "TRUSS")
                    {
                        ElementList.Add(new Element(int.Parse(es[0]), es[1], int.Parse(es[2]), int.Parse(es[3]), int.Parse(es[4]), int.Parse(es[5]), double.Parse(es[6])));
                    }
                    else if (es[1] == "TENSTR" || es[1] == "COMPTR")
                    {
                        ElementList.Add(new Element(int.Parse(es[0]), es[1], int.Parse(es[2]), int.Parse(es[3]), int.Parse(es[4]), int.Parse(es[5]), double.Parse(es[6])));
                    }
                    else if (es[1] == "PLATE")
                    {
                        ElementList.Add(new Element(int.Parse(es[0]), es[1], int.Parse(es[2]), int.Parse(es[3]), int.Parse(es[4]), int.Parse(es[5]), int.Parse(es[6]), int.Parse(es[7]), int.Parse(es[8])));
                    }
                    else
                    {

                    }
                }
                if (s.Contains("*ELEMENT"))
                {
                    isElement = true;
                }
                if (isElement && MidasStringList[i + 1].Contains("*"))
                {
                    break;
                }
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath + "Midas2Ansys.inp"))
                {
                    sw.WriteLine("!单元数据信息");
                    sw.WriteLine("type,1");
                    sw.WriteLine("mat,2");
                    foreach (var elem in ElementList)
                    {
                        string s1 = "secnum,"+elem.Section;
                        string s2 = "en,"+elem.NO+","+elem.N1+","+elem.N2;
                        
                        sw.WriteLine(s1);
                        sw.WriteLine(s2);
                    }
                }
                MessageBox.Show("转换完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入文件失败！");
            }
        
            
            //using (StreamWriter sw = new StreamWriter(filePath + "test.txt"))
            //{
            //    foreach (string s1 in list1)
            //    {
            //        sw.WriteLine(s1);
            //    }
            //}
        }

        private void CalculateAreaButton_Click(object sender, RoutedEventArgs e)
        {
            Node n1 = new Node(1, double.Parse(TextBoxX1.Text), double.Parse(TextBoxY1.Text), double.Parse(TextBoxZ1.Text));
            Node n2 = new Node(2, double.Parse(TextBoxX2.Text), double.Parse(TextBoxY2.Text), double.Parse(TextBoxZ2.Text));
            Node n3 = new Node(3, double.Parse(TextBoxX3.Text), double.Parse(TextBoxY3.Text), double.Parse(TextBoxZ3.Text));

            double[] aa = { n2.X - n1.X, n2.Y - n1.Y, n2.Z - n1.Z };
            double[] bb = { n3.X - n1.X, n3.Y - n1.Y, n3.Z - n1.Z };
            double cosa = (aa[0] * bb[0] + aa[1] * bb[1] + aa[2] * bb[2]) / (Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5));
            double sina = Math.Pow(1 - cosa * cosa, 0.5);
            double area = 0.5 * Math.Pow(aa[0] * aa[0] + aa[1] * aa[1] + aa[2] * aa[2], 0.5) * Math.Pow(bb[0] * bb[0] + bb[1] * bb[1] + bb[2] * bb[2], 0.5) * sina;
            MessageBox.Show(area.ToString());

        }

        private void CalculateTotalAreaButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
