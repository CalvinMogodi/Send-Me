using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Enums
{    public enum UserType : int
    {
        // [Description("None")]
        [EnumMember]
        None = 0,

        // [Description("Admin")]
        [EnumMember]
        Admin = 1,

        // [Description("Normal User")]
        [EnumMember]
        NormalUser = 2,

        // [Description("Courier")]
        [EnumMember]
        Courier = 3,
    }
}
