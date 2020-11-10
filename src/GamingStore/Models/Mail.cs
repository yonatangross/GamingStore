using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Models
{
    public class Mail
    {
        //todo: should be merged with the mail service
        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string Telephone { get; set; }
        
        public string Message { get; set; }
    }
}
