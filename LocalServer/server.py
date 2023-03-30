from flask import Flask
import os


device_ip = "192.168.137.183"
local_path = "C:\\Users\\Murat\\Desktop\\LocalServer\\"


app = Flask(__name__)

@app.route('/')

def handleRequest():
    os.system('adb -s {} exec-out screencap -p > {}screenshot.png'.format(device_ip, local_path));

    # Process the request . . .
    # ...
    detection_result = "laptop"
    # ...
    # Inside this block will be a local YOLOv5 run or a huggingface(etc..) run.

    return detection_result;
 

if __name__ == '__main__':
    os.system('adb connect {}:5555'.format(device_ip))
    app.run(host="192.168.137.1")