using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;

namespace CSVhelper
{
    public class CSVreader
    {
        
        public static int[,] LoadVisemeData(string @filePath, string dataForm)
        {
            using (var reader = new StreamReader(@filePath))
            {
                List<int> offsetData = new List<int>();
                List<int> visemeData = new List<int>();

                string headerLine = reader.ReadLine();
                string line;
                offsetData.Add(0);
                visemeData.Add(0);

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var values = line.Split(',');

                    offsetData.Add((int) (Convert.ToDouble(values[0]) * 10)); // Convert.ToInt32(values[0]) 
                    visemeData.Add(ConvertData(values[1], values[2], dataForm));
                    
                }
                int[] offsetArray = offsetData.ToArray();
                int[] visemeArray = visemeData.ToArray();

                int[,] outputData = new int[offsetArray.Length, 2];

                for (int i = 0; i < offsetArray.Length; i++)
                {
                    outputData[i, 0] = offsetArray[i];
                    outputData[i, 1] = visemeArray[i];
                }

                return outputData;
            }
        }

        private static int ConvertData(string valStr, string val2, string dataForm)
        {
            if (dataForm == "viseme")
            {
                char val = Convert.ToChar(valStr);
                if (val == 'I')
                {
                    switch (val2)
                    {
                        case "angry":
                            val = Convert.ToChar(val + 1);
                            break;
                        case "disgusted":
                            val = Convert.ToChar(val + 2);
                            break;
                        case "happy":
                            val = Convert.ToChar(val + 3);
                            break;
                        case "notImpressed":
                            val = Convert.ToChar(val + 4);
                            break;
                    }
                }
                return val - 'A';
            } else
            {
                int blinkingState = 0;
                switch (valStr)
                {
                    case "open":
                        switch (val2)
                        {
                            case "admonishing":
                                blinkingState = 3;
                                break;
                            case "angry":
                                blinkingState = 4;
                                break;
                            case "disgusted":
                                blinkingState = 5;
                                break;
                            case "happy":
                                blinkingState = 6;
                                break;
                            case "notImpressed":
                                blinkingState = 7;
                                break;
                            case "questioning":
                                blinkingState = 8;
                                break;
                            case "sad":
                                blinkingState = 9;
                                break;
                            default:
                                blinkingState = 0;
                                break;
                        }
                        break;
                    case "half_closed":
                        blinkingState = 1;
                        break;
                    case "closed":
                        blinkingState = 2;
                        break;
                }
                return blinkingState;
            }
        }
    }
}
