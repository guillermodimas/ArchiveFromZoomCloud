using System.Collections.Generic;

namespace ArchiveFromZoomCloud.DataAccess
{
    public interface ISQLDataAccess
    {
        void CommitTransaction();
        void Dispose();
        string GetConnectionString(string name);
        List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        List<T> LoadDataBySQLTEXT<T, U>(string sqlText, string connectionStringName);
        List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters);
        void RollbackTransaction();
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
        void SaveDataInTransaction<T>(string storedProcedure, T parameters);
        void StartTransaction(string connectionStringName);
    }
}