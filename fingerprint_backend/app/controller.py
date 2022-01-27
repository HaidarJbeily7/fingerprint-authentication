from .constants import Constants
from flask import Flask, request
from flask.wrappers import Response
from .matching_utils import is_matched
from werkzeug.utils import secure_filename
import random
import os
import base64

app = Flask(__name__)
STORAGE_PATH = Constants().STORAGE_PATH

@app.route('/register', methods = ['POST'])
def register():
    # try:
    #     print(base64.b64encode(request.form['fp'].encode('utf-8')))
    # except:
    #     pass
    with open("imageToSave.bmp", "wb+") as fh:
        fh.write(u' '.join(request.form['fp'].split(' ')).encode('utf-8'))
    # f = upload_file('fingerprints_images' ,request.form['fp'])
    status_code = 200
    return Response(), status_code
    upload_file('fingerprints_images' ,request.files['FILE']['fp'])


@app.route('/compare', methods = ['POST'])
def compare():
    # print(request.files['fp1'])
    fp1 = upload_file('fingerprints_images' ,request.files['fp1'])
    fp2 = upload_file('fingerprints_images' ,request.files['fp2'])
    res = is_matched(fp1, fp2)
    status_code = 200
    return Response(response=res, status=200)



def random_str(length=10):
    characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'
    randomString = ''
    for i in range(length):
        randomString += random.choice(characters)
    return randomString

def upload_file(folder_name , file):
    folder_path = os.path.join(STORAGE_PATH,folder_name)
    if not os.path.exists(folder_path):
        os.makedirs(folder_path)

    file_client_name = file.name
    file_client_extension = file.mimetype[file.mimetype.index('/')+1:]
    file_name = file_client_name+"_"+random_str(10) + '.' +file_client_extension.lower()
    
    if file:
        file.save(os.path.join(folder_path,file_name))
        return file_name
    return ''
    
