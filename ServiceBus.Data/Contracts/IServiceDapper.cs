using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.Contracts
{
    public interface IServiceDapper
    {
        IEnumerable<T> GetAllRecords<T>();
        
        IEnumerable<T> GetAllRecordsByCount<T>(int count);       
              
        IEnumerable<T> GetAllRecordsByKeys<T>(string Status);

        IEnumerable<T> GetAllGenericEnitities<T>();

        IEnumerable<T> GetAllRecordsByKey<T>(string key, string value);

      //  int UpdateGenericEntityBystatus<T>(string status, long ID);

        //bool LifeHistoryLog(List<Life_History> life_Histories);

        //bool LifeSummaryLog(List<Life_Summary> life_Summaries);

        //bool LifeUpdateAccountNo(Life_Customer life_Customer);

        //bool MutualFundSummaryLog(List<MutualFund_Summary> mutualFund_Summaries);

        //bool MutualFundHistoryLog(List<MutualFund_History> mutualFund_Summaries);

        //bool SecuritiesSummaryLog(List<Securities_Summary> mutualFund_Summaries);

        //bool SecuritiesHistoryLog(List<Securities_History> mutualFund_Summaries);
    }
}
