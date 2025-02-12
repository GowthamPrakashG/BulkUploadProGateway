﻿using ClientSchemaHub.Models.DTO;

namespace ClientSchemaHub.Service.IService
{
    public interface IPostgreSQLService
    {
        public Task<Dictionary<string, List<TableDetailsDTO>>> GetTableDetailsForAllTablesAsync(DBConnectionDTO dBConnection);
        public Task<List<string>> GetTableNamesAsync(DBConnectionDTO dBConnection);

        public Task<TableDetailsDTO> GetTableDetailsAsync(DBConnectionDTO dBConnection, string tableName);

        public Task<List<object>> GetPrimaryColumnDataAsync(DBConnectionDTO dBConnection, string tableName);

        public Task<bool> ConvertAndCallCreateTableModel(DBConnectionDTO connectionDTO, string createQuery);
        public Task<bool> Insertdata(DBConnectionDTO connectionDTO, List<Dictionary<string, string>>? convertedDataList, List<ColumnMetaDataDTO>? booleancolumns, string tablename);
        public Task<bool> IsTableExists(DBConnectionDTO dBConnection, string tableName);
        public Task<List<dynamic>> GetTabledata(DBConnectionDTO dBConnection, string tableName);

        public Task<string> ReceiveHashFromPort(DBConnectionDTO connectionDTO);
        public Task<string> PortCommunication(DBConnectionDTO connectionDTO);


    }
}
