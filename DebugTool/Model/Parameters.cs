using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DebugTool.Model
{
    [Serializable]
    public class Parameters : System.ComponentModel.INotifyPropertyChanged
    {
        [field:NonSerialized]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private bool xpath;
        public bool Xpath
        {
            get { return xpath; }
            set
            {
                xpath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Xpath"));
                }
            }
        }
        public Parameters()
        {

        }
        private string parameter;
        public string Parameter
        {
            get { return parameter; }
            set
            {
                parameter = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Parameter"));
                }
            }
        }

        private ValueClass value;
        public ValueClass Value
        {
            get { return value; }
            set
            {
                this.value = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Value"));
                }
            }
        }
    }


    [Serializable]
    public class ValueClass : System.ComponentModel.INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public ValueClass()
        {


        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Name"));
                }
            }
        }

        private string value;
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Value"));
                }
            }
        }
    }

    [Serializable]
    public class SeleAction
    {
        public MethodInfo Method
        {
            get; set;
        }

        public ObservableCollection<Parameters> ParColl
        {
            get; set;
        }

        public SeleAction()
        {
            ParColl = new ObservableCollection<Parameters>();
        }

    }
}
