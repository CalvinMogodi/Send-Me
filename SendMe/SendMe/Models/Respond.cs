using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMe.Models
{
    public class Respond
    {
        public Boolean IsSuccessful { get; set; }

        public Boolean ErrorOccurred { get; set; }

        public Error Error { get; set; }
    }
    public class Error {

        public Boolean DatabaseError { get; set; }
        public Boolean UserExist { get; set; }
        public Boolean UserIsAlreadyLoggedIn { get; set; }
        public string Message { get; set; }

    }
}
