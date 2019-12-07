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
            Console.WriteLine("0:ブロックを積む 1:家を建てる 2:湧き潰し");
            var Select = int.Parse(Console.ReadLine());

            try
            {
                switch (Select)
                {
                    case 0:
                        await PutBlock();
                        break;
                    case 1:
                        await Building();
                        break;
                    case 2:
                        await PutTorches();
                        break;
                    default:
                        return;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static async Task GetPosition()
        {
            //コマンドを投げる
            var PlayerName = "PlayerName";
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
            }
        }

        static async Task PutBlock()
        {
            await GetPosition();
            //座標をコピー
            double X = PlayerPosition[0];
            double Y = PlayerPosition[1];
            double Z = PlayerPosition[2];
            string BlockName = "stone";
            //1辺5マスのブロック立方体を作成
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
            await GetPosition();
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

        static async Task PutTorches()
        {
            await GetPosition();

            double X = PlayerPosition[0];
            double Y = PlayerPosition[1];
            double Z = PlayerPosition[2];
            //プレイヤーを中心に100マス×100マスの範囲で湧き潰し            
            int RangeX = 100;
            int RangeZ = 100;
            string BlockName = "torch";
            string SearchBlock = "air";

            for(int i = 0; i < RangeX; i++)
            {
                if(i % 7 == 0) //7の倍数ごとに設置
                {
                    for(int j = 0; j < RangeZ; j++)
                    {
                        if(j % 7 == 0)
                        {   //初期値はプレイヤー座標
                            for(int k = (int)Y; k < (255 - (int)Y); k++)
                            {   //ブロックを調べるコマンド
                                string Search = $"/testforblock {X + i} {k} {Z + j} {SearchBlock}";
                                var result = await connection.SendCommandAsync(Search);
                                Console.WriteLine(result);
                                await Task.Delay(5);
                                //文字列検索 空気or草なら松明を置ける
                                if (result.Contains("Successfully") || result.Contains("tallgrass"))
                                {
                                    //草だったら刈る
                                    if (result.Contains("tallgrass"))
                                    {   
                                        string Cut = $"/setblock {X + i} {k} {Z + j} air";
                                        await connection.SendCommandAsync(Cut);
                                    }
                                    //松明を置くコマンド
                                    string PutTorch = $"/setblock {X + i} {k} {Z + j} {BlockName}";
                                    result = await connection.SendCommandAsync(PutTorch);
                                    Console.WriteLine(result);
                                    await Task.Delay(5);
                                    break; //置いたら抜け出す
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}