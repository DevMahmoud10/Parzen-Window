using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pattern_Task5
{
    class Program
    {
        static void Main(string[] args)
        {
            General g = new General("Iris Data.txt");
            g.setIntervals(g.calculate_interval(0.4,3));
            g.parzenWindowClassifier();
            g.getAccuracy();
        }
    }
}
