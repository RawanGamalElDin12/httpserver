using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket

            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);

        }

        public void StartServer()
        {

            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100000);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                //Start the thread
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            int receivedLength;
            byte[] request = new byte[1024];
            while (true)
            {
                try
                {
                    // TODO: Receive request
                   
                     receivedLength = clientSocket.Receive(request);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                       
                        break;
                    }

                    string receivedreq = Encoding.ASCII.GetString(request,0,receivedLength);

                    // TODO: Create a Request object using received request string

                    Request req = new Request(receivedreq);

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = this.HandleRequest(req);
                    
                    // TODO: Send Response back to client
                    byte[] resp = Encoding.ASCII.GetBytes(response.ResponseString);

                    clientSocket.Send(resp);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            
            string content;
            StatusCode code;
            
            try
            {
                
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    code = StatusCode.BadRequest;
                    Response resp1 = new Response(code, "text/html", content, "");
                    return resp1;
                }
                
                
                string physicalPath = Configuration.RootPath + "\\" + request.relativeURI.Substring(1, request.relativeURI.Length - 1);
                
                string filename = request.relativeURI.Substring(1, request.relativeURI.Length - 1);
                string redirection = this.GetRedirectionPagePathIFExist(filename);

                //check for redirect
                if (redirection != "")
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    code = StatusCode.Redirect;
                    
                }
                
                else
                {
                    //TODO: check file exists
                   
                    
                    content = LoadDefaultPage(filename);
                    // physical file not found
                    if (content == "")
                    {
                        code = StatusCode.NotFound;
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    }
                    // Create OK response
                    else
                    {
                        code = StatusCode.OK;
                    }

                }
                
                Response resp = new Response(code, "text/html", content, redirection);
                return resp;
                
                
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                Response resp = new Response(StatusCode.InternalServerError, "text/html", content, null);
                return resp;
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
            
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            string content = "";
           
                if (!File.Exists(filePath))
                {
                Logger.LogException(new Exception(defaultPageName + " Page not Exist"));
                return string.Empty;


                }
            content = File.ReadAllText(filePath);
            return content;


            // else read file and return its content

        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary
                StreamReader reader = new StreamReader(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] rule = line.Split(',');
                    Configuration.RedirectionRules.Add(rule[0], rule[1]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
