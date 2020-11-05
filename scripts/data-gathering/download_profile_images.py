import requests
import os
import uuid

number_of_images_to_generate = 10
save_dir = "images"
url = "https://thispersondoesnotexist.com/image"

payload = {}
headers = {}

if not os.path.exists(save_dir):
    os.makedirs(save_dir)

print("working...")

for i in range(number_of_images_to_generate):
    with open(save_dir + '/' + str(uuid.uuid4()) + '.jpg', 'wb') as handle:
        response = requests.request("GET", url, headers=headers, data=payload)

        if not response.ok:
            print(response)

        for block in response.iter_content(1024):
            if not block:
                break

            handle.write(block)

print("finis!")
