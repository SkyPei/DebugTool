using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugTool.Model
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public  class SeleniumAttribute: Attribute
    {
        public string Name
        {
            get; set;
        }
    }
}
