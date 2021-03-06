using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using System.Security.Permissions;

namespace HerdrServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteServer();
        }
        
        //private static SqlConnection connection;
        private static string server;
        private static string database;
        //private static string Library;
        private static string uid;
        private static string password;
        private static string connectionString;

        static void initializeDatabase(){
            /*
            server = "herdrtestdb.c42aqcn0bv1v.us-east-2.rds.amazonaws.com,3306";
            database = "herdr";
            uid = "herdru1";
            password = "ST3V3nordstrom<3";
            connectionString = "Server=" + server + ";" + "Database=" + 
            database + ";" + "User Id=" + uid + ";" + "Password=" + password + ";";*/

            server = "tcp:3.23.163.170";
            //server = "tcp:127.0.0.1";
            database = "herdr";

            //STANDARD USER
                //uid = "herdru1";
                //password = "ST3V3nordstrom<3";
            //ADMIN USER
                // uid = "admin";
                // password = "ctOqE9NPuC1WtJWXooSD";
            //NEW USER
                uid = "ec2server";
                password = "HavanaBanana!123";
            //NEWER USER
                //uid = "dbadmin";
                //password = "SeniorProject21";
            //LOCAL USER
                //uid = "user1";
                //password = "password";

            connectionString =
                "server=" + server + ";" +
                "port=33060;" +
                //"Network=" + Library + ";" + 
                "database=" + database + ";" +
                "user=" + uid + ";" +
                "password=" + password + ";";

            //Console.WriteLine("Connection String: {0}", connectionString);
        }

        public static void ExecuteServer(){
            // Establish the local endpoint for the socket. Dns.GetHostName 
            // returns the name of the host running the application. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try{

                // Using Bind() method we associate a network address to the Server Socket 
                // All client that will connect to this Server Socket must know this network Address 
                listener.Bind(localEndPoint);

                // Using Listen() method we create the Client list that will want to connect to Server 
                listener.Listen(10);

                while (true){ //LOOP AWAITING CONNECTIONS{
                    Console.WriteLine("Waiting connection ... ");

                    // Suspend while waiting for incoming connection Using 
                    // Accept() method the server will accept connection of client 
                    Socket clientSocket = listener.Accept();
                    Console.WriteLine("Accepted connection ... ");
                    // Data buffer 
                    byte[] bytes = new Byte[16];
                    string data = null;

                    while (true){ //READ DATA IN UNTIL "<EOF>"
                        int numByte = clientSocket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, numByte);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received -> {0} ", data);

                    //SPLIT DATA BY ";"
                    string[] messageIn = data.Split(';');

                    initializeDatabase();
                    Console.WriteLine("INITIALIZED");
                    //connectionString = "server=127.0.0.1;port=3306;user=user1;database=helloworld;password=password";
                    //PROCESS DATA INTO QUERIES
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try{
                            Console.WriteLine("State: {0}", connection.State);
                            connection.Open();
                            Console.WriteLine("State: {0}", connection.State);
                            Console.WriteLine("OPENED");
                        }
                        catch (Exception e){
                            Console.WriteLine("OPEN CONNECTION FAILED: {0}", e.Message);
                        }
                        switch (messageIn[0])
                        {
                            case "1":
                                Console.WriteLine("CASE 1");

                                // This code uses an SqlCommand based on the SqlConnection.
                                using (MySqlCommand command = new MySqlCommand("DESCRIBE hall;", connection))
                                try{
                                    using (MySqlDataReader reader = command.ExecuteReader()){
                                        while (reader.Read()){
                                            Console.WriteLine("{0} {1}", reader.GetInt32(0), reader.GetString(1));
                                        }
                                    }
                                }
                                catch (Exception e){
                                    Console.WriteLine("EXECUTE READER FAILED: {0}", e.Message);
                                }

                                break;

                            case "Log In Request":

                                break;

                            case "Get LocalUser":

                                break;

                            case "Request more Profiles":

                                break;

                            case "Confirm Match":

                                break;

                            case "Block User 2 for user 1.":

                                break;

                            case "Report User for: ":

                                break;

                            case "Refresh Notifications":

                                break;

                            case "Create Profile":

                                break;

                            case "Delete Profile":

                                break;

                            case "Send Contact Request":

                                break;

                            case "Update Profile":

                                break;

                            case "Search For User":

                                break;

                            default:
                                Console.WriteLine("DEFAULT CASE\n");
                                break;
                        }
                        connection.Close();
                    }

                    byte[] message = Encoding.ASCII.GetBytes(data);

                    // Send a message to Client using Send() method 
                    clientSocket.Send(message);

                    // Close client Socket using the Close() method. After closing, 
                    // we can use the closed Socket for a new Client Connection 
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    //connection.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
