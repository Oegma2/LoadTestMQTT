/*
  Copyright (C) 1989-2022 Free Software Foundation, Inc.
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <https://www.gnu.org/licenses/>.

  MQGateway C# application
  
  Aim is to use as simulated MQTT application that spawns 1..xxx clients for small simulations
  A typical use-case is to start 1000 connection to MQTT broker and see if it holds

  >>> IMPORTANT: This is not production code. It is intended to be a simple example for learning! <<<

  modified 26 Aug 2022 
  by Izak Schalk Smit (oegma2) 
*/
using System;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;

namespace MQGateway
{
    class Program
    {
        public static async void Go()
        {
            try
            {
                var mqttFactory = new MqttFactory();

                using (var mqttClient = mqttFactory.CreateMqttClient())
                {
                    var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

                    await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                    // This will throw an exception if the server does not reply.
                    await mqttClient.PingAsync(CancellationToken.None);
                    var applicationMessage = new MqttApplicationMessageBuilder()
                      .WithTopic("samples/temperature/living_room")
                      .WithPayload("19.5")
                      .Build();

                    while (true)
                    {  
                        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

                        Console.WriteLine(DateTime.Now + ": Published to samples/temperature/living_room");
                        Thread.Sleep(1000);

                        // This will throw an exception if the server does not reply.
                        await mqttClient.PingAsync(CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < 500; i++)
            {
                Thread t = new Thread(new ThreadStart(Go));
                t.Start();
                Console.WriteLine($"Started {i}");
            }
            System.Console.ReadLine();
        }
    }
}
