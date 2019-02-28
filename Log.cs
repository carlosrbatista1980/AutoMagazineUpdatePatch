using System;
using System.IO;
using System.Reflection;

public class LogAMG
{
    private string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private string filename = "\\log_instalação.txt";// + DateTime.Now.ToString("hhmmss") + ".txt";
    
    public void Log(string logMessage)
    {
        if (!File.Exists(path + filename))
        {
            File.Create(path + filename).Close();
        }


        using (StreamWriter w = File.AppendText(path + filename))
        {
            w.WriteLine(DateTime.Now.ToString("yyyy/mm/dd " + "hh:mm:ss") + " --> " + logMessage);
            w.Close();
        }
    }

}