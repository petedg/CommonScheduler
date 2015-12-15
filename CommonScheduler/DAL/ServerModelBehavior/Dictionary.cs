using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Dictionary
    {
        public string GetDictionaryValue(string dictionaryName, int dictionaryValueId)
        {
            setDictionaryByName(dictionaryName);
            return new DictionaryValue().GetValue(this, dictionaryValueId);            
        }

        public bool setDictionaryByName(string dictionaryName)
        {
            using (var context = new serverDBEntities())
            {
                var dictionaries = from dictionary in context.Dictionary
                                   where dictionary.NAME == dictionaryName
                                   select dictionary;

                var selectedDictionary = dictionaries.FirstOrDefault();

                this.ID = selectedDictionary.ID;
                this.DATE_CREATED = selectedDictionary.DATE_CREATED;
                this.DATE_MODIFIED = selectedDictionary.DATE_MODIFIED;
                this.ID_CREATED = selectedDictionary.ID_CREATED;
                this.NAME = selectedDictionary.NAME;                        

                return true;
            }
        }
    }
}
