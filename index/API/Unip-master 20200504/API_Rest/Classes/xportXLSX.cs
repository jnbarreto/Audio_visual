using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Web;
using System.Reflection;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using API_Rest.Classes;
using API_Rest.Models;
using System.Security.Permissions;

namespace API_Rest.Classes
{
    static class xportXLSX
    {
        public static string convertMatriz(System.Data.DataTable dt, Link link)
        {
            string path = "UNIP_AudioVisual";

            DateTime dataAtual = DateTime.Now;
            var dataAtualSplit = dataAtual.ToString().Split(' ')[0];
            var horaAtualSplit = dataAtual.ToString().Split(' ')[1];

            Random random = new Random();
            int num = random.Next(10000, 999999999);

            path += "_" + dataAtualSplit + "_" + horaAtualSplit + "_" + num + ".xlsx";
            path = Regex.Replace(path, "/", "", RegexOptions.IgnoreCase);
            path = Regex.Replace(path, ":", "", RegexOptions.IgnoreCase);

            object[,] arr = new object[dt.Rows.Count, dt.Columns.Count];

            //for (var i = 0; i < dt.Columns.Count; i++)
            //{
            //    arr[1, i + 1] = dt.Columns[i].ColumnName;
            //}

            //for (int r = 0; r < dt.Rows.Count; r++)
            //{
            //    DataRow dr1 = dt.Rows[r];
            //    for (int c = 0; c < dt.Columns.Count; c++)
            //    {
            //        arr[r + 2, c + 1] = dr1[c];
            //    }
            //}

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                DataRow dr1 = dt.Rows[r];
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    arr[r, c] = dr1[c];
                }
            }

            link.link = xportXLSX.SaveToExcel(arr, path, link);

            return link.link;
        }

        public static void WriteArray<T>(this _Worksheet sheet, int startRow, int startColumn, T[,] array)
        {
            var row = array.GetLength(0);
            var col = array.GetLength(1);
            Range c1 = (Range)sheet.Cells[startRow, startColumn];
            Range c2 = (Range)sheet.Cells[startRow + row - 1, startColumn + col - 1];
            Range range = sheet.Range[c1, c2];
            range.Value = array;
        }

        public static string SaveToExcel<T>(T[,] data, string path, Link link)
        {
            DB Conn = new DB();
            link.link = "";
            try
            {
                Conn.debug("E", "TSTSIX.log", true);
                //Start Excel and get Application object.
                var oXl = new Application { Visible = false };
                Conn.debug("F", "TSTSIX.log", true);
                //Get a new workbook.
                var oWb = (_Workbook)(oXl.Workbooks.Add(""));
                var oSheet = (_Worksheet)oWb.ActiveSheet;
                //oSheet.WriteArray(1, 1, bufferData1);

                oSheet.WriteArray(1, 1, data);
                Conn.debug("G", "TSTSIX.log", true);
                oXl.Visible = false;
                oXl.UserControl = false;
                oWb.SaveAs(path, XlFileFormat.xlWorkbookDefault, Type.Missing,
                    Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                oWb.Close(false);
                oXl.Quit();
                Conn.debug("H", "TSTSIX.log", true);
            }
            catch (Exception e)
            {
                Conn.debug("Falha ao gerar o Arquivo. ERRO: " + e.ToString(), "", true);
                return link.link;
            }
            Conn.debug("I", "TSTSIX.log", true);
            link.link = MoveFile.move(path, link);
            Conn.debug("J", "TSTSIX.log", true);
            return link.link;
        }
    }
}