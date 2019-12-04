using CoreRCON;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static IPAddress serveraddress = IPAddress.Parse("127.0.0.1");
        static ushort port = 25575;
        static string serverpass = "minecraft";
        //RCONインスタンス生成
        static RCON connection = new RCON(serveraddress, port, serverpass);
        //座標
        static double[] PlayerPosition = new double[3];

        static async Task Main(string[] args)
        {
            Console.WriteLine("0:ブロックを積む 1:家を建てる");
            var Select = int.Parse(Console.ReadLine());

            switch (Select)
            {
                case 0:
                    await GetPosition();
                    await PutBlock();
                    break;
                case 1:
                    await GetPosition();
                    await Building();
                    break;
            }

        }

        static async Task GetPosition()
        {
            //コマンドを投げる
            var PlayerName = "Prau_Splacraft";
            var command = $"/tp {PlayerName} ~ ~ ~";
            var result = await connection.SendCommandAsync(command);
            //結果の表示
            Console.WriteLine(result);
            //正規表現による除去
            string TmpChar = Regex.Replace(result, @"[^0-9-,.]", "");
            //カンマごとに座標を区切って保持
            string[] StrArray = TmpChar.Split(',');

            for (int i = 0; i < StrArray.Length; i++)
            {
                PlayerPosition[i] = double.Parse(StrArray[i]);
                //Console.WriteLine(PlayerPosition[i]);
            }
        }

        static async Task PutBlock()
        {
            double X = PlayerPosition[0];
            double Y = PlayerPosition[1];
            double Z = PlayerPosition[2];
            string BlockName = "stone";

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        string Command = $"/setblock {X + j} {Y + i} {Z + k} {BlockName}";
                        var result = await connection.SendCommandAsync(Command);
                        await Task.Delay(10);
                        Console.WriteLine(result);
                    }
                }
            }
        }

        static async Task Building()
        {
            //建築用のインスタンス
            ConvertFromExcel convertFromExcel = new ConvertFromExcel();
            convertFromExcel.ExcelOpen(); //ファイル読み込み
            convertFromExcel.Convert(); //ブロック参照
            convertFromExcel.ShowSheet(); //読み込んだ値の確認

            double X = PlayerPosition[0];
            double Y = PlayerPosition[1];
            double Z = PlayerPosition[2];
            for (int y = 0; y < convertFromExcel.Value.GetLength(1); y++)
            {
                for (int x = 0; x < convertFromExcel.Value.GetLength(0); x++)
                {
                    for (int z = 0; z < convertFromExcel.Value.GetLength(2); z++)
                    {
                        string SetBlock = $"/setblock { X + x } { Y + y } { Z + z } { convertFromExcel.Value[x, y, z] }";
                        var result = await connection.SendCommandAsync(SetBlock);
                        Console.WriteLine(result);
                        await Task.Delay(5);
                    }
                }
            }
        }
    }
}