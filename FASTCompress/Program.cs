using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace FASTCompress
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath=String.Empty, outputPath=String.Empty;
            long percent=0, minSize=0;
            string[] inputFiles  =null;
            int numThreads = Environment.ProcessorCount;
            ImageFunctions imgFunctions = new ImageFunctions();

            // Check for proper parameters
            if (args.Length < 4)
            {
                Console.WriteLine("\nUsage FASTCompress.exe <percent> <minimum size> <input path> <output path>\nMinimum size is in Kilobytes.\n");

                Exit();
            }

            try
            {
                inputPath = args[2];
                outputPath = args[3];
                percent = Convert.ToInt64(args[0]);
                minSize = Convert.ToInt64(args[1]) * 1024L;
            }
            catch //(Exception ex)
            {
                Console.WriteLine("\nUsage FASTCompress.exe <percent> <minimum size> <input path> <output path>\nMinimum size is in Kilobytes.\n");
                Exit();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Get a list of the files to process
            try
            {
                inputFiles = Directory.GetFiles(inputPath, "*.jpg", SearchOption.AllDirectories);
            }
            catch //(Exception ex)
            {
                Console.WriteLine("\nCould not find images or paths.\n");
                Exit();
            }

            Console.WriteLine("\nCompressing...\n");
                        
            // Reserve processor count # of threads
            Thread[] tArray = new Thread[numThreads];

            // Cycle through the list of files and compress
            Console.WriteLine("\nStarting New Batch\n");
            int count = 0;
            foreach(string inputFile in inputFiles)
            {
                string outputFile = outputPath + inputFile.Replace(inputPath, "");
                Console.WriteLine("Thread # {0}: {1} -> {2}\n", count, inputFile,outputFile);

                // Start each thread
                tArray[count] = new Thread(() => imgFunctions.ResampleImage(inputFile, outputFile, percent, minSize));
                tArray[count].Start();

                count++;

                // When all threads have started, use join to wait until all threads are done before starting new batch
                if (count == tArray.Length)
                {
                    for (int i = 0; i < tArray.Length; i++)
                    {
                        tArray[i].Join();
                    }
                    Console.WriteLine("\nStarting New Batch\n");
                    count = 0;
                }
            }
            
            stopwatch.Stop();
            long sec = stopwatch.ElapsedMilliseconds / 1000L;

            Console.WriteLine("\n\nProcessed {0} pictures in {1} seconds using {2} Cores.\n", inputFiles.Length, sec, numThreads);

            Exit();
        }

        // Exit Gracefully
        static void Exit()
        {
            // Keep the console window open if we are debugging!
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("\n\nHit Enter to Exit");
                Console.ReadLine();
            }

            // Close the program.
            Environment.Exit(0);
        }
    }
}
