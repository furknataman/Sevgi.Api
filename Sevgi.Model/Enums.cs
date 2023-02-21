using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevgi.Model
{

    public enum AuthProviders
    {
        INTERNAL = 0,
        GOOGLE = 1,
        APPLE = 2,
        FIREBASE = 3
    }

    public enum VerificationMethods
    {
        NONE = 0,
        EMAIL = 1,
        PHONE = 2,
        USERNAME = 3
    }
    public enum Genders { 
        MALE,
        FEMALE
    }
}
