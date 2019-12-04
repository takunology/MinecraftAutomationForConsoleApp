using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    public class ExcelRead
    {
        static int SHEET = 5;
        static int ROW = 10;
        static int COL = 10;
        public string[,,] Value = new string[ROW, SHEET, COL];

        public void ExcelOpen()
        {
            //string Path = @$"{Directory.GetCurrentDirectory()}\Excel\House.xlsx";
            string Path = "../../../Excel/House.xlsx";
            var Book = WorkbookFactory.Create(Path); //参照するブックのパス
            //マイクラの座標に合わせるために x, y, z を使う                                          
            for (int y = 0; y < SHEET; y++)
            {
                var Sheet = Book.GetSheetAt(y); //N枚目のシートを参照
                for (int x = 0; x < ROW; x++)
                {
                    for (int z = 0; z < COL; z++)
                    {
                        Value[x, y, z] = GetValue(x, Sheet, z); //読み込んだ値を保持
                    }
                }
            }
        }

        //Excelシート上の文字を読み込んでいく
        public string GetValue(int Row, ISheet Sheet, int Column)
        {
            var row = Sheet.GetRow(Row) ?? Sheet.CreateRow(Row); //例外対策(なければ空のシートを追加)
            var cell = row.GetCell(Column) ?? row.CreateCell(Column);
            string value;

            value = cell.NumericCellValue.ToString();
            return value;
        }
    }
}