using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace WpfQSearchExcel
{
    public class ExcelTools
    {

        public ExcelTools()
        {

        }

        //将Excel数据导入到DataSet中
        public static DataSet ExcelToDS(string Path, string tableName)
        {
            DataSet ds = null;
            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                string strExcel = "";
                OleDbDataAdapter myCommand = null;

                strExcel = string.Format("select * from [{0}$]", tableName);
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                ds = new DataSet();
                myCommand.Fill(ds, "table1");

                conn.Close();

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return ds;
        }

        //获取Excel的工作表
        public static String[] GetExcelSheetNames(string Path)
        {
            DataTable dt = null;
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt == null)
                return null;
            String[] excelSheets = new String[dt.Rows.Count];
            int i = 0;

            // Add the sheet name to the string array.
            foreach (DataRow row in dt.Rows)
            {
                string strSheetTableName = row["TABLE_NAME"].ToString();
                excelSheets[i] = strSheetTableName.Substring(0, strSheetTableName.Length - 1);
                i++;
            }

            return excelSheets;
        }
    }
}
