using DocumentFormat.OpenXml.Office.CustomUI;

namespace learnFromFiles.Services
{
    public class KartenServices
    {
        // idea get  a record of cards 
        // idea for cards is gonna be a string as Question 
        // idea a list of string for answers :
        // for multiple chooise   some n amonth to be true 
        // for only  memory one as the answer 
        // for complete file make something 
        public IDictionary<string, string> SearchResultTmp { get; private set; } = new Dictionary<string, string>();
        // methods either use the export datei autmatisch , it takes the name of the keyowrd and
        // ask you about the key word slatch filename and the asnwer is everything inside 
        
        /// <summary>
        /// use the result from the search that are passed to just make a card for you 
        /// it would make a search in all files and make the idccionary to be  key = researched key+filename 
        /// and the values is the content of the occurence
        /// option a ) searchfile inside here an instanz  or send the result for the last check and search to do it 
        /// </summary>
        /// 
        public void MakeCardAutomatic(string keyword)
        {
            IDictionary<string, string> KarteDictionary = new Dictionary<string, string>();
            foreach (var ocurrence in SearchResultTmp)
            {
                KarteDictionary.Add(ocurrence.Key, ocurrence.Value);
            }

        }
        //ask about how to use these  csv ocurrence , and make for each one a costum question or skip it 
        // need to use the last cards  get elemt from list 

        public void MakeCardCustom(string question = "", string answer = "")
        {
            IDictionary<string, string> PibotKard = new Dictionary<string, string>();
            foreach (var ocurrence in SearchResultTmp)
            {
                if (answer == "" && question != "")
                    PibotKard.Add(question, ocurrence.Value);
                else if (question == "" && answer != null)
                    PibotKard.Add(ocurrence.Key, answer);
                else if (question == "" && answer == "")
                    MakeCardAutomatic(ocurrence.Key);
                else
                    PibotKard.Add(question, answer);
            }
        }
        // karten haben verscheiedene Liste, learn , knew, to remeber, also improtant and not important 
         
    }
}