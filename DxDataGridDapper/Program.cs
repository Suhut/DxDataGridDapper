using DxDataGridDapper.DevExtreme;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace DxDataGridDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var query1 = new DevExtremeListQuery();
            query1.Filter = new List<dynamic>();
            query1.Filter.Add("ColumnName");
            query1.Filter.Add("contains");
            query1.Filter.Add("Value"); 
            var sqlWhere1 = (new DevExtremeHelper()).GetFilterSqlExprByArray(query1.Filter);
            Console.WriteLine("result sqlWhere1 : " + sqlWhere1);

            var query2 = new DevExtremeListQuery();
            query2.Filter = new List<dynamic>(); 
            query2.Filter.Add(JsonExtensions.JsonElementFromObject(new string[] { "ColumnName1", " = ", "Value1" }));
            query2.Filter.Add("and");
            query2.Filter.Add(JsonExtensions.JsonElementFromObject(new string[] { "ColumnName2", " = ", "Value2" })); 
            var sqlWhere2 = (new DevExtremeHelper()).GetFilterSqlExprByArray(query1.Filter);
            Console.WriteLine("result sqlWhere2 : " + sqlWhere2);

            Console.WriteLine("Hello World!");
        }
    }

    public static partial class JsonExtensions
    {
        public static JsonDocument JsonDocumentFromObject<TValue>(TValue value, JsonSerializerOptions options = default)
            => JsonDocumentFromObject(value, typeof(TValue), options);

        public static JsonDocument JsonDocumentFromObject(object value, Type type, JsonSerializerOptions options = default)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, options);
            return JsonDocument.Parse(bytes);
        }

        public static JsonElement JsonElementFromObject<TValue>(TValue value, JsonSerializerOptions options = default)
            => JsonElementFromObject(value, typeof(TValue), options);

        public static JsonElement JsonElementFromObject(object value, Type type, JsonSerializerOptions options = default)
        {
            using var doc = JsonDocumentFromObject(value, type, options);
            return doc.RootElement.Clone();
        }
    }

}
