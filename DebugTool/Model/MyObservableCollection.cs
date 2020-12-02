using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugTool.Model
{
 public   class MyObservableCollection<T>: ObservableCollection<T> 
    {
        private bool _ismyempty;
        public bool IsMyEmpty
        {
            get
            {
                return _ismyempty;
            }
        }

        public Func<MyObservableCollection<T>, bool> IsEmptyWithCondition
        {
            get; set;
        } = new Func<MyObservableCollection<T>, bool>(m => m.Count > 0);

        public  new void Add(T item)
        {
            base.Add(item);
            bool _temp = IsEmptyWithCondition.Invoke(this);
            if ( _temp != _ismyempty)
            {
                _ismyempty = _temp;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsMyEmpty"));
            }
           
        }

        public new bool Remove(T item)
        {
          bool result=  base.Remove(item);
            bool _temp = IsEmptyWithCondition.Invoke(this);
            if (_temp != _ismyempty)
            {
                _ismyempty = _temp;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsMyEmpty"));
            }
            return result;
        }

        public new void Clear ()
        {
            base.Clear();
            bool _temp = IsEmptyWithCondition.Invoke(this);
            if (_temp != _ismyempty)
            {
                _ismyempty = _temp;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsMyEmpty"));
            }

        }


        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            bool _temp = IsEmptyWithCondition.Invoke(this);
            if (_temp != _ismyempty)
            {
                _ismyempty = _temp;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsMyEmpty"));
            }
        }
       


    }
}
