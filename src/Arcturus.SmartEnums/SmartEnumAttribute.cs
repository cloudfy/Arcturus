using System;
using System.Collections.Generic;
using System.Text;

namespace Arcturus.SmartEnums;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SmartEnumAttribute : Attribute
{
    public SmartEnumAttribute(params string[] values)
    {
        Values = values;
    }

    public string[] Values { get; }
}
