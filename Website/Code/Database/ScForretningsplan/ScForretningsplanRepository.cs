using System.Collections.Generic;
using System.Data.SqlClient;

namespace StartupCentral.Code.Database
{
    public static class ScForretningsplanRepository
    {
        #region SQL Table Documentation

        // Available fields in SQL table:
        // Name: Id | DataType: int
        // Name: UniqueId | DataType: uniqueidentifier
        // Name: DocumentId | DataType: int
        // Name: MemberId | DataType: int
        // Name: Seen | DataType: bit
        // Name: Downloaded | DataType: bit
        // Name: CreateDate | DataType: datetime
        // Name: UpdateDate | DataType: datetime
        // SELECT [Id],[UniqueId],[DocumentId],[MemberId],[Seen],[Downloaded],[CreateDate],[UpdateDate]

        #endregion SQL Table Documentation

        public static string DatabaseName;
        public static string TableName = @"scForretningsplan";
        public static string BaseSQL;

        static ScForretningsplanRepository()
        {
            var csb = new SqlConnectionStringBuilder(System.Configuration.ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString);
            DatabaseName = csb.InitialCatalog;
            BaseSQL = $@"
			SELECT *
                FROM [{DatabaseName}].[dbo].[{TableName}]
		    ";
        }

        public static List<ScForretningsplan> GetAll()
        {
            List<ScForretningsplan> resultList = null;

            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
			";

            SqlDataReader dr = Database.QueryReader(com);
            if (dr.HasRows)
            {
                resultList = new List<ScForretningsplan>();

                while (dr.Read())
                {
                    resultList.Add(new ScForretningsplan().Apply(dr));
                }
            }

            com.Connection.Close();
            com.Connection.Dispose();
            com.Dispose();

            return resultList;
        }

        public static List<ScForretningsplan> GetListById(int Id)
        {
            List<ScForretningsplan> resultList = null;

            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
				WHERE Id = @Id
			";
            com.Parameters.AddWithValue("@Id", Id);

            SqlDataReader dr = Database.QueryReader(com);
            if (dr.HasRows)
            {
                resultList = new List<ScForretningsplan>();

                while (dr.Read())
                {
                    resultList.Add(new ScForretningsplan().Apply(dr));
                }
            }

            com.Connection.Close();
            com.Connection.Dispose();
            com.Dispose();

            return resultList;
        }

        public static ScForretningsplan GetById(int Id)
        {
            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
				WHERE Id = @Id
			";
            com.Parameters.AddWithValue("@Id", Id);

            try
            {
                SqlDataReader dr = Database.QueryReader(com);
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        return new ScForretningsplan().Apply(dr);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                com.Connection.Close();
                com.Connection.Dispose();
                com.Dispose();
            }
            return null;
        }

        public static List<ScForretningsplan> GetListByDocumentId(int DocumentId)
        {
            List<ScForretningsplan> resultList = null;

            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
				WHERE DocumentId = @DocumentId
			";
            com.Parameters.AddWithValue("@DocumentId", DocumentId);

            SqlDataReader dr = Database.QueryReader(com);
            if (dr.HasRows)
            {
                resultList = new List<ScForretningsplan>();

                while (dr.Read())
                {
                    resultList.Add(new ScForretningsplan().Apply(dr));
                }
            }

            com.Connection.Close();
            com.Connection.Dispose();
            com.Dispose();

            return resultList;
        }

        public static ScForretningsplan GetByDocumentIdAndMemberId(int DocumentId, int memberId)
        {
            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
				WHERE DocumentId = @DocumentId
                    AND MemberId = @MemberId
			";
            com.Parameters.AddWithValue("@DocumentId", DocumentId);
            com.Parameters.AddWithValue("@MemberId", memberId);

            try
            {
                SqlDataReader dr = Database.QueryReader(com);
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        return new ScForretningsplan().Apply(dr);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                com.Connection.Close();
                com.Connection.Dispose();
                com.Dispose();
            }
            return null;
        }

        public static List<ScForretningsplan> GetListByMemberId(int MemberId)
        {
            List<ScForretningsplan> resultList = null;

            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
				WHERE MemberId = @MemberId
			";
            com.Parameters.AddWithValue("@MemberId", MemberId);

            SqlDataReader dr = Database.QueryReader(com);
            if (dr.HasRows)
            {
                resultList = new List<ScForretningsplan>();

                while (dr.Read())
                {
                    resultList.Add(new ScForretningsplan().Apply(dr));
                }
            }

            com.Connection.Close();
            com.Connection.Dispose();
            com.Dispose();

            return resultList;
        }

        public static ScForretningsplan GetByMemberId(int MemberId)
        {
            SqlCommand com = new SqlCommand();
            com.CommandText = $@"
				{BaseSQL}
				WHERE MemberId = @MemberId
			";
            com.Parameters.AddWithValue("@MemberId", MemberId);

            try
            {
                SqlDataReader dr = Database.QueryReader(com);
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        return new ScForretningsplan().Apply(dr);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                com.Connection.Close();
                com.Connection.Dispose();
                com.Dispose();
            }
            return null;
        }

        public static ScForretningsplan Save(ScForretningsplan item)
        {
            string SQL = string.Empty;
            SqlCommand com = new SqlCommand();

            if (item.Id > 0)
            {
                SQL = $@"
                    UPDATE [{DatabaseName}].[dbo].[{TableName}]
                       SET
							[DocumentId] = @DocumentId,
							[MemberId] = @MemberId,
							[Seen] = @Seen,
							[Downloaded] = @Downloaded,
							[UpdateDate] = @UpdateDate
						WHERE [Id] = @Id

                ";

                com.Parameters.AddWithValue("@Id", item.Id);
                com.Parameters.AddWithValue("@UpdateDate", item.UpdateDate);
            }
            else
            {
                SQL = $@"
                    INSERT INTO [{DatabaseName}].[dbo].[{TableName}]
                               (
								[DocumentId],
								[MemberId],
								[Seen],
								[Downloaded]
								)
                         VALUES
                               (
								@DocumentId,
								@MemberId,
								@Seen,
								@Downloaded
                               );
                    SELECT @@IDENTITY;
                ";
            }

            com.Parameters.AddWithValue("@DocumentId", item.DocumentId);
            com.Parameters.AddWithValue("@MemberId", item.MemberId);
            com.Parameters.AddWithValue("@Seen", item.Seen);
            com.Parameters.AddWithValue("@Downloaded", item.Downloaded);

            com.CommandText = SQL;

            try
            {
                if (item.Id > 0)
                {
                    // Update
                    Database.QueryScalarInt(com);
                }
                else
                {
                    // Insert
                    item.Id = Database.QueryScalarInt(com);
                }

                // Returns it.
                return item;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                com.Connection.Close();
                com.Connection.Dispose();
                com.Dispose();
            }
        }

        public static bool Delete(ScForretningsplan item)
        {
            if (item == null) return false;

            SqlCommand com = new SqlCommand();

            if (item.Id > 0)
            {
                com.CommandText = $@"
					DELETE FROM [{DatabaseName}].[dbo].[{TableName}]
                     WHERE [Id] = @Id;
				";
                com.Parameters.AddWithValue("@Id", item.Id);
            }

            if (com.Parameters.Count > 0)
            {
                Database.ExecuteNonQuery(com);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}