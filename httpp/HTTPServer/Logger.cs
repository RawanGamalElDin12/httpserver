using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        
        
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 

            using (StreamWriter sr = File.AppendText("log.txt"))
            {
                sr.WriteLine("Datetime: {0}", System.DateTime.Now.ToString());
                sr.WriteLine("message: {0}", ex.Message);
                sr.Close();
                //gygvyv
            }
           
            
        }
    }
}
