# learnToolForTex

1) Use case 
    -upload files txt. pdf etc fiel extension 
    - search in your files and make cards or defi from all info that u got with smart search
    
     1) make so that we can upload also pdf- make it so u have tow modes as  to learn and to test your Test 
      2) read pdf an search for keywords there 
     
## TODO
    1)7ef570a47887 = add a submenu for the type of Data to export the result 
    2)1074ec7064de = make the result smaller and the name of the files 
    3)ef24ec7064de = select list of result to be exported 
    4)7ef570eb212f make  so we get everything also to exprot in a txt datei and the source of the file so w¿taht we can learn it later 
    5)4523f23fcd2 = make a Cars page , that gets the name of the search key and also the file , later user can write a card
    6) df2345sdaa = test and make a better fileServices to search rekursive and to get all the result to cards
    7) af223ce45f = make a link pointer to the adress for the ocucrence so that the user can Costumize what to take for the card . Example when the Lecutre or note does have more than the occurence info (linr / parragraph) in a following text. 
    8) af223ce49D = make a log files for the search local and optional for in cloud or more 
### TO START
- Dowload sdk 8.0 for Net  , in linux 
    ```apt install dotnet-sdk-8.0```
- Install razor in your prefer IDE
- Install the needit  nuget package = UglyToad.PdfPig , OpenXml.Packaging;EntytyFrameworkConxtex
in my linux was :
     ``` dotnet add package DocumentFormat.OpenXml 
        dotnet add package Pdfpig ,dotnet add package DocumentFormat.OpenXml 

## CONCETPS 
KARDS: // idea get  a record of cards 
        // idea for cards is gonna be a string as Question 
        // idea a list of string for answers :
        // for multiple chooise   some n amonth to be true 
        // for only  memory one as the answer 
        // for complete file make something 

         //public List <CardListType,List<Card>> CardsLists = new  List 
        // methods either use the export datei autmatisch , it takes the name of the keyowrd and
        // ask you about the key word slatch filename and the asnwer is everything inside 

## DATA STRUCTURES

    -Serachresult : 
     key : filename
     value: ocurrence line or paarapgraph , you sshould choose. 

     -Cards
     key: filename|Question
     Answers: Answer list 
     Cardtype : new, to study, studied, to remember
     Source: filename and could have more info about where the info comes from