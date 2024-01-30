## to use, download https://docs.mongodb.com/database-tools/mongoexport/ and place in /usr/local/bin for easy access

mongoexport --host localhost --port 27017 --db AnimatorDb --collection NPCs --type=csv --out NPCs.csv --fields Name.Prefix,Name.First,Name.Middle,Name.Last,Name.Suffix,Address1,City,State,PostalCode,Birthdate,CellPhone,HomePhone,Email,Attributes.Supply_Needs,Health.PreferredMeal,Health.MedicalConditions,Created




# GCD
mongoexport --host localhost --port 27017 --db AnimatorDb --collection NPCs --type=csv --out GCD.csv --fields _id,Name.Prefix,Name.First,Name.Middle,Name.Last,Name.Suffix,Address,Email,Password,HomePhone,CellPhone,Unit,Branch,Education,Employment,BiologicalSex,Birthdate,Health,Attributes,Relationships,Family,Finances,MentalHealth,ForeignTravel,Career,Workstation,InsiderThreat,Accounts,PGP,CAC,PhotoLink,Campaign,Enclave,Team,Created