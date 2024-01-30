#!/usr/bin/env python3
# -*- coding: utf-8 -*-

from pymongo import MongoClient
from faker import Faker
import time
import re
import json
import random

faker = Faker()


def jsonify(x):
    try:
        return json.loads(json.dumps(x))
    except:
        print(x)
        print(json.dumps(x))
        exit(1)


def easy_password():
    specials = ["@", "#"]
    c = random.choice(specials) * faker.random_int(1, 3)
    return faker.word() + str(faker.random_int(0, 99)) + faker.word() + c


# for _ in range(10):
#     print(easy_password())
# exit(0)

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

ct = 0
for doc in col.find():

    # doc_id = doc["_id"]
    # email = str(doc["Email"])
    # email = str.replace(email, ".mil@mail.mil",
    #                     re.search('@.+', faker.email()).group())
    # email = str.replace(email, ".civ@mail.mil",
    #                     re.search('@.+', faker.safe_email()).group())
    # email = str.replace(email, ".ctr@mail.mil",
    #                     re.search('@.+', faker.free_email()).group())
    # doc["Email"] = email

    # address = jsonify(doc["Address"][0])

    # doc["Address1"] = str(address["Address1"])
    # doc["Address2"] = str(address["Address2"])
    # doc["City"] = str(address["City"])
    # doc["State"] = str(address["State"])
    # doc["PostalCode"] = str(address["PostalCode"])

    # if ct < 177:
    #     doc["team"] = "HQ"
    # elif ct > 177 and ct < 354:
    #     doc["Team"] = "HR"
    # elif ct > 354 and ct < 531:
    #     doc["Team"] = "MP"
    # elif ct > 531 and ct < 708:
    #     doc["Team"] = "MED"
    # elif ct > 708 and ct < 885:
    #     doc["Team"] = "OPS"
    # elif ct > 885 and ct < 1062:
    #     doc["Team"] = "LOG"
    # else:
    #     doc["Team"] = "INTEL"

    doc["team"] = None

    #doc["Password"] = easy_password()

    col.save(doc)
    ct = ct + 1
    print(ct)
