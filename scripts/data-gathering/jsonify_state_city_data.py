'''
Geographic information taken from unitedstateszipcodes.org
'''

import json

states_and_abbrevs_file = "states_and_abbrevs.csv"
city_information_file = "zip_code_database_states.txt"


states = {
    "Total Population": 0,
    "States": {}
}

with open(states_and_abbrevs_file) as file:
    for line in file:
        info = line.rstrip().split(",")
        state_dict = {"Name":info[0], "Population":0, "Abbreviation":info[1], "Cities":{} }
        states["States"][info[1]] = state_dict

#states = {"AL":
#   "Name": "Alabama",
#   "Abbreviation": "AL",
#   "Cities": {}
#}

# zip,type,decommissioned,primary_city,acceptable_cities,state,county,timezone,area_codes,world_region,country,latitude,longitude,irs_estimated_population_2015
#  0    1         2            3               4           5     6       7         8          9          10        11       12         13

city_count = 0
zip_count = 0

with open(city_information_file) as file:
    next(file)
    for line in file:
        info = line.rstrip().split('\t')

        state = info[5]
        population = int(info[13])

        if state in states["States"] and population > 0:
            zip = info[0]
            if len(zip) < 5:
                zip = "0" + zip
            city = info[3]
            county = info[6]
            timezone = info[7]
            state_cities = states["States"][state]["Cities"]
            if city in state_cities:
                state_zip_codes = state_cities[city]["Zip_codes"]
                if zip not in state_zip_codes:
                    states["States"][state]["Cities"][city]["Zip_codes"][zip]= {"Population":population}
                    states["States"][state]["Population"] += population
                    states["States"][state]["Cities"][city]["Population"] += population     
            else:
                city_dict = {
                    "County": county,
                    "Timezone": timezone,
                    "Population":population,
                    "Zip_codes":{
                        zip: {
                            "Population":population
                        }
                    }
                }
                states["States"][state]["Cities"][city] = city_dict
                states["States"][state]["Population"] += population
                city_count += 1
            zip_count += 1
            states["Total Population"] += population


print(f"Cities: {city_count}\nZip Codes: {zip_count}\nTotal Population: {states['Total Population']}")


'''
states = 
{
  "Total Population": 287561832,
  "States": {
    "AL": {
      "Name": "Alabama",
      "Population": 4161893,
      "Abbreviation": "AL",
      "Cities": {
        "Moody": {
          "County": "St. Clair County",
          "Timezone": "America/Chicago",
          "Population": 10390,
          "Zip_codes": {
            "35004": {
              "Population": 10390
            },
          }
        },
    },
}

TRANSLATE ABOVE TO BELOW
states = 
{
    "Total Population": 287561832,
    "States": [
        {
            "Name": "Alabama",
            "Population": 0, 
            "Abbreviation": "AL",
            "Cities": [
                 {
                    "Name": "Moody",
                    "County": "St. Clair County",
                    "Timezone": "America/Chicago",
                    "Population": 10390,
                    "Zip_codes": [
                        {
                            "ID":"35004",
                            "Population":10390
                        }
                    ]
                    }
                 ]
            
        }
    ]
}

This is how the json will be structured:
        PopulationData = {
            "Total Population": 0,
            "States": []
        }
        State = {
            "Name": "",
            "Population": 0, 
            "Abbreviation": "",
            "Cities": []      
        }
        City = {
            "Name": "",
            "County": "",
            "Timezone": "",
            "Population": 0,
            "Zip_codes": []
        }
        ZipCode = {
            "ID":"",
            "Population":0
        }
'''

jsonStates = {
    "TotalPopulation": states['Total Population'],
    "States": []
}

for state in states["States"]:
   
    jsonState = {
       "Name": states["States"][state]["Name"],
       "Population": states["States"][state]["Population"],
       "Abbreviation": states["States"][state]["Abbreviation"],
       "Cities": []
    }
    

    for city in states["States"][state]["Cities"]:
        jsonCity = {
            "Name": city,
            "County": states["States"][state]["Cities"][city]["County"],
            "Timezone": states["States"][state]["Cities"][city]["Timezone"],
            "Population": states["States"][state]["Cities"][city]["Population"],
            "ZipCodes": []
        }
        

        for zip in states["States"][state]["Cities"][city]["Zip_codes"]:
            jsonZip = {
                "Id": zip,
                "Population": states["States"][state]["Cities"][city]["Zip_codes"][zip]["Population"]
            }
            jsonCity["ZipCodes"].append(jsonZip)

        jsonState["Cities"].append(jsonCity)
    
    jsonStates["States"].append(jsonState)

with open('us_population_data.json', 'w') as fp:
    json.dump(jsonStates, fp, indent=2)
