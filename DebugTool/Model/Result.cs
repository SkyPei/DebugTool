using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DebugTool.Model
{

   public class Result: System.ComponentModel.INotifyPropertyChanged
    {
       
        public Result()
        {
            Parameters = new List<string>();
        }
      
        public int ID
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }

        public string FunName
        { get; set; }

        private bool? _run;
        public bool? Run
        {
            get
            {
                return _run;
            }
            set
            {
                _run = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Run"));
                }
            }
        }


        private bool _status;
        public bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Status"));
                }
            }
        }

        private string _output;
        public string Output
        {
            get
            {
                return _output;
            }
            set
            {
                _output = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Output"));
                }
            }
        }

        private string _exception;
        public string Exception
        {
            get
            {
                return _exception;
            }
            set
            {
                _exception = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Exception"));
                }
            }
        }

        public List<string> Parameters
        {
            get; set;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
