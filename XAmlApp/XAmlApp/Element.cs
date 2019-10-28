using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAmlApp
{
    public enum IsElement : byte { IsDrive, IsDirectory, IsFile };
    public struct Element
    {
        public string Name { get; set; }
        public IsElement isElement { get; set; }
        public String Date { get; set; }
    }
    
}
