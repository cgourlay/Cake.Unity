using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Cake.Mix
{
    internal sealed class SqlStatementList : IEnumerable
    {
        private readonly StringCollection _statements;
        private readonly string _delimiter;
        
        internal SqlStatementList(string delimiter)
        {
            _statements = new StringCollection();
            _delimiter = delimiter;
        }

        private void ParseSql(string sql)
        {
            var reader = new StringReader(sql);
            string line;
            var sqlStatement = new StringBuilder();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Length == 0) { continue; }
                if (line.Trim().StartsWith("//") || line.Trim().StartsWith("--")) { continue; }

                if (!Regex.IsMatch(line.Trim(), _delimiter))
                {
                    sqlStatement.Append(line.Trim() + Environment.NewLine);
                }
                else
                {
                    if (line.Trim().Length <= 0) { continue; }

                    var tokens = Regex.Split(line.Trim(), _delimiter);
                    for (var i = 0; i < tokens.Length; i++)
                    {
                        var token = tokens[i];
                        if (i == 0)
                        {
                            if (sqlStatement.Length > 0)
                            {
                                sqlStatement.Append(token);
                                _statements.Add(sqlStatement.ToString());
                                sqlStatement = new StringBuilder();
                            }
                            else
                            {
                                sqlStatement = new StringBuilder();
                                if (token.Trim().Length > 0)
                                {
                                    sqlStatement.Append(token + Environment.NewLine);
                                }
                            }
                        }
                        else
                        {
                            if (sqlStatement.Length > 0)
                            {
                                _statements.Add(sqlStatement.ToString());
                                sqlStatement = new StringBuilder();
                            }

                            if (token.Trim().Length > 0)
                            {
                                sqlStatement.Append(token + Environment.NewLine);
                            }
                        }
                    }
                }
            }

            if (sqlStatement.Length > 0)
            {
                _statements.Add(sqlStatement.ToString());
            }
        }

        internal void ParseSqlFromFile(string file)
        {
            using (var sr = new StreamReader(File.OpenRead(file)))
            {
                var statements = sr.ReadToEnd();
                ParseSql(statements);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_statements).GetEnumerator();
        }
    }
}
