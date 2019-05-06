using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleTimeshift
{
    public class Shifter
    {
        private const string identifier = "-->";
        
        async static public Task Shift(Stream input, Stream output, TimeSpan timeSpan, Encoding encoding, int bufferSize = 1024, bool leaveOpen = false)
        {
            using (var streamReader = new StreamReader(input, encoding, true, bufferSize, leaveOpen))
            using (var streamWriter = new StreamWriter(output, encoding, bufferSize, leaveOpen))
            {
                // Accept this pattern 00:00:49,203 and 00:00:49.203
                var culture = new CultureInfo("pt-BR");
                var stringBuilder = new StringBuilder();
                var line = string.Empty;                

                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    if (line.Contains(identifier))
                    {
                        string[] time = line.Split(new string[] { identifier }, StringSplitOptions.RemoveEmptyEntries);

                        TimeSpan.TryParse(time[0], culture, out TimeSpan start);
                        TimeSpan.TryParse(time[1], culture, out TimeSpan end);

                        var startTime = start.Add(timeSpan).ToString(@"hh\:mm\:ss\.fff");
                        var endTime = end.Add(timeSpan).ToString(@"hh\:mm\:ss\.fff");

                        stringBuilder.AppendLine(String.Format("{0} {1} {2}", startTime, identifier, endTime));                      
                    }
                    else
                    {
                        stringBuilder.AppendLine(line);                       
                    }
                }

                await streamWriter.WriteAsync(stringBuilder.ToString());
            }
        }
    }
}