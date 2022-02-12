using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            this.code = code;

            string content_type = "Content-Type: " + contentType + "\r\n";
            string content_length = "Content-Length: " + content.Length + "\r\n";
            string date = "Date: " + DateTime.Now + "\r\n";

            headerLines.Add(content_type);
            headerLines.Add(content_length);
            headerLines.Add(date);

            string status_line = GetStatusLine(code);

            // TODO: Create the request string
            if (code == StatusCode.Redirect)
            {
                string location = "Location: " + redirectoinPath + "\r\n";
                headerLines.Add(location);
                responseString = status_line + headerLines[0] + headerLines[1] + headerLines[2] + headerLines[3] + "\r\n" + content;
            }
            else
            {
                responseString = status_line + headerLines[0] + headerLines[1] + headerLines[2] + "\r\n" + content;
            }



        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = "HTTP/1.1 " + ((int)code).ToString() + code.ToString() + "\r\n";
            return statusLine;
        }
    }
}