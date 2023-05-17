from flask import Flask
import os
import time
from detect import detect

device_ip = "192.168.137.15"
local_path = "C:\\Users\\Murat\\Desktop\\github\\DesignIt\\ObjectDetection\\yolov7\\screenshot.png"

dictionary = {
    56: 'chair',
    57: 'couch',
    58: 'potted plant',
    59: 'bed',
    60: 'dining table',
    61: 'toilet',
    62: 'tv',
    63: 'laptop',
    68: 'microwave',
    69: 'oven',
    71: 'sink',
    72: 'refrigerator',
    74: 'clock',
    75: 'vase'
}

app = Flask(__name__)

@app.route('/')

def handleRequest():
    # time.sleep(2)
    # return 'laptop'
    img = local_path
    print(img)
    os.system('adb -s {} exec-out screencap -p > {}'.format(device_ip, img));
    pred = detect(img)[0]

    if len(pred) == 0:
        detection_result = 'unk'
    else:
        max_conf = 0
        max_conf_i = -1
        for i in range(len(pred)):
            if pred[i][4] > max_conf:
                max_conf = pred[i][4]
                max_conf_i = i
        
        detection_result = dictionary[int(pred[max_conf_i][5])]

    print(detection_result)
    return detection_result;
 

if __name__ == '__main__':
    os.system('adb tcpip 5555')
    os.system('adb connect {}:5555'.format(device_ip))
    app.run(host="192.168.137.1")