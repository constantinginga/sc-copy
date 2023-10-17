using System;
using System.Data;

namespace StartupCentral.Extensions
{
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Checks if a column's value is DBNull
        /// </summary>
        /// <param name="dataReader">The data reader</param>
        /// <param name="columnName">The column name</param>
        /// <returns>A bool indicating if the column's value is DBNull</returns>
        public static bool IsDBNull(this IDataReader dataReader, string columnName)
        {
            return dataReader[columnName] == DBNull.Value;
        }

        /// <summary>
        /// Checks if a column exists in a data reader
        /// </summary>
        /// <param name="dataReader">The data reader</param>
        /// <param name="columnName">The column name</param>
        /// <returns>A bool indicating the column exists</returns>
        public static bool HasColumn(this IDataReader dataReader, string columnName)
        {
            return dataReader.GetSchemaTable().Select($"ColumnName='{columnName}'").Length > 0;

            //try
            //{
            //    return dataReader.GetOrdinal(columnName) >= 0;
            //}
            //catch (IndexOutOfRangeException)
            //{
            //    return false;
            //}
        }
    }
}