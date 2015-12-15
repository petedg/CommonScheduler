using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class DictionaryValue
    {
        public string GetValue (Dictionary dictionary, int dictionaryValueId)
        {
            using (var context = new serverDBEntities())
            {
                var dictionaryValues = from dictionaryValue in context.DictionaryValue
                                       where dictionaryValue.DICTIONARY_ID == dictionary.ID && dictionaryValue.DV_ID == dictionaryValueId
                                       select dictionaryValue;

                var selectedDictionaryValue = dictionaryValues.FirstOrDefault();

                return selectedDictionaryValue.VALUE;
            }
        }
    }
}
