﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class DictionaryValue
    {
        private serverDBEntities context;

        public DictionaryValue(serverDBEntities context)
        {
            this.context = context;
        }

        public DictionaryValue()
        {
            
        }

        public void SetContext(serverDBEntities context)
        {
            this.context = context;
        }

        public string GetValue (string dictionaryName, int dictionaryValueId)
        {
            var dictionaryValues = from dictionaryValue in context.DictionaryValue
                                   join dictionary in context.Dictionary on dictionaryValue.DICTIONARY_ID equals dictionary.ID
                                   where dictionary.NAME.Equals(dictionaryName) && dictionaryValue.DV_ID == dictionaryValueId
                                   select dictionaryValue;

            var selectedDictionaryValue = dictionaryValues.FirstOrDefault();            

            return selectedDictionaryValue.VALUE;    
        }

        public int GetId(string dictionaryName, string value)
        {
            var dictionaryValues = from dictionaryValue in context.DictionaryValue
                                   join dictionary in context.Dictionary on dictionaryValue.DICTIONARY_ID equals dictionary.ID
                                   where dictionary.NAME.Equals(dictionaryName) && dictionaryValue.VALUE == value
                                   select dictionaryValue;

            var selectedDictionaryValue = dictionaryValues.FirstOrDefault();

            return selectedDictionaryValue.DV_ID;    
        }

        public List<DictionaryValue> GetSemesterTypes()
        {
            var dictionaryValues = from dictionaryValue in context.DictionaryValue
                                   join dictionary in context.Dictionary on dictionaryValue.DICTIONARY_ID equals dictionary.ID
                                   where dictionary.NAME.Equals("Typy semestrów")
                                   select dictionaryValue;            

            return dictionaryValues.ToList();
        }
    }
}
