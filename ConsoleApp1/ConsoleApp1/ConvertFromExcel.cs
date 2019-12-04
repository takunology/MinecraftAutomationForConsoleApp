using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class ConvertFromExcel : ExcelRead
    {
        public void Convert()
        {
            for (int y = 0; y < Value.GetLength(1); y++)
            {
                for (int x = 0; x < Value.GetLength(0); x++)
                {
                    for (int z = 0; z < Value.GetLength(2); z++)
                    {
                        string value = Value[x, y, z];
                        switch (value)
                        {
                            case "0":
                                Value[x, y, z] = "0"; //空気
                                break;

                            case "1":
                                Value[x, y, z] = "1"; //石
                                break;

                            case "2":
                                Value[x, y, z] = "20"; //ガラス
                                break;

                            case "3":
                                Value[x, y, z] = "5 0"; //木材(オーク)
                                break;

                            case "4":
                                Value[x, y, z] = "17"; //原木
                                break;

                            case "5":
                                Value[x, y, z] = "24"; //砂岩
                                break;

                            case "6":
                                Value[x, y, z] = "45"; //レンガ
                                break;
                        }
                    }
                }
            }
        }

        //読み込めたかを確認する
        public void ShowSheet()
        {
            for (int y = 0; y < Value.GetLength(1); y++)
            {
                Console.WriteLine($"{y + 1}枚目のシート");
                for (int x = 0; x < Value.GetLength(0); x++)
                {
                    for (int z = 0; z < Value.GetLength(2); z++)
                    {
                        Console.Write(string.Format("{0, 3}", ($"{Value[x, y, z]}")));
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}