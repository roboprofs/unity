using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;

namespace Logging
{
    public class Logger
    {
        // hello ich bin das logger ;)
        private string filePathFormat = @".\Assets\Data\OutputData\{0}.csv";
        private string strSeperator = ",";

        private List<string[]> data = new List<string[]>();
        private long startTime;

        public void Start()
        {
            startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void Log(string name, string value)
        {
            long experimentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            data.Add(new string[] { experimentTime.ToString(), name, value });
        }

        public void Save(string filename)
        {
            string filePath = String.Format(filePathFormat, filename);
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("time,event,value");
                foreach (string[] d in data)
                {
                    sw.WriteLine(string.Join(strSeperator, d));
                }
            }
        }

    }
}