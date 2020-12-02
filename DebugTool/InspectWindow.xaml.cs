using System;

using System.Reflection;
using System.Windows.Interop;
using mshtml;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DebugTool
{
    /// <summary>
    /// Interaction logic for InspectWindow.xaml
    /// </summary>
    public partial class InspectWindow :Window
    {
        private IHTMLElement lastEle = null;
        private char SPLIT = '"';
        private Cursor moveCursor = null;
        private IntPtr handle ;

        private int hotkeyId = 0;

        private System.Windows.Forms.Timer timer;

        private InspectInfo info;

        private System.Drawing.Point _point;



        public InspectWindow()
        {
            InitializeComponent();
            string prodversion = string.Empty;
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Version ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                prodversion = $"v{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            }
            else
            {
                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                prodversion = $"=debug Mode= v{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            }

            Title = Title + prodversion;
         

            info = new InspectInfo();
            grid.DataContext = info;


            timer = new Timer();
            timer.Interval = 250;
            timer.Tick += timer1_Tick;
            timer.Enabled = false;

        }

        //重写消息循环
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32API.WM_HOTKEY && wParam.ToInt32() == this.hotkeyId) //判断热键
            {
                captureIE();
            }

           
            return IntPtr.Zero;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            captureIE();
        }

        private void captureIE()
        {
            try
            {
                //清空iframecollection 
                

                IntPtr hWnd = Win32API.WindowFromPoint(System.Windows.Forms.Control.MousePosition);

                mshtml.IHTMLDocument2 doc2 = (mshtml.IHTMLDocument2)GetHtmlDocumentByHandle(hWnd);

                if (doc2 != null)
                {
                    System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;

                    Win32API.ScreenToClient(hWnd, ref point);

                    if (point != _point)
                    {
                        _point = point;
                        info.IFrame.Clear();
                        //IHTMLElement element = doc2.elementFromPoint(point.X, point.Y);
                        IHTMLElement element = GetElementFromPoint(doc2, point.X, point.Y);


                        readIEElement(element);
                    }
                   
                }
            }
            catch (Exception e)
            {
                timer.Enabled = false;
                System.Windows.MessageBox.Show("Something error：" + e.Message);
            }
        }

        private IHTMLElement GetElementFromPoint(IHTMLDocument2 doc2, int x, int y)
        {
            IHTMLElement element = null;
            element = doc2.elementFromPoint(x,y);

            
           
            if (element is HTMLIFrameClass )
            {
                //tagPOINT tagpoint = new tagPOINT();
                //tagpoint.x = x;
                //tagpoint.y = y;
                info.IFrame.Add(extractXPath(element));
                

                //IDisplayServices IDS = doc2 as IDisplayServices;
                //IDS.TransformPoint(ref tagpoint, _COORD_SYSTEM.COORD_SYSTEM_CONTENT, _COORD_SYSTEM.COORD_SYSTEM_FRAME,element);
                IHTMLRect clientRec = ((IHTMLElement2)element).getBoundingClientRect();
                IHTMLDocument2 newdoc2= CorssDomainHelper.GetDocumentFromWindow(((HTMLIFrameClass)element).contentWindow);

                element = GetElementFromPoint(newdoc2, x - clientRec.left, y - clientRec.top);
                
               
            }
            else if ( element is HTMLFrameElementClass)
            {
                info.IFrame.Add(extractXPath(element));


                //IDisplayServices IDS = doc2 as IDisplayServices;
                //IDS.TransformPoint(ref tagpoint, _COORD_SYSTEM.COORD_SYSTEM_CONTENT, _COORD_SYSTEM.COORD_SYSTEM_FRAME,element);
                IHTMLRect clientRec = ((IHTMLElement2)element).getBoundingClientRect();
                IHTMLDocument2 newdoc2 = CorssDomainHelper.GetDocumentFromWindow(((HTMLFrameElementClass)element).contentWindow);

                element = GetElementFromPoint(newdoc2, x - clientRec.left, y - clientRec.top);
            }

            return element;
        }
        private object GetHtmlDocumentByHandle(IntPtr hWnd)
        {
            string buffer = new string('\0', 24);
            Win32API.GetClassName(hWnd, ref buffer, 25);

            if (buffer != "Internet Explorer_Server")
                return null;

            return GetComObjectByHandle(Win32API.WM_HTML_GETOBJECT, Win32API.IID_IHTMLDocument, hWnd);
        }
        public static object GetComObjectByHandle(int Msg, Guid riid, IntPtr hWnd)
        {
            object _ComObject;
            int lpdwResult = 0;
            if (!Win32API.SendMessageTimeout(hWnd, Msg, 0, 0, Win32API.SMTO_ABORTIFHUNG, 1000, ref lpdwResult))
                return null;
            if (Win32API.ObjectFromLresult(lpdwResult, ref riid, 0, out _ComObject))
                return null;
            return _ComObject;
        }


        /**
        * 读取元素基本信息和提取XPATH
        */
        private void readIEElement(IHTMLElement e)
        {

            if (lastEle == e)
            {
                return;
            }

            clearBorderHint();

            this.lastEle = e;

            if (e == null)
            {
                return;
            }

            //画红框
            e.style.setAttribute("outline", "2px solid red");

            // 基本信息

            info.ID = e.id;
            info.Name= getElementAttribute(e, "name");
            info.Tag = e.tagName;
            info.Value= getElementAttribute(e, "Value");
            info.Text = e.innerText;
            info.Class = e.className;
            info.HTML = e.outerHTML;



            // xpath
            info.XPath= extractXPath(e);

        }

        private string getElementAttribute(IHTMLElement e, string name)
        {
            dynamic value = e.getAttribute(name);
            return value is System.DBNull ? "" : value + "";
        }

        /**
        * 去掉最后一个元素的红框 
        **/
        private void clearBorderHint()
        {
            // 
            if (this.lastEle != null)
            {
                try
                {
                    this.lastEle.style.setAttribute("outline", "");
                }
                catch
                {
                    //上一个元素可能不存在了
                }
            }
        }


        private string extractXPath(IHTMLElement e)
        {
           

            // id 
            if (e.id != null)
            {
               return "//*[@id=" + SPLIT + e.id + SPLIT + "]";
             
            }

            //往上找
            return getXPathEx(e);

           

           

        }

        /**
         * 一直往上找，找到有id的父元素为止，如果没有，就到html为止。
         * 
         * 返回格式如：
         * //*[@id="formConfig"]/INPUT[1]
         * /HTML/BODY/DIV[1]/DIV/DIV[1]/A[1]/SPAN
         */
        private string getXPathEx(IHTMLElement e)
        {
            IHTMLElement current = e;

            string xpath = "";

            while (current != null)
            {
                // 如果有id，结束
                if (current.id != null)
                {
                    xpath = "//*[@id=" + SPLIT + current.id + SPLIT + "]" + xpath;
                    break;
                }
                else
                {
                    string currentXpath = extractCurrentXpath(current);
                    xpath = currentXpath + xpath;
                }

                current = current.parentElement;
            }

            return xpath;
        }

        /**
        * 当前节点的xpath
        * 返回结果如: /INPUT[2]
        */
        private string extractCurrentXpath(IHTMLElement current)
        {
            string currentXpath = "/" + current.tagName;

            // 计算index
            int index = calculate(current);

            if (index >= 1)
            {
                currentXpath += "[" + index + "]";
            }

            return currentXpath;
        }




        /**
        * 计算当前元素在父元素中相同的tag中的index
        * xpath的index是从1开始的
        */
        private int calculate(IHTMLElement current)
        {
            if (current.parentElement == null)
            {
                return 0;
            }

            IHTMLElementCollection collection = (IHTMLElementCollection)current.parentElement.children;

            int length = collection.length;

            int index = 0, all = 0;

            for (var i = 0; i < length; i++)
            {
                IHTMLElement item =(IHTMLElement) collection.item(i);

                // 实际测试中发生过
                if (item == null)
                {
                    break;
                }

                if (item.tagName == current.tagName)
                {
                    all++;

                    if (item == current)
                    {
                        index = all;
                    }
                }
            }

            // 只有一个元素，就不需要[1]
            if (all == 1)
            {
                return 0;
            }

            // xpath不是从0开始
            return index;
        }

        private void inspectWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;
            this.moveCursor = new Cursor(new System.IO.MemoryStream(Properties.Resources.pen_r));
            Win32API.SetWindowPos(handle, -1, 0, 0, 0, 0, 1 | 2);
            if (!Win32API.RegisterHotKey(handle, this.hotkeyId, Win32API.KeyModifiers.None, Keys.F8))
            {
                System.Windows.MessageBox.Show("HotKey registeration failed.");
            }

        }

        private void inspectWindow_Closed(object sender, EventArgs e)
        {
            clearBorderHint();
            Win32API.UnregisterHotKey(handle, hotkeyId);
        }

        private void Button_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            timer.Enabled = true;
        }

        private void Button_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            timer.Enabled = false;
        }
    }


    public class InspectInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public InspectInfo()
        {
            _iframe = new ObservableCollection<string>();
        }

        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                }

            }

        }


        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }

            }

        }


        private string _tag;
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Tag"));
                }

            }
        }

        private string _class;
        public string Class
        {
            get
            {
                return _class;
            }
            set
            {
                _class = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Class"));
                }

            }

        }

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }

            }

        }


        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }

            }

        }



        private ObservableCollection<string> _iframe;
        public ObservableCollection<string> IFrame
        {
            get
            {
                return _iframe;
            }
            set
            {
                _iframe = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IFrame"));
                }

            }

        }

        private string _xpath;
        public string XPath
        {
            get
            {
                return _xpath;
            }
            set
            {
                _xpath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("XPath"));
                }

            }

        }


        private string _html;
        public string HTML
        {
            get
            {
                return _html;
            }
            set
            {
                _html = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HTML"));
                }

            }

        }


    }
}
