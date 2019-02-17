using System;
using Bitfinex.Net;
using MtApi5;

namespace FinexTicker
{
    internal class App : IDisposable
    {

        public MtApi5Client Mt { get; } = new MtApi5Client();
        public BitfinexSocketClient Finex { get; } = new BitfinexSocketClient();

        public App() => Mt.ConnectionStateChanged += Mt_ConnectionStateChanged;

        public void Start()
        {
            var subscribeResult = Finex.SubscribeToTickerUpdates("tBTCUSD", data =>
            {
                var mid = (data.Ask + data.Bid) / 2;
                if (Mt.ConnectionState != Mt5ConnectionState.Connected &&
                    Mt.ConnectionState != Mt5ConnectionState.Connecting)
                {
                    Mt.BeginConnect(8228);
                }else if (Mt.ConnectionState == Mt5ConnectionState.Connected)
                {
                    Console.WriteLine($"Move line to ${mid}");
                    MoveHorizontalLine(0, "HLine", (double)mid);
                }
            });
        }

        private void Mt_ConnectionStateChanged(object sender, Mt5ConnectionEventArgs e)
        {
            if (e.Status == Mt5ConnectionState.Connected)
            {
                Console.WriteLine("Create HLine");
                var l = CreateHorizontalLine();
                Console.WriteLine(l);
            }
        }

        private bool CreateHorizontalLine(int chartId = 0, string name = "HLine", int subWindow = 0,
            double price = 0, long color = 16777215, long style = 0, int width = 1, bool back = false, bool selection = true,
            bool hidden = false,
            long zOrder = 0)
        {
            if (Mt.ObjectCreate(chartId, name, ENUM_OBJECT.OBJ_HLINE, subWindow, DateTime.Now, price))
            {
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_COLOR, color);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_STYLE, style);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_WIDTH, width);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_BACK, back ? 1 : 0);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_SELECTABLE, selection ? 1 : 0);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_SELECTED, selection ? 1 : 0);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_HIDDEN, hidden ? 1 : 0);
                Mt.ObjectSetInteger(chartId, name, ENUM_OBJECT_PROPERTY_INTEGER.OBJPROP_ZORDER, zOrder);
                return true;
            }
            else
            {
                Console.WriteLine(Mt.GetLastError());
                return false;
            }
        }

        private bool MoveHorizontalLine(int chartId = 0, string name = "HLine", double price = 0)
        {
            Mt.ResetLastError();
            if (Mt.ObjectMove(chartId, name, 0, DateTime.Now, price))
            {
                return true;
            }
            else
            {
                Console.WriteLine(Mt.GetLastError());
                return false;
            }
        }

        private bool DeleteHorizontalLine(int chartId = 0, string name = "HLine")
        {
            Mt.ResetLastError();
            if(Mt.ObjectDelete(chartId, name))
            {
                return true;
            }
            else
            {
                Console.WriteLine(Mt.GetLastError());
                return false;
            }
        }

        public void Dispose()
        {
            DeleteHorizontalLine();
        }
    }
}
