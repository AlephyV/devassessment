using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleTimeshift
{
    public class Shifter
    {
        async static public Task Shift(Stream input, Stream output, TimeSpan timeSpan, Encoding encoding, int bufferSize = 1024, bool leaveOpen = false)
        {
            using (var sr = new StreamReader(input))
            {
                string line = String.Empty;
                List<string> lines = new List<string>();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("-->"))
                    {
                        var timestamps = line.Split(new char[] { ' ', '-', '-', '>', ' ' });
                        timestamps = timestamps.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        var timeSpanToAdd = timeSpan;

                        string[] timestampInitialArray = timestamps[0].Split(':');
                        var hourInitial = int.Parse(timestampInitialArray[0].PadLeft(2, '0'));
                        var minuteInitial = int.Parse(timestampInitialArray[1].PadLeft(2, '0'));
                        var secondInitial = int.Parse((timestampInitialArray[2].Split(',')[0]).PadLeft(2, '0')); //this is for a line in the .srt in incorrect format, i dont know if is proposital or a error in file so it was a correction
                        var milliSecondsInitial = int.Parse(timestampInitialArray[2].Split(',')[1].PadLeft(3, '0'));

                        var timeSpanInitial = new TimeSpan(0, hourInitial, minuteInitial, secondInitial, milliSecondsInitial);
                        var newTimeSpanInitial = timeSpanInitial + timeSpanToAdd;
                        var timeSpanInitialString = MontString(newTimeSpanInitial);

                        string[] timestampFinalArray = timestamps[1].Split(':');

                        //that's my first way before the wrong format that i said
                        //var hourFinal = int.Parse(timestamps[1].Substring(0, 2));

                        var hourFinal = int.Parse(timestampFinalArray[0].PadLeft(2, '0'));
                        var minuteFinal = int.Parse(timestampFinalArray[1].PadLeft(2, '0'));
                        var secondFinal = int.Parse((timestampFinalArray[2].Split(',')[0]).PadLeft(2, '0')); //this is for a line in the .srt in incorrect format, i dont know if is proposital or a error in file so it was a correction
                        var milliSecondsFinal = int.Parse(timestampFinalArray[2].Split(',')[1]);

                        var timeSpanFinal = new TimeSpan(0, hourFinal, minuteFinal, secondFinal, milliSecondsFinal);
                        var newTimeSpanFinal = timeSpanFinal + timeSpanToAdd;
                        var timeSpanFinalString = MontString(newTimeSpanFinal);

                        StringBuilder sbNewLine = new StringBuilder(timeSpanInitialString);
                        sbNewLine.Append(" --> ");
                        sbNewLine.Append(timeSpanFinalString);
                        sbNewLine.Append("\n");

                        string newLine = sbNewLine.ToString();
                        lines.Add(newLine);
                    }
                    else
                    {
                        lines.Add(line + "\n");
                    }
                }

                WriteOutput(output, lines, encoding);
            }
        }

        public static void WriteOutput(Stream output, List<string> lines, Encoding encondig)
        {
            using (var bw = new BinaryWriter(output))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    bw.Write(encondig.GetBytes(lines[i]));
                }
            }
        }

        public static string MontString(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds > 0)
            {
                return timeSpan.ToString().Substring(0, 12);
            }

            return (timeSpan.ToString() + ".").PadRight(12, '0');
        }
    }
}
