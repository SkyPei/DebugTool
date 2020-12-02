using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugTool.Model
{
    [Serializable]
    public class PObejct : System.ComponentModel.INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

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
                if(this.PropertyChanged!=null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Name"));
                }
            }
        }

        private string _xpath;

        public string Xpath
        {
            get
            {
                return _xpath;
            }
            set
            {
                _xpath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Xpath"));
                }
            }
        }
    }
}
