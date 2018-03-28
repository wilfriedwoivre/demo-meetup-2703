using System;

namespace Demo2703.Domain
{
    public class Operation
    {
        public Guid Id { get; set; }
        public int NumberA { get; set; }
        public int NumberB { get; set; }
        public Operand Operand { get; set; }
    }
}