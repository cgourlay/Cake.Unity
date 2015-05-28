namespace Cake.Mix
{
    public sealed class SqlSettings : ISqlSettings
    {
        public SqlSettings(string connectionString, string sqlScript)
        {
            ConnectionString = connectionString;
            Delimiter = "GO";
            IsTransaction = true;
            SqlScript = sqlScript;
        }

        public string ConnectionString { get; private set; }
        public string Delimiter { get; set; }
        public bool IsTransaction { get; set; }
        public string SqlScript{ get; private set; }
    }
}