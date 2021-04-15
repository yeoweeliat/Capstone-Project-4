using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Base Class - System.Exception -> 2 child classes
// (a) System.SystemException (used when type raised by OS or CLR)
// (b) System.ApplicationException (used when type raised by your application)

// when creating a custom exception, need to override 3 exceptions

namespace Grocery.WebApp.Services
{
    //[Nullable] //mark this class as nullable
    public class MyEmailSenderException : ApplicationException
    {
        private const string StandardErrorMessage = "Something went wrong when sending the email...";


        // call base class constructor (each bcc is parameterized, you need to pass the right parameter)
        public MyEmailSenderException() : base(StandardErrorMessage)
        {

        }

        public MyEmailSenderException(string message) : base(message)
        {

        }

        public MyEmailSenderException(string message, Exception innerexception) : base(message, innerexception)
        {

        }

    }
}
