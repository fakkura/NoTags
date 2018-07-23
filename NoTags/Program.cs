using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NoTags
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: NoTags.exe PATH");
                return 1;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("The path specified doesn't exist.");
                return 1;
            }

            try
            {
                var mp3Files = Directory.EnumerateFiles(args[0], "*.mp3", SearchOption.AllDirectories);

                foreach (string currentFile in mp3Files)
                {
                    var mp3 = File.ReadAllBytes(currentFile);

                    Console.WriteLine("Removing tags from " + currentFile + Environment.NewLine);

                    int skip = 0;
                    if (Encoding.ASCII.GetString(mp3, 0, 3) == "ID3")
                    {
                        Console.WriteLine("ID3v2 tags found, stripping.." + Environment.NewLine);
                        skip = 7 + BitConverter.ToInt32(mp3.Skip(6).Take(4).Reverse().ToArray(), 0);
                    }

                    int take = mp3.Length - skip;
                    if (Encoding.ASCII.GetString(mp3, mp3.Length - 128, 3) == "TAG")
                    {
                        Console.WriteLine("ID3v1 tags found, stripping.." + Environment.NewLine);
                        take -= 128;
                    }

                    File.WriteAllBytes(currentFile, mp3.Skip(skip).Take(take).ToArray());
                }

                Console.WriteLine("Finished stripping." + Environment.NewLine);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return 0;
        }
    }
}
