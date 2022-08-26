using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;

namespace LoadTestMQTT_Console
{
    public class Program
    {
        private static MqttClient _mqttClient = null;
        private static string _MqttDns = "localhost";
        private static int _MqttPort = 1883;
        private static string _mqttClientId = string.Empty;
        private static AutoResetEvent _resetMqttSetup = new AutoResetEvent(true);

        static void Main(string[] args)
        {
            _resetMqttSetup.Reset();

            while (true)
            {
                if (_mqttClientId == string.Empty)
                {
                    _mqttClientId = Guid.NewGuid().ToString();
                }

                if (_mqttClient == null)
                {
                    _mqttClient = new MqttClient(_MqttDns, _MqttPort, false, MqttSslProtocols.None, null, null);
                }

                if (!_mqttClient.IsConnected)
                {
                    _mqttClient.Connect(_mqttClientId);

                    _mqttClient.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;
                    _mqttClient.MqttMsgPublished += _mqttClient_MqttMsgPublished;
                    _mqttClient.MqttMsgSubscribed += _mqttClient_MqttMsgSubscribed;
                    _mqttClient.MqttMsgUnsubscribed += _mqttClient_MqttMsgUnsubscribed;

                    string[] topics = { "command/start", "command/stop" };
                    byte[] qos = { 2, 2 };
                    _mqttClient.Subscribe(topics, qos);
                }

                if (_mqttClient.IsConnected)
                {
                    _resetMqttSetup.WaitOne();
                }
                else
                {
                    //todo: need to impl a backoff system - at this stage we sleep and retry every 30sec
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            }
        }

        static void _mqttClient_MqttMsgUnsubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgUnsubscribedEventArgs e)
        {
            Console.WriteLine("_mqttClient_MqttMsgUnsubscribed");
        }

        static void _mqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("_mqttClient_MqttMsgPublishReceived");
        }

        static void _mqttClient_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("_mqttClient_MqttMsgSubscribed");
        }

        static void _mqttClient_MqttMsgPublished(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine("_mqttClient_MqttMsgPublished");
        }
    }
}
