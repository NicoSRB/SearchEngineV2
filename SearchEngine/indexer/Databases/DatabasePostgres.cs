using indexer.Interfaces;
using Microsoft.Data.Sqlite;
using Npgsql;
using Shared;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indexer.Databases
{
    public class DatabasePostgres : IDatabase
    {
        private readonly NpgsqlConnection _connection;
        public DatabasePostgres()
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Database = "indexerdb",
                Username = "postgres",
                Password = "password"
            };

            _connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
            _connection.Open();

            using var cmd = _connection.CreateCommand();

            Execute("DROP TABLE IF EXISTS Occ");

            Execute("DROP TABLE IF EXISTS document");
            Execute("CREATE TABLE document(id INTEGER PRIMARY KEY, url TEXT, idxTime TEXT, creationTime TEXT)");

            Execute("DROP TABLE IF EXISTS word");
            Execute("CREATE TABLE word(id INTEGER PRIMARY KEY, name VARCHAR(255))");

            Execute("CREATE TABLE Occ(wordId INTEGER, docId INTEGER, "
                  + "FOREIGN KEY (wordId) REFERENCES word(id), "
                  + "FOREIGN KEY (docId) REFERENCES document(id))");
            Execute("CREATE INDEX word_index ON Occ (wordId)");
        }

        private void Execute(string sql)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public void InsertAllWords(Dictionary<string, int> res)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                @"INSERT INTO word(id, name) VALUES(@id,@name)";

                var paramName = command.CreateParameter();
                paramName.ParameterName = "name";
                command.Parameters.Add(paramName);

                var paramId = command.CreateParameter();
                paramId.ParameterName = "id";
                command.Parameters.Add(paramId);

                // Insert all entries in the res

                foreach (var p in res)
                {
                    paramName.Value = p.Key;
                    paramId.Value = p.Value;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public void InsertAllOcc(int docId, ISet<int> wordIds)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var command = _connection.CreateCommand();
                command.CommandText =
                @"INSERT INTO occ(wordId, docId) VALUES(@wordId,@docId)";

                var paramwordId = command.CreateParameter();
                paramwordId.ParameterName = "wordId";

                command.Parameters.Add(paramwordId);

                var paramDocId = command.CreateParameter();
                paramDocId.ParameterName = "docId";
                paramDocId.Value = docId;

                command.Parameters.Add(paramDocId);

                foreach (var p in wordIds)
                {
                    paramwordId.Value = p;

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public void InsertWord(int id, string value)
        {
            using var insertCmd = new NpgsqlCommand("INSERT INTO word(id, name) VALUES(@id, @name)", _connection);

            insertCmd.Parameters.AddWithValue("@id", id);
            insertCmd.Parameters.AddWithValue("@name", value);

            insertCmd.ExecuteNonQuery();
        }

        public void InsertDocument(BEDocument doc)
        {
            using var insertCmd = new NpgsqlCommand(
                "INSERT INTO document(id, url, idxTime, creationTime) VALUES(@id, @url, @idxTime, @creationTime)",
                _connection
            );

            insertCmd.Parameters.AddWithValue("@id", doc.mId);
            insertCmd.Parameters.AddWithValue("@url", doc.mUrl);
            insertCmd.Parameters.AddWithValue("@idxTime", doc.mIdxTime);
            insertCmd.Parameters.AddWithValue("@creationTime", doc.mCreationTime);

            insertCmd.ExecuteNonQuery();
        }

        public Dictionary<string, int> GetAllWords()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();

            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM word";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var w = reader.GetString(1);

                    res.Add(w, id);
                }
            }
            return res;
        }

        public int GetDocumentCounts()
        {
            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = "SELECT count(*) FROM document";    

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    var count = reader.GetInt32(0);
                    return count;
                }
            }
            return -1;
        }
    }

}
