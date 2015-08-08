using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
// by Maxter

namespace Frequenzverteilung
{
    class Program
    {
        static int SenderIDCount = 1;
        static StreamReader ReadFile = null;

        static void Main(string[] args)
        {
            try
            {
                Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Read the Data from Text File.
        /// </summary>
        private static void Start()
        {
            Console.WriteLine("Please introduce the Path of the file and press ENTER");
            ReadFile = new StreamReader(Console.ReadLine());
            Console.WriteLine(Environment.NewLine);

            while (!ReadFile.EndOfStream)
            {
                string[] lines = ReadFile.ReadLine().Split(' ');

                if (Char.IsNumber(lines[0], 0))
                {
                    Sender sender = new Sender()
                    {
                        nr = SenderIDCount,
                        x = double.Parse(lines[0], CultureInfo.InvariantCulture),
                        y = double.Parse(lines[1], CultureInfo.InvariantCulture),
                        r = double.Parse(lines[2], CultureInfo.InvariantCulture)
                    };
                    SenderIDCount++;
                    SenderCollection.senderCollection.Add(sender);
                }
            }
            CalculateOverlaps();

            for (int i = 1; i != SenderIDCount; i++)
                FindFrequencies();

            Output();
        }

        private static void Output()
        {
            Console.WriteLine("Senderpositionen (X,Y) und Senderadien:" + Environment.NewLine);
            foreach (Sender sender in SenderCollection.senderCollection)
                Console.WriteLine(String.Format("S{0}: {1}.000, {2}.000, {3}.000", sender.nr, sender.x, sender.y, sender.r));

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Frequenzzuordnung:" + Environment.NewLine);

            foreach (Sender sender in SenderCollection.SenderWithFrequencies)
                Console.WriteLine(String.Format("S{0} -> {1}", sender.nr, sender.frequency));

            Console.WriteLine(Environment.NewLine);
            int count = 1;
            do
            {
                string Sender = String.Empty;
                string s = String.Empty;
                foreach (Sender sender in SenderCollection.SenderWithFrequencies)
                {
                    if (sender.frequency == count)
                    {
                        s += " S" + sender.nr;
                        Sender = String.Format("Frequenz: {0}  Sender: {1}", sender.frequency, s);
                    }
                }
                count++;
                if (!String.IsNullOrEmpty(Sender))
                    Console.WriteLine(Sender);

            } while (count != SenderCollection.SenderWithFrequencies.Count);
            Console.ReadKey();
        }

        /// <summary>
        /// SAVE THE DISABLED(barrier) FREQUENCIES
        /// </summary>
        /// <param name="frequence"> Get the frequencies from "FindFrequencies()" method </param>
        /// <param name="senderOverlaps"> Get the Overlaps from the "FindFrequencies()" method </param>
        private static void DisableFrequencies(int frequence, List<int> senderOverlaps)
        {
            foreach (int overlap in senderOverlaps)
            {
                // If the Sender from " SenderCollection List" is equal to overlaps, save the disabled frequencies for not used it again
                SenderCollection.senderCollection.Where(sender => sender.nr == overlap).ElementAt(0).disabled.Add(frequence);
            }
        }

        /// <summary>
        /// GET THE FREQUENCIES
        /// Find the Disabled(Barrier) frequencies and save it in "SenderWithFrequencies List".
        /// </summary>
        private static void FindFrequencies()
        {
            // Temporal "tempSender Instance" for Frequencies comparation.
            Sender tempSender = new Sender();

            foreach (Sender sender in SenderCollection.senderCollection)
            {
                if (sender.frequency == 0)
                {
                    if (sender.disabled.Count > tempSender.disabled.Count)
                        tempSender = sender;
                    
                    // Phase 1 --> Choose the Sender with the most Overlaps ( 1. die meisten Überschneidungen)
                    else if (sender.overlaps.Count > tempSender.overlaps.Count)
                        tempSender = sender;

                    else if (sender.overlaps.Count == tempSender.overlaps.Count)
                    {
                        // Phase 2 --> Choose the Sender where X have the lowest coordinate ( 2. den westlichsten (kleinste x-Koordinate))
                        if (sender.x < tempSender.x)
                            tempSender = sender;

                        else if (sender.x == tempSender.x)
                        {
                            // Phase 3 --> Choose the Sender where Y have the lowest coordinate ( 3. den südlichsten (kleinste y-Koordinate))
                            if (sender.y < tempSender.y)
                                tempSender = sender;
                        }
                    }
                }
            }

            for (int i = SenderIDCount; i > 0; i--)
            {
                // Get and Asing the most low Frequencie to the Sender
                if (!tempSender.disabled.Contains(i))
                    tempSender.frequency = i;
            }
            // Save the obteined frequencies in the "SenderwithFrequencies List".
            SenderCollection.SenderWithFrequencies.Add(tempSender);
            DisableFrequencies(tempSender.frequency, tempSender.overlaps);
        }

        /// <summary>
        /// CALCULATE THE SENDER WITH MOST OVERLAPS TO CHOOSE THE INITIAL SENDER
        /// Get the sender from a "SenderCollection List", calculate the overlaps and save the result in a "Overlaps List".
        /// Nested " Foreach " used for the calculation
        /// </summary>
        private static void CalculateOverlaps()
        {
            foreach (Sender sender in SenderCollection.senderCollection)
            {
                sender.overlaps = new List<int>();
                foreach (Sender sender2 in SenderCollection.senderCollection)
                {
                    // Check that the sender don't multiply with it self
                    if (sender.nr != sender2.nr)
                    {
                        // Multiply the X , Y coordinates, get the Sum of the Radius and save the Sender with most Overlaps.
                        if (sender.r + sender2.r > Math.Sqrt(Convert.ToDouble(((sender2.x - sender.x) * (sender2.x - sender.x)) + ((sender2.y - sender.y) * (sender2.y - sender.y)))))
                            sender.overlaps.Add(sender2.nr);
                    }
                }
            }
        }
    }
}
