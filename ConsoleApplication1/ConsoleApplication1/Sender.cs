using System.Collections.Generic;

namespace Frequenzverteilung
{
    class Sender
    {
        internal Sender() {}

        public int nr { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double r { get; set; }
        public List<int> overlaps = new List<int>();
        public List<int> disabled = new List<int>();
        public int frequency { get; set; }
    }
}
