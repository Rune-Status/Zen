using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;

namespace Zen.Core.IO
{
    public class LandscapeKeyTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly int[] EmptyKeys = new int[4];

        private readonly Dictionary<int, int[]> keys = new Dictionary<int, int[]>();

        public static LandscapeKeyTable Open(string directory)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException("Invalid directory specified.");

            var table = new LandscapeKeyTable();
            var regex = new Regex("^[0-9]+\\.txt$");

            var files = Directory
                .EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .Select(Path.GetFileName);

            foreach (var name in files)
            {
                if (!regex.IsMatch(name))
                    continue;

                var region = int.Parse(name.Substring(0, name.Length - 4));
                table.keys[region] = ReadKeys($"{directory}{name}");
            }


            Logger.Info($"Loaded {table.keys.Count} landscape keys.");
            return table;
        }

        private static int[] ReadKeys(string name)
        {
            using (var reader = new StreamReader(name))
            {
                var keys = new int[4];
                for (var i = 0; i < 4; i++)
                {
                    var result = reader.ReadLine();
                    if (result == null)
                        throw new Exception($"Invalid data in file {name}");

                    keys[i] = int.Parse(result);
                }
                return keys;
            }
        }

        public int[] GetKeys(int x, int y) => keys[(x << 8) | y] ?? EmptyKeys;
    }
}