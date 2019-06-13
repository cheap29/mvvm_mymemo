using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mymemo.Model
{
    public class MainModel
    {
        public string Title { get; set; }
        public bool Done { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string Memo { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
