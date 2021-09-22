using System;
using System.Configuration;

/// <summary>
/// Summary description for DataAccessHelper
/// </summary>
namespace ChoCastle.Models
{
    public class DataAccessFactory
    {

        public static SQLDataAccessProvider CreateDefaultDataAccess()
        {
            Type dataAccessType = Type.GetType("ChoCastle.Models.SQLDataAccessProvider");
            SQLDataAccessProvider dc = (SQLDataAccessProvider)Activator.CreateInstance(dataAccessType);
            return (dc);

        }
    }
}

