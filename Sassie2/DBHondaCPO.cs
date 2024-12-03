using Microsoft.ApplicationBlocks.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Sassie2
{
    // ICE Documents Data
    public enum DocumentType
    {
        VrmCampaign = 1,//1
        VehicleHistoryReport = 2,
        VrmRepairOrder = 4,//4
        PointInspectionChecklist = 8,//8
    }
    /// <summary>
    /// Honda CPO specific DB activities
    /// </summary>
    public class DBHondaCPO
    {
        public string Connstr = "";  
                
        /// <summary>
        /// constructor
        /// </summary>
        public DBHondaCPO()
        {
            Utilities utility = new Utilities();
            try
            {
                Connstr = utility.GetConnectionString("ApplicationServices");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
        public DataSet GetHondaCPOOCR(int assignment_id, string currentLanguage)
        {
            Utilities utility = new Utilities();
            string WebAppsConnection = utility.GetConnectionString("VEHICLE");
            utility = null;

            DataSet dsResult = new DataSet();
            SqlParameter[] objParameter = new SqlParameter[3];

            try
            {
                objParameter[0] = new SqlParameter("@Assignment_ID", assignment_id);
                objParameter[1] = new SqlParameter("@Language_Code", currentLanguage);
                objParameter[2] = new SqlParameter("@Version_Request", 1);

                dsResult = SqlHelper.ExecuteDataset(WebAppsConnection, CommandType.StoredProcedure, "usp_UDA_OnlineConsultationReport_HondaCPO_Sassie", objParameter);
            }
            catch (Exception ex)
            {
                Utilities oUtility = new Utilities();
                oUtility.SaveSqlCall(SeverityType.RuntimeError, "CPO", "DBHondaCPO.cs", "usp_UDA_OnlineConsultationReport_HondaCPO", objParameter);
                oUtility = null;

                throw new Exception(ex.ToString());
            }
            finally
            {
                objParameter = null;
            }

            return dsResult;
        }

    }
  
}