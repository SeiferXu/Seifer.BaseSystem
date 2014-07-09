using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seifer.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        private string description;
        public string Description
        {
            get { return this.description; }
        }

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="description"></param>  
        public EnumDescriptionAttribute(string description)
            : base()
        {
            this.description = description;
        }
    }
}
