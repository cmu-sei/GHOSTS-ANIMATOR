#!/usr/bin/env python3
# -*- coding: utf-8 -*-

# import the MongoClient class
import io
from pymongo import MongoClient

# import the Pandas library
import pandas

# these libraries are optional
import json
import time

# build a new client instance of MongoClient
mongo_client = MongoClient('localhost', 27017)

# create new database and collection instance
db = mongo_client.AnimatorDb
col = db.NPCs

# start time of script
start_time = time.time()

# # make an API call to the MongoDB server
# cursor = col.find()

# # extract the list of documents from cursor obj
# mongo_docs = list(cursor)

# print("total docs:", len(mongo_docs))
# # restrict the number of docs to export
# # mongo_docs = mongo_docs[:10]  # slice the list
# print("selected docs:", len(mongo_docs))


def jsonify(x):
    return json.loads(str.replace(str.replace(str(x), "None", '""'), "'", '"'))


ct = 1
with open("npcs.csv", 'w') as f:

    h = [
        "Prefix",
        "First",
        "Middle",
        "Last",
        "Suffix",
        "Address1",
        "Address2",
        "City",
        "State",
        "PostalCode",
        "BiologicalSex",
        "Birthdate",
        "CellPhone",
        "HomePhone",
        "Email",
        "Supply_Needs",
        "Preferred_Meal",
        "Medical_Conditions",
        "Created"
    ]

    f.write(",".join(h))

    for doc in col.find().limit(10):

        doc_id = doc["_id"]

        name = jsonify(doc["Name"])
        address = jsonify(doc["Address"][0])
        attributes = jsonify(doc["Attributes"])
        health = jsonify(doc["Health"])

        print(health)

        d = [
            str(doc_id),
            str(name["Prefix"]),
            str(name['First']),
            str(name["Middle"]),
            str(name["Last"]),
            str(name["Suffix"]),
            str(address["Address1"]),
            str(address["Address2"]),
            str(address["City"]),
            str(address["State"]),
            str(address["PostalCode"]),
            str(doc["BiologicalSex"]),
            str(doc["Birthdate"]),
            str(doc["CellPhone"]),
            str(doc["HomePhone"]),
            str(doc["Email"]),
            str(attributes["Supply_Needs"]),
            str(health["PreferredMeal"]),
            str(health["MedicalConditions"]),
            str(doc["Created"])
        ]

        f.write(",".join(d))

        print(str(ct) + ":", time.time()-start_time)
        ct = ct + 1

exit(1)

ct = 1
with open('npcs_output2.csv', 'a') as f:
    # iterate over the list of MongoDB dict documents
    for num, doc in enumerate(mongo_docs):

        # convert ObjectId() to str
        doc["_id"] = str(doc["_id"])

        # get document _id from dict
        doc_id = doc["_id"]

        # create a Series obj from the MongoDB dict
        series_obj = pandas.Series(doc, name=doc_id)

        # create an empty DataFrame for storing documents
        docs = pandas.DataFrame(columns=[])
        # append the MongoDB Series obj to the DataFrame obj
        docs = docs.append(series_obj)

        # export MongoDB documents to a CSV file
        # docs.to_csv("object_rocket.csv", ",")  # CSV delimited by commas

        # export MongoDB documents to CSV
        if ct == 1:
            csv_export = docs.to_csv(sep=",")  # CSV delimited by commas
        else:
            csv_export = docs.to_csv(sep=",", header=False)
        # print("\nCSV data:", csv_export)
        f.write(str(csv_export))

        print(str(ct) + ":", time.time()-start_time)
        ct = ct + 1
