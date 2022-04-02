// See https://aka.ms/new-console-template for more information
using System.Net;

using Intersect.Framework.Networking.UdpSample;

Console.WriteLine("Hello, World!");

var listenEndPoint = new IPEndPoint(IPAddress.Any, 35000);

var serverEndPoint = new IPEndPoint(IPAddress.Loopback, 35000);

var server = new Server(listenEndPoint).Listen();

var client = new Client(serverEndPoint).Connect();

client.Send(null, "test");
