using System;
using System.Linq;

using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Core.Utilities;

namespace Cake.Mix
{
    [CakeAliasCategory("Sql")]
    public static class SqlExtensions
    {
        [CakeMethodAlias]
        [CakeAliasCategory("Build")]
        public static void RunSql(this ICakeContext context, ISqlSettings settings)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (settings == null) { throw new ArgumentNullException("settings"); }

            var sqlRunner = new SqlRunner(context);
            sqlRunner.Run(settings);
        }
    }

    internal sealed class SqlRunner : Tool<ISqlSettings>
    {
        private readonly IGlobber _globber;
        private readonly DirectoryPath _workingDirectory;

        internal SqlRunner(ICakeContext context) : base(context.FileSystem, context.Environment, context.ProcessRunner)
        {
            _workingDirectory = context.Environment.WorkingDirectory;
            _globber = context.Globber;
        }

        private void ExecuteStatements(SqlHelper sqlHelper, ISqlSettings sqlSettings)
        {
            var list = new SqlStatementList(sqlSettings.Delimiter);
            list.ParseSqlFromFile(GetSqlFilePath(sqlSettings));

            foreach (string statement in list)
            {
                sqlHelper.Execute(statement);
            }
        }

        protected override string GetToolName()
        {
            throw new NotImplementedException();
        }

        protected override FilePath GetDefaultToolPath(ISqlSettings settings)
        {
            throw new NotImplementedException();
        }

        internal void Run(ISqlSettings sqlSettings)
        {
            var sqlHelper = new SqlHelper(sqlSettings.ConnectionString, sqlSettings.IsTransaction);

            try
            {
                ExecuteStatements(sqlHelper, sqlSettings);
            }
            catch (Exception exception)
            {
                sqlHelper.Close(commitTransaction: false);
                throw new CakeException("Error while executing SQL statement.", exception);
            }

            sqlHelper.Close();
        }

        private string GetSqlFilePath(ISqlSettings sqlSettings)
        {
            var expression = string.Format("{0}/**/{1}", _workingDirectory, sqlSettings.SqlScript);
            var filePath = _globber.GetFiles(expression).FirstOrDefault();
            return filePath.FullPath;
        }
    }
}