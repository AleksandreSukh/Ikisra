using System;

namespace ProjectAPI.Controllers
{
    public class Statement
    {
        //[Key(true)]
        public Guid Oid { get; set; }
        public StatementState State { get; set; }
        public string Name { get; set; }
    }
}