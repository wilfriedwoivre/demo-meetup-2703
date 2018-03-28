using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2703.Domain
{
    public class Session
    {
        public Guid Id { get; set; }
        public string Name { get; set;  }
        public Difficulty Difficulty { get; set; }
        public int GameCount { get; set; }
        public long TotalScore { get; set; }
    }
}

