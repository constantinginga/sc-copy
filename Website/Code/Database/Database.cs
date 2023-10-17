using System;
using System.Data.SqlClient;

namespace StartupCentral.Code.Database
{
    public static class Database
    {
        public static string GetConnectionString()
        {
            System.Configuration.Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            System.Configuration.ConnectionStringSettings connString;
            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                connString =
                    rootWebConfig.ConnectionStrings.ConnectionStrings["umbracoDbDSN"];
                if (connString != null)
                {
                    Console.WriteLine("Umbraco connection string = \"{0}\"",
                        connString.ConnectionString);
                    return connString.ConnectionString;
                }
                else
                    Console.WriteLine("No Umbraco connection string");
            }

            return "";
        }

        public static SqlDataReader QueryReader(SqlCommand com)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            SqlConnection con = new SqlConnection(GetConnectionString());
            com.Connection = con;

            try
            {
                con.Open();
                return com.ExecuteReader();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("- Exception: QueryReader -----------------------------------------------------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------");
                throw ex;
            }
            finally
            {
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 60)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL QueryReader execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");
                }
                //LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SQL QueryReader execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        public static int ExecuteNonQuery(SqlCommand com)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            SqlConnection con = new SqlConnection(GetConnectionString());
            com.Connection = con;

            try
            {
                con.Open();
                return com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("- Exception: ExecuteNonQuery -------------------------------------------------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------");
                throw ex;
            }
            finally
            {
                con.Close();

                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 60)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL ExecuteNonQuery execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");
                }
                //LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SQL ExecuteNonQuery execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");

                com.Dispose();
                con.Dispose();
            }
        }

        /// <summary>
        /// Return integer value based on scalar SQL command.
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static int QueryScalarInt(SqlCommand com)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            SqlConnection con = new SqlConnection(GetConnectionString());
            com.Connection = con;

            try
            {
                con.Open();
                return Convert.ToInt32(com.ExecuteScalar());
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("- Exception: QueryScalarInt --------------------------------------------------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------");
                throw ex;
            }
            finally
            {
                con.Close();

                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 60)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL QueryScalarInt execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");
                }
                //LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SQL QueryScalarInt execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");

                com.Dispose();
                con.Dispose();
            }
        }

        /// <summary>
        /// Return integer value based on scalar SQL command.
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static string QueryScalarString(SqlCommand com)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            SqlConnection con = new SqlConnection(GetConnectionString());
            com.Connection = con;

            try
            {
                con.Open();
                return Convert.ToString(com.ExecuteScalar());
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("- Exception: QueryScalarString -----------------------------------------------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine("------------------------------------------------------------------------------------------------------------------------------");
                throw ex;
            }
            finally
            {
                con.Close();

                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 60)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL QueryScalarString execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");
                }
                //LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SQL QueryScalarInt execute: {com.CommandText} took {stopwatch.ElapsedMilliseconds}ms");

                com.Dispose();
                con.Dispose();
            }
        }
    }

    public class DatabaseItem
    {
        /// <summary>
        /// Returns the type name of the object.
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.GetType().Name;
            }
        }
    }
}