using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {


            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] seprators = new string[1];
            seprators[0] = "\r\n";
            contentLines = requestString.Split(seprators, StringSplitOptions.None);
            headerLines = new Dictionary<string, string>();
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
                return false;
            // Parse Request line
            if (ParseRequestLine() == false)
                return false;
            // Validate blank line exists
            if (ValidateBlankLine() == false)
                return false;
            // Load header lines into HeaderLines dictionary
            if (LoadHeaderLines() == false)
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            try

            {
                requestLines = contentLines[0].Split(' ');


                if (requestLines.Length < 2 || requestLines.Length >3)
                    return false;
                

                if (requestLines.Length == 2)
                    httpVersion = HTTPVersion.HTTP09;
                else
                {
                    if (requestLines[2] == "HTTP/1.0")
                        httpVersion = HTTPVersion.HTTP10;
                    
                    else if (requestLines[2] == "HTTP/1.1") 
                        httpVersion = HTTPVersion.HTTP11; 

                    else 
                        return false; 
                }
                relativeURI = requestLines[1];
                if (ValidateIsURI(relativeURI) == false)
                    return false;

                if (requestLines[0] == "GET")
                    method = RequestMethod.GET;

                else if (requestLines[0] == "POST")
                    method = RequestMethod.POST;

                else if (requestLines[0] == "HEAD")
                    this.method = RequestMethod.HEAD;
                else
                    return false;

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            try
            {
                int i = 1;
                while (i != contentLines.Length && contentLines[i] != "")
                {
                    string[] arr = contentLines[i].Split(':');
                    headerLines.Add(arr[0], arr[1]);
                    ++i;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

            private bool ValidateBlankLine()
        {
            bool flag = false;
            for (int i = 0; i < contentLines.Length; ++i)
                if (contentLines[i] == "")
                    flag = true;
            return flag;
        }

    }
}
